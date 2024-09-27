using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace IndustrialPark
{
    public static class AutomaticUpdater
    {
        public static DateTime LastCheckedForUpdate;
        public static bool UpdateIndustrialPark(out bool hasChecked, bool forceCheck = false)
        {
            hasChecked = false;
            string owner = "energydrink02";
            string repo = "IndustrialPark";

            try
            {
                if (forceCheck || LastCheckedForUpdate == DateTime.MinValue || DateTime.Now.Subtract(LastCheckedForUpdate).TotalMinutes >= 10)
                {
                    var client = new GitHubClient(new ProductHeaderValue("IP"));
                    Release newRelease = client.Repository.Release.GetLatest(owner, repo).GetAwaiter().GetResult();

                    LastCheckedForUpdate = DateTime.Now;

                    if (newRelease.TagName.Substring(1) == Application.ProductVersion)
                    {
                        hasChecked = true;
                        return false;
                    }

                    string messageText = $"There is an update available: Industrial Park ({newRelease.Name}).\n\n{newRelease.Body}\n\nDo you wish to download it?";
                    DialogResult d = MessageBox.Show(messageText, "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (d == DialogResult.Yes)
                    {
                        string updatedIPfilePath = Path.Combine(Application.StartupPath, "release.zip");
                        string oldIPdestinationPath = Path.Combine(Application.StartupPath, "IndustrialPark_old");

                        using (var webClient = new WebClient())
                        {
                            webClient.Headers.Add(HttpRequestHeader.UserAgent, "Anything");
                            webClient.DownloadFile(newRelease.Assets[0].BrowserDownloadUrl, updatedIPfilePath);
                        }

                        if (Directory.Exists(oldIPdestinationPath))
                            RecursiveDelete(oldIPdestinationPath, false);
                        else
                            Directory.CreateDirectory(oldIPdestinationPath);

                        List<string> directories = new()
                        {
                            "en-GB",
                            "lib",
                            "runtimes",
                            "Resources"
                        };

                        List<string> copyOnly = new()
                        {
                            "ip_settings.json",
                            "default_project.json",
                        };

                        foreach (string dir in Directory.GetDirectories(Application.StartupPath, "*", SearchOption.TopDirectoryOnly))
                        {
                            if (!directories.Contains(Path.GetRelativePath(Application.StartupPath, dir)))
                                continue;

                            CloneDirectory(dir, oldIPdestinationPath);
                        }

                        foreach (string file in Directory.GetFiles(Application.StartupPath, "*", SearchOption.TopDirectoryOnly))
                        {
                            if (copyOnly.Contains(Path.GetFileName(file).ToLower()))
                                File.Copy(file, Path.Combine(oldIPdestinationPath, Path.GetFileName(file)), true);
                            else if (Path.GetFileName(file).ToLower().Equals(Path.GetFileName(updatedIPfilePath)))
                                continue;
                            else
                                File.Move(file, Path.Combine(oldIPdestinationPath, Path.GetFileName(file)), true);
                        }

                        ZipFile.ExtractToDirectory(updatedIPfilePath, Application.StartupPath, true);

                        File.Delete(updatedIPfilePath);

                        return true;
                    }
                }
            }
            catch (ApiException apiex)
            {
                MessageBox.Show("There was an api error checking for updates: " + apiex.Message, "API Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error checking for updates: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return false;
        }

        private static void CloneDirectory(string src, string dest)
        {
            string relativeSrcPath = Path.GetRelativePath(Application.StartupPath, src);
            string newPath = Path.Combine(Application.StartupPath, dest, relativeSrcPath);

            Directory.CreateDirectory(newPath);
            foreach (var dir in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
            {
                CloneDirectory(dir, dest);
            }

            foreach (var file in Directory.GetFiles(src))
            {
                File.Copy(file, Path.Combine(newPath, Path.GetFileName(file)), true);
            }
        }

        private static void RecursiveDelete(string directory, bool deleteRoot = true)
        {
            if (!Directory.Exists(directory) || !directory.Contains(Application.StartupPath))
                return;

            foreach (string dir in Directory.GetDirectories(directory))
                Directory.Delete(dir, true);

            foreach (string file in Directory.GetFiles(directory))
                File.Delete(file);

            if (deleteRoot)
                Directory.Delete(directory);
        }

        public static bool VerifyEditorFiles()
        {
            bool mustUpdate = false;

            try
            {
                if (!Directory.Exists(ArchiveEditorFunctions.editorFilesFolder))
                {
                    mustUpdate = true;
                }
                else
                {
                    if (File.Exists(ArchiveEditorFunctions.editorFilesFolder + "version.json"))
                    {
                        string localVersion = JsonConvert.DeserializeObject<IPversion>(File.ReadAllText(ArchiveEditorFunctions.editorFilesFolder + "version.json")).version;

                        string versionInfoURL = "https://raw.githubusercontent.com/igorseabra4/IndustrialPark-EditorFiles/master/version.json";

                        string updatedJson;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(versionInfoURL);
                        request.AutomaticDecompression = DecompressionMethods.GZip;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                            updatedJson = reader.ReadToEnd();

                        IPversion updatedVersion = JsonConvert.DeserializeObject<IPversion>(updatedJson);

                        if (localVersion != updatedVersion.version)
                            mustUpdate = true;
                    }
                    else
                    {
                        mustUpdate = true;
                    }
                }

                if (mustUpdate)
                {
                    DialogResult dialogResult = MessageBox.Show("An update for IndustrialPark-EditorFiles has been found. Do you wish to download it now?", "Note", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (dialogResult == DialogResult.Yes)
                        DownloadEditorFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return mustUpdate;
        }

        public static void DownloadEditorFiles()
        {
            DownloadAndUnzip(
                "https://github.com/igorseabra4/IndustrialPark-EditorFiles/archive/master.zip",
                Path.Combine(Application.StartupPath, "Resources", "IndustrialPark-EditorFiles.zip"),
                Path.Combine(Application.StartupPath, "Resources", "IndustrialPark-EditorFiles"),
                "IndustrialPark-EditorFiles");
        }

        public static void DownloadAndUnzip(string zipUrl, string destZipPath, string destFolder, string downloadName)
        {
            try
            {
                MessageBox.Show("Will begin download of " + downloadName + " from GitHub to " + destFolder + ". Please wait as this might take a while. Any previously existing files in the folder will be overwritten.");

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri(zipUrl), destZipPath);

                RecursiveDelete(destFolder);

                ZipFile.ExtractToDirectory(destZipPath, destFolder);

                File.Delete(destZipPath);

                MessageBox.Show("Downloaded " + downloadName + " from " + zipUrl + " to " + destFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

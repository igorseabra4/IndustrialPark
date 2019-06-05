using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class AutomaticUpdater
    {
        public static bool UpdateIndustrialPark(out bool hasChecked)
        {
            hasChecked = false;

            try
            {
                string versionInfoURL = "https://raw.githubusercontent.com/igorseabra4/IndustrialPark/master/IndustrialPark/Resources/ip_version.json";

                string updatedJson;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(versionInfoURL);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    updatedJson = reader.ReadToEnd();

                IPversion updatedVersion = JsonConvert.DeserializeObject<IPversion>(updatedJson);
                IPversion oldVersion = new IPversion();

                hasChecked = true;

                if (oldVersion.version != updatedVersion.version)
                {
                    string messageText = "There is an update available for Industrial Park: " + updatedVersion.versionName + ". Do you wish to download it?";
                    DialogResult d = MessageBox.Show(messageText, "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (d == DialogResult.Yes)
                    {
                        string updatedIPfileName = "IndustrialPark_" + updatedVersion.version.Replace('p', 'P') + ".zip";
                        string updatedIPURL = "https://github.com/igorseabra4/IndustrialPark/releases/download/" + updatedVersion.version + "/" + updatedIPfileName;

                        string updatedIPfilePath = Application.StartupPath + "\\Resources\\" + updatedIPfileName;

                        using (var webClient = new WebClient())
                            webClient.DownloadFile(updatedIPURL, updatedIPfilePath);

                        string oldPath = Application.StartupPath + "\\IndustrialPark_old\\";

                        if (!Directory.Exists(oldPath))
                            Directory.CreateDirectory(oldPath);

                        foreach (string s in new string[]
                        {
                                "",
                                "\\Resources",
                                "\\Resources\\importvcolorobj",
                                "\\Resources\\Models",
                                "\\Resources\\SharpDX"
                        })
                        {
                            if (!Directory.Exists(oldPath + s))
                                Directory.CreateDirectory(oldPath + s);

                            foreach (string s2 in Directory.GetFiles(Application.StartupPath + s))
                            {
                                if (Path.GetExtension(s2).ToLower().Equals(".zip") || Path.GetExtension(s2).ToLower().Equals(".json"))
                                    continue;

                                string newFilePath = oldPath + s + "\\" + Path.GetFileName(s2);

                                if (File.Exists(newFilePath))
                                    File.Delete(newFilePath);

                                File.Move(s2, newFilePath);
                            }
                        }

                        ZipFile.ExtractToDirectory(updatedIPfilePath, Application.StartupPath);

                        File.Delete(updatedIPfilePath);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error checking for updates: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return false;
        }

        public static void DownloadEditorFiles()
        {
            try
            {
                string editorFilesZipURL = "https://github.com/igorseabra4/IndustrialPark-EditorFiles/archive/master.zip";
                string destZipPath = Application.StartupPath + "\\Resources\\IndustrialPark-EditorFiles.zip";
                string editorFilesFolder = Application.StartupPath + "\\Resources\\IndustrialPark-EditorFiles\\";

                MessageBox.Show("Will begin download of IndustrialPark-EditorFiles from GitHub to " + editorFilesFolder + ". Please wait as this might take a while. Any previously existing files in the folder will be overwritten.");

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri(editorFilesZipURL), destZipPath);

                RecursiveDelete(editorFilesFolder);

                ZipFile.ExtractToDirectory(destZipPath, editorFilesFolder);

                File.Delete(destZipPath);

                MessageBox.Show("Downloaded IndustrialPark-EditorFiles from https://github.com/igorseabra4/IndustrialPark-EditorFiles/ to " + editorFilesFolder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void RecursiveDelete(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var dir in Directory.GetDirectories(directory))
                RecursiveDelete(dir);

            foreach (var s in Directory.GetFiles(directory))
                File.Delete(s);

            Directory.Delete(directory);
        }
    }
}

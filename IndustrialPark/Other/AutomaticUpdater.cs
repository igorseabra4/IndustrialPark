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
                    string messageText = "There is an update available for Industrial Park:\n" + updatedVersion.versionName + "\nDo you wish to download it?";
                    DialogResult d = MessageBox.Show(messageText, "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (d == DialogResult.Yes)
                    {
                        string updatedIPfileName = "IndustrialPark_" + updatedVersion.version.Replace('p', 'P') + ".zip";
                        string updatedIPURL = "https://github.com/igorseabra4/IndustrialPark/releases/download/" + updatedVersion.version + "/" + updatedIPfileName;

                        string updatedIPfilePath = Application.StartupPath + "/Resources/" + updatedIPfileName;

                        using (var webClient = new WebClient())
                            webClient.DownloadFile(updatedIPURL, updatedIPfilePath);

                        string oldIPdestinationPath = Application.StartupPath + "/IndustrialPark_old/";

                        if (!Directory.Exists(oldIPdestinationPath))
                            Directory.CreateDirectory(oldIPdestinationPath);

                        string[] directoryNames = new string[]
                        {
                                "",
                                "/Resources",
                                "/Resources/Models",
                                "/Resources/Scripts",
                                "/Resources/SharpDX",
                                "/Resources/txdgen_1.0",
                                "/Resources/txdgen_1.0",
                                "/Resources/txdgen_1.0/LICENSES",
                                "/Resources/txdgen_1.0/LICENSES/eirrepo",
                                "/Resources/txdgen_1.0/LICENSES/libimagequant",
                                "/Resources/txdgen_1.0/LICENSES/libjpeg",
                                "/Resources/txdgen_1.0/LICENSES/libpng",
                                "/Resources/txdgen_1.0/LICENSES/libsquish",
                                "/Resources/txdgen_1.0/LICENSES/lzo-2.08",
                                "/Resources/txdgen_1.0/LICENSES/pvrtextool",
                                "/Resources/txdgen_1.0/LICENSES/rwtools",
                                "/runtimes/",
                                "/runtimes/linux-x64",
                                "/runtimes/linux-x64/native",
                                "/runtimes/osx-x64",
                                "/runtimes/osx-x64/native",
                                "/runtimes/win-x64",
                                "/runtimes/win-x64/native",
                                "/runtimes/win-x86",
                                "/runtimes/win-x86/native",
                        };

                        foreach (string localDirPath in directoryNames)
                            if (Directory.Exists(Application.StartupPath + localDirPath))
                            {
                                if (!Directory.Exists(oldIPdestinationPath + localDirPath))
                                    Directory.CreateDirectory(oldIPdestinationPath + localDirPath);

                                foreach (string previousFile in Directory.GetFiles(Application.StartupPath + localDirPath))
                                {
                                    if (Path.GetExtension(previousFile).ToLower().Equals(".zip") || (Path.GetExtension(previousFile).ToLower().Equals(".json") && Path.GetFileNameWithoutExtension(previousFile) != "default_project"))
                                        continue;

                                    string newFilePath = oldIPdestinationPath + localDirPath + "/" + Path.GetFileName(previousFile);

                                    if (File.Exists(newFilePath))
                                        File.Delete(newFilePath);

                                    File.Move(previousFile, newFilePath);
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
                string destZipPath = Application.StartupPath + "/Resources/IndustrialPark-EditorFiles.zip";
                string editorFilesFolder = Application.StartupPath + "/Resources/IndustrialPark-EditorFiles/";

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
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace IndustrialPark
{
    public enum UpdateStatus
    {
        Throttled,
        AlreadyUpToDate,
        Available,
        Downloaded,
        UserDeclined,
        DoesNotExist,
        Error,
    }

    public static class AutomaticUpdater
    {
        public static DateTime LastCheckedForUpdate;

        private static string AutomaticUpdaterPath => Path.Combine(Application.StartupPath, "Updater.exe");

        public static UpdateStatus CheckForUpdates(bool throttle)
        {
            if (throttle && LastCheckedForUpdate != DateTime.MinValue && DateTime.Now.Subtract(LastCheckedForUpdate).TotalHours < 21)
                return UpdateStatus.Throttled;

            if (!File.Exists(AutomaticUpdaterPath))
                return UpdateStatus.DoesNotExist;

            var updater = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = AutomaticUpdaterPath,
                Arguments = "\"" + Application.ExecutablePath + "\"",
                UseShellExecute = false,
                WorkingDirectory = Application.StartupPath,
            });

            if (updater == null)
                return UpdateStatus.Error;

            updater.WaitForExit();

            LastCheckedForUpdate = DateTime.Now;

            if (updater.ExitCode == 0)
                return UpdateStatus.Downloaded;
            
            CheckForUpdatesOnEditorFiles(false);

            if (updater.ExitCode == 1)
                return UpdateStatus.AlreadyUpToDate;

            if (updater.ExitCode == 2)
                return UpdateStatus.UserDeclined;
            
            return UpdateStatus.Error;
        }

        public static string editorFilesFolder => Path.Combine(Program.ResourcesFolder, "IndustrialPark-EditorFiles", "IndustrialPark-EditorFiles-master");
        
        private static string editorFilesVersionJson => Path.Combine(editorFilesFolder, "version.json");

        public static UpdateStatus CheckForUpdatesOnEditorFiles(bool downloadWhenNotFound)
        {
            var result = VerifyEditorFiles();
            if (downloadWhenNotFound && result == UpdateStatus.DoesNotExist)
                return DownloadIfAccepted("The IndustrialPark-EditorFiles folder has not been found under Resources. Do you wish to download it?", "Editor Files Not Found");
            if (result == UpdateStatus.Available)
                return DownloadIfAccepted("An update for IndustrialPark-EditorFiles has been found. Do you wish to download it now? Any previously existing files in the folder will be overwritten.", "Update Found");
            return result;
        }

        private static UpdateStatus DownloadIfAccepted(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes ?
            (DownloadEditorFiles() ? UpdateStatus.Downloaded : UpdateStatus.Error) : UpdateStatus.UserDeclined;

        private static UpdateStatus VerifyEditorFiles()
        {
            if (!Directory.Exists(editorFilesFolder) || !File.Exists(editorFilesVersionJson))
                return UpdateStatus.DoesNotExist;

            try
            {
                string localVersion = JsonConvert.DeserializeObject<IPversion>(File.ReadAllText(editorFilesVersionJson)).version;
                string versionInfoURL = "https://raw.githubusercontent.com/igorseabra4/IndustrialPark-EditorFiles/master/version.json";
                string updatedJson;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(versionInfoURL);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    updatedJson = reader.ReadToEnd();

                IPversion updatedVersion = JsonConvert.DeserializeObject<IPversion>(updatedJson);
                return (localVersion != updatedVersion.version) ? UpdateStatus.Available : UpdateStatus.AlreadyUpToDate;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to check for update:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return UpdateStatus.Error;
            }
        }

        public static bool DownloadEditorFiles()
        {
            return DownloadAndUnzip(
                "https://github.com/igorseabra4/IndustrialPark-EditorFiles/archive/master.zip",
                Path.Combine(Program.ResourcesFolder, "IndustrialPark-EditorFiles.zip"),
                Path.Combine(Program.ResourcesFolder, "IndustrialPark-EditorFiles"),
                "IndustrialPark-EditorFiles");
        }

        public static bool DownloadAndUnzip(string zipUrl, string destZipPath, string destFolder, string downloadName)
        {
            try
            {
                MessageBox.Show("Will begin download of " + downloadName + " from GitHub to " + destFolder + ". Please wait as this might take a while. Any previously existing files in the folder will be overwritten.");

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri(zipUrl), destZipPath);

                ZipFile.ExtractToDirectory(destZipPath, destFolder, overwriteFiles: true);

                MessageBox.Show("Downloaded " + downloadName + " from " + zipUrl + " to " + destFolder);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                if (File.Exists(destZipPath))
                    File.Delete(destZipPath);
            }
        }
    }
}

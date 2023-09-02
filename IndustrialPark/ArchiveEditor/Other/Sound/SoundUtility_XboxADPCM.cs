using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_XboxADPCM
    {
        private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

        public static byte[] ConvertSoundToXboxADPCM(string fileName)
        {
            try
            {
                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" \"{ffmpegOutPath}\"");

                if (!InitXboxADPCM())
                {
                    if (File.Exists(ffmpegOutPath))
                        File.Delete(ffmpegOutPath);
                    return null;
                }

                if (File.Exists(xboxadpcmOutPath))
                    File.Delete(xboxadpcmOutPath);

                ConvertXboxADPCM(ffmpegOutPath, xboxadpcmOutPath);

                var file = File.ReadAllBytes(xboxadpcmOutPath);

                File.Delete(ffmpegOutPath);
                File.Delete(xboxadpcmOutPath);

                return file;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static bool xboxadpcmInitialized = false;
        private static string xboxadpcmDir => Path.Combine(Application.StartupPath, "Resources", "XboxADPCM");
        private static string xboxadpcmPath => Path.Combine(xboxadpcmDir, "XboxADPCM.exe");
        private static string xboxadpcmOutPath => Path.Combine(xboxadpcmDir, "test_sound_out.wav");

        private static Process xboxadpcmProcess;

        private static bool InitXboxADPCM()
        {
            if (!xboxadpcmInitialized)
            {
                if (!File.Exists(xboxadpcmPath))
                {
                    var result = MessageBox.Show("XboxADPCM not found. Do you wish to download it?", "XboxADPCM not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                    {
                        if (!DownloadXboxADPCM())
                            return false;
                    }
                    else
                        return false;
                }

                xboxadpcmProcess = new Process();
                xboxadpcmProcess.StartInfo.FileName = xboxadpcmPath;
                xboxadpcmProcess.StartInfo.CreateNoWindow = true;
                xboxadpcmProcess.StartInfo.RedirectStandardOutput = true;
                xboxadpcmProcess.StartInfo.RedirectStandardError = true;
                xboxadpcmProcess.StartInfo.UseShellExecute = false;
                xboxadpcmProcess.EnableRaisingEvents = true;
                xboxadpcmInitialized = true;
            }

            return true;
        }

        private static void ConvertXboxADPCM(string inPath, string outPath)
        {
            xboxadpcmProcess.StartInfo.Arguments = $"\"{inPath}\" \"{outPath}\"";
            xboxadpcmProcess.Start();
            xboxadpcmProcess.WaitForExit();
        }

        private static bool DownloadXboxADPCM()
        {
            try
            {
                MessageBox.Show("Will begin download of XboxADPCM from GitHub. Please wait as this might take a while.");

                if (!Directory.Exists(xboxadpcmDir))
                    Directory.CreateDirectory(xboxadpcmDir);

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri("https://github.com/Sergeanur/XboxADPCM/releases/download/v1.2/XboxADPCM.exe"), xboxadpcmPath);

                MessageBox.Show("Downloaded XboxADPCM.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
    }
}

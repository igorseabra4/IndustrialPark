using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_PS2VAG
    {
        private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

        public static byte[] ConvertSoundToPS2VAG(string fileName)
        {
            try
            {
                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac 1 \"{ffmpegOutPath}\"");

                if (!InitPS2VAG())
                {
                    if (File.Exists(ffmpegOutPath))
                        File.Delete(ffmpegOutPath);
                    return null;
                }

                if (File.Exists(ps2vagOutPath))
                    File.Delete(ps2vagOutPath);

                ConvertPS2VAG(ffmpegOutPath, ps2vagOutPath);

                var file = File.ReadAllBytes(ps2vagOutPath);

                File.Delete(ffmpegOutPath);
                File.Delete(ps2vagOutPath);

                return file;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static bool ps2vagInitialized = false;
        private static string ps2vagPath => Path.Combine(Application.StartupPath, "Resources", "PS2VAGTool", "AIFF2VAG.exe");
        private static string ps2vagOutPath => Path.Combine(Application.StartupPath, "Resources", "PS2VAGTool", "test_sound_out.vag");

        private static Process ps2vagProcess;

        private static bool InitPS2VAG()
        {
            if (!ps2vagInitialized)
            {
                if (!File.Exists(ps2vagPath))
                {
                    var result = MessageBox.Show("PS2 VAG Tool not found. Do you wish to download it?", "PS2 VAG Tool not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        DownloadPS2VAGTool();
                    else
                        return false;
                }

                ps2vagProcess = new Process();
                ps2vagProcess.StartInfo.FileName = ps2vagPath;
                ps2vagProcess.StartInfo.CreateNoWindow = true;
                ps2vagProcess.StartInfo.RedirectStandardOutput = true;
                ps2vagProcess.StartInfo.RedirectStandardError = true;
                ps2vagProcess.StartInfo.UseShellExecute = false;
                ps2vagProcess.EnableRaisingEvents = true;
                ps2vagInitialized = true;
            }

            return true;
        }

        private static void ConvertPS2VAG(string inPath, string outPath)
        {
            ps2vagProcess.StartInfo.Arguments = $"\"{inPath}\" \"{outPath}\"";
            ps2vagProcess.Start();
            ps2vagProcess.WaitForExit();
        }

        private static void DownloadPS2VAGTool()
        {
            try
            {
                MessageBox.Show("Will begin download of PS2 VAG Tool from GitHub. Please wait as this might take a while.");

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri("https://github.com/eurotools/es-ps2-vag-tool/releases/download/3.0.0.0/AIFF2VAG.exe"), ps2vagPath);

                MessageBox.Show("Downloaded PS2 VAG Tool.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

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

        public static byte[] ConvertSoundToPS2VAG(string fileName, bool looping, int samplerate)
        {
            try
            {
                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ps2vagInPath))
                    File.Delete(ps2vagInPath);

                SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac 1 -ar {samplerate} \"{ps2vagInPath}\"");

                if (!InitPS2VAG())
                    return null;

                if (File.Exists(ps2vagOutPath))
                    File.Delete(ps2vagOutPath);

                ConvertPS2VAG(ps2vagInPath, looping);

                var file = File.ReadAllBytes(ps2vagOutPath);

                File.Delete(ps2vagInPath);
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
        private static string ps2vagDir => Path.Combine(Application.StartupPath, "Resources", "PS2VAGTool");
        private static string ps2vagPath => Path.Combine(ps2vagDir, "AIFF2VAG.exe");
        private static string ps2vagInPath => Path.Combine(ps2vagDir, "test_sound_out.wav");
        private static string ps2vagOutPath => Path.ChangeExtension(ps2vagInPath, "vag");

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

        private static void ConvertPS2VAG(string inPath, bool looping)
        {
            ps2vagProcess.StartInfo.Arguments = $"\"{inPath}\"" + (looping ? " -L" : " -1");
            ps2vagProcess.Start();
            ps2vagProcess.WaitForExit();
        }

        private static void DownloadPS2VAGTool()
        {
            try
            {
                MessageBox.Show("Will begin download of PS2 VAG Tool from GitHub. Please wait as this might take a while.");

                if (!Directory.Exists(ps2vagDir))
                    Directory.CreateDirectory(ps2vagDir);

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

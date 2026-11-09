using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_DSP
    {
        private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

        public static byte[] ConvertSoundToDSP(string fileName, int samplerate)
        {
            try
            {
                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac 1 -ar {samplerate} \"{ffmpegOutPath}\"");

                if (!InitVGAudio())
                {
                    if (File.Exists(ffmpegOutPath))
                        File.Delete(ffmpegOutPath);
                    return null;
                }

                if (File.Exists(vgaudioOutPath))
                    File.Delete(vgaudioOutPath);

                ConvertVGAudio(ffmpegOutPath, vgaudioOutPath);

                var file = File.ReadAllBytes(vgaudioOutPath);

                File.Delete(ffmpegOutPath);
                File.Delete(vgaudioOutPath);

                return file;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static bool vgaudioInitialized = false;
        private static string vgaudioDir => Path.Combine(Application.StartupPath, "Resources", "VGAudio");
        private static string vgaudioPath => Path.Combine(vgaudioDir, "VGAudioCli.exe");
        private static string vgaudioOutPath => Path.Combine(vgaudioDir, "test_sound_out.dsp");

        private static Process vgaudioProcess;

        private static bool InitVGAudio()
        {
            if (!vgaudioInitialized)
            {
                if (!File.Exists(vgaudioPath))
                {
                    var result = MessageBox.Show("VGAudio not found. Do you wish to download it?", "VGAudio not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        DownloadVGAudio();
                    else
                        return false;
                }

                vgaudioProcess = new Process();
                vgaudioProcess.StartInfo.FileName = vgaudioPath;
                vgaudioProcess.StartInfo.CreateNoWindow = true;
                vgaudioProcess.StartInfo.RedirectStandardOutput = true;
                vgaudioProcess.StartInfo.RedirectStandardError = true;
                vgaudioProcess.StartInfo.UseShellExecute = false;
                vgaudioProcess.EnableRaisingEvents = true;
                vgaudioInitialized = true;
            }

            return true;
        }

        private static void ConvertVGAudio(string inPath, string outPath)
        {
            vgaudioProcess.StartInfo.Arguments = $"\"{inPath}\" \"{outPath}\" -f gcadpcm";
            vgaudioProcess.Start();
            vgaudioProcess.WaitForExit();
        }

        private static void DownloadVGAudio()
        {
            try
            {
                MessageBox.Show("Will begin download of VGAudio from GitHub. Please wait as this might take a while.");

                if (!Directory.Exists(vgaudioDir))
                    Directory.CreateDirectory(vgaudioDir);

                using (var webClient = new WebClient())
                    webClient.DownloadFile(new Uri("https://github.com/Thealexbarney/VGAudio/releases/download/v2.2.1/VGAudioCli.exe"), vgaudioPath);

                MessageBox.Show("Downloaded VGAudio.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

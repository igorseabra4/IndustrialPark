using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_ffmpeg
    {
        private static bool ffmpegInitialized = false;
        private static string ffmpegFolder => Path.Combine(Application.StartupPath, "Resources", "ffmpeg");
        private static string ffmpegPath => Path.Combine(ffmpegFolder, "ffmpeg-2023-07-19-git-efa6cec759-essentials_build", "bin", "ffmpeg.exe");
        public static string ffmpegOutPath => Path.Combine(ffmpegFolder, "test_sound_out.wav");
        public static Process ffmpegProcess;

        public static bool InitFfmpeg()
        {
            if (!ffmpegInitialized)
            {
                if (!File.Exists(ffmpegPath))
                {
                    var result = MessageBox.Show("ffmpeg not found under /Resources/ffmpeg. Do you wish to download it?", "ffmpeg not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        DownloadFfmpeg();
                    else
                        return false;
                }

                ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = ffmpegPath;
                ffmpegProcess.StartInfo.CreateNoWindow = true;
                ffmpegProcess.StartInfo.WorkingDirectory = ffmpegFolder;
                ffmpegProcess.StartInfo.RedirectStandardOutput = false;
                ffmpegProcess.StartInfo.RedirectStandardError = false;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.EnableRaisingEvents = true;
                ffmpegInitialized = true;
            }

            return true;
        }

        public static void ConvertFfmpeg(string args)
        {
            ffmpegProcess.StartInfo.Arguments = args;
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
        }

        private static void DownloadFfmpeg()
        {
            AutomaticUpdater.DownloadAndUnzip(
                   "https://github.com/GyanD/codexffmpeg/releases/download/2023-07-19-git-efa6cec759/ffmpeg-2023-07-19-git-efa6cec759-essentials_build.zip",
                   Path.Combine(Application.StartupPath, "Resources", "ffmpeg-2023-07-19-git-efa6cec759-essentials_build.zip"),
                   Path.Combine(Application.StartupPath, "Resources", "ffmpeg"),
                   "ffmpeg");
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_FMOD
    {
        private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

        public static byte[] ConvertSoundToFSB3(string fileName, int frequency, bool forcemono, bool compress)
        {
            try
            {
                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac {(forcemono ? "1" : "2")} -ar {frequency} \"{ffmpegOutPath}\"");

                if (!InitFSBank())
                {
                    if (File.Exists(ffmpegOutPath))
                        File.Delete(ffmpegOutPath);
                    return null;
                }

                if (File.Exists(fsbankOutPath))
                    File.Delete(fsbankOutPath);

                if (File.Exists(fsbankOutPath2))
                    File.Delete(fsbankOutPath2);

                if (File.Exists(fsbankTextPath))
                    File.Delete(fsbankTextPath);

                File.WriteAllText(fsbankTextPath, ffmpegOutPath);

                ConvertFSBank(fsbankTextPath, fsbankOutPath, compress);

                var file = File.ReadAllBytes(fsbankOutPath);

                File.Delete(ffmpegOutPath);
                File.Delete(fsbankOutPath);
                File.Delete(fsbankTextPath);

                if (File.Exists(fsbankOutPath2))
                    File.Delete(fsbankOutPath2);

                return file;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static bool fsbankInitialized = false;
        private static string fsbankDir => Path.Combine(Application.StartupPath, "Resources", "FSBank");
        private static string fsbankPath => Path.Combine(fsbankDir, "fsbank.exe");
        private static string fsbankOutPath => Path.Combine(fsbankDir, "test_sound_out.fsb");
        private static string fsbankOutPath2 => Path.Combine(fsbankDir, "test_sound_out.h");
        private static string fsbankTextPath => Path.Combine(fsbankDir, "list.txt");

        private static Process fsbankProcess;

        private static bool InitFSBank()
        {
            if (!fsbankInitialized)
            {
                if (!File.Exists(fsbankPath))
                    ZipFile.ExtractToDirectory(Path.Combine(Application.StartupPath, "Resources", "FSBank.zip"), fsbankDir);

                fsbankProcess = new Process();
                fsbankProcess.StartInfo.FileName = fsbankPath;
                fsbankProcess.StartInfo.RedirectStandardOutput = true;
                fsbankProcess.StartInfo.RedirectStandardError = true;
                fsbankProcess.StartInfo.UseShellExecute = false;
                fsbankProcess.EnableRaisingEvents = true;
                fsbankInitialized = true;
            }

            return true;
        }

        private static void ConvertFSBank(string inPath, string outPath, bool compress)
        {
            fsbankProcess.StartInfo.Arguments = $"-o \"{outPath}\" \"{inPath}\" -p gc -f {(compress ? "gcadpcm" : "pcm")} -h";
            fsbankProcess.Start();
            fsbankProcess.WaitForExit();
        }
    }
}

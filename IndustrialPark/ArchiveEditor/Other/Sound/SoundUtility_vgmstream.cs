using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility_vgmstream
    {
        public static void DownloadVgmstream()
        {
            AutomaticUpdater.DownloadAndUnzip(
                "https://github.com/vgmstream/vgmstream/releases/download/r1866/vgmstream-win.zip",
                Path.Combine(Application.StartupPath, "Resources", "vgmstream-win.zip"),
                vgmsFolder,
                "vgmstream");

            ZipFile.ExtractToDirectory(
                Path.Combine(Application.StartupPath, "Resources", "vgmstream_ip_fix.zip"),
                vgmsFolder);
        }

        private static bool vgmsInitialized = false;
        private static string vgmsFolder => Path.Combine(Application.StartupPath, "Resources", "vgmstream");
        private static string vgmsPath => Path.Combine(vgmsFolder, "vgmstream-cli.exe");
        private static string vgmsInPath => Path.Combine(vgmsFolder, "test_sound_in");
        private static string vgmsOutPath => Path.Combine(vgmsFolder, "test_sound_out.wav");
        private static Process vgmsProcess;

        private static SoundPlayer player;
        private static uint playerSoundAssetId;

        public static void PlaySound(AssetSound asset, ArchiveEditorFunctions archive)
        {
            try
            {
                if (!InitVgmstream())
                    return;

                if (asset.assetID != playerSoundAssetId)
                {
                    playerSoundAssetId = asset.assetID;
                    var soundData = archive.GetSoundData(asset.assetID, asset.Data);
                    File.WriteAllBytes(vgmsInPath, soundData);

                    ConvertVgmstream(vgmsInPath, vgmsOutPath);

                    if (player != null)
                        player.Dispose();

                    player = new SoundPlayer(new MemoryStream(File.ReadAllBytes(vgmsOutPath)));

                    File.Delete(vgmsInPath);
                    File.Delete(vgmsOutPath);
                }

                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to play sound: " + ex);
            }
        }

        private static bool InitVgmstream()
        {
            if (!vgmsInitialized)
            {
                if (!File.Exists(vgmsPath))
                {
                    var result = MessageBox.Show("vgmstream not found under /Resources/vgmstream. Do you wish to download it?", "vgmstream not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        DownloadVgmstream();
                    else
                        return false;
                }

                vgmsProcess = new Process();
                vgmsProcess.StartInfo.FileName = vgmsPath;
                vgmsProcess.StartInfo.CreateNoWindow = true;
                vgmsProcess.StartInfo.WorkingDirectory = vgmsFolder;
                vgmsProcess.StartInfo.RedirectStandardOutput = true;
                vgmsProcess.StartInfo.RedirectStandardError = true;
                vgmsProcess.StartInfo.UseShellExecute = false;
                vgmsProcess.EnableRaisingEvents = true;
                vgmsInitialized = true;
            }

            return true;
        }

        public static void ConvertVgmstream(string inPath, string outPath, double loopCount = -1, double fadeTime = -1, double fadeDelay = -1, bool ignoreLooping = false)
        {
            List<string> args = new List<string>();

            if (loopCount >= 0)
                args.Add("-l " + loopCount);
            if (fadeTime >= 0)
                args.Add("-f " + fadeTime);
            if (fadeDelay >= 0)
                args.Add("-d " + fadeDelay);
            if (ignoreLooping)
                args.Add("-i");

            args.Add(string.Format("-o \"{0}\"", outPath));
            args.Add(string.Format("\"{0}\"", inPath));

            string argString = string.Join(" ", args.ToArray<string>());

            vgmsProcess.StartInfo.Arguments = argString;
            vgmsProcess.Start();
            vgmsProcess.WaitForExit();
        }

        public static void ClearSound()
        {
            playerSoundAssetId = 0;

            if (player != null)
                player.Dispose();
        }

        public static void StopSound()
        {
            if (player != null)
                player.Stop();
        }

        public static void Dispose()
        {
            if (player != null)
                player.Dispose();
        }

        private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

        public static byte[] GenerateJawData(byte[] bytes, float multiplier)
        {
            try
            {
                if (!InitVgmstream())
                    return null;

                File.WriteAllBytes(vgmsInPath, bytes);
                ConvertVgmstream(vgmsInPath, vgmsOutPath);

                if (!SoundUtility_ffmpeg.InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                ConvertFfmpegJaw(vgmsOutPath, ffmpegOutPath);

                var file = File.ReadAllBytes(ffmpegOutPath);

                File.Delete(vgmsInPath);
                File.Delete(vgmsOutPath);
                File.Delete(ffmpegOutPath);

                return ConvertToJaw(file.Skip(0x4E).ToArray(), multiplier);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static void ConvertFfmpegJaw(string inPath, string outPath)
        {
            SoundUtility_ffmpeg.ffmpegProcess.StartInfo.Arguments = $"-i \"{inPath}\" -ac 1 -ar 60 -sample_fmt s16 \"{outPath}\"";
            SoundUtility_ffmpeg.ffmpegProcess.Start();
            SoundUtility_ffmpeg.ffmpegProcess.WaitForExit();
        }

        private static byte[] ConvertToJaw(byte[] bytes, float multiplier)
        {
            var data = new short[bytes.Length / 2];
            short max = 0;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Math.Abs(BitConverter.ToInt16(bytes, i * 2));
                if (data[i] > max)
                    max = data[i];
            }

            var data2 = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                data2[i] = (byte)Math.Min(multiplier * data[i] / max, 255);

            return data2;
        }
    }
}

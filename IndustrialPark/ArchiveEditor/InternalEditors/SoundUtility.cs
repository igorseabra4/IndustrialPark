using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace IndustrialPark
{
    public static class SoundUtility
    {
        private static bool vgmsInitialized = false;
        private static string vgmsFolder => Path.Combine(Application.StartupPath, "Resources", "vgmstream");
        private static string vgmsPath => Path.Combine(vgmsFolder, "vgmstream-cli.exe");
        private static string vgmsInPath => vgmsFolder + "/test_sound_in";
        private static string vgmsOutPath => vgmsFolder + "/test_sound_out.wav";
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
                        AutomaticUpdater.DownloadVgmstream();
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

        private static bool ffmpegInitialized = false;
        private static string ffmpegFolder => Path.Combine(Application.StartupPath, "Resources", "ffmpeg");
        private static string ffmpegPath => Path.Combine(ffmpegFolder, "ffmpeg-2023-07-19-git-efa6cec759-essentials_build", "bin", "ffmpeg.exe");
        private static string ffmpegOutPath => Path.Combine(ffmpegFolder, "test_sound_out.wav");
        private static Process ffmpegProcess;

        public static byte[] ConvertSoundToDSP(string fileName)
        {
            try
            {
                if (!InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                ConvertFfmpeg(fileName, ffmpegOutPath);

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

        private static bool InitFfmpeg()
        {
            if (!ffmpegInitialized)
            {
                if (!File.Exists(ffmpegPath))
                {
                    var result = MessageBox.Show("ffmpeg not found under /Resources/ffmpeg. Do you wish to download it?", "ffmpeg not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        AutomaticUpdater.DownloadFfmpeg();
                    else
                        return false;
                }

                ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = ffmpegPath;
                ffmpegProcess.StartInfo.CreateNoWindow = true;
                ffmpegProcess.StartInfo.WorkingDirectory = ffmpegFolder;
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.StartInfo.RedirectStandardError = true;
                ffmpegProcess.StartInfo.UseShellExecute = false;
                ffmpegProcess.EnableRaisingEvents = true;
                ffmpegInitialized = true;
            }

            return true;
        }

        private static void ConvertFfmpeg(string inPath, string outPath)
        {
            ffmpegProcess.StartInfo.Arguments = $"-i \"{inPath}\" -ac 1 \"{outPath}\"";
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
        }

        private static bool vgaudioInitialized = false;
        private static string vgaudioPath => Path.Combine(Application.StartupPath, "Resources", "VGAudioCli.exe");
        private static string vgaudioOutPath => Path.Combine(ffmpegFolder, "test_sound_out.dsp");

        private static Process vgaudioProcess;

        private static bool InitVGAudio()
        {
            if (!vgaudioInitialized)
            {
                if (!File.Exists(vgaudioPath))
                {
                    var result = MessageBox.Show("VGAudioCli not found under /Resources. Do you wish to download it?", "VGAudioCli not found", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                    if (result == DialogResult.Yes)
                        AutomaticUpdater.DownloadVGAudio();
                    else
                        return false;
                }

                vgaudioProcess = new Process();
                vgaudioProcess.StartInfo.FileName = vgaudioPath;
                vgaudioProcess.StartInfo.CreateNoWindow = true;
                vgaudioProcess.StartInfo.WorkingDirectory = ffmpegFolder;
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

        public static byte[] GenerateJawData(byte[] bytes)
        {
            try
            {
                if (!InitVgmstream())
                    return null;

                File.WriteAllBytes(vgmsInPath, bytes);
                ConvertVgmstream(vgmsInPath, vgmsOutPath);

                if (!InitFfmpeg())
                    return null;

                if (File.Exists(ffmpegOutPath))
                    File.Delete(ffmpegOutPath);

                ConvertFfmpegJaw(vgmsOutPath, ffmpegOutPath);

                var file = File.ReadAllBytes(ffmpegOutPath);

                File.Delete(vgmsInPath);
                File.Delete(vgmsOutPath);
                File.Delete(ffmpegOutPath);

                return ConvertToJaw(file.Skip(0x4E).ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to convert sound: " + ex);
                return null;
            }
        }

        private static void ConvertFfmpegJaw(string inPath, string outPath)
        {
            ffmpegProcess.StartInfo.Arguments = $"-i \"{inPath}\" -ac 1 -ar 60 -sample_fmt s16 \"{outPath}\"";
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
        }

        private static byte[] ConvertToJaw(byte[] bytes)
        {
            var data = new short[bytes.Length / 2];
            short max = short.MinValue;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Math.Abs(BitConverter.ToInt16(bytes, i * 2));
                if (data[i] > max)
                    max = data[i];
            }

            var data2 = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                data2[i] = (byte)(240f * data[i] / max);

            return data2;
        }
    }
}

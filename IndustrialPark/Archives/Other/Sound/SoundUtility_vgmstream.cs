using IndustrialPark.Archives;
using IndustrialPark.Assets;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Media;
using System.Windows.Forms;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_vgmstream
{
    private static readonly object initializationLock = new();
    private static readonly object playerLock = new();

    private static string vgmsFolder => Path.Combine(Program.ResourcesFolder, "vgmstream");
    private static string vgmsPath => Path.Combine(vgmsFolder, "vgmstream-cli.exe");
    private static string vgmsInPath => Path.Combine(vgmsFolder, "test_sound_in");
    private static string vgmsOutPath => Path.Combine(vgmsFolder, "test_sound_out.wav");
    private static string ffmpegOutPath => SoundUtility_ffmpeg.ffmpegOutPath;

    private static SoundPlayer player;
    private static uint? playerSoundAssetId;
    private static MemoryStream playerStream;

    public static void DownloadVgmstream()
    {
        AutomaticUpdater.DownloadAndUnzip(
            "https://github.com/vgmstream/vgmstream/releases/download/r1866/vgmstream-win.zip",
            Path.Combine(Program.ResourcesFolder, "vgmstream-win.zip"),
            vgmsFolder,
            "vgmstream");

        string patchPath = Path.Combine(Program.ResourcesFolder, "vgmstream_ip_fix.zip");

        if (!File.Exists(patchPath))
            throw new FileNotFoundException("vgmstream patch archive not found.", patchPath);

        ZipFile.ExtractToDirectory(patchPath, vgmsFolder, overwriteFiles: true);

        if (!File.Exists(vgmsPath))
            throw new FileNotFoundException("vgmstream-cli.exe was not found after extraction.", vgmsPath);
    }

    private static bool InitVgmstream()
    {
        lock (initializationLock)
        {
            if (File.Exists(vgmsPath))
                return true;

            var result = MessageBox.Show(
                "vgmstream not found under /Resources/vgmstream. Do you wish to download it?",
                "vgmstream not found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result != DialogResult.Yes)
                return false;

            DownloadVgmstream();

            return File.Exists(vgmsPath);
        }
    }

    public static void ConvertVgmstream(
        string inPath,
        string outPath,
        double loopCount = -1,
        double fadeTime = -1,
        double fadeDelay = -1,
        bool ignoreLooping = false)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = vgmsPath,
            WorkingDirectory = vgmsFolder,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        if (loopCount >= 0)
        {
            process.StartInfo.ArgumentList.Add("-l");
            process.StartInfo.ArgumentList.Add(loopCount.ToString(
                System.Globalization.CultureInfo.InvariantCulture));
        }

        if (fadeTime >= 0)
        {
            process.StartInfo.ArgumentList.Add("-f");
            process.StartInfo.ArgumentList.Add(fadeTime.ToString(
                System.Globalization.CultureInfo.InvariantCulture));
        }

        if (fadeDelay >= 0)
        {
            process.StartInfo.ArgumentList.Add("-d");
            process.StartInfo.ArgumentList.Add(fadeDelay.ToString(
                System.Globalization.CultureInfo.InvariantCulture));
        }

        if (ignoreLooping)
            process.StartInfo.ArgumentList.Add("-i");

        process.StartInfo.ArgumentList.Add("-o");
        process.StartInfo.ArgumentList.Add(outPath);
        process.StartInfo.ArgumentList.Add(inPath);

        if (!process.Start())
            throw new Exception("Unable to start vgmstream.");

        process.WaitForExit();

        //if (process.ExitCode != 0)
        //    throw new Exception($"vgmstream exited with code {process.ExitCode}.");
    }

    public static void ExportToFile(AssetSound asset, Archive archive, string fileName)
    {
        try
        {
            if (!InitVgmstream())
                return;

            File.WriteAllBytes(vgmsInPath, archive.GetSoundData(asset.assetID, asset.Data));

            ConvertVgmstream(vgmsInPath, fileName);

            if (!File.Exists(fileName))
                throw new Exception("vgmstream did not produce the expected output file.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to export [{asset.assetID:X8}] {asset.assetName}: " + ex);
        }
        finally
        {
            if (File.Exists(vgmsInPath))
                File.Delete(vgmsInPath);
        }
    }

    public static void PlaySound(AssetSound asset, Archive archive)
    {
        try
        {
            if (!InitVgmstream())
                return;

            lock (playerLock)
            {
                if (asset.assetID != playerSoundAssetId)
                {
                    File.WriteAllBytes(
                        vgmsInPath,
                        archive.GetSoundData(asset.assetID, asset.Data));

                    try
                    {
                        ConvertVgmstream(vgmsInPath, vgmsOutPath);

                        if (!File.Exists(vgmsOutPath))
                            throw new Exception("vgmstream did not produce the expected WAV file.");

                        var stream = new MemoryStream(File.ReadAllBytes(vgmsOutPath));
                        var newPlayer = new SoundPlayer(stream);

                        DisposePlayer();

                        playerStream = stream;
                        player = newPlayer;
                        playerSoundAssetId = asset.assetID;
                    }
                    finally
                    {
                        if (File.Exists(vgmsInPath))
                            File.Delete(vgmsInPath);
                        if (File.Exists(vgmsOutPath))
                            File.Delete(vgmsOutPath);
                    }
                }

                player?.Play();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to play sound: " + ex);
        }
    }

    public static void ClearSound()
    {
        lock (playerLock)
        {
            DisposePlayer();
            playerSoundAssetId = null;
        }
    }

    public static void StopSound()
    {
        lock (playerLock)
        {
            player?.Stop();
        }
    }

    public static void Dispose()
    {
        lock (playerLock)
        {
            DisposePlayer();
            playerSoundAssetId = null;
        }
    }

    private static void DisposePlayer()
    {
        player?.Stop();
        player?.Dispose();
        player = null;

        playerStream?.Dispose();
        playerStream = null;
    }

    public static byte[]? GenerateJawData(byte[] bytes, float multiplier)
    {
        try
        {
            if (!InitVgmstream())
                return null;

            if (!SoundUtility_ffmpeg.InitFfmpeg())
                return null;

            File.WriteAllBytes(vgmsInPath, bytes);

            ConvertVgmstream(vgmsInPath, vgmsOutPath);

            if (!File.Exists(vgmsOutPath))
                throw new Exception("vgmstream did not produce the expected WAV file.");

            if (File.Exists(ffmpegOutPath))
                File.Delete(ffmpegOutPath);

            ConvertFfmpegJaw(vgmsOutPath, ffmpegOutPath);

            if (!File.Exists(ffmpegOutPath))
                throw new Exception("FFmpeg did not produce the expected PCM file.");

            byte[] file = File.ReadAllBytes(ffmpegOutPath);

            if (file.Length < 0x4E)
                throw new Exception("The generated WAV file is too small.");

            return ConvertToJaw(file.Skip(0x4E).ToArray(), multiplier);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to convert sound: " + ex);
            return null;
        }
        finally
        {
            if (File.Exists(vgmsInPath))
                File.Delete(vgmsInPath);
            if (File.Exists(vgmsOutPath))
                File.Delete(vgmsOutPath);
            if (File.Exists(ffmpegOutPath))
                File.Delete(ffmpegOutPath);
        }
    }

    private static void ConvertFfmpegJaw(string inPath, string outPath)
    {
        SoundUtility_ffmpeg.ConvertFfmpeg(
            $"-i \"{inPath}\" -ac 1 -ar 60 -sample_fmt s16 \"{outPath}\"");
    }

    private static byte[] ConvertToJaw(byte[] bytes, float multiplier)
    {
        if (bytes.Length % 2 != 0)
            throw new Exception("Invalid PCM data: expected 16-bit samples.");

        var data = new int[bytes.Length / 2];
        int max = 0;

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Math.Abs((int)BitConverter.ToInt16(bytes, i * 2));

            if (data[i] > max)
                max = data[i];
        }

        var result = new byte[data.Length];

        if (max == 0)
            return result;

        for (int i = 0; i < data.Length; i++)
        {
            float value = multiplier * data[i] / max;
            result[i] = (byte)Math.Clamp(value, 0, 255);
        }

        return result;
    }
}

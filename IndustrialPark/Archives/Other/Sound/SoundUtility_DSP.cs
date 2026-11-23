using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_DSP
{
    private static readonly HttpClient httpClient = new();
    private static readonly object initializationLock = new();

    private static string vgaudioDir => Path.Combine(Program.ResourcesFolder, "VGAudio");
    private static string vgaudioPath => Path.Combine(vgaudioDir, "VGAudioCli.exe");

    private static bool vgaudioInitialized = false;

    private static readonly string vgaudioDownloadUrl = "https://github.com/Thealexbarney/VGAudio/releases/download/v2.2.1/VGAudioCli.exe";

    public static byte[] ConvertSoundToDSP(string fileName, int samplerate)
    {
        string tempFolder = null;

        try
        {
            if (!SoundUtility_ffmpeg.InitFfmpeg())
                return null;

            if (!InitVGAudio())
                return null;

            // Create temporary files for this conversion.
            tempFolder = Path.Combine(Path.GetTempPath(), $"DSPConversion_{Guid.NewGuid():N}");

            Directory.CreateDirectory(tempFolder);

            string wavPath = Path.Combine(tempFolder, "input.wav");
            string dspPath = Path.Combine(tempFolder, "output.dsp");

            SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac 1 -ar {samplerate} \"{wavPath}\"");

            if (!File.Exists(wavPath))
                throw new InvalidOperationException("FFmpeg did not produce an output file.");

            ConvertVGAudio(wavPath, dspPath);

            if (!File.Exists(dspPath))
                throw new InvalidOperationException("VGAudio did not produce an output file.");

            return File.ReadAllBytes(dspPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Unable to convert sound: " + ex.Message,
                "DSP conversion failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return null;
        }
        finally
        {
            if (tempFolder != null)
               SoundUtility_ffmpeg.TryDeleteDirectory(tempFolder);
        }
    }

    #region VGAudio initialization

    private static bool InitVGAudio()
    {
        lock (initializationLock)
        {
            if (vgaudioInitialized && File.Exists(vgaudioPath))
                return true;

            if (!File.Exists(vgaudioPath))
            {
                var result = MessageBox.Show(
                    "VGAudio not found. Do you wish to download it?",
                    "VGAudio not found",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return false;

                if (!DownloadVGAudio())
                    return false;
            }

            vgaudioInitialized = File.Exists(vgaudioPath);

            return vgaudioInitialized;
        }
    }

    #endregion

    #region VGAudio conversion

    private static void ConvertVGAudio(string inPath, string outPath)
    {
        using var process = new Process();

        process.StartInfo.FileName = vgaudioPath;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = vgaudioDir;

        process.StartInfo.ArgumentList.Add(inPath);
        process.StartInfo.ArgumentList.Add(outPath);
        process.StartInfo.ArgumentList.Add("-f");
        process.StartInfo.ArgumentList.Add("gcadpcm");

        if (!process.Start())
            throw new InvalidOperationException(
                "Could not start VGAudio.");

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new InvalidOperationException(
                $"VGAudio exited with error code {process.ExitCode}.");
    }

    #endregion

    #region VGAudio download

    private static bool DownloadVGAudio()
    {
        string tempPath = vgaudioPath + ".download";

        try
        {
            Directory.CreateDirectory(vgaudioDir);

            MessageBox.Show(
                "Will begin download of VGAudio from GitHub. Please wait as this might take a while.",
                "Downloading VGAudio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            var data = httpClient.GetByteArrayAsync(vgaudioDownloadUrl)
                .GetAwaiter()
                .GetResult();

            File.WriteAllBytes(tempPath, data);

            if (!File.Exists(tempPath) || new FileInfo(tempPath).Length == 0)
                throw new InvalidDataException(
                    "The downloaded VGAudio executable is empty.");

            File.Move(tempPath, vgaudioPath, true);

            MessageBox.Show(
                "Downloaded VGAudio successfully.",
                "VGAudio",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Unable to download VGAudio: " + ex.Message,
                "VGAudio download failed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    #endregion
}


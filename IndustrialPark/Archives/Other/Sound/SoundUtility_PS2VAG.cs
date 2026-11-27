using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_PS2VAG
{
    private static readonly HttpClient httpClient = new();
    private static readonly object initializationLock = new();

    private static string ps2vagDir => Path.Combine(Program.ResourcesFolder, "PS2VAGTool");
    private static string ps2vagPath => Path.Combine(ps2vagDir, "AIFF2VAG.exe");

    private static readonly string ps2vagDownloadUrl = "https://github.com/eurotools/es-ps2-vag-tool/releases/latest/download/AIFF2VAG.exe";

    public static byte[] ConvertSoundToPS2VAG(string fileName, bool looping, int samplerate)
    {
        string tempDir = null;

        try
        {
            if (!SoundUtility_ffmpeg.InitFfmpeg())
                return null;

            if (!InitPS2VAG())
                return null;

            tempDir = Path.Combine(Path.GetTempPath(), $"PS2VAGConversion_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            string wavPath = Path.Combine(tempDir, "input.wav");
            string vagPath = Path.Combine(tempDir, "input.vag");

            SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac 1 -ar {samplerate} \"{wavPath}\"");

            if (!File.Exists(wavPath))
                throw new Exception("FFmpeg did not produce the expected WAV file.");

            ConvertPS2VAG(wavPath, looping);

            if (!File.Exists(vagPath))
                throw new Exception("PS2 VAG Tool did not produce the expected VAG file.");

            return File.ReadAllBytes(vagPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to convert sound: " + ex);
            return null;
        }
        finally
        {
            SoundUtility_ffmpeg.TryDeleteDirectory(tempDir);
        }
    }

    private static bool InitPS2VAG()
    {
        lock (initializationLock)
        {
            if (File.Exists(ps2vagPath))
                return true;

            var result = MessageBox.Show(
                "PS2 VAG Tool not found. Do you wish to download it?",
                "PS2 VAG Tool not found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result != DialogResult.Yes)
                return false;

            return DownloadPS2VAGTool();
        }
    }

    private static void ConvertPS2VAG(string inPath, bool looping)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = ps2vagPath,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        process.StartInfo.ArgumentList.Add(inPath);
        process.StartInfo.ArgumentList.Add(looping ? "-L" : "-1");

        if (!process.Start())
            throw new Exception("Unable to start PS2 VAG Tool.");

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"PS2 VAG Tool exited with code {process.ExitCode}.");
    }

    private static bool DownloadPS2VAGTool()
    {
        string tempPath = ps2vagPath + ".download";

        try
        {
            MessageBox.Show("Will begin download of PS2 VAG Tool from GitHub. Please wait as this might take a while.");

            Directory.CreateDirectory(ps2vagDir);

            byte[] data = httpClient.GetByteArrayAsync(ps2vagDownloadUrl)
                .GetAwaiter()
                .GetResult();

            if (data.Length == 0)
                throw new Exception("The downloaded PS2 VAG Tool file is empty.");

            File.WriteAllBytes(tempPath, data);
            File.Move(tempPath, ps2vagPath, true);

            MessageBox.Show("Downloaded PS2 VAG Tool.");

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to download PS2 VAG Tool: " + ex.Message);
            return false;
        }
        finally
        {
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch
            {
            }
        }
    }
}

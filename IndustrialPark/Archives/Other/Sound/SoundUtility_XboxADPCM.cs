using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_XboxADPCM
{
    private static readonly HttpClient httpClient = new();
    private static readonly object initializationLock = new();

    private static string xboxadpcmDir => Path.Combine(Program.ResourcesFolder, "XboxADPCM");
    private static string xboxadpcmPath => Path.Combine(xboxadpcmDir, "XboxADPCM.exe");

    private static readonly string xboxadpcmDownloadUrl = "https://github.com/Sergeanur/XboxADPCM/releases/latest/download/XboxADPCM.exe";

    public static byte[] ConvertSoundToXboxADPCM(string fileName, bool compress, bool forcemono, int samplerate)
    {
        string tempDir = null;

        try
        {
            if (!SoundUtility_ffmpeg.InitFfmpeg())
                return null;

            tempDir = Path.Combine(Path.GetTempPath(), $"XboxADPCMConversion_{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);

            string wavPath = Path.Combine(tempDir, "input.wav");

            SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -bitexact -map_metadata -1 -ac {(forcemono ? "1" : "2")} -ar {samplerate} \"{wavPath}\"");

            if (!File.Exists(wavPath))
                throw new Exception("FFmpeg did not produce the expected WAV file.");

            if (!compress)
                return File.ReadAllBytes(wavPath);

            if (!InitXboxADPCM())
                return null;

            string outputPath = Path.Combine(tempDir, "output.wav");

            ConvertXboxADPCM(wavPath, outputPath);

            if (!File.Exists(outputPath))
                throw new Exception("XboxADPCM did not produce the expected output file.");

            return File.ReadAllBytes(outputPath);
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

    private static bool InitXboxADPCM()
    {
        lock (initializationLock)
        {
            if (File.Exists(xboxadpcmPath))
                return true;

            var result = MessageBox.Show(
                "XboxADPCM not found. Do you wish to download it?",
                "XboxADPCM not found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);

            if (result != DialogResult.Yes)
                return false;

            return DownloadXboxADPCM();
        }
    }

    private static void ConvertXboxADPCM(string inPath, string outPath)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = xboxadpcmPath,
            CreateNoWindow = true,
            UseShellExecute = false
        };

        process.StartInfo.ArgumentList.Add(inPath);
        process.StartInfo.ArgumentList.Add(outPath);

        if (!process.Start())
            throw new Exception("Unable to start XboxADPCM.");

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception($"XboxADPCM exited with code {process.ExitCode}.");
    }

    private static bool DownloadXboxADPCM()
    {
        string tempPath = xboxadpcmPath + ".download";

        try
        {
            MessageBox.Show("Will begin download of XboxADPCM from GitHub. Please wait as this might take a while.");

            Directory.CreateDirectory(xboxadpcmDir);

            byte[] data = httpClient.GetByteArrayAsync(xboxadpcmDownloadUrl)
                .GetAwaiter()
                .GetResult();

            if (data.Length == 0)
                throw new Exception("The downloaded XboxADPCM file is empty.");

            File.WriteAllBytes(tempPath, data);
            File.Move(tempPath, xboxadpcmPath, true);

            MessageBox.Show("Downloaded XboxADPCM.");

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to download XboxADPCM: " + ex.Message);
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

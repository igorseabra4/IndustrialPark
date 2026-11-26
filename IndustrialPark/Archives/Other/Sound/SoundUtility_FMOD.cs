using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_FMOD
{
    private static readonly object initializationLock = new();

    private static string fsbankDir => Path.Combine(Program.ResourcesFolder, "FSBank");
    private static string fsbankPath => Path.Combine(fsbankDir, "fsbank.exe");

    private static bool fsbankInitialized = false;

    public static byte[] ConvertSoundToFSB3(string fileName, int frequency, bool forcemono, bool compress)
    {
        string? tempFolder = null;

        try
        {
            if (!SoundUtility_ffmpeg.InitFfmpeg())
                return null;

            if (!InitFSBank())
                return null;

            tempFolder = Path.Combine(Path.GetTempPath(), $"FSBConversion_{Guid.NewGuid():N}");

            Directory.CreateDirectory(tempFolder);

            string wavPath = Path.Combine(tempFolder, "input.wav");
            string fsbPath = Path.Combine(tempFolder, "output.fsb");
            string headerPath = Path.Combine(tempFolder, "output.h");
            string listPath = Path.Combine(tempFolder, "list.txt");

            // Decode and resample using FFmpeg.
            SoundUtility_ffmpeg.ConvertFfmpeg($"-i \"{fileName}\" -ac {(forcemono ? "1" : "2")} -ar {frequency} \"{wavPath}\"");

            if (!File.Exists(wavPath))
                throw new InvalidOperationException("FFmpeg did not produce an output file.");

            File.WriteAllText(listPath, wavPath);

            ConvertFSBank(listPath, fsbPath, compress);

            if (!File.Exists(fsbPath))
                throw new InvalidOperationException("FSBank did not produce an output file.");

            return File.ReadAllBytes(fsbPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Unable to convert sound: " + ex.Message,
                "FSB conversion failed",
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

    #region FSBank initialization

    private static bool InitFSBank()
    {
        lock (initializationLock)
        {
            if (fsbankInitialized && File.Exists(fsbankPath))
                return true;

            if (!File.Exists(fsbankPath))
            {
                string archivePath = Path.Combine(Program.ResourcesFolder, "FSBank.zip");

                if (!File.Exists(archivePath))
                    throw new FileNotFoundException("FSBank archive was not found.", archivePath);

                Directory.CreateDirectory(fsbankDir);

                ZipFile.ExtractToDirectory(archivePath, fsbankDir, overwriteFiles: true);
            }

            if (!File.Exists(fsbankPath))
                throw new InvalidDataException("FSBank.exe was not found after extracting the archive.");

            fsbankInitialized = true;

            return true;
        }
    }

    #endregion

    #region FSBank conversion

    private static void ConvertFSBank(string inPath, string outPath, bool compress)
    {
        using var process = new Process();

        process.StartInfo.FileName = fsbankPath;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = fsbankDir;

        process.StartInfo.ArgumentList.Add("-o");
        process.StartInfo.ArgumentList.Add(outPath);
        process.StartInfo.ArgumentList.Add(inPath);
        process.StartInfo.ArgumentList.Add("-p");
        process.StartInfo.ArgumentList.Add("gc");
        process.StartInfo.ArgumentList.Add("-f");
        process.StartInfo.ArgumentList.Add(compress ? "gcadpcm" : "pcm");
        process.StartInfo.ArgumentList.Add("-h");

        if (!process.Start())
            throw new InvalidOperationException("Could not start FSBank.");

        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new InvalidOperationException($"FSBank exited with error code {process.ExitCode}.");
    }

    #endregion
}

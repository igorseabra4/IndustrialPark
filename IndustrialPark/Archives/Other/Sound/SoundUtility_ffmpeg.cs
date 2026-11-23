using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace IndustrialPark.SoundUtility;

public static class SoundUtility_ffmpeg
{
    private static readonly HttpClient httpClient = new();
    private static readonly object processLock = new();

    private static bool ffmpegInitialized = false;

    private static string resourcesFolder => IndustrialPark.Program.ResourcesFolder;
    private static string ffmpegFolder => Path.Combine(resourcesFolder, "ffmpeg");
    private static string ffmpegPath => Path.Combine(ffmpegFolder, "bin", "ffmpeg.exe");
    private static string versionFile => Path.Combine(ffmpegFolder, "version.txt");

    private static string releaseVersionUrl => "https://www.gyan.dev/ffmpeg/builds/release-version";
    private static string ffmpegDownloadUrl(string version) => $"https://github.com/GyanD/codexffmpeg/releases/download/{version}/ffmpeg-{version}-essentials_build.zip";
    public static string ffmpegOutPath => Path.Combine(ffmpegFolder, "test_sound_out.wav");

    public static Process ffmpegProcess;

    #region Initialization

    public static bool InitFfmpeg()
    {
        if (ffmpegInitialized && File.Exists(ffmpegPath))
            return true;

        if (!IsInstalled())
        {
            var result = MessageBox.Show(
                "FFmpeg was not found. Do you wish to download it?",
                "FFmpeg not found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return false;

            if (!UpdateFfmpeg())
                return false;
        }

        ffmpegProcess?.Dispose();

        ffmpegProcess = new Process();

        ffmpegProcess.StartInfo.FileName = ffmpegPath;
        ffmpegProcess.StartInfo.CreateNoWindow = true;
        ffmpegProcess.StartInfo.WorkingDirectory = ffmpegFolder;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardOutput = false;
        ffmpegProcess.StartInfo.RedirectStandardError = false;

        ffmpegProcess.EnableRaisingEvents = true;

        ffmpegInitialized = true;

        return true;
    }

    public static bool IsInstalled() => File.Exists(ffmpegPath) && File.Exists(versionFile);

    #endregion

    #region Conversion

    public static void ConvertFfmpeg(string args)
    {
        lock (processLock)
        {
            if (!ffmpegInitialized)
                throw new InvalidOperationException("FFmpeg has not been initialized.");

            ffmpegProcess.StartInfo.Arguments = args;

            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();
        }
    }

    #endregion

    #region Version management

    public static string GetInstalledVersion() => File.Exists(versionFile) ? File.ReadAllText(versionFile).Trim() : null;

    public static string GetLatestVersion() => httpClient.GetStringAsync(releaseVersionUrl).GetAwaiter().GetResult().Trim();

    public static bool IsUpdateAvailable()
    {
        var installedVersion = GetInstalledVersion();

        if (installedVersion == null)
            return true;

        var latestVersion = GetLatestVersion();

        return !string.Equals(
            installedVersion,
            latestVersion,
            StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Installation / Updating

    public static bool UpdateFfmpeg()
    {
        lock (processLock)
        {
            string tempFolder = null;
            string stagingFolder = null;
            string backupFolder = null;

            try
            {
                var version = GetLatestVersion();

                tempFolder = Path.Combine(resourcesFolder, $"ffmpeg-temp-{Guid.NewGuid():N}");
                stagingFolder = Path.Combine(resourcesFolder, $"ffmpeg-staging-{Guid.NewGuid():N}");
                backupFolder = Path.Combine(resourcesFolder, $"ffmpeg-backup-{Guid.NewGuid():N}");

                Directory.CreateDirectory(tempFolder);
                Directory.CreateDirectory(stagingFolder);

                var zipPath = Path.Combine(tempFolder, "ffmpeg.zip");
                var extractedFolder = Path.Combine(tempFolder, "extracted");

                // Download archive.
                var archive = httpClient.GetByteArrayAsync(ffmpegDownloadUrl(version)).GetAwaiter().GetResult();

                File.WriteAllBytes(zipPath, archive);
                ZipFile.ExtractToDirectory(zipPath, extractedFolder);

                // Locate the extracted FFmpeg directory.
                var binPath = Directory.GetFiles(extractedFolder, "ffmpeg.exe", SearchOption.AllDirectories)
                    .Select(Path.GetDirectoryName)
                    .FirstOrDefault(path => path != null && Path.GetFileName(path).Equals("bin", StringComparison.OrdinalIgnoreCase))
                    ?? throw new InvalidDataException("The downloaded archive does not contain bin/ffmpeg.exe.");

                // Prepare installation directory.
                var stagingBin = Path.Combine(stagingFolder, "bin");

                Directory.Move(binPath, stagingBin);

                // Verify executable.
                var stagedExecutable = Path.Combine(stagingBin, "ffmpeg.exe");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = stagedExecutable;
                    process.StartInfo.Arguments = "-version";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;

                    if (!process.Start())
                        throw new InvalidOperationException("Could not start the downloaded FFmpeg executable.");

                    if (!process.WaitForExit(15000))
                    {
                        process.Kill();
                        throw new TimeoutException("FFmpeg verification timed out.");
                    }

                    if (process.ExitCode != 0)
                        throw new InvalidOperationException("The downloaded FFmpeg executable failed verification.");
                }

                File.WriteAllText(Path.Combine(stagingFolder, "version.txt"), version);

                // Replace existing installation.
                if (Directory.Exists(ffmpegFolder))
                {
                    ffmpegProcess?.Dispose();
                    ffmpegProcess = null;
                    ffmpegInitialized = false;

                    Directory.Move(ffmpegFolder, backupFolder);
                }

                try
                {
                    Directory.Move(stagingFolder, ffmpegFolder);
                }
                catch
                {
                    // Restore previous installation if replacement fails.
                    if (Directory.Exists(backupFolder) && !Directory.Exists(ffmpegFolder))
                        Directory.Move(backupFolder, ffmpegFolder);

                    throw;
                }

                // Remove previous installation.
                if (Directory.Exists(backupFolder))
                    Directory.Delete(backupFolder, true);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to download or install FFmpeg.\n\n{ex.Message}",
                    "FFmpeg installation failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
            finally
            {
                if (stagingFolder != null)
                    TryDeleteDirectory(stagingFolder);
                if (tempFolder != null)
                    TryDeleteDirectory(tempFolder);
            }
        }
    }

    public static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        catch
        {
        }
    }

    #endregion
}

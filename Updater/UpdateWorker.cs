using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace IndustrialPark.Updater;

/// <summary>Manages the temporary installer and its cleanup process.</summary>
internal static class UpdateWorker
{
    /// <summary>Removes a temporary worker directory after its process exits.</summary>
    internal static void Cleanup(string directoryPath, string processId)
    {
        string directory = Path.GetFullPath(directoryPath);
        string parent = Path.GetDirectoryName(directory);
        string name = Path.GetFileName(directory);
        if (!string.Equals(parent, Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase) ||
            !name.StartsWith("IndustrialPark_updater_", StringComparison.Ordinal) ||
            !Guid.TryParseExact(name["IndustrialPark_updater_".Length..], "N", out _))
            throw new InvalidDataException("Invalid temporary updater directory.");
        WaitForExit(int.Parse(processId));
        Directory.Delete(directory, true);
    }

    /// <summary>Waits for the application, installs the release and restarts it.</summary>
    internal static void Install(string appDir, string releasePath, string appProcessId, string parentProcessId,
        string targetExe, ref bool installed)
    {
        WaitForExit(int.Parse(parentProcessId));
        WaitForExit(int.Parse(appProcessId));
        var release = JsonSerializer.Deserialize<ReleaseInfo>(File.ReadAllText(releasePath))
            ?? throw new InvalidDataException("The selected release could not be read.");
        File.Delete(releasePath);
        new UpdateService(new GitHubReleaseProvider()).InstallUpdate(release, appDir);
        installed = true;
        LaunchApplication(targetExe);
    }

    /// <summary>Starts cleanup from the backup or restored installation after a worker exits.</summary>
    internal static void CleanupAfterInstall(string[] args, bool installed)
    {
        if (args.Length != 6 || args[0] != "--install" ||
            !Path.GetFileName(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)).StartsWith("IndustrialPark_updater_", StringComparison.Ordinal))
            return;

        try
        {
            string cleanupDir = installed ? Path.Combine(args[1], UpdateService.OldFolderName) : args[1];
            var cleanup = new ProcessStartInfo(Path.Combine(cleanupDir, "Updater.exe")) { UseShellExecute = false };
            cleanup.ArgumentList.Add("--cleanup");
            cleanup.ArgumentList.Add(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));
            cleanup.ArgumentList.Add(Environment.ProcessId.ToString());
            using Process process = Process.Start(cleanup) ?? throw new InvalidOperationException("The cleanup process could not be started.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Could not clean up temporary updater files: " + ex.Message, "Update Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>Copies the updater to a temporary directory and starts the installer.</summary>
    internal static void StartInstaller(string appDir, string targetExe, string releasePath, int appProcessId)
    {
        string workerDir = Path.Combine(Path.GetTempPath(), $"IndustrialPark_updater_{Guid.NewGuid():N}");
        bool started = false;
        try
        {
            Directory.CreateDirectory(workerDir);
            foreach (string name in new[] { "Updater.exe", "Updater.dll", "Updater.deps.json", "Updater.runtimeconfig.json", "IndustrialPark.dll" })
                File.Copy(Path.Combine(appDir, name), Path.Combine(workerDir, name));
            CopyDirectory(Path.Combine(appDir, "Library"), Path.Combine(workerDir, "Library"));

            var start = new ProcessStartInfo(Path.Combine(workerDir, "Updater.exe")) { UseShellExecute = false };
            start.ArgumentList.Add("--install");
            start.ArgumentList.Add(appDir);
            start.ArgumentList.Add(releasePath);
            start.ArgumentList.Add(appProcessId.ToString());
            start.ArgumentList.Add(Environment.ProcessId.ToString());
            start.ArgumentList.Add(targetExe);
            using Process process = Process.Start(start) ?? throw new InvalidOperationException("The update installer could not be started.");
            started = true;
        }
        finally
        {
            if (!started && Directory.Exists(workerDir))
                Directory.Delete(workerDir, true);
        }
    }

    /// <summary>Copies a directory and its contents for the temporary worker.</summary>
    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (string file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
        foreach (string directory in Directory.GetDirectories(source))
            CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }

    /// <summary>Waits for a process unless it has already exited.</summary>
    private static void WaitForExit(int processId)
    {
        try
        {
            using Process process = Process.GetProcessById(processId);
            process.WaitForExit();
        }
        catch (ArgumentException)
        {
            // The process has already exited.
        }
    }

    /// <summary>Starts the application after installation.</summary>
    private static void LaunchApplication(string targetExe) =>
        Process.Start(new ProcessStartInfo(targetExe) { UseShellExecute = true });
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace IndustrialPark.Updater;

public sealed class UpdateService
{
    public const string OldFolderName = "IndustrialPark_old";
    public const string UserFolderName = "User";
    public const string ResourcesFolderName = "Resources";

    private static readonly string[] BackupDirectories = ["Library", "runtimes"];
    private static readonly string[] BackupExtensions = [".exe", ".dll", ".pdb", ".json", ".config"];
    private static readonly string[] BackupResourceItems = ["SharpDX", "txdgen_1.0", "Models", "FSBank.zip", "JF_SB_lights", "jf01_light_kit", "lights", "vgmstream_ip_fix.zip"];

    private readonly IReleaseProvider provider;

    public UpdateService(IReleaseProvider provider)
    {
        this.provider = provider;
    }

    public ReleaseInfo GetAvailableRelease(string currentVersion) =>
        GetAvailableRelease(currentVersion, false, out _);

    public ReleaseInfo GetAvailableRelease(string currentVersion, bool includeSameOrOlder, out int comparison)
    {
        if (string.IsNullOrWhiteSpace(currentVersion))
            throw new InvalidDataException("The installed application has no version.");

        int suffix = currentVersion.IndexOfAny(['-', '+']);
        int legacyBeta = currentVersion.IndexOf(".beta", StringComparison.OrdinalIgnoreCase);
        if (legacyBeta >= 0 && (suffix < 0 || legacyBeta < suffix))
            suffix = legacyBeta;
        string numericVersion = suffix < 0 ? currentVersion : currentVersion[..suffix];
        if (!Version.TryParse(numericVersion, out Version installed))
            throw new InvalidDataException($"Invalid installed application version: {currentVersion}");

        ReleaseInfo latest = provider.GetLatestRelease();
        string tag = latest.TagName;
        if (string.IsNullOrWhiteSpace(tag) || tag[0] != 'v' ||
            !Version.TryParse(tag[1..], out Version available))
            throw new InvalidDataException($"Invalid release tag: {tag}");

        comparison = NormalizeVersion(available).CompareTo(NormalizeVersion(installed));
        if (!includeSameOrOlder && comparison <= 0)
            return null;

        return latest;
    }

    private static Version NormalizeVersion(Version version) =>
        new(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));

    public void InstallUpdate(ReleaseInfo release, string appDir)
    {
        string zipPath = Path.Combine(Path.GetTempPath(), $"IndustrialPark_update_{Guid.NewGuid():N}.zip");
        string oldDir = Path.Combine(appDir, OldFolderName);
        string workDir = Path.Combine(appDir, $".IndustrialPark_update_{Guid.NewGuid():N}");
        string stagingDir = Path.Combine(workDir, "staging");
        string backupDir = Path.Combine(workDir, "backup");
        string overwrittenDir = Path.Combine(workDir, "overwritten");
        string previousBackup = Path.Combine(workDir, "previous");
        var movedItems = new List<(string Source, string Backup, bool IsDirectory)>();
        var installedFiles = new List<(string Destination, string Original)>();
        var createdDirectories = new List<string>();
        bool previousMoved = false;
        bool backupPromoted = false;
        bool rollbackFailed = false;

        try
        {
            provider.DownloadRelease(release, zipPath);
            ZipFile.ExtractToDirectory(zipPath, stagingDir);
            if (!File.Exists(Path.Combine(stagingDir, "IndustrialPark.exe")))
                throw new InvalidDataException("The update does not contain IndustrialPark.exe.");

            try
            {
                BackupInstallation(appDir, backupDir, movedItems);
                InstallFiles(stagingDir, appDir, overwrittenDir, installedFiles, createdDirectories);

                if (Directory.Exists(oldDir))
                {
                    Directory.Move(oldDir, previousBackup);
                    previousMoved = true;
                }
                Directory.Move(backupDir, oldDir);
                backupPromoted = true;
            }
            catch (Exception installError)
            {
                var rollbackErrors = new List<Exception>();
                void TryRollback(Action action)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        rollbackErrors.Add(ex);
                    }
                }

                TryRollback(() =>
                {
                    if (backupPromoted)
                        Directory.Move(oldDir, backupDir);
                    if (previousMoved)
                        Directory.Move(previousBackup, oldDir);
                });

                for (int i = installedFiles.Count - 1; i >= 0; i--)
                {
                    var (destination, original) = installedFiles[i];
                    TryRollback(() =>
                    {
                        if (original != null)
                            File.Copy(original, destination, true);
                        else if (File.Exists(destination))
                            File.Delete(destination);
                    });
                }
                for (int i = createdDirectories.Count - 1; i >= 0; i--)
                {
                    string directory = createdDirectories[i];
                    TryRollback(() =>
                    {
                        if (Directory.Exists(directory))
                            Directory.Delete(directory);
                    });
                }
                for (int i = movedItems.Count - 1; i >= 0; i--)
                {
                    var (source, backup, isDirectory) = movedItems[i];
                    TryRollback(() =>
                    {
                        if (isDirectory)
                            Directory.Move(backup, source);
                        else
                            File.Move(backup, source);
                    });
                }
                if (rollbackErrors.Count != 0)
                {
                    rollbackFailed = true;
                    rollbackErrors.Insert(0, installError);
                    throw new AggregateException($"Update failed and rollback was incomplete. Recovery files are in {workDir}.", rollbackErrors);
                }
                throw;
            }
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
            // Keep recovery files if rollback could not finish.
            if (Directory.Exists(workDir) && !rollbackFailed)
                Directory.Delete(workDir, true);
        }
    }

    private static void BackupInstallation(string appDir, string oldDir, List<(string Source, string Backup, bool IsDirectory)> movedItems)
    {
        Directory.CreateDirectory(oldDir);

        foreach (string dirName in BackupDirectories)
            MoveItem(Path.Combine(appDir, dirName), Path.Combine(oldDir, dirName), movedItems);

        foreach (string file in Directory.GetFiles(appDir))
        {
            if (BackupExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                MoveItem(file, Path.Combine(oldDir, Path.GetFileName(file)), movedItems);
        }

        foreach (string item in BackupResourceItems)
            MoveItem(Path.Combine(appDir, ResourcesFolderName, item), Path.Combine(oldDir, ResourcesFolderName, item), movedItems);

        string userDir = Path.Combine(appDir, UserFolderName);
        if (Directory.Exists(userDir))
            CopyDirectory(userDir, Path.Combine(oldDir, UserFolderName));
    }

    private static void MoveItem(string source, string destination, List<(string Source, string Backup, bool IsDirectory)> movedItems)
    {
        string destinationParent = Path.GetDirectoryName(destination);
        Directory.CreateDirectory(destinationParent);

        if (Directory.Exists(source))
        {
            Directory.Move(source, destination);
            movedItems.Add((source, destination, true));
        }
        else if (File.Exists(source))
        {
            File.Move(source, destination);
            movedItems.Add((source, destination, false));
        }
    }

    private static void InstallFiles(string source, string destination, string overwrittenDir,
        List<(string Destination, string Original)> installedFiles, List<string> createdDirectories)
    {
        if (!Directory.Exists(destination))
        {
            Directory.CreateDirectory(destination);
            createdDirectories.Add(destination);
        }

        foreach (string file in Directory.GetFiles(source))
        {
            string target = Path.Combine(destination, Path.GetFileName(file));
            string original = null;
            if (File.Exists(target))
            {
                Directory.CreateDirectory(overwrittenDir);
                original = Path.Combine(overwrittenDir, Guid.NewGuid().ToString("N"));
                File.Copy(target, original);
            }
            installedFiles.Add((target, original));
            File.Copy(file, target, true);
        }

        foreach (string dir in Directory.GetDirectories(source))
        {
            string name = Path.GetFileName(dir);
            if (name.Equals(UserFolderName, StringComparison.OrdinalIgnoreCase))
                continue;
            InstallFiles(dir, Path.Combine(destination, name), overwrittenDir, installedFiles, createdDirectories);
        }
    }

    private static void CopyDirectory(string source, string destination, bool skipUserFolder = false)
    {
        Directory.CreateDirectory(destination);

        foreach (string file in Directory.GetFiles(source))
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);

        foreach (string dir in Directory.GetDirectories(source))
        {
            string name = Path.GetFileName(dir);

            if (skipUserFolder && name.Equals(UserFolderName, StringComparison.OrdinalIgnoreCase))
                continue;

            CopyDirectory(dir, Path.Combine(destination, name));
        }
    }
}

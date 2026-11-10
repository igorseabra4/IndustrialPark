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

    public ReleaseInfo GetAvailableRelease(string currentVersion)
    {
        if (string.IsNullOrEmpty(currentVersion) || currentVersion.Contains("beta"))
            return null;

        ReleaseInfo latest = provider.GetLatestRelease();

        if (latest.TagName[1..] == currentVersion)
            return null;

        return latest;
    }

    public void InstallUpdate(ReleaseInfo release, string appDir)
    {
        string zipPath = Path.Combine(Path.GetTempPath(), $"IndustrialPark_update_{Guid.NewGuid():N}.zip");
        string oldDir = Path.Combine(appDir, OldFolderName);

        try
        {
            provider.DownloadRelease(release, zipPath);

            BackupInstallation(appDir, oldDir);

            string stagingDir = Path.Combine(Path.GetTempPath(), $"IndustrialPark_staging_{Guid.NewGuid():N}");
            try
            {
                ZipFile.ExtractToDirectory(zipPath, stagingDir);
                CopyDirectory(stagingDir, appDir, skipUserFolder: true);
            }
            finally
            {
                if (Directory.Exists(stagingDir))
                    Directory.Delete(stagingDir, true);
            }
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    public static void BackupInstallation(string appDir, string oldDir)
    {
        if (Directory.Exists(oldDir))
            Directory.Delete(oldDir, true);

        Directory.CreateDirectory(oldDir);

        foreach (string dirName in BackupDirectories)
            MoveItem(Path.Combine(appDir, dirName), Path.Combine(oldDir, dirName));

        foreach (string file in Directory.GetFiles(appDir))
        {
            if (BackupExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                MoveItem(file, Path.Combine(oldDir, Path.GetFileName(file)));
        }

        foreach (string item in BackupResourceItems)
            MoveItem(Path.Combine(appDir, ResourcesFolderName, item), Path.Combine(oldDir, ResourcesFolderName, item));

        string userDir = Path.Combine(appDir, UserFolderName);
        if (Directory.Exists(userDir))
            CopyDirectory(userDir, Path.Combine(oldDir, UserFolderName));
    }

    private static void MoveItem(string source, string destination)
    {
        string destinationParent = Path.GetDirectoryName(destination);
        Directory.CreateDirectory(destinationParent);

        if (Directory.Exists(source))
            Directory.Move(source, destination);
        else if (File.Exists(source))
            File.Move(source, destination, true);
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

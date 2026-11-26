using IndustrialPark.Updater;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Xunit;

namespace IndustrialParkTest;

public class UpdaterTests : IDisposable
{
    private readonly string root;
    private readonly string appDir;

    public UpdaterTests()
    {
        root = Path.Combine(Path.GetTempPath(), "IndustrialParkUpdaterTests_" + Guid.NewGuid().ToString("N"));
        appDir = Path.Combine(root, "IndustrialPark");
        Directory.CreateDirectory(appDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(root))
            Directory.Delete(root, true);
    }

    private static void WriteFile(string path, string content)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, content);
    }

    private string CreateDummyUpdateZip(Dictionary<string, string> files)
    {
        string zipPath = Path.Combine(root, "update.zip");
        using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            foreach (var pair in files)
            {
                var entry = archive.CreateEntry(pair.Key);
                using var writer = new StreamWriter(entry.Open());
                writer.Write(pair.Value);
            }
        }
        return zipPath;
    }

    private sealed class FakeReleaseProvider : IReleaseProvider
    {
        private readonly ReleaseInfo release;
        private readonly string zipPath;

        public int DownloadCount { get; private set; }
        public int LookupCount { get; private set; }

        public FakeReleaseProvider(ReleaseInfo release, string zipPath)
        {
            this.release = release;
            this.zipPath = zipPath;
        }

        public ReleaseInfo GetLatestRelease()
        {
            LookupCount++;
            return release;
        }

        public void DownloadRelease(ReleaseInfo release, string destinationZipPath)
        {
            DownloadCount++;
            File.Copy(zipPath, destinationZipPath, true);
        }
    }

    [Fact]
    public void UpdateFoundDownloadedAndInstalled()
    {
        WriteFile(Path.Combine(appDir, "IndustrialPark.exe"), "old exe");
        WriteFile(Path.Combine(appDir, "IndustrialPark.dll"), "old dll");
        WriteFile(Path.Combine(appDir, "User", "settings.txt"), "user data");

        string zip = CreateDummyUpdateZip(new Dictionary<string, string>
        {
            ["IndustrialPark.exe"] = "new exe",
            ["IndustrialPark.dll"] = "new dll",
        });

        var release = new ReleaseInfo { TagName = "v2.0.0", Name = "2.0.0", Body = "", DownloadUrl = "dummy" };
        var provider = new FakeReleaseProvider(release, zip);
        var service = new UpdateService(provider);

        ReleaseInfo available = service.GetAvailableRelease("1.0.0");
        Assert.NotNull(available);
        Assert.Equal("v2.0.0", available.TagName);

        service.InstallUpdate(available, appDir);

        Assert.Equal(1, provider.DownloadCount);
        Assert.Equal("new exe", File.ReadAllText(Path.Combine(appDir, "IndustrialPark.exe")));
        Assert.Equal("new dll", File.ReadAllText(Path.Combine(appDir, "IndustrialPark.dll")));
        Assert.Equal("old exe", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "IndustrialPark.exe")));
        Assert.Equal("old dll", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "IndustrialPark.dll")));
        Assert.Equal("user data", File.ReadAllText(Path.Combine(appDir, "User", "settings.txt")));
        Assert.Equal("user data", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "User", "settings.txt")));
    }

    [Fact]
    public void UpdateNotAvailableWhenVersionMatches()
    {
        var release = new ReleaseInfo { TagName = "v1.0.0", Name = "1.0.0", Body = "", DownloadUrl = "dummy" };
        var provider = new FakeReleaseProvider(release, null);
        var service = new UpdateService(provider);

        Assert.Null(service.GetAvailableRelease("1.0.0"));
        Assert.Equal(0, provider.DownloadCount);
    }

    [Theory]
    [InlineData("2026.11.25", "v2026.11.25.0", 0)]
    [InlineData("2026.11.25.1", "v2026.11.25", -1)]
    [InlineData("2026.11.25", "v2026.11.25.1", 1)]
    [InlineData("2026.12.01", "v2026.11.30", -1)]
    [InlineData("2026.11.25", "v2026.12.01", 1)]
    [InlineData("2026.11.25-beta", "v2026.11.25", 0)]
    [InlineData("2026.11.25.1-beta", "v2026.11.25", -1)]
    [InlineData("2026.11.25-beta", "v2026.11.25.1", 1)]
    [InlineData("2026.11.25.BETA.7", "v2026.11.26", 1)]
    [InlineData("2026.11.25-rc.1+build", "v2026.11.26", 1)]
    public void InAppChecksOnlyOfferNewerReleasesButStandaloneOffersAll(string installed, string tag, int expected)
    {
        var release = new ReleaseInfo { TagName = tag };
        var provider = new FakeReleaseProvider(release, null);
        var service = new UpdateService(provider);

        Assert.Equal(expected > 0 ? release : null, service.GetAvailableRelease(installed));
        Assert.Same(release, service.GetAvailableRelease(installed, true, out int comparison));
        Assert.Equal(expected, Math.Sign(comparison));
        Assert.Equal(2, provider.LookupCount);
    }

    [Fact]
    public void InvalidReleaseTagIsReported()
    {
        var service = new UpdateService(new FakeReleaseProvider(new ReleaseInfo { TagName = "latest" }, null));
        Assert.Throws<InvalidDataException>(() => service.GetAvailableRelease("2026.11.25"));
    }
    [Fact]
    public void InvalidUpdateDoesNotMoveInstallationOrDeletePreviousBackup()
    {
        WriteFile(Path.Combine(appDir, "IndustrialPark.exe"), "old exe");
        WriteFile(Path.Combine(appDir, "IndustrialPark_old", "previous.txt"), "previous backup");
        string zip = Path.Combine(root, "invalid.zip");
        File.WriteAllText(zip, "not a zip");
        var release = new ReleaseInfo();
        var service = new UpdateService(new FakeReleaseProvider(release, zip));

        Assert.Throws<InvalidDataException>(() => service.InstallUpdate(release, appDir));
        Assert.Equal("old exe", File.ReadAllText(Path.Combine(appDir, "IndustrialPark.exe")));
        Assert.Equal("previous backup", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "previous.txt")));
    }

    [Fact]
    public void FailedInstallRestoresMovedFilesAndPreviousBackup()
    {
        WriteFile(Path.Combine(appDir, "IndustrialPark.exe"), "old exe");
        WriteFile(Path.Combine(appDir, "Library", "old.dll"), "old library");
        WriteFile(Path.Combine(appDir, "IndustrialPark_old", "previous.txt"), "previous backup");
        WriteFile(Path.Combine(appDir, "notes.txt"), "existing notes");
        WriteFile(Path.Combine(appDir, "blocked"), "existing file");
        string zip = CreateDummyUpdateZip(new Dictionary<string, string>
        {
            ["IndustrialPark.exe"] = "new exe",
            ["notes.txt"] = "new notes",
            ["Library/new.dll"] = "new library",
            ["blocked/file.txt"] = "cannot create directory",
        });
        var release = new ReleaseInfo();
        var service = new UpdateService(new FakeReleaseProvider(release, zip));

        Assert.ThrowsAny<IOException>(() => service.InstallUpdate(release, appDir));
        Assert.Equal("old exe", File.ReadAllText(Path.Combine(appDir, "IndustrialPark.exe")));
        Assert.Equal("old library", File.ReadAllText(Path.Combine(appDir, "Library", "old.dll")));
        Assert.Equal("existing notes", File.ReadAllText(Path.Combine(appDir, "notes.txt")));
        Assert.Equal("existing file", File.ReadAllText(Path.Combine(appDir, "blocked")));
        Assert.False(File.Exists(Path.Combine(appDir, "Library", "new.dll")));
        Assert.Equal("previous backup", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "previous.txt")));
    }

    [Fact]
    public void FailedBackupRestoresItemsMovedBeforeFailure()
    {
        string exe = Path.Combine(appDir, "IndustrialPark.exe");
        WriteFile(exe, "old exe");
        WriteFile(Path.Combine(appDir, "Library", "old.dll"), "old library");
        WriteFile(Path.Combine(appDir, "IndustrialPark_old", "previous.txt"), "previous backup");
        string zip = CreateDummyUpdateZip(new Dictionary<string, string> { ["IndustrialPark.exe"] = "new exe" });
        var release = new ReleaseInfo();
        var service = new UpdateService(new FakeReleaseProvider(release, zip));

        using (File.Open(exe, FileMode.Open, FileAccess.Read, FileShare.None))
            Assert.ThrowsAny<IOException>(() => service.InstallUpdate(release, appDir));

        Assert.Equal("old exe", File.ReadAllText(exe));
        Assert.Equal("old library", File.ReadAllText(Path.Combine(appDir, "Library", "old.dll")));
        Assert.Equal("previous backup", File.ReadAllText(Path.Combine(appDir, "IndustrialPark_old", "previous.txt")));
    }
}

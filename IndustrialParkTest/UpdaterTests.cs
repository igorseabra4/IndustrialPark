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

        public FakeReleaseProvider(ReleaseInfo release, string zipPath)
        {
            this.release = release;
            this.zipPath = zipPath;
        }

        public ReleaseInfo GetLatestRelease() => release;

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

    [Fact]
    public void UpdateNotAvailableForBetaBuild()
    {
        var release = new ReleaseInfo { TagName = "v2.0.0", Name = "2.0.0", Body = "", DownloadUrl = "dummy" };
        var service = new UpdateService(new FakeReleaseProvider(release, null));

        Assert.Null(service.GetAvailableRelease("1.0.0-beta"));
    }
}

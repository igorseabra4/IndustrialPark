using HipHopFile;
using IndustrialPark;
using IndustrialPark.Archives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace IndustrialParkTest;

public class ArchiveSaveTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), "IndustrialParkArchiveTests_" + Guid.NewGuid().ToString("N"));

    public ArchiveSaveTests()
    {
        Directory.CreateDirectory(directory);
    }

    public void Dispose()
    {
        Directory.Delete(directory, true);
    }

    private static Archive CreateArchive() =>
        new Archive(Platform.GameCube, Game.BFBB, DateTime.Now, "test", false, false, false);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveNewFileDoesNotCreateBackup(bool createBackup)
    {
        string path = Path.Combine(directory, "new.hip");
        var archive = CreateArchive();

        archive.Save(path, createBackup);

        Assert.True(File.Exists(path));
        Assert.False(File.Exists(path + ".bak"));
        Assert.Empty(Directory.GetFiles(directory, "*.tmp"));
    }

    [Fact]
    public void SaveKeepsPreviousFileAndUpdatesBackupOnSubsequentSave()
    {
        string path = Path.Combine(directory, "test.hip");
        File.WriteAllText(path, "original");
        var archive = CreateArchive();

        archive.Save(path, true);
        Assert.Equal("original", File.ReadAllText(path + ".bak"));
        byte[] firstSave = File.ReadAllBytes(path);
        Assert.NotEmpty(firstSave);

        archive.Save(createBackup: true);
        Assert.Equal(firstSave, File.ReadAllBytes(path + ".bak"));
        Assert.Empty(Directory.GetFiles(directory, "*.tmp"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SaveWithoutBackupOverwritesFileAndLeavesBackupUntouched(bool hasBackup)
    {
        string path = Path.Combine(directory, "test.hip");
        File.WriteAllText(path, "original");
        if (hasBackup)
            File.WriteAllText(path + ".bak", "previous backup");
        var archive = CreateArchive();

        archive.Save(path, createBackup: false);

        Assert.NotEqual("original", File.ReadAllText(path));
        Assert.Equal(hasBackup, File.Exists(path + ".bak"));
        if (hasBackup)
            Assert.Equal("previous backup", File.ReadAllText(path + ".bak"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void BackupSettingRoundTrips(bool? value)
    {
        var settings = new IPSettings { CreateBackupWhenSaving = value };

        var loaded = JsonConvert.DeserializeObject<IPSettings>(JsonConvert.SerializeObject(settings));

        Assert.Equal(value, loaded.CreateBackupWhenSaving);
        Assert.Null(JsonConvert.DeserializeObject<IPSettings>("{}").CreateBackupWhenSaving);
    }

    [Fact]
    public void SerializationFailureLeavesOriginalAndBackupUntouched()
    {
        string path = Path.Combine(directory, "test.hip");
        File.WriteAllText(path, "original");
        File.WriteAllText(path + ".bak", "previous backup");
        var archive = CreateArchive();
        int layer = archive.AddLayer();
        ((List<uint>)archive.GetLayerAssetIDs(layer)).Add(123);

        Assert.Throws<KeyNotFoundException>(() => archive.Save(path, true));
        Assert.Equal("original", File.ReadAllText(path));
        Assert.Equal("previous backup", File.ReadAllText(path + ".bak"));
        Assert.Empty(Directory.GetFiles(directory, "*.tmp"));
    }
}

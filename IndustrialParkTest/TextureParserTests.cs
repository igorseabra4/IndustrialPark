using HipHopFile;
using IndustrialPark.Archives;
using RenderWareFile;
using System;
using System.IO;
using Xunit;

namespace IndustrialParkTest;

public class TextureParserTests
{
    [Fact]
    public void FailedTextureImportResetsParserFlag()
    {
        string missingFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txd");

        Assert.ThrowsAny<IOException>(() => TextureConverter.GetAssetsFromTextureDictionary(missingFile, false, Game.BFBB, Platform.GameCube));
        Assert.False(ReadFileMethods.treatStuffAsByteArray);
    }
}

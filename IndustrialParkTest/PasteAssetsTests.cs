using HipHopFile;
using IndustrialPark;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
using Newtonsoft.Json;
using System;
using Xunit;

namespace IndustrialParkTest;

public class PasteAssetsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void ReplacementSettingRoundTrips(bool? value)
    {
        var settings = new IPSettings { ReplaceAssetsOnPaste = value };

        var loaded = JsonConvert.DeserializeObject<IPSettings>(JsonConvert.SerializeObject(settings));

        Assert.Equal(value, loaded.ReplaceAssetsOnPaste);
    }

    [Fact]
    public void MissingReplacementSettingDefaultsToAsk()
    {
        Assert.Null(JsonConvert.DeserializeObject<IPSettings>("{}").ReplaceAssetsOnPaste);
    }

    [Theory]
    [InlineData("{\"ReplaceAssetsOnPaste\":true}", true)]
    [InlineData("{\"ReplaceAssetsOnPaste\":false}", false)]
    public void BooleanReplacementSettingsLoad(string json, bool expected)
    {
        Assert.Equal(expected, JsonConvert.DeserializeObject<IPSettings>(json).ReplaceAssetsOnPaste);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void PasteReplacementChoicePreservesReferences(bool replaceExisting)
    {
        var archive = new Archive(Platform.GameCube, Game.BFBB, DateTime.Now, "test", false, false, false);
        int layer = archive.AddLayer();
        var existing = new AssetGeneric("target", AssetType.RawImage, [1]);
        var source = new AssetGeneric("target", AssetType.RawImage, [2]);
        var group = new AssetGRUP("group") { Items = [source.assetID] };
        existing.SetGame(Game.BFBB);
        source.SetGame(Game.BFBB);
        group.SetGame(Game.BFBB);
        archive.AddNewAsset(existing, layer);

        var clipboard = AssetService.AssetsToClipboard([source, group], Platform.GameCube, null);
        var converted = AssetService.ClipboardToAssets(Platform.GameCube, Game.BFBB, true,
            replaceExisting, archive.ContainsAsset, false, clipboard);
        var pasted = archive.AddMultipleAssets(converted, layer, replaceExisting);

        var pastedTarget = Assert.IsType<AssetGeneric>(pasted[0]);
        var pastedGroup = Assert.IsType<AssetGRUP>(pasted[1]);
        Assert.Equal(pastedTarget.assetID, (uint)pastedGroup.Items[0]);
        Assert.Equal(replaceExisting, pastedTarget.assetID == existing.assetID);
        Assert.Equal(new byte[] { 2 }, pastedTarget.Data);
        Assert.Equal(replaceExisting ? 2 : 3, archive.AssetCount);
        if (!replaceExisting)
            Assert.Same(existing, archive.GetAsset(existing.assetID));
    }
}

using HipHopFile;
using IndustrialPark.Assets;

namespace IndustrialPark.Undo;

public class AssetDataSwappedAction : IReversibleAction
{
    private readonly Archives.Archive Archive;
    public readonly uint AssetID;
    public readonly string AssetName;
    public byte[] PreviousData;
    public byte[] NewData;
    private readonly Game Game;
    private readonly Platform Platform;

    public AssetDataSwappedAction(Archives.Archive archive, uint assetID, byte[] removedAssetData, byte[] addedAssetData, Game game, Platform platform)
    {
        this.Archive = archive;
        this.AssetID = assetID;
        this.AssetName = archive.TryGetAsset(assetID, out Asset asset) ? asset.assetName : null;
        this.PreviousData = removedAssetData;
        this.NewData = addedAssetData;
        this.Game = game;
        this.Platform = platform;
    }

    public void Undo()
    {
        Program.EditorManager.SetAssetData(Archive, AssetID, PreviousData, Game, Platform);
    }

    public void Redo()
    {
        Program.EditorManager.SetAssetData(Archive, AssetID, NewData, Game, Platform);
    }

    public override string ToString() => $"[{AssetID:X8}] {AssetName ?? ""} property changed";
}
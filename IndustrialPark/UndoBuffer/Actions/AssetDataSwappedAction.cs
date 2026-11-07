using HipHopFile;

namespace IndustrialPark;

public class AssetDataSwappedAction : IReversibleAction
{
    private readonly Archive.Archive Archive;
    public readonly uint AssetID;
    public readonly string AssetName;
    public byte[] PreviousData;
    public byte[] NewData;
    private readonly Platform Platform;

    public AssetDataSwappedAction(Archive.Archive archive, uint assetID, byte[] removedAssetData, byte[] addedAssetData, Platform platform)
    {
        this.Archive = archive;
        this.AssetID = assetID;
        this.AssetName = archive.TryGetAsset(assetID, out Asset asset) ? asset.assetName : null;
        this.PreviousData = removedAssetData;
        this.NewData = addedAssetData;
        this.Platform = platform;
    }

    public void Undo()
    {
        Program.EditorManager.SetAssetData(Archive, AssetID, PreviousData, Platform);
    }

    public void Redo()
    {
        Program.EditorManager.SetAssetData(Archive, AssetID, NewData, Platform);
    }

    public override string ToString() => $"[{AssetID:X8}] {AssetName ?? ""} property changed";
}
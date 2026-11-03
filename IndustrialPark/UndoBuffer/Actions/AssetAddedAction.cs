using HipHopFile;
using IndustrialPark.Archive;

namespace IndustrialPark;

public class AssetAddedAction : IReversibleAction
{
    public uint AssetID => AHDR.assetID;
    public bool AssetRemoved => assetRemoved;
    public byte[] AssetData => AHDR.data;

    private AssetRepository assetRepository;
    public readonly Section_AHDR AHDR;
    private readonly Game game;
    private readonly Platform platform;
    private bool assetRemoved;

    public AssetAddedAction(ArchiveSession session, AssetRepository assetRepository, Asset asset, bool assetRemoved = false)
    {
        this.assetRepository = assetRepository;
        AHDR = asset.BuildAHDR(session.Platform.Endianness());
        this.game = session.Game;
        this.platform = session.Platform;
        this.assetRemoved = assetRemoved;
    }

    public void Undo()
    {
        if (assetRemoved)
            AddAsset();
        else
            RemoveAsset();
    }

    public void Redo()
    {
        if (assetRemoved)
            RemoveAsset();
        else
            AddAsset();
    }

    private void AddAsset()
    {
        var asset = AssetService.ConvertHeaderToAsset(AHDR, game, platform, false, out _);
        assetRepository.Add(asset);
    }

    private void RemoveAsset()
    {
        assetRepository.Remove(AHDR.assetID);
    }

    public override string ToString()
    {
        return $"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} ({AssetTypeContainer.AssetTypeToString(AHDR.assetType)}) { (assetRemoved ? "removed" : "added") }";
    }

    public void UpdateAssetData(byte[] data)
    {
        AHDR.data = data;
    }
}

using HipHopFile;

namespace IndustrialPark;

public class AssetRemovedAction : IReversibleAction
{
    private ArchiveEditorFunctions archive;

    private Section_AHDR AHDR;

    private int layerIndex;

    public AssetRemovedAction(ArchiveEditorFunctions archive, Section_AHDR AHDR, int layerIndex)
    {
        this.archive = archive;
        this.AHDR = AHDR;
        this.layerIndex = layerIndex;
    }

    public void Undo()
    {
        archive.AddAsset(AHDR, archive.game, archive.platform.Endianness(), true, layerIndex);
    }

    public void Redo()
    {
        archive.RemoveAsset(AHDR.assetID);
    }
}

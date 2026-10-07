using HipHopFile;

namespace IndustrialPark;

public class AssetAddedAction : IReversibleAction
{
    private ArchiveEditorFunctions archive;

    private Section_AHDR AHDR;

    private int layerIndex;

    public AssetAddedAction(ArchiveEditorFunctions archive, Section_AHDR AHDR, int layerIndex)
    {
        this.archive = archive;
        this.AHDR = AHDR;
        this.layerIndex = layerIndex;
    }

    public void Undo()
    {
        archive.RemoveAsset(AHDR.assetID);
    }

    public void Redo()
    {
        archive.AddAsset(AHDR, archive.game, archive.platform.Endianness(), true, layerIndex);
    }
}

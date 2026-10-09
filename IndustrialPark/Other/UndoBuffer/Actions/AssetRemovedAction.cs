using HipHopFile;
using System.IO;

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
        archive.AddAsset(AHDR, archive.game, archive.platform.Endianness(), layerIndex, true);
        Program.MainForm.RefreshAssetList(archive);
    }

    public void Redo()
    {
        archive.RemoveAsset(AHDR.assetID);
        Program.MainForm.RefreshAssetList(archive);
    }

    public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

    public override string ToString()
    {
        return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] {AHDR.ADBG.assetName} removed";
    }
}

using HipHopFile;
using System.IO;

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
        Program.MainForm.RefreshAssetList(archive);
    }

    public void Redo()
    {
        if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.SoundStream)
        {
            archive.AddSoundToSNDI(AHDR.data, AHDR.assetID, AHDR.assetType, out byte[] soundData);
            AHDR.data = soundData;
        }
        archive.AddAsset(AHDR, archive.game, archive.platform.Endianness(), layerIndex, true);
        Program.MainForm.RefreshAssetList(archive);
    }

    public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

    public override string ToString()
    {
        return $"[{Path.GetFileNameWithoutExtension(archive.currentlyOpenFilePath)}] {AHDR.ADBG.assetName} created";
    }
}

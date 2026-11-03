using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public class AssetHistoryManager
{
    private readonly Archive archive;
    private readonly Dictionary<uint, byte[]> history;

    public AssetHistoryManager(Archive archive)
    {
        this.archive = archive;
        history = new Dictionary<uint, byte[]>();
        Program.UndoBuffer.ActionAdded += UndoBuffer_ActionAdded;
    }

    public void AddAsset(Asset asset)
    {
        history[asset.assetID] = asset.Serialize(archive.Platform.Endianness());
    }

    public void AddAsset(uint assetID, byte[] data)
    {
        history[assetID] = data;
    }

    public void RemoveAsset(uint assetID)
    {
        history.Remove(assetID);
    }

    public void FlushChangesToUndoBuffer(ArchiveEditor archiveEditor, Asset asset, bool hard = false)
    {
        if (!history.TryGetValue(asset.assetID, out byte[] previous))
            return;

        var next = asset.Serialize(archive.Platform.Endianness());

        if (Enumerable.SequenceEqual(previous, next))
            return;

        var action = new AssetDataSwappedAction(archive, asset.assetID, previous, next, archive.Platform);

        if (hard)
            Program.UndoBuffer.AddAction(new MultiAction(archiveEditor, [action]));
        else
            Program.UndoBuffer.AddActionToTransaction(archiveEditor, action);
    }

    private void UndoBuffer_ActionAdded(MultiAction action)
    {
        if (action.archiveEditor.archive != archive)
            return;

        foreach (var a in action.GetActions)
        {
            if (a is AssetAddedAction add && history.ContainsKey(add.AssetID))
                history[add.AssetID] = add.AssetData;
            if (a is AssetDataSwappedAction s && history.ContainsKey(s.AssetID))
                history[s.AssetID] = s.NewData;
        }
    }
}

using IndustrialPark.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IndustrialPark;

public class MultiAction : IReversibleAction
{
    public readonly ArchiveEditor archiveEditor;
    private List<IReversibleAction> actions;

    public MultiAction(ArchiveEditor archiveEditor, IEnumerable<IReversibleAction> actions)
    {
        this.archiveEditor = archiveEditor;
        this.actions = [..actions];
    }

    public void Undo()
    {
        for (int i = actions.Count - 1; i >= 0; i--)
            actions[i].Undo();
        archiveEditor.NotifyArchiveStateChanged();
    }

    public void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
            actions[i].Redo();
        archiveEditor.NotifyArchiveStateChanged();
    }

    public override string ToString()
    {
        var fileName = Path.GetFileName(archiveEditor.archive?.CurrentFilePath);
        if (actions.Count == 1)
            return $"[{fileName}] {actions.FirstOrDefault()}";

        var result = new StringBuilder();
        result.AppendLine($"[{fileName}]");
        foreach (var action in actions)
        {
            var str = action.ToString();
            if (!str.IsWhiteSpace())
                result.AppendLine($"\t{action.ToString()}");
        }
        return result.ToString();
    }

    public bool IsEmpty => actions.Count == 0;

    public List<IReversibleAction> GetActions => [.. actions];

    public void ConsolidateActions()
    {
        actions = ConsolidateActions(archiveEditor.archive, actions);
    }

    private static List<IReversibleAction> ConsolidateActions(Archive.Archive archive, List<IReversibleAction> actions)
    {
        var result = new List<IReversibleAction>();
        var assetActions = new Dictionary<uint, (int Index, IReversibleAction Action)>();

        foreach (var action in actions)
        {
            uint assetID = action switch
            {
                AssetAddedAction added => added.AssetID,
                AssetDataSwappedAction swapped => swapped.AssetID,
                _ => 0
            };

            if (action is not AssetAddedAction && action is not AssetDataSwappedAction)
            {
                result.Add(action);
                continue;
            }

            if (!assetActions.TryGetValue(assetID, out var previous))
            {
                assetActions[assetID] = (result.Count, action);
                result.Add(action);
                continue;
            }

            switch (previous.Action, action)
            {
                // Added -> Removed: discard both.
                case (AssetAddedAction { AssetRemoved: false }, AssetAddedAction { AssetRemoved: true }):
                    result[previous.Index] = null;
                    assetActions.Remove(assetID);
                    break;

                // Removed -> Added: replace with a data swap.
                case (AssetAddedAction { AssetRemoved: true } removed, AssetAddedAction { AssetRemoved: false } added):
                    result[previous.Index] = new AssetDataSwappedAction(archive, assetID, removed.AssetData, added.AssetData, archive.Platform);
                    break;

                // Added -> Modified: keep addition with final data.
                case (AssetAddedAction { AssetRemoved: false } added, AssetDataSwappedAction swapped):
                    added.UpdateAssetData(swapped.NewData);
                    break;

                // Removed -> Modified: discard modification.
                case (AssetAddedAction { AssetRemoved: true }, AssetDataSwappedAction):
                    break;

                // Modified -> Modified: preserve first and last data.
                case (AssetDataSwappedAction previousSwap, AssetDataSwappedAction nextSwap):
                    previousSwap.NewData = nextSwap.NewData;
                    break;

                // Modified -> Removed: replace with removal.
                case (AssetDataSwappedAction previousSwap, AssetAddedAction { AssetRemoved: true } removed):
                    result[previous.Index] = removed;
                    break;

                // Modified -> Added: replace with a data swap.
                case (AssetDataSwappedAction previousSwap, AssetAddedAction { AssetRemoved: false } added):
                    previousSwap.NewData = added.AssetData;
                    break;
            }
        }

        return result.Where(a => a != null).ToList();
    }
}

using System;
using System.Collections.Generic;

namespace IndustrialPark;

public class AllLayersMovedAction : IReversibleAction
{
    private readonly ArchiveEditorFunctions archive;
    private readonly int[] newIndices;

    public AllLayersMovedAction(ArchiveEditorFunctions archive, List<Layer> layersOld, List<Layer> layersNew)
    {
        this.archive = archive;

        if (layersOld.Count != layersNew.Count)
            throw new ArgumentException("Both lists must contain the same layers");

        int[] newIndices = new int[layersOld.Count];
        for (int i = 0; i < layersOld.Count; i++)
        {
            newIndices[i] = layersNew.IndexOf(layersOld[i]);

            if (newIndices[i] == -1)
                throw new ArgumentException("Both lists must contain the same layers");
        }
    }

    public AllLayersMovedAction(ArchiveEditorFunctions archive, int[] newIndices)
    {
        if (newIndices.Length != archive.LayerCount)
            throw new ArgumentException("Both lists must contain the same layers");

        this.archive = archive;
        this.newIndices = newIndices;
    }

    private int[] ReverseDict()
    {
        var result = new int[newIndices.Length];
        for (int i = 0; i < newIndices.Length; i++)
            result[newIndices[i]] = i;
        return result;
    }

    public void Undo()
    {
        archive.SetLayerIndices(ReverseDict());
        Program.MainForm.RefreshLayerList(archive);
    }

    public void Redo()
    {
        archive.SetLayerIndices(newIndices);
        Program.MainForm.RefreshLayerList(archive);
    }

    public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;

    public override string ToString()
    {
        return $"[{archive.LevelName.ToUpper()}] Layers reordered";
    }
}
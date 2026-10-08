using System.Collections.Generic;

namespace IndustrialPark;

public class SelectionAction : IReversibleAction
{
    private List<uint> selection;

    public SelectionAction()
    {
        selection = null;
    }

    public SelectionAction(IEnumerable<uint> selection)
    {
        this.selection = [..selection];
    }

    public SelectionAction(uint assetID)
    {
        this.selection = [assetID];
    }

    public void Undo()
    {
        if (selection.Count != 0)
            Program.MainForm.SetSelectedIndices(selection);
    }

    public void Redo()
    {
        Undo();
    }

    public override string ToString()
    {
        return "";
    }

    public bool ContainsArchive(ArchiveEditorFunctions archive) => false;
}

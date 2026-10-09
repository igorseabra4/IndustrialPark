using System.Collections.Generic;
using System.Linq;

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
        this.selection = selection.Any() ? [..selection] : null;
    }

    public SelectionAction(uint assetID)
    {
        selection = [assetID];
    }

    public void Undo()
    {
        if (selection != null && selection.Count > 0)
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

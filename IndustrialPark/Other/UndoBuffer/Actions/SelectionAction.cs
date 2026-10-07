using System.Collections.Generic;

namespace IndustrialPark;

public class SelectionAction(List<uint> selection) : IReversibleAction
{
    public void Undo()
    {
        if (selection.Count != 0)
            Program.MainForm.SetSelectedIndices(selection);
    }

    public void Redo()
    {
        Undo();
    }
}

using System.Collections.Generic;

namespace IndustrialPark;

public class MultiAction : IReversibleAction
{
    private List<IReversibleAction> actions;

    public MultiAction(List<IReversibleAction> actions)
    {
        this.actions = actions;
    }

    public void Undo()
    {
        foreach (var action in this.actions)
            action.Undo();
    }

    public void Redo()
    {
        foreach (var action in this.actions)
            action.Redo();
    }
}

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
        for (int i = actions.Count - 1; i >= 0; i--)
            actions[i].Undo();
    }

    public void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
            actions[i].Redo();
    }
}

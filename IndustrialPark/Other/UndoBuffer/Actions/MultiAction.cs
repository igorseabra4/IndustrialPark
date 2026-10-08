using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IndustrialPark;

public class MultiAction : IReversibleAction
{
    private List<IReversibleAction> actions;

    public MultiAction(IEnumerable<IReversibleAction> actions)
    {
        this.actions = [..actions];
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

    public bool ContainsArchive(ArchiveEditorFunctions archive) => actions.Any(action => action.ContainsArchive(archive));

    public override string ToString()
    {
        var result = new StringBuilder();
        result.AppendLine("[Action]");
        foreach (var action in actions)
        {
            var str = action.ToString();
            if (!str.IsWhiteSpace())
                result.AppendLine($"\t{action.ToString()}");
        }
        return result.ToString();
    }
}

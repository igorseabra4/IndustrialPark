using System.Collections.Generic;

namespace IndustrialPark;

public class UndoBuffer
{
    private readonly List<IReversibleAction> actions = [];
    private int currentIndex = -1;

    public void AddAction(IReversibleAction action)
    {
        int redoCount = actions.Count - currentIndex - 1;
        if (redoCount > 0)
            actions.RemoveRange(currentIndex + 1, redoCount);

        actions.Add(action);
        currentIndex++;
    }

    public void Undo()
    {
        if (currentIndex >= 0)
            actions[currentIndex--].Undo();
    }

    public void Redo()
    {
        if (currentIndex < actions.Count - 1)
            actions[++currentIndex].Redo();
    }

    public void RemoveActionsOfArchive(ArchiveEditorFunctions archive)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].ContainsArchive(archive))
            {
                actions.RemoveAt(i);
                if (i <= currentIndex)
                    currentIndex--;
                i--;
            }
        }
    }
}

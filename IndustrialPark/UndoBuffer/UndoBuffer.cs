using System;
using System.Collections.Generic;
using System.Text;

namespace IndustrialPark;

public class UndoBuffer
{
    private readonly List<MultiAction> actions = [];
    private int currentIndex = -1;

    public bool CanUndo => currentIndex >= 0;
    public bool CanRedo => currentIndex < actions.Count - 1;

    /// <summary>
    /// Raised whenever CanUndo or CanRedo may have changed.
    /// </summary>
    public event Action Changed;
    public event Action<MultiAction> ActionAdded;

    public void Undo()
    {
        if (CanUndo)
        {
            actions[currentIndex--].Undo();
            Changed?.Invoke();
        }
    }

    public void Redo()
    {
        if (CanRedo)
        {
            actions[++currentIndex].Redo();
            Changed?.Invoke();
        }
    }

    public void RemoveActionsOfArchive(ArchiveEditor archiveEditor)
    {
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            if (actions[i].archiveEditor == archiveEditor)
            {
                if (i <= currentIndex)
                    currentIndex--;
                actions.RemoveAt(i);
            }
        }
        Changed?.Invoke();
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        if (currentIndex == -1)
            result.AppendLine("[Current State]");
        for (int i = 0; i < actions.Count; i++)
        {
            result.AppendLine(actions[i].ToString().TrimEnd('\r', '\n'));
            if (currentIndex == i)
                result.AppendLine("[Current State]");
        }
        return result.ToString();
    }

    private Dictionary<ArchiveEditor, List<IReversibleAction>> currentArchiveTransactions = new Dictionary<ArchiveEditor, List<IReversibleAction>>();

    public void BeginTransaction(ArchiveEditor archive)
    {
        currentArchiveTransactions[archive] = [];
    }

    public void AddActionToTransaction(ArchiveEditor archive, IReversibleAction action)
    {
        if (currentArchiveTransactions.TryGetValue(archive, out List<IReversibleAction> actions))
            actions.Add(action);
    }

    public void EndTransaction(ArchiveEditor archive)
    {
        if (currentArchiveTransactions.TryGetValue(archive, out List<IReversibleAction> actions))
        {
            AddAction(new MultiAction(archive, actions));
            currentArchiveTransactions.Remove(archive);
        }
        else
        {
            MessageBox.Show("You are trying to finish a transaction that has not started.");
        }
    }

    public void AddAction(MultiAction action)
    {
        if (action.IsEmpty)
            return;

        int redoCount = actions.Count - currentIndex - 1;
        if (redoCount > 0)
            actions.RemoveRange(currentIndex + 1, redoCount);

        action.ConsolidateActions();
        actions.Add(action);
        currentIndex++;
        Changed?.Invoke();
        ActionAdded?.Invoke(action);
    }
}

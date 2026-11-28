using IndustrialPark.ArchiveEditors;
using IndustrialPark.Archives;
using System.Collections.Generic;
using System.IO;

namespace IndustrialPark;

public class Logger
{
    public delegate void OutputHandler(string message, bool important);
    public event OutputHandler Output;
    public List<string> Lines { get; private set; } = new List<string>();

    public Logger()
    {
    }

    public void Subscribe(OutputHandler action)
    {
        Output += action;
    }

    public void Unsubscribe(OutputHandler action)
    {
        Output -= action;
    }

    private void Logged(bool important)
    {
        Output?.Invoke(Get(), important);
    }

    public string Get() => string.Join("\n\n", Lines);

    public void Log(string message, bool important = false)
    {
        Lines.Add(message);
        Logged(important);
    }

    public void Log(ArchiveEditor archiveEditor, string message, bool important = false)
    {
        Lines.Add($"[{Path.GetFileName(archiveEditor.archive?.CurrentFilePath ?? "Empty")}]: {message}");
        Logged(important);
    }

    public void Log(Archive archive, string message, bool important = false)
    {
        Lines.Add($"[{Path.GetFileName(archive?.CurrentFilePath ?? "Empty")}]: {message}");
        Logged(important);
    }

    public void Clear()
    {
        Lines.Clear();
        Logged(false);
    }
}

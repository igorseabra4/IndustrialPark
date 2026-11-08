using HipHopFile;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.Scripting;

public static class Archive
{
    public static ArchiveInterface New(Game game, Platform platform)
    {
        return new ArchiveInterface(game, platform);
    }

    public static ArchiveInterface FromPath(string filePath)
    {
        return new ArchiveInterface(filePath);
    }

    public static List<ArchiveInterface> FromPaths(List<string> filePaths)
    {
        List<ArchiveInterface> archives = new List<ArchiveInterface>();
        foreach (var path in filePaths)
            archives.Add(FromPath(path));
        return archives;
    }

    public static List<ArchiveInterface> FromOpenInstance()
    {
        return Program.ArchiveEditors
            .Where(ae => ae.archive != null)
            .Select(ae => new ArchiveInterface(ae.archive))
            .ToList();
    }

    public static ArchiveInterface FromOpenInstance(string name)
    {
        var ae = Program.ArchiveEditors
            .Where(ae => ae.archive != null)
            .FirstOrDefault(ae => Path.GetFileName(ae.GetCurrentFilePath()).Equals(name));
        return ae == null ? null : new ArchiveInterface(ae.archive);
    }
}

using HipHopFile;
using System;
using System.IO;

namespace IndustrialPark.Archive;

public class ArchiveSession
{
    public Section_PACK PACK { get; private set; }

    public Game Game { get; private set; }

    public Platform Platform { get; private set; }

    public string CurrentFilePath { get; set; }

    public string LevelName => string.IsNullOrEmpty(CurrentFilePath) ? "" : Path.GetFileNameWithoutExtension(CurrentFilePath).ToLower();

    public event Action<Platform, Game, DateTime, string> ArchiveHeaderSet;

    public event Action<int> ModDateSet;

    public ArchiveSession(Platform platform, Game game, Section_PACK PACK, string fileName)
    {
        this.Platform = platform;
        this.Game = game;
        this.PACK = PACK;
        CurrentFilePath = fileName;
    }

    public ArchiveSession(Platform platform, Game game, DateTime fileDate, string dateString)
    {
        this.Platform = platform;
        this.Game = game;
        this.PACK = ArchiveService.GeneratePACK(platform, game, fileDate, dateString);
        CurrentFilePath = null;
    }

    public void SetArchiveHeader(Platform platform, Game game, DateTime fileDate, string dateString)
    {
        this.Platform = platform;
        this.Game = game;
        this.PACK = ArchiveService.GeneratePACK(platform, game, fileDate, dateString);

        ArchiveHeaderSet?.Invoke(platform, game, fileDate, dateString);
    }

    public void SetModDate(int value)
    {
        PACK.PMOD.modDate = value;

        ModDateSet?.Invoke(value);
    }
}
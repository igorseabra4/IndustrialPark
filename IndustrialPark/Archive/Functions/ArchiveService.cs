using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark.Archive;

public static class ArchiveService
{
    public static Section_PACK GeneratePACK(Platform platform, Game game, DateTime fileDate, string dateString)
    {
        var PACK = new Section_PACK
        {
            PMOD = new Section_PMOD((int)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()),
            PCRT = new Section_PCRT((int)((DateTimeOffset)fileDate).ToUnixTimeSeconds(), dateString),
            PCNT = new Section_PCNT(0, 0, 0, 0, 0),
            PFLG = new Section_PFLG(46)
        };

        if (game != Game.Scooby)
        {
            PACK.PVER = new Section_PVER(2, 655375, 1);
            PACK.PLAT = new Section_PLAT
            { regionFormat = "NTSC" };
        }

        switch (game)
        {
            case Game.Scooby:
                PACK.PVER = new Section_PVER(2, 262150, 1);
                break;
            case Game.BFBB:
                PACK.PLAT.language = "US Common";
                PACK.PLAT.targetGame = "Sponge Bob";
                switch (platform)
                {
                    case Platform.GameCube:
                        PACK.PFLG = new Section_PFLG(36241454);
                        PACK.PLAT.targetPlatform = "GC";
                        PACK.PLAT.targetPlatformName = "GameCube";
                        break;
                    case Platform.PS2:
                        PACK.PFLG = new Section_PFLG(36438062);
                        PACK.PLAT.targetPlatform = "P2";
                        PACK.PLAT.targetPlatformName = "PlayStation 2";
                        break;
                    case Platform.Xbox:
                        PACK.PFLG = new Section_PFLG(36306990);
                        PACK.PLAT.targetPlatform = "XB";
                        PACK.PLAT.targetPlatformName = "Xbox";
                        break;
                }
                break;
            case Game.Incredibles:
            case Game.ROTU:
            case Game.RatProto:
                PACK.PLAT.language = "US";
                PACK.PLAT.targetGame = "Incredibles";
                switch (platform)
                {
                    case Platform.Xbox:
                        PACK.PLAT.targetPlatform = "BX";
                        break;
                    case Platform.GameCube:
                        PACK.PLAT.targetPlatform = "GC";
                        break;
                    case Platform.PS2:
                        PACK.PLAT.targetPlatform = "PS2";
                        break;
                }
                break;
        }

        return PACK;
    }

    public static HipFile BuildHipFile(ArchiveSession session, IEnumerable<Asset> assets, LayerRepository layerManager)
    {
        var DICT = new Section_DICT();

        foreach (var asset in assets)
        {
            asset.SetGame(session.Game);
            if (asset is AssetSNDI_PS2 sndips2)
                sndips2.OrderEntries();
            DICT.ATOC.AHDRList.Add(asset.BuildAHDR(session.Platform.Endianness()));
        }

        DICT.LTOC.LHDRList = layerManager.AutoLayers ?
            LayerService.BuildLayers(assets, session.Game).Select(l => l.BuildLHDR(session.Game)).ToList() :
            layerManager.ToLayerHeaders(session.Game).ToList();

        var HIPB = new Section_HIPB
        {
            HasNoLayers = Convert.ToInt32(layerManager.AutoLayers),
            ScoobyPlatform = session.Platform,
            IncrediblesGame = session.Game,
            LayerNames = layerManager.GetLayerNames()
        };

        session.SetModDate((int)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());

        return new HipFile(session.Game, session.Platform, session.PACK, DICT, HIPB);
    }

    public static void LogAssetOrder(Archive archive, Dictionary<uint, Section_AHDR> AHDRs)
    {
        var tempLog = new List<string>();
        var diff = false;
        for (int i = 0; i < archive.LayerCount; i++)
        {
            tempLog.Add(archive.GetLayerType(i).ToString());
            foreach (var u in archive.GetLayerAssetIDs(i))
            {
                var asset = archive.GetAsset(u);
                string line = asset.assetType.ToString();
                if (asset is AssetPLAT plat)
                    line += " " + plat.PlatformType.ToString();
                else if (asset is AssetDYNA dyna)
                    line += " " + dyna.Type.ToString();
                if (!tempLog.Contains(line))
                    tempLog.Add(line);
            }

            var list = archive.GetLayerAssetIDs(i);
            var sortedList = archive.GetLayerAssetIDs(i).OrderBy(u => AHDRs[u].GetCompareValue(archive.Game)).ToList();
            if (!Enumerable.SequenceEqual(list, sortedList))
            {
                diff = true;
                tempLog.Add("sorting failed");
                var exp = "";
                foreach (var l in list)
                    exp += l.ToString("X8") + " ";
                var res = "";
                foreach (var l in sortedList)
                    res += l.ToString("X8") + " ";
                tempLog.Add(exp);
                tempLog.Add(res);
            }

            tempLog.Add("");
        }

        if (diff)
            File.WriteAllLines(Path.GetFileName(archive.CurrentFilePath) + "_orderlog.txt", tempLog.ToArray());
    }
}
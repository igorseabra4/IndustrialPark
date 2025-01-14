using HipHopFile;
using Newtonsoft.Json;
using RenderWareFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    [JsonObject(MemberSerialization.Fields)]
    internal class AssetClipboard
    {
        internal List<Game> games;
        internal List<Platform> platforms;
        internal List<Section_AHDR_Clipboard> assets;
        internal List<AssetID[]> jspExtraInfo;
        internal List<int?> rwVersions;

        internal AssetClipboard()
        {
            games = new List<Game>();
            platforms = new List<Platform>();
            assets = new List<Section_AHDR_Clipboard>();
            jspExtraInfo = new List<AssetID[]>();
            rwVersions = new List<int?>();
        }

        internal void Add(Game game, Platform platform, Section_AHDR asset, AssetID[] jspExtraInfo, RWVersion? rwVer)
        {
            games.Add(game);
            platforms.Add(platform);
            assets.Add(new Section_AHDR_Clipboard(asset));
            this.jspExtraInfo.Add(jspExtraInfo);
            rwVersions.Add(rwVer?.Write());
        }
    }

    /// <summary>
    /// A more basic class of AHDR for clipboard
    /// </summary>
    [JsonObject(MemberSerialization.Fields)]
    internal class Section_AHDR_Clipboard
    {
        internal uint assetID;
        internal AssetType assetType;
        internal AHDRFlags flags;
        internal string assetName;
        internal string assetFileName;
        internal int checksum;
        internal byte[] data;

        internal Section_AHDR_Clipboard(Section_AHDR ahdr)
        {
            assetID = ahdr.assetID;
            assetType = ahdr.assetType;
            flags = ahdr.flags;
            assetName = ahdr.ADBG.assetName;
            assetFileName = ahdr.ADBG.assetFileName;
            checksum = ahdr.ADBG.checksum;
            data = ahdr.data;
        }

        internal Section_AHDR ToAHDR()
        {
            return new Section_AHDR(assetID, assetType, flags,
                new Section_ADBG(0, assetName, assetFileName, checksum),
                data);
        }
        
    }
}
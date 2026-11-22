using HipHopFile;
using IndustrialPark.Assets;
using Newtonsoft.Json;

namespace IndustrialPark.Archives;

[JsonObject(MemberSerialization.Fields)]
public class AssetClipboard
{
    public Section_AHDR_Clipboard AHDR;
    public AssetID[] jspExtraInfo;
    public int? rwVersion;

    public AssetClipboard(Section_AHDR ahdr, AssetID[] jspExtraInfo, int? rwVersion)
    {
        this.AHDR = new Section_AHDR_Clipboard(ahdr);
        this.jspExtraInfo = jspExtraInfo;
        this.rwVersion = rwVersion;
    }
}

/// <summary>
/// A more basic class of AHDR for clipboard
/// </summary>
[JsonObject(MemberSerialization.Fields)]
public class Section_AHDR_Clipboard
{
    internal uint assetID;
    internal AssetType assetType;
    internal AHDRFlags flags;
    internal string assetName;
    internal string assetFileName;
    internal int checksum;
    internal byte[] data;
    internal Game game;
    internal Platform platform;

    internal Section_AHDR_Clipboard(Section_AHDR ahdr)
    {
        assetID = ahdr.assetID;
        assetType = ahdr.assetType;
        flags = ahdr.flags;
        assetName = ahdr.ADBG.assetName;
        assetFileName = ahdr.ADBG.assetFileName;
        checksum = ahdr.ADBG.checksum;
        data = ahdr.data;
        game = ahdr.Game;
        platform = ahdr.Platform;
    }

    internal Section_AHDR ToAHDR() => new Section_AHDR(assetID, assetType, flags,
            new Section_ADBG(0, assetName, assetFileName, checksum),
            data, game, platform);
}
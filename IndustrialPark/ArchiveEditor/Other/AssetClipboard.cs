using HipHopFile;
using Newtonsoft.Json;

namespace IndustrialPark
{
    [JsonObject(MemberSerialization.Fields)]
    public class AssetClipboard
    {
        public Game game;
        public Platform platform;
        public Section_AHDR_Clipboard AHDR;
        public AssetID[] jspExtraInfo;
        public int? rwVersion;

        public AssetClipboard(Game game, Platform platform, Section_AHDR ahdr, AssetID[] jspExtraInfo, int? rwVersion)
        {
            this.game = game;
            this.platform = platform;
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
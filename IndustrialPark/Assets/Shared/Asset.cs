using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class Asset : GenericAssetDataContainer
    {
        public bool isSelected;
        public bool isInvisible = false;

        public Game game;
        public Platform platform;

        public uint assetID;
        public string assetName;
        public string assetFileName;
        public AssetType assetType;
        public AHDRFlags flags;
        public int checksum;

        [Browsable(false)]
        public virtual string DataLength => "";

        public Asset(string assetName, AssetType assetType)
        {
            game = Game.Unknown;
            platform = Platform.Unknown;

            this.assetType = assetType;
            this.assetName = assetName;
            assetID = new AssetID(assetName);
            assetFileName = "";
            flags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType);
        }

        public Asset(Section_AHDR AHDR, Game game, Platform platform)
        {
            this.game = game;
            this.platform = platform;

            assetID = AHDR.assetID;
            assetName = AHDR.ADBG.assetName;
            assetFileName = AHDR.ADBG.assetFileName;
            assetType = AHDR.assetType;
            flags = AHDR.flags;
            checksum = AHDR.ADBG.checksum;
        }

        // use with DUPC VIL only
        protected Asset() { }

        public Section_AHDR BuildAHDR() =>
            new Section_AHDR(assetID, assetType, flags,
                new Section_ADBG(0, assetName, assetFileName, checksum),
                Serialize(game, platform));

        public override string ToString() =>  $"{assetName} [{assetID:X8}]";

        public override int GetHashCode() => assetID.GetHashCode();

        public virtual void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
        }
    }
}
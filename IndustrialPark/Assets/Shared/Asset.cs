using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class Asset : GenericAssetDataContainer
    {
        public bool isSelected;
        public bool isInvisible = false;

        [Browsable(false)]
        public virtual bool SkipBuildTesting => false;

        [Browsable(false)]
        public uint assetID { get; set; }
        public string assetName;
        public string assetFileName;
        public AssetType assetType;
        public AHDRFlags flags;
        public int checksum;

        [Browsable(false)]
        public virtual string AssetInfo => "";
        [Browsable(false)]
        public virtual string AssetInfoLinks => "-";
        [Browsable(false)]
        public virtual string TypeString => AssetTypeContainer.AssetTypeToString(assetType);

        public Asset(string assetName, AssetType assetType)
        {
            _game = Game.Unknown;

            this.assetType = assetType;
            this.assetName = assetName;
            assetID = new AssetID(assetName);
            assetFileName = "";
            flags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType);
        }

        public Asset(Section_AHDR AHDR, Game game)
        {
            _game = game;

            assetID = AHDR.assetID;
            assetName = AHDR.ADBG.assetName;
            assetFileName = AHDR.ADBG.assetFileName;
            assetType = AHDR.assetType;
            flags = AHDR.flags;
            checksum = AHDR.ADBG.checksum;
        }

        // use with DUPC VIL only
        protected Asset() { }

        public Section_AHDR BuildAHDR(Endianness endianness)
        {
            return new Section_AHDR(assetID, assetType, flags,
                new Section_ADBG(0, assetName, assetFileName, checksum),
                Serialize(endianness));
        }

        public override string ToString() => $"{assetName} [{assetID:X8}]";

        public override int GetHashCode() => (int)assetID;

        public override bool Equals(object obj)
        {
            if (obj is Asset other)
                return other.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
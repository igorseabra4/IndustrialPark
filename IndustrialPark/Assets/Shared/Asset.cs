using HipHopFile;

namespace IndustrialPark
{
    public abstract class Asset : GenericAssetDataContainer
    {
        public bool isSelected;
        public bool isInvisible = false;

        public AssetID AssetID;
        public string assetName;
        public string assetFileName;
        public int checksum;

        public Asset(string assetName)
        {
            this.assetName = assetName;
            AssetID = assetName;
            assetFileName = "";
            checksum = 0;
        }

        public Asset(Section_AHDR AHDR)
        {
            AssetID = AHDR.assetID;
            assetName = AHDR.ADBG.assetName;
            assetFileName = AHDR.ADBG.assetFileName;
            checksum = AHDR.ADBG.checksum;
        }

        // use with DUPC VIL only
        protected Asset() { }

        public override string ToString() =>  $"{assetName} [{AssetID:X8}]";

        public override int GetHashCode() => AssetID.GetHashCode();

        public virtual void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
        }
    }
}
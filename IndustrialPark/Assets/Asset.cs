using HipHopFile;

namespace IndustrialPark
{
    public abstract class Asset
    {
        public Section_AHDR AHDR;
        public bool isSelected;

        public Asset(Section_AHDR AHDR)
        {
            this.AHDR = AHDR;
        }

        public abstract void Setup(SharpRenderer renderer, bool defaultMode);

        public override string ToString()
        {
            return AHDR.ADBG.assetName + " [" + AHDR.assetID.ToString("X8") + "]";
        }
    }
}
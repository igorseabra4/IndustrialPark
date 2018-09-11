using HipHopFile;

namespace IndustrialPark
{
    public class AssetCNTR : ObjectAsset
    {
        public AssetCNTR(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0xC;
        }

        public short Count
        {
            get => ReadShort(0x8);
            set => Write(0x8, value);
        }

        public short Unknown
        {
            get => ReadShort(0xA);
            set => Write(0xA, value);
        }
    }
}
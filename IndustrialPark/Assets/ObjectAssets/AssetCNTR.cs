using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCNTR : ObjectAsset
    {
        public AssetCNTR(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0xC;

        [Category("Counter")]
        public short Count
        {
            get => ReadShort(0x8);
            set => Write(0x8, value);
        }

        [Category("Counter")]
        public short Unknown
        {
            get => ReadShort(0xA);
            set => Write(0xA, value);
        }
    }
}
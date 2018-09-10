using HipHopFile;

namespace IndustrialPark
{
    public class AssetSIMP : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        protected override int EventStartOffset
        {
            get => 0x60 + Offset;
        }

        public AssetSIMP(Section_AHDR AHDR) : base(AHDR) { }

        public float UnknownFloat_54
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        public int Unknown_58
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        public AssetID Unknown_5C
        {
            get => ReadUInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }
    }
}
using HipHopFile;

namespace IndustrialPark
{
    public class AssetBOUL : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        protected override int EventStartOffset
        {
            get => 0x9C + Offset;
        }

        public AssetBOUL(Section_AHDR AHDR) : base(AHDR) { }

        public float UnknownFloat54
        {
            get => ReadUInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        public float UnknownFloat58
        {
            get => ReadUInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        public float UnknownFloat5C
        {
            get => ReadUInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        public float UnknownFloat60
        {
            get => ReadUInt(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        public float UnknownFloat64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        public float UnknownFloat68
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        public float UnknownFloat70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        public float UnknownFloat74
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }
        
        public int UnknownInt78
        {
            get => ReadInt(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        public float UnknownFloat7C
        {
            get => ReadFloat(0x7C);
            set => Write(0x7C, value);
        }

        public int UnknownInt80
        {
            get => ReadInt(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        public AssetID UnknownAssetID84
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        public float UnknownFloat88
        {
            get => ReadFloat(0x88);
            set => Write(0x88, value);
        }

        public float UnknownFloat8C
        {
            get => ReadFloat(0x8C);
            set => Write(0x8C, value);
        }

        public float UnknownFloat90
        {
            get => ReadFloat(0x90);
            set => Write(0x90, value);
        }

        public float UnknownFloat94
        {
            get => ReadFloat(0x94);
            set => Write(0x94, value);
        }

        public float UnknownFloat98
        {
            get => ReadFloat(0x98);
            set => Write(0x98, value);
        }
    }
}
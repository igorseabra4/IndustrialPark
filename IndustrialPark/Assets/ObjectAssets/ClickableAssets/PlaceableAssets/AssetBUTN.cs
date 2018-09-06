using HipHopFile;

namespace IndustrialPark
{
    public class AssetBUTN : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        protected override int getEventStartOffset()
        {
            return 0x9C + Offset;
        }

        public AssetBUTN(Section_AHDR AHDR) : base(AHDR) { }
        
        public AssetID UnknownAssetID
        {
            get { return ReadUInt(0x54 + Offset); }
            set { Write(0x54 + Offset, value); }
        }

        public int MaybeType
        {
            get { return ReadInt(0x58 + Offset); }
            set { Write(0x58 + Offset, value); }
        }

        public int Unknown1
        {
            get { return ReadInt(0x60 + Offset); }
            set { Write(0x60 + Offset, value); }
        }

        public float UnknownFloat1
        {
            get { return ReadFloat(0x64 + Offset); }
            set { Write(0x64 + Offset, value); }
        }

        public int Unknown2
        {
            get { return ReadInt(0x68 + Offset); }
            set { Write(0x68 + Offset, value); }
        }

        public byte UnknownByte1
        {
            get { return ReadByte(0x6C + Offset); }
            set { Write(0x6C + Offset, value); }
        }

        public byte UnknownByte2
        {
            get { return ReadByte(0x6D + Offset); }
            set { Write(0x6D + Offset, value); }
        }

        public byte UnknownByte3
        {
            get { return ReadByte(0x6E + Offset); }
            set { Write(0x6E + Offset, value); }
        }

        public byte UnknownByte4
        {
            get { return ReadByte(0x6F + Offset); }
            set { Write(0x6F + Offset, value); }
        }

        public byte UnknownByte5
        {
            get { return ReadByte(0x70 + Offset); }
            set { Write(0x70 + Offset, value); }
        }

        public byte UnknownByte6
        {
            get { return ReadByte(0x71 + Offset); }
            set { Write(0x71 + Offset, value); }
        }

        public byte UnknownByte7
        {
            get { return ReadByte(0x72 + Offset); }
            set { Write(0x72 + Offset, value); }
        }

        public byte UnknownByte8
        {
            get { return ReadByte(0x73 + Offset); }
            set { Write(0x73 + Offset, value); }
        }

        public float UnknownFloat2
        {
            get { return ReadFloat(0x74); }
            set { Write(0x74, value); }
        }

        public float UnknownFloat3
        {
            get { return ReadFloat(0x78 + Offset); }
            set { Write(0x78 + Offset, value); }
        }

        public int UnknownInt1
        {
            get { return ReadInt(0x7C + Offset); }
            set { Write(0x7C + Offset, value); }
        }

        public int UnknownInt2
        {
            get { return ReadInt(0x80 + Offset); }
            set { Write(0x80 + Offset, value); }
        }

        public int UnknownInt3
        {
            get { return ReadInt(0x84 + Offset); }
            set { Write(0x84 + Offset, value); }
        }

        public int UnknownInt4
        {
            get { return ReadInt(0x88 + Offset); }
            set { Write(0x88 + Offset, value); }
        }

        public int UnknownInt5
        {
            get { return ReadInt(0x8C + Offset); }
            set { Write(0x8C + Offset, value); }
        }

        public int UnknownInt6
        {
            get { return ReadInt(0x90 + Offset); }
            set { Write(0x90 + Offset, value); }
        }

        public int UnknownInt7
        {
            get { return ReadInt(0x94 + Offset); }
            set { Write(0x94 + Offset, value); }
        }

        public int UnknownInt8
        {
            get { return ReadInt(0x98 + Offset); }
            set { Write(0x98 + Offset, value); }
        }
    }
}
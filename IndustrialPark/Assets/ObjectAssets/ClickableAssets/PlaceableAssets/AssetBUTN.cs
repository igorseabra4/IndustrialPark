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

        public AssetBUTN(Section_AHDR AHDR) : base(AHDR) { }
        
        public AssetID UnknownAssetID
        {
            get { return ReadUInt(0x54); }
            set { Write(0x54, value); }
        }

        public int MaybeType
        {
            get { return ReadInt(0x58); }
            set { Write(0x58, value); }
        }

        public int Unknown1
        {
            get { return ReadInt(0x60); }
            set { Write(0x60, value); }
        }

        public float UnknownFloat1
        {
            get { return ReadFloat(0x64); }
            set { Write(0x64, value); }
        }

        public int Unknown2
        {
            get { return ReadInt(0x68); }
            set { Write(0x68, value); }
        }

        public byte UnknownByte1
        {
            get { return ReadByte(0x6C); }
            set { Write(0x6C, value); }
        }

        public byte UnknownByte2
        {
            get { return ReadByte(0x6D); }
            set { Write(0x6D, value); }
        }

        public byte UnknownByte3
        {
            get { return ReadByte(0x6E); }
            set { Write(0x6E, value); }
        }

        public byte UnknownByte4
        {
            get { return ReadByte(0x6F); }
            set { Write(0x6F, value); }
        }

        public byte UnknownByte5
        {
            get { return ReadByte(0x70); }
            set { Write(0x70, value); }
        }

        public byte UnknownByte6
        {
            get { return ReadByte(0x71); }
            set { Write(0x71, value); }
        }

        public byte UnknownByte7
        {
            get { return ReadByte(0x72); }
            set { Write(0x72, value); }
        }

        public byte UnknownByte8
        {
            get { return ReadByte(0x73); }
            set { Write(0x73, value); }
        }

        public float UnknownFloat2
        {
            get { return ReadFloat(0x74); }
            set { Write(0x74, value); }
        }

        public float UnknownFloat3
        {
            get { return ReadFloat(0x78); }
            set { Write(0x78, value); }
        }

        public AssetEvent[] Events
        {
            get { return ReadEvents(0x9C); }
            set { WriteEvents(0x9C, value); }
        }
    }
}
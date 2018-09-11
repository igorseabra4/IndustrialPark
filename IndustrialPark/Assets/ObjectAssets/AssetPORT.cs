using HipHopFile;

namespace IndustrialPark
{
    public class AssetPORT : ObjectAsset
    {
        public AssetPORT(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x18;
        }

        public AssetID Camera_Unknown
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        public AssetID Destination_MRKR
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        public float Rotation
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        public string Destination_Level
        {
            get => ReadString(0x14, 4);
            set => Write(0x14, value, 4);
        }
    }
}
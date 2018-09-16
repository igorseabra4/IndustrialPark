using HipHopFile;

namespace IndustrialPark
{
    public class AssetSFX : ObjectAsset
    {
        public AssetSFX(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x30;
        }

        public short Short08
        {
            get => ReadShort(0x8);
            set => Write(0x8, value);
        }

        public short Short0A
        {
            get => ReadShort(0xA);
            set => Write(0xA, value);
        }

        public float UnknownFloat0C
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }

        public AssetID SoundAssetID
        {
            get => ReadUInt(0x10);
            set => Write(0x10, value);
        }

        public float UnknownFloat14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        public int UnknownInt18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        public float UnknownFloat1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        public float UnknownFloat20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }

        public float UnknownFloat24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        public float UnknownFloat28
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }

        public float UnknownFloat2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
    }
}
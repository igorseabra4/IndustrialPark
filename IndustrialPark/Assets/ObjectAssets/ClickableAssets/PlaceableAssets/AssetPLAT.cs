using HipHopFile;

namespace IndustrialPark
{
    public class AssetPLAT : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }

        protected override int EventStartOffset { get => 0xC0 + Offset; }

        public AssetPLAT(Section_AHDR AHDR) : base(AHDR) { }

        public byte PlatformType
        {
            get => ReadByte(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        public byte UnknownByte_55
        {
            get => ReadByte(0x55 + Offset);
            set => Write(0x55 + Offset, value);
        }

        public short CollisionType
        {
            get => ReadShort(0x56 + Offset);
            set => Write(0x56 + Offset, value);
        }

        public float UnknownFloat_58
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        public float UnknownFloat_5C
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        public float UnknownFloat_60
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        public float UnknownFloat_64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        public float UnknownFloat_68
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        public float UnknownFloat_6C
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        public float UnknownFloat_70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        public float UnknownFloat_74
        {
            get => ReadFloat(0x74 + Offset);
            set => Write(0x74 + Offset, value);
        }

        public float UnknownFloat_78
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        public float UnknownFloat_7C
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        public float UnknownFloat_80
        {
            get => ReadFloat(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        public float UnknownFloat_84
        {
            get => ReadFloat(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        public float UnknownFloat_88
        {
            get => ReadFloat(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        public float UnknownFloat_8C
        {
            get => ReadFloat(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        public byte UnknownByte_90
        {
            get => ReadByte(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        public byte UnknownByte_91
        {
            get => ReadByte(0x91 + Offset);
            set => Write(0x91 + Offset, value);
        }

        public byte UnknownByte_92
        {
            get => ReadByte(0x92 + Offset);
            set => Write(0x92 + Offset, value);
        }

        public byte UnknownByte_93
        {
            get => ReadByte(0x93 + Offset);
            set => Write(0x93 + Offset, value);
        }

        public byte UnknownByte_94
        {
            get => ReadByte(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        public byte UnknownByte_95
        {
            get => ReadByte(0x95 + Offset);
            set => Write(0x95 + Offset, value);
        }

        public byte UnknownByte_96
        {
            get => ReadByte(0x96 + Offset);
            set => Write(0x96 + Offset, value);
        }

        public byte UnknownByte_97
        {
            get => ReadByte(0x97 + Offset);
            set => Write(0x97 + Offset, value);
        }

        public float MovementTranslationDistance
        {
            get => ReadFloat(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }

        public float MovementTranslationTime
        {
            get => ReadFloat(0x9C + Offset);
            set => Write(0x9C + Offset, value);
        }

        public float UnknownFloat_A0
        {
            get => ReadFloat(0xA0 + Offset);
            set => Write(0xA0 + Offset, value);
        }

        public float UnknownFloat_A4
        {
            get => ReadFloat(0xA4 + Offset);
            set => Write(0xA4 + Offset, value);
        }

        public float MovementRotationDegrees
        {
            get => ReadFloat(0xA8 + Offset);
            set => Write(0xA8 + Offset, value);
        }

        public float MovementRotationTime
        {
            get => ReadFloat(0xAC + Offset);
            set => Write(0xAC + Offset, value);
        }

        public float UnknownFloat_B0
        {
            get => ReadFloat(0xB0 + Offset);
            set => Write(0xB0 + Offset, value);
        }

        public float UnknownFloat_B4
        {
            get => ReadFloat(0xB4 + Offset);
            set => Write(0xB4 + Offset, value);
        }

        public float MovementEndPointTime
        {
            get => ReadFloat(0xB8 + Offset);
            set => Write(0xB8 + Offset, value);
        }

        public float MovementStartPointTime
        {
            get => ReadFloat(0xBC + Offset);
            set => Write(0xBC + Offset, value);
        }
    }
}
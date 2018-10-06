using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetBUTN : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender { get => dontRender; }

        protected override int EventStartOffset { get => 0x9C + Offset; }

        public AssetBUTN(Section_AHDR AHDR) : base(AHDR) { }

        [Category("Button")]
        public AssetID PressedModelAssetID
        {
            get => ReadUInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Button")]
        public int ButtonType
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt5C
        {
            get => ReadInt(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Button")]
        public bool HoldEnabled
        {
            get => ReadInt(0x60 + Offset) != 0;
            set => Write(0x60 + Offset, value ? 1 : 0);
        }

        [Category("Button"), TypeConverter(typeof(FloatTypeConverter))]
        public float HoldTime
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Button")]
        public int HitMask
        {
            get => ReadInt(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6C
        {
            get => ReadByte(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6D
        {
            get => ReadByte(0x6D + Offset);
            set => Write(0x6D + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6E
        {
            get => ReadByte(0x6E + Offset);
            set => Write(0x6E + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte6F
        {
            get => ReadByte(0x6F + Offset);
            set => Write(0x6F + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte70
        {
            get => ReadByte(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte71
        {
            get => ReadByte(0x71 + Offset);
            set => Write(0x71 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte72
        {
            get => ReadByte(0x72 + Offset);
            set => Write(0x72 + Offset, value);
        }

        [Category("Button")]
        public byte UnknownByte73
        {
            get => ReadByte(0x73 + Offset);
            set => Write(0x73 + Offset, value);
        }

        [Category("Button")]
        public float PressedOffset
        {
            get => ReadFloat(0x74);
            set => Write(0x74, value);
        }

        [Category("Button")]
        public float TransitionTime
        {
            get => ReadFloat(0x78 + Offset);
            set => Write(0x78 + Offset, value);
        }

        [Category("Button")]
        public float TransitionEaseIn
        {
            get => ReadFloat(0x7C + Offset);
            set => Write(0x7C + Offset, value);
        }

        [Category("Button")]
        public float TransitionEaseOut
        {
            get => ReadFloat(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt84
        {
            get => ReadInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt88
        {
            get => ReadInt(0x88 + Offset);
            set => Write(0x88 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt8C
        {
            get => ReadInt(0x8C + Offset);
            set => Write(0x8C + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt90
        {
            get => ReadInt(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt94
        {
            get => ReadInt(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("Button")]
        public int UnknownInt98
        {
            get => ReadInt(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }
    }
}
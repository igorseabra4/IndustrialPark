using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : EntityAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x60 + Offset;

        public AssetSIMP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        private const string categoryName = "Simple Object";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float AnimSpeed
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category(categoryName)]
        public int InitialAnimState
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor CollType => ByteFlagsDescriptor(0x5C + Offset,
            null, "Solid", null, null, null, null, null, "Ledge Grab");

        [Browsable(false)]
        public byte CollTypeByte
        {
            get => ReadByte(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName)]
        public DynamicTypeDescriptor SimpFlags => ByteFlagsDescriptor(0x5D + Offset);
        
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding5E
        {
            get => ReadByte(0x5E + Offset);
            set => Write(0x5E + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding5F
        {
            get => ReadByte(0x5F + Offset);
            set => Write(0x5F + Offset, value);
        }
    }
}
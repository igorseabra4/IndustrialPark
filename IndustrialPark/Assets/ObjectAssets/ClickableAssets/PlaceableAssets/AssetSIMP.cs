using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x60 + Offset;

        public AssetSIMP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }
        
        [Category("Simple Object"), TypeConverter(typeof(FloatTypeConverter))]
        public float AnimSpeed
        {
            get => ReadFloat(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Simple Object")]
        public int InitialAnimState
        {
            get => ReadInt(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Simple Object"), TypeConverter(typeof(HexByteTypeConverter)), Description("02 = Solid\n82 = Ledge Grab")]
        public byte CollType
        {
            get => ReadByte(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Simple Object"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte SimpFlags
        {
            get => ReadByte(0x5D + Offset);
            set => Write(0x5D + Offset, value);
        }

        [Category("Simple Object"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding5E
        {
            get => ReadByte(0x5E + Offset);
            set => Write(0x5E + Offset, value);
        }

        [Category("Simple Object"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Padding5F
        {
            get => ReadByte(0x5F + Offset);
            set => Write(0x5F + Offset, value);
        }
    }
}
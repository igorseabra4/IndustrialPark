using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetSIMP : EntityAsset
    {
        public static bool dontRender = false;

        public override bool DontRender => dontRender;

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

        [Category(categoryName), Description("Only Static is functional.")]
        public DynamicTypeDescriptor CollType => ByteFlagsDescriptor(0x5C + Offset,
            "Trigger", "Static", "Dynamic", "NPC", "Player");

        [Browsable(false)]
        public byte CollTypeByte
        {
            get => ReadByte(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName), Description("Always 0.")]
        public DynamicTypeDescriptor SimpFlags => ByteFlagsDescriptor(0x5D + Offset);
        
        [Category(categoryName), TypeConverter(typeof(HexUShortTypeConverter)), Browsable(false)]
        public short Padding5E
        {
            get => ReadShort(0x5E + Offset);
            set => Write(0x5E + Offset, value);
        }
    }
}
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetHANG : EntityAsset
    {
        public static bool dontRender = false;

        public override bool DontRender => dontRender;
        
        public AssetHANG(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        protected override int EventStartOffset => 0x74 + Offset;

        protected const string categoryName = "Hangable";

        [Category(categoryName)]
        public DynamicTypeDescriptor HangFlags => IntFlagsDescriptor(0x54 + Offset);

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float PivotOffset
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float LeverArm
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Gravity
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Acceleration
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Decay
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float GrabDelay
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float StopDeceleration
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }
    }
}
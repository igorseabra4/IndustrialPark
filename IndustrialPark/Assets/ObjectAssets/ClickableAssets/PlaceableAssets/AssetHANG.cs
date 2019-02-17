using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetHANG : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;
        
        public AssetHANG(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x74 + Offset;

        [Category("Hangable")]
        public int HangFlags
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float PivotOffset
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float LeverArm
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float Gravity
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float Acceleration
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float Decay
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float GrabDelay
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float StopDeceleration
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }
    }
}
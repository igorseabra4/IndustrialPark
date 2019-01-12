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
        public int UnknownInt54
        {
            get => ReadInt(0x54 + Offset);
            set => Write(0x54 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat58
        {
            get => ReadFloat(0x58 + Offset);
            set => Write(0x58 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5C
        {
            get => ReadFloat(0x5C + Offset);
            set => Write(0x5C + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat60
        {
            get => ReadFloat(0x60 + Offset);
            set => Write(0x60 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat64
        {
            get => ReadFloat(0x64 + Offset);
            set => Write(0x64 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68
        {
            get => ReadFloat(0x68 + Offset);
            set => Write(0x68 + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6C
        {
            get => ReadFloat(0x6C + Offset);
            set => Write(0x6C + Offset, value);
        }

        [Category("Hangable"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat70
        {
            get => ReadFloat(0x70 + Offset);
            set => Write(0x70 + Offset, value);
        }
    }
}
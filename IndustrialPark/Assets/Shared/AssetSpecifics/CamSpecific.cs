using System.ComponentModel;

namespace IndustrialPark
{
    public class CamSpecific_Generic : AssetSpecific_Generic
    {
        public CamSpecific_Generic(AssetCAM asset) : base(asset, 0x60) { }
    }

    public class CamSpecific_Follow : CamSpecific_Generic
    {
        public CamSpecific_Follow(AssetCAM asset) : base(asset) { }

        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Rotation
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Distance
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float Height
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float RubberBand
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float StartSpeed
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [Category("Follow"), TypeConverter(typeof(FloatTypeConverter))]
        public float EndSpeed
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
    }

    public class CamSpecific_Shoulder : CamSpecific_Generic
    {
        public CamSpecific_Shoulder(AssetCAM asset) : base(asset) { }

        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Distance
        {
            get => ReadFloat(0x0);
            set => Write(0x0, value);
        }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float Height
        {
            get => ReadFloat(0x4);
            set => Write(0x4, value);
        }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RealignSpeed
        {
            get => ReadFloat(0x8);
            set => Write(0x8, value);
        }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RealignDelay
        {
            get => ReadFloat(0xC);
            set => Write(0xC, value);
        }
    }

    public class CamSpecific_Static : CamSpecific_Generic
    {
        public CamSpecific_Static(AssetCAM asset) : base(asset) { }

        [Category("Static")]
        public int Unused
        {
            get => ReadInt(0);
            set => Write(0, value);
        }
    }

    public class CamSpecific_Path : CamSpecific_Generic
    {
        public CamSpecific_Path(AssetCAM asset) : base(asset) { }

        [Category("Shoulder")]
        public AssetID Unknown_AssetID
        {
            get => ReadUInt(0);
            set => Write(0, value);
        }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float TimeEnd
        {
            get => ReadFloat(4);
            set => Write(4, value);
        }
        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float TimeDelay
        {
            get => ReadFloat(8);
            set => Write(8, value);
        }
    }

    public class CamSpecific_StaticFollow : CamSpecific_Generic
    {
        public CamSpecific_StaticFollow(AssetCAM asset) : base(asset) { }

        [Category("Shoulder"), TypeConverter(typeof(FloatTypeConverter))]
        public float RubberBand
        {
            get => ReadFloat(0);
            set => Write(0, value);
        }
    }
}

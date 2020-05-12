using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaRumble : DynaBase
    {
        public string Note => "Version is always 3";

        public override int StructSize => 0x28;

        public DynaRumble(AssetDYNA asset) : base(asset) { }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_00
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_04
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_08
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        public short UnknownShort_0C
        {
            get => ReadShort(0x0C);
            set => Write(0x0C, value);
        }
        public short UnknownShort_0A
        {
            get => ReadShort(0x0A);
            set => Write(0x0A, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_14
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_1C
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_20
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_24
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
    }
}
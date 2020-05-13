using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSpotlight : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x38;

        public DynaEffectSpotlight(AssetDYNA asset) : base(asset) { }

        public int UnknownInt_00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        public AssetID Origin_Entity_AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }
        public AssetID Target_Entity_AssetID
        {
            get => ReadUInt(0x08);
            set => Write(0x08, value);
        }
        public int UnknownInt_0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
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
            get => MathUtil.RadiansToDegrees(ReadFloat(0x14));
            set => Write(0x14, MathUtil.DegreesToRadians(value));
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_18
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_1C 
        {
            get => ReadByte(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_1D
        {
            get => ReadByte(0x1D);
            set => Write(0x1D, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_1E
        {
            get => ReadByte(0x1E);
            set => Write(0x1E, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_1F
        {
            get => ReadByte(0x1F);
            set => Write(0x1F, value);
        }
        public int UnknownInt_20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
        public int UnknownInt_24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }
        public int UnknownInt_28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_2C
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_30
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_34
        {
            get => ReadByte(0x34);
            set => Write(0x34, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_35
        {
            get => ReadByte(0x35);
            set => Write(0x35, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_36
        {
            get => ReadByte(0x36);
            set => Write(0x36, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte_37
        {
            get => ReadByte(0x37);
            set => Write(0x37, value);
        }
    }
}
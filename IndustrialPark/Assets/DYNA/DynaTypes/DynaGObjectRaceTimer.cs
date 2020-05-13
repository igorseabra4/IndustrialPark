using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectRaceTimer : DynaBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x18;

        public DynaGObjectRaceTimer(AssetDYNA asset) : base(asset) { }
        
        public int UnknownInt_00
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        public int UnknownInt_04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        public int UnknownInt_08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat_0C
        {
            get => ReadFloat(0x0C);
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
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
    }
}
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaCamTweak : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x10;

        public DynaCamTweak(AssetDYNA asset) : base(asset) { }
        
        public int Priority
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float Time
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float PitchAdjust
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DistAdjust
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
    }
}
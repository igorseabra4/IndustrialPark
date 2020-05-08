using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class DynaHud : DynaBase
    {
        public DynaHud(AssetDYNA asset) : base(asset) { }
        
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX
        {
            get => ReadFloat(0x00);
            set => Write(0x00, value);
        }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleX
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleY
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleZ
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }
    }
}
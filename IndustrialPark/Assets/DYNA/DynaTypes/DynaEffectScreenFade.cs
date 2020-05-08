using AssetEditorColors;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x10;

        public DynaEffectScreenFade(AssetDYNA asset) : base(asset) { }
                
        private byte ColorR
        {
            get => ReadByte(0x00);
            set => Write(0x00, value);
        }
        private byte ColorG
        {
            get => ReadByte(0x01);
            set => Write(0x01, value);
        }
        private byte ColorB
        {
            get => ReadByte(0x02);
            set => Write(0x02, value);
        }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor Color
        {
            get => new MyColor(ColorR, ColorG, ColorB, ColorAlpha);
            set
            {
                ColorR = value.R;
                ColorG = value.G;
                ColorB = value.B;
            }
        }
        [DisplayName("Color 1 Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => ReadByte(0x03);
            set => Write(0x03, value);
        }

        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
    }
}
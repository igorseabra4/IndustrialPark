using AssetEditorColors;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class DynaHudMeterFont : DynaHudMeter
    {
        public string Note => "Version is always 2 or 3. Version 2 does not use any of the flags.";

        public override int StructSize => version == 3 ? 0x64 : 0x60;

        private readonly int version;
        public DynaHudMeterFont(AssetDYNA asset, int version) : base(asset)
        {
            this.version = version;
        }
                
        public FontEnum Font
        {
            get => (FontEnum)ReadInt(0x3C);
            set => Write(0x3C, (int)value);
        }
        public int FontJustify
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontWidth
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontHeight
        {
            get => ReadFloat(0x48);
            set => Write(0x48, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontSpace
        {
            get => ReadFloat(0x4C);
            set => Write(0x4C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ShadowXOffset
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ShadowYOffset
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        private byte ColorR
        {
            get => ReadByte(0x58);
            set => Write(0x58, value);
        }
        private byte ColorG
        {
            get => ReadByte(0x59);
            set => Write(0x59, value);
        }
        private byte ColorB
        {
            get => ReadByte(0x5A);
            set => Write(0x5A, value);
        }
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color (R, G, B)")]
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
        [DisplayName("Color Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => ReadByte(0x5B);
            set => Write(0x5B, value);
        }
        private byte ShadowColorR
        {
            get => ReadByte(0x5C);
            set => Write(0x5C, value);
        }
        private byte ShadowColorG
        {
            get => ReadByte(0x5D);
            set => Write(0x5D, value);
        }
        private byte ShadowColorB
        {
            get => ReadByte(0x5E);
            set => Write(0x5E, value);
        }
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Shadow Color (R, G, B)")]
        public MyColor ShadowColor
        {
            get => new MyColor(ShadowColorR, ShadowColorG, ShadowColorB, ShadowColorAlpha);
            set
            {
                ShadowColorR = value.R;
                ShadowColorG = value.G;
                ShadowColorB = value.B;
            }
        }
        [DisplayName("Shadow Color Alpha (0 - 255)")]
        public byte ShadowColorAlpha
        {
            get => ReadByte(0x5F);
            set => Write(0x5F, value);
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte CounterModeFlag
        {
            get => version == 3 ? ReadByte(0x60) : (byte)0;
            set 
            {
                if (version == 3)
                    Write(0x60, value);
            }
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2
        {
            get => version == 3 ? ReadByte(0x61) : (byte)0;
            set
            {
                if (version == 3)
                    Write(0x61, value);
            }
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3
        {
            get => version == 3 ? ReadByte(0x62) : (byte)0;
            set
            {
                if (version == 3)
                    Write(0x62, value);
            }
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4
        {
            get => version == 3 ? ReadByte(0x63) : (byte)0;
            set
            {
                if (version == 3)
                    Write(0x63, value);
            }
        }
    }
}
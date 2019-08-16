using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaHudMeterFontV3 : DynaHudMeterBase
    {
        public DynaHudMeterFontV3(Platform platform) : base(platform)
        {
        }
        
        public DynaHudMeterFontV3(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            Font = (FontEnum)Switch(BitConverter.ToInt32(Data, 0x3C));
            FontJustify = Switch(BitConverter.ToInt32(Data, 0x40));
            FontWidth = Switch(BitConverter.ToSingle(Data, 0x44));
            FontHeight = Switch(BitConverter.ToSingle(Data, 0x48));
            FontSpace = Switch(BitConverter.ToSingle(Data, 0x4C));
            ShadowXOffset = Switch(BitConverter.ToSingle(Data, 0x50));
            ShadowYOffset = Switch(BitConverter.ToSingle(Data, 0x54));
            ColorR = Data[0x58];
            ColorG = Data[0x59];
            ColorB = Data[0x5A];
            ColorAlpha = Data[0x5B];
            ShadowColorR = Data[0x5C];
            ShadowColorG = Data[0x5D];
            ShadowColorB = Data[0x5E];
            ShadowColorAlpha = Data[0x5F];
            CounterModeFlag = Data[0x60];
            Flag2 = Data[0x61];
            Flag3 = Data[0x62];
            Flag4 = Data[0x63];
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch((int)Font)));
            list.AddRange(BitConverter.GetBytes(Switch(FontJustify)));
            list.AddRange(BitConverter.GetBytes(Switch(FontWidth)));
            list.AddRange(BitConverter.GetBytes(Switch(FontHeight)));
            list.AddRange(BitConverter.GetBytes(Switch(FontSpace)));
            list.AddRange(BitConverter.GetBytes(Switch(ShadowXOffset)));
            list.AddRange(BitConverter.GetBytes(Switch(ShadowYOffset)));
            list.Add(ColorR);
            list.Add(ColorG);
            list.Add(ColorB);
            list.Add(ColorAlpha);
            list.Add(ShadowColorR);
            list.Add(ShadowColorG);
            list.Add(ShadowColorB);
            list.Add(ShadowColorAlpha);
            list.Add(CounterModeFlag);
            list.Add(Flag2);
            list.Add(Flag3);
            list.Add(Flag4);

            return list.ToArray();
        }
        public FontEnum Font { get; set; }
        public int FontJustify { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontWidth { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontHeight { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float FontSpace { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ShadowXOffset { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float ShadowYOffset { get; set; }
        private byte ColorR;
        private byte ColorG;
        private byte ColorB;
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
        public byte ColorAlpha { get; set; }
        private byte ShadowColorR;
        private byte ShadowColorG;
        private byte ShadowColorB;
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
        public byte ShadowColorAlpha { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte CounterModeFlag { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4 { get; set; }
    }
}
using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudMeterFontV3 : DynaBase
    {
        public override string Note => "Version is always 3";

        public DynaHudMeterFontV3() : base()
        {
            SoundID1 = 0;
            SoundID2 = 0;
            SoundID3 = 0;
            SoundID4 = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (SoundID1 == assetID)
                return true;
            if (SoundID2 == assetID)
                return true;
            if (SoundID3 == assetID)
                return true;
            if (SoundID4 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaHudMeterFontV3(IEnumerable<byte> enumerable) : base (enumerable)
        {
            PositionX = Switch(BitConverter.ToSingle(Data, 0x0));
            PositionY = Switch(BitConverter.ToSingle(Data, 0x4));
            PositionZ = Switch(BitConverter.ToSingle(Data, 0x8));
            TextWidth = Switch(BitConverter.ToSingle(Data, 0xC));
            TextHeight = Switch(BitConverter.ToSingle(Data, 0x10));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x14));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x18));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat4 = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownFloat5 = Switch(BitConverter.ToSingle(Data, 0x24));
            UnknownFloat6 = Switch(BitConverter.ToSingle(Data, 0x28));
            SoundID1 = Switch(BitConverter.ToUInt32(Data, 0x2C));
            SoundID2 = Switch(BitConverter.ToUInt32(Data, 0x30));
            SoundID3 = Switch(BitConverter.ToUInt32(Data, 0x34));
            SoundID4 = Switch(BitConverter.ToUInt32(Data, 0x38));
            Font = Switch(BitConverter.ToInt32(Data, 0x3C));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x40));
            UnknownFloat7 = Switch(BitConverter.ToSingle(Data, 0x44));
            UnknownFloat8 = Switch(BitConverter.ToSingle(Data, 0x48));
            UnknownFloat9 = Switch(BitConverter.ToSingle(Data, 0x4C));
            ShadowXOffset = Switch(BitConverter.ToSingle(Data, 0x50));
            ShadowYOffset = Switch(BitConverter.ToSingle(Data, 0x54));
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x58)));
            ColorR = abgr[3];
            ColorG = abgr[2];
            ColorB = abgr[1];
            ColorAlpha = abgr[0];
            byte[] abgr2 = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x5C)));
            ShadowColorR = abgr2[3];
            ShadowColorG = abgr2[2];
            ShadowColorB = abgr2[1];
            ShadowColorAlpha = abgr2[0];
            Flag1 = Data[0x60];
            Flag2 = Data[0x61];
            Flag3 = Data[0x62];
            Flag4 = Data[0x63];
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(PositionX)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionY)));
            list.AddRange(BitConverter.GetBytes(Switch(PositionZ)));
            list.AddRange(BitConverter.GetBytes(Switch(TextWidth)));
            list.AddRange(BitConverter.GetBytes(Switch(TextHeight)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat6)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID1)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID2)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID3)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID4)));
            list.AddRange(BitConverter.GetBytes(Switch(Font)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat7)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat8)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat9)));
            list.AddRange(BitConverter.GetBytes(Switch(ShadowXOffset)));
            list.AddRange(BitConverter.GetBytes(Switch(ShadowYOffset)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToSingle(new byte[] { ColorAlpha, ColorB, ColorG, ColorR }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToSingle(new byte[] { ShadowColorAlpha, ShadowColorB, ShadowColorG, ShadowColorR }, 0))));
            list.Add(Flag1);
            list.Add(Flag2);
            list.Add(Flag3);
            list.Add(Flag4);

            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TextWidth { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float TextHeight { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat4 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6 { get; set; }
        public AssetID SoundID1 { get; set; }
        public AssetID SoundID2 { get; set; }
        public AssetID SoundID3 { get; set; }
        public AssetID SoundID4 { get; set; }
        public int Font { get; set; }
        public int UnknownInt1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat7 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat9 { get; set; }
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
        public byte Flag1 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag2 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag3 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Flag4 { get; set; }
    }
}
﻿using AssetEditorColors;
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
            Unknown3 = 0;
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
            if (Unknown3 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaHudMeterFontV3(IEnumerable<byte> enumerable) : base (enumerable)
        {
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x0));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x4));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0x8));
            UnknownFloat4 = Switch(BitConverter.ToSingle(Data, 0xC));
            UnknownFloat5 = Switch(BitConverter.ToSingle(Data, 0x10));
            UnknownFloat6 = Switch(BitConverter.ToSingle(Data, 0x14));
            UnknownFloat7 = Switch(BitConverter.ToSingle(Data, 0x18));
            UnknownFloat8 = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownFloat9 = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownFloat10 = Switch(BitConverter.ToSingle(Data, 0x24));
            UnknownFloat11 = Switch(BitConverter.ToSingle(Data, 0x28));
            SoundID1 = Switch(BitConverter.ToUInt32(Data, 0x2C));
            SoundID2 = Switch(BitConverter.ToUInt32(Data, 0x30));
            SoundID3 = Switch(BitConverter.ToUInt32(Data, 0x34));
            SoundID4 = Switch(BitConverter.ToUInt32(Data, 0x38));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x3C));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x40));
            UnknownFloat12 = Switch(BitConverter.ToSingle(Data, 0x44));
            UnknownFloat13 = Switch(BitConverter.ToSingle(Data, 0x48));
            UnknownFloat14 = Switch(BitConverter.ToSingle(Data, 0x4C));
            UnknownFloat15 = Switch(BitConverter.ToSingle(Data, 0x50));
            UnknownFloat16 = Switch(BitConverter.ToSingle(Data, 0x54));
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToSingle(Data, 0x58)));
            Color1R = abgr[3];
            Color1G = abgr[2];
            Color1B = abgr[1];
            ColorAlpha = abgr[0];
            Unknown3 = Switch(BitConverter.ToUInt32(Data, 0x5C));
            Flag1 = Data[0x60];
            Flag2 = Data[0x61];
            Flag3 = Data[0x62];
            Flag4 = Data[0x63];
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat6)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat7)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat8)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat9)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat10)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat11)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID1)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID2)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID3)));
            list.AddRange(BitConverter.GetBytes(Switch(SoundID4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat12)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat13)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat14)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat15)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat16)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToSingle(new byte[] { ColorAlpha, Color1B, Color1G, Color1R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown3)));
            list.Add(Flag1);
            list.Add(Flag2);
            list.Add(Flag3);
            list.Add(Flag4);

            return list.ToArray();
        }

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
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat7 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat9 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat11 { get; set; }
        public AssetID SoundID1 { get; set; }
        public AssetID SoundID2 { get; set; }
        public AssetID SoundID3 { get; set; }
        public AssetID SoundID4 { get; set; }
        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat12 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat13 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat14 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat15 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat16 { get; set; }
        private byte Color1R;
        private byte Color1G;
        private byte Color1B;
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor Color
        {
            get => new MyColor(Color1R, Color1G, Color1B, ColorAlpha);
            set
            {
                Color1R = value.R;
                Color1G = value.G;
                Color1B = value.B;
            }
        }
        [DisplayName("Color 1 Alpha (0 - 255)")]
        private byte ColorAlpha { get; set; }
        public AssetID Unknown3 { get; set; }
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
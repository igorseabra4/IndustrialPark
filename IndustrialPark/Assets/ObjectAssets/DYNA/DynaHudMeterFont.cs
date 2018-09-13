using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudMeterFont : DynaBase
    {
        public override string Note => "Version is always 3";

        public DynaHudMeterFont() : base()
        {
            SoundID1 = 0;
            SoundID2 = 0;
            SoundID3 = 0;
            SoundID4 = 0;
            Unknown3 = 0;
        }

        public DynaHudMeterFont(IEnumerable<byte> enumerable) : base (enumerable)
        {
            UnknownFloat1 = Switch(BitConverter.ToSingle(data, 0x0));
            UnknownFloat2 = Switch(BitConverter.ToSingle(data, 0x4));
            UnknownFloat3 = Switch(BitConverter.ToSingle(data, 0x8));
            UnknownFloat4 = Switch(BitConverter.ToSingle(data, 0xC));
            UnknownFloat5 = Switch(BitConverter.ToSingle(data, 0x10));
            UnknownFloat6 = Switch(BitConverter.ToSingle(data, 0x14));
            UnknownFloat7 = Switch(BitConverter.ToSingle(data, 0x18));
            UnknownFloat8 = Switch(BitConverter.ToSingle(data, 0x1C));
            UnknownFloat9 = Switch(BitConverter.ToSingle(data, 0x20));
            UnknownFloat10 = Switch(BitConverter.ToSingle(data, 0x24));
            UnknownFloat11 = Switch(BitConverter.ToSingle(data, 0x28));
            SoundID1 = Switch(BitConverter.ToUInt32(data, 0x2C));
            SoundID2 = Switch(BitConverter.ToUInt32(data, 0x30));
            SoundID3 = Switch(BitConverter.ToUInt32(data, 0x34));
            SoundID4 = Switch(BitConverter.ToUInt32(data, 0x38));
            UnknownInt1 = Switch(BitConverter.ToInt32(data, 0x3C));
            UnknownInt2 = Switch(BitConverter.ToInt32(data, 0x40));
            UnknownFloat12 = Switch(BitConverter.ToSingle(data, 0x44));
            UnknownFloat13 = Switch(BitConverter.ToSingle(data, 0x48));
            UnknownFloat14 = Switch(BitConverter.ToSingle(data, 0x4C));
            UnknownFloat15 = Switch(BitConverter.ToSingle(data, 0x50));
            UnknownFloat16 = Switch(BitConverter.ToSingle(data, 0x54));
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToSingle(data, 0x58)));
            Color1R = abgr[3];
            Color1G = abgr[2];
            Color1B = abgr[1];
            Color1Alpha = abgr[0];
            Unknown3 = Switch(BitConverter.ToUInt32(data, 0x5C));
            Flag1 = data[0x60];
            Flag2 = data[0x61];
            Flag3 = data[0x62];
            Flag4 = data[0x63];
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
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToSingle(new byte[] { Color1Alpha, Color1B, Color1G, Color1R }, 0))));
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
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor Color1
        {
            get => new MyColor(Color1R, Color1G, Color1B, Color1Alpha);
            set
            {
                Color1R = value.R;
                Color1G = value.G;
                Color1B = value.B;
            }
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        private byte Color1Alpha { get; set; }
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
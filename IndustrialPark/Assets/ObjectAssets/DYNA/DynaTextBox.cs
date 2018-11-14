using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTextBox : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaTextBox() : base()
        {
            TextID = 0;
            TextureID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (TextID == assetID)
                return true;
            if (TextureID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public DynaTextBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            TextID = Switch(BitConverter.ToUInt32(Data, 0x00));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x04));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x08));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0x0C));
            UnknownFloat4 = Switch(BitConverter.ToSingle(Data, 0x10));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x14));
            UnknownFloat5 = Switch(BitConverter.ToSingle(Data, 0x18));
            UnknownFloat6 = Switch(BitConverter.ToSingle(Data, 0x1C));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x20));
            UnknownInt3 = Switch(BitConverter.ToInt32(Data, 0x24));
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x28)));
            Color1R = abgr[3];
            Color1G = abgr[2];
            Color1B = abgr[1];
            Color1Alpha = abgr[0];
            UnknownFloat7 = Switch(BitConverter.ToSingle(Data, 0x2C));
            UnknownFloat8 = Switch(BitConverter.ToSingle(Data, 0x30));
            UnknownFloat9 = Switch(BitConverter.ToSingle(Data, 0x34));
            UnknownFloat10 = Switch(BitConverter.ToSingle(Data, 0x38));
            UnknownInt4 = Switch(BitConverter.ToInt32(Data, 0x3C));
            UnknownInt5 = Switch(BitConverter.ToInt32(Data, 0x40));
            UnknownFloat11 = Switch(BitConverter.ToSingle(Data, 0x44));
            UnknownInt6 = Switch(BitConverter.ToInt32(Data, 0x48));
            byte[] abgr2 = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x4C)));
            Color2R = abgr2[3];
            Color2G = abgr2[2];
            Color2B = abgr2[1];
            Color2Alpha = abgr2[0];
            TextureID = Switch(BitConverter.ToUInt32(Data, 0x50));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(TextID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat6)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt3)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToInt32(new byte[] { Color1Alpha, Color1B, Color1G, Color1R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat7)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat8)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat9)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat10)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt4)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt5)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat11)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt6)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToInt32(new byte[] { Color2Alpha, Color2B, Color2G, Color2R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(TextureID)));
            return list.ToArray();
        }

        public AssetID TextID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat4 { get; set; }
        public int UnknownInt1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat6 { get; set; }
        public int UnknownInt2 { get; set; }
        public int UnknownInt3 { get; set; }
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
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat7 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat8 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat9 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10 { get; set; }
        public int UnknownInt4 { get; set; }
        public int UnknownInt5 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat11 { get; set; }
        public int UnknownInt6 { get; set; }
        private byte Color2R;
        private byte Color2G;
        private byte Color2B;
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor Color2
        {
            get => new MyColor(Color2R, Color2G, Color2B, Color2Alpha);
            set
            {
                Color2R = value.R;
                Color2G = value.G;
                Color2B = value.B;
            }
        }
        [TypeConverter(typeof(HexByteTypeConverter))]
        private byte Color2Alpha { get; set; }
        public AssetID TextureID { get; set; }
    }
}
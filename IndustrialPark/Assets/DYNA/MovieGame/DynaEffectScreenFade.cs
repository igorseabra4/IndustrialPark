using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaEffectScreenFade() : base() { }

        public DynaEffectScreenFade(IEnumerable<byte> enumerable) : base (enumerable)
        {
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x0)));
            Color1R = abgr[3];
            Color1G = abgr[2];
            Color1B = abgr[1];
            ColorAlpha = abgr[0];
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x4));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x8));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0xC));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToInt32(new byte[] { ColorAlpha, Color1B, Color1G, Color1R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            return list.ToArray();
        }
        
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
        public byte ColorAlpha { get; set; }
        
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }      
    }
}
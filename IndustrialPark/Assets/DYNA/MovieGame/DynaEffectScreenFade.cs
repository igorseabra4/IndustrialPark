using AssetEditorColors;
using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaEffectScreenFade(Platform platform) : base(platform) { }

        public DynaEffectScreenFade(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            ColorR = Data[0];
            ColorG = Data[1];
            ColorB = Data[2];
            ColorAlpha = Data[3];
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x4));
            UnknownFloat2 = Switch(BitConverter.ToSingle(Data, 0x8));
            UnknownFloat3 = Switch(BitConverter.ToSingle(Data, 0xC));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.Add(ColorR);
            list.Add(ColorG);
            list.Add(ColorB);
            list.Add(ColorAlpha);
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat3)));
            return list.ToArray();
        }
        
        private byte ColorR;
        private byte ColorG;
        private byte ColorB;

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
        public byte ColorAlpha { get; set; }
        
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat2 { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat3 { get; set; }      
    }
}
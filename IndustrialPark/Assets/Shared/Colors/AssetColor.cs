using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace AssetEditorColors
{
    [TypeConverter(typeof(AssetColorConverter)), Editor(typeof(AssetColorEditor), typeof(UITypeEditor)), Description("Hex Color (RRGGBBAA)")]
    public class AssetColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        
        public AssetColor()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 255;
        }

        public AssetColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public AssetColor(string value)
        {
            R = Convert.ToByte(value.Substring(0, 2), 16);
            G = Convert.ToByte(value.Substring(2, 2), 16);
            B = Convert.ToByte(value.Substring(4, 2), 16);
            A = Convert.ToByte(value.Substring(6, 2), 16);
        }

        public AssetColor(int ARGB)
        {
            byte[] argb = BitConverter.GetBytes(ARGB);

            A = argb[3];
            R = argb[2];
            G = argb[1];
            B = argb[0];
        }

        public int GetARGB()
        {
            return BitConverter.ToInt32(new byte[] { B, G, R, A }, 0);
        }

        public override string ToString()
        {
            return $"{R:X2}{G:X2}{B:X2}{A:X2}";
        }
    }
}

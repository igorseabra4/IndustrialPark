using SharpDX;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark.AssetEditorColors
{
    [TypeConverter(typeof(AssetColorTypeConverter)), Editor(typeof(AssetColorEditor), typeof(UITypeEditor)), Description("Hex Color (RRGGBBAA)")]
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

        public AssetColor(uint RGBA)
        {
            byte[] rgba = BitConverter.GetBytes(RGBA);
            R = rgba[3];
            G = rgba[2];
            B = rgba[1];
            A = rgba[0];
        }

        public static AssetColor FromVector4(float R, float G, float B, float A) => new AssetColor((byte)(R * 255f), (byte)(G * 255f), (byte)(B * 255f), (byte)(A * 255f));

        public static AssetColor FromVector4(Vector4 color) => FromVector4(color.X, color.Y, color.Z, color.W);

        public static implicit operator uint(AssetColor color) => (uint)(color.R << 24 | color.G << 16 | color.B << 8 | color.A);

        public static implicit operator AssetColor(RenderWareFile.Color color) => new AssetColor(color.R, color.G, color.B, color.A);

        public int GetARGB()
        {
            return BitConverter.ToInt32(new byte[] { B, G, R, A }, 0);
        }

        public override string ToString()
        {
            return $"{R:X2}{G:X2}{B:X2}{A:X2}";
        }

        public Vector4 ToVector4() => new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
    }
}

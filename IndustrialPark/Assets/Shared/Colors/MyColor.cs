﻿using System;
using System.ComponentModel;

namespace AssetEditorColors
{
    [TypeConverter(typeof(MyColorConverter))]
    public class MyColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }
        
        public MyColor()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
        }

        public MyColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public MyColor(string value)
        {
            R = Convert.ToByte(value.Substring(0, 2), 16);
            G = Convert.ToByte(value.Substring(2, 2), 16);
            B = Convert.ToByte(value.Substring(4, 2), 16);
            A = Convert.ToByte(value.Substring(6, 2), 16);
        }

        public MyColor(int ARGB)
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
            return $"{R.ToString()}, {G.ToString()}, {B.ToString()}";
        }
    }
}

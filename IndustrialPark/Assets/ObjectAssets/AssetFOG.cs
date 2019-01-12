﻿using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using AssetEditorColors;
using System.Windows.Forms;

namespace IndustrialPark
{
    public class AssetFOG : ObjectAsset
    {
        public AssetFOG(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x24;
        
        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("End Color (R, G, B)")]
        public MyColor Color1
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { Color1Alpha, value.B, value.G, value.R }, 0));
        }

        [Category("Fog"), DisplayName("End Color Alpha (0 - 255)")]
        public byte Color1Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return abgr[0];
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { value, Color1.B, Color1.G, Color1.R }, 0));
        }

        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Start Color (R, G, B)")]
        public MyColor Color2
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { Color2Alpha, value.B, value.G, value.R }, 0));
        }

        [Category("Fog"), DisplayName("Start Color Alpha (0 - 255)")]
        public byte Color2Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return abgr[0];
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { value, Color2.B, Color2.G, Color2.R }, 0));
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat10
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float StartDistance
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float EndDistance
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float TransitionTime
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Fog")]
        public int UnknownInt20
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
    }
}
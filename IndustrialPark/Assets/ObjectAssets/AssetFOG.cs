using HipHopFile;
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
        public MyColor BackgroundColor
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { BackgroundColorAlpha, value.B, value.G, value.R }, 0));
        }

        [Category("Fog"), DisplayName("End Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return abgr[0];
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { value, BackgroundColor.B, BackgroundColor.G, BackgroundColor.R }, 0));
        }

        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Start Color (R, G, B)")]
        public MyColor FogColor
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { FogColorAlpha, value.B, value.G, value.R }, 0));
        }

        [Category("Fog"), DisplayName("Start Color Alpha (0 - 255)")]
        public byte FogColorAlpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return abgr[0];
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { value, FogColor.B, FogColor.G, FogColor.R }, 0));
        }

        [Category("Fog"), TypeConverter(typeof(FloatTypeConverter))]
        public float FogDensity
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
        public byte FogType
        {
            get => ReadByte(0x20);
            set => Write(0x20, value);
        }

        [Category("Fog")]
        public byte Padding21
        {
            get => ReadByte(0x21);
            set => Write(0x21, value);
        }

        [Category("Fog")]
        public byte Padding22
        {
            get => ReadByte(0x22);
            set => Write(0x22, value);
        }

        [Category("Fog")]
        public byte Padding23
        {
            get => ReadByte(0x23);
            set => Write(0x23, value);
        }
    }
}
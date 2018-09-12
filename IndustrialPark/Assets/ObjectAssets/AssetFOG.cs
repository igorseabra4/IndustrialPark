using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing;
using ColorDialogExample;

namespace IndustrialPark
{
    public class AssetFOG : ObjectAsset
    {
        public AssetFOG(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x24;
        }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor Color1
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { Color1Alpha, value.B, value.G, value.R }, 0));
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Color1Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8));
                return abgr[0];
            }

            set => Write(0x8, BitConverter.ToInt32(new byte[] { value, Color1.B, Color1.G, Color1.R }, 0));
        }

        [Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor Color2
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { Color2Alpha, value.B, value.G, value.R }, 0));
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Color2Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0xC));
                return abgr[0];
            }

            set => Write(0xC, BitConverter.ToInt32(new byte[] { value, Color2.B, Color2.G, Color2.R }, 0));
        }

        public float UnknownFloat1
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        public float UnknownFloat2
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        public float UnknownFloat3
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        public float UnknownFloat4
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }

        public int Unknown5
        {
            get => ReadInt(0x20);
            set => Write(0x20, value);
        }
    }
}
using AssetEditorColors;
using HipHopFile;
using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetUIFT : AssetUI
    {
        public static new bool dontRender = false;

        protected override bool DontRender { get => dontRender; }

        protected override int EventStartOffset { get => 0xA4 + Offset; }

        public AssetUIFT(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (TextAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("UIFont")]
        public short UnknownShort_80
        {
            get => ReadShort(0x80 + Offset);
            set => Write(0x80 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_82
        {
            get => ReadShort(0x82 + Offset);
            set => Write(0x82 + Offset, value);
        }

        [Category("UIFont")]
        public AssetID TextAssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("UIFont"), Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor BackgroundColor
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x88));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0x88, BitConverter.ToInt32(new byte[] { BackgroundColorAlpha, value.B, value.G, value.R }, 0));
        }

        [Category("UIFont"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte BackgroundColorAlpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x88));
                return abgr[0];
            }

            set => Write(0x88, BitConverter.ToInt32(new byte[] { value, BackgroundColor.B, BackgroundColor.G, BackgroundColor.R }, 0));
        }

        [Category("UIFont"), Editor(typeof(MyColorEditor), typeof(UITypeEditor))]
        public MyColor FontColor
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8C));
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Write(0x8C, BitConverter.ToInt32(new byte[] { FontColorAlpha, value.B, value.G, value.R }, 0));
        }

        [Category("UIFont"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte FontColorAlpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(ReadInt(0x8C));
                return abgr[0];
            }

            set => Write(0x8C, BitConverter.ToInt32(new byte[] { value, FontColor.B, FontColor.G, FontColor.R }, 0));
        }

        [Category("UIFont")]
        public short UnknownShort_90
        {
            get => ReadShort(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_92
        {
            get => ReadShort(0x92 + Offset);
            set => Write(0x92 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_94
        {
            get => ReadShort(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_96
        {
            get => ReadShort(0x96 + Offset);
            set => Write(0x96 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_98
        {
            get => ReadShort(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_9A
        {
            get => ReadShort(0x9A + Offset);
            set => Write(0x9A + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_9C
        {
            get => ReadShort(0x9C + Offset);
            set => Write(0x9C + Offset, value);
        }

        [Category("UIFont")]
        public short UnknownShort_9E
        {
            get => ReadShort(0x9E + Offset);
            set => Write(0x9E + Offset, value);
        }

        [Category("UIFont")]
        public int UnknownInt_A0
        {
            get => ReadInt(0xA0 + Offset);
            set => Write(0xA0 + Offset, value);
        }
    }
}
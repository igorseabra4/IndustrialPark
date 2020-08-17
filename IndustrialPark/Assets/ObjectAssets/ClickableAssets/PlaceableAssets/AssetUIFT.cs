using AssetEditorColors;
using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class AssetUIFT : AssetUI
    {
        public static new bool dontRender = false;

        public override bool DontRender => dontRender;

        protected override int EventStartOffset => 0xA4 + Offset + (game == Game.Scooby ? -0x04 : 0);

        public AssetUIFT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID) => TextAssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TextAssetID, ref result);
        }

        [Category("UIFont")]
        public DynamicTypeDescriptor UIFontFlags => ShortFlagsDescriptor(0x80 + Offset, "Align Left", "Align Right", "Align Center", "Show Background");
        
        [Category("UIFont")]
        public byte UIFontMode
        {
            get => ReadByte(0x82 + Offset);
            set => Write(0x82 + Offset, value);
        }

        [Category("UIFont")]
        public FontEnum FontID
        {
            get => (FontEnum)ReadByte(0x83 + Offset);
            set => Write(0x83 + Offset, (byte)value);
        }

        [Category("UIFont")]
        public AssetID TextAssetID
        {
            get => ReadUInt(0x84 + Offset);
            set => Write(0x84 + Offset, value);
        }

        [Category("UIFont"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Background Color (R, G, B)")]
        public MyColor BackgroundColor
        {
            get => new MyColor(Data[0x88 + Offset], Data[0x89 + Offset], Data[0x8A + Offset], Data[0x8B + Offset]);
            set
            {
                Data[0x88 + Offset] = value.R;
                Data[0x89 + Offset] = value.G;
                Data[0x8A + Offset] = value.B;
            }
        }

        [Category("UIFont"), DisplayName("Background Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get => ReadByte(0x8B + Offset);
            set => Write(0x8B + Offset, value);
        }

        [Category("UIFont"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Font Color (R, G, B)")]
        public MyColor FontColor
        {
            get => new MyColor(Data[0x8C + Offset], Data[0x8D + Offset], Data[0x8E + Offset], Data[0x8F + Offset]);
            set
            {
                Data[0x8C + Offset] = value.R;
                Data[0x8D + Offset] = value.G;
                Data[0x8E + Offset] = value.B;
            }
        }

        [Category("UIFont"), DisplayName("Font Color Alpha (0 - 255)")]
        public byte FontColorAlpha
        {
            get => ReadByte(0x8F + Offset);
            set => Write(0x8F + Offset, value);
        }

        [Category("UIFont")]
        public short Padding_Top
        {
            get => ReadShort(0x90 + Offset);
            set => Write(0x90 + Offset, value);
        }

        [Category("UIFont")]
        public short Padding_Bottom
        {
            get => ReadShort(0x92 + Offset);
            set => Write(0x92 + Offset, value);
        }

        [Category("UIFont")]
        public short Padding_Left
        {
            get => ReadShort(0x94 + Offset);
            set => Write(0x94 + Offset, value);
        }

        [Category("UIFont")]
        public short Padding_Right
        {
            get => ReadShort(0x96 + Offset);
            set => Write(0x96 + Offset, value);
        }

        [Category("UIFont")]
        public short Spacing_Horizontal
        {
            get => ReadShort(0x98 + Offset);
            set => Write(0x98 + Offset, value);
        }

        [Category("UIFont")]
        public short Spacing_Vertical
        {
            get => ReadShort(0x9A + Offset);
            set => Write(0x9A + Offset, value);
        }

        [Category("UIFont")]
        public short Char_Width
        {
            get => ReadShort(0x9C + Offset);
            set => Write(0x9C + Offset, value);
        }

        [Category("UIFont")]
        public short Char_Height
        {
            get => ReadShort(0x9E + Offset);
            set => Write(0x9E + Offset, value);
        }

        [Category("UIFont")]
        public int MaxHeight
        {
            get => game == Game.Scooby ? 0 : ReadInt(0xA0 + Offset);
            set
            {
                if (game != Game.Scooby)
                    Write(0xA0 + Offset, value);
            }
        }
    }
}
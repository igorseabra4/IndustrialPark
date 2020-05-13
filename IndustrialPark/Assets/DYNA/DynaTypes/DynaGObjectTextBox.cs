using AssetEditorColors;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class DynaGObjectTextBox : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x54;

        public DynaGObjectTextBox(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (DefaultTextID == assetID)
                return true;
            if (BackgroundTextureID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(DefaultTextID, ref result);
            Asset.Verify(BackgroundTextureID, ref result);
        }
        
        public AssetID DefaultTextID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
        public float XPosition
        {
            get => ReadFloat(0x04);
            set => Write(0x04, value);
        }
        public float YPosition
        {
            get => ReadFloat(0x08);
            set => Write(0x08, value);
        }
        public float Width
        {
            get => ReadFloat(0x0C);
            set => Write(0x0C, value);
        }
        public float Height
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }
        public FontEnum Font
        {
            get => (FontEnum)ReadInt(0x14);
            set => Write(0x14, (int)value);
        }
        public float TextWidth
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        public float TextHeight
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        public float CharSpacingX
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        public float CharSpacingY
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }

        private byte Color1R
        {
            get => ReadByte(0x28);
            set => Write(0x28, value);
        }
        private byte Color1G
        {
            get => ReadByte(0x29);
            set => Write(0x29, value);
        }
        private byte Color1B
        {
            get => ReadByte(0x2A);
            set => Write(0x2A, value);
        }
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor TextColor
        {
            get => new MyColor(Color1R, Color1G, Color1B, Color1Alpha);            
            set
            {
                Color1R = value.R;
                Color1G = value.G;
                Color1B = value.B;
            }
        }
        [DisplayName("Color 1 Alpha (0 - 255)")]
        public byte Color1Alpha
        {
            get => ReadByte(0x2B);
            set => Write(0x2B, value);
        }
        public float LeftMargin
        {
            get => ReadFloat(0x2C);
            set => Write(0x2C, value);
        }
        public float TopMargin
        {
            get => ReadFloat(0x30);
            set => Write(0x30, value);
        }
        public float RightMargin
        {
            get => ReadFloat(0x34);
            set => Write(0x34, value);
        }
        public float BottomMargin
        {
            get => ReadFloat(0x38);
            set => Write(0x38, value);
        }
        public int TextAlign
        {
            get => ReadInt(0x3C);
            set => Write(0x3C, value);
        }
        public int ExpandMode
        {
            get => ReadInt(0x40);
            set => Write(0x40, value);
        }
        public float MaxHeight
        {
            get => ReadFloat(0x44);
            set => Write(0x44, value);
        }
        public int BackgroundMode
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }
        private byte Color2R
        {
            get => ReadByte(0x4C);
            set => Write(0x4C, value);
        }
        private byte Color2G
        {
            get => ReadByte(0x4D);
            set => Write(0x4D, value);
        }
        private byte Color2B
        {
            get => ReadByte(0x4E);
            set => Write(0x4E, value);
        }
        [Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 2 (R, G, B)")]
        public MyColor BackgroundColor
        {
            get => new MyColor(Color2R, Color2G, Color2B, Color2Alpha);
            set
            {
                Color2R = value.R;
                Color2G = value.G;
                Color2B = value.B;
            }
        }
        [DisplayName("Color 2 Alpha (0 - 255)")]
        public byte Color2Alpha
        {
            get => ReadByte(0x4F);
            set => Write(0x4F, value);
        }

        public AssetID BackgroundTextureID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
    }
}
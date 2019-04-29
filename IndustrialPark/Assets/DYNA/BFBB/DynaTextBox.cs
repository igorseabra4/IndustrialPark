using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaTextBox : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaTextBox() : base()
        {
            DefaultTextID = 0;
            BackgroundTextureID = 0;
        }

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

        public DynaTextBox(IEnumerable<byte> enumerable) : base (enumerable)
        {
            DefaultTextID = Switch(BitConverter.ToUInt32(Data, 0x00));
            XPosition = Switch(BitConverter.ToSingle(Data, 0x04));
            YPosition = Switch(BitConverter.ToSingle(Data, 0x08));
            Width = Switch(BitConverter.ToSingle(Data, 0x0C));
            Height = Switch(BitConverter.ToSingle(Data, 0x10));
            Font = Switch(BitConverter.ToInt32(Data, 0x14));
            TextWidth = Switch(BitConverter.ToSingle(Data, 0x18));
            TextHeight = Switch(BitConverter.ToSingle(Data, 0x1C));
            CharSpacing = Switch(BitConverter.ToSingle(Data, 0x20));
            UnknownInt_34 = Switch(BitConverter.ToInt32(Data, 0x24));
            byte[] abgr = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x28)));
            Color1R = abgr[3];
            Color1G = abgr[2];
            Color1B = abgr[1];
            Color1Alpha = abgr[0];
            LeftMargin = Switch(BitConverter.ToSingle(Data, 0x2C));
            TopMargin = Switch(BitConverter.ToSingle(Data, 0x30));
            RightMargin = Switch(BitConverter.ToSingle(Data, 0x34));
            BottomMargin = Switch(BitConverter.ToSingle(Data, 0x38));
            TextAlign = Switch(BitConverter.ToInt32(Data, 0x3C));
            UnknownInt_50 = Switch(BitConverter.ToInt32(Data, 0x40));
            UnknownFloat_54 = Switch(BitConverter.ToSingle(Data, 0x44));
            BackgroundMode = Switch(BitConverter.ToInt32(Data, 0x48));
            byte[] abgr2 = BitConverter.GetBytes(Switch(BitConverter.ToInt32(Data, 0x4C)));
            Color2R = abgr2[3];
            Color2G = abgr2[2];
            Color2B = abgr2[1];
            Color2Alpha = abgr2[0];
            BackgroundTextureID = Switch(BitConverter.ToUInt32(Data, 0x50));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch(DefaultTextID)));
            list.AddRange(BitConverter.GetBytes(Switch(XPosition)));
            list.AddRange(BitConverter.GetBytes(Switch(YPosition)));
            list.AddRange(BitConverter.GetBytes(Switch(Width)));
            list.AddRange(BitConverter.GetBytes(Switch(Height)));
            list.AddRange(BitConverter.GetBytes(Switch(Font)));
            list.AddRange(BitConverter.GetBytes(Switch(TextWidth)));
            list.AddRange(BitConverter.GetBytes(Switch(TextHeight)));
            list.AddRange(BitConverter.GetBytes(Switch(CharSpacing)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt_34)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToInt32(new byte[] { Color1Alpha, Color1B, Color1G, Color1R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(LeftMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(TopMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(RightMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(BottomMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(TextAlign)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt_50)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat_54)));
            list.AddRange(BitConverter.GetBytes(Switch(BackgroundMode)));
            list.AddRange(BitConverter.GetBytes(Switch(BitConverter.ToInt32(new byte[] { Color2Alpha, Color2B, Color2G, Color2R }, 0))));
            list.AddRange(BitConverter.GetBytes(Switch(BackgroundTextureID)));
            return list.ToArray();
        }

        public AssetID DefaultTextID { get; set; }
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public int Font { get; set; }
        public float TextWidth { get; set; }
        public float TextHeight { get; set; }
        public float CharSpacing { get; set; }
        public int UnknownInt_34 { get; set; }

        private byte Color1R;
        private byte Color1G;
        private byte Color1B;
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
        public byte Color1Alpha { get; set; }

        public float LeftMargin { get; set; }
        public float TopMargin { get; set; }
        public float RightMargin { get; set; }
        public float BottomMargin { get; set; }
        public int TextAlign { get; set; }
        public int UnknownInt_50 { get; set; }
        public float UnknownFloat_54 { get; set; }
        public int BackgroundMode { get; set; }

        private byte Color2R;
        private byte Color2G;
        private byte Color2B;
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
        public byte Color2Alpha { get; set; }

        public AssetID BackgroundTextureID { get; set; }
    }
}
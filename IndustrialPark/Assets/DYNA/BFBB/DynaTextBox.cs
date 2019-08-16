using AssetEditorColors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaTextBox : DynaBase
    {
        public override string Note => "Version is always 1";

        public DynaTextBox(Platform platform) : base(platform)
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

        public DynaTextBox(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            DefaultTextID = Switch(BitConverter.ToUInt32(Data, 0x00));
            XPosition = Switch(BitConverter.ToSingle(Data, 0x04));
            YPosition = Switch(BitConverter.ToSingle(Data, 0x08));
            Width = Switch(BitConverter.ToSingle(Data, 0x0C));
            Height = Switch(BitConverter.ToSingle(Data, 0x10));
            Font = (FontEnum)Switch(BitConverter.ToInt32(Data, 0x14));
            TextWidth = Switch(BitConverter.ToSingle(Data, 0x18));
            TextHeight = Switch(BitConverter.ToSingle(Data, 0x1C));
            CharSpacingX = Switch(BitConverter.ToSingle(Data, 0x20));
            CharSpacingY = Switch(BitConverter.ToSingle(Data, 0x24));
            Color1R = Data[0x28];
            Color1G = Data[0x29];
            Color1B = Data[0x2A];
            Color1Alpha = Data[0x2B];
            LeftMargin = Switch(BitConverter.ToSingle(Data, 0x2C));
            TopMargin = Switch(BitConverter.ToSingle(Data, 0x30));
            RightMargin = Switch(BitConverter.ToSingle(Data, 0x34));
            BottomMargin = Switch(BitConverter.ToSingle(Data, 0x38));
            TextAlign = Switch(BitConverter.ToInt32(Data, 0x3C));
            ExpandMode = Switch(BitConverter.ToInt32(Data, 0x40));
            MaxHeight = Switch(BitConverter.ToSingle(Data, 0x44));
            BackgroundMode = Switch(BitConverter.ToInt32(Data, 0x48));
            Color2R = Data[0x4C];
            Color2G = Data[0x4D];
            Color2B = Data[0x4E];
            Color2Alpha = Data[0x4F];
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
            list.AddRange(BitConverter.GetBytes(Switch((int)Font)));
            list.AddRange(BitConverter.GetBytes(Switch(TextWidth)));
            list.AddRange(BitConverter.GetBytes(Switch(TextHeight)));
            list.AddRange(BitConverter.GetBytes(Switch(CharSpacingX)));
            list.AddRange(BitConverter.GetBytes(Switch(CharSpacingY)));
            list.Add(Color1R);
            list.Add(Color1G);
            list.Add(Color1B);
            list.Add(Color1Alpha);
            list.AddRange(BitConverter.GetBytes(Switch(LeftMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(TopMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(RightMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(BottomMargin)));
            list.AddRange(BitConverter.GetBytes(Switch(TextAlign)));
            list.AddRange(BitConverter.GetBytes(Switch(ExpandMode)));
            list.AddRange(BitConverter.GetBytes(Switch(MaxHeight)));
            list.AddRange(BitConverter.GetBytes(Switch(BackgroundMode)));
            list.Add(Color2R);
            list.Add(Color2G);
            list.Add(Color2B);
            list.Add(Color2Alpha);
            list.AddRange(BitConverter.GetBytes(Switch(BackgroundTextureID)));
            return list.ToArray();
        }

        public AssetID DefaultTextID { get; set; }
        public float XPosition { get; set; }
        public float YPosition { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public FontEnum Font { get; set; }
        public float TextWidth { get; set; }
        public float TextHeight { get; set; }
        public float CharSpacingX { get; set; }
        public float CharSpacingY { get; set; }

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
        public int ExpandMode { get; set; }
        public float MaxHeight { get; set; }
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
using AssetEditorColors;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectTextBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:text_box";

        public override string Note => "Version is always 1 for BFBB or 3 for Movie/Incredibles.";

        [Category(dynaCategoryName)]
        public AssetID DefaultTextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle XPosition { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle YPosition { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Width { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Height { get; set; }
        [Category(dynaCategoryName)]
        public FontEnum Font { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TextWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TextHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CharSpacingX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CharSpacingY { get; set; }
        [Category(dynaCategoryName), DisplayName("Color (R, G, B)")]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName), DisplayName("Color Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => Color.A;
            set => Color.A = value;
        }
        [Category(dynaCategoryName)]
        public AssetSingle LeftMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TopMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RightMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BottomMargin { get; set; }
        [Category(dynaCategoryName)]
        public int XAlignment { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public int YAlignment { get; set; }
        [Category(dynaCategoryName)]
        public int ExpandMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxHeight { get; set; }
        [Category(dynaCategoryName)]
        public int BackgroundMode { get; set; }
        [Category(dynaCategoryName), DisplayName("Backdrop Color (R, G, B)")]
        public AssetColor BackdropColor { get; set; }
        [Category(dynaCategoryName), DisplayName("Backdrop Color Alpha (0 - 255)")]
        public byte BackdropColorAlpha
        {
            get => BackdropColor.A;
            set => BackdropColor.A = value;
        }
        [Category(dynaCategoryName)]
        public AssetID BackgroundTextureID { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle BackgroundBorderU { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle BackgroundBorderV { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle BackgroundBorderWidth { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle BackgroundBorderHeight { get; set; }
        [Category(dynaCategoryName), DisplayName("Shadow Color (R, G, B)"), Description("Only in version 3")]
        public AssetColor ShadowColor { get; set; }
        [Category(dynaCategoryName), DisplayName("Shadow Color Alpha (0 - 255)"), Description("Only in version 3")]
        public byte ShadowColorAlpha
        {
            get => ShadowColor.A;
            set => ShadowColor.A = value;
        }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle ShadowOffsetX { get; set; }
        [Category(dynaCategoryName), Description("Only in version 3")]
        public AssetSingle ShadowOffsetY { get; set; }

        public DynaGObjectTextBox(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__text_box, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            DefaultTextID = reader.ReadUInt32();
            XPosition = reader.ReadSingle();
            YPosition = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Font = (FontEnum)reader.ReadInt32();
            TextWidth = reader.ReadSingle();
            TextHeight = reader.ReadSingle();
            CharSpacingX = reader.ReadSingle();
            CharSpacingY = reader.ReadSingle();
            Color = reader.ReadColor();
            LeftMargin = reader.ReadSingle();
            TopMargin = reader.ReadSingle();
            RightMargin = reader.ReadSingle();
            BottomMargin = reader.ReadSingle();
            XAlignment = reader.ReadInt32();
            if (Version == 3)
                YAlignment = reader.ReadInt32();
            ExpandMode = reader.ReadInt32();
            MaxHeight = reader.ReadSingle();
            BackgroundMode = reader.ReadInt32();
            BackdropColor = reader.ReadColor();
            BackgroundTextureID = reader.ReadUInt32();
            if (Version == 3)
            {
                BackgroundBorderU = reader.ReadSingle();
                BackgroundBorderV = reader.ReadSingle();
                BackgroundBorderWidth = reader.ReadSingle();
                BackgroundBorderHeight = reader.ReadSingle();
                ShadowColor = reader.ReadColor();
                ShadowOffsetX = reader.ReadSingle();
                ShadowOffsetY = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(DefaultTextID);
            writer.Write(XPosition);
            writer.Write(YPosition);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write((int)Font);
            writer.Write(TextWidth);
            writer.Write(TextHeight);
            writer.Write(CharSpacingX);
            writer.Write(CharSpacingY);
            writer.Write(Color);
            writer.Write(LeftMargin);
            writer.Write(TopMargin);
            writer.Write(RightMargin);
            writer.Write(BottomMargin);
            writer.Write(XAlignment);
            if (Version == 3)
                writer.Write(YAlignment);
            writer.Write(ExpandMode);
            writer.Write(MaxHeight);
            writer.Write(BackgroundMode);
            writer.Write(BackdropColor);
            writer.Write(BackgroundTextureID);
            if (Version == 3)
            {
                writer.Write(BackgroundBorderU);
                writer.Write(BackgroundBorderV);
                writer.Write(BackgroundBorderWidth);
                writer.Write(BackgroundBorderHeight);
                writer.Write(ShadowColor);
                writer.Write(ShadowOffsetX);
                writer.Write(ShadowOffsetY);
            }

            return writer.ToArray();
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
            Verify(DefaultTextID, ref result);
            Verify(BackgroundTextureID, ref result);
        }
    }
}
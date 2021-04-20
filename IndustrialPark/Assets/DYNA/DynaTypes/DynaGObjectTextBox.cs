using AssetEditorColors;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{

    public class DynaGObjectTextBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:text_box";

        protected override int constVersion => 1;

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
        public int TextAlign { get; set; }
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
            Color = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            LeftMargin = reader.ReadSingle();
            TopMargin = reader.ReadSingle();
            RightMargin = reader.ReadSingle();
            BottomMargin = reader.ReadSingle();
            TextAlign = reader.ReadInt32();
            ExpandMode = reader.ReadInt32();
            MaxHeight = reader.ReadSingle();
            BackgroundMode = reader.ReadInt32();
            BackdropColor = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            BackgroundTextureID = reader.ReadUInt32();
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
            writer.Write(TextAlign);
            writer.Write(ExpandMode);
            writer.Write(MaxHeight);
            writer.Write(BackgroundMode);
            writer.Write(BackdropColor);
            writer.Write(BackgroundTextureID);

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
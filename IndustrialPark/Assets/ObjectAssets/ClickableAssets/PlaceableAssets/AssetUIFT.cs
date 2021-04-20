using AssetEditorColors;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetUIFT : AssetUI
    {
        private const string categoryName = "UIFont";

        [Category(categoryName)]
        public FlagBitmask UIFontFlags { get; set; } = ShortFlagsDescriptor(
              "Align Left",
              "Align Right",
              "Align Center",
              "Show Background");
        [Category(categoryName)]
        public byte UIFontMode { get; set; }
        [Category(categoryName)]
        public FontEnum FontID { get; set; }
        [Category(categoryName)]
        public AssetID TextAssetID { get; set; }
        [Category(categoryName), DisplayName("Background Color (R, G, B)")]
        public AssetColor BackgroundColor { get; set; }
        [Category(categoryName), DisplayName("Background Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get => BackgroundColor.A;
            set => BackgroundColor.A = value;
        }
        [Category(categoryName), DisplayName("Font Color (R, G, B)")]
        public AssetColor FontColor { get; set; }
        [Category(categoryName), DisplayName("Font Color Alpha (0 - 255)")]
        public byte FontColorAlpha
        {
            get => BackgroundColor.A;
            set => BackgroundColor.A = value;
        }
        [Category(categoryName)]
        public short Padding_Top { get; set; }
        [Category(categoryName)]
        public short Padding_Bottom { get; set; }
        [Category(categoryName)]
        public short Padding_Left { get; set; }
        [Category(categoryName)]
        public short Padding_Right { get; set; }
        [Category(categoryName)]
        public short Spacing_Horizontal { get; set; }
        [Category(categoryName)]
        public short Spacing_Vertical { get; set; }
        [Category(categoryName)]
        public short Char_Width { get; set; }
        [Category(categoryName)]
        public short Char_Height { get; set; }
        [Category(categoryName)]
        public int MaxHeight { get; set; } // not in scooby

        public AssetUIFT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = game == Game.BFBB ? 0x80 : 0x7C;

            UIFontFlags.FlagValueShort = reader.ReadUInt16();
            UIFontMode = reader.ReadByte();
            FontID = (FontEnum)reader.ReadByte();
            TextAssetID = reader.ReadUInt32();
            BackgroundColor = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            FontColor = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntity(game, platform));
            writer.Write(SerializeUIData(platform));

            writer.Write(UIFontFlags.FlagValueShort);
            writer.Write(UIFontMode);
            writer.Write((byte)FontID);
            writer.Write(TextAssetID);
            writer.Write(BackgroundColor);
            writer.Write(FontColor);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static new bool dontRender = false;

        public override bool DontRender => dontRender;

        public override bool HasReference(uint assetID) => TextAssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(TextAssetID, ref result);
        }
    }
}
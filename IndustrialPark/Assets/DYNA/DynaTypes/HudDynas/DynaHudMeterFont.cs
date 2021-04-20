using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaHudMeterFont : DynaHudMeter
    {
        private const string dynaCategoryName = "hud:meter:font";

        public override string Note => "Version is always 2 or 3. Version 2 does not use CounterModeFlag.";

        [Category(dynaCategoryName)]
        public FontEnum Font { get; set; }
        [Category(dynaCategoryName)]
        public int FontJustify { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FontWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FontHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FontSpace { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShadowXOffset { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShadowYOffset { get; set; }
        [Category(dynaCategoryName), DisplayName("Color (R, G, B)")]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName), DisplayName("Color Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => Color.A;
            set => Color.A = value;
        }
        [Category(dynaCategoryName), DisplayName("Color (R, G, B)")]
        public AssetColor ShadowColor { get; set; }
        [Category(dynaCategoryName), DisplayName("Color Alpha (0 - 255)")]
        public byte ShadowColorAlpha
        {
            get => ShadowColor.A;
            set => ShadowColor.A = value;
        }
        [Category(dynaCategoryName)]
        public AssetByte CounterModeFlag { get; set; }

        public DynaHudMeterFont(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.hud__meter__font, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaHudMeterEnd;

            Font = (FontEnum)reader.ReadInt32();
            FontJustify = reader.ReadInt32();
            FontWidth = reader.ReadSingle();
            FontHeight = reader.ReadSingle();
            FontSpace = reader.ReadSingle();
            ShadowXOffset = reader.ReadSingle();
            ShadowYOffset = reader.ReadSingle();
            Color = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            ShadowColor = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

            if (Version == 3)
                CounterModeFlag = reader.ReadByte();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeDynaHudMeter(platform));
            writer.Write((int)Font);
            writer.Write(FontJustify);
            writer.Write(FontWidth);
            writer.Write(FontHeight);
            writer.Write(FontSpace);
            writer.Write(ShadowXOffset);
            writer.Write(ShadowYOffset);
            writer.Write(Color);
            writer.Write(ShadowColor);
            if (Version == 3)
                writer.Write(CounterModeFlag);

            return writer.ToArray();
        }
    }
}
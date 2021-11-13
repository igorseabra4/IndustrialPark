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
        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor ShadowColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte CounterModeFlag { get; set; }

        public DynaHudMeterFont(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.hud__meter__font, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudMeterEnd;

                Font = (FontEnum)reader.ReadInt32();
                FontJustify = reader.ReadInt32();
                FontWidth = reader.ReadSingle();
                FontHeight = reader.ReadSingle();
                FontSpace = reader.ReadSingle();
                ShadowXOffset = reader.ReadSingle();
                ShadowYOffset = reader.ReadSingle();
                Color = reader.ReadColor();
                ShadowColor = reader.ReadColor();

                if (Version == 3)
                {
                    CounterModeFlag = reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                    reader.ReadByte();
                }
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaHudMeter(endianness));
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
                {
                    writer.Write(CounterModeFlag);
                    writer.Write((byte)0);
                    writer.Write((short)0);
                }

                return writer.ToArray();
            }
        }
    }
}
using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : AssetDYNA
    {
        private const string dynaCategoryName = "effect:ScreenFade";

        protected override int constVersion => 1;
        
        [Category(dynaCategoryName), DisplayName("Color (R, G, B)")]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName), DisplayName("Color Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => Color.A;
            set => Color.A = value;
        }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat3 { get; set; }

        public DynaEffectScreenFade(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.effect__ScreenFade, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            Color = new AssetColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            UnknownFloat1 = reader.ReadSingle();
            UnknownFloat2 = reader.ReadSingle();
            UnknownFloat3 = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Color);
            writer.Write(UnknownFloat1);
            writer.Write(UnknownFloat2);
            writer.Write(UnknownFloat3);

            return writer.ToArray();
        }
    }
}
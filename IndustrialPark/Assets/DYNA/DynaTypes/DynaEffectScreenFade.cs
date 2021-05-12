using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : AssetDYNA
    {
        private const string dynaCategoryName = "effect:ScreenFade";

        protected override short constVersion => 1;
        
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

        public DynaEffectScreenFade(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__ScreenFade, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Color = reader.ReadColor();
                UnknownFloat1 = reader.ReadSingle();
                UnknownFloat2 = reader.ReadSingle();
                UnknownFloat3 = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Color);
                writer.Write(UnknownFloat1);
                writer.Write(UnknownFloat2);
                writer.Write(UnknownFloat3);

                return writer.ToArray();
            }
        }
    }
}
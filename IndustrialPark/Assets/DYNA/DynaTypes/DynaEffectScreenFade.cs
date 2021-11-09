using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectScreenFade : AssetDYNA
    {
        private const string dynaCategoryName = "effect:ScreenFade";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeDownTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WaitTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle FadeUpTime { get; set; }

        public DynaEffectScreenFade(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__ScreenFade, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Color = reader.ReadColor();
                FadeDownTime = reader.ReadSingle();
                WaitTime = reader.ReadSingle();
                FadeUpTime = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Color);
                writer.Write(FadeDownTime);
                writer.Write(WaitTime);
                writer.Write(FadeUpTime);

                return writer.ToArray();
            }
        }
    }
}
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectRumble : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Rumble";

        protected override int constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_08 { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_0C { get; set; }
        [Category(dynaCategoryName)]
        public short UnknownShort_0A { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_1C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_20 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_24 { get; set; }

        public DynaEffectRumble(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.effect__Rumble, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownFloat_00 = reader.ReadSingle();
            UnknownFloat_04 = reader.ReadSingle();
            UnknownFloat_08 = reader.ReadSingle();
            UnknownShort_0C = reader.ReadInt16();
            UnknownShort_0A = reader.ReadInt16();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14 = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
            UnknownFloat_1C = reader.ReadSingle();
            UnknownFloat_20 = reader.ReadSingle();
            UnknownFloat_24 = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UnknownFloat_00);
            writer.Write(UnknownFloat_04);
            writer.Write(UnknownFloat_08);
            writer.Write(UnknownShort_0C);
            writer.Write(UnknownShort_0A);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownFloat_14);
            writer.Write(UnknownFloat_18);
            writer.Write(UnknownFloat_1C);
            writer.Write(UnknownFloat_20);
            writer.Write(UnknownFloat_24);

            return writer.ToArray();
        }
    }
}
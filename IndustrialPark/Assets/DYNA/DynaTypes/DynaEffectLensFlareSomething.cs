using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectLensFlareSource : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Lens Flare Source";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetID Unknown00 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown04 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown14 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Unknown18 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown1C { get; set; }

        public DynaEffectLensFlareSource(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__LensFlareSource, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Unknown00 = reader.ReadUInt32();
                Unknown04 = reader.ReadUInt32();
                Unknown08 = reader.ReadUInt32();
                Unknown0C = reader.ReadUInt32();
                Unknown10 = reader.ReadSingle();
                Unknown14 = reader.ReadSingle();
                Unknown18 = reader.ReadSingle();
                Unknown1C = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unknown00);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(Unknown0C);
                writer.Write(Unknown10);
                writer.Write(Unknown14);
                writer.Write(Unknown18);
                writer.Write(Unknown1C);

                return writer.ToArray();
            }
        }
    }
}
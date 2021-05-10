using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class AssetVILP : Asset
    {
        private const string vilpName = "VILP";

        [Category(vilpName)]
        public AssetID Unknown_00 { get; set; }
        [Category(vilpName)]
        public int Unknown_04 { get; set; }
        [Category(vilpName)]
        public int Unknown_08 { get; set; }
        [Category(vilpName)]
        public int Unknown_0C { get; set; }
        [Category(vilpName)]
        public int Unknown_10 { get; set; }
        [Category(vilpName)]
        public int Unknown_14 { get; set; }
        [Category(vilpName)]
        public int Unknown_18 { get; set; }
        [Category(vilpName)]
        public AssetSingle Unknown_1C { get; set; }
        [Category(vilpName)]
        public int Unknown_20 { get; set; }

        public AssetVILP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);

            Unknown_00 = reader.ReadUInt32();
            Unknown_04 = reader.ReadInt32();
            Unknown_08 = reader.ReadInt32();
            Unknown_0C = reader.ReadInt32();
            Unknown_10 = reader.ReadInt32();
            Unknown_14 = reader.ReadInt32();
            Unknown_18 = reader.ReadInt32();
            Unknown_1C = reader.ReadSingle();
            Unknown_20 = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(Unknown_00);
            writer.Write(Unknown_04);
            writer.Write(Unknown_08);
            writer.Write(Unknown_0C);
            writer.Write(Unknown_10);
            writer.Write(Unknown_14);
            writer.Write(Unknown_18);
            writer.Write(Unknown_1C);
            writer.Write(Unknown_20);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Unknown_00 == assetID || base.HasReference(assetID);
    }
}
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetGUST : BaseAsset
    {
        private const string catName = "Gust";

        [Category(catName)]
        public FlagBitmask GustFlags { get; set; } = IntFlagsDescriptor("Start on", "No leaves");
        [Category(catName)]
        public AssetID Volume_AssetID { get; set; }
        [Category(catName)]
        public int UnknownInt10 { get; set; }
        [Category(catName)]
        public AssetSingle StrengthX { get; set; }
        [Category(catName)]
        public AssetSingle StrengthY { get; set; }
        [Category(catName)]
        public AssetSingle StrengthZ { get; set; }
        [Category(catName)]
        public AssetSingle UnknownFloat20 { get; set; }
        [Category(catName)]
        public AssetSingle UnknownFloat24 { get; set; }

        public AssetGUST(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                GustFlags.FlagValueInt = reader.ReadUInt32();
                Volume_AssetID = reader.ReadUInt32();
                UnknownInt10 = reader.ReadInt32();
                StrengthX = reader.ReadSingle();
                StrengthY = reader.ReadSingle();
                StrengthZ = reader.ReadSingle();
                UnknownFloat20 = reader.ReadSingle();
                UnknownFloat24 = reader.ReadSingle();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(GustFlags.FlagValueInt);
                writer.Write(Volume_AssetID);
                writer.Write(UnknownInt10);
                writer.Write(StrengthX);
                writer.Write(StrengthY);
                writer.Write(StrengthZ);
                writer.Write(UnknownFloat20);
                writer.Write(UnknownFloat24);
                writer.Write(SerializeLinks(endianness));

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => Volume_AssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Volume_AssetID == 0)
                result.Add("GUST with Volume_AssetID set to 0");
            Verify(Volume_AssetID, ref result);
        }
    }
}
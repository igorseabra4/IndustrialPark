using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetGUST : BaseAsset
    {
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(Volume);

        private const string catName = "Gust";

        [Category(catName)]
        public FlagBitmask GustFlags { get; set; } = IntFlagsDescriptor("Start on", "No leaves");
        [Category(catName), ValidReferenceRequired]
        public AssetID Volume { get; set; }
        [Category(catName)]
        public AssetID Effect { get; set; }
        [Category(catName)]
        public AssetSingle StrengthX { get; set; }
        [Category(catName)]
        public AssetSingle StrengthY { get; set; }
        [Category(catName)]
        public AssetSingle StrengthZ { get; set; }
        [Category(catName)]
        public AssetSingle Fade { get; set; }
        [Category(catName)]
        public AssetSingle PartMod { get; set; }

        public AssetGUST(string assetName) : base(assetName, AssetType.Gust, BaseAssetType.Gust)
        {
            GustFlags.FlagValueInt = 1;
            StrengthY = 1f;
            Fade = 0.5f;
            PartMod = 1f;
        }

        public AssetGUST(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                GustFlags.FlagValueInt = reader.ReadUInt32();
                Volume = reader.ReadUInt32();
                Effect = reader.ReadUInt32();
                StrengthX = reader.ReadSingle();
                StrengthY = reader.ReadSingle();
                StrengthZ = reader.ReadSingle();
                Fade = reader.ReadSingle();
                PartMod = reader.ReadSingle();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

            base.Serialize(writer);
            writer.Write(GustFlags.FlagValueInt);
            writer.Write(Volume);
            writer.Write(Effect);
            writer.Write(StrengthX);
            writer.Write(StrengthY);
            writer.Write(StrengthZ);
            writer.Write(Fade);
            writer.Write(PartMod);
            SerializeLinks(writer);


        }
    }
}
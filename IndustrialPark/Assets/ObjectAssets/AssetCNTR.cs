using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCNTR : BaseAsset
    {
        public override string AssetInfo => $"Count: {Count}";

        [Category("Counter")]
        public short Count { get; set; }

        public AssetCNTR(string assetName) : base(assetName, AssetType.Counter, BaseAssetType.Counter)
        {
        }

        public AssetCNTR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;
                Count = reader.ReadInt16();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(Count);
                writer.Write((short)0);
                writer.Write(SerializeLinks(endianness));

                return writer.ToArray();
            }
        }
    }
}
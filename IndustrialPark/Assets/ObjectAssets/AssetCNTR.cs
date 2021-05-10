using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCNTR : BaseAsset
    {
        [Category("Counter")]
        public short Count { get; set; }

        public AssetCNTR(string assetName) : base(assetName, AssetType.CNTR, BaseAssetType.Counter)
        {
        }

        public AssetCNTR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            Count = reader.ReadInt16();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(SerializeBase(endianness));

            writer.Write(Count);
            writer.Write((short)0);

            writer.Write(SerializeLinks(endianness));

            return writer.ToArray();
        }
    }
}
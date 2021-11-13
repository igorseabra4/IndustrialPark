using HipHopFile;

namespace IndustrialPark
{
    public class AssetDPAT : BaseAsset
    {
        public AssetDPAT(string assetName) : base(assetName, AssetType.DPAT, BaseAssetType.Dispatcher) { }

        public AssetDPAT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness) { }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }
    }
}
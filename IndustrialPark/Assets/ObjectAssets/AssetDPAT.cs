using HipHopFile;

namespace IndustrialPark
{
    public class AssetDPAT : BaseAsset
    {
        public AssetDPAT(string assetName) : base(assetName, AssetType.DPAT, BaseAssetType.Dispatcher) { }

        public AssetDPAT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));
            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }
    }
}
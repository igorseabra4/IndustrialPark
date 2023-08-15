using HipHopFile;

namespace IndustrialPark
{
    public class AssetDPAT : BaseAsset
    {
        public AssetDPAT(string assetName) : base(assetName, AssetType.Dispatcher, BaseAssetType.Dispatcher) { }

        public AssetDPAT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness) { }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            SerializeLinks(writer);
        }
    }
}
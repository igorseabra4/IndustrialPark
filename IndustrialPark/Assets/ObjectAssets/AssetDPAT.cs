using HipHopFile;

namespace IndustrialPark.Assets
{
    public class AssetDPAT : BaseAsset
    {
        public AssetDPAT(string assetName) : base(assetName, AssetType.Dispatcher, BaseAssetType.Dispatcher) { }

        public AssetDPAT(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, reader) { }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            SerializeLinks(writer);
        }
    }
}
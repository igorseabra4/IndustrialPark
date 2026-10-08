namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public AssetAddedAction GetAssetAddedAction(Asset asset)
        {
            return new AssetAddedAction(this, asset.BuildAHDR(platform.Endianness()), GetLayerFromAssetID(asset.assetID));
        }

        public AssetAddedAction GetAssetAddedAction(uint assetID)
        {
            return GetAssetAddedAction(GetFromAssetID(assetID));
        }

        public AssetRemovedAction GetAssetRemovedAction(Asset asset)
        {
            return new AssetRemovedAction(this, asset.BuildAHDR(platform.Endianness()), GetLayerFromAssetID(asset.assetID));
        }

        public AssetRemovedAction GetAssetRemovedAction(uint assetID)
        {
            return GetAssetRemovedAction(GetFromAssetID(assetID));
        }
    }
}

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private AssetAddedAction GetAssetAddedAction(Asset asset)
        {
            return new AssetAddedAction(this, asset.BuildAHDR(platform.Endianness()), GetLayerFromAssetID(asset.assetID));
        }

        private AssetAddedAction GetAssetAddedAction(uint assetID)
        {
            return GetAssetAddedAction(GetFromAssetID(assetID));
        }

        private AssetRemovedAction GetAssetRemovedAction(Asset asset)
        {
            return new AssetRemovedAction(this, asset.BuildAHDR(platform.Endianness()), GetLayerFromAssetID(asset.assetID));
        }

        private AssetRemovedAction GetAssetRemovedAction(uint assetID)
        {
            return GetAssetRemovedAction(GetFromAssetID(assetID));
        }
    }
}

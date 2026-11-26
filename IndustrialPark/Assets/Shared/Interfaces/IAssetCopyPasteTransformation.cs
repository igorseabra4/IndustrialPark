using IndustrialPark.ArchiveEditors;

namespace IndustrialPark.Assets;

public interface IAssetCopyPasteTransformation
{
    AssetTransformation GetTransformation();
    void SetTransformation(AssetTransformation transformation);
    void CalculateTransformation(SharpRenderer renderer);
}

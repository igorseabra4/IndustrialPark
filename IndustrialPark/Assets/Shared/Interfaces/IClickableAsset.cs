using SharpDX;

namespace IndustrialPark
{
    public interface IClickableAsset
    {
        AssetSingle PositionX { get; set; }
        AssetSingle PositionY { get; set; }
        AssetSingle PositionZ { get; set; }
        uint assetID { get; }
        bool isSelected { get; }

        float? GetIntersectionPosition(SharpRenderer renderer, Ray ray);

        BoundingBox GetBoundingBox();
    }
}
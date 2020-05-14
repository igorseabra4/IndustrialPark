using SharpDX;

namespace IndustrialPark
{
    public interface IClickableAsset
    {
        float PositionX { get; set; }
        float PositionY { get; set; }
        float PositionZ { get; set; }

        float? GetIntersectionPosition(SharpRenderer renderer, Ray ray);

        BoundingBox GetBoundingBox();
    }
}
using SharpDX;

namespace IndustrialPark
{
    public interface IScalableAsset
    {
        float ScaleX { get; set; }
        float ScaleY { get; set; }
        float ScaleZ { get; set; }

        BoundingBox GetBoundingBox();
    }
}
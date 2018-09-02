using SharpDX;

namespace IndustrialPark
{
    public interface IRenderableAsset
    {
        void CreateTransformMatrix();

        void Draw(SharpRenderer renderer);

        BoundingBox GetBoundingBox();
    }
}
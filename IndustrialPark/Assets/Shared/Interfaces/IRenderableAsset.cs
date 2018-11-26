using SharpDX;

namespace IndustrialPark
{
    public interface IRenderableAsset
    {
        void CreateTransformMatrix();

        void Draw(SharpRenderer renderer);

        BoundingBox GetBoundingBox();

        float GetDistance(Vector3 cameraPosition);

        float? IntersectsWith(Ray ray);
    }
}
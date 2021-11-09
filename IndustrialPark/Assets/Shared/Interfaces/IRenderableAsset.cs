using SharpDX;

namespace IndustrialPark
{
    public interface IRenderableAsset
    {
        void CreateTransformMatrix();

        float GetDistanceFrom(Vector3 position);

        bool ShouldDraw(SharpRenderer renderer);

        void Draw(SharpRenderer renderer);

        float? GetIntersectionPosition(SharpRenderer renderer, SharpDX.Ray ray);

        bool SpecialBlendMode { get; }
    }
}
using SharpDX;

namespace IndustrialPark.Assets
{
    public interface IRenderableAsset
    {
        uint assetID { get; }

        void CalculateTransformation(SharpRenderer renderer);

        float GetDistanceFrom(Vector3 position);

        bool ShouldDraw(SharpRenderer renderer);

        void Draw(SharpRenderer renderer);

        float? GetIntersectionPosition(SharpRenderer renderer, SharpDX.Ray ray);

        bool SpecialBlendMode(SharpRenderer renderer);
    }
}
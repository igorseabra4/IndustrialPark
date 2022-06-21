using SharpDX;

namespace IndustrialPark
{
    public interface IAssetWithModel
    {
        void Draw(SharpRenderer renderer, Matrix worldViewProjection, Vector4 color, Vector3 uvAnimOffset);

        RenderWareModelFile GetRenderWareModelFile();

        void RemoveFromDictionary();

        bool SpecialBlendMode { get; }
    }
}
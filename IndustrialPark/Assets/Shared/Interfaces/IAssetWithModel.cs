using SharpDX;

namespace IndustrialPark
{
    public interface IAssetWithModel
    {
        void Draw(SharpRenderer renderer, Matrix worldViewProjection, Vector4 color, Vector3 uvAnimOffset, bool isSelected);

        RenderWareModelFile GetRenderWareModelFile();

        void RemoveFromDictionary();

        bool SpecialBlendMode { get; }

        Matrix TransformMatrix { get; }

        void ApplyScale(Vector3 factor);
    }
}
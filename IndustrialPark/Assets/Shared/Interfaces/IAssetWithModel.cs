using IndustrialPark.Archive;
using SharpDX;

namespace IndustrialPark
{
    public interface IAssetWithModel
    {
        uint assetID { get; }

        void Draw(SharpRenderer renderer, Matrix worldViewProjection, Vector4 color, Vector3 uvAnimOffset, bool isSelected);

        RenderWareModelFile GetRenderWareModelFile(AssetRenderer rendererA);

        bool SpecialBlendMode(SharpRenderer renderer);

        Matrix TransformMatrix { get; }

        void ApplyScale(Vector3 factor);

        string fileNameWithoutExtension { get; }
    }
}
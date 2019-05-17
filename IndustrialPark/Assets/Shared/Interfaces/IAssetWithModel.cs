using SharpDX;

namespace IndustrialPark
{
    public interface IAssetWithModel
    {
        void Draw(SharpRenderer renderer, Matrix worldViewProjection, Vector4 color, Vector3 uvAnimOffset);

        bool HasRenderWareModelFile();

        RenderWareModelFile GetRenderWareModelFile();

        void MovieRemoveFromDictionary();
    }
}
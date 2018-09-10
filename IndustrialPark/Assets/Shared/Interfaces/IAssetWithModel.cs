using SharpDX;

namespace IndustrialPark
{
    public interface IAssetWithModel
    {
        void Draw(SharpRenderer renderer, Matrix worldViewProjection, bool isSelected);

        RenderWareModelFile GetRenderWareModelFile();
    }
}
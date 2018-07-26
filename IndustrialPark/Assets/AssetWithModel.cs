using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public abstract class AssetWithModel : Asset
    {
        public AssetWithModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public abstract void Draw(Matrix worldViewProjection, bool isSelected);

        public abstract RenderWareModelFile GetRenderWareModelFile();
    }
}
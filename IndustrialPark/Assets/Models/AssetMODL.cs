using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMODL : AssetRenderWareModel, IAssetWithModel
    {
        public AssetMODL(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            model.Render(renderer, world, isSelected ? renderer.selectedObjectColor * color : color);
        }
    }
}
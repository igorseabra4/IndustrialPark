using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetLevelModel : RenderableAsset
    { 
        public RenderWareModelFile model;

        public AssetLevelModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            model.ignoreNormals = true;
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile), AHDR.containedFile);
            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public new void Draw(SharpRenderer renderer)
        {
            model.Render(renderer, Matrix.Identity, isSelected);
        }
    }
}
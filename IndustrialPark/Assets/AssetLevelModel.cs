using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetLevelModel : RenderableAsset
    { 
        RenderWareModelFile model;

        public AssetLevelModel(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(bool defaultMode = true)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName)
            {
                rwChunkList = RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile)
            };
            model.SetForRendering();
            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public new void Draw()
        {
            model.Render(Matrix.Identity);
        }
    }
}
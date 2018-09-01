using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMODL : AssetWithModel
    {
        RenderWareModelFile model;

        public AssetMODL(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile), AHDR.containedFile);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override void Draw(SharpRenderer renderer, Matrix world, bool isSelected)
        {
            model.Render(renderer, world, isSelected);
        }

        public override RenderWareModelFile GetRenderWareModelFile()
        {
            return model;
        }
    }
}
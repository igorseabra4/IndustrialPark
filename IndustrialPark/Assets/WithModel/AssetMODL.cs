using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetMODL : Asset, IAssetWithModel
    {
        RenderWareModelFile model;

        public AssetMODL(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.data), AHDR.data);
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public void Draw(SharpRenderer renderer, Matrix world, bool isSelected)
        {
            model.Render(renderer, world, isSelected);
        }

        public RenderWareModelFile GetRenderWareModelFile()
        {
            return model;
        }
    }
}
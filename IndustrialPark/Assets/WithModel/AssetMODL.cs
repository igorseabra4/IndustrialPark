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

        public void Draw(SharpRenderer renderer, Matrix world, Vector4 color)
        {
            model.Render(renderer, world, color);
        }

        public RenderWareModelFile GetRenderWareModelFile()
        {
            return model;
        }

        public bool HasRenderWareModelFile()
        {
            return model != null;
        }
    }
}
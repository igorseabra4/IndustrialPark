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

        public override void Setup(bool defaultMode = true)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName)
            {
                rwChunkList = RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile)
            };

            model.SetForRendering();
            ArchiveEditorFunctions.AddToRenderingDictionary(AHDR.assetID, this);
        }

        public override void Draw(Matrix world)
        {
            model.Render(world);
        }
    }
}
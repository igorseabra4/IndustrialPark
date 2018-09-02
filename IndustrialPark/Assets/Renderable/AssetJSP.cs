using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetJSP : Asset, IRenderableAsset
    {
        private BoundingBox boundingBox;

        public RenderWareModelFile model;
        public static bool dontRender = false;

        public AssetJSP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup(SharpRenderer renderer)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile), AHDR.containedFile);
            
            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public void CreateTransformMatrix()
        {
            boundingBox = BoundingBox.FromPoints(model.GetVertexList().ToArray());
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public void Draw(SharpRenderer renderer)
        {
            if (dontRender) return;

            model.Render(renderer, Matrix.Identity, isSelected);
        }
    }
}
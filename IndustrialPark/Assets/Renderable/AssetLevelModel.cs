using HipHopFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetJSP : RenderableAsset
    { 
        public RenderWareModelFile model;
        public static bool dontRender = false;
        public static bool dontRenderCollision;

        public AssetJSP(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            model = new RenderWareModelFile(AHDR.ADBG.assetName);
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile), AHDR.containedFile);

            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void CreateTransformMatrix()
        {
            world = Matrix.Identity;
            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(model.GetVertexList().ToArray());
        }

        public override void Draw(SharpRenderer renderer)
        {
            if ((dontRender & !model.isCollision) | (dontRenderCollision & model.isCollision)) return;

            model.Render(renderer, world, isSelected);
        }
        
        protected override float? TriangleIntersection(Ray r, float initialDistance)
        {
            float? smallestDistance = null;

            foreach (RenderWareFile.Triangle t in model.triangleList)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(model.vertexListG[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(model.vertexListG[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(model.vertexListG[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                    if (distance < smallestDistance)
                        smallestDistance = distance;
            }

            return smallestDistance;
        }
    }
}
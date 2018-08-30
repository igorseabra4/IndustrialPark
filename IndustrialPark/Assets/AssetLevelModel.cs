using HipHopFile;
using SharpDX;
using System.Collections.Generic;

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
            //model.ignoreNormals = true;
            model.SetForRendering(renderer.device, RenderWareFile.ReadFileMethods.ReadRenderWareFile(AHDR.containedFile), AHDR.containedFile);

            boundingBox = CreateBoundingBox();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public override void Draw(SharpRenderer renderer)
        {
            model.Render(renderer, Matrix.Identity, isSelected);
        }

        protected override BoundingBox CreateBoundingBox()
        {
            return BoundingBox.FromPoints(model.GetVertexList().ToArray());
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
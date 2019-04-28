using System;
using HipHopFile;
using RenderWareFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetJSP : AssetRenderWareModel, IRenderableAsset
    {
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public AssetJSP(Section_AHDR AHDR) : base(AHDR) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.renderableAssetSetJSP.Add(this);
        }

        public void CreateTransformMatrix()
        {
            boundingBox = BoundingBox.FromPoints(model.vertexListG.ToArray());
        }

        public BoundingBox GetBoundingBox()
        {
            return boundingBox;
        }

        public float GetDistance(Vector3 cameraPosition)
        {
            return 0f;
        }
        
        public void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible) return;

            model.Render(renderer, Matrix.Identity, isSelected ? renderer.selectedObjectColor : Vector4.One);
        }

        public float? IntersectsWith(Ray ray)
        {
            if (dontRender || isInvisible)
                return null;

            return TriangleIntersection(ray);
        }

        private float? TriangleIntersection(Ray r)
        {
            bool hasIntersected = false;
            float smallestDistance = 2000f;

            foreach (Triangle t in model.triangleList)
            {
                Vector3 v1 = model.vertexListG[t.vertex1];
                Vector3 v2 = model.vertexListG[t.vertex2];
                Vector3 v3 = model.vertexListG[t.vertex3];

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            return null;
        }
    }
}
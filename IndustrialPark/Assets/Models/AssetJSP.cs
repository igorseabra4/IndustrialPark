using HipHopFile;
using RenderWareFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetJSP : AssetRenderWareModel, IRenderableAsset
    {
        private BoundingBox boundingBox;

        public static bool dontRender = false;

        public AssetJSP(string assetName, AssetType assetType, byte[] data) : base(assetName, assetType, data) { }

        public AssetJSP(Section_AHDR AHDR, Game game) : base(AHDR, game) { }

        public void CreateTransformMatrix(SharpRenderer renderer)
        {
            base.SetupModel(renderer);
            if (model != null)
                boundingBox = BoundingBox.FromPoints(model.vertexListG.ToArray());
        }

        public float GetDistanceFrom(Vector3 cameraPosition) => 0;

        public bool ShouldDraw(SharpRenderer renderer)
        {
            if (isSelected)
                return true;
            if (dontRender)
                return false;
            if (isInvisible)
                return false;

            return renderer.frustum.Intersects(ref boundingBox);
        }

        public void Draw(SharpRenderer renderer)
        {
            xJSPNodeInfo[] nodeInfos = renderer.Assets.GetFromJspNodeInfos(assetID);
            model.RenderJsp(renderer, Matrix.Identity, isSelected, _dontDrawMeshNumber, nodeInfos);
        }

        public float? GetIntersectionPosition(SharpRenderer renderer, Ray ray)
        {
            if (!ShouldDraw(renderer))
                return null;

            float? smallestDistance = null;

            foreach (Triangle t in model.triangleList)
            {
                Vector3 v1 = model.vertexListG[t.vertex1];
                Vector3 v2 = model.vertexListG[t.vertex2];
                Vector3 v3 = model.vertexListG[t.vertex3];

                if (ray.Intersects(ref v1, ref v2, ref v3, out float distance))
                    if (smallestDistance == null || distance < smallestDistance)
                        smallestDistance = distance;
            }

            return smallestDistance;
        }

        [Browsable(false)]
        public bool SpecialBlendMode(SharpRenderer renderer) => false;
    }
}
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

        public AssetJSP(string assetName, AssetType assetType, byte[] data, SharpRenderer renderer) : base(assetName, assetType, data, renderer) { }

        public AssetJSP(Section_AHDR AHDR, Game game, Endianness endianness, SharpRenderer renderer) : base(AHDR, game, endianness, renderer) { }

        public override void Setup(SharpRenderer renderer)
        {
            base.Setup(renderer);
            CreateTransformMatrix();
            ArchiveEditorFunctions.AddToRenderableJSPs(this);
        }

        public void CreateTransformMatrix()
        {
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
            ArchiveEditorFunctions.jspInfoNodeInfo.TryGetValue(assetID, out xJSPNodeInfo[] nodeFlags);
            model.RenderJsp(renderer, Matrix.Identity, isSelected, _dontDrawMeshNumber, nodeFlags);
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
        public bool SpecialBlendMode => false;

        public override void ApplyScale(Vector3 factor)
        {
            base.ApplyScale(factor);
            if (model != null)
                CreateTransformMatrix();
        }
    }
}
using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPLYR : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender => dontRender;

        protected override int EventStartOffset => 0x54 + Offset;

        public AssetPLYR(Section_AHDR AHDR) : base(AHDR) { }

        public override bool HasReference(uint assetID)
        {
            if (LightKitID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        protected override void CreateBoundingBox()
        {
            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], world);

            boundingBox = BoundingBox.FromPoints(vertices);
        }

        protected override float? TriangleIntersection(Ray r, float distance)
        {
            return TriangleIntersection(r, distance, SharpRenderer.pyramidTriangles, SharpRenderer.pyramidVertices);
        }

        private float? TriangleIntersection(Ray r, float initialDistance, List<Triangle> triangles, List<Vector3> vertices)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world);
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world);
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world);

                if (r.Intersects(ref v1, ref v2, ref v3, out float distance))
                {
                    hasIntersected = true;

                    if (distance < smallestDistance)
                        smallestDistance = distance;
                }
            }

            if (hasIntersected)
                return smallestDistance;
            else return null;
        }

        public override void Draw(SharpRenderer renderer)
        {
            if (dontRender || isInvisible) return;

            renderer.DrawPyramid(world, isSelected, 1f);
        }

        [Category("Player References")]
        public AssetID LightKitID
        {
            get => ReadUInt(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct);
            set => Write(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct, value);
        }
    }
}
using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class AssetPLYR : PlaceableAsset
    {
        public static bool dontRender = false;

        protected override bool DontRender()
        {
            return dontRender;
        }

        protected override int getEventStartOffset()
        {
            return 0x54 + Offset;
        }

        public AssetPLYR(Section_AHDR AHDR) : base(AHDR) { }
        
        protected override void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.cubeVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected override float? TriangleIntersection(Ray r, float distance)
        {
            return TriangleIntersection(r, distance, SharpRenderer.cubeTriangles, SharpRenderer.cubeVertices);
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
            if (dontRender) return;

            renderer.DrawCube(world, isSelected);
        }

        public AssetID LightKitID
        {
            get { return ReadUInt(getEventStartOffset() + AmountOfEvents * AssetEvent.sizeOfStruct); }
            set { Write(getEventStartOffset() + AmountOfEvents * AssetEvent.sizeOfStruct, value); }
        }
    }
}
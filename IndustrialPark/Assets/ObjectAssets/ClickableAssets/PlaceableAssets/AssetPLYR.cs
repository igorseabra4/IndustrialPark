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

        protected override int EventStartOffset
        {
            get => 0x54 + Offset;
        }

        public AssetPLYR(Section_AHDR AHDR) : base(AHDR) { }

        public override void CreateTransformMatrix()
        {
            world = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(_position + new Vector3(0f, 0.5f, 0f));

            CreateBoundingBox();
        }

        protected override void CreateBoundingBox()
        {
            boundingBox = BoundingBox.FromPoints(SharpRenderer.pyramidVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
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
            if (dontRender) return;

            renderer.DrawPyramid(world, isSelected, 1f);
        }

        public AssetID LightKitID
        {
            get => ReadUInt(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct);
            set => Write(EventStartOffset + AmountOfEvents * AssetEvent.sizeOfStruct, value);
        }
    }
}
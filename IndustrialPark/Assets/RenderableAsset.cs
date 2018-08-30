using HipHopFile;
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public abstract class RenderableAsset : Asset
    {
        public RenderableAsset(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public byte[] Data
        {
            get { return AHDR.containedFile; }
            set { AHDR.containedFile = value; }
        }
                
        public Matrix world;
                
        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public virtual void CreateTransformMatrix()
        {
            world = Matrix.Identity;

            boundingBox = CreateBoundingBox();
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, world);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, world);
        }

        protected abstract BoundingBox CreateBoundingBox();

        public abstract void Draw(SharpRenderer renderer);

        public BoundingBox boundingBox;

        public virtual float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected float? TriangleIntersection(Ray r, float initialDistance, List<Models.Triangle> triangles, List<Vector3> vertices, float multiplier = 1f)
        {
            bool hasIntersected = false;
            float smallestDistance = 1000f;

            foreach (Models.Triangle t in triangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(vertices[t.vertex1], world) * multiplier;
                Vector3 v2 = (Vector3)Vector3.Transform(vertices[t.vertex2], world) * multiplier;
                Vector3 v3 = (Vector3)Vector3.Transform(vertices[t.vertex3], world) * multiplier;

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

        protected abstract float? TriangleIntersection(Ray r, float initialDistance);
    }
}
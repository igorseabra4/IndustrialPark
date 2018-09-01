using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public abstract class RenderableAsset : Asset
    {
        public Matrix world;
        public BoundingBox boundingBox;

        public RenderableAsset(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override void Setup(SharpRenderer renderer, bool defaultMode = true)
        {
            CreateTransformMatrix();

            ArchiveEditorFunctions.renderableAssetSet.Add(this);
        }

        public abstract void CreateTransformMatrix();

        protected abstract void CreateBoundingBox();
        
        public virtual void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }

        public virtual float? IntersectsWith(Ray ray)
        {
            if (ray.Intersects(ref boundingBox, out float distance))
                return TriangleIntersection(ray, distance);
            return null;
        }

        protected abstract float? TriangleIntersection(Ray r, float distance);

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

        public virtual Vector3 GetGizmoCenter()
        {
            return boundingBox.Center;
        }

        public virtual float GetGizmoRadius()
        {
            return Math.Max(Math.Max(boundingBox.Size.X, boundingBox.Size.Y), boundingBox.Size.Z) * 0.9f;
        }
    }
}
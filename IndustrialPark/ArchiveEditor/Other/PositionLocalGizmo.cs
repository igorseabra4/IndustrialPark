using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class PositionLocalGizmo : GizmoBase
    {
        public PositionLocalGizmo(GizmoType type) : base(type)
        {
        }

        public void SetPosition(BoundingSphere Sphere, Matrix Rotation)
        {
            if (Sphere.Radius < 1f)
                Sphere.Radius = 1f;

            switch (type)
            {
                case GizmoType.X:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) *
                        Matrix.RotationY(MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(Sphere.Radius, 0f, 0f)) *
                        Rotation * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Y:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) *
                        Matrix.RotationX(-MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(0f, Sphere.Radius, 0f)) *
                        Rotation * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Z:
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) *
                        Matrix.Translation(new Vector3(0f, 0f, Sphere.Radius)) *
                        Rotation * Matrix.Translation(Sphere.Center);
                    break;
            }

            boundingBox = BoundingBox.FromPoints(SharpRenderer.pyramidVertices.ToArray());
            boundingBox.Maximum = (Vector3)Vector3.Transform(boundingBox.Maximum, transformMatrix);
            boundingBox.Minimum = (Vector3)Vector3.Transform(boundingBox.Minimum, transformMatrix);
        }

        public override SharpMesh Mesh => SharpRenderer.Pyramid;

        public override bool TriangleIntersection(Ray r)
        {
            List<Vector3> pyramidVertices = SharpRenderer.pyramidVertices;

            foreach (Models.Triangle t in SharpRenderer.pyramidTriangles)
            {
                Vector3 v1 = (Vector3)Vector3.Transform(pyramidVertices[t.vertex1], transformMatrix);
                Vector3 v2 = (Vector3)Vector3.Transform(pyramidVertices[t.vertex2], transformMatrix);
                Vector3 v3 = (Vector3)Vector3.Transform(pyramidVertices[t.vertex3], transformMatrix);

                if (r.Intersects(ref v1, ref v2, ref v3))
                    return true;
            }
            return false;
        }
    }
}
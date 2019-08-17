using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class PositionGizmo : GizmoBase
    {
        public PositionGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
        }

        public void SetPosition(BoundingSphere Sphere)
        {
            if (Sphere.Radius < 1f)
                Sphere.Radius = 1f;

            switch (type)
            {
                case GizmoType.X:
                    Sphere.Center.X += Sphere.Radius;
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationY(MathUtil.Pi / 2) * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Y:
                    Sphere.Center.Y += Sphere.Radius;
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.RotationX(-MathUtil.Pi / 2) * Matrix.Translation(Sphere.Center);
                    break;
                case GizmoType.Z:
                    Sphere.Center.Z += Sphere.Radius;
                    transformMatrix = Matrix.Scaling(Sphere.Radius / 2f) * Matrix.Translation(Sphere.Center);
                    break;
            }

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override SharpMesh Mesh => SharpRenderer.Pyramid;

        protected override List<Models.Triangle> triangleList => SharpRenderer.pyramidTriangles;
    }
}
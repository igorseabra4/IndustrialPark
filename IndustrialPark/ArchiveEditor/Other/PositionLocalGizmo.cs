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

            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }
        
        public override SharpMesh Mesh => SharpRenderer.Pyramid;

        protected override List<Models.Triangle> triangleList => SharpRenderer.pyramidTriangles;
    }
}
using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class PositionLocalGizmo : GizmoBase
    {
        public PositionLocalGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
        }

        public void SetPosition(Vector3 Position, float Radius, Matrix Rotation)
        {
            switch (type)
            {
                case GizmoType.X:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationY(MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(Radius, 0f, 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Y:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationX(-MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(0f, Radius, 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Z:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.Translation(new Vector3(0f, 0f, Radius)) *
                        Rotation * Matrix.Translation(Position);
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
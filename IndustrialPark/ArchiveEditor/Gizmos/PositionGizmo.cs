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

        public void SetPosition(Vector3 Position, float Radius)
        {
            switch (type)
            {
                case GizmoType.X:
                    Position.X += Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationY(MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.Y:
                    Position.Y += Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationX(-MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.Z:
                    Position.Z += Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.Translation(Position);
                    break;

                case GizmoType.TrigX1:
                    Position.X -= Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationY(-MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigY1:
                    Position.Y -= Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationX(MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigZ1:
                    Position.Z -= Radius;
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(Position);
                    break;
            }

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], transformMatrix);
            boundingBox = BoundingBox.FromPoints(vertices);
        }

        public void SetPosition(BoundingBox boundingBox, float Distance)
        {
            Vector3 Position = boundingBox.Center;
            switch (type)
            {
                case GizmoType.X:
                    Position.X = boundingBox.Maximum.X + Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.RotationY(MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.Y:
                    Position.Y = boundingBox.Minimum.Y - Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.RotationX(MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.Z:
                    Position.Z = boundingBox.Maximum.Z + Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.Translation(Position);
                    break;

                case GizmoType.TrigX1:
                    Position.X = boundingBox.Minimum.X - Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.RotationY(-MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigY1:
                    Position.Y = boundingBox.Maximum.Y + Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.RotationX(-MathUtil.Pi / 2) * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigZ1:
                    Position.Z = boundingBox.Minimum.Z - Distance;
                    transformMatrix = Matrix.Scaling(Distance) * Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(Position);
                    break;
            }

            for (int i = 0; i < SharpRenderer.pyramidVertices.Count; i++)
                vertices[i] = (Vector3)Vector3.Transform(SharpRenderer.pyramidVertices[i], transformMatrix);
            base.boundingBox = BoundingBox.FromPoints(vertices);
        }

        public override SharpMesh Mesh => SharpRenderer.Pyramid;

        protected override List<Models.Triangle> triangleList => SharpRenderer.pyramidTriangles;
    }
}
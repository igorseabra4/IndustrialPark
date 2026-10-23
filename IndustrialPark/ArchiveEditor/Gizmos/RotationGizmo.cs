using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RotationGizmo(GizmoType type) : GizmoBase(type)
    {
        public void SetPosition(Vector3 Position, float Radius, Matrix Rotation)
        {
            switch (Type)
            {
                case GizmoType.Yaw:
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Pitch:
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationZ(MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Roll:
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationX(MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    break;
            }

            ResetBounds();
        }

        public override SharpMesh Mesh => SharpRenderer.Torus;
        protected override List<Models.Triangle> triangleList => SharpRenderer.torusTriangles;
        protected override List<Vector3> vertexList => SharpRenderer.torusVertices;
        protected override bool drawLine => false;
    }
}
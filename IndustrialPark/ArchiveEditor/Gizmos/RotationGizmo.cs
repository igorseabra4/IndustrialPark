using SharpDX;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class RotationGizmo(GizmoType type) : GizmoBase(type)
    {
        public void SetPosition(Vector3 Position, float distance, Matrix Rotation)
        {
            transformMatrix = Matrix.Scaling(distance / 4f, distance / 12f, distance / 4f);
            switch (Type)
            {
                case GizmoType.Pitch:
                    transformMatrix *= Matrix.RotationZ(MathUtil.PiOverTwo);
                    break;
                case GizmoType.Roll:
                    transformMatrix *= Matrix.RotationX(MathUtil.PiOverTwo);
                    break;
            }
            transformMatrix *= Rotation * Matrix.Translation(Position);

            ResetBounds(Rotation);
        }

        public override SharpMesh Mesh => SharpRenderer.Torus;
        protected override List<Models.Triangle> triangleList => SharpRenderer.torusTriangles;
        protected override List<Vector3> vertexList => SharpRenderer.torusVertices;
        protected override bool drawLine => false;
    }
}
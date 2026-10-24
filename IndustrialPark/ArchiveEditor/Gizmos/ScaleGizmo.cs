using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class ScaleGizmo : GizmoBase
    {
        public ScaleGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.cubeVertices.Count];
        }

        public void SetPosition(Vector3 Position, float distance, Matrix Rotation)
        {
            distance = Math.Abs(distance / 5f);

            lineTransformMatrix = Matrix.Scaling(distance);
            transformMatrix = Matrix.Scaling(distance / 4f);

            switch (Type)
            {
                case GizmoType.ScaleX:
                    lineTransformMatrix *= Matrix.RotationY(MathUtil.PiOverTwo);
                    transformMatrix *= Matrix.Translation(distance, 0f, 0f);
                    break;
                case GizmoType.ScaleY:
                    lineTransformMatrix *= Matrix.RotationX(-MathUtil.PiOverTwo);
                    transformMatrix *= Matrix.Translation(0f, distance, 0f);
                    break;
                case GizmoType.ScaleZ:
                    transformMatrix *= Matrix.Translation(0f, 0f, distance);
                    break;
                case GizmoType.ScaleAll:
                    transformMatrix = Matrix.Scaling(distance / 5f);
                    break;
            }

            lineTransformMatrix *= Rotation * Matrix.Translation(Position);
            transformMatrix *= Rotation * Matrix.Translation(Position);

            ResetBounds(Rotation);
        }

        public override SharpMesh Mesh => SharpRenderer.Cube;
        protected override List<Vector3> vertexList => SharpRenderer.cubeVertices;
        protected override List<Models.Triangle> triangleList => SharpRenderer.cubeTriangles;
        protected override bool drawLine => Type != GizmoType.ScaleAll;
    }
}
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

        public void SetPosition(Vector3 Position, float Radius, Matrix Rotation)
        {
            Radius = Math.Abs(Radius);

            switch (Type)
            {
                case GizmoType.ScaleX:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Matrix.RotationY(MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 4f) * Matrix.Translation(Radius, 0f, 0f) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.ScaleY:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Matrix.RotationX(-MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 4f) * Matrix.Translation(0f, Radius, 0f) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.ScaleZ:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 4f) * Matrix.Translation(0f, 0f, Radius) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.ScaleAll:
                    transformMatrix = Matrix.Scaling(Radius / 6f) * Rotation * Matrix.Translation(Position);
                    break;
            }

            ResetBounds(Rotation);
        }

        public override SharpMesh Mesh => SharpRenderer.Cube;
        protected override List<Vector3> vertexList => SharpRenderer.cubeVertices;
        protected override List<Models.Triangle> triangleList => SharpRenderer.cubeTriangles;
        protected override bool drawLine => Type != GizmoType.ScaleAll;
    }
}
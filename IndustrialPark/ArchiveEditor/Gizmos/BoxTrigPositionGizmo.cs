using SharpDX;
using System;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class BoxTrigPositionGizmo : GizmoBase
    {
        public BoxTrigPositionGizmo(GizmoType type) : base(type)
        {
            vertices = new Vector3[SharpRenderer.pyramidVertices.Count];
        }

        public void SetPosition(Vector3 Position, Vector3 Size, float Radius, Matrix Rotation)
        {
            switch (type)
            {
                case GizmoType.X:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationY(MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(Radius + Math.Abs(Size.X), 0f, 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Y:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationX(-MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(0f, Radius + Math.Abs(Size.Y), 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Z:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.Translation(new Vector3(0f, 0f, Radius + Math.Abs(Size.Z))) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigX1:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationY(-MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(-Radius - Math.Abs(Size.X), 0f, 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigY1:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationX(MathUtil.Pi / 2) *
                        Matrix.Translation(new Vector3(0f, -Radius - Math.Abs(Size.Y), 0f)) *
                        Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.TrigZ1:
                    transformMatrix = Matrix.Scaling(Radius / 2f) *
                        Matrix.RotationX(MathUtil.Pi) *
                        Matrix.Translation(new Vector3(0f, 0f, -Radius - Math.Abs(Size.Z))) *
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
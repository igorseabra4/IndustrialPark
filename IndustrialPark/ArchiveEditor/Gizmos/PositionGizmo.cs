using SharpDX;

namespace IndustrialPark
{
    public class PositionGizmo(GizmoType type) : GizmoBase(type)
    {
        public void SetPosition(Vector3 Position, float Radius, Matrix Rotation)
        {
            switch (Type)
            {
                case GizmoType.X:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Matrix.RotationY(MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationY(MathUtil.PiOverTwo) * Matrix.Translation(Radius, 0f, 0f) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Y:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Matrix.RotationX(-MathUtil.PiOverTwo) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0f, Radius, 0f) * Rotation * Matrix.Translation(Position);
                    break;
                case GizmoType.Z:
                    lineTransformMatrix = Matrix.Scaling(Radius) * Rotation * Matrix.Translation(Position);
                    transformMatrix = Matrix.Scaling(Radius / 2f) * Matrix.Translation(0f, 0f, Radius) * Rotation * Matrix.Translation(Position);
                    break;
            }

            ResetBounds(Rotation);
        }
    }
}
using SharpDX;

namespace IndustrialPark
{
    public class PositionGizmo(GizmoType type) : GizmoBase(type)
    {
        public void SetPosition(Vector3 Position, float distance, Matrix Rotation)
        {
            distance /= 4f;

            lineTransformMatrix = Matrix.Scaling(distance * 3f / 4f);
            transformMatrix = Matrix.Scaling(distance / 4f, distance / 4f, distance / 2f);

            switch (Type)
            {
                case GizmoType.X:
                    lineTransformMatrix *= Matrix.RotationY(MathUtil.PiOverTwo);
                    transformMatrix *= Matrix.RotationY(MathUtil.PiOverTwo) * Matrix.Translation(distance, 0f, 0f);
                    break;
                case GizmoType.Y:
                    lineTransformMatrix *= Matrix.RotationX(-MathUtil.PiOverTwo);
                    transformMatrix *= Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0f, distance, 0f);
                    break;
                case GizmoType.Z:
                    transformMatrix *= Matrix.Translation(0f, 0f, distance);
                    break;
            }

            lineTransformMatrix *= Rotation * Matrix.Translation(Position);
            transformMatrix *= Rotation * Matrix.Translation(Position);

            ResetBounds(Rotation);
        }
    }
}
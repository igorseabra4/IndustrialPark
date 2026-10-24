using SharpDX;
using System;

namespace IndustrialPark;

public class BoxTrigPositionGizmo(GizmoType type) : GizmoBase(type)
{
    protected override bool drawLine => false;

    public void SetPosition(Vector3 Position, Vector3 Size, float distance, Matrix Rotation)
    {
        distance /= 5f;
        transformMatrix = Matrix.Scaling(distance / 2f);
        switch (Type)
        {
            case GizmoType.X:
                transformMatrix *=
                    Matrix.RotationY(MathUtil.PiOverTwo) *
                    Matrix.Translation(distance + Math.Abs(Size.X), 0f, 0f);
                break;
            case GizmoType.Y:
                transformMatrix *=
                    Matrix.RotationX(-MathUtil.PiOverTwo) *
                    Matrix.Translation(0f, distance + Math.Abs(Size.Y), 0f);
                break;
            case GizmoType.Z:
                transformMatrix *=
                    Matrix.Translation(0f, 0f, distance + Math.Abs(Size.Z));
                break;
            case GizmoType.MinX:
                transformMatrix *=
                    Matrix.RotationY(-MathUtil.PiOverTwo) *
                    Matrix.Translation(-distance - Math.Abs(Size.X), 0f, 0f);
                break;
            case GizmoType.MinY:
                transformMatrix *=
                    Matrix.RotationX(MathUtil.PiOverTwo) *
                    Matrix.Translation(0f, -distance - Math.Abs(Size.Y), 0f);
                break;
            case GizmoType.MinZ:
                transformMatrix *=
                    Matrix.RotationX(MathUtil.Pi) *
                    Matrix.Translation(0f, 0f, -distance - Math.Abs(Size.Z));
                break;
        }
        transformMatrix *= Rotation * Matrix.Translation(Position);

        ResetBounds(Rotation);
    }
}
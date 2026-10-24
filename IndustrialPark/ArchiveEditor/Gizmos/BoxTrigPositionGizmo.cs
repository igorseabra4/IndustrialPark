using SharpDX;
using System;

namespace IndustrialPark;

public class BoxTrigPositionGizmo(GizmoType type) : GizmoBase(type)
{
    protected override bool drawLine => false;

    public void SetPosition(Vector3 Position, Vector3 Size, float Radius, Matrix Rotation)
    {
        switch (Type)
        {
            case GizmoType.X:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.RotationY(MathUtil.PiOverTwo) *
                    Matrix.Translation(new Vector3(Radius + Math.Abs(Size.X), 0f, 0f)) *
                    Rotation * Matrix.Translation(Position);
                break;
            case GizmoType.Y:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.RotationX(-MathUtil.PiOverTwo) *
                    Matrix.Translation(new Vector3(0f, Radius + Math.Abs(Size.Y), 0f)) *
                    Rotation * Matrix.Translation(Position);
                break;
            case GizmoType.Z:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.Translation(new Vector3(0f, 0f, Radius + Math.Abs(Size.Z))) *
                    Rotation * Matrix.Translation(Position);
                break;
            case GizmoType.MinX:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.RotationY(-MathUtil.PiOverTwo) *
                    Matrix.Translation(new Vector3(-Radius - Math.Abs(Size.X), 0f, 0f)) *
                    Rotation * Matrix.Translation(Position);
                break;
            case GizmoType.MinY:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.RotationX(MathUtil.PiOverTwo) *
                    Matrix.Translation(new Vector3(0f, -Radius - Math.Abs(Size.Y), 0f)) *
                    Rotation * Matrix.Translation(Position);
                break;
            case GizmoType.MinZ:
                transformMatrix = Matrix.Scaling(Radius / 2f) *
                    Matrix.RotationX(MathUtil.Pi) *
                    Matrix.Translation(new Vector3(0f, 0f, -Radius - Math.Abs(Size.Z))) *
                    Rotation * Matrix.Translation(Position);
                break;
        }

        ResetBounds(Rotation);
    }
}
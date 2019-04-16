using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private static PositionGizmo[] positionGizmos;
        private static RotationGizmo[] rotationGizmos;
        private static ScaleGizmo[] scaleGizmos;
        private static PositionLocalGizmo[] positionLocalGizmos;

        public static void SetUpGizmos()
        {
            positionGizmos = new PositionGizmo[3]{
                new PositionGizmo(GizmoType.X),
                new PositionGizmo(GizmoType.Y),
                new PositionGizmo(GizmoType.Z)};

            rotationGizmos = new RotationGizmo[3]{
                new RotationGizmo(GizmoType.Yaw),
                new RotationGizmo(GizmoType.Pitch),
                new RotationGizmo(GizmoType.Roll)};

            scaleGizmos = new ScaleGizmo[4]{
                new ScaleGizmo(GizmoType.ScaleX),
                new ScaleGizmo(GizmoType.ScaleY),
                new ScaleGizmo(GizmoType.ScaleZ),
                new ScaleGizmo(GizmoType.ScaleAll)};

            positionLocalGizmos = new PositionLocalGizmo[3]{
                new PositionLocalGizmo(GizmoType.X),
                new PositionLocalGizmo(GizmoType.Y),
                new PositionLocalGizmo(GizmoType.Z)};
        }

        private static bool DrawGizmos = false;
        public static GizmoMode CurrentGizmoMode { get; private set; } = GizmoMode.Position;
        public static bool FinishedMovingGizmo = false;

        public static void RenderGizmos(SharpRenderer renderer)
        {
            if (DrawGizmos)
                switch (CurrentGizmoMode)
                {
                    case GizmoMode.Position:
                        foreach (PositionGizmo g in positionGizmos)
                            g.Draw(renderer);
                        break;
                    case GizmoMode.Rotation:
                        for (int i = 2; i >= 0; i--)
                            rotationGizmos[i].Draw(renderer);
                        break;
                    case GizmoMode.Scale:
                        foreach (ScaleGizmo g in scaleGizmos)
                            g.Draw(renderer);
                        break;
                    case GizmoMode.PositionLocal:
                        foreach (PositionLocalGizmo g in positionLocalGizmos)
                            g.Draw(renderer);
                        break;
                }
        }

        public static void UpdateGizmoPosition()
        {
            DrawGizmos = false;
            // Position Gizmos
            {
                BoundingSphere bs = new BoundingSphere();
                bool found = false;

                foreach (IClickableAsset a in allCurrentlySelectedAssets.OfType<IClickableAsset>())
                    if (!found)
                    {
                        found = true;
                        bs = a.GetGizmoCenter();
                    }
                    else
                        bs = BoundingSphere.Merge(bs, a.GetGizmoCenter());
                
                if (found)
                {
                    UpdatePositionGizmo(bs);
                    if (CurrentGizmoMode == GizmoMode.Position)
                        DrawGizmos = true;
                }
            }

            // Rotation Gizmos
            if (allCurrentlySelectedAssets.OfType<IRotatableAsset>().Count() == 1)
            {
                IRotatableAsset ira = allCurrentlySelectedAssets.OfType<IRotatableAsset>().FirstOrDefault();
                SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);
                UpdateRotationGizmo(ira.GetObjectCenter());
                if (CurrentGizmoMode == GizmoMode.Rotation)
                    DrawGizmos = true;
            }

            // Scale Gizmos
            {
                BoundingBox bb = new BoundingBox();
                bool found = false;

                foreach (IScalableAsset a in allCurrentlySelectedAssets.OfType<IScalableAsset>())
                {
                    if (!found)
                    {
                        found = true;
                        bb = a.GetBoundingBox();
                    }
                    else
                        bb = BoundingBox.Merge(bb, a.GetBoundingBox());

                    if (CurrentGizmoMode == GizmoMode.Scale && a is IRotatableAsset ira)
                        SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);
                }

                if (found)
                {
                    UpdateScaleGizmo(bb);
                    if (CurrentGizmoMode == GizmoMode.Scale)
                        DrawGizmos = true;
                }
            }

            // Position Local Gizmos
            if (allCurrentlySelectedAssets.OfType<IClickableAsset>().Count() == 1)
            {
                UpdatePositionLocalGizmo(allCurrentlySelectedAssets.OfType<IClickableAsset>().FirstOrDefault().GetGizmoCenter());
                if (CurrentGizmoMode == GizmoMode.PositionLocal)
                    DrawGizmos = true;
            }
        }
        
        private static Vector3 GizmoCenterPosition;
        private static Matrix GizmoCenterRotation;
            
        private static void UpdatePositionGizmo(BoundingSphere position)
        {
            GizmoCenterPosition = position.Center;

            foreach (PositionGizmo g in positionGizmos)
                g.SetPosition(position);
        }

        private static void UpdateRotationGizmo(BoundingSphere position)
        {
            GizmoCenterPosition = position.Center;

            foreach (RotationGizmo g in rotationGizmos)
                g.SetPosition(position, GizmoCenterRotation);
        }

        private static void UpdateScaleGizmo(BoundingBox position)
        {
            GizmoCenterPosition = position.Center;

            foreach (ScaleGizmo g in scaleGizmos)
                g.SetPosition(position, GizmoCenterRotation);
        }

        private static void UpdatePositionLocalGizmo(BoundingSphere position)
        {
            GizmoCenterPosition = position.Center;

            foreach (PositionLocalGizmo g in positionLocalGizmos)
                g.SetPosition(position, GizmoCenterRotation);
        }

        private static void SetCenterRotation(float Yaw, float Pitch, float Roll)
        {
            GizmoCenterRotation = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll));
        }

        public static void GizmoSelect(Ray r)
        {
            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                    {
                        float dist = 1000f;
                        int index = -1;

                        for (int g = 0; g < positionGizmos.Length; g++)
                        {
                            float? distance = positionGizmos[g].IntersectsWith(r);
                            if (distance != null)
                            {
                                if (distance < dist)
                                {
                                    dist = (float)distance;
                                    index = g;
                                }
                            }
                        }

                        if (index != -1)
                            positionGizmos[index].isSelected = true;
                    }
                    break;
                case GizmoMode.Rotation:
                    {
                        float dist = 1000f;
                        int index = -1;

                        for (int g = 0; g < rotationGizmos.Length; g++)
                        {
                            float? distance = rotationGizmos[g].IntersectsWith(r);
                            if (distance != null)
                            {
                                if (distance < dist)
                                {
                                    dist = (float)distance;
                                    index = g;
                                }
                            }
                        }

                        if (index != -1)
                            rotationGizmos[index].isSelected = true;
                    }
                    break;
                case GizmoMode.Scale:
                    {
                        float dist = 1000f;
                        int index = -1;

                        for (int g = 0; g < scaleGizmos.Length; g++)
                        {
                            float? distance = scaleGizmos[g].IntersectsWith(r);
                            if (distance != null)
                            {
                                if (distance < dist)
                                {
                                    dist = (float)distance;
                                    index = g;
                                }
                            }
                        }

                        if (index != -1)
                            scaleGizmos[index].isSelected = true;
                    }
                    break;
                case GizmoMode.PositionLocal:
                    {
                        float dist = 1000f;
                        int index = -1;

                        for (int g = 0; g < positionLocalGizmos.Length; g++)
                        {
                            float? distance = positionLocalGizmos[g].IntersectsWith(r);
                            if (distance != null)
                            {
                                if (distance < dist)
                                {
                                    dist = (float)distance;
                                    index = g;
                                }
                            }
                        }

                        if (index != -1)
                            positionLocalGizmos[index].isSelected = true;
                    }
                    break;
            }
        }

        public static void ScreenUnclicked()
        {
            foreach (PositionGizmo g in positionGizmos)
                g.isSelected = false;
            foreach (RotationGizmo g in rotationGizmos)
                g.isSelected = false;
            foreach (ScaleGizmo g in scaleGizmos)
                g.isSelected = false;
            foreach (PositionLocalGizmo g in positionLocalGizmos)
                g.isSelected = false;
        }

        public void MouseMoveForPosition(Matrix viewProjection, int distanceX, int distanceY)
        {
            if (positionGizmos[0].isSelected || positionGizmos[1].isSelected || positionGizmos[2].isSelected)
            {
                foreach (Asset a in currentlySelectedAssets)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (a is IClickableAsset ra)
                    {
                        if (positionGizmos[0].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitX, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.PositionX += (distanceX * direction.X - distanceY * direction.Y) / 10;
                        }
                        else if (positionGizmos[1].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.PositionY += (distanceX * direction.X - distanceY * direction.Y) / 10;
                        }
                        else if (positionGizmos[2].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.PositionZ += (distanceX * direction.X - distanceY * direction.Y) / 10;
                        }
                    }

                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForRotation(Matrix viewProjection, int distanceX, int distanceY)
        {
            if (rotationGizmos[0].isSelected || rotationGizmos[1].isSelected || rotationGizmos[2].isSelected)
            {
                foreach (Asset a in currentlySelectedAssets)
                {
                    Vector3 center = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (a is IRotatableAsset ra)
                    {
                        if (rotationGizmos[0].isSelected)
                        {
                            //Vector3 direction1 = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);
                            //Vector3 direction2 = rotationGizmos[0].clickPosition - GizmoCenterPosition;
                            //Vector3 tangent = (Vector3)Vector3.Transform(Vector3.Cross(direction2, direction1), viewProjection);

                            //Vector3 direction = tangent - center;
                            //direction.Z = 0;
                            //direction.Normalize();

                            //ra.Yaw -= (distanceX * direction.X - distanceY * direction.Y) / 10;
                            ra.Yaw += distanceX;
                        }
                        else if (rotationGizmos[1].isSelected)
                        {
                            //Vector3 direction1 = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);
                            //Vector3 direction2 = rotationGizmos[1].clickPosition - GizmoCenterPosition;
                            //Vector3 tangent = (Vector3)Vector3.Transform(Vector3.Cross(direction2, direction1), viewProjection);

                            //Vector3 direction = tangent - center;
                            //direction.Z = 0;
                            //direction.Normalize();

                            //ra.Pitch -= (distanceX * direction.X - distanceY * direction.Y) / 10;
                            ra.Pitch += distanceX;
                        }
                        else if (rotationGizmos[2].isSelected)
                        {
                            //Vector3 direction1 = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);
                            //Vector3 direction2 = rotationGizmos[2].clickPosition - GizmoCenterPosition;
                            //Vector3 tangent = (Vector3)Vector3.Transform(Vector3.Cross(direction2, direction1), viewProjection);

                            //Vector3 direction = tangent - center;
                            //direction.Z = 0;
                            //direction.Normalize();

                            //ra.Roll -= (distanceX * direction.X - distanceY * direction.Y) / 10;
                            ra.Roll += distanceX;
                        }
                    }

                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForScale(Matrix viewProjection, int distanceX, int distanceY)
        {
            if (scaleGizmos[0].isSelected || scaleGizmos[1].isSelected || scaleGizmos[2].isSelected || scaleGizmos[3].isSelected)
            {
                foreach (Asset a in currentlySelectedAssets)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (a is IScalableAsset ra)
                    {
                        if (scaleGizmos[0].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation), viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.ScaleX += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        }
                        else if (scaleGizmos[1].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation), viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.ScaleY += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        }
                        else if (scaleGizmos[2].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation), viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.ScaleZ += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        }
                        else if (scaleGizmos[3].isSelected)
                        {
                            ra.ScaleX += distanceX / 40f;
                            ra.ScaleY += distanceX / 40f;
                            ra.ScaleZ += distanceX / 40f;
                        }
                    }

                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForPositionLocal(Matrix viewProjection, int distanceX, int distanceY)
        {
            if (positionLocalGizmos[0].isSelected || positionLocalGizmos[1].isSelected || positionLocalGizmos[2].isSelected)
            {
                foreach (Asset a in currentlySelectedAssets)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (a is IClickableAsset ra)
                    {
                        Vector3 movementDirection = new Vector3();

                        if (positionLocalGizmos[0].isSelected)
                            movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);
                        else if (positionLocalGizmos[1].isSelected)
                            movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);
                        else if (positionLocalGizmos[2].isSelected)
                            movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        ra.PositionX += movementDirection.X * (distanceX * direction.X - distanceY * direction.Y) / 10f;
                        ra.PositionY += movementDirection.Y * (distanceX * direction.X - distanceY * direction.Y) / 10f;
                        ra.PositionZ += movementDirection.Z * (distanceX * direction.X - distanceY * direction.Y) / 10f;
                    }

                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public static GizmoMode ToggleGizmoType(GizmoMode mode = GizmoMode.Null)
        {
            ScreenUnclicked();

            if (mode == GizmoMode.Null)
            {
                if (CurrentGizmoMode == GizmoMode.Position)
                    CurrentGizmoMode = GizmoMode.Rotation;
                else if (CurrentGizmoMode == GizmoMode.Rotation)
                    CurrentGizmoMode = GizmoMode.Scale;
                else if (CurrentGizmoMode == GizmoMode.Scale)
                    CurrentGizmoMode = GizmoMode.PositionLocal;
                else if (CurrentGizmoMode == GizmoMode.PositionLocal)
                    CurrentGizmoMode = GizmoMode.Position;
            }
            else CurrentGizmoMode = mode;

            return CurrentGizmoMode;
        }
    }
}
using SharpDX;
using System;
using System.Linq;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private static PositionGizmo[] positionGizmos;
        private static BoxTrigPositionGizmo[] triggerPositionGizmos;
        private static RotationGizmo[] rotationGizmos;
        private static ScaleGizmo[] scaleGizmos;
        private static PositionLocalGizmo[] positionLocalGizmos;

        public static void SetUpGizmos()
        {
            positionGizmos = new PositionGizmo[3]{
                new PositionGizmo(GizmoType.X),
                new PositionGizmo(GizmoType.Y),
                new PositionGizmo(GizmoType.Z)};

            triggerPositionGizmos = new BoxTrigPositionGizmo[6]{
                new BoxTrigPositionGizmo(GizmoType.X),
                new BoxTrigPositionGizmo(GizmoType.Y),
                new BoxTrigPositionGizmo(GizmoType.Z),
                new BoxTrigPositionGizmo(GizmoType.TrigX1),
                new BoxTrigPositionGizmo(GizmoType.TrigY1),
                new BoxTrigPositionGizmo(GizmoType.TrigZ1)};

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

            if (Grid.X < 0.001f)
                Grid.X = 1f;
            if (Grid.Y < 0.001f)
                Grid.Y = 1f;
            if (Grid.Z < 0.001f)
                Grid.Z = 1f;
        }

        public static GizmoMode CurrentGizmoMode { get; private set; } = GizmoMode.Position;
        public static bool FinishedMovingGizmo = false;
        public static bool TriggerGizmo = false;

        public static void RenderGizmos(SharpRenderer renderer)
        {
            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                    {
                        if (allCurrentlySelectedAssets.OfType<IClickableAsset>().Count() == 1 &&
                        allCurrentlySelectedAssets.OfType<IClickableAsset>().FirstOrDefault() is AssetTRIG TRIG
                        && TRIG.Shape == TriggerShape.Box)
                        {
                            TriggerGizmo = true;

                            GizmoCenterPosition = TRIG.Position;
                            float distance = Vector3.Distance(renderer.Camera.Position, TRIG.GetBoundingBox().Center) / 5f;

                            foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                            {
                                g.SetPosition(TRIG.GetBoundingBox(), distance);
                                g.Draw(renderer);
                            }

                            distance = Vector3.Distance(renderer.Camera.Position, TRIG.Position) / 5f;

                            foreach (PositionGizmo g in positionGizmos)
                            {
                                g.SetPosition(TRIG.Position, distance);
                                g.Draw(renderer);
                            }
                        }
                        else
                        {
                            TriggerGizmo = false;

                            BoundingBox bb = new BoundingBox();
                            bool found = false;

                            foreach (IClickableAsset a in allCurrentlySelectedAssets.OfType<IClickableAsset>())
                                if (!found && ShouldUseDyna(a))
                                {
                                    found = true;
                                    bb = a.GetBoundingBox();
                                }
                                else if (ShouldUseDyna(a))
                                    bb = BoundingBox.Merge(bb, a.GetBoundingBox());
                            
                            if (found)
                            {
                                GizmoCenterPosition = bb.Center;
                                float distance = Vector3.Distance(renderer.Camera.Position, bb.Center) / 5f;

                                foreach (PositionGizmo g in positionGizmos)
                                {
                                    g.SetPosition(bb.Center, distance);
                                    g.Draw(renderer);
                                }
                            }
                        }
                    }
                    break;
                case GizmoMode.Rotation:
                    if (allCurrentlySelectedAssets.OfType<IRotatableAsset>().Count() != 0)
                    {
                        IRotatableAsset ira = allCurrentlySelectedAssets.OfType<IRotatableAsset>().FirstOrDefault();
                        SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);

                        GizmoCenterPosition = ira.Position;
                        float distance = Vector3.Distance(renderer.Camera.Position, ira.Position) / 2f;

                        for (int i = 2; i >= 0; i--)
                        {
                            rotationGizmos[i].SetPosition(ira.Position, distance, GizmoCenterRotation);
                            rotationGizmos[i].Draw(renderer);
                        }
                    }
                    break;
                case GizmoMode.Scale:
                    if (allCurrentlySelectedAssets.OfType<IScalableAsset>().Count() != 0)
                    {
                        IScalableAsset isa = allCurrentlySelectedAssets.OfType<IScalableAsset>().FirstOrDefault();
                        if (isa is IRotatableAsset ira)
                            SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);
                        else
                            SetCenterRotation(0, 0, 0);

                        GizmoCenterPosition = isa.Position;
                        float distance = Vector3.Distance(renderer.Camera.Position, isa.Position) / 5f;
                        
                        foreach (ScaleGizmo g in scaleGizmos)
                        {
                            g.SetPosition(isa.Position, distance, GizmoCenterRotation);
                            g.Draw(renderer);
                        }
                    }
                    break;
                case GizmoMode.PositionLocal:
                    if (allCurrentlySelectedAssets.OfType<IClickableAsset>().Count() == 1)
                    {
                        if (allCurrentlySelectedAssets.OfType<IClickableAsset>().FirstOrDefault() is AssetTRIG TRIG && TRIG.Shape == TriggerShape.Box)
                            return;

                        IClickableAsset ica = allCurrentlySelectedAssets.OfType<IClickableAsset>().FirstOrDefault();

                        GizmoCenterPosition = ica.GetBoundingBox().Center;

                        float radius = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;

                        foreach (PositionLocalGizmo g in positionLocalGizmos)
                        { 
                            g.SetPosition(ica.GetBoundingBox().Center, radius, GizmoCenterRotation);
                            g.Draw(renderer);
                        }
                    }
                    break;
            }
        }

        private static bool ShouldUseDyna(IClickableAsset a) => !(a is AssetDYNA dyna && !dyna.IsRenderableClickable);
        
        private static Vector3 GizmoCenterPosition;
        private static Matrix GizmoCenterRotation;
        
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

                        if (TriggerGizmo)
                        {
                            for (int g = 0; g < triggerPositionGizmos.Length; g++)
                            {
                                float? distance = triggerPositionGizmos[g].IntersectsWith(r);
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
                                triggerPositionGizmos[index].isSelected = true;
                        }

                        if (index == -1)
                        {
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
            foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                g.isSelected = false;
            foreach (RotationGizmo g in rotationGizmos)
                g.isSelected = false;
            foreach (ScaleGizmo g in scaleGizmos)
                g.isSelected = false;
            foreach (PositionLocalGizmo g in positionLocalGizmos)
                g.isSelected = false;
        }

        private void RefreshAssetEditor(uint assetID)
        {
            foreach (var v in internalEditors)
                if (v.GetAssetID() == assetID)
                    v.RefreshPropertyGrid();
        }

        public void MouseMoveForPosition(Matrix viewProjection, int distanceX, int distanceY, bool grid)
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

                            float movement = distanceX * direction.X - distanceY * direction.Y;
                            if (grid)
                                ra.PositionX = SnapToGrid(ra.PositionX + movement, GizmoType.X);
                            else
                                ra.PositionX += movement / 10;

                            if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                                trig.Position0X = trig.PositionX;
                        }
                        else if (positionGizmos[1].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            float movement = distanceX * direction.X - distanceY * direction.Y;
                            if (grid)
                                ra.PositionY = SnapToGrid(ra.PositionY + movement, GizmoType.Y);
                            else
                                ra.PositionY += movement / 10;

                            if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                                trig.Position0Y = trig.PositionY;
                        }
                        else if (positionGizmos[2].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            float movement = distanceX * direction.X - distanceY * direction.Y;
                            if (grid)
                                ra.PositionZ = SnapToGrid(ra.PositionZ + movement, GizmoType.Z);
                            else
                                ra.PositionZ += movement / 10;

                            if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                                trig.Position0Z = trig.PositionZ;
                        }

                        if (a is AssetDYNA dyna)
                            dyna.OnDynaSpecificPropertyChange(dyna.DynaBase);
                        RefreshAssetEditor(a.AHDR.assetID);
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }

            if (triggerPositionGizmos[0].isSelected || triggerPositionGizmos[1].isSelected || triggerPositionGizmos[2].isSelected
                || triggerPositionGizmos[3].isSelected || triggerPositionGizmos[4].isSelected || triggerPositionGizmos[5].isSelected)
            {
                foreach (Asset a in currentlySelectedAssets)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (a is AssetTRIG ra)
                    {
                        if (triggerPositionGizmos[0].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitX, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position1X += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position1X = SnapToGrid(ra.Position1X, GizmoType.X);
                        }
                        else if (triggerPositionGizmos[1].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position1Y += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position1Y = SnapToGrid(ra.Position1Y, GizmoType.Y);
                        }
                        else if (triggerPositionGizmos[2].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position1Z += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position1Z = SnapToGrid(ra.Position1Z, GizmoType.Z);
                        }
                        else if (triggerPositionGizmos[3].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitX, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position0X += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position0X = SnapToGrid(ra.Position0X, GizmoType.X);
                        }
                        else if (triggerPositionGizmos[4].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position0Y += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position0Y = SnapToGrid(ra.Position0Y, GizmoType.Y);
                        }
                        else if (triggerPositionGizmos[5].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.Position0Z += (distanceX * direction.X - distanceY * direction.Y) / 10;
                            if (grid)
                                ra.Position0Z = SnapToGrid(ra.Position0Z, GizmoType.Z);
                        }
                        RefreshAssetEditor(a.AHDR.assetID);
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForRotation(Matrix viewProjection, int distanceX, bool grid)//, int distanceY)
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
                            if (grid)
                                ra.Yaw = SnapToGrid(ra.Yaw + distanceX, GizmoType.X);
                            else
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
                            if (grid)
                                ra.Pitch = SnapToGrid(ra.Pitch + distanceX, GizmoType.Y);
                            else
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
                            if (grid)
                                ra.Roll = SnapToGrid(ra.Roll + distanceX, GizmoType.Z);
                            else
                                ra.Roll += distanceX;
                        }

                        if (a is AssetDYNA dyna)
                            dyna.OnDynaSpecificPropertyChange(dyna.DynaBase);
                        RefreshAssetEditor(a.AHDR.assetID);
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForScale(Matrix viewProjection, int distanceX, int distanceY, bool grid)
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
                            if (grid)
                                ra.ScaleX = SnapToGrid(ra.ScaleX, GizmoType.X);
                        }
                        else if (scaleGizmos[1].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation), viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.ScaleY += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                            if (grid)
                                ra.ScaleY = SnapToGrid(ra.ScaleY, GizmoType.Y);
                        }
                        else if (scaleGizmos[2].isSelected)
                        {
                            Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation), viewProjection);
                            Vector3 direction = direction2 - direction1;
                            direction.Z = 0;
                            direction.Normalize();

                            ra.ScaleZ += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                            if (grid)
                                ra.ScaleZ = SnapToGrid(ra.ScaleZ, GizmoType.Z);
                        }
                        else if (scaleGizmos[3].isSelected)
                        {
                            ra.ScaleX += distanceX / 40f;
                            ra.ScaleY += distanceX / 40f;
                            ra.ScaleZ += distanceX / 40f;
                        }

                        if (a is AssetDYNA dyna)
                            dyna.OnDynaSpecificPropertyChange(dyna.DynaBase);
                        RefreshAssetEditor(a.AHDR.assetID);
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForPositionLocal(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            if (positionLocalGizmos[0].isSelected || positionLocalGizmos[1].isSelected || positionLocalGizmos[2].isSelected)
            {
                Vector3 movementDirection = new Vector3();

                if (positionLocalGizmos[0].isSelected)
                    movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);
                else if (positionLocalGizmos[1].isSelected)
                    movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);
                else if (positionLocalGizmos[2].isSelected)
                    movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                Vector3 direction = direction2 - (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);
                direction.Z = 0;
                direction.Normalize();

                foreach (Asset a in currentlySelectedAssets)
                {
                    if (a is IClickableAsset ra)
                    {
                        float movement = distanceX * direction.X - distanceY * direction.Y;

                        if (grid)
                        {
                            ra.PositionX = SnapToGrid(ra.PositionX + movementDirection.X * movement, GizmoType.X);
                            ra.PositionY = SnapToGrid(ra.PositionY + movementDirection.Y * movement, GizmoType.Y);
                            ra.PositionZ = SnapToGrid(ra.PositionZ + movementDirection.Z * movement, GizmoType.Z);
                        }
                        else
                        {
                            ra.PositionX += movementDirection.X * movement / 10f;
                            ra.PositionY += movementDirection.Y * movement / 10f;
                            ra.PositionZ += movementDirection.Z * movement / 10f;
                        }

                        if (a is AssetDYNA dyna)
                            dyna.OnDynaSpecificPropertyChange(dyna.DynaBase);

                        if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                        {
                            trig.Position0X = trig.PositionX;
                            trig.Position0Y = trig.PositionY;
                            trig.Position0Z = trig.PositionZ;
                        }
                        RefreshAssetEditor(a.AHDR.assetID);
                    }
                }

                FinishedMovingGizmo = true;
                UnsavedChanges = true;
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

        private float SnapToGrid(float value, GizmoType gizmo)
        {
            if (gizmo == GizmoType.X)
                return RoundToNearest(value, Grid.X);
            if (gizmo == GizmoType.Y)
                return RoundToNearest(value, Grid.Y);
            if (gizmo == GizmoType.Z)
                return RoundToNearest(value, Grid.Z);
            return 0;
        }

        private float RoundToNearest(float n, float x)
        {
            return (float)Math.Round(n / x) * x;
        }

        public static Vector3 Grid;
    }
}
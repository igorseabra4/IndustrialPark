using SharpDX;
using System;
using System.Collections.Generic;
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
                    var icas = allCurrentlySelectedAssets.OfType<IClickableAsset>();
                    if (icas.Count() == 1)
                    {
                        if (icas.FirstOrDefault() is AssetTRIG TRIG && TRIG.Shape == TriggerShape.Box)
                        {
                            TriggerGizmo = true;
                            SetCenterRotation(TRIG.Yaw, TRIG.Pitch, TRIG.Roll);
                            GizmoCenterPosition = TRIG.GetBoundingBox().Center;
                            float radius = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;
                            Vector3 TrigBound = new Vector3(TRIG.MaximumX - TRIG.MinimumX, TRIG.MaximumY - TRIG.MinimumY, TRIG.MaximumZ - TRIG.MinimumZ) / 2f;

                            foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                            {
                                g.SetPosition(TRIG.GetBoundingBox().Center, TrigBound, radius, GizmoCenterRotation);
                                g.Draw(renderer);
                            }

                            var trig_pos = new Vector3(TRIG.PositionX, TRIG.PositionY, TRIG.PositionZ);

                            radius = Vector3.Distance(renderer.Camera.Position, trig_pos) / 5f;

                            foreach (PositionGizmo g in positionGizmos)
                            {
                                g.SetPosition(trig_pos, radius);
                                g.Draw(renderer);
                            }
                            return;
                        }
                        if (icas.FirstOrDefault() is AssetVOLU VOLU && VOLU.VolumeShape is VolumeBox box)
                        {
                            TriggerGizmo = true;
                            SetCenterRotation(0, 0, 0);
                            GizmoCenterPosition = box.GetBoundingBox().Center;
                            float radius = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;
                            Vector3 TrigBound = new Vector3(box.MaximumX - box.MinimumX, box.MaximumY - box.MinimumY, box.MaximumZ - box.MinimumZ) / 2f;

                            foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                            {
                                g.SetPosition(box.GetBoundingBox().Center, TrigBound, radius, GizmoCenterRotation);
                                g.Draw(renderer);
                            }

                            var trig_pos = new Vector3(box.CenterX, box.CenterY, box.CenterZ);

                            radius = Vector3.Distance(renderer.Camera.Position, trig_pos) / 5f;

                            foreach (PositionGizmo g in positionGizmos)
                            {
                                g.SetPosition(trig_pos, radius);
                                g.Draw(renderer);
                            }

                            return;
                        }
                    }

                    TriggerGizmo = false;

                    BoundingBox bb = new BoundingBox();
                    bool found = false;

                    foreach (var a in allCurrentlySelectedAssets.OfType<IClickableAsset>())
                        if (!found)
                        {
                            found = true;
                            bb = a.GetBoundingBox();
                        }
                        else
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
                    return;
                case GizmoMode.Rotation:
                    var iras = allCurrentlySelectedAssets.OfType<IRotatableAsset>();
                    if (iras.Count() != 0)
                    {
                        var ira = iras.FirstOrDefault();
                        SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);

                        var ira_pos = new Vector3(ira.PositionX, ira.PositionY, ira.PositionZ);
                        GizmoCenterPosition = ira_pos;
                        float distance = Vector3.Distance(renderer.Camera.Position, ira_pos) / 2f;

                        for (int i = 2; i >= 0; i--)
                        {
                            rotationGizmos[i].SetPosition(ira_pos, distance, GizmoCenterRotation);
                            rotationGizmos[i].Draw(renderer);
                        }
                    }
                    return;
                case GizmoMode.Scale:
                    var isas = allCurrentlySelectedAssets.OfType<IScalableAsset>();
                    if (isas.Count() != 0)
                    {
                        var isa = isas.FirstOrDefault();
                        if (isa is IRotatableAsset ira)
                            SetCenterRotation(ira.Yaw, ira.Pitch, ira.Roll);
                        else
                            SetCenterRotation(0, 0, 0);

                        var isa_pos = new Vector3(isa.PositionX, isa.PositionY, isa.PositionZ);
                        GizmoCenterPosition = isa_pos;
                        float distance = Vector3.Distance(renderer.Camera.Position, isa_pos) / 5f;

                        foreach (ScaleGizmo g in scaleGizmos)
                        {
                            g.SetPosition(isa_pos, distance, GizmoCenterRotation);
                            g.Draw(renderer);
                        }
                    }
                    return;
                case GizmoMode.PositionLocal:
                    var icas2 = allCurrentlySelectedAssets.OfType<IClickableAsset>();
                    if (icas2.Count() == 1)
                    {
                        var ica = icas2.FirstOrDefault();

                        if (ica is AssetTRIG TRIG && TRIG.Shape == TriggerShape.Box)
                            return;
                        if (ica is AssetVOLU VOLU && VOLU.Shape == VolumeType.Box)
                            return;

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
                    float dist = 10000f;
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
            
            // Reset the transformation values
            totalDistanceMoved = 0;
            totalDistanceRotated = 0;
        }

        private void RefreshAssetEditors()
        {
            foreach (var v in internalEditors)
                v.RefreshPropertyGrid();
        }

        private void RefreshAssetEditor(uint assetID)
        {
            // Constantly refreshing the asset editor windows isn't very fast, and it blocks the renderer...
            // ...so the transformation won't appear in the editor until the mouse is released
            // To mitigate this, we can only call this function if the asset was modified (scaled/moved/rotated)
            // It's still a little slow though and dips the framerate - Issue #79
            // A better solution would be to only refresh the modified properties/limit the number of refreshes
            // but this works for now
            
            foreach (var v in internalEditors)
                if (v.GetAssetID() == assetID)
                    v.RefreshPropertyGrid();
        }

        // Used for snapping to grid when moving assets (Issue #63)
        // Sum the total distance moved for each mouse move action (resets on mouse up)
        // Find the "target position" by adding the original position to the total distance moved
        // Snap the target position to the grid (quantize)
        private static float totalDistanceMoved;
        private static float originalPosition;
        private static float movementScale = 0.1f;
        
        public void MouseMoveForPosition(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            bool assetModified = false;
            
            if (positionGizmos[0].isSelected || positionGizmos[1].isSelected || positionGizmos[2].isSelected)
            {
                var selectedClickableAssets = from Asset a in CurrentlySelectedAssets where a is IClickableAsset ica select (IClickableAsset)a;
                if (!selectedClickableAssets.Any())
                    return;

                Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                foreach (var ra in selectedClickableAssets)
                {
                    if (positionGizmos[0].isSelected) // X MOVEMENT
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitX, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.PositionX;
                        
                        float movement = distanceX * direction.X - distanceY * direction.Y;
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        if (grid)
                        {
                            var prevPos = ra.PositionX;
                            ra.PositionX = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.X);
                            if (ra.PositionX != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.PositionX += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }

                        if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                            trig.MinimumX = trig.PositionX;
                    }
                    else if (positionGizmos[1].isSelected) // Y MOVEMENT
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();
                        
                        if (totalDistanceMoved == 0)
                            originalPosition = ra.PositionY;

                        float movement = distanceX * direction.X - distanceY * direction.Y;

                        if (ra is AssetUI)
                            movement *= -1;
                        
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        if (grid)
                        {
                            var prevPos = ra.PositionY;
                            ra.PositionY = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Y);
                            if (ra.PositionY != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.PositionY += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }

                        if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                            trig.MinimumY = trig.PositionY;
                    }
                    else if (positionGizmos[2].isSelected) // Z MOVEMENT
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();
                        
                        if (totalDistanceMoved == 0)
                            originalPosition = ra.PositionZ;

                        float movement = distanceX * direction.X - distanceY * direction.Y;
                        
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        if (grid)
                        {
                            var prevPos = ra.PositionZ;
                            ra.PositionZ = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Z);
                            if (ra.PositionZ != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.PositionZ += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }

                        if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                            trig.MinimumZ = trig.PositionZ;
                    }

                    if (assetModified)
                    {
                        RefreshAssetEditor(((Asset)ra).assetID);
                        assetModified = false;
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }

            // Volumes (Box Triggers)
            if (triggerPositionGizmos[0].isSelected || triggerPositionGizmos[1].isSelected || triggerPositionGizmos[2].isSelected
                || triggerPositionGizmos[3].isSelected || triggerPositionGizmos[4].isSelected || triggerPositionGizmos[5].isSelected)
            {
                var selectedVolumes = new List<IVolumeAsset>();
                selectedVolumes.AddRange((from a in CurrentlySelectedAssets where a is AssetTRIG trig && trig.Shape == TriggerShape.Box select (AssetTRIG)a).ToList());
                selectedVolumes.AddRange((from a in CurrentlySelectedAssets where a is AssetVOLU volu && volu.VolumeShape is VolumeBox select (VolumeBox)((AssetVOLU)a).VolumeShape).ToList());

                if (!selectedVolumes.Any())
                    return;

                foreach (IVolumeAsset ra in selectedVolumes)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (triggerPositionGizmos[0].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MaximumX;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        if (grid)
                        {
                            var prevPos = ra.MaximumX;
                            ra.MaximumX = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.X);
                            if (ra.MaximumX != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MaximumX += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }
                    else if (triggerPositionGizmos[1].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MaximumY;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        
                        if (grid)
                        {
                            var prevPos = ra.MaximumY;
                            ra.MaximumY = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Y);
                            if (ra.MaximumY != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MaximumY += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }
                    else if (triggerPositionGizmos[2].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MaximumZ;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        
                        if (grid)
                        {
                            var prevPos = ra.MaximumZ;
                            ra.MaximumZ = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Z);
                            if (ra.MaximumZ != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MaximumZ += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }
                    else if (triggerPositionGizmos[3].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MinimumX;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        
                        if (grid)
                        {
                            var prevPos = ra.MinimumX;
                            ra.MinimumX = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.X);
                            if (ra.MinimumX != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MinimumX += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }
                    else if (triggerPositionGizmos[4].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MinimumY;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        
                        if (grid)
                        {
                            var prevPos = ra.MinimumY;
                            ra.MinimumY = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Y);
                            if (ra.MinimumY != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MinimumY += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }
                    else if (triggerPositionGizmos[5].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        if (totalDistanceMoved == 0)
                            originalPosition = ra.MinimumZ;

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved += scaledMoveDistance;
                        
                        if (grid)
                        {
                            var prevPos = ra.MinimumZ;
                            ra.MinimumZ = SnapToGrid(originalPosition + totalDistanceMoved, GizmoType.Z);
                            if (ra.MinimumZ != prevPos)
                                assetModified = true;
                        }
                        else
                        {
                            ra.MinimumZ += movement * movementScale;
                            if (movement != 0)
                                assetModified = true;
                        }
                    }

                    if (assetModified)
                    {
                        RefreshAssetEditor(((Asset)ra).assetID);
                        assetModified = false;
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        private static float totalDistanceRotated;
        private static float originalRotation;
        
        public void MouseMoveForRotation(Matrix viewProjection, int distanceX, bool grid)//, int distanceY)
        {
            bool assetModified = false;
            
            if (rotationGizmos[0].isSelected || rotationGizmos[1].isSelected || rotationGizmos[2].isSelected)
            {
                var selectedRotatableAssets = from Asset a in CurrentlySelectedAssets where a is IRotatableAsset ica select (IRotatableAsset)a;
                if (!selectedRotatableAssets.Any())
                    return;

                foreach (var ra in selectedRotatableAssets)
                {
                    Vector3 center = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (rotationGizmos[0].isSelected)
                    {
                        //Vector3 direction1 = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);
                        //Vector3 direction2 = rotationGizmos[0].clickPosition - GizmoCenterPosition;
                        //Vector3 tangent = (Vector3)Vector3.Transform(Vector3.Cross(direction2, direction1), viewProjection);

                        //Vector3 direction = tangent - center;
                        //direction.Z = 0;
                        //direction.Normalize();

                        //ra.Yaw -= (distanceX * direction.X - distanceY * direction.Y) / 10;
                        
                        if (totalDistanceRotated == 0)
                            originalRotation = ra.Yaw;

                        totalDistanceRotated += distanceX;
                        
                        var prevYaw = ra.Yaw;

                        if (grid)
                            ra.Yaw = SnapToIncrement(originalRotation + totalDistanceRotated);
                        else
                            ra.Yaw += distanceX;
                        
                        if (ra.Yaw != prevYaw)
                            assetModified = true;
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

                        if (totalDistanceRotated == 0)
                            originalRotation = ra.Pitch;

                        totalDistanceRotated += distanceX;
                        
                        var prevPitch = ra.Pitch;

                        if (grid)
                            ra.Pitch = SnapToIncrement(originalRotation + totalDistanceRotated);
                        else
                            ra.Pitch += distanceX;
                        
                        if (ra.Pitch != prevPitch)
                            assetModified = true;
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
                        
                        if (totalDistanceRotated == 0)
                            originalRotation = ra.Roll;

                        totalDistanceRotated += distanceX;
                        
                        var prevRoll = ra.Roll;

                        if (grid)
                            ra.Roll = SnapToIncrement(originalRotation + totalDistanceRotated);
                        else
                            ra.Roll += distanceX;

                        if (ra.Roll != prevRoll)
                            assetModified = true;
                    }

                    if (assetModified)
                    {
                        RefreshAssetEditor(((Asset)ra).assetID);
                        assetModified = false;
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForScale(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            bool assetModified = false;
            
            if (scaleGizmos[0].isSelected || scaleGizmos[1].isSelected || scaleGizmos[2].isSelected || scaleGizmos[3].isSelected)
            {
                var selectedScalableAssets = from Asset a in CurrentlySelectedAssets where a is IScalableAsset ica select (IScalableAsset)a;
                if (!selectedScalableAssets.Any())
                    return;

                foreach (var ra in selectedScalableAssets)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (scaleGizmos[0].isSelected)
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation), viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        var prevScale = ra.ScaleX;
                        ra.ScaleX += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        if (grid)
                            ra.ScaleX = SnapToGrid(ra.ScaleX, GizmoType.X);

                        if (ra.ScaleX != prevScale)
                            assetModified = true;

                    }
                    else if (scaleGizmos[1].isSelected)
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation), viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        var prevScale = ra.ScaleY;
                        ra.ScaleY += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        if (grid)
                            ra.ScaleY = SnapToGrid(ra.ScaleY, GizmoType.Y);
                        
                        if (ra.ScaleY != prevScale)
                            assetModified = true;
                    }
                    else if (scaleGizmos[2].isSelected)
                    {
                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation), viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        var prevScale = ra.ScaleZ;
                        ra.ScaleZ += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        if (grid)
                            ra.ScaleZ = SnapToGrid(ra.ScaleZ, GizmoType.Z);
                        
                        if (ra.ScaleZ != prevScale)
                            assetModified = true;
                    }
                    else if (scaleGizmos[3].isSelected)
                    {
                        var prevScaleX = ra.ScaleX;
                        
                        ra.ScaleX += distanceX / 40f;
                        ra.ScaleY += distanceX / 40f;
                        ra.ScaleZ += distanceX / 40f;
                        
                        if (prevScaleX != ra.ScaleX)
                            assetModified = true;
                    }

                    if (assetModified)
                    {
                        RefreshAssetEditor(((Asset)ra).assetID);
                        assetModified = false;
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForPositionLocal(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            bool assetModified = false;
            
            if (positionLocalGizmos[0].isSelected || positionLocalGizmos[1].isSelected || positionLocalGizmos[2].isSelected)
            {
                var selectedClickableAssets = from Asset a in CurrentlySelectedAssets where a is IClickableAsset ica select (IClickableAsset)a;
                if (!selectedClickableAssets.Any())
                    return;

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

                foreach (var ra in selectedClickableAssets)
                {
                    float movement = distanceX * direction.X - distanceY * direction.Y;

                    var prevPos = new Vector3(ra.PositionX, ra.PositionY, ra.PositionZ);
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
                    if (prevPos != new Vector3(ra.PositionX, ra.PositionY, ra.PositionZ))
                        assetModified = true;

                    
                    if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                    {
                        var prevMin = new Vector3(trig.MinimumX, trig.MinimumY, trig.MinimumZ);

                        trig.MinimumX = trig.PositionX;
                        trig.MinimumY = trig.PositionY;
                        trig.MinimumZ = trig.PositionZ;
                        
                        if (prevMin != new Vector3(trig.MinimumX, trig.MinimumY, trig.MinimumZ))
                            assetModified = true;
                    }

                    if (assetModified)
                    {
                        RefreshAssetEditor(((Asset)ra).assetID);
                        assetModified = false;
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
            else
                CurrentGizmoMode = mode;

            return CurrentGizmoMode;
        }

        /// <summary>
        /// Rounds a value to the nearest specified increment.
        /// Useful for snapping an arbitrary rotation value
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="increment">The increment</param>
        /// <returns>The value rounded to the nearest increment</returns>
        private float SnapToIncrement(float value, float increment = 15.0f)
        {
            // Round to the nearest increment
            return (float)Math.Round(value / increment) * increment;
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
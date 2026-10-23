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

        public static void SetUpGizmos()
        {
            positionGizmos = [
                new PositionGizmo(GizmoType.X),
                new PositionGizmo(GizmoType.Y),
                new PositionGizmo(GizmoType.Z)];

            triggerPositionGizmos = [
                new BoxTrigPositionGizmo(GizmoType.X),
                new BoxTrigPositionGizmo(GizmoType.Y),
                new BoxTrigPositionGizmo(GizmoType.Z),
                new BoxTrigPositionGizmo(GizmoType.TrigX1),
                new BoxTrigPositionGizmo(GizmoType.TrigY1),
                new BoxTrigPositionGizmo(GizmoType.TrigZ1)];

            rotationGizmos = [
                new RotationGizmo(GizmoType.Yaw),
                new RotationGizmo(GizmoType.Pitch),
                new RotationGizmo(GizmoType.Roll)];

            scaleGizmos = [
                new ScaleGizmo(GizmoType.ScaleX),
                new ScaleGizmo(GizmoType.ScaleY),
                new ScaleGizmo(GizmoType.ScaleZ),
                new ScaleGizmo(GizmoType.ScaleAll)];

            if (Grid.X < 0.001f)
                Grid.X = 1f;
            if (Grid.Y < 0.001f)
                Grid.Y = 1f;
            if (Grid.Z < 0.001f)
                Grid.Z = 1f;
        }

        public static GizmoMode CurrentGizmoMode { get; private set; } = GizmoMode.Position;
        public static bool IsMovingGizmo = false;
        public static bool TriggerGizmo = false;
        public static bool SkipGizmos = false;

        public static void RenderGizmos(SharpRenderer renderer)
        {
            // calculate which gizmos should show up and where and render them

            if (SkipGizmos && !IsMovingGizmo)
                return;

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                {
                    var clickableAssets = allCurrentlySelectedAssets.OfType<IClickableAsset>().ToList();
                    if (clickableAssets.Count == 0)
                        return;

                    BoundingBox boundingBox = clickableAssets[0].GetBoundingBox();
                    foreach (var asset in clickableAssets.Skip(1))
                        boundingBox = BoundingBox.Merge(boundingBox, asset.GetBoundingBox());
                    GizmoCenterPosition = boundingBox.Center;
                    float distance = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;

                    foreach (PositionGizmo gizmo in positionGizmos)
                        gizmo.SetPosition(GizmoCenterPosition, distance, Matrix.Identity);
                    foreach (PositionGizmo gizmo in positionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                        gizmo.Draw(renderer);
                    return;
                }
                case GizmoMode.Rotation:
                {
                    var cameras = allCurrentlySelectedAssets.OfType<AssetCAM>().ToList();
                    if (cameras.Count != 0)
                    {
                        var asset = cameras[0];
                        GizmoCenterRotation = GetRotationMatrix(asset);
                        GizmoCenterPosition = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                    }
                    else
                    {
                        var rotatableAssets = allCurrentlySelectedAssets.OfType<IRotatableAsset>().ToList();
                        if (rotatableAssets.Count == 0)
                            return;

                        var asset = rotatableAssets[0];
                        GizmoCenterRotation = GetRotationMatrix(asset.Yaw, asset.Pitch, asset.Roll);
                        GizmoCenterPosition = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                    }

                    float distance = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 2f;

                    for (int i = rotationGizmos.Length - 1; i >= 0; i--)
                    {
                        rotationGizmos[i].SetPosition(GizmoCenterPosition, distance, GizmoCenterRotation);
                        rotationGizmos[i].Draw(renderer);
                    }
                    return;
                }
                case GizmoMode.Scale:
                    var selectedAssets = allCurrentlySelectedAssets;
                    var scalableAssets = selectedAssets.OfType<IScalableAsset>().ToList();
                    var boxVolumes = selectedAssets.OfType<AssetVOLU>().Where(v => v.VolumeShape is VolumeBox).ToList();

                    TriggerGizmo = false;
                    BoundingBox bbox = default;
                    IScalableAsset primaryAsset = scalableAssets.FirstOrDefault();

                    if (boxVolumes.Count > 0)
                    {
                        var box = (VolumeBox)boxVolumes[0].VolumeShape;
                        TriggerGizmo = true;
                        bbox = box.GetBoundingBox();
                        GizmoCenterRotation = GetRotationMatrix(0, 0, 0);
                    }
                    if (primaryAsset != null)
                    {
                        if (primaryAsset is AssetTRIG { Shape: TriggerShape.Box } trigger)
                        {
                            TriggerGizmo = true;
                            bbox = trigger.GetBoundingBox();
                            GizmoCenterRotation = GetRotationMatrix(trigger.Yaw, trigger.Pitch, trigger.Roll);
                        }
                        else if (primaryAsset is IRotatableAsset rotatable)
                        {
                            GizmoCenterRotation = GetRotationMatrix(rotatable.Yaw, rotatable.Pitch, rotatable.Roll);
                        }
                        else
                        {
                            GizmoCenterRotation = GetRotationMatrix(0, 0, 0);
                        }
                    }
                    if (primaryAsset != null || boxVolumes.Count > 0)
                    {
                        GizmoCenterPosition = TriggerGizmo ? bbox.Center : new Vector3(primaryAsset.PositionX, primaryAsset.PositionY, primaryAsset.PositionZ);
                        float distance = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;
                        foreach (ScaleGizmo gizmo in scaleGizmos)
                            gizmo.SetPosition(GizmoCenterPosition, distance, GizmoCenterRotation);
                        foreach (ScaleGizmo gizmo in scaleGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer);
                    }
                    return;
                case GizmoMode.PositionLocal:
                {
                    var clickableAssets = allCurrentlySelectedAssets.OfType<IClickableAsset>().Take(2).ToList();
                    if (clickableAssets.Count != 1)
                        return;

                    var asset = clickableAssets[0];
                    GizmoCenterPosition = asset.GetBoundingBox().Center;
                    float distance = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;

                    TriggerGizmo = false;
                    IVolumeAsset volume = null;
                    if (asset is AssetTRIG { Shape: TriggerShape.Box } trigger)
                    {
                        TriggerGizmo = true;
                        GizmoCenterRotation = GetRotationMatrix(trigger.Yaw, trigger.Pitch, trigger.Roll);
                        volume = trigger;
                    }
                    else if (asset is AssetVOLU { VolumeShape: VolumeBox box })
                    {
                        TriggerGizmo = true;
                        GizmoCenterRotation = GetRotationMatrix(0, 0, 0);
                        volume = box;
                    }
                    else if (asset is AssetCAM cam)
                    {
                        GizmoCenterRotation = GetRotationMatrix(cam);
                    }
                    else if (asset is IRotatableAsset ra)
                    {
                        GizmoCenterRotation = GetRotationMatrix(ra.Yaw, ra.Pitch, ra.Roll);
                    }
                    else
                    {
                        GizmoCenterRotation = GetRotationMatrix(0, 0, 0);
                    }

                    if (TriggerGizmo)
                    {
                        Vector3 halfExtents = new Vector3(volume.MaximumX - volume.MinimumX, volume.MaximumY - volume.MinimumY, volume.MaximumZ - volume.MinimumZ) / 2f;
                        Vector3 center = volume.GetBoundingBox().Center;

                        foreach (BoxTrigPositionGizmo gizmo in triggerPositionGizmos)
                            gizmo.SetPosition(center, halfExtents, distance, GizmoCenterRotation);
                        foreach (BoxTrigPositionGizmo gizmo in triggerPositionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer);
                    }
                    else
                    {
                        foreach (PositionGizmo gizmo in positionGizmos)
                            gizmo.SetPosition(GizmoCenterPosition, distance, GizmoCenterRotation);
                        foreach (PositionGizmo gizmo in positionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer);
                    }

                    return;
                }
            }
        }

        private static Vector3 GizmoCenterPosition;
        private static Matrix GizmoCenterRotation;

        private static Matrix GetRotationMatrix(float Yaw, float Pitch, float Roll) =>
            Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll));

        private static Matrix GetRotationMatrix(AssetCAM cam)
        {
            Vector3 forward = Vector3.Normalize(new Vector3(cam.NormalizedForwardX, cam.NormalizedForwardY, cam.NormalizedForwardZ));
            Vector3 up = Vector3.Normalize(new Vector3(cam.NormalizedUpX, cam.NormalizedUpY, cam.NormalizedUpZ));
            Vector3 left = Vector3.Normalize(Vector3.Cross(up, forward));

            return new Matrix(
                left.X, left.Y, left.Z, 0f,
                up.X, up.Y, up.Z, 0f,
                forward.X, forward.Y, forward.Z, 0f,
                0f, 0f, 0f, 1f);
        }

        public static void ScreenClickedGizmos(Ray r)
        {
            // check which gizmo was selected by the ray, if any
            // set currently gizmoing assets and boxes
            // save original positions for future undo and grid

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                {
                    (int index, _) = GetGizmoIntersectionIndex(positionGizmos, r);
                    if (index != -1 && !positionGizmos[index].isSelected)
                    {
                        SaveOriginalPositions();
                        SaveOriginalPositionsBoxes();
                        positionGizmos[index].isSelected = true;
                        IsMovingGizmo = true;
                    }
                }
                break;
                case GizmoMode.Rotation:
                {
                    (int index, _) = GetGizmoIntersectionIndex(rotationGizmos, r);
                    if (index != -1 && !rotationGizmos[index].isSelected)
                    {
                        SaveOriginalRotations();
                        SaveOriginalRotationsCameras();
                        rotationGizmos[index].isSelected = true;
                        IsMovingGizmo = true;
                    }
                }
                break;
                case GizmoMode.Scale:
                {
                    (int index, _) = GetGizmoIntersectionIndex(scaleGizmos, r);
                    if (index != -1 && !scaleGizmos[index].isSelected)
                    {
                        SaveOriginalPositionsBoxes();
                        SaveOriginalScales();
                        scaleGizmos[index].isSelected = true;
                        IsMovingGizmo = true;
                    }
                }
                break;
                case GizmoMode.PositionLocal:
                {
                    if (TriggerGizmo)
                    {
                        (int index, _) = GetGizmoIntersectionIndex(triggerPositionGizmos, r);
                        if (index != -1 && !triggerPositionGizmos[index].isSelected)
                        {
                            SaveOriginalPositionsBoxes();
                            triggerPositionGizmos[index].isSelected = true;
                            IsMovingGizmo = true;
                        }
                    }
                    else
                    {
                        (int index, _) = GetGizmoIntersectionIndex(positionGizmos, r);
                        if (index != -1 && !positionGizmos[index].isSelected)
                        {
                            SaveOriginalPositions();
                            positionGizmos[index].isSelected = true;
                            IsMovingGizmo = true;
                        }
                    }
                }
                break;
            }
        }

        private static (int index, float distance) GetGizmoIntersectionIndex(GizmoBase[] gizmos, Ray r)
        {
            int index = -1;
            float dist = 10000f;
            for (int g = 0; g < gizmos.Length; g++)
            {
                float? distance = gizmos[g].IntersectsWith(r);
                if (distance != null && distance < dist)
                {
                    dist = (float)distance;
                    index = g;
                }
            }
            return (index, dist);
        }

        private static void SaveOriginalPositions()
        {
            currentlyMoving = allCurrentlySelectedAssets.OfType<IClickableAsset>().ToList();
            foreach (var asset in currentlyMoving)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
        }

        private static void SaveOriginalRotations()
        {
            currentlyRotating = allCurrentlySelectedAssets.OfType<IRotatableAsset>().ToList();
            foreach (var asset in currentlyRotating)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.Yaw, asset.Pitch, asset.Roll);
        }

        private static void SaveOriginalScales()
        {
            currentlyScaling = allCurrentlySelectedAssets.OfType<IScalableAsset>().Where(a => !currentlyMovingBoxes.Contains((Asset)a)).ToList();
            foreach (var asset in currentlyScaling)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.ScaleX, asset.ScaleY, asset.ScaleZ);
        }

        private static void SaveOriginalPositionsBoxes()
        {
            currentlyMovingBoxes = allCurrentlySelectedAssets.Where(a => a is AssetTRIG trig && trig.Shape == TriggerShape.Box || a is AssetVOLU volu && volu.VolumeShape is VolumeBox).ToList();
            foreach (var asset in currentlyMovingBoxes)
            {
                var box = asset is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)asset).VolumeShape;
                originalPositionsBoxes[asset.assetID] = (new Vector3(box.MaximumX, box.MaximumY, box.MaximumZ), new Vector3(box.MinimumX, box.MinimumY, box.MinimumZ));
            }
        }

        private static void SaveOriginalRotationsCameras()
        {
            currentlyRotatingCameras = allCurrentlySelectedAssets.OfType<AssetCAM>().ToList();
            foreach (var cam in currentlyRotatingCameras)
                originalRotationsCameras[cam.assetID] = cam.GetCameraOrientation();
        }

        public static void ScreenUnclicked()
        {
            if (!IsMovingGizmo)
                return;

            IsMovingGizmo = false;

            // deselect all gizmos, save new positions to undo buffer, clear caches

            totalDistanceMoved = Vector3.Zero;

            var actions = new List<IReversibleAction>();

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                    foreach (PositionGizmo g in positionGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;
                        foreach (var a in currentlyMoving)
                        {
                            var archive = ArchiveContainingAsset((Asset)a);
                            archive.RefreshAssetEditor(a.assetID);
                            switch (g.Type)
                            {
                                case GizmoType.X:
                                    actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionX", originalPositions[a.assetID].X, a.PositionX));
                                    break;
                                case GizmoType.Y:
                                    actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionY", originalPositions[a.assetID].Y, a.PositionY));
                                    break;
                                case GizmoType.Z:
                                    actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionZ", originalPositions[a.assetID].Z, a.PositionZ));
                                    break;
                            }
                            if (a is AssetTRIG trig)
                            {
                                if (trig.Shape == TriggerShape.Box)
                                {
                                    archive.AddToActionsBoxMovement(trig, g.Type, actions);
                                }
                                else
                                {
                                    switch (g.Type)
                                    {
                                        case GizmoType.X:
                                            actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumX", originalPositions[a.assetID].X, a.PositionX));
                                            break;
                                        case GizmoType.Y:
                                            actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumY", originalPositions[a.assetID].Y, a.PositionY));
                                            break;
                                        case GizmoType.Z:
                                            actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumZ", originalPositions[a.assetID].Z, a.PositionZ));
                                            break;
                                    }
                                }
                            }
                            else if (a is AssetVOLU volu)
                            {
                                archive.AddToActionsBoxMovement(volu, g.Type, actions);
                            }
                            archive.UnsavedChanges = true;
                        }
                    }
                    break;
                case GizmoMode.Rotation:
                    foreach (RotationGizmo g in rotationGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;
                        foreach (var a in currentlyRotating)
                        {
                            var archive = ArchiveContainingAsset((Asset)a);
                            archive.RefreshAssetEditor(((Asset)a).assetID);
                            var og = originalPositions[((Asset)a).assetID];
                            if (og.X != a.Yaw)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "Yaw", og.X, a.Yaw));
                            if (og.Y != a.Pitch)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "Pitch", og.Y, a.Pitch));
                            if (og.Z != a.Roll)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "Roll", og.Z, a.Roll));
                            archive.UnsavedChanges = true;
                        }
                        foreach (var cam in currentlyRotatingCameras)
                        {
                            var archive = ArchiveContainingAsset(cam);
                            archive.RefreshAssetEditor(cam.assetID);
                            var (Forward, Up, Left) = originalRotationsCameras[cam.assetID];
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedForwardX", Forward.X, cam.NormalizedForwardX));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedForwardY", Forward.Y, cam.NormalizedForwardY));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedForwardZ", Forward.Z, cam.NormalizedForwardZ));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedLeftX", Left.X, cam.NormalizedLeftX));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedLeftY", Left.Y, cam.NormalizedLeftY));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedLeftZ", Left.Z, cam.NormalizedLeftZ));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedUpX", Up.X, cam.NormalizedUpX));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedUpY", Up.Y, cam.NormalizedUpY));
                            actions.Add(new AssetPropertyChangedAction(archive, cam, "NormalizedUpZ", Up.Z, cam.NormalizedUpZ));
                            archive.UnsavedChanges = true;
                        }
                    }
                    break;
                case GizmoMode.Scale:
                    foreach (ScaleGizmo g in scaleGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;

                        foreach (var a in currentlyScaling)
                        {
                            var archive = ArchiveContainingAsset((Asset)a);
                            archive.RefreshAssetEditor(((Asset)a).assetID);
                            if (g.Type == GizmoType.ScaleX || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "ScaleX", originalPositions[((Asset)a).assetID].X, a.ScaleX));
                            if (g.Type == GizmoType.ScaleY || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "ScaleY", originalPositions[((Asset)a).assetID].Y, a.ScaleY));
                            if (g.Type == GizmoType.ScaleZ || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "ScaleZ", originalPositions[((Asset)a).assetID].Z, a.ScaleZ));
                            archive.UnsavedChanges = true;
                        }
                        foreach (var a in currentlyMovingBoxes)
                        {
                            var archive = ArchiveContainingAsset(a);
                            archive.RefreshAssetEditor(a.assetID);
                            archive.AddToActionsBoxMovement(a, g.Type, actions);
                            archive.UnsavedChanges = true;
                        }
                    }
                    break;
                case GizmoMode.PositionLocal:
                    foreach (PositionGizmo g in positionGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;
                        foreach (var a in currentlyMoving)
                        {
                            var archive = ArchiveContainingAsset((Asset)a);
                            archive.RefreshAssetEditor(((Asset)a).assetID);
                            actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionX", originalPositions[((Asset)a).assetID].X, a.PositionX));
                            actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionY", originalPositions[((Asset)a).assetID].Y, a.PositionY));
                            actions.Add(new AssetPropertyChangedAction(archive, (Asset)a, "PositionZ", originalPositions[((Asset)a).assetID].Z, a.PositionZ));
                            archive.UnsavedChanges = true;
                        }
                    }
                    foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;

                        foreach (var a in currentlyMovingBoxes)
                        {
                            var archive = ArchiveContainingAsset(a);
                            archive.RefreshAssetEditor(a.assetID);
                            archive.AddToActionsBoxMovement(a, g.Type, actions);
                            archive.UnsavedChanges = true;
                        }
                    }
                    break;
            }

            originalPositions.Clear();
            currentlyMoving.Clear();
            currentlyRotating.Clear();
            currentlyScaling.Clear();
            originalPositionsBoxes.Clear();
            currentlyMovingBoxes.Clear();
            originalRotationsCameras.Clear();
            currentlyRotatingCameras.Clear();

            actions.Add(new SelectionAction(allCurrentlySelectedAssets.Select(asset => asset.assetID)));
            Program.UndoBuffer.AddAction(actions);
        }

        private void AddToActionsBoxMovement(Asset boxAsset, GizmoType type, List<IReversibleAction> actions)
        {
            (Vector3 ogMaximum, Vector3 ogMinimum) = originalPositionsBoxes[boxAsset.assetID];
            if (boxAsset is AssetTRIG trig)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.TrigX1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumX", ogMinimum.X, trig.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumX", ogMaximum.X, trig.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.TrigY1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumY", ogMinimum.Y, trig.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumY", ogMaximum.Y, trig.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.TrigZ1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumZ", ogMinimum.Z, trig.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumZ", ogMaximum.Z, trig.MaximumZ));
                        break;
                }
            }
            else if (boxAsset is AssetVOLU volume && volume.VolumeShape is VolumeBox box)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.TrigX1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumX"], ogMinimum.X, box.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumX"], ogMaximum.X, box.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.TrigY1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumY"], ogMinimum.Y, box.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumY"], ogMaximum.Y, box.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.TrigZ1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumZ"], ogMinimum.Z, box.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumZ"], ogMaximum.Z, box.MaximumZ));
                        break;
                }
            }
        }

        public void RefreshAssetEditor(uint assetID)
        {
            foreach (var v in internalEditors)
                if (v.GetAssetID() == assetID)
                    v.RefreshPropertyGrid();
            foreach (var v in multiInternalEditors)
                if (v.AssetIDs.Contains(assetID))
                    v.RefreshPropertyGrid();
        }

        private static List<IClickableAsset> currentlyMoving = new List<IClickableAsset>();
        private static List<IRotatableAsset> currentlyRotating = new List<IRotatableAsset>();
        private static List<IScalableAsset> currentlyScaling = new List<IScalableAsset>();
        private static List<Asset> currentlyMovingBoxes = new List<Asset>();
        private static Dictionary<uint, Vector3> originalPositions = new Dictionary<uint, Vector3>();
        private static Dictionary<uint, (Vector3 Maximum, Vector3 Minimum)> originalPositionsBoxes = new Dictionary<uint, (Vector3, Vector3)>();
        private static Vector3 totalDistanceMoved = Vector3.Zero;
        private static float movementScale = 0.1f;
        public static Vector3 Grid;
        public static bool RotationGizmoLocalAxis = true;

        public static void MouseMoveGizmos(Matrix viewProjection, int deltaX, int deltaY, bool grid)
        {
            if (!allCurrentlySelectedAssets.Any())
                return;

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                    if (currentlyMoving.Count != 0)
                        MouseMoveForPosition(viewProjection, deltaX, deltaY, grid);
                    break;
                case GizmoMode.Rotation:
                    if (currentlyRotating.Count != 0)
                    {
                        if (RotationGizmoLocalAxis)
                            MouseMoveForRotationLocalAxis(deltaX, grid);
                        else
                            MouseMoveForRotationSingleAxis(deltaX, grid);
                    }
                    if (currentlyRotatingCameras.Count != 0)
                        MouseMoveForCameraRotation(deltaX, grid, RotationGizmoLocalAxis);
                    break;
                case GizmoMode.Scale:
                    if (currentlyScaling.Count != 0)
                        MouseMoveForScale(viewProjection, deltaX, deltaY, grid);
                    if (currentlyMovingBoxes.Count != 0)
                        MouseMoveForScaleBoxes(viewProjection, deltaX, deltaY, grid);
                    break;
                case GizmoMode.PositionLocal:
                    if (currentlyMoving.Count != 0)
                        MouseMoveForPositionLocal(viewProjection, deltaX, deltaY, grid);
                    if (currentlyMovingBoxes.Count != 0)
                        MouseMoveForPositionBoxes(viewProjection, deltaX, deltaY, grid);
                    break;
            }
        }


        private static void MouseMoveForPosition(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            int axis = Array.FindIndex(positionGizmos, pg => pg.isSelected);
            if (axis == -1)
                return;

            Vector3 direction1 = Vector3.TransformCoordinate(GizmoCenterPosition, viewProjection);
            Vector3 axisVector = axis switch
            {
                0 => Vector3.UnitX,
                1 => Vector3.UnitY,
                _ => Vector3.UnitZ
            };
            Vector3 direction2 = Vector3.TransformCoordinate(GizmoCenterPosition + axisVector, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float scaledMovement = movement * movementScale;
            GizmoType gizmoType = axis switch
            {
                0 => GizmoType.X,
                1 => GizmoType.Y,
                _ => GizmoType.Z
            };

            foreach (var asset in currentlyMoving)
            {
                float assetMovement = scaledMovement;
                if (axis == 1 && asset is AssetUI)
                    assetMovement *= -1;

                switch (axis)
                {
                    case 0:
                        totalDistanceMoved.X += scaledMovement;
                        break;
                    case 1:
                        totalDistanceMoved.Y += scaledMovement;
                        break;
                    case 2:
                        totalDistanceMoved.Z += scaledMovement;
                        break;
                }

                uint assetID = ((Asset)asset).assetID;

                float totalAxisMovement = axis switch
                {
                    0 => totalDistanceMoved.X,
                    1 => totalDistanceMoved.Y,
                    _ => totalDistanceMoved.Z
                };

                if (asset is AssetTRIG trig && trig.Shape == TriggerShape.Box)
                {
                    var originalPosition = originalPositions[assetID];
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    switch (axis)
                    {
                        case 0:
                            if (grid)
                            {
                                trig.PositionX = SnapToGrid(originalPosition.X + totalAxisMovement, gizmoType);
                                trig.MinimumX = SnapToGrid(Minimum.X + totalAxisMovement, gizmoType);
                                trig.MaximumX = SnapToGrid(Maximum.X + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                trig.PositionX += assetMovement;
                                trig.MinimumX += assetMovement;
                                trig.MaximumX += assetMovement;
                            }
                            break;
                        case 1:
                            if (grid)
                            {
                                trig.PositionY = SnapToGrid(originalPosition.Y + totalAxisMovement, gizmoType);
                                trig.MinimumY = SnapToGrid(Minimum.Y + totalAxisMovement, gizmoType);
                                trig.MaximumY = SnapToGrid(Maximum.Y + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                trig.PositionY += assetMovement;
                                trig.MinimumY += assetMovement;
                                trig.MaximumY += assetMovement;
                            }
                            break;
                        case 2:
                            if (grid)
                            {
                                trig.PositionZ = SnapToGrid(originalPosition.Z + totalAxisMovement, gizmoType);
                                trig.MinimumZ = SnapToGrid(Minimum.Z + totalAxisMovement, gizmoType);
                                trig.MaximumZ = SnapToGrid(Maximum.Z + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                trig.PositionZ += assetMovement;
                                trig.MinimumZ += assetMovement;
                                trig.MaximumZ += assetMovement;
                            }
                            break;
                    }
                }
                else if (asset is AssetVOLU volu && volu.VolumeShape is VolumeBox vbox)
                {
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    switch (axis)
                    {
                        case 0:
                            if (grid)
                            {
                                vbox.MinimumX = SnapToGrid(Minimum.X + totalAxisMovement, gizmoType);
                                vbox.MaximumX = SnapToGrid(Maximum.X + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                vbox.MinimumX += assetMovement;
                                vbox.MaximumX += assetMovement;
                            }
                            break;
                        case 1:
                            if (grid)
                            {
                                vbox.MinimumY = SnapToGrid(Minimum.Y + totalAxisMovement, gizmoType);
                                vbox.MaximumY = SnapToGrid(Maximum.Y + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                vbox.MinimumY += assetMovement;
                                vbox.MaximumY += assetMovement;
                            }
                            break;
                        case 2:
                            if (grid)
                            {
                                vbox.MinimumZ = SnapToGrid(Minimum.Z + totalAxisMovement, gizmoType);
                                vbox.MaximumZ = SnapToGrid(Maximum.Z + totalAxisMovement, gizmoType);
                            }
                            else
                            {
                                vbox.MinimumZ += assetMovement;
                                vbox.MaximumZ += assetMovement;
                            }
                            break;
                    }
                }
                else
                {
                    var originalPosition = originalPositions[assetID];
                    float originalAxisPosition = axis switch
                    {
                        0 => originalPosition.X,
                        1 => originalPosition.Y,
                        _ => originalPosition.Z
                    };
                    float newPosition = grid
                        ? SnapToGrid(originalAxisPosition + totalAxisMovement, gizmoType)
                        : GetCurrentPosition(asset, axis) + assetMovement;

                    SetPosition(asset, axis, newPosition);

                    if (asset is AssetTRIG roundTrig)
                        SetTriggerMinimum(roundTrig, axis, newPosition);
                }
            }
        }

        private static float GetCurrentPosition(IClickableAsset asset, int axis)
        {
            return axis switch
            {
                0 => asset.PositionX,
                1 => asset.PositionY,
                _ => asset.PositionZ
            };
        }

        private static void SetPosition(IClickableAsset asset, int axis, float value)
        {
            switch (axis)
            {
                case 0:
                    asset.PositionX = value;
                    break;
                case 1:
                    asset.PositionY = value;
                    break;
                case 2:
                    asset.PositionZ = value;
                    break;
            }
        }

        private static void SetTriggerMinimum(AssetTRIG trigger, int axis, float value)
        {
            switch (axis)
            {
                case 0:
                    trigger.MinimumX = value;
                    break;
                case 1:
                    trigger.MinimumY = value;
                    break;
                case 2:
                    trigger.MinimumZ = value;
                    break;
            }
        }

        private static void MouseMoveForPositionBoxes(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            int selectedGizmo = Array.FindIndex(triggerPositionGizmos, g => g.isSelected);
            if (selectedGizmo == -1)
                return;

            int axis = selectedGizmo % 3;
            bool maximum = selectedGizmo < 3;

            Vector3 axisVector = axis switch
            {
                0 => Vector3.UnitX,
                1 => Vector3.UnitY,
                _ => Vector3.UnitZ
            };
            Vector3 movementDirection = Vector3.TransformCoordinate(axisVector, GizmoCenterRotation);
            Vector3 direction1 = Vector3.TransformCoordinate(GizmoCenterPosition, viewProjection);
            Vector3 direction2 = Vector3.TransformCoordinate(GizmoCenterPosition + movementDirection, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float scaledMovement = movement * movementScale;

            GizmoType gizmoType = axis switch
            {
                0 => GizmoType.X,
                1 => GizmoType.Y,
                _ => GizmoType.Z
            };

            foreach (var asset in currentlyMovingBoxes)
            {
                IVolumeAsset box = asset is AssetTRIG trigger ? trigger : (IVolumeAsset)((AssetVOLU)asset).VolumeShape;
                uint assetID = asset.assetID;
                
                switch (axis)
                {
                    case 0:
                        totalDistanceMoved.X += scaledMovement;
                        break;
                    case 1:
                        totalDistanceMoved.Y += scaledMovement;
                        break;
                    case 2:
                        totalDistanceMoved.Z += scaledMovement;
                        break;
                }
                float totalAxisMovement = axis switch
                {
                    0 => totalDistanceMoved.X,
                    1 => totalDistanceMoved.Y,
                    _ => totalDistanceMoved.Z
                };
                float currentBound = axis switch
                {
                    0 when maximum => box.MaximumX,
                    0 => box.MinimumX,
                    1 when maximum => box.MaximumY,
                    1 => box.MinimumY,
                    2 when maximum => box.MaximumZ,
                    _ => box.MinimumZ
                };
                float newBound;

                if (grid)
                {
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    float originalBound = axis switch
                    {
                        0 when maximum => Maximum.X,
                        0 => Minimum.X,
                        1 when maximum => Maximum.Y,
                        1 => Minimum.Y,
                        2 when maximum => Maximum.Z,
                        _ => Minimum.Z
                    };
                    newBound = SnapToGrid(originalBound + totalAxisMovement, gizmoType);
                }
                else
                {
                    newBound = currentBound + scaledMovement;
                }

                switch (axis)
                {
                    case 0 when maximum:
                        box.MaximumX = newBound;
                        break;
                    case 0:
                        box.MinimumX = newBound;
                        break;
                    case 1 when maximum:
                        box.MaximumY = newBound;
                        break;
                    case 1:
                        box.MinimumY = newBound;
                        break;
                    case 2 when maximum:
                        box.MaximumZ = newBound;
                        break;
                    case 2:
                        box.MinimumZ = newBound;
                        break;
                }
            }
        }

        private static void MouseMoveForRotationLocalAxis(int distanceX, bool grid)
        {
            var index = Array.FindIndex(rotationGizmos, g => g.isSelected);
            if (index < 0)
                return;
            var axis = (Axis)index;

            switch (axis)
            {
                case Axis.X:
                    totalDistanceMoved.X += distanceX;
                    break;
                case Axis.Y:
                    totalDistanceMoved.Y += distanceX;
                    break;
                case Axis.Z:
                    totalDistanceMoved.Z += distanceX;
                    break;
            }

            float angle = axis switch
            {
                Axis.X => totalDistanceMoved.X,
                Axis.Y => totalDistanceMoved.Y,
                _ => totalDistanceMoved.Z,
            };

            Vector3 localAxis = axis switch
            {
                Axis.X => Vector3.UnitY,
                Axis.Y => Vector3.UnitX,
                _ => Vector3.UnitZ
            };

            Matrix deltaRotation = Matrix.RotationAxis(localAxis, MathUtil.DegreesToRadians(angle));

            foreach (var ra in currentlyRotating)
            {
                Vector3 original = originalPositions[((Asset)ra).assetID];
                Matrix originalRotation = Matrix.RotationYawPitchRoll(
                    MathUtil.DegreesToRadians(original.X),
                    MathUtil.DegreesToRadians(original.Y),
                    MathUtil.DegreesToRadians(original.Z));
                Matrix result = deltaRotation * originalRotation;

                float pitch = (float)Math.Asin(Math.Clamp(-result.M32, -1f, 1f));
                float cosPitch = (float)Math.Cos(pitch);
                float yaw, roll;
                if (Math.Abs(cosPitch) > 0.0001f)
                {
                    yaw = (float)Math.Atan2(result.M31, result.M33);
                    roll = (float)Math.Atan2(result.M12, result.M22);
                }
                else
                {
                    yaw = (float)Math.Atan2(-result.M13, result.M11);
                    roll = 0f;
                }

                if (grid)
                {
                    ra.Yaw = SnapToIncrement(MathUtil.RadiansToDegrees(yaw));
                    ra.Pitch = SnapToIncrement(MathUtil.RadiansToDegrees(pitch));
                    ra.Roll = SnapToIncrement(MathUtil.RadiansToDegrees(roll));
                }
                else
                {
                    ra.Yaw = MathUtil.RadiansToDegrees(yaw);
                    ra.Pitch = MathUtil.RadiansToDegrees(pitch);
                    ra.Roll = MathUtil.RadiansToDegrees(roll);
                }
            }
        }

        private static void MouseMoveForRotationSingleAxis(int distanceX, bool grid)
        {
            Axis axis = (Axis)Array.FindIndex(rotationGizmos, g => g.isSelected);
            if (axis < 0)
                return;
            foreach (var ra in currentlyRotating)
            {
                switch (axis)
                {
                    case Axis.X:
                        totalDistanceMoved.X += distanceX;
                        if (grid)
                            ra.Yaw = SnapToIncrement(originalPositions[((Asset)ra).assetID].X + totalDistanceMoved.X);
                        else
                            ra.Yaw += distanceX;
                        break;
                    case Axis.Y:
                        totalDistanceMoved.Y += distanceX;
                        if (grid)
                            ra.Pitch = SnapToIncrement(originalPositions[((Asset)ra).assetID].Y + totalDistanceMoved.Y);
                        else
                            ra.Pitch += distanceX;
                        break;
                    case Axis.Z:
                        totalDistanceMoved.Z += distanceX;
                        if (grid)
                            ra.Roll = SnapToIncrement(originalPositions[((Asset)ra).assetID].Z + totalDistanceMoved.Z);
                        else
                            ra.Roll += distanceX;
                        break;
                }
            }
        }

        private static void MouseMoveForScale(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            int index = Array.FindIndex(scaleGizmos, sg => sg.isSelected);
            if (index == -1)
                return;

            Vector3 direction = Vector3.Zero;
            float movement;
            if (index < 3)
            {
                Vector3 axisVector = index switch
                {
                    0 => Vector3.UnitX,
                    1 => Vector3.UnitY,
                    _ => Vector3.UnitZ
                };
                Vector3 direction1 = Vector3.TransformCoordinate(GizmoCenterPosition, viewProjection);
                Vector3 movementDirection = Vector3.TransformCoordinate(axisVector, GizmoCenterRotation);
                Vector3 direction2 = Vector3.TransformCoordinate(GizmoCenterPosition + movementDirection, viewProjection);
                direction = direction2 - direction1;
                direction.Z = 0;
                direction.Normalize();
                movement = (distanceX * direction.X - distanceY * direction.Y) / 40f;
            }
            else
            {
                movement = distanceX / 40f;
            }

            foreach (var asset in currentlyScaling)
            {
                switch (index)
                {
                    case 0:
                        asset.ScaleX += movement;
                        if (grid)
                            asset.ScaleX = SnapToGrid(asset.ScaleX, GizmoType.X);
                        totalDistanceMoved.X = asset.ScaleX - originalPositions[((Asset)asset).assetID].X;
                        continue;
                    case 1:
                        asset.ScaleY += movement;
                        if (grid)
                            asset.ScaleY = SnapToGrid(asset.ScaleY, GizmoType.Y);
                        totalDistanceMoved.Y = asset.ScaleY - originalPositions[((Asset)asset).assetID].Y;
                        continue;
                    case 2:
                        asset.ScaleZ += movement;
                        if (grid)
                            asset.ScaleZ = SnapToGrid(asset.ScaleZ, GizmoType.Z);
                        totalDistanceMoved.Z = asset.ScaleZ - originalPositions[((Asset)asset).assetID].Z;
                        continue;
                    case 3:
                    {
                        float factor = MathF.Exp(movement / 10f);
                        asset.ScaleX *= factor;
                        asset.ScaleY *= factor;
                        asset.ScaleZ *= factor;
                        totalDistanceMoved.X = asset.ScaleX - originalPositions[((Asset)asset).assetID].X;
                        totalDistanceMoved.Y = asset.ScaleY - originalPositions[((Asset)asset).assetID].Y;
                        totalDistanceMoved.Z = asset.ScaleZ - originalPositions[((Asset)asset).assetID].Z;
                        continue;
                    }
                }
            }
        }

        private static void MouseMoveForScaleBoxes(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            int index = Array.FindIndex(scaleGizmos, sg => sg.isSelected);
            if (index == -1)
                return;

            Vector3 direction = Vector3.Zero;
            float movement;
            if (index < 3)
            {
                Vector3 axisVector = index switch
                {
                    0 => Vector3.UnitX,
                    1 => Vector3.UnitY,
                    _ => Vector3.UnitZ
                };

                Vector3 direction1 = Vector3.TransformCoordinate(GizmoCenterPosition, viewProjection);
                Vector3 movementDirection = Vector3.TransformCoordinate(axisVector, GizmoCenterRotation);
                Vector3 direction2 = Vector3.TransformCoordinate(
                    GizmoCenterPosition + movementDirection, viewProjection);

                direction = direction2 - direction1;
                direction.Z = 0;
                direction.Normalize();

                movement = distanceX * direction.X - distanceY * direction.Y;
            }
            else
            {
                movement = distanceX;
            }
            float scaledMovement = movement * movementScale;

            foreach (var asset in currentlyMovingBoxes)
            {
                var box = asset is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)asset).VolumeShape;
                uint assetID = asset.assetID;

                if (index < 3)
                {
                    switch (index)
                    {
                        case 0:
                            totalDistanceMoved.X += scaledMovement;
                            break;
                        case 1:
                            totalDistanceMoved.Y += scaledMovement;
                            break;
                        case 2:
                            totalDistanceMoved.Z += scaledMovement;
                            break;
                    }
                }
                else
                {
                    totalDistanceMoved += scaledMovement;
                }

                if (index == 3)
                {
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    float factor = MathF.Exp(totalDistanceMoved.X / 10f);
                    float centerX = (Maximum.X + Minimum.X) / 2f;
                    float centerY = (Maximum.Y + Minimum.Y) / 2f;
                    float centerZ = (Maximum.Z + Minimum.Z) / 2f;
                    float halfX = (Maximum.X - Minimum.X) / 2f * factor;
                    float halfY = (Maximum.Y - Minimum.Y) / 2f * factor;
                    float halfZ = (Maximum.Z - Minimum.Z) / 2f * factor;
                    float maximumX = centerX + halfX;
                    float minimumX = centerX - halfX;
                    float maximumY = centerY + halfY;
                    float minimumY = centerY - halfY;
                    float maximumZ = centerZ + halfZ;
                    float minimumZ = centerZ - halfZ;
                    if (grid)
                    {
                        box.MaximumX = SnapToGrid(maximumX, GizmoType.X);
                        box.MinimumX = SnapToGrid(minimumX, GizmoType.X);
                        box.MaximumY = SnapToGrid(maximumY, GizmoType.Y);
                        box.MinimumY = SnapToGrid(minimumY, GizmoType.Y);
                        box.MaximumZ = SnapToGrid(maximumZ, GizmoType.Z);
                        box.MinimumZ = SnapToGrid(minimumZ, GizmoType.Z);
                    }
                    else
                    {
                        box.MaximumX = maximumX;
                        box.MinimumX = minimumX;
                        box.MaximumY = maximumY;
                        box.MinimumY = minimumY;
                        box.MaximumZ = maximumZ;
                        box.MinimumZ = minimumZ;
                    }
                }
                else if (grid)
                {
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    switch (index)
                    {
                        case 0:
                            box.MaximumX = SnapToGrid(Maximum.X + totalDistanceMoved.X, GizmoType.X);
                            box.MinimumX = SnapToGrid(Minimum.X - totalDistanceMoved.X, GizmoType.X);
                            break;
                        case 1:
                            box.MaximumY = SnapToGrid(Maximum.Y + totalDistanceMoved.Y, GizmoType.Y);
                            box.MinimumY = SnapToGrid(Minimum.Y - totalDistanceMoved.Y, GizmoType.Y);
                            break;
                        case 2:
                            box.MaximumZ = SnapToGrid(Maximum.Z + totalDistanceMoved.Z, GizmoType.Z);
                            box.MinimumZ = SnapToGrid(Minimum.Z - totalDistanceMoved.Z, GizmoType.Z);
                            break;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            box.MaximumX += scaledMovement;
                            box.MinimumX -= scaledMovement;
                            break;
                        case 1:
                            box.MaximumY += scaledMovement;
                            box.MinimumY -= scaledMovement;
                            break;
                        case 2:
                            box.MaximumZ += scaledMovement;
                            box.MinimumZ -= scaledMovement;
                            break;
                    }
                }
            }
        }

        private static void MouseMoveForPositionLocal(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            int index = Array.FindIndex(positionGizmos, pg => pg.isSelected);
            if (index == -1)
                return;

            Vector3 axisVector = index switch
            {
                0 => Vector3.UnitX,
                1 => Vector3.UnitY,
                _ => Vector3.UnitZ,
            };

            Vector3 movementDirection = Vector3.TransformCoordinate(axisVector, GizmoCenterRotation);
            Vector3 direction1 = Vector3.TransformCoordinate(GizmoCenterPosition, viewProjection);
            Vector3 direction2 = Vector3.TransformCoordinate(GizmoCenterPosition + movementDirection, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float movementScale = grid ? 1f : ArchiveEditorFunctions.movementScale;

            foreach (var asset in currentlyMoving)
            {
                asset.PositionX += movementDirection.X * movement * movementScale;
                asset.PositionY += movementDirection.Y * movement * movementScale;
                asset.PositionZ += movementDirection.Z * movement * movementScale;

                if (grid)
                {
                    asset.PositionX = SnapToGrid(asset.PositionX, GizmoType.X);
                    asset.PositionY = SnapToGrid(asset.PositionY, GizmoType.Y);
                    asset.PositionZ = SnapToGrid(asset.PositionZ, GizmoType.Z);
                }

                totalDistanceMoved = originalPositions[((Asset)asset).assetID] - new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);

                if (asset is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                {
                    trig.MinimumX = trig.PositionX;
                    trig.MinimumY = trig.PositionY;
                    trig.MinimumZ = trig.PositionZ;
                }
            }
        }

        private static List<AssetCAM> currentlyRotatingCameras = new List<AssetCAM>();
        private static readonly Dictionary<uint, (Vector3 Forward, Vector3 Up, Vector3 Left)> originalRotationsCameras = new();

        private static void MouseMoveForCameraRotation(int distanceX, bool grid, bool localAxis)
        {
            int index = Array.FindIndex(rotationGizmos, g => g.isSelected);
            if (index < 0)
                return;

            var axis = (Axis)index;
            switch (axis)
            {
                case Axis.X:
                    totalDistanceMoved.X += distanceX;
                    break;
                case Axis.Y:
                    totalDistanceMoved.Y += distanceX;
                    break;
                case Axis.Z:
                    totalDistanceMoved.Z += distanceX;
                    break;
            }

            float angle = axis switch
            {
                Axis.X => totalDistanceMoved.X,
                Axis.Y => totalDistanceMoved.Y,
                _ => totalDistanceMoved.Z
            };
            if (grid)
                angle = SnapToIncrement(angle);

            foreach (var cam in currentlyRotatingCameras)
            {
                (Vector3 Forward, Vector3 Up, Vector3 Left) = originalRotationsCameras[cam.assetID];

                Vector3 rotationAxis;

                if (localAxis)
                {
                    rotationAxis = axis switch
                    {
                        Axis.X => Up,
                        Axis.Y => Left,
                        _ => Forward
                    };
                }
                else
                {
                    rotationAxis = axis switch
                    {
                        Axis.X => Vector3.UnitY,
                        Axis.Y => Vector3.UnitX,
                        _ => Vector3.UnitZ
                    };
                }

                cam.ApplyRotation(Forward, Up, rotationAxis, angle);
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
        private static float SnapToIncrement(float value, float increment = 15.0f)
        {
            // Round to the nearest increment
            return (float)Math.Round(value / increment) * increment;
        }

        private static float SnapToGrid(float value, GizmoType gizmo)
        {
            return gizmo switch
            {
                GizmoType.X => RoundToNearest(value, Grid.X),
                GizmoType.Y => RoundToNearest(value, Grid.Y),
                GizmoType.Z => RoundToNearest(value, Grid.Z),
                _ => 0
            };
        }

        private static float RoundToNearest(float n, float x)
        {
            return (float)Math.Round(n / x) * x;
        }
    }
}
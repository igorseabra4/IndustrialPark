using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark
{
    public class GizmoManager
    {
        private readonly PositionGizmo[] positionGizmos = [
            new PositionGizmo(GizmoType.X),
            new PositionGizmo(GizmoType.Y),
            new PositionGizmo(GizmoType.Z)
        ];

        private readonly BoxTrigPositionGizmo[] triggerPositionGizmos = [
            new BoxTrigPositionGizmo(GizmoType.X),
            new BoxTrigPositionGizmo(GizmoType.Y),
            new BoxTrigPositionGizmo(GizmoType.Z),
            new BoxTrigPositionGizmo(GizmoType.MinX),
            new BoxTrigPositionGizmo(GizmoType.MinY),
            new BoxTrigPositionGizmo(GizmoType.MinZ)
        ];

        private readonly RotationGizmo[] rotationGizmos = [
            new RotationGizmo(GizmoType.Yaw),
            new RotationGizmo(GizmoType.Pitch),
            new RotationGizmo(GizmoType.Roll)
        ];

        private readonly ScaleGizmo[] scaleGizmos = [
            new ScaleGizmo(GizmoType.ScaleX),
            new ScaleGizmo(GizmoType.ScaleY),
            new ScaleGizmo(GizmoType.ScaleZ),
            new ScaleGizmo(GizmoType.ScaleAll)
        ];

        public GizmoMode CurrentGizmoMode
        {
            get;
            set
            {
                FinishGizmoMovement();
                field = value != GizmoMode.Cycle ? value :
                    CurrentGizmoMode switch
                    {
                        GizmoMode.Position => GizmoMode.Rotation,
                        GizmoMode.Rotation => GizmoMode.Scale,
                        GizmoMode.Scale => GizmoMode.PositionLocal,
                        _ => GizmoMode.Position,
                    };
            }
        } = GizmoMode.None;

        public bool SkipGizmos { get; set; } = false;
        public bool RotationGizmoLocalAxis { get; set; } = true;
        public bool IsMovingGizmo { get; private set; } = false;

        private Func<IEnumerable<Asset>> GetSelectedAssets;

        private List<IClickableAsset> currentlyMoving = new List<IClickableAsset>();
        private List<IRotatableAsset> currentlyRotating = new List<IRotatableAsset>();
        private List<IScalableAsset> currentlyScaling = new List<IScalableAsset>();
        private List<Asset> currentlyMovingBoxes = new List<Asset>();
        private List<AssetCAM> currentlyRotatingCameras = new List<AssetCAM>();

        private readonly Dictionary<uint, Vector3> originalPositions = new Dictionary<uint, Vector3>();
        private readonly Dictionary<uint, (Vector3 Maximum, Vector3 Minimum)> originalPositionsBoxes = new Dictionary<uint, (Vector3, Vector3)>();
        private readonly Dictionary<uint, (Vector3 Forward, Vector3 Up, Vector3 Left)> originalRotationsCameras = new();

        private bool TriggerGizmo = false;

        private Vector3 totalDistanceMoved = Vector3.Zero;
        private readonly float movementScale = 0.1f;
        public Vector3 Grid
        {
            get;
            set
            {
                field = value;
                field.X = Math.Max(0.0001f, field.X);
                field.Y = Math.Max(0.0001f, field.Y);
                field.Z = Math.Max(0.0001f, field.Z);
            }
        }

        public GizmoManager(Func<IEnumerable<Asset>> getSelectedAssets, Vector3? grid = null)
        {
            GetSelectedAssets = getSelectedAssets;
            Grid = grid ?? Vector3.One;
        }

        public void RenderGizmos(SharpRenderer renderer)
        {
            // calculate which gizmos should show up and where and render them

            if (SkipGizmos && !IsMovingGizmo)
                return;

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                {
                    var clickableAssets = GetSelectedAssets().OfType<IClickableAsset>().ToList();
                    if (clickableAssets.Count == 0)
                        return;

                    BoundingBox boundingBox = clickableAssets[0].GetBoundingBox();
                    foreach (var asset in clickableAssets.Skip(1))
                        boundingBox = BoundingBox.Merge(boundingBox, asset.GetBoundingBox());
                    var gizmoPosition = clickableAssets.Count > 1 ? boundingBox.Center :
                        new Vector3(clickableAssets[0].PositionX, clickableAssets[0].PositionY, clickableAssets[0].PositionZ);
                    float distance = Vector3.Distance(renderer.Camera.Position, gizmoPosition);

                    foreach (PositionGizmo gizmo in positionGizmos)
                        gizmo.SetPosition(gizmoPosition, distance, Matrix.Identity);
                    foreach (PositionGizmo gizmo in positionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                        gizmo.Draw(renderer, IsMovingGizmo);
                    return;
                }
                case GizmoMode.Rotation:
                {
                    var cameras = GetSelectedAssets().OfType<AssetCAM>().ToList();

                    Vector3 gizmoPosition;
                    Matrix gizmoRotation;

                    if (cameras.Count != 0)
                    {
                        var asset = cameras[0];
                        gizmoRotation = asset.GetCameraRotationMatrix();
                        gizmoPosition = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                    }
                    else
                    {
                        var rotatableAssets = GetSelectedAssets().OfType<IRotatableAsset>().ToList();
                        if (rotatableAssets.Count == 0)
                            return;

                        var asset = rotatableAssets[0];
                        gizmoRotation = GetRotationMatrix(asset.Yaw, asset.Pitch, asset.Roll);
                        gizmoPosition = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                    }

                    float distance = Vector3.Distance(renderer.Camera.Position, gizmoPosition);

                    for (int i = rotationGizmos.Length - 1; i >= 0; i--)
                    {
                        rotationGizmos[i].SetPosition(gizmoPosition, distance, gizmoRotation);
                        rotationGizmos[i].Draw(renderer, IsMovingGizmo);
                    }
                    return;
                }
                case GizmoMode.Scale:
                {
                    var selectedAssets = GetSelectedAssets();
                    var scalableAssets = selectedAssets.OfType<IScalableAsset>().ToList();
                    var boxVolumes = selectedAssets.OfType<AssetVOLU>().Where(v => v.VolumeShape is VolumeBox).ToList();

                    TriggerGizmo = false;
                    BoundingBox bbox = default;
                    IScalableAsset primaryAsset = scalableAssets.FirstOrDefault();
                    Matrix gizmoRotation = default;

                    if (boxVolumes.Count > 0)
                    {
                        var box = (VolumeBox)boxVolumes[0].VolumeShape;
                        TriggerGizmo = true;
                        bbox = box.GetBoundingBox();
                        gizmoRotation = Matrix.Identity;
                    }
                    if (primaryAsset != null)
                    {
                        if (primaryAsset is AssetTRIG { Shape: TriggerShape.Box } trigger)
                        {
                            TriggerGizmo = true;
                            bbox = trigger.GetBoundingBox();
                            gizmoRotation = GetRotationMatrix(trigger.Yaw, trigger.Pitch, trigger.Roll);
                        }
                        else if (primaryAsset is IRotatableAsset rotatable)
                        {
                            gizmoRotation = GetRotationMatrix(rotatable.Yaw, rotatable.Pitch, rotatable.Roll);
                        }
                        else
                        {
                            gizmoRotation = Matrix.Identity;
                        }
                    }
                    if (primaryAsset != null || boxVolumes.Count > 0)
                    {
                        var gizmoPosition = TriggerGizmo ? bbox.Center : new Vector3(primaryAsset.PositionX, primaryAsset.PositionY, primaryAsset.PositionZ);
                        float distance = Vector3.Distance(renderer.Camera.Position, gizmoPosition);
                        foreach (ScaleGizmo gizmo in scaleGizmos)
                            gizmo.SetPosition(gizmoPosition, distance, gizmoRotation);
                        foreach (ScaleGizmo gizmo in scaleGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer, IsMovingGizmo);
                    }
                    return;
                }
                case GizmoMode.PositionLocal:
                {
                    var clickableAssets = GetSelectedAssets().OfType<IClickableAsset>().Take(2).ToList();
                    if (clickableAssets.Count != 1)
                        return;

                    var asset = clickableAssets[0];

                    Matrix gizmoRotation = default;
                    TriggerGizmo = false;
                    IVolumeAsset volume = null;

                    switch (asset)
                    {
                        case AssetTRIG { Shape: TriggerShape.Box } trigger:
                            TriggerGizmo = true;
                            gizmoRotation = GetRotationMatrix(trigger.Yaw, trigger.Pitch, trigger.Roll);
                            volume = trigger;
                            break;
                        case AssetVOLU { VolumeShape: VolumeBox box }:
                            TriggerGizmo = true;
                            gizmoRotation = Matrix.Identity;
                            volume = box;
                            break;
                        case AssetCAM cam:
                            gizmoRotation = cam.GetCameraRotationMatrix();
                            break;
                        case IRotatableAsset ra:
                            gizmoRotation = GetRotationMatrix(ra.Yaw, ra.Pitch, ra.Roll);
                            break;
                        default:
                            gizmoRotation = Matrix.Identity;
                            break;
                    }

                    if (TriggerGizmo)
                    {
                        Vector3 halfExtents = new Vector3(volume.MaximumX - volume.MinimumX, volume.MaximumY - volume.MinimumY, volume.MaximumZ - volume.MinimumZ) / 2f;
                        Vector3 center = volume.GetBoundingBox().Center;
                        float distance = Vector3.Distance(renderer.Camera.Position, center);

                        foreach (BoxTrigPositionGizmo gizmo in triggerPositionGizmos)
                            gizmo.SetPosition(center, halfExtents, distance, gizmoRotation);
                        foreach (BoxTrigPositionGizmo gizmo in triggerPositionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer, IsMovingGizmo);
                    }
                    else
                    {
                        var gizmoPosition = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                        float distance = Vector3.Distance(renderer.Camera.Position, gizmoPosition);

                        foreach (PositionGizmo gizmo in positionGizmos)
                            gizmo.SetPosition(gizmoPosition, distance, gizmoRotation);
                        foreach (PositionGizmo gizmo in positionGizmos.OrderByDescending(g => g.GetDistance(renderer.Camera.Position)))
                            gizmo.Draw(renderer, IsMovingGizmo);
                    }

                    return;
                }
            }
        }

        private static Matrix GetRotationMatrix(float Yaw, float Pitch, float Roll) =>
            Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll));

        public void TrySelectGizmo(Ray r)
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

        private void SaveOriginalPositions()
        {
            currentlyMoving = GetSelectedAssets().OfType<IClickableAsset>().ToList();
            foreach (var asset in currentlyMoving)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
        }

        private void SaveOriginalRotations()
        {
            currentlyRotating = GetSelectedAssets().OfType<IRotatableAsset>().ToList();
            foreach (var asset in currentlyRotating)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.Yaw, asset.Pitch, asset.Roll);
        }

        private void SaveOriginalScales()
        {
            currentlyScaling = GetSelectedAssets().OfType<IScalableAsset>().Where(a => !currentlyMovingBoxes.Contains((Asset)a)).ToList();
            foreach (var asset in currentlyScaling)
                originalPositions[((Asset)asset).assetID] = new Vector3(asset.ScaleX, asset.ScaleY, asset.ScaleZ);
        }

        private void SaveOriginalPositionsBoxes()
        {
            currentlyMovingBoxes = GetSelectedAssets().Where(a => a is AssetTRIG trig && trig.Shape == TriggerShape.Box || a is AssetVOLU volu && volu.VolumeShape is VolumeBox).ToList();
            foreach (var asset in currentlyMovingBoxes)
            {
                var box = asset is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)asset).VolumeShape;
                originalPositionsBoxes[asset.assetID] = (new Vector3(box.MaximumX, box.MaximumY, box.MaximumZ), new Vector3(box.MinimumX, box.MinimumY, box.MinimumZ));
            }
        }

        private void SaveOriginalRotationsCameras()
        {
            currentlyRotatingCameras = GetSelectedAssets().OfType<AssetCAM>().ToList();
            foreach (var cam in currentlyRotatingCameras)
                originalRotationsCameras[cam.assetID] = cam.GetCameraOrientation();
        }

        public void FinishGizmoMovement()
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset((Asset)a);
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
                                    AddToActionsBoxMovement(archive, trig, g.Type, actions);
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
                                AddToActionsBoxMovement(archive, volu, g.Type, actions);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset((Asset)a);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset(cam);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset((Asset)a);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset(a);
                            archive.RefreshAssetEditor(a.assetID);
                            AddToActionsBoxMovement(archive, a, g.Type, actions);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset((Asset)a);
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
                            var archive = ArchiveEditorFunctions.ArchiveContainingAsset(a);
                            archive.RefreshAssetEditor(a.assetID);
                            AddToActionsBoxMovement(archive, a, g.Type, actions);
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

            actions.Add(new SelectionAction(GetSelectedAssets().Select(asset => asset.assetID)));
            Program.UndoBuffer.AddAction(actions);
        }

        private void AddToActionsBoxMovement(ArchiveEditorFunctions archive, Asset boxAsset, GizmoType type, List<IReversibleAction> actions)
        {
            (Vector3 ogMaximum, Vector3 ogMinimum) = originalPositionsBoxes[boxAsset.assetID];
            if (boxAsset is AssetTRIG trig)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.MinX:
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumX", ogMinimum.X, trig.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MaximumX", ogMaximum.X, trig.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.MinY:
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumY", ogMinimum.Y, trig.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MaximumY", ogMaximum.Y, trig.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.MinZ:
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MinimumZ", ogMinimum.Z, trig.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(archive, trig, "MaximumZ", ogMaximum.Z, trig.MaximumZ));
                        break;
                }
            }
            else if (boxAsset is AssetVOLU volume && volume.VolumeShape is VolumeBox box)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.MinX:
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MinimumX"], ogMinimum.X, box.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MaximumX"], ogMaximum.X, box.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.MinY:
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MinimumY"], ogMinimum.Y, box.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MaximumY"], ogMaximum.Y, box.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.MinZ:
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MinimumZ"], ogMinimum.Z, box.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(archive, volume, ["VolumeShape", "MaximumZ"], ogMaximum.Z, box.MaximumZ));
                        break;
                }
            }
        }

        public void MouseMoveGizmos(Matrix viewProjection, int deltaX, int deltaY, bool grid)
        {
            if (!IsMovingGizmo)
                return;

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                    if (currentlyMoving.Count != 0)
                        GizmoPositionMove(viewProjection, deltaX, deltaY, grid);
                    break;
                case GizmoMode.Rotation:
                    if (currentlyRotating.Count != 0)
                    {
                        if (RotationGizmoLocalAxis)
                            GizmoRotationLocalAxisMove(deltaX, grid);
                        else
                            GizmoRotationSingleAxisMove(deltaX, grid);
                    }
                    if (currentlyRotatingCameras.Count != 0)
                        GizmoRotationCameraMove(deltaX, grid, RotationGizmoLocalAxis);
                    break;
                case GizmoMode.Scale:
                    if (currentlyScaling.Count != 0)
                        GizmoScaleMove(viewProjection, deltaX, deltaY, grid);
                    if (currentlyMovingBoxes.Count != 0)
                        GizmoScaleBoxesMove(viewProjection, deltaX, deltaY, grid);
                    break;
                case GizmoMode.PositionLocal:
                    if (currentlyMoving.Count != 0)
                        GizmoPositionLocalMove(viewProjection, deltaX, deltaY, grid);
                    if (currentlyMovingBoxes.Count != 0)
                        GizmoPositionLocalBoxesMove(viewProjection, deltaX, deltaY, grid);
                    break;
            }
        }

        private void GizmoPositionMove(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            var gizmo = positionGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            Vector3 direction1 = Vector3.TransformCoordinate(gizmo.Position, viewProjection);
            Vector3 direction2 = Vector3.TransformCoordinate(gizmo.Position + gizmo.AxisVector, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float scaledMovement = movement * movementScale;

            foreach (var asset in currentlyMoving)
            {
                float assetMovement = scaledMovement;
                if (gizmo.Type == GizmoType.Y && asset is AssetUI)
                    assetMovement *= -1;

                switch (gizmo.Type)
                {
                    case GizmoType.X:
                        totalDistanceMoved.X += scaledMovement;
                        break;
                    case GizmoType.Y:
                        totalDistanceMoved.Y += scaledMovement;
                        break;
                    case GizmoType.Z:
                        totalDistanceMoved.Z += scaledMovement;
                        break;
                }

                uint assetID = ((Asset)asset).assetID;

                float totalAxisMovement = gizmo.Type switch
                {
                    GizmoType.X => totalDistanceMoved.X,
                    GizmoType.Y => totalDistanceMoved.Y,
                    _ => totalDistanceMoved.Z,
                };

                if (asset is AssetTRIG trig && trig.Shape == TriggerShape.Box)
                {
                    var originalPosition = originalPositions[assetID];
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    switch (gizmo.Type)
                    {
                        case GizmoType.X:
                            if (grid)
                            {
                                trig.PositionX = SnapToGrid(originalPosition.X + totalAxisMovement, gizmo.Type);
                                trig.MinimumX = SnapToGrid(Minimum.X + totalAxisMovement, gizmo.Type);
                                trig.MaximumX = SnapToGrid(Maximum.X + totalAxisMovement, gizmo.Type);
                            }
                            else
                            {
                                trig.PositionX += assetMovement;
                                trig.MinimumX += assetMovement;
                                trig.MaximumX += assetMovement;
                            }
                            break;
                        case GizmoType.Y:
                            if (grid)
                            {
                                trig.PositionY = SnapToGrid(originalPosition.Y + totalAxisMovement, gizmo.Type);
                                trig.MinimumY = SnapToGrid(Minimum.Y + totalAxisMovement, gizmo.Type);
                                trig.MaximumY = SnapToGrid(Maximum.Y + totalAxisMovement, gizmo.Type);
                            }
                            else
                            {
                                trig.PositionY += assetMovement;
                                trig.MinimumY += assetMovement;
                                trig.MaximumY += assetMovement;
                            }
                            break;
                        case GizmoType.Z:
                            if (grid)
                            {
                                trig.PositionZ = SnapToGrid(originalPosition.Z + totalAxisMovement, gizmo.Type);
                                trig.MinimumZ = SnapToGrid(Minimum.Z + totalAxisMovement, gizmo.Type);
                                trig.MaximumZ = SnapToGrid(Maximum.Z + totalAxisMovement, gizmo.Type);
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
                    switch (gizmo.Type)
                    {
                        case GizmoType.X:
                            if (grid)
                            {
                                vbox.MinimumX = SnapToGrid(Minimum.X + totalAxisMovement, gizmo.Type);
                                vbox.MaximumX = SnapToGrid(Maximum.X + totalAxisMovement, gizmo.Type);
                            }
                            else
                            {
                                vbox.MinimumX += assetMovement;
                                vbox.MaximumX += assetMovement;
                            }
                            break;
                        case GizmoType.Y:
                            if (grid)
                            {
                                vbox.MinimumY = SnapToGrid(Minimum.Y + totalAxisMovement, gizmo.Type);
                                vbox.MaximumY = SnapToGrid(Maximum.Y + totalAxisMovement, gizmo.Type);
                            }
                            else
                            {
                                vbox.MinimumY += assetMovement;
                                vbox.MaximumY += assetMovement;
                            }
                            break;
                        case GizmoType.Z:
                            if (grid)
                            {
                                vbox.MinimumZ = SnapToGrid(Minimum.Z + totalAxisMovement, gizmo.Type);
                                vbox.MaximumZ = SnapToGrid(Maximum.Z + totalAxisMovement, gizmo.Type);
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
                    float originalAxisPosition = gizmo.Type switch
                    {
                        GizmoType.X => originalPosition.X,
                        GizmoType.Y => originalPosition.Y,
                        _ => originalPosition.Z
                    };
                    float newPosition = grid
                        ? SnapToGrid(originalAxisPosition + totalAxisMovement, gizmo.Type)
                        : GetCurrentPosition(asset, gizmo.Type) + assetMovement;

                    SetPosition(asset, gizmo.Type, newPosition);

                    if (asset is AssetTRIG roundTrig)
                        SetTriggerMinimum(roundTrig, gizmo.Type, newPosition);
                }
            }
        }

        private static float GetCurrentPosition(IClickableAsset asset, GizmoType axis)
        {
            return axis switch
            {
                GizmoType.X => asset.PositionX,
                GizmoType.Y => asset.PositionY,
                _ => asset.PositionZ
            };
        }

        private static void SetPosition(IClickableAsset asset, GizmoType axis, float value)
        {
            switch (axis)
            {
                case GizmoType.X:
                    asset.PositionX = value;
                    break;
                case GizmoType.Y:
                    asset.PositionY = value;
                    break;
                case GizmoType.Z:
                    asset.PositionZ = value;
                    break;
            }
        }

        private static void SetTriggerMinimum(AssetTRIG trigger, GizmoType axis, float value)
        {
            switch (axis)
            {
                case GizmoType.X:
                    trigger.MinimumX = value;
                    break;
                case GizmoType.Y:
                    trigger.MinimumY = value;
                    break;
                case GizmoType.Z:
                    trigger.MinimumZ = value;
                    break;
            }
        }

        private void GizmoPositionLocalBoxesMove(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            var gizmo = triggerPositionGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            Vector3 movementDirection = Vector3.TransformCoordinate(gizmo.AxisVector, gizmo.Rotation);
            Vector3 direction1 = Vector3.TransformCoordinate(gizmo.Position, viewProjection);
            Vector3 direction2 = Vector3.TransformCoordinate(gizmo.Position + movementDirection, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float scaledMovement = movement * movementScale;

            foreach (var asset in currentlyMovingBoxes)
            {
                IVolumeAsset box = asset is AssetTRIG trigger ? trigger : (IVolumeAsset)((AssetVOLU)asset).VolumeShape;
                uint assetID = asset.assetID;
                
                switch (gizmo.Type)
                {
                    case GizmoType.X:
                    case GizmoType.MinX:
                        totalDistanceMoved.X += scaledMovement;
                        break;
                    case GizmoType.Y:
                    case GizmoType.MinY:
                        totalDistanceMoved.Y += scaledMovement;
                        break;
                    case GizmoType.Z:
                    case GizmoType.MinZ:
                        totalDistanceMoved.Z += scaledMovement;
                        break;
                }
                float totalAxisMovement = gizmo.Type switch
                {
                    GizmoType.X or GizmoType.MinX => totalDistanceMoved.X,
                    GizmoType.Y or GizmoType.MinY => totalDistanceMoved.Y,
                    _ => totalDistanceMoved.Z
                };
                float currentBound = gizmo.Type switch
                {
                    GizmoType.X => box.MaximumX,
                    GizmoType.MinX => box.MinimumX,
                    GizmoType.Y => box.MaximumY,
                    GizmoType.MinY => box.MinimumY,
                    GizmoType.Z => box.MaximumZ,
                    _ => box.MinimumZ
                };
                float newBound;

                if (grid)
                {
                    var (Maximum, Minimum) = originalPositionsBoxes[assetID];
                    float originalBound = gizmo.Type switch
                    {
                        GizmoType.X => Maximum.X,
                        GizmoType.MinX => Minimum.X,
                        GizmoType.Y => Maximum.Y,
                        GizmoType.MinY => Minimum.Y,
                        GizmoType.Z => Maximum.Z,
                        _ => Minimum.Z
                    };
                    newBound = SnapToGrid(originalBound + totalAxisMovement, gizmo.Type);
                }
                else
                {
                    newBound = currentBound + scaledMovement;
                }

                switch (gizmo.Type)
                {
                    case GizmoType.X:
                        box.MaximumX = newBound;
                        break;
                    case GizmoType.MinX:
                        box.MinimumX = newBound;
                        break;
                    case GizmoType.Y:
                        box.MaximumY = newBound;
                        break;
                    case GizmoType.MinY:
                        box.MinimumY = newBound;
                        break;
                    case GizmoType.Z:
                        box.MaximumZ = newBound;
                        break;
                    case GizmoType.MinZ:
                        box.MinimumZ = newBound;
                        break;
                }
            }
        }

        private void GizmoRotationLocalAxisMove(int distanceX, bool grid)
        {
            var gizmo = rotationGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            switch (gizmo.Type)
            {
                case GizmoType.Yaw:
                    totalDistanceMoved.X += distanceX;
                    break;
                case GizmoType.Pitch:
                    totalDistanceMoved.Y += distanceX;
                    break;
                case GizmoType.Roll:
                    totalDistanceMoved.Z += distanceX;
                    break;
            }

            float angle = gizmo.Type switch
            {
                GizmoType.Yaw => totalDistanceMoved.X,
                GizmoType.Pitch => totalDistanceMoved.Y,
                _ => totalDistanceMoved.Z,
            };

            Vector3 localAxis = gizmo.Type switch
            {
                GizmoType.Yaw => Vector3.UnitY,
                GizmoType.Pitch => Vector3.UnitX,
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

        private void GizmoRotationSingleAxisMove(int distanceX, bool grid)
        {
            var gizmo = rotationGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            foreach (var ra in currentlyRotating)
            {
                switch (gizmo.Type)
                {
                    case GizmoType.Yaw:
                        totalDistanceMoved.X += distanceX;
                        if (grid)
                            ra.Yaw = SnapToIncrement(originalPositions[((Asset)ra).assetID].X + totalDistanceMoved.X);
                        else
                            ra.Yaw += distanceX;
                        break;
                    case GizmoType.Pitch:
                        totalDistanceMoved.Y += distanceX;
                        if (grid)
                            ra.Pitch = SnapToIncrement(originalPositions[((Asset)ra).assetID].Y + totalDistanceMoved.Y);
                        else
                            ra.Pitch += distanceX;
                        break;
                    case GizmoType.Roll:
                        totalDistanceMoved.Z += distanceX;
                        if (grid)
                            ra.Roll = SnapToIncrement(originalPositions[((Asset)ra).assetID].Z + totalDistanceMoved.Z);
                        else
                            ra.Roll += distanceX;
                        break;
                }
            }
        }

        private void GizmoScaleMove(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            var gizmo = scaleGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            Vector3 direction = Vector3.Zero;
            float movement;
            if (gizmo.Type != GizmoType.ScaleAll)
            {
                Vector3 direction1 = Vector3.TransformCoordinate(gizmo.Position, viewProjection);
                Vector3 movementDirection = Vector3.TransformCoordinate(gizmo.AxisVector, gizmo.Rotation);
                Vector3 direction2 = Vector3.TransformCoordinate(gizmo.Position + movementDirection, viewProjection);
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
                switch (gizmo.Type)
                {
                    case GizmoType.ScaleX:
                        asset.ScaleX += movement;
                        if (grid)
                            asset.ScaleX = SnapToGrid(asset.ScaleX, GizmoType.X);
                        totalDistanceMoved.X = asset.ScaleX - originalPositions[((Asset)asset).assetID].X;
                        continue;
                    case GizmoType.ScaleY:
                        asset.ScaleY += movement;
                        if (grid)
                            asset.ScaleY = SnapToGrid(asset.ScaleY, GizmoType.Y);
                        totalDistanceMoved.Y = asset.ScaleY - originalPositions[((Asset)asset).assetID].Y;
                        continue;
                    case GizmoType.ScaleZ:
                        asset.ScaleZ += movement;
                        if (grid)
                            asset.ScaleZ = SnapToGrid(asset.ScaleZ, GizmoType.Z);
                        totalDistanceMoved.Z = asset.ScaleZ - originalPositions[((Asset)asset).assetID].Z;
                        continue;
                    case GizmoType.ScaleAll:
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

        private void GizmoScaleBoxesMove(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            var gizmo = scaleGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            Vector3 direction = Vector3.Zero;
            float movement;
            if (gizmo.Type != GizmoType.ScaleAll)
            {
                Vector3 direction1 = Vector3.TransformCoordinate(gizmo.Position, viewProjection);
                Vector3 movementDirection = Vector3.TransformCoordinate(gizmo.AxisVector, gizmo.Rotation);
                Vector3 direction2 = Vector3.TransformCoordinate(gizmo.Position + movementDirection, viewProjection);

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

                if (gizmo.Type != GizmoType.ScaleAll)
                {
                    switch (gizmo.Type)
                    {
                        case GizmoType.X:
                            totalDistanceMoved.X += scaledMovement;
                            break;
                        case GizmoType.Y:
                            totalDistanceMoved.Y += scaledMovement;
                            break;
                        case GizmoType.Z:
                            totalDistanceMoved.Z += scaledMovement;
                            break;
                    }
                }
                else
                {
                    totalDistanceMoved += scaledMovement;
                }
                
                if (gizmo.Type == GizmoType.ScaleAll)
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
                    switch (gizmo.Type)
                    {
                        case GizmoType.ScaleX:
                            box.MaximumX = SnapToGrid(Maximum.X + totalDistanceMoved.X, GizmoType.X);
                            box.MinimumX = SnapToGrid(Minimum.X - totalDistanceMoved.X, GizmoType.X);
                            break;
                        case GizmoType.ScaleY:
                            box.MaximumY = SnapToGrid(Maximum.Y + totalDistanceMoved.Y, GizmoType.Y);
                            box.MinimumY = SnapToGrid(Minimum.Y - totalDistanceMoved.Y, GizmoType.Y);
                            break;
                        case GizmoType.ScaleZ:
                            box.MaximumZ = SnapToGrid(Maximum.Z + totalDistanceMoved.Z, GizmoType.Z);
                            box.MinimumZ = SnapToGrid(Minimum.Z - totalDistanceMoved.Z, GizmoType.Z);
                            break;
                    }
                }
                else
                {
                    switch (gizmo.Type)
                    {
                        case GizmoType.ScaleX:
                            box.MaximumX += scaledMovement;
                            box.MinimumX -= scaledMovement;
                            break;
                        case GizmoType.ScaleY:
                            box.MaximumY += scaledMovement;
                            box.MinimumY -= scaledMovement;
                            break;
                        case GizmoType.ScaleZ:
                            box.MaximumZ += scaledMovement;
                            box.MinimumZ -= scaledMovement;
                            break;
                    }
                }
            }
        }

        private void GizmoPositionLocalMove(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            var gizmo = positionGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            Vector3 movementDirection = Vector3.TransformCoordinate(gizmo.AxisVector, gizmo.Rotation);
            Vector3 direction1 = Vector3.TransformCoordinate(gizmo.Position, viewProjection);
            Vector3 direction2 = Vector3.TransformCoordinate(gizmo.Position + movementDirection, viewProjection);
            Vector3 direction = direction2 - direction1;
            direction.Z = 0;
            direction.Normalize();
            float movement = distanceX * direction.X - distanceY * direction.Y;
            float movementScale = grid ? 1f : this.movementScale;

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

        private void GizmoRotationCameraMove(int distanceX, bool grid, bool localAxis)
        {
            var gizmo = rotationGizmos.FirstOrDefault(pg => pg.isSelected);
            if (gizmo == null)
                return;

            switch (gizmo.Type)
            {
                case GizmoType.Yaw:
                    totalDistanceMoved.X += distanceX;
                    break;
                case GizmoType.Pitch:
                    totalDistanceMoved.Y += distanceX;
                    break;
                case GizmoType.Roll:
                    totalDistanceMoved.Z += distanceX;
                    break;
            }

            float angle = gizmo.Type switch
            {
                GizmoType.Yaw => totalDistanceMoved.X,
                GizmoType.Pitch => totalDistanceMoved.Y,
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
                    rotationAxis = gizmo.Type switch
                    {
                        GizmoType.Yaw => Up,
                        GizmoType.Pitch => Left,
                        _ => Forward
                    };
                }
                else
                {
                    rotationAxis = gizmo.AxisVector;
                }

                cam.ApplyRotation(Forward, Up, rotationAxis, angle);
            }
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
            return (float)Math.Round(value / increment) * increment;
        }

        private float SnapToGrid(float value, GizmoType gizmo) => gizmo switch
        {
            GizmoType.X or GizmoType.MinX => RoundToNearest(value, Grid.X),
            GizmoType.Y or GizmoType.MinY => RoundToNearest(value, Grid.Y),
            GizmoType.Z or GizmoType.MinZ => RoundToNearest(value, Grid.Z),
            _ => 0
        };

        private static float RoundToNearest(float n, float x)
        {
            return (float)Math.Round(n / x) * x;
        }
    }
}
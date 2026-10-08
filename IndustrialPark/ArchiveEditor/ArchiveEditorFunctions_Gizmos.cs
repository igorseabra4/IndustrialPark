using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
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
        private static GizmoBase[] allGizmos;

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

            allGizmos = [.. positionGizmos, .. triggerPositionGizmos, .. rotationGizmos, .. scaleGizmos, .. positionLocalGizmos];

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
            // calculate which gizmos should show up and where and render them

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
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
                    if (iras.Any())
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
                    if (isas.Any())
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
                    var icas = allCurrentlySelectedAssets.OfType<IClickableAsset>();
                    if (icas.Count() != 1)
                        return;
                    var ica = icas.FirstOrDefault();
                    GizmoCenterPosition = ica.GetBoundingBox().Center;
                    float radius = Vector3.Distance(renderer.Camera.Position, GizmoCenterPosition) / 5f;
                    TriggerGizmo = false;
                    IVolumeAsset volume = null;
                    BoundingBox bbox = new BoundingBox();
                    Vector3 trig_pos = new Vector3();
                    if (ica is AssetTRIG TRIG && TRIG.Shape == TriggerShape.Box)
                    {
                        TriggerGizmo = true;
                        SetCenterRotation(TRIG.Yaw, TRIG.Pitch, TRIG.Roll);
                        bbox = TRIG.GetBoundingBox();
                        volume = TRIG;
                        trig_pos = new Vector3(TRIG.PositionX, TRIG.PositionY, TRIG.PositionZ);
                    }
                    else if (ica is AssetVOLU VOLU && VOLU.VolumeShape is VolumeBox box)
                    {
                        TriggerGizmo = true;
                        SetCenterRotation(0, 0, 0);
                        bbox = box.GetBoundingBox();
                        volume = box;
                        trig_pos = new Vector3(box.CenterX, box.CenterY, box.CenterZ);
                    }
                    if (TriggerGizmo)
                    {
                        Vector3 TrigBound = new Vector3(volume.MaximumX - volume.MinimumX, volume.MaximumY - volume.MinimumY, volume.MaximumZ - volume.MinimumZ) / 2f;
                        foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                        {
                            g.SetPosition(bbox.Center, TrigBound, radius, GizmoCenterRotation);
                            g.Draw(renderer);
                        }
                    }
                    else
                    {
                        foreach (PositionLocalGizmo g in positionLocalGizmos)
                        {
                            g.SetPosition(ica.GetBoundingBox().Center, radius, GizmoCenterRotation);
                            g.Draw(renderer);
                        }
                    }
                    return;
            }
        }

        private static Vector3 GizmoCenterPosition;
        private static Matrix GizmoCenterRotation;

        private static void SetCenterRotation(float Yaw, float Pitch, float Roll)
        {
            GizmoCenterRotation = Matrix.RotationYawPitchRoll(MathUtil.DegreesToRadians(Yaw), MathUtil.DegreesToRadians(Pitch), MathUtil.DegreesToRadians(Roll));
        }

        public void GizmoSelect(Ray r)
        {
            // check which gizmo was selected by the ray, if any
            // set currently gizmoing assets and boxes
            // save original positions for future undo and grid

            switch (CurrentGizmoMode)
            {
                case GizmoMode.Position:
                {
                    int index = GetGizmoIntersectionIndex(positionGizmos, r);
                    if (index != -1 && !positionGizmos[index].isSelected)
                    {
                        currentlyMoving = CurrentlySelectedAssets.OfType<IClickableAsset>().ToList();
                        foreach (var asset in currentlyMoving)
                            originalPositions[((Asset)asset).assetID] = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                        positionGizmos[index].isSelected = currentlyMoving.Count != 0;

                        var currentlyMovingBoxes = CurrentlySelectedAssets.Where(a => a is AssetTRIG trig && trig.Shape == TriggerShape.Box || a is AssetVOLU volu && volu.VolumeShape is VolumeBox);
                        foreach (var boxAsset in currentlyMovingBoxes)
                        {
                            var box = boxAsset is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)boxAsset).VolumeShape;
                            originalPositionsBoxes[boxAsset.assetID] = (new Vector3(box.MaximumX, box.MaximumY, box.MaximumZ), new Vector3(box.MinimumX, box.MinimumY, box.MinimumZ));
                        }
                    }
                }
                break;
                case GizmoMode.Rotation:
                {
                    int index = GetGizmoIntersectionIndex(rotationGizmos, r);
                    if (index != -1 && !rotationGizmos[index].isSelected)
                    {
                        currentlyRotating = CurrentlySelectedAssets.OfType<IRotatableAsset>().ToList();
                        foreach (var asset in currentlyRotating)
                            originalPositions[((Asset)asset).assetID] = new Vector3(asset.Yaw, asset.Pitch, asset.Roll);
                        rotationGizmos[index].isSelected = currentlyRotating.Count != 0;
                    }
                }
                break;
                case GizmoMode.Scale:
                {
                    int index = GetGizmoIntersectionIndex(scaleGizmos, r);
                    if (index != -1 && !scaleGizmos[index].isSelected)
                    {
                        currentlyScaling = CurrentlySelectedAssets.OfType<IScalableAsset>().ToList();
                        foreach (var asset in currentlyScaling)
                            originalPositions[((Asset)asset).assetID] = new Vector3(asset.ScaleX, asset.ScaleY, asset.ScaleZ);
                        scaleGizmos[index].isSelected = currentlyScaling.Count != 0;
                    }
                }
                break;
                case GizmoMode.PositionLocal:
                {
                    if (TriggerGizmo)
                    {
                        int index = GetGizmoIntersectionIndex(triggerPositionGizmos, r);
                        if (index != -1 && !triggerPositionGizmos[index].isSelected)
                        {
                            currentlyMovingBox = CurrentlySelectedAssets.FirstOrDefault(a => a is AssetTRIG trig && trig.Shape == TriggerShape.Box || a is AssetVOLU volu && volu.VolumeShape is VolumeBox, null);
                            if (currentlyMovingBox != null)
                            {
                                var box = currentlyMovingBox is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)currentlyMovingBox).VolumeShape;
                                originalPositionsBoxes[currentlyMovingBox.assetID] = (new Vector3(box.MaximumX, box.MaximumY, box.MaximumZ), new Vector3(box.MinimumX, box.MinimumY, box.MinimumZ));
                                triggerPositionGizmos[index].isSelected = true;
                            }
                        }
                    }
                    else
                    {
                        int index = GetGizmoIntersectionIndex(positionLocalGizmos, r);
                        if (index != -1 && !positionLocalGizmos[index].isSelected)
                        {
                            currentlyMoving = CurrentlySelectedAssets.OfType<IClickableAsset>().ToList();
                            foreach (var asset in currentlyMoving)
                                originalPositions[((Asset)asset).assetID] = new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ);
                            positionLocalGizmos[index].isSelected = currentlyMoving.Count != 0;
                        }
                    }
                }
                break;
            }
        }

        private static int GetGizmoIntersectionIndex(GizmoBase[] gizmos, Ray r)
        {
            int index = -1;
            float dist = 1000f;
            for (int g = 0; g < gizmos.Length; g++)
            {
                float? distance = gizmos[g].IntersectsWith(r);
                if (distance != null && distance < dist)
                {
                    dist = (float)distance;
                    index = g;
                }
            }
            return index;
        }

        public void ScreenUnclicked()
        {
            if (!allGizmos.Any(g => g.isSelected))
                return;
            if (!CurrentlySelectedAssets.Any())
                return;

            // deselect all gizmos, save new positions to undo buffer, clear caches

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
                            RefreshAssetEditor(((Asset)a).assetID);
                            switch (g.Type)
                            {
                                case GizmoType.X:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionX", originalPositions[((Asset)a).assetID].X, a.PositionX));
                                    break;
                                case GizmoType.Y:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionY", originalPositions[((Asset)a).assetID].Y, a.PositionY));
                                    break;
                                case GizmoType.Z:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionZ", originalPositions[((Asset)a).assetID].Z, a.PositionZ));
                                    break;
                            }
                            if (a is AssetTRIG trig)
                            {
                                if (trig.Shape == TriggerShape.Box)
                                {
                                    AddToActionsBoxMovement(trig, g.Type, actions);
                                }
                                else
                                {
                                    switch (g.Type)
                                    {
                                        case GizmoType.X:
                                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "MinimumX", originalPositions[((Asset)a).assetID].X, a.PositionX));
                                            break;
                                        case GizmoType.Y:
                                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "MinimumY", originalPositions[((Asset)a).assetID].Y, a.PositionY));
                                            break;
                                        case GizmoType.Z:
                                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "MinimumZ", originalPositions[((Asset)a).assetID].Z, a.PositionZ));
                                            break;
                                    }
                                }
                            }
                            else if (a is AssetVOLU volu)
                            {
                                AddToActionsBoxMovement(volu, g.Type, actions);
                            }
                        }
                        if (currentlyMoving.Count != 0)
                        {
                            currentlyMoving.Clear();
                            originalPositions.Clear();
                            originalPositionsBoxes.Clear();
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
                            RefreshAssetEditor(((Asset)a).assetID);
                            switch (g.Type)
                            {
                                case GizmoType.Yaw:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "Yaw", originalPositions[((Asset)a).assetID].X, a.Yaw));
                                    break;
                                case GizmoType.Pitch:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "Pitch", originalPositions[((Asset)a).assetID].Y, a.Pitch));
                                    break;
                                case GizmoType.Roll:
                                    actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "Roll", originalPositions[((Asset)a).assetID].Z, a.Roll));
                                    break;
                            }
                        }
                        if (currentlyRotating.Count != 0)
                        {
                            currentlyRotating.Clear();
                            originalPositions.Clear();
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
                            RefreshAssetEditor(((Asset)a).assetID);
                            if (g.Type == GizmoType.ScaleX || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "ScaleX", originalPositions[((Asset)a).assetID].X, a.ScaleX));
                            if (g.Type == GizmoType.ScaleY || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "ScaleY", originalPositions[((Asset)a).assetID].Y, a.ScaleY));
                            if (g.Type == GizmoType.ScaleZ || g.Type == GizmoType.ScaleAll)
                                actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "ScaleZ", originalPositions[((Asset)a).assetID].Z, a.ScaleZ));
                        }
                        if (currentlyScaling.Count != 0)
                        {
                            currentlyScaling.Clear();
                            originalPositions.Clear();
                        }
                    }
                    break;
                case GizmoMode.PositionLocal:
                    foreach (PositionLocalGizmo g in positionLocalGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;
                        foreach (var a in currentlyMoving)
                        {
                            RefreshAssetEditor(((Asset)a).assetID);
                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionX", originalPositions[((Asset)a).assetID].X, a.PositionX));
                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionY", originalPositions[((Asset)a).assetID].Y, a.PositionY));
                            actions.Add(new AssetPropertyChangedAction(this, (Asset)a, "PositionZ", originalPositions[((Asset)a).assetID].Z, a.PositionZ));
                        }
                        if (currentlyMoving.Count != 0)
                        {
                            currentlyMoving.Clear();
                            originalPositions.Clear();
                        }
                    }
                    foreach (BoxTrigPositionGizmo g in triggerPositionGizmos)
                    {
                        if (!g.isSelected)
                            continue;
                        g.isSelected = false;
                        RefreshAssetEditor(currentlyMovingBox.assetID);
                        AddToActionsBoxMovement(currentlyMovingBox, g.Type, actions);

                        currentlyMoving.Clear();
                        originalPositions.Clear();
                        originalPositionsBoxes.Clear();
                    }
                    break;
            }

            actions.Add(new SelectionAction(CurrentlySelectedAssets.Select(asset => asset.assetID).ToList()));
            Program.UndoBuffer.AddAction(actions);
        }

        private void AddToActionsBoxMovement(Asset boxAsset, GizmoType type, List<IReversibleAction> actions)
        {
            if (boxAsset is AssetTRIG trig)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.TrigX1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumX", originalPositionsBoxes[boxAsset.assetID].Minimum.X, trig.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumX", originalPositionsBoxes[boxAsset.assetID].Maximum.X, trig.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.TrigY1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumY", originalPositionsBoxes[boxAsset.assetID].Minimum.Y, trig.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumY", originalPositionsBoxes[boxAsset.assetID].Maximum.Y, trig.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.TrigZ1:
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MinimumZ", originalPositionsBoxes[boxAsset.assetID].Minimum.Z, trig.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(this, trig, "MaximumZ", originalPositionsBoxes[boxAsset.assetID].Maximum.Z, trig.MaximumZ));
                        break;
                }
            }
            else if (boxAsset is AssetVOLU volume && volume.VolumeShape is VolumeBox box)
            {
                switch (type)
                {
                    case GizmoType.X:
                    case GizmoType.TrigX1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumX"], originalPositionsBoxes[boxAsset.assetID].Minimum.X, box.MinimumX));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumX"], originalPositionsBoxes[boxAsset.assetID].Maximum.X, box.MaximumX));
                        break;
                    case GizmoType.Y:
                    case GizmoType.TrigY1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumY"], originalPositionsBoxes[boxAsset.assetID].Minimum.Y, box.MinimumY));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumY"], originalPositionsBoxes[boxAsset.assetID].Maximum.Y, box.MaximumY));
                        break;
                    case GizmoType.Z:
                    case GizmoType.TrigZ1:
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MinimumZ"], originalPositionsBoxes[boxAsset.assetID].Minimum.Z, box.MinimumZ));
                        actions.Add(new AssetPropertyChangedAction(this, volume, ["VolumeShape", "MaximumZ"], originalPositionsBoxes[boxAsset.assetID].Maximum.Z, box.MaximumZ));
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
        private static Asset currentlyMovingBox = null;
        private static Dictionary<uint, Vector3> originalPositions = new Dictionary<uint, Vector3>();
        private static Dictionary<uint, (Vector3 Maximum, Vector3 Minimum)> originalPositionsBoxes = new Dictionary<uint, (Vector3, Vector3)>();
        private static Vector3 totalDistanceMoved = Vector3.Zero;
        private static float movementScale = 0.1f;
        public static Vector3 Grid;

        public void MouseMoveForPosition(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            if (!positionGizmos.Any(gizmo => gizmo.isSelected))
                return;

            Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

            foreach (var ra in currentlyMoving.Cast<IClickableAsset>())
            {
                if (positionGizmos[0].isSelected) // X MOVEMENT
                {
                    Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitX, viewProjection);
                    Vector3 direction = direction2 - direction1;
                    direction.Z = 0;
                    direction.Normalize();

                    float movement = distanceX * direction.X - distanceY * direction.Y;
                    float scaledMoveDistance = movement * movementScale;
                    totalDistanceMoved.X += scaledMoveDistance;

                    if (ra is AssetTRIG trig && trig.Shape == TriggerShape.Box)
                    {
                        if (grid)
                        {
                            trig.PositionX = SnapToGrid(originalPositions[((Asset)ra).assetID].X + totalDistanceMoved.X, GizmoType.X);
                            trig.MinimumX = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.X + totalDistanceMoved.X, GizmoType.X);
                            trig.MaximumX = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.X + totalDistanceMoved.X, GizmoType.X);
                        }
                        else
                        {
                            ra.PositionX += movement * movementScale;
                            trig.MinimumX += movement * movementScale;
                            trig.MaximumX += movement * movementScale;
                        }
                    }
                    else if (ra is AssetVOLU volu && volu.VolumeShape is VolumeBox vbox)
                    {
                        if (grid)
                        {
                            vbox.MinimumX = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.X + totalDistanceMoved.X, GizmoType.X);
                            vbox.MaximumX = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.X + totalDistanceMoved.X, GizmoType.X);
                        }
                        else
                        {
                            vbox.MinimumX += movement * movementScale;
                            vbox.MaximumX += movement * movementScale;
                        }
                    }
                    else
                    {
                        if (grid)
                            ra.PositionX = SnapToGrid(originalPositions[((Asset)ra).assetID].X + totalDistanceMoved.X, GizmoType.X);
                        else
                            ra.PositionX += movement * movementScale;

                        if (ra is AssetTRIG roundTrig)
                            roundTrig.MinimumX = roundTrig.PositionX;
                    }
                }
                else if (positionGizmos[1].isSelected) // Y MOVEMENT
                {
                    Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitY, viewProjection);
                    Vector3 direction = direction2 - direction1;
                    direction.Z = 0;
                    direction.Normalize();

                    float movement = distanceX * direction.X - distanceY * direction.Y;
                    if (ra is AssetUI)
                        movement *= -1;
                    float scaledMoveDistance = movement * movementScale;
                    totalDistanceMoved.Y += scaledMoveDistance;

                    if (ra is AssetTRIG trig && trig.Shape == TriggerShape.Box)
                    {
                        if (grid)
                        {
                            trig.PositionY = SnapToGrid(originalPositions[((Asset)ra).assetID].Y + totalDistanceMoved.Y, GizmoType.Y);
                            trig.MinimumY = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.Y + totalDistanceMoved.Y, GizmoType.Y);
                            trig.MaximumY = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.Y + totalDistanceMoved.Y, GizmoType.Y);
                        }
                        else
                        {
                            ra.PositionY += movement * movementScale;
                            trig.MinimumY += movement * movementScale;
                            trig.MaximumY += movement * movementScale;
                        }
                    }
                    else if (ra is AssetVOLU volu && volu.VolumeShape is VolumeBox vbox)
                    {
                        if (grid)
                        {
                            vbox.MinimumY = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.Y + totalDistanceMoved.Y, GizmoType.Y);
                            vbox.MaximumY = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.Y + totalDistanceMoved.Y, GizmoType.Y);
                        }
                        else
                        {
                            vbox.MinimumY += movement * movementScale;
                            vbox.MaximumY += movement * movementScale;
                        }
                    }
                    else
                    {
                        if (grid)
                            ra.PositionY = SnapToGrid(originalPositions[((Asset)ra).assetID].Y + totalDistanceMoved.Y, GizmoType.Y);
                        else
                            ra.PositionY += movement * movementScale;

                        if (ra is AssetTRIG roundTrig)
                            roundTrig.MinimumY = roundTrig.PositionY;
                    }
                }
                else if (positionGizmos[2].isSelected) // Z MOVEMENT
                {
                    Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + Vector3.UnitZ, viewProjection);
                    Vector3 direction = direction2 - direction1;
                    direction.Z = 0;
                    direction.Normalize();

                    float movement = distanceX * direction.X - distanceY * direction.Y;
                    float scaledMoveDistance = movement * movementScale;
                    totalDistanceMoved.Z += scaledMoveDistance;

                    if (ra is AssetTRIG trig && trig.Shape == TriggerShape.Box)
                    {
                        if (grid)
                        {
                            trig.PositionZ = SnapToGrid(originalPositions[((Asset)ra).assetID].Z + totalDistanceMoved.Z, GizmoType.Z);
                            trig.MinimumZ = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.Z + totalDistanceMoved.Z, GizmoType.Z);
                            trig.MaximumZ = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.Z + totalDistanceMoved.Z, GizmoType.Z);
                        }
                        else
                        {
                            ra.PositionZ += movement * movementScale;
                            trig.MinimumZ += movement * movementScale;
                            trig.MaximumZ += movement * movementScale;
                        }
                    }
                    else if (ra is AssetVOLU volu && volu.VolumeShape is VolumeBox vbox)
                    {
                        if (grid)
                        {
                            vbox.MinimumZ = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Minimum.Z + totalDistanceMoved.Z, GizmoType.Z);
                            vbox.MaximumZ = SnapToGrid(originalPositionsBoxes[((Asset)ra).assetID].Maximum.Z + totalDistanceMoved.Z, GizmoType.Z);
                        }
                        else
                        {
                            vbox.MinimumZ += movement * movementScale;
                            vbox.MaximumZ += movement * movementScale;
                        }
                    }
                    else
                    {
                        if (grid)
                            ra.PositionZ = SnapToGrid(originalPositions[((Asset)ra).assetID].Z + totalDistanceMoved.Z, GizmoType.Z);
                        else
                            ra.PositionZ += movement * movementScale;

                        if (ra is AssetTRIG roundTrig)
                            roundTrig.MinimumZ = roundTrig.PositionZ;
                    }
                }

                FinishedMovingGizmo = true;
                UnsavedChanges = true;
            }
        }

        public void MouseMoveForPositionTriggers(Matrix viewProjection, int distanceX, int distanceY, bool grid)
        {
            // Volumes (Box Triggers)
            if (triggerPositionGizmos[0].isSelected || triggerPositionGizmos[1].isSelected || triggerPositionGizmos[2].isSelected
                || triggerPositionGizmos[3].isSelected || triggerPositionGizmos[4].isSelected || triggerPositionGizmos[5].isSelected)
            {
                var box = currentlyMovingBox is AssetTRIG trig ? trig : (IVolumeAsset)((AssetVOLU)currentlyMovingBox).VolumeShape;
                if (box != null)
                {
                    Vector3 direction1 = (Vector3)Vector3.Transform(GizmoCenterPosition, viewProjection);

                    if (triggerPositionGizmos[0].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.X += scaledMoveDistance;
                        if (grid)
                            box.MaximumX = SnapToGrid(originalPositions[((Asset)box).assetID].X + totalDistanceMoved.X, GizmoType.X);
                        else
                            box.MaximumX += movement * movementScale;
                    }
                    else if (triggerPositionGizmos[1].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.Y += scaledMoveDistance;

                        if (grid)
                            box.MaximumY = SnapToGrid(originalPositions[((Asset)box).assetID].Y + totalDistanceMoved.Y, GizmoType.Y);
                        else
                            box.MaximumY += movement * movementScale;
                    }
                    else if (triggerPositionGizmos[2].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.Z += scaledMoveDistance;

                        if (grid)
                            box.MaximumZ = SnapToGrid(originalPositions[((Asset)box).assetID].Z + totalDistanceMoved.Z, GizmoType.Z);
                        else
                            box.MaximumZ += movement * movementScale;
                    }
                    else if (triggerPositionGizmos[3].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitX, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.X += scaledMoveDistance;

                        if (grid)
                            box.MinimumX = SnapToGrid(originalPositions[((Asset)box).assetID].X + totalDistanceMoved.X, GizmoType.X);
                        else
                            box.MinimumX += movement * movementScale;
                    }
                    else if (triggerPositionGizmos[4].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitY, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.Y += scaledMoveDistance;

                        if (grid)
                            box.MinimumY = SnapToGrid(originalPositions[((Asset)box).assetID].Y + totalDistanceMoved.Y, GizmoType.Y);
                        else
                            box.MinimumY += movement * movementScale;
                    }
                    else if (triggerPositionGizmos[5].isSelected)
                    {
                        Vector3 movementDirection = (Vector3)Vector3.Transform(Vector3.UnitZ, GizmoCenterRotation);

                        Vector3 direction2 = (Vector3)Vector3.Transform(GizmoCenterPosition + movementDirection, viewProjection);
                        Vector3 direction = direction2 - direction1;
                        direction.Z = 0;
                        direction.Normalize();

                        float movement = (distanceX * direction.X - distanceY * direction.Y);
                        float scaledMoveDistance = movement * movementScale;
                        totalDistanceMoved.Z += scaledMoveDistance;

                        if (grid)
                            box.MinimumZ = SnapToGrid(originalPositions[((Asset)box).assetID].Z + totalDistanceMoved.Z, GizmoType.Z);
                        else
                            box.MinimumZ += movement * movementScale;
                    }

                    FinishedMovingGizmo = true;
                    UnsavedChanges = true;
                }
            }
        }

        public void MouseMoveForRotation(Matrix viewProjection, int distanceX, bool grid)
        {
            if (rotationGizmos[0].isSelected || rotationGizmos[1].isSelected || rotationGizmos[2].isSelected)
            {
                foreach (var ra in currentlyRotating)
                {
                    if (rotationGizmos[0].isSelected)
                    {
                        totalDistanceMoved.X += distanceX;

                        if (grid)
                            ra.Yaw = SnapToIncrement(originalPositions[((Asset)ra).assetID].X + totalDistanceMoved.X);
                        else
                            ra.Yaw += distanceX;
                    }
                    else if (rotationGizmos[1].isSelected)
                    {
                        totalDistanceMoved.Y += distanceX;

                        if (grid)
                            ra.Pitch = SnapToIncrement(originalPositions[((Asset)ra).assetID].Y + totalDistanceMoved.Y);
                        else
                            ra.Pitch += distanceX;
                    }
                    else if (rotationGizmos[2].isSelected)
                    {
                        totalDistanceMoved.Z += distanceX;

                        if (grid)
                            ra.Roll = SnapToIncrement(originalPositions[((Asset)ra).assetID].Z + totalDistanceMoved.Z);
                        else
                            ra.Roll += distanceX;
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

                        ra.ScaleX += (distanceX * direction.X - distanceY * direction.Y) / 40f;
                        if (grid)
                            ra.ScaleX = SnapToGrid(ra.ScaleX, GizmoType.X);

                        totalDistanceMoved.X = ra.ScaleX - originalPositions[((Asset)ra).assetID].X;
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

                        totalDistanceMoved.Y = ra.ScaleY - originalPositions[((Asset)ra).assetID].Y;
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

                        totalDistanceMoved.Z = ra.ScaleZ - originalPositions[((Asset)ra).assetID].Z;
                    }
                    else if (scaleGizmos[3].isSelected)
                    {
                        ra.ScaleX += distanceX / 40f;
                        ra.ScaleY += distanceX / 40f;
                        ra.ScaleZ += distanceX / 40f;

                        totalDistanceMoved += distanceX / 40f;
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

                foreach (var ra in currentlyMoving)
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

                    totalDistanceMoved = originalPositions[((Asset)ra).assetID] - new Vector3(ra.PositionX, ra.PositionY, ra.PositionZ);

                    if (ra is AssetTRIG trig && trig.Shape != TriggerShape.Box)
                    {
                        trig.MinimumX = trig.PositionX;
                        trig.MinimumY = trig.PositionY;
                        trig.MinimumZ = trig.PositionZ;
                    }
                }

                FinishedMovingGizmo = true;
                UnsavedChanges = true;
            }
        }

        public static GizmoMode ToggleGizmoType(GizmoMode mode = GizmoMode.Null)
        {
            Program.MainForm.ScreenUnclicked();

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
    }
}
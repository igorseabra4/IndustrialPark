using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class MainForm
{
    private bool mouseMode = false;
    private bool cursorHidden = false;
    private FlyModeCursor flyModeCursor = FlyModeCursor.Crosshair;
    private System.Drawing.Point MouseCenter;
    private MouseEventArgs oldMousePosition = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);
    public bool TranslucentWhenOutOfFocus;

    private void renderPanel_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ScreenClicked(ViewRectangle, e.X, e.Y, false);
        }
        else if (e.Button == MouseButtons.Right && PressedKeys.Contains(Keys.ShiftKey))
        {
            Vector3 Position = GetScreenClickedPosition(ViewRectangle, e.X, e.Y);

            if (Program.ArchiveManager.GetTargetArchiveEditor() is ArchiveEditor archiveEditor)
                archiveEditor.PlaceTemplate(CurrentAssetTemplate, Position);
            else
                MessageBox.Show($"The template could not be placed because no Archive Editor is selected as the target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        else if (e.Button == MouseButtons.Right)
        {
            var selected = ArchiveEditors.Any(ae => ae.HasSelectedAssets);

            contextUndoToolStripMenuItem.Enabled = Program.UndoBuffer.CanUndo;
            contextRedoToolStripMenuItem.Enabled = Program.UndoBuffer.CanRedo;
            contextHideToolStripMenuItem.Enabled = selected;
            contextCopyToolStripMenuItem.Enabled = selected;
            contextPasteToolStripMenuItem.Enabled = Program.ArchiveManager.GetTargetArchiveEditor() is ArchiveEditor target && (target.archive.AutoLayers || target.SelectedLayerIndex != -1);
            contextEditDataToolStripMenuItem.Enabled = selected;
            contextRemoveToolStripMenuItem.Enabled = selected;

            contextMenuStripMain.Show(renderPanel.PointToScreen(e.Location));
        }
    }

    private void renderPanel_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.None)
            ScreenUnclicked();

        if (renderer.isDrawingUI)
        {
            float x = ((e.X - renderPanel.ClientRectangle.X) * 640f / renderPanel.ClientRectangle.Width);
            float y = ((e.Y - renderPanel.ClientRectangle.Y) * 480f / renderPanel.ClientRectangle.Height);

            SetToolStripStatusLabel(string.Format("Position: [{0:0.0000}, {1:0.0000}]", x, y)
                + " FPS: " + $"{renderer.sharpFPS.FPS:0.0000}");
        }
        else
        {
            if (mouseMode)
            {
                switch (flyModeCursor)
                {
                    case FlyModeCursor.Default:
                        if (Cursor.Current != Cursors.Default)
                            Cursor = Cursors.Default;
                        break;
                    case FlyModeCursor.Crosshair:
                        if (Cursor.Current != Cursors.Cross)
                            Cursor = Cursors.Cross;
                        break;
                    case FlyModeCursor.Hide:
                        if (!cursorHidden)
                        {
                            cursorHidden = true;
                            Cursor.Hide();
                        }
                        break;
                }

                renderer.Camera.AddYaw((Cursor.Position.X - MouseCenter.X) * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                renderer.Camera.AddPitch((Cursor.Position.Y - MouseCenter.Y) * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);

                Cursor.Position = MouseCenter;
            }
            else
            {
                if (cursorHidden)
                {
                    cursorHidden = false;
                    Cursor.Show();
                }

                if (Cursor.Current != Cursors.Default)
                    Cursor = Cursors.Default;

                if (e.Button == MouseButtons.Middle)
                {
                    float deltaX = e.X - oldMousePosition.X;
                    float deltaY = e.Y - oldMousePosition.Y;
                    if (PressedKeys.Contains(Keys.ControlKey))
                    {
                        renderer.Camera.AddPositionForward(-deltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                    }
                    else if (PressedKeys.Contains(Keys.ShiftKey) && !PressedKeys.Contains(Keys.W) && !PressedKeys.Contains(Keys.A) && !PressedKeys.Contains(Keys.S) && !PressedKeys.Contains(Keys.D))
                    {
                        renderer.Camera.AddPositionSideways(deltaX * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                        renderer.Camera.AddPositionUp(deltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                    }
                    else
                    {
                        float rollRadians = MathUtil.DegreesToRadians(renderer.Camera.Roll);
                        float adjustedDeltaX = deltaX * (float)Math.Cos(rollRadians) - deltaY * (float)Math.Sin(rollRadians);
                        float adjustedDeltaY = deltaX * (float)Math.Sin(rollRadians) + deltaY * (float)Math.Cos(rollRadians);
                        renderer.Camera.AddYaw(adjustedDeltaX * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
                        renderer.Camera.AddPitch(adjustedDeltaY * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);

                        // Clamp camera pitch so it doesn't loop around
                        float pitchPadding = 0.01f;
                        renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
                            -90 + pitchPadding,
                            90 - pitchPadding);
                    }
                }
            }

            if (e.Delta != 0)
                renderer.Camera.AddPositionForward(e.Delta * SharpCamera.DEFAULT_MOUSE_SPEED_MULT);
        }

        if (gizmoManager.IsMovingGizmo)
        {
            var gridKey = UserKeyBindings.FirstOrDefault(x => x.Action == KeyboardAction.SnapToGrid).Binding.Key;
            var grid = PressedKeys.Contains(gridKey);

            gizmoManager.MouseMoveGizmos(renderer.viewProjection, e.X - oldMousePosition.X, e.Y - oldMousePosition.Y, grid);
        }

        oldMousePosition = e;
    }

    private void renderPanel_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            ScreenClicked(ViewRectangle, e.X, e.Y, true);
    }

    private void renderPanel_MouseUp(object sender, MouseEventArgs e)
    {
        ScreenUnclicked();
    }

    private void renderPanel_MouseLeave(object sender, EventArgs e)
    {
        ScreenUnclicked();
    }

    public void ScreenClicked(Rectangle viewRectangle, int X, int Y, bool isMouseDown)
    {
        if (gizmoManager.IsMovingGizmo)
            return;

        bool addToSelection = PressedKeys.Contains(Keys.ShiftKey);
        bool toggleSelection = PressedKeys.Contains(Keys.ControlKey);

        Ray ray = Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection);

        if (isMouseDown)
        {
            if (!addToSelection && !toggleSelection)
                gizmoManager.TrySelectGizmo(ray);
        }
        else
        {
            // both ctrl and shift
            var removeFromSelection = addToSelection && toggleSelection;

            // asset previous selection state must be equal to this, ignored if null
            bool? alreadySelectedFilter = removeFromSelection ? true : addToSelection ? false : null;

            var asset = (Asset)renderer.Assets.GetClickedAsset(renderer, ray, alreadySelectedFilter);
            var archive = Program.ArchiveManager.ArchiveEditorContainingAsset(asset);

            if (asset == null && (addToSelection || toggleSelection))
                return;

            foreach (ArchiveEditor ae in ArchiveEditors)
                if (removeFromSelection)
                    ae.RemoveFromSelection(ae == archive ? asset.assetID : null);
                else
                    ae.SetSelectedAsset(ae == archive ? asset : null, addToSelection, toggleSelection);
            if (Program.EditorManager.AutoDockSelectedAssetEditors)
                renderPanel.Focus();
        }
    }

    public Vector3 GetScreenClickedPosition(Rectangle viewRectangle, int X, int Y) =>
        renderer.Assets.GetRayIntersectionPosition(renderer, Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection));

    public void ScreenUnclicked()
    {
        gizmoManager.FinishGizmoMovement();
    }

    private void MouseModeToggle()
    {
        mouseMode = !mouseMode;
    }

    private void ResetMouseCenter(object sender, EventArgs e)
    {
        MouseCenter = renderPanel.PointToScreen(new System.Drawing.Point(renderPanel.Width / 2, renderPanel.Height / 2));
    }

    public void KeyboardController()
    {
        gizmoManager.SkipGizmos = PressedKeys.Contains(Keys.ShiftKey) || PressedKeys.Contains(Keys.ControlKey);

        if (!renderPanel.Focused || renderer.isDrawingUI)
            return;

        if (PressedKeys.Contains(Keys.A) & PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddYaw(-renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.A))
            renderer.Camera.AddPositionSideways(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.D) & PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddYaw(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.D))
            renderer.Camera.AddPositionSideways(-renderer.TransformScaleFactor);

        if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddPitch(-renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ShiftKey))
            renderer.Camera.AddPositionUp(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.W))
            renderer.Camera.AddPositionForward(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ControlKey) & !PressedKeys.Contains(Keys.ShiftKey))
            renderer.Camera.AddPitch(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddPositionUp(-renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.S) & !PressedKeys.Contains(Keys.ShiftKey) & !PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddPositionForward(-renderer.TransformScaleFactor);

        if (PressedKeys.Contains(Keys.Q) & PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddRoll(renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.Q))
            renderer.Camera.AddPositionUp(-renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.E) & PressedKeys.Contains(Keys.ControlKey))
            renderer.Camera.AddRoll(-renderer.TransformScaleFactor);
        else if (PressedKeys.Contains(Keys.E))
            renderer.Camera.AddPositionUp(renderer.TransformScaleFactor);

        float pitchPadding = 0.01f;
        renderer.Camera.Pitch = MathUtil.Clamp(renderer.Camera.Pitch,
            -90 + pitchPadding,
            90 - pitchPadding);
    }

    private readonly HashSet<Keys> PressedKeys = new HashSet<Keys>();

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Handled)
            return;

        PressedKeys.Add(e.KeyCode);

        if ((PressedKeys.Contains(Keys.Y) && PressedKeys.Contains(Keys.ControlKey)) ||
            (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey)))
        {
            Program.UndoBuffer.Redo();
            e.Handled = true;
            return;
        }
        else if (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey))
        {
            Program.UndoBuffer.Undo();
            e.Handled = true;
            return;
        }
        else if (PressedKeys.Contains(Keys.S) && PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey))
        {
            saveAllOpenHIPsToolStripMenuItem.PerformClick();
            e.Handled = true;
            return;
        }

        if (!renderPanel.Focused)
            return;

        Keys key = e.KeyCode;
        Keys modifiers = e.Modifiers;

        IEnumerable<(KeyboardAction Action, KeyBinding Binding)> all = [.. PresetKeyBindings, .. UserKeyBindings];
        var bindings = all.Where(x => x.Binding.Key == key && x.Binding.Modifiers == modifiers);
        if (bindings.Any())
        {
            HandleAction(bindings.First().Action);
            e.Handled = true;
            return;
        }

        // Open archive editors with ctrl and 1-9
        if (PressedKeys.Contains(Keys.ControlKey))
        {
            // in enum, Keys.D1 through Keys.D9 are numbers 49 to 57.
            int numberKeyOffset = 49;
            var editorsToShow = new List<ArchiveEditor>();

            foreach (var item in PressedKeys)
            {
                if ((int)item >= numberKeyOffset
                    && (int)item <= numberKeyOffset + 8
                    && (int)item - numberKeyOffset < ArchiveEditors.Count)
                {
                    editorsToShow.Add(ArchiveEditors[(int)item - numberKeyOffset]);
                    e.Handled = true;
                }
            }

            foreach (var ae in editorsToShow)
                Program.ArchiveManager.ShowArchiveEditor(ae, this);
        }
    }

    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        PressedKeys.Remove(e.KeyCode);
    }

    private void MainForm_Deactivate(object sender, EventArgs e)
    {
        PressedKeys.Clear();
    }

    private readonly HashSet<(KeyboardAction Action, KeyBinding Binding)> PresetKeyBindings = IpPresetBindings.ToHashSet();

    private readonly HashSet<(KeyboardAction Action, KeyBinding Binding)> UserKeyBindings = new HashSet<(KeyboardAction Action, KeyBinding Binding)>();

    public record KeyBinding(Keys Key, Keys Modifiers = Keys.None)
    {
        public override string ToString()
        {
            var result = new StringBuilder();
            if (Modifiers.HasFlag(Keys.Control))
                result.Append("Ctrl + ");
            if (Modifiers.HasFlag(Keys.Shift))
                result.Append("Shift + ");
            if (Modifiers.HasFlag(Keys.Alt))
                result.Append("Alt + ");
            result.Append(Key.ToString());
            return result.ToString();
        }
    }

    public KeyBinding GetKeyBind(KeyboardAction editorAction)
    {
        IEnumerable<(KeyboardAction Action, KeyBinding Binding)> all = [.. PresetKeyBindings, .. UserKeyBindings];
        return all.Where(b => b.Action == editorAction).Select(b => b.Binding).FirstOrDefault();
    }

    public bool IsKeyBindPreset(KeyboardAction editorAction) => PresetKeyBindings.Any(b => b.Action == editorAction);

    public void SetKeyBind(KeyboardAction editorAction, KeyBinding binding, bool displayError)
    {
        var bindingModifiers = (editorAction == KeyboardAction.SnapToGrid) ? Keys.None : binding.Modifiers;

        var existing = PresetKeyBindings.Where(x => x.Binding.Key == binding.Key && x.Binding.Modifiers == bindingModifiers);

        if (existing.Any())
        {
            if (displayError)
                MessageBox.Show($"The binding {binding} is already assigned and cannot be changed.", "Key Binding Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UserKeyBindings.RemoveWhere(x => x.Action == editorAction || (x.Binding.Key == binding.Key && x.Binding.Modifiers == binding.Modifiers));
        UserKeyBindings.Add((editorAction, new KeyBinding(binding.Key, bindingModifiers)));
    }

    public void ResetKeyBind(KeyboardAction editorAction)
    {
        RemoveKeyBind(editorAction);
        foreach (var binding in IpModernBindings.Where(b => b.Action == editorAction))
            UserKeyBindings.Add(binding);
    }

    public void RemoveKeyBind(KeyboardAction editorAction)
    {
        UserKeyBindings.RemoveWhere(x => x.Action == editorAction);
    }

    public void ResetKeyBinds(bool useClassic)
    {
        UserKeyBindings.Clear();
        foreach (var binding in IpModernBindings)
            UserKeyBindings.Add(binding);
        if (useClassic)
            foreach (var binding in IpClassicBindings)
                SetKeyBind(binding.Action, binding.Binding, false);
    }

    public void HandleAction(KeyboardAction action)
    {
        // note: only do perform click on buttons from menuStrip1 and toolStripMain
        switch (action)
        {
            case KeyboardAction.Copy:
                Program.ArchiveManager.CopySelectedAssets(PressedKeys.Contains(Keys.ShiftKey));
                break;
            case KeyboardAction.Paste:
                Program.ArchiveManager.PasteAssets();
                return;
            case KeyboardAction.Delete:
                if (!ConfirmBeforeDelete || MessageBox.Show("Are you sure you want to delete the selected assets?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    Program.ArchiveManager.DeleteSelectedAssets();
                return;
            case KeyboardAction.ResetCamera:
                renderer.Camera.Reset();
                return;
            case KeyboardAction.ResetRoll:
                renderer.Camera.ResetRoll();
                return;
            case KeyboardAction.ToggleMouseMode:
                MouseModeToggle();
                return;
            case KeyboardAction.NewArchiveEditor:
                AddArchiveEditor(refreshTexturesModels: false);
                return;
            case KeyboardAction.OpenLevel:
                new OpenLevel().Show(this);
                return;
            case KeyboardAction.SelectPreviousTemplate:
                SelectPreviousTemplate();
                break;
            case KeyboardAction.SelectNextTemplate:
                SelectNextTemplate();
                break;
            case KeyboardAction.OpenInternalEditors:
                Program.EditorManager.OpenInternalEditors();
                break;
            case KeyboardAction.OpenHeaderEditor:
                Program.ArchiveManager.OpenHeaderEditor();
                break;
            case KeyboardAction.OpenSelectedAssetArchiveEditor:
                Program.ArchiveManager.OpenSelectedAssetArchiveEditor(this);
                break;
            case KeyboardAction.CloseMostForms:
                CloseMostForms();
                break;
            case KeyboardAction.HideSelectedAssets:
                Program.ArchiveManager.HideSelectedAssets();
                break;
            case KeyboardAction.UnhideAllAssets:
                UnhideAllAssets();
                break;
            case KeyboardAction.DropSelectedAssets:
                Program.ArchiveManager.DropSelectedAssets();
                break;
            case KeyboardAction.CycleGizmoMode:
                SetGizmoMode(GizmoMode.Cycle);
                break;
            case KeyboardAction.RunGame:
                launchGameToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleCulling:
                backfaceCullingToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleFog:
                fogToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleWireframe:
                wireframeToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleHideInvisibleMeshes:
                hideInvisibleGeometryToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleLightKit:
                lightKitToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleMovementPreview:
                movementPreviewToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleVertexColors:
                vertexColorsToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleUIMode:
                uIModeToolStripMenuItem.PerformClick();
                break;
            case KeyboardAction.ToggleTextures:
                texturesToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.IncreaseCameraSpeed:
                renderer.Camera.IncreaseCameraSpeed(SharpCamera.DEFAULT_SPEED_INCREMENT);
                return;
            case KeyboardAction.DecreaseCameraSpeed:
                renderer.Camera.IncreaseCameraSpeed(-SharpCamera.DEFAULT_SPEED_INCREMENT);
                return;
            case KeyboardAction.IncreaseRotationSpeed:
                renderer.Camera.IncreaseCameraRotationSpeed(SharpCamera.DEFAULT_SPEED_ROT_INCREMENT);
                return;
            case KeyboardAction.DecreaseRotationSpeed:
                renderer.Camera.IncreaseCameraRotationSpeed(-SharpCamera.DEFAULT_SPEED_ROT_INCREMENT);
                return;
            case KeyboardAction.ViewControls:
                viewControlsToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.ViewConfig:
                viewConfigToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.ViewAbout:
                aboutToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.SaveAllOpenHIPs:
                saveAllOpenHIPsToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.BuildAndRunPS2ISO:
                buildAndRunPS2ISOToolStripMenuItem.PerformClick();
                return;
            case KeyboardAction.CreateGameCubeBanner:
                createGameCubeBannerToolStripMenuItem.PerformClick();
                return;
        }
    }

    public static readonly List<(KeyboardAction Action, KeyBinding Binding)> IpPresetBindings = new()
    {
        (KeyboardAction.Copy, new KeyBinding(Keys.C, Keys.Control)),
        (KeyboardAction.Paste, new KeyBinding(Keys.V, Keys.Control)),
        (KeyboardAction.Delete, new KeyBinding(Keys.Delete)),
        (KeyboardAction.NewArchiveEditor, new KeyBinding(Keys.N, Keys.Control)),
        (KeyboardAction.OpenLevel, new KeyBinding(Keys.O, Keys.Control)),

        (KeyboardAction.None, new KeyBinding(Keys.W)),
        (KeyboardAction.None, new KeyBinding(Keys.A)),
        (KeyboardAction.None, new KeyBinding(Keys.S)),
        (KeyboardAction.None, new KeyBinding(Keys.D)),
        (KeyboardAction.None, new KeyBinding(Keys.E)),
        (KeyboardAction.None, new KeyBinding(Keys.Q)),
        (KeyboardAction.None, new KeyBinding(Keys.W, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.A, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.S, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.D, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.E, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.Q, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.W, Keys.Shift)),
        (KeyboardAction.None, new KeyBinding(Keys.A, Keys.Shift)),
        (KeyboardAction.None, new KeyBinding(Keys.S, Keys.Shift)),
        (KeyboardAction.None, new KeyBinding(Keys.D, Keys.Shift)),

        (KeyboardAction.None, new KeyBinding(Keys.Z, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.Z, Keys.Control | Keys.Shift)),
        (KeyboardAction.None, new KeyBinding(Keys.Y, Keys.Control)),
        (KeyboardAction.None, new KeyBinding(Keys.S, Keys.Control | Keys.Shift)),
    };

    public static List<(KeyboardAction Action, KeyBinding Binding)> IpModernBindings =>
    [
        (KeyboardAction.SnapToGrid, new KeyBinding(Keys.X)),
        (KeyboardAction.SelectPreviousTemplate, new KeyBinding(Keys.Oemcomma)),
        (KeyboardAction.SelectNextTemplate, new KeyBinding(Keys.OemPeriod)),
        (KeyboardAction.OpenHeaderEditor, new KeyBinding(Keys.H, Keys.Control)),
        (KeyboardAction.OpenInternalEditors, new KeyBinding(Keys.G, Keys.Control)),
        (KeyboardAction.OpenSelectedAssetArchiveEditor, new KeyBinding(Keys.G, Keys.Alt)),
        (KeyboardAction.CloseMostForms, new KeyBinding(Keys.H, Keys.Control | Keys.Shift)),
        (KeyboardAction.HideSelectedAssets, new KeyBinding(Keys.H)),
        (KeyboardAction.UnhideAllAssets, new KeyBinding(Keys.H, Keys.Alt)),
        (KeyboardAction.DropSelectedAssets, new KeyBinding(Keys.B, Keys.Control)),
        (KeyboardAction.CycleGizmoMode, new KeyBinding(Keys.Tab)),
        (KeyboardAction.ToggleCulling, new KeyBinding(Keys.C)),
        (KeyboardAction.ToggleFog, new KeyBinding(Keys.F)),
        (KeyboardAction.ToggleWireframe, new KeyBinding(Keys.Y)),
        (KeyboardAction.ToggleHideInvisibleMeshes, new KeyBinding(Keys.I)),
        (KeyboardAction.ToggleLightKit, new KeyBinding(Keys.L)),
        (KeyboardAction.ToggleVertexColors, new KeyBinding(Keys.V)),
        (KeyboardAction.ToggleUIMode, new KeyBinding(Keys.U)),
        (KeyboardAction.ToggleMovementPreview, new KeyBinding(Keys.M)),
        (KeyboardAction.ToggleMouseMode, new KeyBinding(Keys.Z)),
        (KeyboardAction.ToggleTextures, new KeyBinding(Keys.T)),
        (KeyboardAction.ResetCamera, new KeyBinding(Keys.R)),
        (KeyboardAction.ResetRoll, new KeyBinding(Keys.R, Keys.Control)),
        (KeyboardAction.DecreaseCameraSpeed, new KeyBinding(Keys.D1)),
        (KeyboardAction.IncreaseCameraSpeed, new KeyBinding(Keys.D2)),
        (KeyboardAction.DecreaseRotationSpeed, new KeyBinding(Keys.D3)),
        (KeyboardAction.IncreaseRotationSpeed, new KeyBinding(Keys.D4)),
        (KeyboardAction.ViewControls, new KeyBinding(Keys.F1)),
        (KeyboardAction.ViewConfig, new KeyBinding(Keys.F2)),
        (KeyboardAction.ViewAbout, new KeyBinding(Keys.F3)),
        (KeyboardAction.SaveAllOpenHIPs, new KeyBinding(Keys.F4)),
        (KeyboardAction.RunGame, new KeyBinding(Keys.F5)),
        (KeyboardAction.BuildAndRunPS2ISO, new KeyBinding(Keys.F6)),
        (KeyboardAction.CreateGameCubeBanner, new KeyBinding(Keys.F7)),
    ];

    public static List<(KeyboardAction Action, KeyBinding Binding)> IpClassicBindings =>
    [
        (KeyboardAction.SelectPreviousTemplate, new KeyBinding(Keys.B)),
        (KeyboardAction.SelectNextTemplate, new KeyBinding(Keys.N)),
        (KeyboardAction.OpenInternalEditors, new KeyBinding(Keys.G)),
        (KeyboardAction.OpenSelectedAssetArchiveEditor, new KeyBinding(Keys.G, Keys.Control)),
        (KeyboardAction.CloseMostForms, new KeyBinding(Keys.H, Keys.Control | Keys.Shift)),
        (KeyboardAction.DropSelectedAssets, new KeyBinding(Keys.H)),
        (KeyboardAction.HideSelectedAssets, new KeyBinding(Keys.H, Keys.Control)),
        (KeyboardAction.CycleGizmoMode, new KeyBinding(Keys.V)),
        (KeyboardAction.ToggleCulling, new KeyBinding(Keys.C)),
        (KeyboardAction.ToggleFog, new KeyBinding(Keys.F)),
        (KeyboardAction.ToggleWireframe, new KeyBinding(Keys.Y)),
        (KeyboardAction.ToggleLightKit, new KeyBinding(Keys.L)),
        (KeyboardAction.ToggleVertexColors, new KeyBinding(Keys.P)),
        (KeyboardAction.ToggleUIMode, new KeyBinding(Keys.U)),
        (KeyboardAction.ToggleMouseMode, new KeyBinding(Keys.Z)),
        (KeyboardAction.SnapToGrid, new KeyBinding(Keys.T)),
    ];
}

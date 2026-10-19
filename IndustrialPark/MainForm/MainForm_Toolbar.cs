using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class MainForm
    {
        // The toolbar items are created in MainForm.Designer.cs; this file holds their behaviour.

        private Dictionary<GizmoMode, ToolStripButton> toolbarGizmoButtons;
        private Dictionary<ToolStripButton, AssetType> toolbarVisibilityButtons;

        // Archive editors in the same order as the items of toolStripComboBoxArchive
        private readonly List<ArchiveEditor> toolbarArchiveEditors = new List<ArchiveEditor>();
        private bool updatingToolbar = false;

        private void SetupToolbar()
        {
            toolStripMain.Renderer = new SmoothImageToolStripRenderer();

            toolbarGizmoButtons = new Dictionary<GizmoMode, ToolStripButton>
            {
                { GizmoMode.Position, toolStripButtonGizmoPosition },
                { GizmoMode.Rotation, toolStripButtonGizmoRotation },
                { GizmoMode.Scale, toolStripButtonGizmoScale },
                { GizmoMode.PositionLocal, toolStripButtonGizmoPositionLocal },
            };

            SetupToolbarAssetTypeChooser();
            SetToolbarAssetTypes(DefaultToolbarAssetTypes);

            Program.UndoBuffer.Changed += UpdateToolbarUndoButtons;

            UpdateToolbarUndoButtons();
            UpdateToolbarArchives();
            UpdateToolbarGizmoButtons(ArchiveEditorFunctions.CurrentGizmoMode);
            UpdateToolbarVisibilityButtons();
            toolStripButtonWireframe.Checked = wireframeFToolStripMenuItem.Checked;

            // The menu item is toggled by its click, the L key and project loading, so follow its checked state
            useLightKitsForRenderingToolStripMenuItem.CheckedChanged += (sender, e) =>
                toolStripButtonLightKit.Checked = useLightKitsForRenderingToolStripMenuItem.Checked;
            toolStripButtonLightKit.Checked = useLightKitsForRenderingToolStripMenuItem.Checked;
        }

        private void showToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetToolbarVisible(!showToolbarToolStripMenuItem.Checked);
        }

        private void SetToolbarVisible(bool visible)
        {
            showToolbarToolStripMenuItem.Checked = visible;
            toolStripMain.Visible = visible;
        }

        #region Undo and redo

        private void UpdateToolbarUndoButtons()
        {
            toolStripButtonUndo.Enabled = Program.UndoBuffer.CanUndo;
            toolStripButtonRedo.Enabled = Program.UndoBuffer.CanRedo;
        }

        private void toolStripButtonUndo_Click(object sender, EventArgs e) => Program.UndoBuffer.Undo();

        private void toolStripButtonRedo_Click(object sender, EventArgs e) => Program.UndoBuffer.Redo();

        #endregion

        #region Archive and layer

        /// <summary>
        /// Rebuilds the archive dropdown from the open archive editors and selects the one with Target.
        /// </summary>
        public void UpdateToolbarArchives()
        {
            if (toolbarGizmoButtons == null || updatingToolbar)
                return;

            updatingToolbar = true;

            toolbarArchiveEditors.Clear();
            toolbarArchiveEditors.AddRange(archiveEditors);

            toolStripComboBoxArchive.Items.Clear();
            foreach (var ae in toolbarArchiveEditors)
                toolStripComboBoxArchive.Items.Add(Path.GetFileName(ae.GetCurrentlyOpenFileName()));

            toolStripComboBoxArchive.SelectedIndex = toolbarArchiveEditors.FindIndex(ae => ae.TemplateFocus);
            toolStripComboBoxArchive.Enabled = toolbarArchiveEditors.Count > 0;

            updatingToolbar = false;

            UpdateToolbarLayers();
        }

        private ArchiveEditor ToolbarActiveArchiveEditor =>
            toolStripComboBoxArchive.SelectedIndex == -1 ? null : toolbarArchiveEditors[toolStripComboBoxArchive.SelectedIndex];

        /// <summary>
        /// Rebuilds the layer dropdown from the active archive. Disabled when there is no active archive or it has no layers.
        /// </summary>
        private void UpdateToolbarLayers()
        {
            if (toolbarGizmoButtons == null || updatingToolbar)
                return;

            updatingToolbar = true;

            toolStripComboBoxLayer.Items.Clear();

            var ae = ToolbarActiveArchiveEditor;
            if (ae == null || ae.archive.NoLayers)
            {
                toolStripComboBoxLayer.Enabled = false;
                toolStripComboBoxLayer.ToolTipText = ae == null ? "No active archive" : "This archive has no layers";
            }
            else
            {
                toolStripComboBoxLayer.Enabled = true;
                toolStripComboBoxLayer.ToolTipText = "Active layer of the active archive: templates are placed here";
                toolStripComboBoxLayer.Items.AddRange(ae.LayerNames.ToArray());
                toolStripComboBoxLayer.SelectedIndex = ae.SelectedLayerIndex;
            }

            updatingToolbar = false;
        }

        private void toolStripComboBoxArchive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingToolbar)
                return;

            ToolbarActiveArchiveEditor?.SetTemplateFocus();
            UpdateToolbarLayers();
            renderPanel.Focus();
        }

        private void toolStripComboBoxLayer_DropDown(object sender, EventArgs e)
        {
            // Layers may have been added or renamed in the archive editor
            UpdateToolbarLayers();
        }

        private void toolStripComboBoxLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updatingToolbar)
                return;

            var ae = ToolbarActiveArchiveEditor;
            if (ae != null && !ae.archive.NoLayers)
            {
                updatingToolbar = true;
                ae.SelectedLayerIndex = toolStripComboBoxLayer.SelectedIndex;
                updatingToolbar = false;
            }
            renderPanel.Focus();
        }

        private void toolStripComboBox_DropDownClosed(object sender, EventArgs e)
        {
            // Give keyboard focus back to the 3D view so camera keys work
            renderPanel.Focus();
        }

        /// <summary>
        /// Called by an archive editor when its Template Focus or selected layer changes.
        /// </summary>
        private void ArchiveEditor_ToolbarStateChanged(ArchiveEditor sender)
        {
            if (updatingToolbar)
                return;

            if (sender.TemplateFocus && ToolbarActiveArchiveEditor != sender)
                UpdateToolbarArchives();
            else if (ToolbarActiveArchiveEditor == sender)
            {
                if (sender.TemplateFocus)
                    UpdateToolbarLayers();
                else
                    UpdateToolbarArchives();
            }
        }

        #endregion

        #region Gizmos

        private void UpdateToolbarGizmoButtons(GizmoMode mode)
        {
            if (toolbarGizmoButtons == null)
                return;

            foreach (var entry in toolbarGizmoButtons)
                entry.Value.Checked = entry.Key == mode;
        }

        private void toolStripButtonGizmoPosition_Click(object sender, EventArgs e) => ToggleGizmoType(GizmoMode.Position);

        private void toolStripButtonGizmoRotation_Click(object sender, EventArgs e) => ToggleGizmoType(GizmoMode.Rotation);

        private void toolStripButtonGizmoScale_Click(object sender, EventArgs e) => ToggleGizmoType(GizmoMode.Scale);

        private void toolStripButtonGizmoPositionLocal_Click(object sender, EventArgs e) => ToggleGizmoType(GizmoMode.PositionLocal);

        #endregion

        #region Asset visibility

        private const int MaxToolbarAssetTypes = 10;

        private static readonly AssetType[] DefaultToolbarAssetTypes =
        {
            AssetType.Trigger,
            AssetType.Volume,
            AssetType.MovePoint,
            AssetType.SFX,
            AssetType.SDFX,
        };

        // Asset types with a visibility button on the toolbar, in toolbar order
        private readonly List<AssetType> toolbarAssetTypes = new List<AssetType>();

        /// <summary>
        /// Converts saved asset type names to asset types, dropping unknown names, types without a visibility toggle and duplicates.
        /// </summary>
        private static IEnumerable<AssetType> ParseToolbarAssetTypes(IEnumerable<string> names)
        {
            foreach (var name in names)
                if (Enum.TryParse(name, out AssetType assetType) && assetViewTypes.ContainsKey(assetType))
                    yield return assetType;
        }

        /// <summary>
        /// Replaces the toolbar's visibility buttons with one per asset type (up to MaxToolbarAssetTypes), placed after the type chooser.
        /// </summary>
        private void SetToolbarAssetTypes(IEnumerable<AssetType> assetTypes)
        {
            // Callers may pass a query over toolbarAssetTypes, which is cleared below, so evaluate it first
            var newAssetTypes = assetTypes.Where(assetViewTypes.ContainsKey).Distinct().Take(MaxToolbarAssetTypes).ToList();

            toolStripMain.SuspendLayout();

            if (toolbarVisibilityButtons != null)
                foreach (var button in toolbarVisibilityButtons.Keys)
                {
                    toolStripMain.Items.Remove(button);
                    button.Dispose();
                }

            toolbarAssetTypes.Clear();
            toolbarAssetTypes.AddRange(newAssetTypes);

            toolbarVisibilityButtons = new Dictionary<ToolStripButton, AssetType>();
            int index = toolStripMain.Items.IndexOf(toolStripDropDownButtonShowTypes) + 1;
            foreach (var assetType in toolbarAssetTypes)
            {
                var button = new ToolStripButton { DisplayStyle = ToolStripItemDisplayStyle.Text };
                button.Click += toolStripButtonShow_Click;
                toolStripMain.Items.Insert(index++, button);
                toolbarVisibilityButtons.Add(button, assetType);
            }

            toolStripMain.ResumeLayout();

            UpdateToolbarVisibilityButtons();
        }

        private const int ToolbarAssetTypeListMaxHeight = 400;

        // The type chooser is a scrollable checked list hosted in the dropdown; a plain menu this long can only page with arrow buttons
        private CheckedListBox toolbarAssetTypeList;
        private ToolStripControlHost toolbarAssetTypeListHost;
        private ToolStripLabel toolbarAssetTypeCountLabel;

        // Asset types in the same order as the items of toolbarAssetTypeList
        private readonly List<AssetType> toolbarAssetTypeChoices = new List<AssetType>();

        private void SetupToolbarAssetTypeChooser()
        {
            toolbarAssetTypeList = new CheckedListBox
            {
                CheckOnClick = true,
                BorderStyle = BorderStyle.None,
                IntegralHeight = false,
            };
            toolbarAssetTypeList.ItemCheck += toolbarAssetTypeList_ItemCheck;

            toolbarAssetTypeListHost = new ToolStripControlHost(toolbarAssetTypeList)
            {
                AutoSize = false,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
            };

            toolbarAssetTypeCountLabel = new ToolStripLabel { Margin = new Padding(4, 2, 4, 2) };

            var dropDown = new ToolStripDropDown { Padding = new Padding(1) };
            dropDown.Items.AddRange(new ToolStripItem[] { toolbarAssetTypeListHost, toolbarAssetTypeCountLabel });
            dropDown.Opened += (sender, e) => toolbarAssetTypeList.Focus(); // so the mouse wheel scrolls the list
            toolStripDropDownButtonShowTypes.DropDown = dropDown;
        }

        private void toolStripDropDownButtonShowTypes_DropDownOpening(object sender, EventArgs e)
        {
            var choices = assetViewTypes.Keys
                .Select(t => (assetType: t, name: AssetTypeContainer.AssetTypeToString(t)))
                .OrderBy(t => t.name)
                .ToList();

            updatingToolbar = true;

            toolbarAssetTypeChoices.Clear();
            toolbarAssetTypeList.BeginUpdate();
            toolbarAssetTypeList.Items.Clear();
            foreach (var (assetType, name) in choices)
            {
                toolbarAssetTypeChoices.Add(assetType);
                toolbarAssetTypeList.Items.Add(name, toolbarAssetTypes.Contains(assetType));
            }
            toolbarAssetTypeList.EndUpdate();

            updatingToolbar = false;

            // Wide enough for the longest name plus the check box and scroll bar; tall enough for every item up to the maximum height
            int textWidth = choices.Max(t => TextRenderer.MeasureText(t.name, toolbarAssetTypeList.Font).Width);
            int width = textWidth + SystemInformation.MenuCheckSize.Width + SystemInformation.VerticalScrollBarWidth + 12;
            int height = Math.Min(choices.Count * toolbarAssetTypeList.ItemHeight, ToolbarAssetTypeListMaxHeight);
            toolbarAssetTypeListHost.Size = new System.Drawing.Size(width, height);

            UpdateToolbarAssetTypeCountLabel();
        }

        private void toolbarAssetTypeList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (updatingToolbar)
                return;

            var assetType = toolbarAssetTypeChoices[e.Index];

            // New types go at the end so the existing buttons don't move
            if (e.NewValue == CheckState.Unchecked)
                SetToolbarAssetTypes(toolbarAssetTypes.Where(t => t != assetType));
            else if (toolbarAssetTypes.Count < MaxToolbarAssetTypes)
                SetToolbarAssetTypes(toolbarAssetTypes.Append(assetType));
            else
                e.NewValue = CheckState.Unchecked; // the toolbar is full

            UpdateToolbarAssetTypeCountLabel();
        }

        private void UpdateToolbarAssetTypeCountLabel()
        {
            toolbarAssetTypeCountLabel.Text = toolbarAssetTypes.Count >= MaxToolbarAssetTypes
                ? $"{MaxToolbarAssetTypes} of {MaxToolbarAssetTypes} selected (untick one to add another)"
                : $"{toolbarAssetTypes.Count} of {MaxToolbarAssetTypes} selected";
        }

        private void toolStripButtonShow_Click(object sender, EventArgs e)
        {
            var assetType = toolbarVisibilityButtons[(ToolStripButton)sender];

            var field = assetViewTypes[assetType].GetField("dontRender");
            field.SetValue(null, !(bool)field.GetValue(null));

            if (assetViewToolStripMenuItems != null)
                foreach (var item in assetViewToolStripMenuItems)
                    if ((AssetType)item.Tag == assetType)
                        item.Checked = !(bool)field.GetValue(null);

            UpdateToolbarVisibilityButtons();
        }

        /// <summary>
        /// Syncs the toolbar's visibility buttons with the dontRender flags (which the Display menu also changes).
        /// </summary>
        private void UpdateToolbarVisibilityButtons()
        {
            if (toolbarVisibilityButtons == null)
                return;

            foreach (var entry in toolbarVisibilityButtons)
            {
                bool dontRender = (bool)assetViewTypes[entry.Value].GetField("dontRender").GetValue(null);
                string name = AssetTypeContainer.AssetTypeToString(entry.Value);
                entry.Key.Text = name;
                entry.Key.Checked = !dontRender;
                entry.Key.ToolTipText = (dontRender ? "Show " : "Hide ") + name;
            }
        }

        #endregion

        #region Editing and testing

        private void toolStripButtonDropToGround_Click(object sender, EventArgs e) => DropSelectedAssets();

        private void toolStripButtonWireframe_Click(object sender, EventArgs e) => wireframeFToolStripMenuItem.PerformClick();

        private void toolStripButtonLightKit_Click(object sender, EventArgs e) => useLightKitsForRenderingToolStripMenuItem.PerformClick();

        private void toolStripButtonSaveAll_Click(object sender, EventArgs e) => saveAllOpenHIPsToolStripMenuItem_Click(sender, e);

        private void toolStripButtonRunGame_Click(object sender, EventArgs e) => runGameF5ToolStripMenuItem_Click(sender, e);

        private void toolStripButtonSaveAllAndRun_Click(object sender, EventArgs e)
        {
            // Save on this thread so the files are written before the game starts
            foreach (var ae in archiveEditors)
                if (ae.archive.UnsavedChanges && !ae.SaveAndWait())
                {
                    MessageBox.Show($"{Path.GetFileName(ae.GetCurrentlyOpenFileName())} was not saved, so the game was not started.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            runGameF5ToolStripMenuItem_Click(sender, e);
        }

        #endregion
    }
}

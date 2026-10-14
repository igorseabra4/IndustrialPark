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

            toolbarVisibilityButtons = new Dictionary<ToolStripButton, AssetType>
            {
                { toolStripButtonShowTrigger, AssetType.Trigger },
                { toolStripButtonShowVolume, AssetType.Volume },
                { toolStripButtonShowMovePoint, AssetType.MovePoint },
                { toolStripButtonShowSFX, AssetType.SFX },
                { toolStripButtonShowSDFX, AssetType.SDFX },
            };

            UpdateToolbarArchives();
            UpdateToolbarGizmoButtons(ArchiveEditorFunctions.CurrentGizmoMode);
            UpdateToolbarVisibilityButtons();
            toolStripButtonWireframe.Checked = wireframeFToolStripMenuItem.Checked;
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

        #region Archive and layer

        /// <summary>
        /// Rebuilds the archive dropdown from the open archive editors and selects the one with Template Focus.
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

        private void toolStripButtonWireframe_Click(object sender, EventArgs e) => ToggleWireFrame();

        private void toolStripButtonSaveAll_Click(object sender, EventArgs e) => saveAllOpenHIPsToolStripMenuItem_Click(sender, e);

        private void toolStripButtonRunGame_Click(object sender, EventArgs e) => TryToRunGame();

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

            TryToRunGame();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.MainForm;

namespace IndustrialPark
{
    public partial class ViewControls : Form
    {
        public class ShortcutInfo
        {
            public string Input { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public KeyboardAction? EditorAction;
            public bool IsPreset;

            public ShortcutInfo(string input, string description, string category, KeyboardAction? editorAction = null)
            {
                Input = input;
                Description = description;
                Category = category;
                EditorAction = editorAction;
            }

            public ShortcutInfo(string description, string category, KeyboardAction editorAction)
            {
                Input = null;
                Description = description;
                Category = category;
                EditorAction = editorAction;
            }
        }

        public ViewControls()
        {
            InitializeComponent();

            foreach (var s in shortcuts)
                s.IsPreset = !s.EditorAction.HasValue || Program.MainForm.IsKeybindPreset(s.EditorAction.Value);

            shortcutBindingSource.DataSource = shortcuts;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = shortcutBindingSource;

            dataGridView1.CellFormatting += (_, e) =>
            {
                if (dataGridView1.Rows[e.RowIndex].DataBoundItem is not ShortcutInfo shortcut)
                    return;

                if (shortcut.EditorAction.HasValue)
                {
                    if (!shortcut.IsPreset)
                        e.CellStyle.BackColor = Program.IsDarkMode ? System.Drawing.Color.DarkBlue : System.Drawing.Color.LightBlue;
                    e.FormattingApplied = true;
                }
            };
            dataGridView1.DataBindingComplete += (_, _) =>
            {
                dataGridView1.Columns[0].Width = 136;
                dataGridView1.Columns[1].Width = 360;
                dataGridView1.Columns[2].Width = 66;
            };
            KeyPreview = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
            Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_capturingRow >= 0)
                return;
            MessageBox.Show($"Form size: {Width}, {Height}\nColumn widths: {dataGridView1.Columns[0].Width}, {dataGridView1.Columns[1].Width}, {dataGridView1.Columns[2].Width}");
        }

        private readonly BindingSource shortcutBindingSource = new();

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string search = textBoxSearch.Text.Trim();

            shortcutBindingSource.DataSource = string.IsNullOrEmpty(search)
                ? shortcuts
                : shortcuts.Where(s =>
                    s.Input.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                  .ToList();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Rows[e.RowIndex].DataBoundItem is not ShortcutInfo shortcut || shortcut.IsPreset)
                return;

            _capturingRow = e.RowIndex;
        }

        private readonly List<ShortcutInfo> shortcuts = new()
        {
            new("W / A / S / D", "Move camera forward / left / backward / right", "Camera"),
            new("E / Q", "Move camera up / down", "Camera"),
            new("Shift + W / A / S  D", "Move camera up / left / down / right", "Camera"),
            new("Ctrl + W / A / S / D", "Rotate camera up / left / down / right (pitch and yaw)", "Camera"),
            new("Ctrl + E / Q", "Rotate camera sideways (roll)", "Camera"),
            new("Reset camera roll", "Camera", KeyboardAction.ResetRoll),
            new("Middle mouse + drag", "Rotate camera", "Camera"),
            new("Shift + Middle mouse + drag", "Move camera up / left / down / right", "Camera"),
            new("Ctrl + Middle mouse + drag", "Move camera forward / backward", "Camera"),
            new("Mouse wheel", "Move camera forward / backward", "Camera"),
            new("Decrease camera movement speed", "Camera", KeyboardAction.DecreaseCameraSpeed),
            new("Increase camera movement speed", "Camera", KeyboardAction.IncreaseCameraSpeed),
            new("Decrease camera rotation speed", "Camera", KeyboardAction.DecreaseRotationSpeed),
            new("Increase camera rotation speed", "Camera", KeyboardAction.IncreaseRotationSpeed),
            new("Reset camera view to origin", "Camera", KeyboardAction.ResetCamera),
            new("Toggle fly mode; rotate with the mouse and move with the keyboard", "Camera", KeyboardAction.ToggleMouseMode),

            new("Left mouse", "Select an asset", "Selection"),
            new("Ctrl + Left mouse", "Toggle asset selection", "Selection"),
            new("Shift + Left mouse", "Add asset to selection", "Selection"),
            new("Ctrl + Shift + Left mouse", "Remove asset from selection", "Selection"),
            new("Left mouse + drag", "Transform selected assets with gizmo", "Editing"),
            new("Right mouse", "Choose a gizmo or template from context menu", "Selection"),
            new("Shift + Right mouse", "Place a template", "Editing"),
            new("Select previous template", "Selection", KeyboardAction.SelectPreviousTemplate),
            new("Select next template", "Selection", KeyboardAction.SelectNextTemplate),
            new("Open Asset Data Editor for selected assets", "Tools", KeyboardAction.OpenInternalEditors),
            new("Open Asset Header Editor for selected assets", "Tools", KeyboardAction.OpenHeaderEditor),
            new("Open Archive Editor containing selected assets", "Tools", KeyboardAction.OpenSelectedAssetArchiveEditor),
            new("Hide selected assets", "View", KeyboardAction.HideSelectedAssets),
            new("Unhide all individually hidden assets", "View", KeyboardAction.UnhideAllAssets),
            new(" (twice)", "Unhide all asset types", "View", KeyboardAction.UnhideAllAssets),
            new("Drop selected assets to floor below", "Editing", KeyboardAction.DropSelectedAssets),
            new("Hold while transforming assets with gizmos to snap to grid", "Editing", KeyboardAction.SnapToGrid),
            new("Cycle through gizmos", "Selection", KeyboardAction.CycleGizmoMode),
            new("Delete selected assets", "Editing", KeyboardAction.Delete),
            new("Ctrl + Z", "Undo", "Editing"),
            new("Ctrl + Y", "Redo", "Editing"),
            new("Ctrl + Shift + Z", "Redo", "Editing"),
            new("Ctrl + 1–9", "Open corresponding Archive Editor window", "Tools"),
            new("Close all forms except Archive Editors", "Tools", KeyboardAction.CloseMostForms),

            new("Open new Archive Editor window", "Tools", KeyboardAction.NewArchiveEditor),
            new("Open Level - opens all HIPs needed for a level", "Tools", KeyboardAction.OpenLevel),
            new("Copy selected assets", "Selection", KeyboardAction.Copy),
            new("Paste assets from clipboard", "Editing", KeyboardAction.Paste),

            new("Toggle backface culling", "View", KeyboardAction.ToggleCulling),
            new("Toggle wireframe mode", "View", KeyboardAction.ToggleWireframe),
            new("Toggle vertex color display", "View", KeyboardAction.ToggleVertexColors),
            new("Toggle invisible geometry display", "View", KeyboardAction.ToggleHideInvisibleMeshes),
            new("Toggle light kit view", "View", KeyboardAction.ToggleLightKit),
            new("Toggle fog", "View", KeyboardAction.ToggleFog),
            new("Toggle movement preview", "View", KeyboardAction.ToggleMovementPreview),
            new("Toggle UI mode", "View", KeyboardAction.ToggleUIMode),

            new("Open View Controls", "Tools", KeyboardAction.ViewControls),
            new("Open View Config", "Tools", KeyboardAction.ViewConfig),
            new("Open About Box", "Tools", KeyboardAction.ViewAbout),
            new("Ctrl + Shift + S", "Save all open Archive Editors", "Tools"),
            new("Save all open Archive Editors", "Tools", KeyboardAction.SaveAllOpenHIPs),
            new("Launch game", "Tools", KeyboardAction.RunGame),
            new("Open PS2 ISO Build/Run tool", "Tools", KeyboardAction.BuildAndRunPS2ISO),
            new("Open GameCube banner tool", "Tools", KeyboardAction.CreateGameCubeBanner),
        };

        private int _capturingRow
        {
            get;
            set
            {
                field = value;

                if (value >= 0)
                    labelEditHelp.Text = "Press a key to bind, or Esc to cancel";
                else
                    labelEditHelp.Text = "Double-click blue commands to edit";
            }
        } = -1;

        private void ViewControls_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _capturingRow = -1;
                dataGridView1.Refresh();
                e.Handled = true;
                return;
            }

            if (_capturingRow < 0)
                return;

            if (e.KeyCode is Keys.ControlKey or Keys.ShiftKey or Keys.Menu)
                return;

            Keys modifiers = Control.ModifierKeys & (Keys.Control | Keys.Shift | Keys.Alt);

            var binding = new KeyBinding(e.KeyCode, modifiers);

            var shortcut = (ShortcutInfo)dataGridView1.Rows[_capturingRow].DataBoundItem;

            Program.MainForm.SetKeyBind(shortcut.EditorAction.Value, binding);

            _capturingRow = -1;

            dataGridView1.Refresh();

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            if (e.RowIndex == _capturingRow)
            {
                e.Value = "Press a key...";
                e.FormattingApplied = true;
                return;
            }

            if (dataGridView1.Rows[e.RowIndex].DataBoundItem is ShortcutInfo shortcut && shortcut.EditorAction.HasValue)
            {
                var binding = Program.MainForm.GetKeyBind(shortcut.EditorAction.Value);
                e.Value = binding.ToString() + shortcut.Input;
                e.FormattingApplied = true;
            }
        }

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            Program.MainForm.ResetKeybinds();
            dataGridView1.Refresh();
        }
    }
}

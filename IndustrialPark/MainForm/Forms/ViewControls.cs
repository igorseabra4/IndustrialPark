using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

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
            originalText = labelEditHelp.Text;

            foreach (var s in shortcuts)
                s.IsPreset = !s.EditorAction.HasValue || Program.MainForm.IsKeyBindPreset(s.EditorAction.Value);

            shortcutBindingSource.DataSource = shortcuts;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = shortcutBindingSource;

            dataGridView1.DataBindingComplete += (_, _) =>
            {
                dataGridView1.Columns[0].Width = 136;
                dataGridView1.Columns[1].Width = 360;
                dataGridView1.Columns[2].Width = 66;
            };
            KeyPreview = true;

            foreach (DataGridViewRow row in dataGridView1.Rows)
                row.Selected = false;
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
            _capturingRow = -1;
            string search = textBoxSearch.Text.Trim();

            shortcutBindingSource.DataSource = string.IsNullOrEmpty(search)
                ? shortcuts
                : [.. shortcuts.Where(s =>
                    (s.Input?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (s.Category?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)
                    )];

            foreach (DataGridViewRow row in dataGridView1.Rows)
                row.Selected = false;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].DataBoundItem is not ShortcutInfo shortcut)
                return;

            if (shortcut.EditorAction.HasValue)
            {
                var binding = Program.MainForm.GetKeyBind(shortcut.EditorAction.Value);

                if (!shortcut.IsPreset)
                    e.CellStyle.BackColor = binding == null ?
                        (Program.IsDarkMode ? System.Drawing.Color.DarkRed : System.Drawing.Color.Orange) :
                        (Program.IsDarkMode ? System.Drawing.Color.DarkSlateBlue : System.Drawing.Color.LightBlue);

                if (e.ColumnIndex == 0)
                {
                    if (e.RowIndex == _capturingRow)
                        e.Value = "Press a key...";
                    else if (binding != null)
                        e.Value = binding.ToString() + shortcut.Input;
                    else
                        e.Value = "Unassigned";
                }

                e.FormattingApplied = true;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Rows[e.RowIndex].DataBoundItem is not ShortcutInfo shortcut || shortcut.IsPreset)
                return;

            _capturingRow = e.RowIndex;
            dataGridView1.Invalidate();
        }

        private readonly string originalText;
        private int _capturingRow
        {
            get;
            set
            {
                field = value;

                if (value >= 0)
                    labelEditHelp.Text = "Press a key to bind, or Esc to cancel";
                else
                    labelEditHelp.Text = originalText;
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

            Keys modifiers = ModifierKeys & (Keys.Control | Keys.Shift | Keys.Alt);
            var binding = new MainForm.KeyBinding(e.KeyCode, modifiers);
            var shortcut = (ShortcutInfo)dataGridView1.Rows[_capturingRow].DataBoundItem;
            Program.MainForm.SetKeyBind(shortcut.EditorAction.Value, binding, true);
            _capturingRow = -1;
            dataGridView1.Refresh();

            e.Handled = true;
            e.SuppressKeyPress = true;
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
            new("Hide selected assets", "View", KeyboardAction.HideSelectedAssets),
            new("Unhide all individually hidden assets", "View", KeyboardAction.UnhideAllAssets),
            new(" (twice)", "Unhide all asset types", "View", KeyboardAction.UnhideAllAssets),
            new("Open Asset Header Editor for selected assets", "Tools", KeyboardAction.OpenHeaderEditor),
            new("Open Asset Data Editor for selected assets", "Tools", KeyboardAction.OpenInternalEditors),
            new("Open Archive Editor containing selected assets", "Tools", KeyboardAction.OpenSelectedAssetArchiveEditor),
            new("Close all forms except Archive Editors", "Tools", KeyboardAction.CloseMostForms),
            new("Drop selected assets to floor below", "Editing", KeyboardAction.DropSelectedAssets),
            new("Hold while transforming assets with gizmos to snap to grid", "Editing", KeyboardAction.SnapToGrid),
            new("Cycle through gizmos", "Selection", KeyboardAction.CycleGizmoMode),
            new("Copy selected assets", "Selection", KeyboardAction.Copy),
            new("Paste assets from clipboard", "Editing", KeyboardAction.Paste),
            new("Delete selected assets", "Editing", KeyboardAction.Delete),
            new("Ctrl + Z", "Undo", "Editing"),
            new("Ctrl + Y", "Redo", "Editing"),
            new("Ctrl + Shift + Z", "Redo", "Editing"),
            new("Ctrl + 1–9", "Open corresponding Archive Editor window", "Tools"),

            new("Open new Archive Editor window", "Tools", KeyboardAction.NewArchiveEditor),
            new("Open Level - opens all HIPs needed for a level", "Tools", KeyboardAction.OpenLevel),

            new("Toggle backface culling", "View", KeyboardAction.ToggleCulling),
            new("Toggle wireframe mode", "View", KeyboardAction.ToggleWireframe),
            new("Toggle texture display", "View", KeyboardAction.ToggleTextures),
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
            new("Open C# Script Editor", "Tools", KeyboardAction.OpenScriptEditor),
            new("Open new C# Script Editor", "Tools", KeyboardAction.OpenNewScriptEditor),
        };

        private void buttonDefaults_Click(object sender, EventArgs e)
        {
            bool classic = false;
            if (textBoxSearch.Text.Contains("classic", StringComparison.InvariantCultureIgnoreCase))
            {
                classic = true;
                textBoxSearch.Text = "";
            }
            Program.MainForm.ResetKeyBinds(classic);
            dataGridView1.Refresh();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.Rows[e.RowIndex].DataBoundItem is not ShortcutInfo shortcut || shortcut.IsPreset)
                return;

            if (e.Button == MouseButtons.Right)
            {
                _capturingRow = -1;
                Program.MainForm.ResetKeyBind(shortcut.EditorAction.Value);
                dataGridView1.Invalidate();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                _capturingRow = -1;
                Program.MainForm.RemoveKeyBind(shortcut.EditorAction.Value);
                dataGridView1.Invalidate();
            }
        }
    }
}

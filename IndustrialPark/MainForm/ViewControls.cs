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
            public string Control { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }

            public ShortcutInfo(string shortcut, string description, string category)
            {
                Control = shortcut;
                Description = description;
                Category = category;
            }
        }

        public ViewControls()
        {
            InitializeComponent();

            shortcutBindingSource.DataSource = shortcuts;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = shortcutBindingSource;

            TopMost = true;
        }

        private void ViewControls_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns[0].Width = 136;
            dataGridView1.Columns[1].Width = 360;
            dataGridView1.Columns[2].Width = 66;
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
            MessageBox.Show($"Form size: {Width}, {Height}\nColumn widths: {dataGridView1.Columns[0].Width}, {dataGridView1.Columns[1].Width}, {dataGridView1.Columns[2].Width}");
        }

        private readonly BindingSource shortcutBindingSource = new();

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            string search = textBoxSearch.Text.Trim();

            shortcutBindingSource.DataSource = string.IsNullOrEmpty(search)
                ? shortcuts
                : shortcuts.Where(s =>
                    s.Control.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    s.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                  .ToList();
        }

        private readonly List<ShortcutInfo> shortcuts = new()
        {
            new("W / A / S / D", "Move camera forward / left / backward / right", "Camera"),
            new("E / Q", "Move camera up / down", "Camera"),
            new("Shift + W / S", "Move camera up / down", "Camera"),
            new("Ctrl + W / A / S / D", "Rotate camera (pitch and yaw)", "Camera"),
            new("Ctrl + E / Q", "Rotate camera sideways (roll)", "Camera"),
            new("Ctrl + R", "Reset camera roll", "Camera"),
            new("1 / 2", "Decrease / increase camera movement speed", "Camera"),
            new("3 / 4", "Decrease / increase camera rotation speed", "Camera"),
            new("R", "Reset camera view to origin", "Camera"),
            new("Z", "Toggle fly mode; rotate with the mouse and move with the keyboard", "Camera"),
            new("Middle mouse + drag", "Rotate camera", "Camera"),
            new("Mouse wheel", "Move camera forward / backward", "Camera"),
            new("Ctrl + Right mouse + drag", "Pan camera up / left / down / right", "Camera"),
            
            new("Left mouse", "Select an asset", "Selection"),
            new("Ctrl + Left mouse", "Toggle asset selection", "Selection"),
            new("Shift + Left mouse", "Add asset to selection", "Selection"),
            new("Ctrl + Shift + Left mouse", "Remove asset from selection", "Selection"),
            new("Left mouse + drag", "Transform selected assets with gizmo", "Editing"),
            new("Right mouse", "Choose a gizmo or template from context menu", "Selection"),
            new("Shift + Right mouse", "Place a template", "Editing"),
            new("B / N", "Select previous / next template", "Selection"),
            new("G", "Open Asset Data Editor for selected assets", "Tools"),
            new("Ctrl + G", "Open Archive Editor containing selected assets", "Tools"),
            new("Ctrl + H", "Drop selected assets to floor below", "Editing"),
            new("J", "Hide selected assets", "View"),
            new("Ctrl + J", "Unhide all individually hidden assets", "View"),
            new("Ctrl + J (twice)", "Unhide all asset types", "View"),
            new("T", "Hold while transforming assets with gizmos to snap to grid", "Editing"),
            new("V", "Cycle through gizmos", "Selection"),
            new("Delete", "Delete selected assets", "Editing"),
            new("Ctrl + Z", "Undo", "Editing"),
            new("Ctrl + Y", "Redo", "Editing"),
            new("Ctrl + Shift + Z", "Redo", "Editing"),
            new("Ctrl + 1–9", "Open corresponding Archive Editor window", "Tools"),

            new("C", "Toggle backface culling", "View"),
            new("F", "Toggle fog", "View"),
            new("H", "Toggle invisible geometry display", "View"),
            new("L", "Toggle light kit view", "View"),
            new("M", "Toggle movement preview", "View"),
            new("P", "Toggle vertex color display", "View"),
            new("U", "Toggle UI mode", "View"),
            new("Y", "Toggle wireframe mode", "View"),

            new("F1", "Open View Controls", "Tools"),
            new("F2", "Open View Config", "Tools"),
            new("F3", "Open About Box", "Tools"),
            new("Ctrl + Shift + S / F4", "Save all open Archive Editors", "Tools"),
            new("F5", "Launch game", "Tools"),
            new("F6", "Open PS2 ISO Build/Run tool", "Tools"),
            new("Ctrl + Shift + H", "Close all forms except Archive Editors", "Tools")
        };
    }
}

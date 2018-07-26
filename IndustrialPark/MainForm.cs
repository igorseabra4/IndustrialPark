using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            new SharpRenderer(renderPanel);
        }
        
        public void SetToolStripStatusLabel(string Text)
        {
            toolStripStatusLabel1.Text = Text;
        }

        private bool mouseMode = false;
        private System.Drawing.Point MouseCenter = new System.Drawing.Point();
        private MouseEventArgs oldMousePosition;

        private bool loopNotStarted = true;

        private void MouseMoveControl(object sender, MouseEventArgs e)
        {
            if (mouseMode)
            {
                SharpRenderer.Camera.AddYaw(MathUtil.DegreesToRadians(Cursor.Position.X - MouseCenter.X) / 4);
                SharpRenderer.Camera.AddPitch(MathUtil.DegreesToRadians(Cursor.Position.Y - MouseCenter.Y) / 4);

                Cursor.Position = MouseCenter;
            }
            else
            {
                if (e.Button == MouseButtons.Middle)
                {
                    SharpRenderer.Camera.AddYaw(MathUtil.DegreesToRadians(e.X - oldMousePosition.X));
                    SharpRenderer.Camera.AddPitch(MathUtil.DegreesToRadians(e.Y - oldMousePosition.Y));
                }
                if (e.Button == MouseButtons.Right)
                {
                    SharpRenderer.Camera.AddPositionSideways(e.X - oldMousePosition.X);
                    SharpRenderer.Camera.AddPositionUp(e.Y - oldMousePosition.Y);
                }
            }

            SharpRenderer.Camera.AddPositionForward(e.Delta / 24);
            oldMousePosition = e;

            if (loopNotStarted)
            {
                loopNotStarted = false;
                SharpRenderer.RunMainLoop(renderPanel);
            }
        }

        private void MouseModeToggle()
        {
            mouseMode = !mouseMode;
        }

        private void ResetMouseCenter(object sender, EventArgs e)
        {
            MouseCenter = renderPanel.PointToScreen(new System.Drawing.Point(renderPanel.Width / 2, renderPanel.Height / 2));
        }

        private HashSet<Keys> PressedKeys = new HashSet<Keys>();

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PressedKeys.Contains(e.KeyCode))
                PressedKeys.Add(e.KeyCode);

            if (e.KeyCode == Keys.Z)
                MouseModeToggle();
            else if (e.KeyCode == Keys.Q)
                SharpRenderer.Camera.IncreaseCameraSpeed(-1);
            else if (e.KeyCode == Keys.E)
                SharpRenderer.Camera.IncreaseCameraSpeed(1);
            else if (e.KeyCode == Keys.C)
                ToggleCulling();
            else if (e.KeyCode == Keys.F)
                ToggleWireFrame();
            else if (e.KeyCode == Keys.X)
                ToggleLevelModel();
            else if (e.KeyCode == Keys.G)
                ToggleObjects();

            if (e.KeyCode == Keys.F1)
                Program.viewConfig.Show();
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKeys.Remove(e.KeyCode);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            PressedKeys.Clear();
        }

        public void KeyboardController()
        {
            if (PressedKeys.Contains(Keys.A) & PressedKeys.Contains(Keys.ControlKey))
                SharpRenderer.Camera.AddYaw(-0.05f);
            else if (PressedKeys.Contains(Keys.A))
                SharpRenderer.Camera.AddPositionSideways(0.25f);

            if (PressedKeys.Contains(Keys.D) & PressedKeys.Contains(Keys.ControlKey))
                SharpRenderer.Camera.AddYaw(0.05f);
            else if (PressedKeys.Contains(Keys.D))
                SharpRenderer.Camera.AddPositionSideways(-0.25f);

            if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ControlKey))
                SharpRenderer.Camera.AddPitch(-0.05f);
            else if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ShiftKey))
                SharpRenderer.Camera.AddPositionUp(0.25f);
            else if (PressedKeys.Contains(Keys.W))
                SharpRenderer.Camera.AddPositionForward(0.25f);

            if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ControlKey))
                SharpRenderer.Camera.AddPitch(0.05f);
            else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ShiftKey))
                SharpRenderer.Camera.AddPositionUp(-0.25f);
            else if (PressedKeys.Contains(Keys.S))
                SharpRenderer.Camera.AddPositionForward(-0.25f);

            if (PressedKeys.Contains(Keys.R))
                SharpRenderer.Camera.Reset();
        }

        public List<ArchiveEditor> archiveEditors = new List<ArchiveEditor>();

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ArchiveEditor temp = new ArchiveEditor();
            archiveEditors.Add(temp);
            temp.Show();

            ToolStripMenuItem tempMenuItem = new ToolStripMenuItem("Empty");
            tempMenuItem.Click += new EventHandler(ToolStripClick);

            archiveEditorToolStripMenuItem.DropDownItems.Add(tempMenuItem);
        }

        public void ToolStripClick(object sender, EventArgs e)
        {
            archiveEditors[archiveEditorToolStripMenuItem.DropDownItems.IndexOf(sender as ToolStripItem) - 2].Show();
        }

        public void SetToolStripItemName(ArchiveEditor sender, string newName)
        {
            archiveEditorToolStripMenuItem.DropDownItems[archiveEditors.IndexOf(sender) + 2].Text = newName;
        }

        public void CloseAssetEditor(ArchiveEditor sender)
        {
            int index = archiveEditors.IndexOf(sender);
            archiveEditorToolStripMenuItem.DropDownItems.RemoveAt(index + 2);
            archiveEditors.RemoveAt(index);
        }

        private void noCullingCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleCulling();
        }

        public void ToggleCulling()
        {
            noCullingCToolStripMenuItem.Checked = !noCullingCToolStripMenuItem.Checked;
            if (noCullingCToolStripMenuItem.Checked)
                SharpRenderer.device.SetNormalCullMode(CullMode.None);
            else
                SharpRenderer.device.SetNormalCullMode(CullMode.Back);
        }

        private void wireframeFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleWireFrame();
        }

        public void ToggleWireFrame()
        {
            wireframeFToolStripMenuItem.Checked = !wireframeFToolStripMenuItem.Checked;
            if (wireframeFToolStripMenuItem.Checked)
            {
                SharpRenderer.device.SetNormalFillMode(FillMode.Wireframe);
            }
            else
            {
                SharpRenderer.device.SetNormalFillMode(FillMode.Solid);
            }
        }

        private void BackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                SharpRenderer.backgroundColor = new Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
        }
        
        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Program.viewConfig.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void viewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.viewConfig.Show();
        }

        private void levelModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleLevelModel();
        }

        private void ToggleLevelModel()
        {
            levelModelToolStripMenuItem.Checked = !levelModelToolStripMenuItem.Checked;
            SharpRenderer.SetLevelModel(levelModelToolStripMenuItem.Checked);
        }

        private void objectModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleObjects();
        }

        private void ToggleObjects()
        {
            objectModelsToolStripMenuItem.Checked = !objectModelsToolStripMenuItem.Checked;
            SharpRenderer.SetObjects(objectModelsToolStripMenuItem.Checked);
        }

        private void renderPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ScreenClicked(new Rectangle(
                    renderPanel.ClientRectangle.X,
                    renderPanel.ClientRectangle.Y,
                    renderPanel.ClientRectangle.Width,
                    renderPanel.ClientRectangle.Height), e.X, e.Y);
            }
        }

        public void ScreenClicked(Rectangle viewRectangle, int X, int Y)
        {
            Ray ray = Ray.GetPickRay(X, Y, new Viewport(viewRectangle), SharpRenderer.viewProjection);
            foreach (ArchiveEditor ae in archiveEditors)
            {
                ae.ScreenClicked(ray);
            }
        }
    }
}

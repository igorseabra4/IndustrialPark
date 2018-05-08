using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
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
        
        private void openHIPHOPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HipHopFunctions.openFilePair();
        }

        private void exportTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HipHopFunctions.ExportAllTextures();
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
    }
}

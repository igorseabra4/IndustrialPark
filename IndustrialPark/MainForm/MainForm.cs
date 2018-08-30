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

            renderer = new SharpRenderer(renderPanel);
        }

        public void SetToolStripStatusLabel(string Text)
        {
            toolStripStatusLabel1.Text = Text;
        }

        public SharpRenderer renderer;

        private bool mouseMode = false;
        private System.Drawing.Point MouseCenter = new System.Drawing.Point();
        private MouseEventArgs oldMousePosition = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        private bool loopNotStarted = true;

        private void MouseMoveControl(object sender, MouseEventArgs e)
        {
            if (mouseMode)
            {
                renderer.Camera.AddYaw(MathUtil.DegreesToRadians(Cursor.Position.X - MouseCenter.X) / 4);
                renderer.Camera.AddPitch(MathUtil.DegreesToRadians(Cursor.Position.Y - MouseCenter.Y) / 4);

                Cursor.Position = MouseCenter;
            }
            else
            {
                int deltaX = e.X - oldMousePosition.X;
                int deltaY = e.Y - oldMousePosition.Y;

                if (e.Button == MouseButtons.Middle)
                {
                    renderer.Camera.AddYaw(MathUtil.DegreesToRadians(e.X - oldMousePosition.X));
                    renderer.Camera.AddPitch(MathUtil.DegreesToRadians(e.Y - oldMousePosition.Y));
                }
                if (e.Button == MouseButtons.Right)
                {
                    renderer.Camera.AddPositionSideways(e.X - oldMousePosition.X);
                    renderer.Camera.AddPositionUp(e.Y - oldMousePosition.Y);
                }

                foreach (ArchiveEditor ae in archiveEditors)
                {
                    ae.MouseMoveX(renderer.Camera, deltaX);
                    ae.MouseMoveY(renderer.Camera, deltaY);
                }
            }

            renderer.Camera.AddPositionForward(e.Delta / 24);
            oldMousePosition = e;

            if (loopNotStarted)
            {
                loopNotStarted = false;
                renderer.RunMainLoop(renderPanel);
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
                renderer.Camera.IncreaseCameraSpeed(-1);
            else if (e.KeyCode == Keys.E)
                renderer.Camera.IncreaseCameraSpeed(1);
            else if (e.KeyCode == Keys.C)
                ToggleCulling();
            else if (e.KeyCode == Keys.F)
                ToggleWireFrame();
            else if (e.KeyCode == Keys.X)
                ToggleLevelModel();
            else if (e.KeyCode == Keys.G)
                ToggleObjects();

            if (e.KeyCode == Keys.F1)
                Program.ViewConfig.Show();
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
                renderer.Camera.AddYaw(-0.05f);
            else if (PressedKeys.Contains(Keys.A))
                renderer.Camera.AddPositionSideways(0.25f);

            if (PressedKeys.Contains(Keys.D) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddYaw(0.05f);
            else if (PressedKeys.Contains(Keys.D))
                renderer.Camera.AddPositionSideways(-0.25f);

            if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPitch(-0.05f);
            else if (PressedKeys.Contains(Keys.W) & PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPositionUp(0.25f);
            else if (PressedKeys.Contains(Keys.W))
                renderer.Camera.AddPositionForward(0.25f);

            if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ControlKey))
                renderer.Camera.AddPitch(0.05f);
            else if (PressedKeys.Contains(Keys.S) & PressedKeys.Contains(Keys.ShiftKey))
                renderer.Camera.AddPositionUp(-0.25f);
            else if (PressedKeys.Contains(Keys.S))
                renderer.Camera.AddPositionForward(-0.25f);

            if (PressedKeys.Contains(Keys.R))
                renderer.Camera.Reset();
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
                renderer.device.SetNormalCullMode(CullMode.None);
            else
                renderer.device.SetNormalCullMode(CullMode.Back);
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
                renderer.device.SetNormalFillMode(FillMode.Wireframe);
            }
            else
            {
                renderer.device.SetNormalFillMode(FillMode.Solid);
            }
        }

        private void BackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
                renderer.backgroundColor = new Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
        }
        
        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            Program.ViewConfig.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void viewConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ViewConfig.Show();
        }

        private void levelModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleLevelModel();
        }

        private void ToggleLevelModel()
        {
            levelModelToolStripMenuItem.Checked = !levelModelToolStripMenuItem.Checked;
            renderer.SetLevelModel(levelModelToolStripMenuItem.Checked);
        }

        private void objectModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleObjects();
        }

        private void ToggleObjects()
        {
            objectModelsToolStripMenuItem.Checked = !objectModelsToolStripMenuItem.Checked;
            renderer.SetObjects(objectModelsToolStripMenuItem.Checked);
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

        private void renderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ScreenClicked(new Rectangle(
                    renderPanel.ClientRectangle.X,
                    renderPanel.ClientRectangle.Y,
                    renderPanel.ClientRectangle.Width,
                    renderPanel.ClientRectangle.Height), e.X, e.Y, true);
            }
        }

        public void ScreenClicked(Rectangle viewRectangle, int X, int Y, bool isMouseDown = false)
        {
            Ray ray = Ray.GetPickRay(X, Y, new Viewport(viewRectangle), renderer.viewProjection);
            foreach (ArchiveEditor ae in archiveEditors)
                ae.ScreenClicked(ray, isMouseDown);
        }

        private void renderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.ScreenUnclicked();
        }

        private void renderPanel_MouseLeave(object sender, EventArgs e)
        {
            foreach (ArchiveEditor ae in archiveEditors)
                ae.ScreenUnclicked();
        }

        private void addTextureFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFile = new FolderBrowserDialog();
            if (openFile.ShowDialog() == DialogResult.OK)
                TextureManager.LoadTexturesFromFolder(openFile.SelectedPath);
        }

        private void clearTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextureManager.ClearTextures();
        }

        private void assetsWithModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            assetsWithModelToolStripMenuItem.Checked = !assetsWithModelToolStripMenuItem.Checked;
            PlaceableAsset.dontRender = !assetsWithModelToolStripMenuItem.Checked;
        }

        private void mVPTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mVPTToolStripMenuItem.Checked = !mVPTToolStripMenuItem.Checked;
            AssetMVPT.dontRender = !mVPTToolStripMenuItem.Checked;
        }

        private void pKUPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pKUPToolStripMenuItem.Checked = !pKUPToolStripMenuItem.Checked;
            AssetPKUP.dontRender = !pKUPToolStripMenuItem.Checked;
        }

        private void tRIGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tRIGToolStripMenuItem.Checked = !tRIGToolStripMenuItem.Checked;
            AssetTRIG.dontRender = !tRIGToolStripMenuItem.Checked;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalFlyEditor : Form, IInternalEditor
    {
        public InternalFlyEditor(AssetFLY asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            Text = $"[{asset.assetType}] {asset}";
            UpdateListbox();
        }

        public void RefreshPropertyGrid()
        {
            listBoxFlyEntries.Refresh();
            propertyGridSpecific.Refresh();
        }

        private readonly ArchiveEditorFunctions archive;

        private void InternalDynaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recording || playing)
            {
                e.Cancel = true;
            }
            else
            {
                Program.MainForm.renderer.StopFly();
                archive.CloseInternalEditor(this);
            }
        }

        public uint GetAssetID()
        {
            return asset.assetID;
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private readonly AssetFLY asset;

        private void UpdateListbox()
        {
            listBoxFlyEntries.Items.Clear();
            foreach (FlyFrame entry in asset.Frames)
            {
                listBoxFlyEntries.Items.Add(entry);
                maxFrame = entry.FrameNumer > maxFrame ? entry.FrameNumer : maxFrame;
            }
        }

        private void UpdateAssetEntries()
        {
            int selectedIndex = listBoxFlyEntries.SelectedIndex;

            List<FlyFrame> entries = new List<FlyFrame>();
            var count = 0;
            maxFrame = 0;
            foreach (FlyFrame entry in listBoxFlyEntries.Items)
            {
                entry.FrameNumer = count++;
                entries.Add(entry);
                if (entry.FrameNumer > maxFrame)
                    maxFrame = entry.FrameNumer;
            }
            asset.Frames = entries.ToArray();
            archive.UnsavedChanges = true;

            UpdateListbox();

            try
            {
                listBoxFlyEntries.SelectedIndex = selectedIndex;
            }
            catch
            {
                try
                {
                    listBoxFlyEntries.SelectedIndex = selectedIndex - 1;
                }
                catch { }
            }
        }

        private void listBoxFlyEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridSpecific.SelectedObject = (FlyFrame)listBoxFlyEntries.SelectedItem;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var entry = new FlyFrame() { FrameNumer = listBoxFlyEntries.Items.Count };
            SetViewToFly(entry);
            listBoxFlyEntries.Items.Add(entry);
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxFlyEntries.SelectedIndex != -1)
            {
                listBoxFlyEntries.Items.RemoveAt(listBoxFlyEntries.SelectedIndex);
                UpdateAssetEntries();
            }
        }

        private void propertyGridSpecific_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            RefreshPropertyGrid();
        }

        private void buttonGetView_Click(object sender, EventArgs e)
        {
            if (listBoxFlyEntries.SelectedIndex != -1)
                SetViewToFly((FlyFrame)propertyGridSpecific.SelectedObject);
            RefreshPropertyGrid();
        }

        private void SetViewToFly(FlyFrame entry)
        {
            entry.CameraPosition = Program.MainForm.renderer.Camera.Position;
            entry.CameraNormalizedRight = Program.MainForm.renderer.Camera.Right;
            entry.CameraNormalizedUp = Program.MainForm.renderer.Camera.Up;
            entry.CameraNormalizedBackward = -Program.MainForm.renderer.Camera.Forward;
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (propertyGridSpecific.SelectedObject is FlyFrame flyEntry)
                Program.MainForm.renderer.Camera.SetPositionFlyEntry(flyEntry);
        }

        private bool playing = false;

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (playing)
            {
                Stop();

                buttonPlay.Text = "Preview";

                buttonAdd.Enabled = true;
                buttonRemove.Enabled = true;
                buttonRecord.Enabled = true;
                buttonGetPos.Enabled = true;
                buttonView.Enabled = true;
            }
            else
            {
                playing = true;

                buttonPlay.Text = "Stop Preview";

                buttonAdd.Enabled = false;
                buttonRemove.Enabled = false;
                buttonRecord.Enabled = false;
                buttonGetPos.Enabled = false;
                buttonView.Enabled = false;

                Program.MainForm.renderer.PlayFly(this);
            }
        }

        private bool recording = false;

        private void buttonRecord_Click(object sender, EventArgs e)
        {
            if (recording)
            {
                Stop();

                buttonRecord.Text = "Start Recording";
                UpdateAssetEntries();

                buttonRemove.Enabled = true;
                buttonGetPos.Enabled = true;
                buttonPlay.Enabled = true;
                buttonView.Enabled = true;
            }
            else
            {
                buttonRemove.Enabled = false;
                buttonGetPos.Enabled = false;
                buttonPlay.Enabled = false;
                buttonView.Enabled = false;

                recording = true;
                Program.MainForm.renderer.RecordFly(this);
                buttonRecord.Text = "Stop Recording";
            }
        }

        private void Stop()
        {
            playing = false;
            recording = false;

            currentFrame = 0;
            labelFrame.Text = "";
            Program.MainForm.renderer.StopFly();
        }

        private int currentFrame = 0;
        private int maxFrame = 0;
        private bool switcher = false;

        public void Play()
        {
            foreach (FlyFrame entry in listBoxFlyEntries.Items)
            {
                if (entry.FrameNumer == currentFrame)
                {
                    Program.MainForm.renderer.Camera.SetPositionFlyEntry(entry);
                    break;
                }
            }

            switcher = !switcher;
            if (switcher)
            {
                currentFrame++;
                if (currentFrame > maxFrame)
                    currentFrame = 0;
                labelFrame.Text = "Frame: " + currentFrame;
            }
        }

        public void Record()
        {
            var newFly = new FlyFrame
            {
                FrameNumer = listBoxFlyEntries.Items.Count,
                ApertureX = 0.98f,
                ApertureY = 0.735f,
                Focal = 26.69057f
            };
            SetViewToFly(newFly);

            var lastIndex = listBoxFlyEntries.Items.Count - 1;

            if (lastIndex < 0 || !((FlyFrame)listBoxFlyEntries.Items[lastIndex]).NearlySimilar(newFly))
            {
                switcher = !switcher;
                if (switcher)
                    listBoxFlyEntries.Items.Add(newFly);
                labelFrame.Text = "Frame: " + newFly.FrameNumer;
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Process.Start(AboutBox.WikiLink + asset.assetType.ToString());
        }
    }
}
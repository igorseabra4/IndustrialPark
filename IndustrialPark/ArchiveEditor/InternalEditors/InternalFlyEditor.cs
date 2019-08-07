using System;
using System.Collections.Generic;
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

            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
            UpdateListbox();
        }

        private ArchiveEditorFunctions archive;

        private void InternalDynaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.MainForm.renderer.StopFly();
            archive.CloseInternalEditor(this);
        }

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private AssetFLY asset;

        private void UpdateListbox()
        {
            listBoxFlyEntries.Items.Clear();
            foreach (EntryFLY entry in asset.FLY_Entries)
            {
                listBoxFlyEntries.Items.Add(entry);
                maxFrame = entry.FrameNumer > maxFrame ? entry.FrameNumer : maxFrame;
            }
        }

        private void UpdateAssetEntries()
        {
            int selectedIndex = listBoxFlyEntries.SelectedIndex;

            List<EntryFLY> entries = new List<EntryFLY>();
            foreach (EntryFLY entry in listBoxFlyEntries.Items)
            {
                entries.Add(entry);
                maxFrame = entry.FrameNumer > maxFrame ? entry.FrameNumer : maxFrame;
            }
            asset.FLY_Entries = entries.ToArray();
            archive.UnsavedChanges = true;

            UpdateListbox();

            try
            {
                listBoxFlyEntries.SelectedIndex = selectedIndex;
            }
            catch {
                try
                {
                    listBoxFlyEntries.SelectedIndex = selectedIndex - 1;
                }
                catch { }
            }
        }

        private void listBoxFlyEntries_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridSpecific.SelectedObject = (EntryFLY)listBoxFlyEntries.SelectedItem;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            listBoxFlyEntries.Items.Add(new EntryFLY() { FrameNumer = listBoxFlyEntries.Items.Count });
            UpdateAssetEntries();
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
            UpdateAssetEntries();
        }

        public void SetHideHelp(bool _)
        {
        }

        private void buttonGetPos_Click(object sender, EventArgs e)
        {
            ((EntryFLY)propertyGridSpecific.SelectedObject).CameraPosition = Program.MainForm.renderer.Camera.Position;
            UpdateAssetEntries();
        }

        private void buttonGetDir_Click(object sender, EventArgs e)
        {
            ((EntryFLY)propertyGridSpecific.SelectedObject).CameraNormalizedLeft = -Program.MainForm.renderer.Camera.Right;
            ((EntryFLY)propertyGridSpecific.SelectedObject).CameraNormalizedUp = Program.MainForm.renderer.Camera.Up;
            ((EntryFLY)propertyGridSpecific.SelectedObject).CameraNormalizedBackward = -Program.MainForm.renderer.Camera.Forward;
            UpdateAssetEntries();
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (propertyGridSpecific.SelectedObject is EntryFLY flyEntry)
                Program.MainForm.renderer.Camera.SetPositionFlyEntry(flyEntry);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            Program.MainForm.renderer.PlayFly(this);
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            currentFrame = 0;
            labelFrame.Text = "";
            Program.MainForm.renderer.StopFly();
        }

        private int currentFrame = 0;
        private int maxFrame = 0;
        private bool switcher = false;

        public void Play()
        {
            foreach (EntryFLY entry in listBoxFlyEntries.Items)
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
    }
}
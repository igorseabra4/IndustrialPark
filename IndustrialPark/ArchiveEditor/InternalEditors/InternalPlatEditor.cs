using SharpDX;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalPlatEditor : Form, IInternalEditor
    {
        public InternalPlatEditor(AssetPLAT asset, ArchiveEditorFunctions archive, bool hideHelp)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";

            asset.PlatSpecificChosen += UpdatePlatSpecifics;

            UpdatePlatSpecifics();

            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGrid_PlatSpecific.HelpVisible = !hideHelp;
            propertyGrid_Motion.HelpVisible = !hideHelp;
        }

        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            asset.PlatSpecificChosen -= UpdatePlatSpecifics;
            archive.CloseInternalEditor(this);
        }

        private AssetPLAT asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            UpdatePlatSpecifics();
        }

        private void UpdatePlatSpecifics()
        {
            propertyGrid_PlatSpecific.SelectedObject = asset.PlatSpecific;
            propertyGrid_Motion.SelectedObject = asset.Motion;
        }

        private void propertyGrid_PlatSpecific_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            asset.PlatSpecific = (PlatSpecific_Generic)propertyGrid_PlatSpecific.SelectedObject;
        }

        private void propertyGrid_Motion_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
            asset.Motion = (Motion)propertyGrid_Motion.SelectedObject;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        public void SetHideHelp(bool hideHelp)
        {
            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGrid_PlatSpecific.HelpVisible = !hideHelp;
            propertyGrid_Motion.HelpVisible = !hideHelp;
        }
    }
}

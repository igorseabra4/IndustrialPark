using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalWireEditor : Form, IInternalEditor
    {
        public InternalWireEditor(AssetWIRE asset, ArchiveEditorFunctions archive, bool hideHelp)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            
            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetWIRE asset;
        private ArchiveEditorFunctions archive;
        
        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                asset.ToObj(saveFile.FileName);
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                asset.FromObj(openFile.FileName);
                archive.UnsavedChanges = true;
            }
        }

        public void SetHideHelp(bool _)
        {
        }
    }
}

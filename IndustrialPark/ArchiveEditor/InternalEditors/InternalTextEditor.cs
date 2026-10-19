using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalTextEditor : InternalEditor
    {
        public InternalTextEditor(AssetTEXT asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            Text = $"[{this.asset.assetType}] {this.asset}";
            richTextBoxAssetText.Text = this.asset.Text;
        }

        public override bool CanBeDocked => true;

        public override void RefreshPropertyGrid()
        {
            richTextBoxAssetText.Refresh();
        }

        private void InternalTextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetTEXT asset;

        private void richTextBoxAssetText_TextChanged(object sender, EventArgs e)
        {
            asset.Text = richTextBoxAssetText.Text;
            archive.UnsavedChanges = true;
            updateListView(asset);
        }

        public override uint GetAssetID()
        {
            return asset.assetID;
        }

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            ArchiveEditorFunctions.OpenWikiPage(asset);
        }

        private void editHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ae = archive.GetArchiveEditor();
            if (ae.EditHeader(asset.assetID, out uint newAssetID))
                ae.OpenInternalEditor(newAssetID);
        }

        private void copyAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.CopyAssetsToClipboard(asset.assetID);
        }

        private void deleteAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"Are you sure you want to delete asset [{asset.assetID:X8}] {asset.assetName} ({AssetTypeContainer.AssetTypeToString(asset.assetType)})?", "Confirm asset removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                var actions = new List<IReversibleAction>();
                archive.GetArchiveEditor().RemoveAssets([asset.assetID], actions);
                Program.UndoBuffer.AddAction(actions);
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalTextEditor : Form, IInternalEditor
    {
        public InternalTextEditor(AssetTEXT asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            Text = $"[{this.asset.assetType}] {this.asset}";
            richTextBoxAssetText.Text = this.asset.Text;
        }

        public void RefreshPropertyGrid()
        {
            richTextBoxAssetText.Refresh();
        }

        private void InternalTextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetTEXT asset;
        private ArchiveEditorFunctions archive;
        private readonly Action<Asset> updateListView;

        private void richTextBoxAssetText_TextChanged(object sender, EventArgs e)
        {
            asset.Text = richTextBoxAssetText.Text;
            archive.UnsavedChanges = true;
            updateListView(asset);
        }

        public uint GetAssetID()
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
    }
}

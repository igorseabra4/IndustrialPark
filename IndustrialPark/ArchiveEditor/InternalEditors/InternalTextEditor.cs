using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalTextEditor : Form, IInternalEditor
    {
        public InternalTextEditor(AssetTEXT asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            Text = $"[{this.asset.AHDR.assetType}] {this.asset}";
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

        private void richTextBoxAssetText_TextChanged(object sender, System.EventArgs e)
        {
            asset.Text = richTextBoxAssetText.Text;
            archive.UnsavedChanges = true;
        }

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }
        public void SetHideHelp(bool _)
        {
        }
    }
}

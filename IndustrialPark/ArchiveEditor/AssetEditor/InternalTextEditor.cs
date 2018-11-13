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

            labelAssetName.Text = $"[{this.asset.AHDR.assetType.ToString()}] {this.asset.ToString()}";
            richTextBoxAssetText.Text = this.asset.Text;
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
        }

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }
    }
}

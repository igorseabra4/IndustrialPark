using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalTextEditor : Form, IInternalEditor
    {
        public InternalTextEditor(AssetTEXT asset, ArchiveEditor archiveEditor)
        {
            InitializeComponent();
            TopMost = true;

            assetTEXT = asset;
            this.archiveEditor = archiveEditor;

            labelAssetName.Text = $"[{assetTEXT.AHDR.assetType.ToString()}] {assetTEXT.ToString()}";
            richTextBoxAssetText.Text = assetTEXT.Text;
        }

        private void InternalTextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archiveEditor.RemoveInternalEditor(this);
        }

        private AssetTEXT assetTEXT;
        private ArchiveEditor archiveEditor;

        private void richTextBoxAssetText_TextChanged(object sender, System.EventArgs e)
        {
            assetTEXT.Text = richTextBoxAssetText.Text;
        }

        public uint GetAssetID()
        {
            return assetTEXT.AHDR.assetID;
        }
    }
}

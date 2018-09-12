using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalTextEditor : Form
    {
        public InternalTextEditor(AssetTEXT asset)
        {
            InitializeComponent();
            TopMost = true;

            assetTEXT = asset;

            labelAssetName.Text = $"[{assetTEXT.AHDR.assetType.ToString()}] {assetTEXT.ToString()}";
            richTextBoxAssetText.Text = assetTEXT.Text;
        }

        private AssetTEXT assetTEXT;

        private void richTextBoxAssetText_TextChanged(object sender, System.EventArgs e)
        {
            assetTEXT.Text = richTextBoxAssetText.Text;
        }
    }
}

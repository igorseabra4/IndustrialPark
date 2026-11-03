using System;

namespace IndustrialPark
{
    public partial class InternalTextEditor : InternalEditor
    {
        public InternalTextEditor(AssetTEXT asset, ArchiveEditor archive) : base(asset, archive)
        {
            InitializeComponent();
            richTextBoxAssetText.Text = this.asset.Text;

            if (!EnableAssetToolButtons)
            {
                buttonFindCallers.Visible = false;
                buttonHelp.Visible = false;
                tableLayoutPanel1.RowCount = 1;
            }
        }

        public override void RefreshPropertyGrid()
        {
            richTextBoxAssetText.Refresh();
        }

        private new AssetTEXT asset => (AssetTEXT)base.asset;

        private void richTextBoxAssetText_TextChanged(object sender, EventArgs e)
        {
            asset.Text = richTextBoxAssetText.Text;
        }
    }
}

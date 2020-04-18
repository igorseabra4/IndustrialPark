using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class InternalDynaEditor : Form, IInternalEditor
    {
        public InternalDynaEditor(AssetDYNA asset, ArchiveEditorFunctions archive, bool hideHelp)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            propertyGridAsset.SelectedObject = asset;
            propertyGridDynaType.SelectedObject = asset.DynaBase;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";

            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGridDynaType.HelpVisible = !hideHelp;
        }

        private void InternalDynaEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetDYNA asset;
        private ArchiveEditorFunctions archive;

        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            propertyGridDynaType.SelectedObject = asset.DynaBase;
            archive.UnsavedChanges = true;
        }

        private void propertyGridDynaType_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            asset.DynaBase = (DynaBase)propertyGridDynaType.SelectedObject;
            archive.UnsavedChanges = true;
        }

        public void RefreshPropertyGrid()
        {
            propertyGridDynaType.Refresh();
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        public void SetHideHelp(bool hideHelp)
        {
            propertyGridAsset.HelpVisible = !hideHelp;
            propertyGridDynaType.HelpVisible = !hideHelp;
        }
    }
}

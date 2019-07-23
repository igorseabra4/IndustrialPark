using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ChoosePlatformDialog : Form
    {
        public ChoosePlatformDialog()
        {
            InitializeComponent();

            TopMost = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAssetTypes.SelectedIndex == 0)
                ArchiveEditorFunctions.defaultScoobyPlatform = Platform.GameCube;
            else if (comboBoxAssetTypes.SelectedIndex == 1)
                ArchiveEditorFunctions.defaultScoobyPlatform = Platform.Xbox;
            else if (comboBoxAssetTypes.SelectedIndex == 2)
                ArchiveEditorFunctions.defaultScoobyPlatform = Platform.PS2;

            buttonOK.Enabled = true;
        }
    }
}

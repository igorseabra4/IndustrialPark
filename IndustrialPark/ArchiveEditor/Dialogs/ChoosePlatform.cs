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
            comboBoxAssetTypes.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAssetTypes.SelectedIndex == 0)
                platform = Platform.GameCube;
            else if (comboBoxAssetTypes.SelectedIndex == 1)
                platform = Platform.Xbox;
            else if (comboBoxAssetTypes.SelectedIndex == 2)
                platform = Platform.PS2;

            buttonOK.Enabled = true;
        }

        private Platform platform = Platform.GameCube;

        public static Platform GetPlatform()
        {
            ChoosePlatformDialog a = new ChoosePlatformDialog();
            a.ShowDialog();
            return a.platform;
        }
    }
}

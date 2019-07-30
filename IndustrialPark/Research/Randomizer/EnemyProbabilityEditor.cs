using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class RandomizerSettingsEditor : Form
    {
        public RandomizerSettingsEditor(RandomizerSettings asset)
        {
            InitializeComponent();
            propertyGridAsset.SelectedObject = asset;
        }

        private void ButtonHelp_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

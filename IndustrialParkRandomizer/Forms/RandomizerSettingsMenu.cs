using System;
using System.Windows.Forms;

namespace IndustrialPark.Randomizer
{
    public partial class RandomizerSettingsMenu : Form
    {
        public RandomizerSettingsMenu(RandomizerSettings asset)
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

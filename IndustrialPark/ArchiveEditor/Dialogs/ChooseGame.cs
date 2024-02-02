using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ChooseGame : Form
    {
        public ChooseGame()
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
                game = Game.Incredibles;
            else if (comboBoxAssetTypes.SelectedIndex == 1)
                game = Game.ROTU;
            else if (comboBoxAssetTypes.SelectedIndex == 2)
                game = Game.RatProto;

            buttonOK.Enabled = true;
        }

        private Game game = Game.Incredibles;

        public static Game GetGame()
        {
            using (var dialog = new ChooseGame())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    return dialog.game;
                return Game.Unknown;
            }
        }
    }
}

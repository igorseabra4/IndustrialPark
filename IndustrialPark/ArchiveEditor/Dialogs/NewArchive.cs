using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class NewArchive : Form
    {
        public static (Platform platform, Game game, DateTime fileDate, string dateString, bool addDefaultAssets, bool autoLayers)? GetNewArchive()
        {
            NewArchive newArchive = new NewArchive();
            if (newArchive.ShowDialog() == DialogResult.OK)
                return (
                    newArchive.platform,
                    newArchive.game,
                    newArchive.dateTimePicker1.Value,
                    newArchive.textBoxPCRT.Text,
                    newArchive.checkBoxDefaultAssets.Checked,
                    newArchive.checkBoxAutoLayers.Checked
                );
            return null;
        }

        public static (Platform newPlatform, Game newGame, DateTime fileDate, string dateString)? GetExistingArchive(Platform previousPlatform, Game previousGame, int previousDate, string previousDateString)
        {
            NewArchive newArchive = new NewArchive(previousPlatform, previousGame, previousDate, previousDateString);
            if (newArchive.ShowDialog() == DialogResult.OK)
                return (
                    newArchive.platform,
                    newArchive.game,
                    newArchive.dateTimePicker1.Value,
                    newArchive.textBoxPCRT.Text
                );
            return null;
        }

        private NewArchive()
        {
            InitializeComponent();

            comboBoxGame.Items.Add("Scooby-Doo: Night Of 100 Frights");
            comboBoxGame.Items.Add("Spongebob Squarepants: Battle For Bikini Bottom");
            comboBoxGame.Items.Add("The SpongeBob SquarePants Movie Game/The Incredibles");
            comboBoxGame.Items.Add("Rise of the Underminer");
            comboBoxGame.Items.Add("Ratatouille Prototype");

            comboBoxPlatform.Items.Add("GameCube");
            comboBoxPlatform.Items.Add("Xbox / PC");
            comboBoxPlatform.Items.Add("Playstation 2");

            dateTimePicker1.MinDate = DateTime.MinValue;
            dateTimePicker1.MaxDate = DateTime.MaxValue;

            dateTimePicker1.Value = DateTime.Now;
        }

        private NewArchive(Platform previousPlatform, Game previousGame, int previousDate, string previousDateString) : this()
        {
            checkBoxDefaultAssets.Visible = false;
            checkBoxAutoLayers.Visible = false;

            Text = "Convert Archive";
            buttonOK.Text = "Convert";

            switch (previousGame)
            {
                case Game.Scooby:
                    comboBoxGame.SelectedIndex = 0;
                    break;
                case Game.BFBB:
                    comboBoxGame.SelectedIndex = 1;
                    break;
                case Game.Incredibles:
                    comboBoxGame.SelectedIndex = 2;
                    break;
                case Game.ROTU:
                    comboBoxGame.SelectedIndex = 3;
                    break;
                case Game.RatProto:
                    comboBoxGame.SelectedIndex = 4;
                    break;
            }

            switch (previousPlatform)
            {
                case Platform.PS2:
                    comboBoxPlatform.SelectedIndex = 2;
                    break;
                case Platform.GameCube:
                    comboBoxPlatform.SelectedIndex = 0;
                    break;
                case Platform.Xbox:
                    comboBoxPlatform.SelectedIndex = 1;
                    break;
            }

            try
            {
                dateTimePicker1.Value = DateTimeOffset.FromUnixTimeSeconds(previousDate).UtcDateTime;
            }
            catch
            {
                dateTimePicker1.Value = new DateTime(1970, 1, 1).AddSeconds(previousDate);
            }

            textBoxPCRT.Text = previousDateString;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private Game game;
        private Platform platform;

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGame.SelectedIndex == 0)
                game = Game.Scooby;
            else if (comboBoxGame.SelectedIndex == 1)
                game = Game.BFBB;
            else if (comboBoxGame.SelectedIndex == 2)
                game = Game.Incredibles;
            else if (comboBoxGame.SelectedIndex == 3)
                game = Game.ROTU;
            else if (comboBoxGame.SelectedIndex == 4)
                game = Game.RatProto;

            if (comboBoxPlatform.SelectedIndex == 0)
                platform = Platform.GameCube;
            else if (comboBoxPlatform.SelectedIndex == 1)
                platform = Platform.Xbox;
            else if (comboBoxPlatform.SelectedIndex == 2)
                platform = Platform.PS2;

            buttonOK.Enabled = comboBoxGame.SelectedIndex != -1 && comboBoxPlatform.SelectedIndex != -1;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBoxPCRT.Text = dateTimePicker1.Value.ToString();
        }
    }
}

using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class NewArchive : Form
    {
        public static (Section_PACK PACK, Platform platform, Game game, bool addDefaultAssets, bool noLayers)? GetNewArchive()
        {
            NewArchive newArchive = new NewArchive();
            newArchive.ShowDialog();

            if (newArchive.result != null)
                return (newArchive.result, newArchive.platform, newArchive.game, newArchive.checkBoxDefaultAssets.Checked, newArchive.checkBoxNoLayers.Checked);
            return null;
        }

        public static (Section_PACK PACK, Platform newPlatform, Game newGame) GetExistingArchive(Platform previousPlatform, Game previousGame, int previousDate, string previousDateString)
        {
            NewArchive newArchive = new NewArchive(previousPlatform, previousGame, previousDate, previousDateString);

            newArchive.ShowDialog();

            if (newArchive.OK)
                return (newArchive.result, newArchive.platform, newArchive.game);
            return (null, 0, 0);
        }

        private NewArchive()
        {
            InitializeComponent();
            TopMost = true;

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
            checkBoxNoLayers.Visible = false;

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
                dateTimePicker1.Value = new DateTime(previousDate);
            }
            catch
            {
                dateTimePicker1.Value = new DateTime(1970, 1, 1).AddSeconds(previousDate);
            }

            textBoxPCRT.Text = previousDateString;
        }

        private bool OK = false;
        private Section_PACK result = null;
        private Game game;
        private Platform platform;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK = true;

            Section_PACK PACK = new Section_PACK();

            PACK.PMOD = new Section_PMOD((int)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            PACK.PCRT = new Section_PCRT((int)((DateTimeOffset)dateTimePicker1.Value).ToUnixTimeSeconds(), textBoxPCRT.Text);
            PACK.PCNT = new Section_PCNT(0, 0, 0, 0, 0);
            PACK.PFLG = new Section_PFLG(46);

            if (game != Game.Scooby)
            {
                PACK.PVER = new Section_PVER(2, 655375, 1);
                PACK.PLAT = new Section_PLAT
                { regionFormat = "NTSC" };
            }

            switch (game)
            {
                case Game.Scooby:
                    PACK.PVER = new Section_PVER(2, 262150, 1);
                    break;
                case Game.BFBB:
                    PACK.PLAT.language = "US Common";
                    PACK.PLAT.targetGame = "Sponge Bob";
                    switch (platform)
                    {
                        case Platform.GameCube:
                            PACK.PFLG = new Section_PFLG(36241454);
                            PACK.PLAT.targetPlatform = "GC";
                            PACK.PLAT.targetPlatformName = "GameCube";
                            break;
                        case Platform.PS2:
                            PACK.PFLG = new Section_PFLG(36438062);
                            PACK.PLAT.targetPlatform = "P2";
                            PACK.PLAT.targetPlatformName = "PlayStation 2";
                            break;
                        case Platform.Xbox:
                            PACK.PFLG = new Section_PFLG(36306990);
                            PACK.PLAT.targetPlatform = "XB";
                            PACK.PLAT.targetPlatformName = "Xbox";
                            break;
                    }
                    break;
                case Game.Incredibles:
                case Game.ROTU:
                case Game.RatProto:
                    PACK.PLAT.language = "US";
                    PACK.PLAT.targetGame = "Incredibles";
                    switch (platform)
                    {
                        case Platform.Xbox:
                            PACK.PLAT.targetPlatform = "BX";
                            break;
                        case Platform.GameCube:
                            PACK.PLAT.targetPlatform = "GC";
                            break;
                        case Platform.PS2:
                            PACK.PLAT.targetPlatform = "PS2";
                            break;
                    }
                    break;
            }

            result = PACK;

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

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

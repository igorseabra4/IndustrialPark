using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class NewArchive : Form
    {
        public static HipSection[] GetNewArchive(out bool OK, out Platform platform, out Game game)
        {
            NewArchive newArchive = new NewArchive();
            newArchive.ShowDialog();
            platform = newArchive.platform;
            game = newArchive.game;
            OK = newArchive.OK;
            return newArchive.result;
        }

        public NewArchive()
        {
            InitializeComponent();
        }

        private void NewArchive_Load(object sender, EventArgs e)
        {
            TopMost = true;

            comboBoxGame.Items.Add("Scooby-Doo: Night Of 100 Frights");
            comboBoxGame.Items.Add("Spongebob Squarepants: Battle For Bikini Bottom");
            comboBoxGame.Items.Add("The Incredibles/Movie Game/Rise of the Underminer");

            comboBoxPlatform.Items.Add("GameCube");
            comboBoxPlatform.Items.Add("Xbox / PC");
            comboBoxPlatform.Items.Add("Playstation 2");

            dateTimePicker1.Value = DateTime.Now;
        }

        private bool OK = false;
        private Platform platform = Platform.Unknown;
        private Game game = Game.Unknown;
        private HipSection[] result = null;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            OK = true;

            if (comboBoxGame.SelectedIndex == 0)
                game = Game.Scooby;
            else if (comboBoxGame.SelectedIndex == 1)
                game = Game.BFBB;
            else if (comboBoxGame.SelectedIndex == 2)
                game = Game.Incredibles;
            if (comboBoxPlatform.SelectedIndex == 0)
                platform = Platform.GameCube;
            else if (comboBoxPlatform.SelectedIndex == 1)
                platform = Platform.Xbox;
            else if (comboBoxPlatform.SelectedIndex == 2)
                platform = Platform.PS2;

            Section_HIPA HIPA = new Section_HIPA();
            Section_PACK PACK = new Section_PACK();
            Section_DICT DICT = new Section_DICT
            {
                ATOC = new Section_ATOC()
                { AINF = new Section_AINF(0) },
                LTOC = new Section_LTOC()
                { LINF = new Section_LINF(0) },
            };
            Section_STRM STRM = new Section_STRM
            {
                DHDR = new Section_DHDR(-1),
                DPAK = new Section_DPAK()
            };

            PACK.PMOD = new Section_PMOD((int)dateTimePicker1.Value.ToBinary());
            PACK.PCRT = new Section_PCRT((int)dateTimePicker1.Value.ToBinary(), textBoxPCRT.Text);
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
                            platform = Platform.GameCube;
                            break;
                        case Platform.PS2:
                            PACK.PFLG = new Section_PFLG(36438062);
                            PACK.PLAT.targetPlatform = "P2";
                            PACK.PLAT.targetPlatformName = "PlayStation 2";
                            platform = Platform.PS2;
                            break;
                        case Platform.Xbox:
                            PACK.PFLG = new Section_PFLG(36306990);
                            PACK.PLAT.targetPlatform = "XB";
                            PACK.PLAT.targetPlatformName = "Xbox";
                            platform = Platform.Xbox;
                            break;
                    }
                    break;
                case Game.Incredibles:
                    PACK.PLAT.language = "US";
                    PACK.PLAT.targetGame = "Incredibles";
                    switch (platform)
                    {
                        case Platform.Xbox:
                            PACK.PLAT.targetPlatform = "BX";
                            platform = Platform.Xbox;
                            break;
                        case Platform.GameCube:
                            PACK.PLAT.targetPlatform = "GC";
                            platform = Platform.GameCube;
                            break;
                        case Platform.PS2:
                            PACK.PLAT.targetPlatform = "PS2";
                            platform = Platform.PS2;
                            break;
                    }
                    break;
            }

            result = new HipSection[] { HIPA, PACK, DICT, STRM };

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = comboBoxGame.SelectedIndex != -1 && comboBoxPlatform.SelectedIndex != -1;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            textBoxPCRT.Text = dateTimePicker1.Value.ToString();
        }
    }
}

using HipHopFile;
using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AddAssetDialog : Form
    {
        System.Drawing.Color defaultColor;

        public AddAssetDialog()
        {
            InitializeComponent();

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
                comboBoxAssetTypes.Items.Add(o);

            buttonOK.Enabled = false;
            TopMost = true;
            defaultColor = textBoxAssetID.BackColor;
        }

        public AddAssetDialog(Section_AHDR AHDR)
        {
            InitializeComponent();

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
                comboBoxAssetTypes.Items.Add(o);

            TopMost = true;
            defaultColor = textBoxAssetID.BackColor;

            comboBoxAssetTypes.SelectedItem = AHDR.assetType;
            textBoxAssetID.Text = "0x" + AHDR.assetID.ToString("X8");
            checkSourceFile.Checked = (AHDR.flags & AHDRFlags.SOURCE_FILE) != 0;
            checkSourceVirtual.Checked = (AHDR.flags & AHDRFlags.SOURCE_VIRTUAL) != 0;
            checkReadT.Checked = (AHDR.flags & AHDRFlags.READ_TRANSFORM) != 0;
            checkWriteT.Checked = (AHDR.flags & AHDRFlags.WRITE_TRANSFORM) != 0;

            assetID = AHDR.assetID;
            assetType = AHDR.assetType;
            assetName = AHDR.ADBG.assetName;
            assetFileName = AHDR.ADBG.assetFileName;
            checksum = AHDR.ADBG.checksum;
            data = AHDR.data;

            textBoxAssetName.Text = AHDR.ADBG.assetName;
            textBoxAssetFileName.Text = AHDR.ADBG.assetFileName;
            labelRawDataSize.Text = "Raw Data Size: " + AHDR.fileSize.ToString();
            textBoxChecksum.Text = "0x"+ AHDR.ADBG.checksum.ToString("X8");

            VerifyTemplate();
        }

        private string GetMiscSettingsString(byte[] MiscSettings)
        {
            if (MiscSettings.Length == 0) return "";

            string output = String.Format("{0, 2:X2}", MiscSettings[0]) + String.Format("{0, 2:X2}", MiscSettings[1])
                + String.Format("{0, 2:X2}", MiscSettings[2]) + String.Format("{0, 2:X2}", MiscSettings[3]);

            for (int i = 4; i < MiscSettings.Length; i += 4)
            {
                output += " " +
                String.Format("{0, 2:X2}", MiscSettings[i]) + String.Format("{0, 2:X2}", MiscSettings[i + 1]) +
                String.Format("{0, 2:X2}", MiscSettings[i + 2]) + String.Format("{0, 2:X2}", MiscSettings[i + 3]);
            }

            return output;
        }

        public static Section_AHDR GetAsset(AddAssetDialog a, out bool success)
        {
            DialogResult d = a.ShowDialog();
            if (d == DialogResult.OK)
            {
                AHDRFlags flags =
                    (a.checkSourceFile.Checked ? AHDRFlags.SOURCE_FILE : 0) |
                    (a.checkSourceVirtual.Checked ? AHDRFlags.SOURCE_VIRTUAL : 0) |
                    (a.checkReadT.Checked ? AHDRFlags.READ_TRANSFORM : 0) |
                    (a.checkWriteT.Checked ? AHDRFlags.WRITE_TRANSFORM : 0);

                Section_ADBG ADBG = new Section_ADBG(0, a.assetName, a.assetFileName, a.checksum);

                Section_AHDR AHDR = new Section_AHDR(a.assetID, a.assetType, flags, ADBG, a.data)
                {
                    fileSize = a.data.Length,
                    plusValue = 0
                };

                int alignment = 16;
                if (Functions.currentGame == Game.BFBB)
                {
                    if (AHDR.assetType == AssetType.CSN |
                        AHDR.assetType == AssetType.SND |
                        AHDR.assetType == AssetType.SNDS)
                        alignment = 32;
                    else if (AHDR.assetType == AssetType.CRDT)
                        alignment = 4;
                }

                int value = AHDR.fileSize % alignment;
                if (value != 0)
                    AHDR.plusValue = alignment - value;

                success = true;
                return AHDR;
            }
            else
            {
                success = false;
                return null;
            }
        }

        public static Section_AHDR GetAsset(Section_AHDR AHDR, out bool success)
        {
            return GetAsset(new AddAssetDialog(AHDR), out success);
        }
        
        uint assetID = 0;
        AssetType assetType = 0;
        string assetName = "";
        string assetFileName = "";

        int checksum = 0;
        byte[] data = new byte[0];

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            assetType = (AssetType)comboBoxAssetTypes.SelectedItem;

            switch (assetType)
            {
                case AssetType.ALST:
                case AssetType.BOUL:
                case AssetType.BUTN:
                case AssetType.CAM:
                case AssetType.CNTR:
                case AssetType.COLL:
                case AssetType.COND:
                case AssetType.CSNM:
                case AssetType.CTOC:
                case AssetType.DPAT:
                case AssetType.DSCO:
                case AssetType.DSTR:
                case AssetType.DYNA:
                case AssetType.EGEN:
                case AssetType.ENV:
                case AssetType.FOG:
                case AssetType.HANG:
                case AssetType.GRUP:
                case AssetType.JAW:
                case AssetType.LODT:
                case AssetType.MAPR:
                case AssetType.MINF:
                case AssetType.MRKR:
                case AssetType.MVPT:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PEND:
                case AssetType.PICK:
                case AssetType.PIPT:
                case AssetType.PKUP:
                case AssetType.PLAT:
                case AssetType.PLYR:
                case AssetType.PORT:
                case AssetType.SFX:
                case AssetType.SHDW:
                case AssetType.SHRP:
                case AssetType.SIMP:
                case AssetType.SNDI:
                case AssetType.SURF:
                case AssetType.TEXT:
                case AssetType.TIMR:
                case AssetType.TRIG:
                case AssetType.UI:
                case AssetType.UIFT:
                case AssetType.VIL:
                case AssetType.VILP:
                    checkSourceVirtual.Checked = true;
                    checkSourceFile.Checked = false;
                    checkReadT.Checked = false;
                    checkWriteT.Checked = false;
                    break;
                case AssetType.CSN:
                case AssetType.FLY:
                case AssetType.RAW:
                    checkSourceVirtual.Checked = false;
                    checkSourceFile.Checked = true;
                    checkReadT.Checked = false;
                    checkWriteT.Checked = false;
                    break;
                case AssetType.ANIM:
                case AssetType.CRDT:
                case AssetType.SND:
                case AssetType.SNDS:
                    checkSourceVirtual.Checked = false;
                    checkSourceFile.Checked = true;
                    checkReadT.Checked = false;
                    checkWriteT.Checked = true;
                    break;
                case AssetType.MODL:
                    checkSourceVirtual.Checked = false;
                    checkSourceFile.Checked = true;
                    checkReadT.Checked = true;
                    checkWriteT.Checked = false;
                    break;
                case AssetType.ATBL:
                case AssetType.JSP:
                case AssetType.RWTX:
                    checkSourceVirtual.Checked = true;
                    checkSourceFile.Checked = false;
                    checkReadT.Checked = true;
                    checkWriteT.Checked = false;
                    break;
                case AssetType.LKIT:
                    checkSourceVirtual.Checked = false;
                    checkSourceFile.Checked = true;
                    checkReadT.Checked = true;
                    checkWriteT.Checked = true;
                    break;
                default:
                    MessageBox.Show("Note: I have not been able to figure out the flags automatically for this asset type. I recommend leaving the same ones that are present in the original asset.");
                    break;
            }

            VerifyTemplate();
        }

        private void textBoxAssetID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBoxAssetID.BackColor = defaultColor;
                assetID = Convert.ToUInt32(textBoxAssetID.Text, 16);
            }
            catch
            {
                textBoxAssetID.BackColor = System.Drawing.Color.Red;
                assetID = 0;
            }
        }

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                data = File.ReadAllBytes(openFileDialog.FileName);
                textBoxAssetName.Text = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                labelRawDataSize.Text = "Raw Data Size: " + data.Length.ToString();
                buttonOK.Enabled = true;
            }
        }

        private void textBoxAssetName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxAssetName.Text.Contains(";"))
                textBoxAssetName.Text = textBoxAssetName.Text.Replace(";", "_");
            else
            {
                assetName = textBoxAssetName.Text;

                if (assetType != AssetType.BSP | assetType != AssetType.JSP)
                    textBoxAssetID.Text = "0x" + Functions.BKDRHash(assetName).ToString("X8");
            }
        }

        private void textBoxAssetFilename_TextChanged(object sender, EventArgs e)
        {
            assetFileName = textBoxAssetFileName.Text;
        }

        private void textBoxChecksum_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBoxChecksum.BackColor = defaultColor;
                checksum = Convert.ToInt32(textBoxChecksum.Text, 16);
            }
            catch
            {
                textBoxChecksum.BackColor = System.Drawing.Color.Red;
                checksum = 0;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private byte[] Template = new byte[0];

        private void VerifyTemplate()
        {
            string[] files = Directory.GetFiles("Resources\\Templates\\" + Functions.currentGame.ToString() + "\\");
            foreach (string s in files)
                if (Path.GetFileName(s) == assetType.ToString())
                {
                    Template = File.ReadAllBytes(s);
                    buttonGrabTemplate.Enabled = true;
                    return;
                }

            Template = new byte[0];
            buttonGrabTemplate.Enabled = false;
        }

        private void buttonGrabTemplate_Click(object sender, EventArgs e)
        {
            data = Template;
            textBoxAssetName.Text = assetType.ToString() + "_NEW";
            labelRawDataSize.Text = "Raw Data Size: " + data.Length.ToString();
            buttonOK.Enabled = true;
        }
    }
}

using HipHopFile;
using System;
using System.Collections.Generic;
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
            textBoxAssetID.Text = AHDR.assetID.ToString("X8");
            checkSourceFile.Checked = (AHDR.flags & AHDRFlags.SOURCE_FILE) != 0;
            checkSourceVirtual.Checked = (AHDR.flags & AHDRFlags.SOURCE_VIRTUAL) != 0;
            checkReadT.Checked = (AHDR.flags & AHDRFlags.READ_TRANSFORM) != 0;
            checkWriteT.Checked = (AHDR.flags & AHDRFlags.WRITE_TRANSFORM) != 0;

            assetID = AHDR.assetID;
            assetType = AHDR.assetType;
            assetName = AHDR.ADBG.assetName;
            assetFileName = AHDR.ADBG.assetFileName;
            checksum = AHDR.ADBG.checksum;
            data = AHDR.containedFile;

            textBoxAssetName.Text = AHDR.ADBG.assetName;
            textBoxAssetFileName.Text = AHDR.ADBG.assetFileName;
            labelRawDataSize.Text = "Raw Data Size: " + AHDR.fileSize.ToString();
            textBoxChecksum.Text = AHDR.ADBG.checksum.ToString("X8");
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
                Section_AHDR AHDR = new Section_AHDR(
                    a.assetID,
                    a.assetType,

                    (a.checkSourceFile.Checked ? AHDRFlags.SOURCE_FILE : 0) |
                    (a.checkSourceVirtual.Checked ? AHDRFlags.SOURCE_VIRTUAL : 0) |
                    (a.checkReadT.Checked ? AHDRFlags.READ_TRANSFORM : 0) |
                    (a.checkWriteT.Checked ? AHDRFlags.WRITE_TRANSFORM : 0),

                    new Section_ADBG(0, a.assetName, a.assetFileName, a.checksum))
                {
                    containedFile = a.data,
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
        
        uint assetID;
        AssetType assetType;
        string assetName;
        string assetFileName;

        int checksum;
        byte[] data;

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            assetType = (AssetType)comboBoxAssetTypes.SelectedItem;
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
                labelRawDataSize.Text = "Raw Data Size: " + data.Length.ToString();
                buttonOK.Enabled = true;
            }
        }

        private void textBoxAssetName_TextChanged(object sender, EventArgs e)
        {
            assetName = textBoxAssetName.Text;
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
    }
}

using HipHopFile;
using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AssetHeader : Form
    {
        System.Drawing.Color defaultColor;

        public AssetHeader()
        {
            InitializeComponent();

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
                comboBoxAssetTypes.Items.Add(o);

            buttonOK.Enabled = false;
            TopMost = true;
            defaultColor = textBoxAssetID.BackColor;
        }

        public AssetHeader(Section_AHDR AHDR)
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
        }

        public static Section_AHDR GetAsset(AssetHeader a, out bool success)
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
            return GetAsset(new AssetHeader(AHDR), out success);
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

            AHDRFlags flags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType);

            checkSourceFile.Checked = (flags & AHDRFlags.SOURCE_FILE) != 0;
            checkSourceVirtual.Checked = (flags & AHDRFlags.SOURCE_VIRTUAL) != 0;
            checkReadT.Checked = (flags & AHDRFlags.READ_TRANSFORM) != 0;
            checkWriteT.Checked = (flags & AHDRFlags.WRITE_TRANSFORM) != 0;

            label1.Visible = flags == 0;
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
                textBoxAssetName.Text = Path.GetFileName(openFileDialog.FileName);
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

                if (assetType != AssetType.BSP && assetType != AssetType.JSP)
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
    }
}

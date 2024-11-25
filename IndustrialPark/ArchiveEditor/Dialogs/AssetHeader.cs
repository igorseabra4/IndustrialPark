using HipHopFile;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AssetHeader : Form
    {
        private bool programIsChangingStuff = false;
        System.Drawing.Color defaultColor;

        public AssetHeader()
        {
            InitializeComponent();

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)).Cast<AssetType>().OrderBy(f => f.ToString()))
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(o));

            buttonOK.Enabled = false;
            TopMost = true;
            defaultColor = textBoxAssetID.BackColor;
        }

        public AssetHeader(Section_AHDR AHDR)
        {
            InitializeComponent();

            programIsChangingStuff = true;

            if (AHDR.assetType == AssetType.Texture || AHDR.assetType == AssetType.TextureStream)
            {
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.Texture));
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.TextureStream));

                comboBoxAssetTypes.SelectedIndex = AHDR.assetType == AssetType.Texture ? 0 : 1;
            }
            //else if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.SoundStream)
            //{
            //    comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.Sound));
            //    comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.SoundStream));
            //}
            else if (AHDR.assetType == AssetType.SimpleObject || AHDR.assetType == AssetType.Track)
            {
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.SimpleObject));
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.Track));

                comboBoxAssetTypes.SelectedIndex = AHDR.assetType == AssetType.SimpleObject ? 0 : 1;
            }
            else
            {
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AHDR.assetType));
                comboBoxAssetTypes.SelectedIndex = 0;
            }

            TopMost = true;
            defaultColor = textBoxAssetID.BackColor;

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
            labelRawDataSize.Text = "Raw Data Size: " + AHDR.data.Length.ToString();
            textBoxChecksum.Text = "0x" + AHDR.ADBG.checksum.ToString("X8");

            programIsChangingStuff = false;
        }

        private static Section_AHDR GetAsset(AssetHeader a)
        {
            if (a.ShowDialog() == DialogResult.OK)
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

                return AHDR;
            }
            return null;
        }

        public static Section_AHDR GetAsset(Section_AHDR AHDR)
        {
            return GetAsset(new AssetHeader(AHDR));
        }

        public static Section_AHDR GetAsset()
        {
            return GetAsset(new AssetHeader());
        }

        uint assetID = 0;
        AssetType assetType = 0;
        string assetName = "";
        string assetFileName = "";

        int checksum = 0;
        byte[] data = new byte[0];

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff || comboBoxAssetTypes.Items.Count < 2)
                return;

            var container = (AssetTypeContainer)comboBoxAssetTypes.SelectedItem;
            assetType = container.assetType;

            AHDRFlags flags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType);

            checkSourceFile.Checked = (flags & AHDRFlags.SOURCE_FILE) != 0;
            checkSourceVirtual.Checked = (flags & AHDRFlags.SOURCE_VIRTUAL) != 0;
            checkReadT.Checked = (flags & AHDRFlags.READ_TRANSFORM) != 0;
            checkWriteT.Checked = (flags & AHDRFlags.WRITE_TRANSFORM) != 0;

            label1.Visible = flags == 0;
        }

        private void textBoxAssetID_TextChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

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
            if (programIsChangingStuff)
                return;

            if (textBoxAssetName.Text.Contains(";"))
                textBoxAssetName.Text = textBoxAssetName.Text.Replace(";", "_");
            else
            {
                assetName = textBoxAssetName.Text;

                if ( assetType != AssetType.JSP)
                    textBoxAssetID.Text = "0x" + Functions.BKDRHash(assetName).ToString("X8");
            }
        }

        private void textBoxAssetFilename_TextChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

            assetFileName = textBoxAssetFileName.Text;
        }

        private void textBoxChecksum_TextChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

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

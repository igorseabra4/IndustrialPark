using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AddMultipleAssets : Form
    {
        public AddMultipleAssets()
        {
            InitializeComponent();

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
                comboBoxAssetTypes.Items.Add(o);

            buttonOK.Enabled = false;
            TopMost = true;
        }

        AssetType assetType = AssetType.Null;
        AHDRFlags AHDRflags = 0;
        List<string> assetNames = new List<string>();
        List<byte[]> assetData = new List<byte[]>();

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog.FileNames)
                {
                    assetNames.Add(Path.GetFileNameWithoutExtension(s));
                    assetData.Add(File.ReadAllBytes(s));
                }
                buttonOK.Enabled = true;
                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();

            foreach (string s in assetNames)
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(s));
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            assetType = (AssetType)comboBoxAssetTypes.SelectedItem;
            AHDRflags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType);

            label1.Visible = AHDRflags == 0;
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                assetNames.RemoveAt(listBox1.SelectedIndex);
                assetData.RemoveAt(listBox1.SelectedIndex);

                UpdateListBox();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static List<Section_AHDR> GetAssets(out bool success, out bool overwrite)
        {
            AddMultipleAssets a = new AddMultipleAssets();
            DialogResult d = a.ShowDialog();
            if (d == DialogResult.OK)
            {
                List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                for (int i = 0; i < a.assetNames.Count; i++)
                {
                    Section_ADBG ADBG = new Section_ADBG(0, a.assetNames[i], "", 0);

                    Section_AHDR AHDR = new Section_AHDR(Functions.BKDRHash(a.assetNames[i]), a.assetType, a.AHDRflags, ADBG, a.assetData[i])
                    {
                        fileSize = a.assetData[i].Length
                    };

                    AHDRs.Add(AHDR);
                }

                success = true;
                overwrite = a.checkBoxOverwrite.Checked;
                return AHDRs;
            }
            else
            {
                success = false;
                overwrite = a.checkBoxOverwrite.Checked;
                return null;
            }
        }

    }
}

using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AddMultipleAssets : Form
    {
        public AddMultipleAssets()
        {
            InitializeComponent();

            comboBoxAssetTypes.Items.AddRange(Enum.GetValues(typeof(AssetType)).Cast<AssetType>().OrderBy(f => f.ToString()).Cast<object>().ToArray());

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
                    // if (Path.GetExtension(s).ToLower() == ".anim")
                    // If importing 'Animation' files keep the extension ".anm"
                    if (assetType == AssetType.Animation)
                    {
                        assetNames.Add(Path.GetFileName(s));
                    }
                    else
                    {
                        assetNames.Add(Path.GetFileNameWithoutExtension(s));
                    }
                    
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
                // If importing 'Animation' files keep the extension ".anm"
                if (assetType == AssetType.Animation)
                {
                    listBox1.Items.Add(Path.GetFileName(s));
                }
                else
                {
                    listBox1.Items.Add(Path.GetFileNameWithoutExtension(s));
                }
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

        public static (List<Section_AHDR> AHDRs, bool overwrite) GetAssets()
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

                return (AHDRs, a.checkBoxOverwrite.Checked);
            }
            return (null, false);
        }
    }
}

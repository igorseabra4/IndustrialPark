using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.BSP_IO_Shared;
using static IndustrialPark.Models.Assimp_IO;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        public ImportModel()
        {
            InitializeComponent();
            
            buttonOK.Enabled = false;
            TopMost = true;
            comboBoxAssetTypes.Items.Add(AssetType.MODL);
            comboBoxAssetTypes.Items.Add(AssetType.BSP);
            comboBoxAssetTypes.SelectedItem = AssetType.MODL;
        }

        List<string> filePaths = new List<string>();

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = GetImportFilter()
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog.FileNames)
                    filePaths.Add(s);

                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();

            foreach (string s in filePaths)
                listBox1.Items.Add(Path.GetFileName(s));

            buttonOK.Enabled = listBox1.Items.Count > 0;
        }
        
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                filePaths.RemoveAt(listBox1.SelectedIndex);
                UpdateListBox();
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static List<Section_AHDR> GetAssets(Game game, out bool success, out bool overwrite, out bool simps, out bool ledgegrab, out bool piptVcolors)
        {
            using (ImportModel a = new ImportModel())
                if (a.ShowDialog() == DialogResult.OK)
                {
                    List<Section_AHDR> AHDRs = new List<Section_AHDR>();
                    simps = a.checkBoxGenSimps.Checked;
                    ledgegrab = a.checkBoxLedgeGrab.Checked;

                    if (simps)
                        MessageBox.Show("a SIMP for each imported MODL will be generated and placed on a new DEFAULT layer.");

                    foreach (string filePath in a.filePaths)
                    {
                        string assetName;
                        AssetType assetType = (AssetType)a.comboBoxAssetTypes.SelectedItem;

                        byte[] assetData;

                        if (assetType == AssetType.MODL)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".dff";

                            assetData = Path.GetExtension(filePath).ToLower().Equals(".dff") ?
                                    File.ReadAllBytes(filePath) :
                                    ReadFileMethods.ExportRenderWareFile(
                                        CreateDFFFromAssimp(filePath,
                                        a.checkBoxFlipUVs.Checked,
                                        a.checkBoxIgnoreMeshColors.Checked),
                                        modelRenderWareVersion(game));
                        }
                        else if (assetType == AssetType.BSP)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".bsp";

                            assetData = Path.GetExtension(filePath).ToLower().Equals(".bsp") ?
                                    File.ReadAllBytes(filePath) :
                                    ReadFileMethods.ExportRenderWareFile(
                                        CreateBSPFromAssimp(filePath,
                                        a.checkBoxFlipUVs.Checked,
                                        a.checkBoxIgnoreMeshColors.Checked),
                                        modelRenderWareVersion(game));
                        }
                        else throw new ArgumentException();

                        AHDRs.Add(
                            new Section_AHDR(
                                Functions.BKDRHash(assetName),
                                assetType,
                                ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType),
                                new Section_ADBG(0, assetName, "", 0),
                                assetData));
                    }

                    success = true;
                    overwrite = a.checkBoxOverwrite.Checked;
                    piptVcolors = a.checkBoxEnableVcolors.Checked;
                    return AHDRs;
                }
                else
                {
                    success = overwrite = simps = ledgegrab = piptVcolors = false;
                    return null;
                }
        }

        private void checkBoxGenSimps_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLedgeGrab.Enabled = checkBoxGenSimps.Checked;
        }
    }
}

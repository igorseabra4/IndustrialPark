using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.Models.BSP_IO_Shared;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        public ImportModel()
        {
            InitializeComponent();

            buttonOK.Enabled = false;
            TopMost = true;
            comboBoxAssetTypes.Items.Add(AssetType.Model);
            comboBoxAssetTypes.Items.Add(AssetType.BSP);
            comboBoxAssetTypes.SelectedItem = AssetType.Model;
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

        public static (List<Section_AHDR> AHDRs, bool overwrite, bool simps, bool ledgeGrab, bool piptVColors, bool solidSimps) GetModels(Game game)
        {
            using (ImportModel a = new ImportModel())
                if (a.ShowDialog() == DialogResult.OK)
                {
                    List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                    AssetType assetType = (AssetType)a.comboBoxAssetTypes.SelectedItem;

                    if (assetType == AssetType.Model)
                    {
                        if (a.checkBoxGenSimps.Checked)
                            MessageBox.Show("a SIMP for each imported MODL will be generated and placed on a new DEFAULT layer.");
                    }

                    foreach (string filePath in a.filePaths)
                    {
                        string assetName;

                        byte[] assetData;

                        ReadFileMethods.treatStuffAsByteArray = false;

                        if (assetType == AssetType.Model)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".dff";

                            try
                            {
                                assetData = Path.GetExtension(filePath).ToLower().Equals(".dff") ?
                                    File.ReadAllBytes(filePath) :
                                    ReadFileMethods.ExportRenderWareFile(
                                        CreateDFFFromAssimp(filePath,
                                        a.checkBoxFlipUVs.Checked,
                                        a.checkBoxIgnoreMeshColors.Checked),
                                        modelRenderWareVersion(game));
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("Model could not be imported.\nPlease check that the vertex/triangle counts do not exceed "
                                    + TRI_AND_VERTEX_LIMIT + ".",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Model could not be imported.",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false);
                            }

                        }
                        else if (assetType == AssetType.BSP)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".bsp";

                            try
                            {
                                assetData = Path.GetExtension(filePath).ToLower().Equals(".bsp") ?
                                    File.ReadAllBytes(filePath) :
                                    ReadFileMethods.ExportRenderWareFile(
                                        CreateBSPFromAssimp(filePath,
                                        a.checkBoxFlipUVs.Checked,
                                        a.checkBoxIgnoreMeshColors.Checked),
                                        modelRenderWareVersion(game));
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("Model could not be imported.\nPlease check that:\n- Vertex/triangle counts do not exceed "
                                    + TRI_AND_VERTEX_LIMIT + "\n- Number of vertices matches texture coordinate and vertex color counts",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Model could not be imported.",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false);
                            }

                        }
                        else throw new ArgumentException();

                        AHDRs.Add(new Section_AHDR(
                                Functions.BKDRHash(assetName),
                                assetType,
                                ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType),
                                new Section_ADBG(0, assetName, "", 0),
                                assetData));
                    }

                    return (AHDRs, a.checkBoxOverwrite.Checked, a.checkBoxGenSimps.Checked, a.checkBoxLedgeGrab.Checked, a.checkBoxEnableVcolors.Checked, a.checkBoxSolidSimps.Checked);
                }

            return (null, false, false, false, false, false);
        }

        private void checkBoxGenSimps_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLedgeGrab.Enabled = checkBoxGenSimps.Checked;
            checkBoxSolidSimps.Enabled = checkBoxGenSimps.Checked;
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((AssetType)comboBoxAssetTypes.SelectedItem == AssetType.BSP)
            {
                checkBoxGenSimps.Checked = false;
                checkBoxGenSimps.Enabled = false;
                checkBoxSolidSimps.Checked = false;
                checkBoxSolidSimps.Enabled = false;
                checkBoxLedgeGrab.Checked = false;
                checkBoxLedgeGrab.Enabled = false;
                checkBoxEnableVcolors.Checked = false;
                checkBoxEnableVcolors.Enabled = false;
            }
            else
            {
                checkBoxGenSimps.Enabled = true;
                checkBoxEnableVcolors.Enabled = true;
                checkBoxLedgeGrab.Enabled = checkBoxGenSimps.Checked;
                checkBoxSolidSimps.Enabled = checkBoxGenSimps.Checked;
            }
        }
    }
}

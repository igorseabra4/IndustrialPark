using HipHopFile;
using IndustrialPark.RenderWare;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.Models.BSP_IO_Shared;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        private bool isSingleModel = false;
        public ImportModel(Platform platform, AssetType assetType, bool noLayers)
        {
            InitializeComponent();

            buttonOK.Enabled = false;
            TopMost = true;
            isSingleModel = assetType != AssetType.Null;

            comboBoxAssetTypes.Items.Add(AssetType.Model);
            comboBoxAssetTypes.Items.Add(AssetType.BSP);
            comboBoxAssetTypes.SelectedItem = AssetType.Model;

            checkBoxUseExistingDefaultLayer.Visible = !noLayers;

            if (isSingleModel)
            {
                grpSIMP.Visible = false;
                Text = "Select Model";
                buttonImportRawData.Text = "Select Model...";
                checkBoxOverwrite.Enabled = false;
                Height -= grpSIMP.Height;
                comboBoxAssetTypes.SelectedItem = assetType;
                comboBoxAssetTypes.Enabled = false;
            }

            if (platform != Platform.GameCube)
                checkBoxNativeData.Enabled = false;

            if (platform == Platform.PS2)
                (checkBoxCreatePIPT.Checked, checkBoxCreatePIPT.Enabled) = (true, false);

            if (assetType != AssetType.BSP)
                checkBoxNoMaterials.Visible = false;

        }

        List<string> filePaths = new List<string>();

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = isSingleModel ? false : true,
                Filter = GetImportFilter()
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog.FileNames)
                    filePaths.Add(s);

                if (isSingleModel)
                {
                    filePaths.Clear();
                    filePaths.Add(openFileDialog.FileName);
                }

                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();

            if (isSingleModel)
                listBox1.Items.Add(Path.GetFileName(filePaths[0]));
            else
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

        /// <summary>
        /// Opens a dialog and creates a renderware model from the selected model file. 
        /// If the file extension ends with ".dff" or ".bsp" it will be read raw.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="platform"></param>
        /// <param name="model"></param>
        /// <returns>True if model was created/read successfully, false otherwise</returns>
        public static bool GetModel(Game game, Platform platform, AssetType assetType, out byte[] model, out bool createPipt)
        {
            model = null;
            createPipt = false;

            using (ImportModel a = new ImportModel(platform, assetType, false))
                if (a.ShowDialog() == DialogResult.OK)
                {
                    createPipt = a.checkBoxCreatePIPT.Checked;
                    ReadFileMethods.treatStuffAsByteArray = false;

                    try
                    {
                        if (assetType == AssetType.Model)
                        {
                            model = Path.GetExtension(a.filePaths[0]).ToLower().Equals(".dff") ? File.ReadAllBytes(a.filePaths[0]) :
                                ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(a.filePaths[0],
                                    a.checkBoxTriStrips.Checked,
                                    a.checkBoxFlipUVs.Checked,
                                    a.checkBoxIgnoreMeshColors.Checked,
                                    a.checkBoxVertexColors.Checked,
                                    a.checkBoxTexCoords.Checked,
                                    a.checkBoxNormals.Checked,
                                    a.checkBoxGeoTriangles.Checked,
                                    a.checkBoxMultiAtomic.Checked,
                                    a.checkBoxNativeData.Checked,
                                    a.checkBoxCollTree.Checked,
                                    a.checkBoxBinMesh.Checked), modelRenderWareVersion(game));
                        }
                        else if (assetType == AssetType.BSP)
                        {
                            model = Path.GetExtension(a.filePaths[0]).ToLower().Equals(".bsp") ? File.ReadAllBytes(a.filePaths[0]) :
                                RwUtility.PerformBSPConversion(a.filePaths[0], game, 
                                a.checkBoxNormals.Checked, a.checkBoxVertexColors.Checked, a.checkBoxTexCoords.Checked, a.checkBoxTriStrips.Checked,
                                a.checkBoxIgnoreMeshColors.Checked, a.checkBoxNoMaterials.Checked, a.checkBoxFlipUVs.Checked, a.checkBoxCollTree.Checked);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error importing BSP", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    return true;
                }
            return false;
        }

        public static (List<Section_AHDR> AHDRs, bool overwrite, bool simps, bool ledgeGrab, bool piptVColors, bool solidSimps, bool jsp, bool useExistingDefaultLayer) GetModels(Game game, Platform platform, bool noLayers)
        {
            using (ImportModel a = new ImportModel(platform, AssetType.Null, noLayers))
                if (a.ShowDialog() == DialogResult.OK)
                {
                    List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                    AssetType assetType = (AssetType)a.comboBoxAssetTypes.SelectedItem;

                    foreach (string filePath in a.filePaths)
                    {
                        string assetName;

                        byte[] assetData;

                        ReadFileMethods.treatStuffAsByteArray = false;

                        if (assetType == AssetType.Model || assetType == AssetType.JSP)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".dff";

                            try
                            {
                                assetData = Path.GetExtension(filePath).ToLower().Equals(".dff") ?
                                    File.ReadAllBytes(filePath) :
                                    ReadFileMethods.ExportRenderWareFile(
                                        CreateDFFFromAssimp(filePath,
                                            a.checkBoxTriStrips.Checked,
                                            a.checkBoxFlipUVs.Checked,
                                            a.checkBoxIgnoreMeshColors.Checked,
                                            a.checkBoxVertexColors.Checked,
                                            a.checkBoxTexCoords.Checked,
                                            a.checkBoxNormals.Checked,
                                            a.checkBoxGeoTriangles.Checked,
                                            a.checkBoxMultiAtomic.Checked,
                                            a.checkBoxNativeData.Checked,
                                            a.checkBoxCollTree.Checked,
                                            a.checkBoxBinMesh.Checked),
                                        modelRenderWareVersion(game));
                            }
                            catch (ArgumentException e)
                            {
                                MessageBox.Show("Model could not be imported.\nPlease check that the vertex/triangle counts do not exceed "
                                    + TRI_AND_VERTEX_LIMIT + ".\n " + e.Message,
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false, false, false);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show($"Model could not be imported.\n{e.Message}",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false, false, false);
                            }
                        }
                        else if (assetType == AssetType.BSP)
                        {
                            assetName = Path.GetFileNameWithoutExtension(filePath) + ".bsp";

                            try
                            {
                                assetData = Path.GetExtension(filePath).ToLower().Equals(".bsp") ? File.ReadAllBytes(filePath) :
                                    RwUtility.PerformBSPConversion(filePath, game,
                                    a.checkBoxNormals.Checked, a.checkBoxVertexColors.Checked, a.checkBoxTexCoords.Checked, a.checkBoxTriStrips.Checked,
                                    a.checkBoxIgnoreMeshColors.Checked, a.checkBoxNoMaterials.Checked, a.checkBoxFlipUVs.Checked, a.checkBoxCollTree.Checked);
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("Model could not be imported.\nPlease check that:\n- Vertex/triangle counts do not exceed "
                                    + TRI_AND_VERTEX_LIMIT + "\n- Number of vertices matches texture coordinate and vertex color counts",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false, false, false);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show($"{e.Message}",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false, false, false);
                            }

                        }
                        else
                            throw new ArgumentException();

                        AHDRs.Add(new Section_AHDR(
                                Functions.BKDRHash(assetName),
                                assetType,
                                ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType),
                                new Section_ADBG(0, assetName, "", 0),
                                assetData));
                    }

                    return (AHDRs,
                        a.checkBoxOverwrite.Checked,
                        a.checkBoxGenSimps.Checked,
                        a.checkBoxLedgeGrab.Checked,
                        a.checkBoxCreatePIPT.Checked,
                        a.checkBoxSolidSimps.Checked,
                        assetType == AssetType.JSP,
                        a.checkBoxUseExistingDefaultLayer.Checked);
                }

            return (null, false, false, false, false, false, false, false);
        }

        private void checkBoxGenSimps_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxLedgeGrab.Enabled = checkBoxGenSimps.Checked;
            checkBoxSolidSimps.Enabled = checkBoxGenSimps.Checked;
            checkBoxUseExistingDefaultLayer.Enabled = checkBoxGenSimps.Checked;
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((AssetType)comboBoxAssetTypes.SelectedItem != AssetType.Model)
            {
                checkBoxGenSimps.Checked = false;
                checkBoxGenSimps.Enabled = false;
                checkBoxSolidSimps.Checked = false;
                checkBoxSolidSimps.Enabled = false;
                checkBoxLedgeGrab.Checked = false;
                checkBoxLedgeGrab.Enabled = false;
                checkBoxCreatePIPT.Checked = false;
                checkBoxCreatePIPT.Enabled = false;

                checkBoxNativeData.Checked = false;
                checkBoxNativeData.Enabled = false;
                checkBoxCollTree.Checked = true;
                checkBoxGeoTriangles.Enabled = false;
                checkBoxIgnoreMeshColors.Enabled = false;
                checkBoxBinMesh.Enabled = false;
                checkBoxMultiAtomic.Enabled = false;
                checkBoxTexCoords.Checked = true;
                checkBoxNormals.Checked = false;
                checkBoxVertexColors.Checked = true;
            }
            else
            {
                checkBoxGenSimps.Enabled = true;
                checkBoxCreatePIPT.Enabled = true;
                checkBoxLedgeGrab.Enabled = checkBoxGenSimps.Checked;
                checkBoxSolidSimps.Enabled = checkBoxGenSimps.Checked;
            }
        }

        private void checkBoxCollision_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxCollTree.Enabled = checkBoxGeoTriangles.Checked;
        }

        private void checkBoxNativeData_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxTriStrips.Checked = checkBoxNativeData.Checked;
            checkBoxTriStrips.Enabled = !checkBoxNativeData.Checked;
            checkBoxGeoTriangles.Checked = !checkBoxNativeData.Checked;
            checkBoxGeoTriangles.Enabled = !checkBoxNativeData.Checked;
            checkBoxMultiAtomic.Checked = checkBoxNativeData.Checked;
            checkBoxMultiAtomic.Enabled = !checkBoxNativeData.Checked;
            checkBoxSolidSimps.Checked = !checkBoxNativeData.Checked;
            checkBoxBinMesh.Checked = checkBoxNativeData.Checked;
            checkBoxBinMesh.Enabled = !checkBoxNativeData.Checked;
        }

        private void checkBoxBinMesh_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxTriStrips.Enabled = checkBoxBinMesh.Checked;
        }
    }
}

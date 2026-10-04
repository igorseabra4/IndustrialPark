using HipHopFile;
using IndustrialPark.RenderWare;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;

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
            comboBoxAssetTypes.Items.Add(AssetType.JSP);
            comboBoxAssetTypes.SelectedItem = AssetType.Model;

            checkBoxUseExistingDefaultLayer.Visible = !noLayers;

            if (isSingleModel)
            {
                grpSIMP.Visible = false;
                Text = "Select Model";
                buttonImportRawData.Text = "Select Model...";
                Height -= grpSIMP.Height;
                comboBoxAssetTypes.SelectedItem = assetType;
                comboBoxAssetTypes.Enabled = false;
            }

            //if (platform != Platform.GameCube || assetType == AssetType.JSP)
            //    checkBoxNativeData.Enabled = false;

            if (platform == Platform.PS2)
                (checkBoxCreatePIPT.Checked, checkBoxCreatePIPT.Enabled) = (true, false);

            if (assetType != AssetType.BSP)
                checkBoxNoMaterials.Enabled = false;

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
                        if (assetType == AssetType.Model || assetType == AssetType.JSP)
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
                                    a.checkBoxBinMesh.Checked), GetModelRenderWareVersion(game));
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

        public static (List<Section_AHDR> AHDRs, bool overwrite) GetJSP(Game game, Platform platform, bool noLayers)
        {
            using (ImportModel a = new ImportModel(platform, AssetType.JSP, noLayers))
                if (a.ShowDialog() == DialogResult.OK)
                {
                    List<Section_AHDR> AHDRs = new List<Section_AHDR>();
                    string assetName;
                    byte[] assetData;
                    ReadFileMethods.treatStuffAsByteArray = false;

                    assetName = Path.GetFileNameWithoutExtension(a.filePaths[0]);

                    try
                    {
                        assetData = ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(a.filePaths[0],
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
                            GetModelRenderWareVersion(game));
                    }
                    catch (ArgumentException)
                    {
                        MessageBox.Show("Model could not be imported.\nPlease check that the vertex/triangle counts do not exceed "
                            + TRI_AND_VERTEX_LIMIT + ".",
                            "Error Importing Model",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return (null, false);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Model could not be imported.",
                            "Error Importing Model",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return (null, false);
                    }

                    var clumps = a.checkBoxSplitClump.Checked ? SplitClump(assetData) : ReadFileMethods.ReadRenderWareFile(assetData);
                    for (int i = 0; i < clumps.Length; i++)
                    {
                        AHDRs.Add(new Section_AHDR(Functions.BKDRHash(assetName + i), AssetType.JSP, ArchiveEditorFunctions.AHDRFlagsFromAssetType(AssetType.JSP),
                            new Section_ADBG(0, assetName + i, "", 0),
                            ReadFileMethods.ExportRenderWareFile(clumps[i], GetModelRenderWareVersion(game))));
                    }

                    return (AHDRs, a.checkBoxOverwrite.Checked);
                }
            return (null, false);
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
                            assetName = Path.GetFileNameWithoutExtension(filePath) + (assetType != AssetType.JSP ? ".dff" : "");

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
                                        GetModelRenderWareVersion(game));
                            }
                            catch (ArgumentException)
                            {
                                MessageBox.Show("Model could not be imported.\nPlease check that the vertex/triangle counts do not exceed "
                                    + TRI_AND_VERTEX_LIMIT + ".",
                                    "Error Importing Model",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return (null, false, false, false, false, false, false, false);
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Model could not be imported.",
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

                        if (assetType == AssetType.JSP)
                        {
                            var clumps = a.checkBoxSplitClump.Checked ? SplitClump(assetData) : ReadFileMethods.ReadRenderWareFile(assetData);
                            for (int i = 0; i < clumps.Length; i++)
                            {
                                AHDRs.Add(new Section_AHDR(Functions.BKDRHash(assetName + i), AssetType.JSP, ArchiveEditorFunctions.AHDRFlagsFromAssetType(AssetType.JSP),
                                    new Section_ADBG(0, assetName + i, "", 0),
                                    ReadFileMethods.ExportRenderWareFile(clumps[i], GetModelRenderWareVersion(game))));
                            }
                        }
                        else
                            AHDRs.Add(new Section_AHDR(Functions.BKDRHash(assetName), assetType, ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType),
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
                bool isJSP = (AssetType)comboBoxAssetTypes.SelectedItem == AssetType.JSP;
                checkBoxCreatePIPT.Enabled = false;
                checkBoxSplitClump.Enabled = isJSP;
                grpSIMP.Enabled = false;

                checkBoxTexCoords.Checked = true;
                (checkBoxNormals.Enabled, checkBoxNormals.Checked) = (!isJSP, false);
                checkBoxVertexColors.Checked = true;
                //checkBoxNativeData.Checked = checkBoxNativeData.Enabled = false;
                checkBoxTriStrips.Checked = false;
                (checkBoxMultiAtomic.Enabled, checkBoxMultiAtomic.Checked) = (false, isJSP);
                checkBoxIgnoreMeshColors.Enabled = true;
                (checkBoxBinMesh.Enabled, checkBoxBinMesh.Checked) = (false, isJSP);
                checkBoxGeoTriangles.Enabled = checkBoxGeoTriangles.Checked = false;
                checkBoxCollTree.Enabled = checkBoxCollTree.Checked = !isJSP;
            }
            else
            {
                grpSIMP.Enabled = true;
                checkBoxGenSimps.Enabled = true;
                checkBoxGenSimps.Checked = false;

                checkBoxTexCoords.Checked = true;
                checkBoxNormals.Checked = true;
                checkBoxVertexColors.Checked = false;
                //(checkBoxNativeData.Enabled, checkBoxNativeData.Checked) = (true, false);
                checkBoxTriStrips.Checked = false;
                (checkBoxMultiAtomic.Enabled, checkBoxMultiAtomic.Checked) = (true, false);
                checkBoxIgnoreMeshColors.Checked = true;
                checkBoxBinMesh.Enabled = checkBoxBinMesh.Checked = true;
                checkBoxGeoTriangles.Enabled = checkBoxGeoTriangles.Checked = true;
                checkBoxCollTree.Enabled = checkBoxCollTree.Checked = true;
            }
        }

        private void checkBoxGeoTriangles_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxCollTree.Enabled = checkBoxGeoTriangles.Checked;
        }

        private void checkBoxNativeData_CheckedChanged(object sender, EventArgs e)
        {
            if ((AssetType)comboBoxAssetTypes.SelectedItem == AssetType.BSP)
                return;

            bool isJSP = ((AssetType)comboBoxAssetTypes.SelectedItem) == AssetType.JSP;
            checkBoxTriStrips.Checked = checkBoxNativeData.Checked;
            checkBoxTriStrips.Enabled = !checkBoxNativeData.Checked;
            checkBoxGeoTriangles.Checked = !isJSP && !checkBoxNativeData.Checked;
            checkBoxGeoTriangles.Enabled = !isJSP && !checkBoxNativeData.Checked;
            checkBoxMultiAtomic.Checked = checkBoxNativeData.Checked;
            checkBoxMultiAtomic.Enabled = !checkBoxNativeData.Checked;
            checkBoxSolidSimps.Checked = !isJSP && !checkBoxNativeData.Checked;
            checkBoxBinMesh.Checked = checkBoxNativeData.Checked;
            checkBoxBinMesh.Enabled = !checkBoxNativeData.Checked;
        }

        public static RWVersion GetModelRenderWareVersion(Game game)
        {
            return game switch
            {
                Game.Scooby => new RWVersion(3, 1),
                _ => new RWVersion(3, 5)
            };
        }

        /// <summary>
        /// Splits a single clump to 3 smaller ones with balanced geometry count. 
        /// Very primitive and only guaranteed to work with imported JSP's.
        /// </summary>
        /// <param name="dff"></param>
        /// <returns>3 <see cref="Clump_0010"/>s</returns>
        /// <exception cref="Exception"></exception>
        public static RWSection[] SplitClump(byte[] dff)
        {
            List<RWSection> clumps = new();

            if (ReadFileMethods.ReadRenderWareFile(dff)[0] is Clump_0010 clump)
            {
                if (clump.geometryList.geometryList.Count < 3)
                    throw new Exception("Clump needs to have at least 3 geometries for splitting");

                List<Frame> frames = new();
                List<Geometry_000F> geometries = new();
                List<Atomic_0014> atomics = new();

                int start = 0;
                for (int i = 0; i < 3; i++)
                {
                    int step = (clump.atomicList.Count - start + (3 - i - 1)) / (3 - i);
                    int newIndex = 0;
                    for (int atomIndex = start; atomIndex < start + step; atomIndex++)
                    {
                        Atomic_0014 atomic = clump.atomicList[atomIndex];

                        frames.Add(clump.frameList.frameListStruct.frames[atomic.atomicStruct.frameIndex]);
                        geometries.Add(clump.geometryList.geometryList[atomic.atomicStruct.geometryIndex]);

                        atomic.atomicStruct.geometryIndex = atomic.atomicStruct.frameIndex = newIndex++;
                        atomics.Add(atomic);
                    }

                    clumps.Add(new Clump_0010()
                    {
                        clumpStruct = new ClumpStruct_0001()
                        {
                            atomicCount = atomics.Count,
                        },
                        frameList = new FrameList_000E()
                        {
                            frameListStruct = new FrameListStruct_0001()
                            {
                                frames = frames.GetRange(0, frames.Count),
                            },
                            extensionList = Enumerable.Range(0, frames.Count).Select(_ => new Extension_0003()).ToList()
                        },
                        geometryList = new GeometryList_001A()
                        {
                            geometryListStruct = new GeometryListStruct_0001()
                            {
                                numberOfGeometries = geometries.Count,
                            },
                            geometryList = geometries.GetRange(0, geometries.Count),
                        },
                        atomicList = atomics.GetRange(0, atomics.Count),
                        clumpExtension = new Extension_0003()
                    });
                    frames.Clear();
                    geometries.Clear();
                    atomics.Clear();
                    start += step;
                }
            }
            return clumps.ToArray();
        }
    }
}

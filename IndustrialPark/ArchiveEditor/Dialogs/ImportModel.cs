using HipHopFile;
using IndustrialPark.RenderWare;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static IndustrialPark.Models.Assimp_IO;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        public static ImportModelResult GetImportData(Game game, Platform platform)
        {
            using ImportModel a = new ImportModel(platform, AssetType.Null);
            if (a.ShowDialog() != DialogResult.OK || a.filePaths == null || !a.filePaths.Any())
                return null;

            ReadFileMethods.treatStuffAsByteArray = false;

            var ahdrCollections = new List<List<Section_AHDR>>();

            foreach (string filePath in a.filePaths)
            {
                string assetName = Path.GetFileNameWithoutExtension(filePath);

                if (a.assetType == AssetType.JSP || a.assetType == AssetType.Model)
                {
                    try
                    {
                        byte[] assetData = Path.GetExtension(filePath).ToLower().Equals(".dff") ?
                            File.ReadAllBytes(filePath) :
                            ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(filePath, a.GetImportModelSettings()), GetModelRenderWareVersion(game));

                        if (a.assetType == AssetType.JSP)
                        {
                            byte[][] clumps = a.checkBoxSplitClump.Checked ?
                            SplitClump(assetData).Select(c => ReadFileMethods.ExportRenderWareFile(c, GetModelRenderWareVersion(game))).ToArray() :
                            [assetData];

                            var clumpAHDRs = clumps.Select((data, i) => new Section_AHDR($"{assetName}{i}", a.assetType, AHDRFlagsFromAssetType(a.assetType), data));
                            ahdrCollections.Add([.. clumpAHDRs]);
                        }
                        else
                        {
                            ahdrCollections.Add([new Section_AHDR(assetName, a.assetType, AHDRFlagsFromAssetType(a.assetType), assetData)]);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Model {assetName} could not be imported:\n{ex.Message}",
                            "Error Importing Model",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else // BSP
                {
                    assetName += ".bsp";
                    try
                    {
                        byte[] assetData = Path.GetExtension(filePath).ToLower().Equals(".bsp") ?
                            File.ReadAllBytes(filePath) :
                            RwUtility.PerformBSPConversion(filePath, game,
                            a.checkBoxNormals.Checked,
                            a.checkBoxVertexColors.Checked,
                            a.checkBoxTexCoords.Checked,
                            a.checkBoxTriStrips.Checked,
                            a.checkBoxIgnoreMeshColors.Checked,
                            a.checkBoxNoMaterials.Checked,
                            a.checkBoxFlipUVs.Checked,
                            a.checkBoxCollTree.Checked);

                        ahdrCollections.Add([new Section_AHDR(assetName, a.assetType, AHDRFlagsFromAssetType(a.assetType), assetData)]);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Model {assetName} could not be imported:\n{ex.Message}",
                            "Error Importing Model",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }

            return new ImportModelResult()
            {
                AHDRCollections = ahdrCollections,
                AssetType = a.assetType,
                Overwrite = a.checkBoxOverwrite.Checked,
                GenerateSimps = a.checkBoxGenSimps.Checked,
                LedgeGrabSimps = a.checkBoxLedgeGrab.Checked,
                PiptVColors = a.checkBoxCreatePIPT.Checked,
                SolidSimps = a.checkBoxSolidSimps.Checked,
            };
        }

        public static byte[] GetSingleModel(Game game, Platform platform, AssetType assetType)
        {
            using ImportModel a = new ImportModel(platform, assetType);
            if (a.ShowDialog() != DialogResult.OK || a.filePaths == null || !a.filePaths.Any())
                return null;

            ReadFileMethods.treatStuffAsByteArray = false;

            try
            {
                if (assetType == AssetType.Model || assetType == AssetType.JSP)
                {
                    return Path.GetExtension(a.filePaths[0]).ToLower().Equals(".dff") ? File.ReadAllBytes(a.filePaths[0]) :
                        ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(a.filePaths[0], a.GetImportModelSettings()), GetModelRenderWareVersion(game));
                }
                if (assetType == AssetType.BSP)
                {
                    return Path.GetExtension(a.filePaths[0]).ToLower().Equals(".bsp") ? File.ReadAllBytes(a.filePaths[0]) :
                            RwUtility.PerformBSPConversion(a.filePaths[0], game,
                            a.checkBoxNormals.Checked, a.checkBoxVertexColors.Checked, a.checkBoxTexCoords.Checked, a.checkBoxTriStrips.Checked,
                            a.checkBoxIgnoreMeshColors.Checked, a.checkBoxNoMaterials.Checked, a.checkBoxFlipUVs.Checked, a.checkBoxCollTree.Checked);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Model could not be imported:\n{ex.Message}",
                    "Error Importing Model",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            return null;
        }

        private List<string> filePaths;
        private bool isReplacing;
        private Platform platform;
        private AssetType assetType => comboBoxAssetTypes.SelectedItem != null ? (AssetType)comboBoxAssetTypes.SelectedItem : AssetType.Null;

        public ImportModel(Platform platform, AssetType assetType)
        {
            InitializeComponent();

            buttonOK.Enabled = false;
            TopMost = true;
            isReplacing = assetType != AssetType.Null;

            comboBoxAssetTypes.Items.Add(AssetType.Model);
            comboBoxAssetTypes.Items.Add(AssetType.JSP);
            comboBoxAssetTypes.Items.Add(AssetType.BSP);

            this.platform = platform;

            comboBoxAssetTypes.SelectedItem = isReplacing ? assetType : AssetType.Model;

            if (isReplacing)
            {
                Text = "Select Model";
                buttonImportRawData.Text = "Select Model...";
                comboBoxAssetTypes.Enabled = false;
            }

            filePaths = new List<string>();

            listBox1.AllowDrop = true;
        }

        private void UpdateAvailableCheckboxes()
        {
            checkBoxFlipUVs.Enabled = true;

            (checkBoxCreatePIPT.Enabled, checkBoxCreatePIPT.Checked) =
                (platform == Platform.PS2) ?
                (false, assetType == AssetType.Model && !isReplacing) :
                (assetType == AssetType.Model && !isReplacing, false);

            checkBoxOverwrite.Enabled = !isReplacing;
            if (isReplacing)
                checkBoxOverwrite.Checked = true;

            checkBoxNoMaterials.Enabled = assetType == AssetType.BSP;
            if (!checkBoxNoMaterials.Enabled)
                checkBoxNoMaterials.Checked = false;

            checkBoxSplitClump.Enabled = assetType == AssetType.JSP && !isReplacing;
            if (!checkBoxSplitClump.Enabled)
                checkBoxSplitClump.Checked = false;

            checkBoxTexCoords.Enabled = true;

            checkBoxNormals.Enabled = assetType != AssetType.JSP;
            if (assetType == AssetType.JSP)
                checkBoxNormals.Checked = false;
            if (assetType == AssetType.Model)
                checkBoxNormals.Checked = true;

            checkBoxVertexColors.Enabled = true;
            if (assetType != AssetType.Model)
                checkBoxVertexColors.Checked = true;

            checkBoxNativeData.Enabled = false;
            checkBoxNativeData.Checked = false;

            checkBoxTriStrips.Enabled = true;

            checkBoxMultiAtomic.Enabled = assetType == AssetType.Model;
            checkBoxMultiAtomic.Checked = assetType == AssetType.JSP;

            checkBoxIgnoreMeshColors.Enabled = true;
            checkBoxIgnoreMeshColors.Checked = true;

            checkBoxBinMesh.Enabled = assetType == AssetType.Model;
            checkBoxBinMesh.Checked = assetType != AssetType.BSP;

            checkBoxGeoTriangles.Enabled = assetType == AssetType.Model;
            checkBoxGeoTriangles.Checked = assetType == AssetType.Model;

            checkBoxCollTree.Enabled = assetType != AssetType.JSP;
            checkBoxCollTree.Checked = assetType != AssetType.JSP;

            if (isReplacing || assetType != AssetType.Model)
            {
                grpSIMP.Enabled = false;
                (checkBoxGenSimps.Enabled, checkBoxGenSimps.Checked) = (false, false);
                (checkBoxSolidSimps.Enabled, checkBoxSolidSimps.Checked) = (false, false);
                (checkBoxLedgeGrab.Enabled, checkBoxLedgeGrab.Checked) = (false, false);
            }
            else
            {
                grpSIMP.Enabled = true;
                checkBoxGenSimps.Enabled = true;
                (checkBoxSolidSimps.Enabled, checkBoxSolidSimps.Checked) = (checkBoxGenSimps.Checked, checkBoxGenSimps.Checked);
                (checkBoxLedgeGrab.Enabled, checkBoxLedgeGrab.Checked) = (checkBoxGenSimps.Checked, checkBoxGenSimps.Checked);
            }
        }

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = !isReplacing,
                Filter = GetImportFilter()
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (isReplacing)
                {
                    filePaths.Clear();
                    filePaths.Add(openFileDialog.FileName);
                }
                else
                {
                    foreach (string s in openFileDialog.FileNames)
                        filePaths.Add(s);
                }

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

        private void checkBoxGenSimps_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAvailableCheckboxes();
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAvailableCheckboxes();
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

        public ImportModelSettings GetImportModelSettings()
        {
            return new ImportModelSettings()
            {
                TriStrips = checkBoxTriStrips.Checked,
                FlipUVs = checkBoxFlipUVs.Checked,
                IgnoreMeshColors = checkBoxIgnoreMeshColors.Checked,
                VertexColors = checkBoxVertexColors.Checked,
                TexCoords = checkBoxTexCoords.Checked,
                Normals = checkBoxNormals.Checked,
                GeoTriangles = checkBoxGeoTriangles.Checked,
                MultiAtomic = checkBoxMultiAtomic.Checked,
                NativeData = checkBoxNativeData.Checked,
                CollTree = checkBoxCollTree.Checked,
                BinMesh = checkBoxBinMesh.Checked
            };
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
                if (clump.geometryList.geometryList.Length < 3)
                    throw new Exception("Clump needs to have at least 3 geometries for splitting");

                List<Frame> frames = new();
                List<Geometry_000F> geometries = new();
                List<Atomic_0014> atomics = new();

                int start = 0;
                for (int i = 0; i < 3; i++)
                {
                    int step = (clump.atomicList.Length - start + (3 - i - 1)) / (3 - i);
                    int newIndex = 0;
                    for (int atomIndex = start; atomIndex < start + step; atomIndex++)
                    {
                        Atomic_0014 atomic = clump.atomicList[atomIndex];

                        frames.Add(clump.frameList.frameListStruct.frames[atomic.atomicStruct.frameIndex]);
                        geometries.Add(clump.geometryList.geometryList[atomic.atomicStruct.geometryIndex]);

                        atomic.atomicStruct.geometryIndex = atomic.atomicStruct.frameIndex = newIndex++;
                        atomics.Add(atomic);
                    }

                    clumps.Add(new Clump_0010(
                        frameList: new FrameList_000E(
                            [.. frames],
                            [.. Enumerable.Range(0, frames.Count).Select(_ => new Extension_0003())]
                        ),
                        geometryList: new GeometryList_001A([.. geometries]),
                        atomicList: [.. atomics],
                        clumpExtension: new Extension_0003()
                    ));
                    frames.Clear();
                    geometries.Clear();
                    atomics.Clear();
                    start += step;
                }
            }
            return clumps.ToArray();
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
                return;
            foreach (string file in files)
                filePaths.Add(file);
            UpdateListBox();
        }
    }
}

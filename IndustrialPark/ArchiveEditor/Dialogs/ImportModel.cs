using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.BSP_IO_ReadOBJ;
using static IndustrialPark.Models.BSP_IO_CreateBSP;
using static IndustrialPark.Models.BSP_IO_Collada;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        public ImportModel()
        {
            InitializeComponent();

            //comboBoxAssetTypes.Items.Add("BSP");
            //comboBoxAssetTypes.Items.Add("MODL");

            buttonOK.Enabled = false;
            TopMost = true;
        }

        List<string> filePaths = new List<string>();

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "All supported formats|*.obj;*.dae|OBJ files|*.obj|DAE files|*.dae|All files|*"
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

        public static List<Section_AHDR> GetAssets(out bool success)
        {
            ImportModel a = new ImportModel();
            if (a.ShowDialog() == DialogResult.OK)
            {
                List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                for (int i = 0; i < a.filePaths.Count; i++)
                {
                    ModelConverterData m;

                    if (Path.GetExtension(a.filePaths[i]).ToLower().Equals(".obj"))
                        m = ReadOBJFile(a.filePaths[i]);
                    else if (Path.GetExtension(a.filePaths[i]).ToLower().Equals(".dae"))
                        m = ConvertDataFromDAEObject(ReadDAEFile(a.filePaths[i]), false);
                    else
                    {
                        MessageBox.Show("Unknown file format for " + a.filePaths[i] + ", skipping");
                        continue;
                    }

                    byte[] data;
                    AssetType assetType;
                    string assetName = Path.GetFileNameWithoutExtension(a.filePaths[i]); // + ".dff";

                    //switch (a.comboBoxAssetTypes.SelectedIndex)
                    //{
                    //    case 0:
                    //        assetType = AssetType.BSP;
                    //        data = ReadFileMethods.ExportRenderWareFile(CreateBSPFile(m, a.checkBoxFlipUVs.Checked), scoobyRenderWareVersion);
                    //        break;
                    //    case 1:
                    //        assetType = AssetType.MODL; // scooby
                    //        data = ReadFileMethods.ExportRenderWareFile(CreateDFFFile(m, a.checkBoxFlipUVs.Checked), scoobyRenderWareVersion);
                    //        break;
                    //    case 2:
                    //        assetType = AssetType.MODL; // bfbb
                    //        data = ReadFileMethods.ExportRenderWareFile(CreateDFFFile(m, a.checkBoxFlipUVs.Checked), bfbbRenderWareVersion);
                    //        break;
                    //    default:
                    //        assetType = AssetType.MODL; // movie
                    //        data = ReadFileMethods.ExportRenderWareFile(CreateDFFFile(m, a.checkBoxFlipUVs.Checked), tssmRenderWareVersion);
                    //        break;
                    //}

                    assetType = AssetType.MODL;
                    data = ReadFileMethods.ExportRenderWareFile(CreateDFFFile(m, a.checkBoxFlipUVs.Checked), currentRenderWareVersion);

                    Section_ADBG ADBG = new Section_ADBG(0, assetName, "", 0);
                    Section_AHDR AHDR = new Section_AHDR(Functions.BKDRHash(assetName), assetType, ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType), ADBG, data);

                    AHDRs.Add(AHDR);
                }

                success = true;
                return AHDRs;
            }
            else
            {
                success = false;
                return null;
            }
        }

    }
}

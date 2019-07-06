using HipHopFile;
using RenderWareFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.BSP_IO_Shared;
using static IndustrialPark.Models.Model_IO_Assimp;

namespace IndustrialPark
{
    public partial class ImportModel : Form
    {
        public ImportModel()
        {
            InitializeComponent();
            
            buttonOK.Enabled = false;
            TopMost = true;
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

        public static List<Section_AHDR> GetAssets(out bool success)
        {
            ImportModel a = new ImportModel();
            if (a.ShowDialog() == DialogResult.OK)
            {
                List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                for (int i = 0; i < a.filePaths.Count; i++)
                {
                    byte[] data;
                    AssetType assetType;
                    string assetName = Path.GetFileNameWithoutExtension(a.filePaths[i]); // + ".dff";
                    
                    assetType = AssetType.MODL;
                    data = Path.GetExtension(a.filePaths[i]).ToLower().Equals(".dff") ? File.ReadAllBytes(a.filePaths[i]) :
                        ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(a.filePaths[i], a.checkBoxFlipUVs.Checked), currentRenderWareVersion);

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

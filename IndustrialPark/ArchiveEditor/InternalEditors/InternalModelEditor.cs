using RenderWareFile;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.Models.BSP_IO_CreateBSP;
using static IndustrialPark.Models.BSP_IO_ReadOBJ;
using static IndustrialPark.Models.BSP_IO_Collada;
using System.Linq;
using IndustrialPark.Models;

namespace IndustrialPark
{
    public partial class InternalModelEditor : Form, IInternalEditor
    {
        public InternalModelEditor(AssetRenderWareModel asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;

            if (asset.AHDR.assetType != HipHopFile.AssetType.MODL)
                buttonImport.Enabled = false;

            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetRenderWareModel asset;
        private ArchiveEditorFunctions archive;
        
        public uint GetAssetID()
        {
            return asset.AHDR.assetID;
        }

        private void buttonFindCallers_Click(object sender, System.EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
        }

        private void buttonHelp_Click(object sender, System.EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        private void buttonExport_Click(object sender, System.EventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog()
            {
                Filter = "OBJ Files|*.obj|" + (asset.AHDR.assetType == HipHopFile.AssetType.MODL ? "DFF Files | *.dff" : "BSP Files | *.bsp"),
                FileName = Path.ChangeExtension(asset.AHDR.ADBG.assetName, ".obj")
            };
            if (a.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(a.FileName).ToLower() == ".obj")
                {
                    if (asset.AHDR.assetType == HipHopFile.AssetType.BSP)
                        ConvertBSPtoOBJ(a.FileName, asset.GetRenderWareModelFile(), true);
                    else
                        ConvertDFFtoOBJ(a.FileName, asset.GetRenderWareModelFile(), true);
                }
                else
                    File.WriteAllBytes(a.FileName, asset.Data);
            }
        }

        private void buttonImport_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog()
            {
                Filter = asset.AHDR.assetType == HipHopFile.AssetType.BSP ?
                "All supported types|*.dae;*.obj;*.bsp|DAE Files|*.dae|OBJ Files|*.obj|BSP Files|*.bsp|All files|*.*" :
                "All supported types|*.dae;*.obj;*.dff|DAE Files|*.dae|OBJ Files|*.obj|DFF Files|*.dff|All files|*.*"
            };

            if (a.ShowDialog() == DialogResult.OK)
            {
                archive.UnsavedChanges = true;

                string i = a.FileName;

                ModelConverterData m;

                if (Path.GetExtension(i).ToLower() == ".obj")
                    m = ReadOBJFile(i);
                else if (Path.GetExtension(i).ToLower() == ".dae")
                    m = ConvertDataFromDAEObject(ReadDAEFile(i), false);
                else
                {
                    asset.GetRenderWareModelFile().Dispose();
                    asset.Data = File.ReadAllBytes(i);
                    asset.Setup(Program.MainForm.renderer);
                    return;
                }

                RWSection[] rws;

                if (asset.AHDR.assetType == HipHopFile.AssetType.BSP)
                    rws = CreateBSPFile(m, true);
                else
                    rws = CreateDFFFile(m, true);

                asset.GetRenderWareModelFile().Dispose();
                asset.Data = ReadFileMethods.ExportRenderWareFile(rws, currentRenderWareVersion);
                asset.Setup(Program.MainForm.renderer);
            }
        }
    }
}

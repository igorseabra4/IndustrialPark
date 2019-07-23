using System;
using System.IO;
using System.Windows.Forms;
using RenderWareFile;
using IndustrialPark.Models;
using static IndustrialPark.Models.BSP_IO_Shared;
using static IndustrialPark.Models.BSP_IO_CreateBSP;
using static IndustrialPark.Models.Model_IO_Assimp;
using System.Collections.Generic;

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

        private void buttonFindCallers_Click(object sender, EventArgs e)
        {
            Program.MainForm.FindWhoTargets(GetAssetID());
        }

        private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            archive.UnsavedChanges = true;
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AboutBox.WikiLink + asset.AHDR.assetType.ToString());
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            ChooseTarget.GetTarget(out bool success, out Assimp.ExportFormatDescription format, out string textureExtension);

            if (success)
            {
                SaveFileDialog a = new SaveFileDialog()
                {
                    Filter = format == null ? "RenderWare BSP|*.bsp" : format.Description + "|*." + format.FileExtension,
                };

                if (a.ShowDialog() == DialogResult.OK)
                {                    
                    if (format == null)
                        File.WriteAllBytes(a.FileName, asset.Data);
                    else if (format.FileExtension.ToLower().Equals("obj") && asset.AHDR.assetType == HipHopFile.AssetType.BSP)
                        ConvertBSPtoOBJ(a.FileName, asset.GetRenderWareModelFile(), true);
                    else
                        ExportAssimp(Path.ChangeExtension(a.FileName, format.FileExtension), asset.GetRenderWareModelFile(), true, format, textureExtension);

                    if (checkBoxTextures.Checked)
                    {
                        string folderName = Path.GetDirectoryName(a.FileName);
                        ReadFileMethods.treatStuffAsByteArray = true;
                        var bitmaps = archive.GetTexturesAsBitmaps(asset.Textures);
                        ReadFileMethods.treatStuffAsByteArray = false;
                        foreach (string textureName in bitmaps.Keys)
                            bitmaps[textureName].Save(folderName + "\\" + textureName + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = GetImportFilter(), // "All supported types|*.dae;*.obj;*.bsp|DAE Files|*.dae|OBJ Files|*.obj|BSP Files|*.bsp|All files|*.*",
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                asset.Data = Path.GetExtension(openFile.FileName).ToLower().Equals(".dff") ? File.ReadAllBytes(openFile.FileName) :
                    ReadFileMethods.ExportRenderWareFile(CreateDFFFromAssimp(openFile.FileName, false), currentRenderWareVersion);
                
                asset.Setup(Program.MainForm.renderer);

                archive.UnsavedChanges = true;
            }
        }
    }
}

using RenderWareFile;
using System.IO;
using System.Windows.Forms;
using RenderWareFile.Sections;
using System.Collections.Generic;

namespace IndustrialPark
{
    public partial class InternalTextureEditor : Form, IInternalEditor
    {
        public InternalTextureEditor(AssetRWTX asset, ArchiveEditorFunctions archive)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            
            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = $"[{asset.AHDR.assetType.ToString()}] {asset.ToString()}";
        }

        private void InternalCamEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            archive.CloseInternalEditor(this);
        }

        private AssetRWTX asset;
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
                //Filter = "All supported formats|*.png;*.rwtex|PNG Files|*.png|RWTEX Files|*.rwtex",
                Filter = "RWTEX Files|*.rwtex",
                FileName = Path.ChangeExtension(asset.AHDR.ADBG.assetName, ".rwtex")
            };
            if (a.ShowDialog() == DialogResult.OK)
            {
                //if (Path.GetExtension(a.FileName).ToLower() == ".rwtex")
                    ArchiveEditorFunctions.ExportSingleTextureToRWTEX(asset.Data, a.FileName);
                //else
                //    MessageBox.Show("Unsupported");
            }
        }

        private void buttonImport_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog a = new OpenFileDialog()
            {
                Filter = ImportTextures.filter
            };

            if (a.ShowDialog() == DialogResult.OK)
            {
                archive.UnsavedChanges = true;

                string i = a.FileName;

                if (Path.GetExtension(i).ToLower().Equals(".rwtex"))
                {
                    asset.Data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                    {
                        textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                        textureNativeList = new List<TextureNative_0015>() { new TextureNative_0015().FromBytes(File.ReadAllBytes(i)) },
                        textureDictionaryExtension = new Extension_0003()
                    }, ArchiveEditorFunctions.currentTextureVersion);
                }
                else
                {
                    asset.Data = ArchiveEditorFunctions.CreateRWTXFromBitmap(i, false, false, true, true).data;
                }

                archive.EnableTextureForDisplay(asset);
            }
        }
    }
}

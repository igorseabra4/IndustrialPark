using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.TextureIOHelper;

namespace IndustrialPark
{
    public partial class InternalTextureEditor : Form, IInternalEditor
    {
        public InternalTextureEditor(AssetRWTX asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
        {
            InitializeComponent();
            TopMost = true;

            this.asset = asset;
            this.archive = archive;
            this.updateListView = updateListView;

            DynamicTypeDescriptor dt = new DynamicTypeDescriptor(asset.GetType());
            asset.SetDynamicProperties(dt);
            propertyGridAsset.SelectedObject = dt.FromComponent(asset);
            Text = $"[{asset.assetType}] {asset}";

            RefreshPropertyGrid();
        }

        public void RefreshPropertyGrid()
        {
            if (b != null)
                b.Dispose();
            foreach (Bitmap bitmap in archive.ExportTXDToBitmap(asset.Data).Values)
                b = bitmap;
            pictureBox1.Image = b;

            tableLayoutPanel1.RowStyles[1].SizeType = SizeType.Absolute;
            tableLayoutPanel1.RowStyles[1].Height = b.Height + 16;

            updateListView(asset);
        }

        private void InternalTextureEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            b.Dispose();
            archive.CloseInternalEditor(this);
        }

        private readonly AssetRWTX asset;
        private readonly ArchiveEditorFunctions archive;
        private Bitmap b;
        private readonly Action<Asset> updateListView;

        public uint GetAssetID()
        {
            return asset.assetID;
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
            ArchiveEditorFunctions.OpenWikiPage(asset);
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog a = new SaveFileDialog()
            {
                Filter = "All supported formats|*.png;*.bmp;*.gif;*.jpeg;*.jpg;*.tiff;*.rwtex" +
                "|PNG Files|*.png" +
                "|BMP Files|*.bmp" +
                "|GIF Files|*.gif" +
                "|JPG Files|*.jpeg;*.jpg;" +
                "|TIFF Files|*.tiff" +
                "|RWTEX Files|*.rwtex",

                FileName = Path.ChangeExtension(asset.assetName, ".png")
            };
            if (a.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(a.FileName).ToLower())
                {
                    case ".rwtex":
                        ExportSingleTextureToRWTEX(asset.Data, a.FileName);
                        break;
                    case ".png":
                        b.Save(a.FileName, ImageFormat.Png);
                        break;
                    case ".bmp":
                        b.Save(a.FileName, ImageFormat.Bmp);
                        break;
                    case ".gif":
                        b.Save(a.FileName, ImageFormat.Gif);
                        break;
                    case ".jpg":
                    case ".jpeg":
                        b.Save(a.FileName, ImageFormat.Jpeg);
                        break;
                    case ".tiff":
                        b.Save(a.FileName, ImageFormat.Tiff);
                        break;
                    default:
                        MessageBox.Show("Unknown image format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void buttonImport_Click(object sender, EventArgs e)
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
                    }, currentTextureVersion(archive.game, archive.platform));
                }
                else
                {
                    asset.Data = CreateRWTXFromBitmap(archive.game, archive.platform, i, false, checkBoxFlipTextures.Checked,
                        checkBoxMipmaps.Checked, checkBoxCompress.Checked, checkBoxTransFix.Checked).data;
                }

                if (asset.game == HipHopFile.Game.Scooby)
                {
                    byte[] data = asset.Data;
                    FixTextureForScooby(ref data);
                    asset.Data = data;
                }

                RefreshPropertyGrid();
                archive.EnableTextureForDisplay(asset);
            }
        }

        private void checkBoxTransFix_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTransFix.Checked)
                checkBoxCompress.Checked = false;
        }

        private void checkBoxCompress_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCompress.Checked)
                checkBoxTransFix.Checked = false;
        }
    }
}

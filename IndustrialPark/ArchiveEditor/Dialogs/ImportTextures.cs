using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using static IndustrialPark.TextureIOHelper;

namespace IndustrialPark
{
    public partial class ImportTextures : Form
    {
        public ImportTextures()
        {
            InitializeComponent();

            buttonOK.Enabled = false;
            TopMost = true;
        }

        public static string filter => "All supported formats|*.bmp;*.png;*.gif;*.jpg;*.jpeg;*.jpe;*.jif;*.jfif;*.jfi;*.tif;*.tiff;*.rwtex|All files|*";

        private List<string> filePaths = new List<string>();

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = filter
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

        public static (List<Section_AHDR> AHDRs, bool overwrite) GetAssets(Game game, Platform platform)
        {
            ImportTextures a = new ImportTextures();
            if (a.ShowDialog() == DialogResult.OK)
            {
                ReadFileMethods.treatStuffAsByteArray = true;

                List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                List<string> forBitmap = new List<string>();

                for (int i = 0; i < a.filePaths.Count; i++)
                {
                    if (Path.GetExtension(a.filePaths[i]).ToLower().Equals(".rwtex"))
                    {
                        byte[] data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                        {
                            textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                            textureNativeList = new List<TextureNative_0015>() { new TextureNative_0015().FromBytes(File.ReadAllBytes(a.filePaths[i])) },
                            textureDictionaryExtension = new Extension_0003()
                        }, currentTextureVersion(game, platform));

                        string assetName = Path.GetFileNameWithoutExtension(a.filePaths[i]) + (a.checkBoxRW3.Checked ? ".RW3" : "");

                        Section_ADBG ADBG = new Section_ADBG(0, assetName, "", 0);
                        Section_AHDR AHDR = new Section_AHDR(Functions.BKDRHash(assetName), AssetType.Texture, ArchiveEditorFunctions.AHDRFlagsFromAssetType(AssetType.Texture), ADBG, data);

                        AHDRs.Add(AHDR);
                    }
                    else
                    {
                        forBitmap.Add(a.filePaths[i]);
                    }
                }

                AHDRs.AddRange(CreateRWTXsFromBitmaps(game, platform, forBitmap, a.checkBoxRW3.Checked, a.checkBoxFlipTextures.Checked,
                    a.checkBoxMipmaps.Checked, a.checkBoxCompress.Checked, a.checkBoxTransFix.Checked));

                ReadFileMethods.treatStuffAsByteArray = false;

                if (game == Game.Scooby)
                    for (int i = 0; i < AHDRs.Count; i++)
                    {
                        byte[] data = AHDRs[i].data;
                        FixTextureForScooby(ref data);
                        AHDRs[i].data = data;
                    }

                return (AHDRs, a.checkBoxOverwrite.Checked);
            }
            return (null, false);
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

using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditor : Form
    {
        public ArchiveEditorFunctions archive;
        private bool unsavedChanges = false;

        public ArchiveEditor()
        {
            InitializeComponent();
            TopMost = true;
            archive = new ArchiveEditorFunctions();

            programIsChangingStuff = true;

            foreach (LayerType o in Enum.GetValues(typeof(LayerType)))
            {
                comboBoxLayerTypes.Items.Add(o.ToString());
            }

            programIsChangingStuff = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }
        
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "HIP/HOP Files|*.hip;*.hop",
                Title = "Please choose a HIP or HOP file"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFile.FileName);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (unsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return;
                //if (result == DialogResult.Yes) SaveFile();
            }

            archive.Dispose();
            Program.mainForm.CloseAssetEditor(this);
            Close();
        }

        private void exportTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ArchiveEditor editor in Program.mainForm.archiveEditors)
                foreach (Asset asset in editor.archive.GetAllAssets())
                {
                    if (asset is AssetRWTX texture)
                        ArchiveEditorFunctions.ExportTextureAsset(texture, editor.archive.fileNamePrefix);
                }
        }

        private void OpenFile(string fileName)
        {
            archive.OpenFile(fileName);
            toolStripStatusLabel1.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            Program.mainForm.SetToolStripItemName(this, Text);
            unsavedChanges = false;
            PopulateLayerComboBox();
        }

        private bool programIsChangingStuff = false;

        private string LayerToString(int index)
        {
            return "Layer " + index.ToString() + ": "
                + archive.DICT.LTOC.LHDRList[index].layerType.ToString()
                + " [" + archive.DICT.LTOC.LHDRList[index].assetIDlist.Count() + "]";
        }

        private void PopulateLayerComboBox()
        {
            programIsChangingStuff = true;

            comboBoxLayers.Items.Clear();
            for (int i = 0; i < archive.DICT.LTOC.LHDRList.Count; i++)
            {
                comboBoxLayers.Items.Add(LayerToString(i));
            }

            programIsChangingStuff = false;
        }

        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            comboBoxLayerTypes.SelectedItem = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType.ToString();

            PopulateAssetsComboAndListBox();

            programIsChangingStuff = false;
        }
        
        private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                foreach (LayerType o in Enum.GetValues(typeof(LayerType)))
                {
                    if(o.ToString() == (string)comboBoxLayerTypes.SelectedItem)
                    {
                        archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType = o;
                        comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                        return;
                    }
                }
                throw new Exception("Invalid layer type");
            }
        }

        private void buttonAddLayer_Click(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            archive.DICT.LTOC.LHDRList.Add(new Section_LHDR()
            {
                assetIDlist = new List<int>(),
                LDBG = new Section_LDBG(-1)
            });
            comboBoxLayers.Items.Add(LayerToString(archive.DICT.LTOC.LHDRList.Count - 1));
            comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;

            programIsChangingStuff = false;
        }

        private void buttonRemoveLayer_Click(object sender, EventArgs e)
        {
            programIsChangingStuff = true;
            
            archive.RemoveLayer(comboBoxLayers.SelectedIndex);

            comboBoxLayers.Items.RemoveAt(comboBoxLayers.SelectedIndex);

            for (int i = 0; i < archive.DICT.LTOC.LHDRList.Count; i++)
            {
                comboBoxLayers.Items[i] = LayerToString(i);
            }

            programIsChangingStuff = false;
        }
        
        private void PopulateAssetsComboAndListBox()
        {
            PopulateAssetsComboBox();

            programIsChangingStuff = true;

            List<int> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
            List <AssetType> assetTypeList = new List<AssetType>();
            for (int i = 0; i < assetIDs.Count(); i++)
            {
                if (!assetTypeList.Contains(archive.GetFromAssetID(assetIDs[i]).AHDR.assetType))
                    assetTypeList.Add(archive.GetFromAssetID(assetIDs[i]).AHDR.assetType);
            }
            assetTypeList.Sort();

            comboBoxAssetTypes.Items.Clear();
            comboBoxAssetTypes.Items.Add("All");
            comboBoxAssetTypes.Items.AddRange(assetTypeList.ToArray().Cast<object>().ToArray());

            comboBoxAssetTypes.SelectedIndex = 0;

            programIsChangingStuff = false;
        }

        private void PopulateAssetsComboBox(AssetType type = AssetType.Null)
        {
            listBoxAssets.Items.Clear();

            List<int> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
            List<string> assetList = new List<string>();
            for (int i = 0; i < assetIDs.Count(); i++)
            {
                if (type == AssetType.Null)
                    assetList.Add(archive.GetFromAssetID(assetIDs[i]).ToString());
                else
                {
                    if (archive.GetFromAssetID(assetIDs[i]).AHDR.assetType == type)
                        assetList.Add(archive.GetFromAssetID(assetIDs[i]).ToString());
                }
            }

            assetList.Sort();
            listBoxAssets.Items.AddRange(assetList.ToArray());
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
            {
                if (comboBoxAssetTypes.SelectedIndex == 0)
                    PopulateAssetsComboBox();
                else
                    PopulateAssetsComboBox((AssetType)comboBoxAssetTypes.SelectedItem);
            }
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            Section_AHDR AHDR = AddAssetDialog.GetAsset(new AddAssetDialog(), out bool success);

            if (success)
            {
                archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                PopulateAssetsComboAndListBox();
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;
            
            Section_AHDR AHDR = archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR;
            Section_AHDR newAHDR = new Section_AHDR(AHDR.assetID + 1, AHDR.assetType, AHDR.flags, new Section_ADBG(AHDR.ADBG.alignment, AHDR.ADBG.assetName, AHDR.ADBG.assetFileName, AHDR.ADBG.checksum))
            {
                fileOffset = AHDR.fileOffset,
                fileSize = AHDR.fileSize,
                containedFile = AHDR.containedFile,
                plusValue = AHDR.plusValue
            };
            
            archive.AddAsset(comboBoxLayers.SelectedIndex, newAHDR);
            PopulateAssetsComboAndListBox();
        }

        private void buttonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            archive.RemoveAsset(comboBoxLayers.SelectedIndex, CurrentlySelectedAssetID());

            listBoxAssets.Items.Remove(listBoxAssets.SelectedItem);
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            Section_AHDR AHDR = AddAssetDialog.GetAsset(archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR, out bool success);

            if (success)
            {
                archive.RemoveAsset(comboBoxLayers.SelectedIndex, CurrentlySelectedAssetID());
                archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                PopulateAssetsComboAndListBox();
            }
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
               File.WriteAllBytes(saveFileDialog.FileName, archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR.containedFile);
            }
        }

        private int CurrentlySelectedAssetID()
        {
            return Convert.ToInt32((listBoxAssets.SelectedItem as string).Substring((listBoxAssets.SelectedItem as string).IndexOf('[') + 1, 8), 16);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                archive.currentlyOpenFilePath = saveFileDialog.FileName;
                archive.Save();
            }
        }

        private void exportKnowlifesINIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "TXT files|*.txt"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Dictionary<string, List<SharpDX.Vector3[]>> knowlifesDictionary = new Dictionary<string, List<SharpDX.Vector3[]>>();

                foreach (RenderableAsset ra in ArchiveEditorFunctions.renderableAssetSet)
                {
                    string objectName = ra.AHDR.ADBG.assetName.Trim(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_', ' '});

                    if (!knowlifesDictionary.ContainsKey(objectName))
                        knowlifesDictionary.Add(objectName, new List<SharpDX.Vector3[]>());

                    knowlifesDictionary[objectName].Add(new SharpDX.Vector3[] { ra.Position, ra.Scale });
                }

                //StreamWriter streamWriter = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.Create));

                foreach (string key in knowlifesDictionary.Keys)
                {
                    StreamWriter streamWriter = new StreamWriter(new FileStream(Path.Combine(Path.GetDirectoryName(saveFileDialog.FileName), key + ".txt"), FileMode.Create));

                    foreach (SharpDX.Vector3[] v in knowlifesDictionary[key])
                        streamWriter.WriteLine(v[0].X.ToString() + ", " + v[0].Y.ToString() + ", " + v[0].Z.ToString() + " , " + v[1].X.ToString() + ", " + v[1].Y.ToString() + ", " + v[1].Z.ToString());
                    streamWriter.Write(key);
                    //streamWriter.WriteLine();
                    
                    streamWriter.Close();
                }

                //streamWriter.Close();
            }

        }
    }
}
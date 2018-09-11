using HipHopFile;
using SharpDX;
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
            defaultColor = textBoxFindAsset.BackColor;

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
                if (result == DialogResult.Yes) archive.Save();
            }

            archive.Dispose();
            Program.MainForm.CloseAssetEditor(this);
            Close();
        }

        private void exportTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ArchiveEditor editor in Program.MainForm.archiveEditors)
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
            Program.MainForm.SetToolStripItemName(this, Text);
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            unsavedChanges = false;
            PopulateLayerComboBox();
            PopulateAssetsComboBox();
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
            try
            { 
                archive.DICT.LTOC.LHDRList.Add(new Section_LHDR()
                {
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                });
                comboBoxLayers.Items.Add(LayerToString(archive.DICT.LTOC.LHDRList.Count - 1));
                comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add layer: " + ex.Message);
            }

            programIsChangingStuff = false;
        }

        private void buttonRemoveLayer_Click(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            try
            {
                archive.RemoveLayer(comboBoxLayers.SelectedIndex);

                comboBoxLayers.Items.RemoveAt(comboBoxLayers.SelectedIndex);

                for (int i = 0; i < archive.DICT.LTOC.LHDRList.Count; i++)
                {
                    comboBoxLayers.Items[i] = LayerToString(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to remove layer: " + ex.Message);
            }
            programIsChangingStuff = false;
        }
        
        private void PopulateAssetsComboAndListBox()
        {
            PopulateAssetsComboBox();

            programIsChangingStuff = true;

            List<uint> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
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
            if (comboBoxLayers.SelectedItem == null) return;

            List<uint> assetIDs = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].assetIDlist;
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
                try
                {
                    unsavedChanges = true;
                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    PopulateAssetsComboAndListBox();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }
            }
        }

        private int numCopies = 0;

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            unsavedChanges = true;

            Section_AHDR AHDR = archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR;

            string newAssetName = AHDR.ADBG.assetName + "_COPY_" + numCopies.ToString();

            uint newAssetId = Functions.BKDRHash(newAssetName);
            while (archive.DictionaryHasKey(newAssetId))
                newAssetId++;

            Section_AHDR newAHDR = new Section_AHDR(newAssetId, AHDR.assetType, AHDR.flags, new Section_ADBG(AHDR.ADBG.alignment, newAssetName, AHDR.ADBG.assetFileName, AHDR.ADBG.checksum))
            {
                fileOffset = AHDR.fileOffset,
                fileSize = AHDR.fileSize,
                plusValue = AHDR.plusValue
            };

            newAHDR.data = new byte[AHDR.data.Length];
            for (int i = 0; i < newAHDR.data.Length; i++)
                newAHDR.data[i] = AHDR.data[i];

            archive.AddAsset(comboBoxLayers.SelectedIndex, newAHDR);
            PopulateAssetsComboAndListBox();

            numCopies++;
        }

        private void buttonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            archive.RemoveAsset(CurrentlySelectedAssetID());

            unsavedChanges = true;

            listBoxAssets.Items.Remove(listBoxAssets.SelectedItem);
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetID()) is IClickableAsset a)
                Program.MainForm.renderer.Camera.SetPosition(new Vector3(a.PositionX, a.PositionY, a.PositionZ) - 8 * Program.MainForm.renderer.Camera.GetForward());
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint aid = CurrentlySelectedAssetID();
                Section_AHDR AHDR = AddAssetDialog.GetAsset(archive.GetFromAssetID(aid).AHDR, out bool success);

                if (success)
                {
                    unsavedChanges = true;
                    archive.RemoveAsset(aid);
                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    PopulateAssetsComboAndListBox();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to edit asset: " + ex.Message);
            }
        }
        
        private void buttonInternalEdit_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem != null)
            {
                new InternalAssetEditor(archive.GetFromAssetID(CurrentlySelectedAssetID())).Show();
                unsavedChanges = true;
            }
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem == null)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR.ADBG.assetName
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(saveFileDialog.FileName, archive.GetFromAssetID(CurrentlySelectedAssetID()).AHDR.data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to export asset raw data: " + ex.Message);
                }
            }
        }

        private uint CurrentlySelectedAssetID()
        {
            if (listBoxAssets.SelectedItem != null)
                return GetAssetIDFromName(listBoxAssets.SelectedItem as string);
            else return 0;
        }

        private uint GetAssetIDFromName(string name)
        {
            return Convert.ToUInt32(name.Substring(name.IndexOf('[') + 1, 8), 16);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.Save();
            unsavedChanges = false;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                archive.currentlyOpenFilePath = saveFileDialog.FileName;
                archive.Save();
                unsavedChanges = false;
            }
        }
        
        private void listBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem != null)
            {
                archive.SelectAsset(CurrentlySelectedAssetID());

                if (archive.GetFromAssetID(CurrentlySelectedAssetID()) is IClickableAsset a)
                    buttonView.Visible = true;
                else
                    buttonView.Visible = false;
            }
        }

        public void ScreenClicked(Ray ray, bool isMouseDown)
        {
            if (isMouseDown)
                archive.GizmoSelect(ray);
            else if (archive.FinishedMovingGizmo)
                archive.FinishedMovingGizmo = false;
            else
            {
                uint index = archive.ScreenClicked(ray);
                if (index != 0)
                    SetSelectedIndex(index);
            }
        }

        public void ScreenUnclicked()
        {
            archive.ScreenUnclicked();
        }

        public void MouseMoveX(SharpCamera camera, int deltaX)
        {
            archive.MouseMoveX(camera, deltaX);
        }

        public void MouseMoveY(SharpCamera camera, int deltaY)
        {
            archive.MouseMoveY(camera, deltaY);
        }

        private void SetSelectedIndex(uint assetID)
        {
            try
            {
                comboBoxLayers.SelectedIndex = archive.GetSelectedLayerIndex();
                for (int i = 0; i < listBoxAssets.Items.Count; i++)
                {
                    if (GetAssetIDFromName(listBoxAssets.Items[i] as string) == assetID)
                    {
                        listBoxAssets.SelectedIndex = i;
                        return;
                    }
                }
                if (comboBoxAssetTypes.SelectedIndex != 0)
                {
                    comboBoxAssetTypes.SelectedIndex = 0;
                    SetSelectedIndex(assetID);
                }
            }
            catch { }
        }

        System.Drawing.Color defaultColor;
        private void textBoxFindAsset_TextChanged(object sender, EventArgs e)
        {
            uint assetID = 0;
            try
            {
                textBoxFindAsset.BackColor = defaultColor;
                assetID = Convert.ToUInt32(textBoxFindAsset.Text, 16);
                SetSelectedIndex(assetID);
            }
            catch
            {
                textBoxFindAsset.BackColor = System.Drawing.Color.Red;
            }
        }
        
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clickThisToolStripMenuItem.Visible = false;
            if (MainForm.alternateNamingMode)
            {
                assetIDAssetNameToolStripMenuItem.Checked = true;
                assetNameAssetIDToolStripMenuItem.Checked = false;
            }
            else
            {
                assetIDAssetNameToolStripMenuItem.Checked = false;
                assetNameAssetIDToolStripMenuItem.Checked = true;
            }
        }
        
        private void assetNameAssetIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            assetIDAssetNameToolStripMenuItem.Checked = false;
            assetNameAssetIDToolStripMenuItem.Checked = true;
            MainForm.alternateNamingMode = false;
        }

        private void assetIDAssetNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            assetIDAssetNameToolStripMenuItem.Checked = true;
            assetNameAssetIDToolStripMenuItem.Checked = false;
            MainForm.alternateNamingMode = true;
        }

        private void importTXDArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };
            if (openTXD.ShowDialog() == DialogResult.OK)
            {
                archive.AddTextureDictionary(openTXD.FileName);
                //comboBoxLayers.Items.Add(LayerToString(archive.DICT.LTOC.LHDRList.Count - 1));
                //comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
            }
        }
    }
}
using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditor : Form
    {
        public ArchiveEditorFunctions archive;

        public ArchiveEditor(string filePath)
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

            if (!string.IsNullOrWhiteSpace(filePath))
                OpenFile(filePath);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) archive.Save();
            }

            if (archive.New())
            {
                archive.UnsavedChanges = true;

                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                buttonAddLayer.Enabled = true;
                importTXDArchiveToolStripMenuItem.Enabled = true;
                exportTXDArchiveToolStripMenuItem.Enabled = true;

                PopulateLayerComboBox();
                PopulateAssetList();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) archive.Save();
            }

            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "HIP/HOP Files|*.hip;*.hop",
                Title = "Please choose a HIP or HOP file"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Thread t = new Thread(() => OpenFile(openFile.FileName));
                t.Start();
            }
        }

        private void OpenFile(string fileName)
        {
            archive.OpenFile(fileName);

            if (!InvokeRequired)
                FinishedOpening(fileName);
            else
                Invoke(new Action<string>(FinishedOpening), fileName);
        }

        private void FinishedOpening(string fileName)
        {
            toolStripStatusLabelCurrentFilename.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            Program.MainForm.SetToolStripItemName(this, Text);
            archive.UnsavedChanges = false;

            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            buttonAddLayer.Enabled = true;
            importTXDArchiveToolStripMenuItem.Enabled = true;
            exportTXDArchiveToolStripMenuItem.Enabled = true;

            PopulateLayerComboBox();
            PopulateAssetList();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        public void Save()
        {
            if (archive.currentlyOpenFilePath == null)
                saveAsToolStripMenuItem_Click(null, null);
            else
            {
                Thread t = new Thread(archive.Save);
                t.Start();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                archive.currentlyOpenFilePath = saveFileDialog.FileName;
                archive.Save();

                Text = Path.GetFileName(saveFileDialog.FileName);
                Program.MainForm.SetToolStripItemName(this, Text);
                toolStripStatusLabelCurrentFilename.Text = "File: " + saveFileDialog.FileName;
                archive.UnsavedChanges = false;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) archive.Save();
            }

            CloseArchiveEditor();
        }

        public void CloseArchiveEditor()
        {
            archive.Dispose();

            Program.MainForm.CloseAssetEditor(this);
            Close();
        }

        public void DisposeAll()
        {
            archive.Dispose();
        }

        private bool programIsChangingStuff = false;

        public bool HasAsset(uint assetID)
        {
            return archive.ContainsAsset(assetID);
        }

        public string GetAssetNameFromID(uint assetID)
        {
            return archive.GetFromAssetID(assetID).AHDR.ADBG.assetName;
        }

        public string GetCurrentlyOpenFileName()
        {
            if (string.IsNullOrWhiteSpace(archive.currentlyOpenFilePath))
                return "Empty";
            else
                return archive.currentlyOpenFilePath;
        }

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
                comboBoxLayers.Items.Add(LayerToString(i));

            programIsChangingStuff = false;
        }

        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            programIsChangingStuff = true;

            comboBoxLayerTypes.SelectedItem = archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType.ToString();

            PopulateAssetListAndComboBox();

            buttonAddAsset.Enabled = true;
            buttonPaste.Enabled = true;
            buttonRemoveLayer.Enabled = true;
            buttonArrowUp.Enabled = true;
            buttonArrowDown.Enabled = true;

            programIsChangingStuff = false;
        }
        
        private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLayerTypes.SelectedItem == null)
                return;

            if (!programIsChangingStuff)
            {
                foreach (LayerType o in Enum.GetValues(typeof(LayerType)))
                {
                    if(o.ToString() == (string)comboBoxLayerTypes.SelectedItem)
                    {
                        archive.DICT.LTOC.LHDRList[comboBoxLayers.SelectedIndex].layerType = o;
                        comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                        archive.UnsavedChanges = true;
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
                archive.UnsavedChanges = true;
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

            int previndex = comboBoxLayers.SelectedIndex;

            archive.RemoveLayer(comboBoxLayers.SelectedIndex);

            PopulateLayerComboBox();

            programIsChangingStuff = false;

            if (comboBoxLayers.Items.Count > 0)
                comboBoxLayers.SelectedIndex = Math.Max(previndex - 1, 0);
            else
            {
                comboBoxLayers.SelectedItem = null;
                comboBoxLayerTypes.SelectedItem = null;

                comboBoxAssetTypes.Items.Clear();
                listBoxAssets.Items.Clear();

                buttonAddAsset.Enabled = false;
                buttonPaste.Enabled = false;
                buttonRemoveLayer.Enabled = false;
                buttonArrowUp.Enabled = false;
                buttonArrowDown.Enabled = false;

                buttonCopy.Enabled = false;
                buttonDuplicate.Enabled = false;
                buttonRemoveAsset.Enabled = false;
                buttonExportRaw.Enabled = false;
                buttonInternalEdit.Enabled = false;
            }
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            int previndex = comboBoxLayers.SelectedIndex;
            archive.MoveLayerUp(comboBoxLayers.SelectedIndex);
            PopulateLayerComboBox();
            comboBoxLayers.SelectedIndex = Math.Max(previndex - 1, 0);
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            int previndex = comboBoxLayers.SelectedIndex;
            archive.MoveLayerDown(comboBoxLayers.SelectedIndex);
            PopulateLayerComboBox();
            comboBoxLayers.SelectedIndex = Math.Min(previndex + 1, comboBoxLayers.Items.Count - 1);
        }

        private void PopulateAssetListAndComboBox()
        {
            PopulateAssetList();

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

        private void PopulateAssetList(AssetType type = AssetType.Null)
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
                    PopulateAssetList();
                else
                    PopulateAssetList((AssetType)comboBoxAssetTypes.SelectedItem);
            }
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            Section_AHDR AHDR = AddAssetDialog.GetAsset(new AddAssetDialog(), out bool success);

            if (success)
            {
                try
                {
                    while (archive.ContainsAsset(AHDR.assetID))
                    {
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID.ToString("X8")}]. Will change it to [{(AHDR.assetID + 1).ToString("X8")}].");
                        AHDR.assetID++;
                    }
                    archive.UnsavedChanges = true;
                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                    PopulateAssetListAndComboBox();
                    SetSelectedIndexes(new List<uint>() { AHDR.assetID });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            archive.UnsavedChanges = true;

            List<uint> finalIndexes = new List<uint>();
            foreach (uint u in archive.GetCurrentlySelectedAssetIDs())
            {
                string serializedObject = JsonConvert.SerializeObject(archive.GetFromAssetID(u).AHDR);
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                archive.AddAssetWithUniqueID(comboBoxLayers.SelectedIndex, AHDR);

                finalIndexes.Add(AHDR.assetID);
            }

            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
            PopulateAssetListAndComboBox();

            SetSelectedIndexes(finalIndexes);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            List<Section_AHDR> copiedAHDRs = new List<Section_AHDR>();

            foreach (uint u in archive.GetCurrentlySelectedAssetIDs())
                copiedAHDRs.Add(archive.GetFromAssetID(u).AHDR);

            Clipboard.SetText(JsonConvert.SerializeObject(copiedAHDRs));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            List<Section_AHDR> AHDRs;
            List<uint> assetIDs = new List<uint>();

            archive.UnsavedChanges = true;

            try
            {
                AHDRs = JsonConvert.DeserializeObject<List<Section_AHDR>>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting object from clipboard: " + ex.Message + ". Are you sure you have an asset copied?");
                return;
            }

            foreach (Section_AHDR AHDR in AHDRs)
            {
                archive.AddAssetWithUniqueID(comboBoxLayers.SelectedIndex, AHDR);
                assetIDs.Add(AHDR.assetID);
            }

            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
            PopulateAssetListAndComboBox();

            SetSelectedIndexes(assetIDs);
        }

        private void buttonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            archive.RemoveAsset(CurrentlySelectedAssetIDs());
            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);

            archive.UnsavedChanges = true;

            listBoxAssets.Items.Remove(listBoxAssets.SelectedItem);
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedIndex < 0) return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is AssetCAM cam)
                Program.MainForm.renderer.Camera.SetPositionCamera(cam);
            else if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset a)
                Program.MainForm.renderer.Camera.SetPosition(new Vector3(a.PositionX, a.PositionY, a.PositionZ) - 8 * Program.MainForm.renderer.Camera.GetForward());
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint oldAssetID = CurrentlySelectedAssetIDs()[0];
                Section_AHDR AHDR = AddAssetDialog.GetAsset(archive.GetFromAssetID(oldAssetID).AHDR, out bool success);

                if (success)
                {
                    archive.UnsavedChanges = true;
                    archive.RemoveAsset(oldAssetID);

                    while (archive.ContainsAsset(AHDR.assetID))
                    {
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID.ToString("X8")}]. Will change it to [{(AHDR.assetID + 1).ToString("X8")}].");
                        AHDR.assetID++;
                    }

                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR);
                    PopulateAssetListAndComboBox();
                    SetSelectedIndexes(new List<uint>() { AHDR.assetID });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to edit asset: " + ex.Message);
            }
        }

        private void buttonInternalEdit_Click(object sender, EventArgs e)
        {
            OpenInternalEditors();
        }

        public void OpenInternalEditors()
        {
            archive.OpenInternalEditor(archive.GetCurrentlySelectedAssetIDs());
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            if (listBoxAssets.SelectedItem == null)
                return;

            if (CurrentlySelectedAssetIDs().Count == 1)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]).AHDR.ADBG.assetName
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    try
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]).AHDR.data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to export asset raw data: " + ex.Message);
                    }
            }
            else
            {
                CommonOpenFileDialog saveFileDialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };
                if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
                    foreach (uint u in CurrentlySelectedAssetIDs())
                        try
                        {
                            File.WriteAllBytes(saveFileDialog.FileName + "\\" + archive.GetFromAssetID(u).AHDR.ADBG.assetName, archive.GetFromAssetID(u).AHDR.data);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Unable to export asset raw data: " + ex.Message);
                        }
            }
        }

        private List<uint> CurrentlySelectedAssetIDs()
        {
            List<uint> list = new List<uint>();
            foreach (object i in listBoxAssets.SelectedItems)
                list.Add(GetAssetIDFromName(i as string));
            return list;
        }

        private uint GetAssetIDFromName(string name)
        {
            return Convert.ToUInt32(name.Substring(name.IndexOf('[') + 1, 8), 16);
        }
        
        private void listBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            archive.ClearSelectedAssets();
            foreach (string s in listBoxAssets.SelectedItems)
                archive.SelectAsset(GetAssetIDFromName(s), true);

            if (listBoxAssets.SelectedItems.Count == 0)
            {
                buttonCopy.Enabled = false;
                buttonDuplicate.Enabled = false;
                buttonRemoveAsset.Enabled = false;
                buttonExportRaw.Enabled = false;
                buttonInternalEdit.Enabled = false;
            }
            else
            {
                buttonCopy.Enabled = true;
                buttonDuplicate.Enabled = true;
                buttonRemoveAsset.Enabled = true;
                buttonExportRaw.Enabled = true;
                buttonInternalEdit.Enabled = true;
            }

            if (listBoxAssets.SelectedItems.Count == 1)
            {
                buttonEditAsset.Enabled = true;

                if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset a)
                {
                    if (a is AssetDYNA dyna)
                        buttonView.Enabled = dyna.IsRenderableClickable;
                    else
                        buttonView.Enabled = true;
                }
                else
                    buttonView.Enabled = false;
            }
            else
            {
                buttonEditAsset.Enabled = false;
                buttonView.Enabled = false;
            }
        }

        public void MouseMoveX(SharpCamera camera, int deltaX)
        {
            archive.MouseMoveX(camera, deltaX);
        }

        public void MouseMoveY(SharpCamera camera, int deltaY)
        {
            archive.MouseMoveY(camera, deltaY);
        }

        public void SetSelectedIndexes(List<uint> assetIDs, bool add = false)
        {
            if (assetIDs.Contains(0) && !add)
            {
                listBoxAssets.SelectedIndices.Clear();
                archive.UpdateGizmoPosition();
                return;
            }

            if (add)
                assetIDs.AddRange(CurrentlySelectedAssetIDs());

            listBoxAssets.SelectedIndices.Clear();

            AssetType assetType = AssetType.Null;

            foreach (uint u in assetIDs)
                if (archive.ContainsAsset(u))
                {
                    assetType = archive.GetFromAssetID(u).AHDR.assetType;
                    comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(u);
                    break;
                }

            foreach (uint u in assetIDs)
                if (archive.ContainsAsset(u) && archive.GetFromAssetID(u).AHDR.assetType != assetType)
                {
                    assetType = AssetType.Null;
                    break;
                }

            comboBoxAssetTypes.SelectedItem = assetType;
            PopulateAssetList(assetType);

            foreach (uint u in assetIDs)
            {
                if (!archive.ContainsAsset(u))
                    continue;

                for (int i = 0; i < listBoxAssets.Items.Count; i++)
                    if (GetAssetIDFromName(listBoxAssets.Items[i] as string) == u)
                        listBoxAssets.SelectedIndices.Add(i);
            }
        }

        public void FindWhoTargets(uint assetID)
        {
            archive.FindWhoTargets(assetID);
        }

        System.Drawing.Color defaultColor;

        private void textBoxFindAsset_TextChanged(object sender, EventArgs e)
        {
            uint assetID = 0;
            try
            {
                textBoxFindAsset.BackColor = defaultColor;
                assetID = AssetIDTypeConverter.AssetIDFromString(textBoxFindAsset.Text);
                SetSelectedIndexes(new List<uint>() { assetID });
            }
            catch
            {
                textBoxFindAsset.BackColor = System.Drawing.Color.Red;
            }
        }

        private void hipHopToolExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                archive.ExportHip(openFile.FileName);
        }

        private void importHIPArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            { Filter = "All supported file types|*.hip;*.hop;*.ini|HIP archives|*.hip|HOP archives|*.hop|HipHopTool INI|*.ini" };

            if (openFile.ShowDialog() == DialogResult.OK)
                archive.ImportHip(openFile.FileName);
        }

        private void exportTXDArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveTXD = new SaveFileDialog() { Filter = "TXD archives|*.txd" };

            if (saveTXD.ShowDialog() == DialogResult.OK)
                archive.ExportTextureDictionary(saveTXD.FileName);
        }

        private void importTXDArchiveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };

            if (openTXD.ShowDialog() == DialogResult.OK)
            {
                archive.AddTextureDictionary(openTXD.FileName);
                PopulateLayerComboBox();
            }
        }
    }
}
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
                hipHopToolExportToolStripMenuItem.Enabled = true;
                importHIPArchiveToolStripMenuItem.Enabled = true;

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
            hipHopToolExportToolStripMenuItem.Enabled = true;
            importHIPArchiveToolStripMenuItem.Enabled = true;
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
            importMultipleAssetsToolStripMenuItem.Enabled = true;

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
                importMultipleAssetsToolStripMenuItem.Enabled = false;

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
            Section_AHDR AHDR = AddAssetDialog.GetAsset(new AddAssetDialog(), out bool success, out bool setPosition);

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
                    if (setPosition)
                        archive.SetAssetPositionToView(AHDR.assetID);

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

        private void importMultipleAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Section_AHDR> AHDRs = AddMultipleAssetDialog.GetAssets(out bool success);

            if (success)
            {
                archive.UnsavedChanges = true;

                try
                {
                    List<uint> assetIDs = new List<uint>();
                    
                    foreach (Section_AHDR AHDR in AHDRs)
                    {
                        if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                        {
                            try
                            {
                                archive.AddSoundToSNDI(AHDR.data, AHDR.assetID, AHDR.assetType, out byte[] soundData);
                                AHDR.data = soundData;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                        archive.AddAssetWithUniqueID(comboBoxLayers.SelectedIndex, AHDR);
                        assetIDs.Add(AHDR.assetID);
                    }

                    comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                    PopulateAssetListAndComboBox();

                    SetSelectedIndexes(assetIDs);
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
            {
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(JsonConvert.SerializeObject(archive.GetFromAssetID(u).AHDR));

                if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                {
                    List<byte> file = new List<byte>();
                    file.AddRange(archive.GetHeaderFromSNDI(AHDR.assetID));
                    file.AddRange(AHDR.data);

                    if (new string(new char[] { (char)file[0], (char)file[1], (char)file[2], (char)file[3] }) == "RIFF")
                    {
                        byte[] chunkSizeArr = BitConverter.GetBytes(file.Count - 8);

                        file[4] = chunkSizeArr[0];
                        file[5] = chunkSizeArr[1];
                        file[6] = chunkSizeArr[2];
                        file[7] = chunkSizeArr[3];
                    }

                    AHDR.data = file.ToArray();
                }

                copiedAHDRs.Add(AHDR);
            }

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
                MessageBox.Show("Error pasting objects from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            foreach (Section_AHDR AHDR in AHDRs)
            {
                if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                {
                    try
                    {
                        archive.AddSoundToSNDI(AHDR.data, AHDR.assetID, AHDR.assetType, out byte[] soundData);
                        AHDR.data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

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

            AssetType a = AssetType.Null;
            if (comboBoxAssetTypes.SelectedIndex > 0)
                a = (AssetType)comboBoxAssetTypes.SelectedItem;
            int prevIndex = listBoxAssets.SelectedIndex;

            archive.RemoveAsset(CurrentlySelectedAssetIDs());
            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);

            archive.UnsavedChanges = true;

            listBoxAssets.Items.Remove(listBoxAssets.SelectedItem);

            comboBoxAssetTypes.SelectedItem = a;

            if (listBoxAssets.Items.Count > 0)
                try { listBoxAssets.SelectedIndex = prevIndex; }
                catch
                {
                    try { listBoxAssets.SelectedIndex = prevIndex - 1; }
                    catch { }
                }
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
                Section_AHDR AHDR = AddAssetDialog.GetAsset(archive.GetFromAssetID(oldAssetID).AHDR, out bool success, out bool setPosition);

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
                    if (setPosition)
                        archive.SetAssetPositionToView(AHDR.assetID);

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
            { Filter = "All supported file types|*.hip;*.hop;*.ini|HIP archives|*.hip|HOP archives|*.hop|HipHopTool INI|*.ini",
            Multiselect = true };

            if (openFile.ShowDialog() == DialogResult.OK)
                archive.ImportHip(openFile.FileNames);
            PopulateLayerComboBox();
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

        private void toolStripMenuItem_Add_Click(object sender, EventArgs e)
        {
            buttonAddAsset_Click(null, null);
        }

        private void toolStripMenuItem_Duplicate_Click(object sender, EventArgs e)
        {
            buttonDuplicate_Click(null, null);
        }

        private void toolStripMenuItem_Copy_Click(object sender, EventArgs e)
        {
            buttonCopy_Click(null, null);
        }

        private void toolStripMenuItem_Paste_Click(object sender, EventArgs e)
        {
            buttonPaste_Click(null, null);
        }

        private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
        {
            buttonRemoveAsset_Click(null, null);
        }

        private void toolStripMenuItem_View_Click(object sender, EventArgs e)
        {
            buttonView_Click(null, null);
        }

        private void toolStripMenuItem_Export_Click(object sender, EventArgs e)
        {
            buttonExportRaw_Click(null, null);
        }

        private void toolStripMenuItem_EditHeader_Click(object sender, EventArgs e)
        {
            buttonEditAsset_Click(null, null);
        }

        private void toolStripMenuItem_EditData_Click(object sender, EventArgs e)
        {
            buttonInternalEdit_Click(null, null);
        }

        private void toolStripMenuItem_AddMulti_Click(object sender, EventArgs e)
        {
            importMultipleAssetsToolStripMenuItem_Click(null, null);
        }

        private void listBoxAssets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I && e.Modifiers == Keys.Control)
            {
                if (buttonAddAsset.Enabled)
                    buttonAddAsset_Click(null, null);
            }
            else if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
            {
                if (buttonDuplicate.Enabled)
                    buttonDuplicate_Click(null, null);
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (buttonCopy.Enabled)
                    buttonCopy_Click(null, null);
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (buttonPaste.Enabled)
                    buttonPaste_Click(null, null);
            }
            else if (e.KeyCode == Keys.G && e.Modifiers == Keys.Control)
            {
                if (buttonInternalEdit.Enabled)
                    buttonInternalEdit_Click(null, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (buttonRemoveAsset.Enabled)
                    buttonRemoveAsset_Click(null, null);
            }
        }

        private void listBoxAssets_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                toolStripMenuItem_Add.Enabled = buttonAddAsset.Enabled;
                toolStripMenuItem_AddMulti.Enabled = importMultipleAssetsToolStripMenuItem.Enabled;
                toolStripMenuItem_Duplicate.Enabled = buttonDuplicate.Enabled;
                toolStripMenuItem_Copy.Enabled = buttonCopy.Enabled;
                toolStripMenuItem_Paste.Enabled = buttonPaste.Enabled;
                toolStripMenuItem_Remove.Enabled = buttonRemoveAsset.Enabled;
                toolStripMenuItem_View.Enabled = buttonView.Enabled;
                toolStripMenuItem_Export.Enabled = buttonExportRaw.Enabled;
                toolStripMenuItem_EditHeader.Enabled = buttonEditAsset.Enabled;
                toolStripMenuItem_EditData.Enabled = buttonInternalEdit.Enabled;

                contextMenuStrip_ListBoxAssets.Show(listBoxAssets.PointToScreen(e.Location));
            }
        }

        public void PlaceTemplate(Vector3 position)
        {
            if (comboBoxLayers.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a layer to place your asset in!");
                return;
            }

            List<uint> assetIDs = new List<uint>();

            archive.PlaceTemplate(position, comboBoxLayers.SelectedIndex, out bool success, ref assetIDs);

            if (success)
            {
                archive.UnsavedChanges = true;

                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = LayerToString(comboBoxLayers.SelectedIndex);
                PopulateAssetListAndComboBox();

                SetSelectedIndexes(assetIDs);
            }
        }

        public bool TemplateFocus { get; private set; } = false;

        public void TemplateFocusOff()
        {
            TemplateFocus = false;

            labelTemplateFocus.Text = "Template Focus\nOFF";
            labelTemplateFocus.ForeColor = System.Drawing.Color.Red;
        }

        private void labelTemplateFocus_Click(object sender, EventArgs e)
        {
            Program.MainForm.ClearTemplateFocus();

            TemplateFocus = true;

            labelTemplateFocus.Text = "Template Focus\nON";
            labelTemplateFocus.ForeColor = System.Drawing.Color.Green;
        }
    }
}
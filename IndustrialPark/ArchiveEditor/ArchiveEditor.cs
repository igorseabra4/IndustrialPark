using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
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

            textBoxFindAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            archive.SetTextboxForAutocomplete(textBoxFindAsset);
            
            if (!string.IsNullOrWhiteSpace(filePath))
                OpenFile(filePath);

            ArchiveEditorFunctions.PopulateTemplateMenusAt(addTemplateToolStripMenuItem, TemplateToolStripMenuItem_Click);
            listViewAssets_SizeChanged(null, null);
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

                PopulateLayerTypeComboBox();

                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
                buttonAddLayer.Enabled = true;
                tXDArchiveToolStripMenuItem.Enabled = true;
                hipHopToolExportToolStripMenuItem.Enabled = true;
                importHIPArchiveToolStripMenuItem.Enabled = true;
                collapseLayersToolStripMenuItem.Enabled = true;
                mergeSimilarAssetsToolStripMenuItem.Enabled = true;
                verifyArchiveToolStripMenuItem.Enabled = true;
                applyScaleToolStripMenuItem.Enabled = true;

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
                OpenFile(openFile.FileName);
            }
        }

        private void OpenFile(string fileName)
        {
            archive.OpenFile(fileName);
            
            toolStripStatusLabelCurrentFilename.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            Program.MainForm.SetToolStripItemName(this, Text);
            archive.UnsavedChanges = false;

            PopulateLayerTypeComboBox();

            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            hipHopToolExportToolStripMenuItem.Enabled = true;
            importHIPArchiveToolStripMenuItem.Enabled = true;
            tXDArchiveToolStripMenuItem.Enabled = true;
            buttonAddLayer.Enabled = true;
            collapseLayersToolStripMenuItem.Enabled = true;
            mergeSimilarAssetsToolStripMenuItem.Enabled = true;
            verifyArchiveToolStripMenuItem.Enabled = true;
            applyScaleToolStripMenuItem.Enabled = true;

            PopulateLayerComboBox();
            PopulateAssetList();
        }

        private void PopulateLayerTypeComboBox()
        {
            programIsChangingStuff = true;

            comboBoxLayerTypes.Items.Clear();
            if (Functions.currentGame == Game.Incredibles)
                foreach (var t in Enum.GetValues(typeof(LayerType_TSSM)))
                    comboBoxLayerTypes.Items.Add(t);
            else
                foreach (var t in Enum.GetValues(typeof(LayerType_BFBB)))
                    comboBoxLayerTypes.Items.Add(t);

            programIsChangingStuff = false;
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
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = archive.currentlyOpenFilePath,
                Filter = "HIP/HOP Files|*.hip;*.hop",
                Title = "Please choose a HIP or HOP file to save as"
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                archive.Save(saveFileDialog.FileName);

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
            if (verifyResult != null)
                if (!verifyResult.IsDisposed)
                    verifyResult.Close();

            archive.Dispose();

            Program.MainForm.CloseArchiveEditor(this);
            Close();
        }

        public void DisposeForClosing()
        {
            archive.DisposeForClosing();
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

        private void PopulateLayerComboBox()
        {
            programIsChangingStuff = true;

            comboBoxLayers.Items.Clear();
            for (int i = 0; i < archive.GetLayerCount(); i++)
                comboBoxLayers.Items.Add(archive.LayerToString(i));

            programIsChangingStuff = false;
        }

        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

            programIsChangingStuff = true;

            if (comboBoxLayers.SelectedIndex == -1)
            {
                comboBoxLayerTypes.SelectedItem = null;

                comboBoxAssetTypes.Items.Clear();
                comboBoxAssetTypes.SelectedIndex = -1;
                PopulateAssetList();

                buttonAddAsset.Enabled = false;
                buttonPaste.Enabled = false;
                buttonRemoveLayer.Enabled = false;
                buttonArrowUp.Enabled = false;
                buttonArrowDown.Enabled = false;
                importMultipleAssetsToolStripMenuItem.Enabled = false;
                importModelsToolStripMenuItem.Enabled = false;
                addTemplateToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (Functions.currentGame == Game.Incredibles)
                    comboBoxLayerTypes.SelectedItem = (LayerType_TSSM)archive.GetLayerType(comboBoxLayers.SelectedIndex);
                else
                    comboBoxLayerTypes.SelectedItem = (LayerType_BFBB)archive.GetLayerType(comboBoxLayers.SelectedIndex);

                PopulateAssetListAndComboBox();

                buttonAddAsset.Enabled = true;
                buttonPaste.Enabled = true;
                buttonRemoveLayer.Enabled = true;
                buttonArrowUp.Enabled = true;
                buttonArrowDown.Enabled = true;
                importMultipleAssetsToolStripMenuItem.Enabled = true;
                importModelsToolStripMenuItem.Enabled = true;
                addTemplateToolStripMenuItem.Enabled = true;
            }

            programIsChangingStuff = false;
        }
        
        private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff || comboBoxLayers.SelectedItem == null)
                return;

            archive.SetLayerType(comboBoxLayers.SelectedIndex, (int)comboBoxLayerTypes.SelectedItem);
            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
            archive.UnsavedChanges = true;
        }

        private void buttonAddLayer_Click(object sender, EventArgs e)
        {
            try
            {
                archive.AddLayer();
                comboBoxLayers.Items.Add(archive.LayerToString(archive.GetLayerCount() - 1));
                comboBoxLayers.SelectedIndex = comboBoxLayers.Items.Count - 1;
                PopulateAssetListAndComboBox();
                archive.UnsavedChanges = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add layer: " + ex.Message);
            }
        }

        private void buttonRemoveLayer_Click(object sender, EventArgs e)
        {
            int previndex = comboBoxLayers.SelectedIndex;

            archive.RemoveLayer(comboBoxLayers.SelectedIndex);

            PopulateLayerComboBox();

            if (comboBoxLayers.Items.Count > 0)
                comboBoxLayers.SelectedIndex = Math.Max(previndex - 1, 0);
            else
            {
                comboBoxLayers.SelectedItem = null;
                comboBoxLayerTypes.SelectedItem = null;

                comboBoxAssetTypes.Items.Clear();
                listViewAssets.Items.Clear();

                buttonAddAsset.Enabled = false;
                buttonPaste.Enabled = false;
                buttonRemoveLayer.Enabled = false;
                collapseLayersToolStripMenuItem.Enabled = false;
                mergeSimilarAssetsToolStripMenuItem.Enabled = false;
                verifyArchiveToolStripMenuItem.Enabled = false;
                applyScaleToolStripMenuItem.Enabled = false;
                buttonArrowUp.Enabled = false;
                buttonArrowDown.Enabled = false;
                importMultipleAssetsToolStripMenuItem.Enabled = false;
                importModelsToolStripMenuItem.Enabled = false;
                addTemplateToolStripMenuItem.Enabled = false;

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
            programIsChangingStuff = true;
            
            comboBoxAssetTypes.Items.Clear();
            comboBoxAssetTypes.Items.Add("All");
            comboBoxAssetTypes.Items.AddRange(archive.AssetTypesOnLayer(comboBoxLayers.SelectedIndex).Cast<object>().ToArray());

            comboBoxAssetTypes.SelectedIndex = 0;
            PopulateAssetList();

            programIsChangingStuff = false;
        }

        AssetType curType = AssetType.Null;

        private void PopulateAssetList(AssetType type = AssetType.Null, bool select = false, List<uint> selectionAssetIDs = null)
        {
            curType = type;
            listViewAssets.BeginUpdate();
            listViewAssets.Items.Clear();
            
            if (comboBoxLayers.SelectedItem != null)
            {
                List<uint> assetIDs = archive.GetAssetIDsOnLayer(comboBoxLayers.SelectedIndex);
                List<ListViewItem> items = new List<ListViewItem>();

                for (int i = 0; i < assetIDs.Count(); i++)
                {
                    Asset asset = archive.GetFromAssetID(assetIDs[i]);
                    if (type == AssetType.Null || asset.AHDR.assetType == type)
                    {
                        bool selected = (select == true) && selectionAssetIDs.Contains(asset.AHDR.assetID);
                        items.Add(new ListViewItem(asset.ToString())
                        {
                            Checked = !asset.isInvisible,
                            Selected = selected
                        });
                    }
                }

                listViewAssets.Items.AddRange(items.ToArray());
            }

            listViewAssets.EndUpdate();

            if (select)
            {
                int ensureVisible = -1;

                for (int i = 0; i < listViewAssets.Items.Count; i++)
                {
                    if (listViewAssets.Items[i].Selected)
                        ensureVisible = i;
                }

                if (ensureVisible != -1)
                    listViewAssets.EnsureVisible(ensureVisible);
            }

            toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";
        }

        private void checkedListBoxAssets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            archive.GetFromAssetID(GetAssetIDFromName(listViewAssets.Items[e.Index].Text)).isInvisible = e.NewValue != CheckState.Checked;
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
                PopulateAssetList(comboBoxAssetTypes.SelectedIndex == 0 ? AssetType.Null : (AssetType)comboBoxAssetTypes.SelectedItem);
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            archive.CreateNewAsset(comboBoxLayers.SelectedIndex, out bool success, out uint assetID);

            if (success)
            {
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                SetSelectedIndices(new List<uint>() { assetID }, true);
            }
        }

        private void importMultipleAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Section_AHDR> AHDRs = AddMultipleAssets.GetAssets(out bool success);
            if (success)
            {
                archive.ImportMultipleAssets(comboBoxLayers.SelectedIndex, AHDRs, out List<uint> assetIDs);
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                SetSelectedIndices(assetIDs, true);
            }
        }

        private void importModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Section_AHDR> AHDRs = ImportModel.GetAssets(out bool success);
            if (success)
            {
                archive.ImportMultipleAssets(comboBoxLayers.SelectedIndex, AHDRs, out List<uint> assetIDs);
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                SetSelectedIndices(assetIDs, true);
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0) return;

            archive.DuplicateSelectedAssets(comboBoxLayers.SelectedIndex, out List<uint> finalIndices);
            
            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
            SetSelectedIndices(finalIndices, false);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0) return;

            archive.CopyAssetsToClipboard();
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            archive.PasteAssetsFromClipboard(comboBoxLayers.SelectedIndex, out List<uint> finalIndices);

            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
            
            SetSelectedIndices(finalIndices, true);
        }

        private void ButtonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0) return;

            programIsChangingStuff = true;

            AssetType a = AssetType.Null;
            if (comboBoxAssetTypes.SelectedIndex > 0)
                a = (AssetType)comboBoxAssetTypes.SelectedItem;
            var prevIndex = listViewAssets.SelectedIndices[0];

            archive.RemoveAsset(CurrentlySelectedAssetIDs());

            comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);

            archive.UnsavedChanges = true;

            foreach (ListViewItem v in listViewAssets.SelectedItems)
                listViewAssets.Items.Remove(v);

            programIsChangingStuff = false;

            if (a != curType)
                comboBoxAssetTypes.SelectedItem = a;
            else
            {
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    listViewAssets.Items[i].Selected = false;

                if (listViewAssets.Items.Count > 0)
                    try { listViewAssets.Items[prevIndex].Selected = true; }
                    catch
                    {
                        try { listViewAssets.Items[prevIndex - 1].Selected = true; }
                        catch { }
                    }
            }

            if (listViewAssets.Items.Count == 0)
                PopulateAssetListAndComboBox();
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0) return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is AssetCAM cam)
                Program.MainForm.renderer.Camera.SetPositionCamera(cam);
            else if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset a)
                Program.MainForm.renderer.Camera.SetPosition(new Vector3(a.PositionX, a.PositionY, a.PositionZ) - 8 * Program.MainForm.renderer.Camera.Forward);
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint oldAssetID = CurrentlySelectedAssetIDs()[0];
                Section_AHDR AHDR = AssetHeader.GetAsset(archive.GetFromAssetID(oldAssetID).AHDR, out bool success, out bool setPosition);

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

                    //PopulateAssetListAndComboBox();
                    SetSelectedIndices(new List<uint>() { AHDR.assetID }, true);
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
            archive.OpenInternalEditor(archive.GetCurrentlySelectedAssetIDs(), false);
        }

        public void DeleteSelectedAssets()
        {
            ButtonRemoveAsset_Click(null, null);
            foreach (ListViewItem v in listViewAssets.Items)
                v.Selected = false;
        }

        private void buttonExportRaw_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0)
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
            foreach (ListViewItem v in listViewAssets.SelectedItems)
                list.Add(GetAssetIDFromName(v.Text));
            return list;
        }

        private uint GetAssetIDFromName(string name)
        {
            if (MainForm.alternateNamingMode)
                return Convert.ToUInt32(name.Substring(name.IndexOf('[') + 1, 8), 16);
            return Convert.ToUInt32(name.Substring(name.LastIndexOf('[') + 1, 8), 16);
        }
        
        private void checkedListBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

            toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";

            archive.ClearSelectedAssets();
            foreach (ListViewItem v in listViewAssets.SelectedItems)
                archive.SelectAsset(GetAssetIDFromName(v.Text), true);

            if (listViewAssets.SelectedItems.Count == 0)
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

            if (listViewAssets.SelectedItems.Count == 1)
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

        public void MouseMoveGeneric(Matrix viewProjection, int deltaX, int deltaY)
        {
            archive.MouseMoveForPosition(viewProjection, deltaX, deltaY);
            archive.MouseMoveForRotation(viewProjection, deltaX);//, deltaY);
            archive.MouseMoveForScale(viewProjection, deltaX, deltaY);
            archive.MouseMoveForPositionLocal(viewProjection, deltaX, deltaY);
        }

        public void SetSelectedIndices(List<uint> assetIDs, bool newlyAddedObjects, bool add = false)
        {
            if (assetIDs.Contains(0) && !add)
            {
                listViewAssets.SelectedIndices.Clear();
                ArchiveEditorFunctions.UpdateGizmoPosition();
                return;
            }

            foreach (uint u in assetIDs)
                if (!archive.ContainsAsset(u))
                {
                    listViewAssets.SelectedIndices.Clear();
                    listViewAssets.EndUpdate();
                    return;
                }

            if (add)
                assetIDs.AddRange(CurrentlySelectedAssetIDs());

            AssetType assetType = AssetType.Null;

            foreach (uint u in assetIDs)
            {
                assetType = archive.GetFromAssetID(u).AHDR.assetType;
                if (archive.GetLayerFromAssetID(u) != comboBoxLayers.SelectedIndex)
                    comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(u);
                break;
            }

            foreach (uint u in assetIDs)
                if (archive.GetFromAssetID(u).AHDR.assetType != assetType)
                {
                    assetType = AssetType.Null;
                    break;
                }

            if (curType != assetType || newlyAddedObjects)
            {
                if (assetType == AssetType.Null)
                    comboBoxAssetTypes.SelectedIndex = 0;
                else
                    comboBoxAssetTypes.SelectedItem = assetType;

                PopulateAssetList(assetType, true, assetIDs);
                return;
            }
            else
            {
                listViewAssets.SelectedIndices.Clear();
                int last = 0;
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    if (assetIDs.Contains(GetAssetIDFromName(listViewAssets.Items[i].Text)))
                    {
                        listViewAssets.SelectedIndices.Add(i);
                        last = i;
                    }
                listViewAssets.EnsureVisible(last);
            }
        }

        readonly System.Drawing.Color defaultColor;

        private void textBoxFindAsset_TextChanged(object sender, EventArgs e)
        {
            uint assetID = 0;
            try
            {
                textBoxFindAsset.BackColor = defaultColor;
                assetID = AssetIDTypeConverter.AssetIDFromString(textBoxFindAsset.Text);
            }
            catch
            {
                textBoxFindAsset.BackColor = System.Drawing.Color.Red;
            }

            if (assetID != 0 && archive.ContainsAsset(assetID))
                SetSelectedIndices(new List<uint>() { assetID }, false);
            else
                foreach (Asset a in archive.GetAllAssets())
                    if (a.AHDR.ADBG.assetName.ToLower().Contains(textBoxFindAsset.Text.ToLower()) && !a.isSelected)
                    {
                        SetSelectedIndices(new List<uint>() { a.AHDR.assetID }, false);
                        return;
                    }
        }

        private void collapseLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.CollapseLayers();
            PopulateLayerComboBox();
        }

        private ScrollableMessageBox verifyResult;

        private void verifyArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (verifyResult != null)
                if (!verifyResult.IsDisposed)
                    verifyResult.Close();

            verifyResult = new ScrollableMessageBox("Verify results on " + Text, archive.VerifyArchive());
            verifyResult.Show();
        }

        private void applyScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vector3 scale = ApplyScale.GetScale(out bool OKed);

            if (OKed)
            {
                string question = $"Are you sure you want to scale all assets by a factor of [{scale.X}, {scale.Y}, {scale.Z}]? To undo this, you must scale it by a factor of [{1 / scale.X}, {1 / scale.Y}, {1 / scale.Z}], and precision might be lost in the process, resulting in some objects slighly off from their intented placement.";
                DialogResult dialogResult = MessageBox.Show(question, "Apply Scale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                    archive.ApplyScale(scale);
            }
        }

        private void MergeSimilarAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.MergeSimilar();
            comboBoxLayers_SelectedIndexChanged(sender, e);
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

        private void checkedListBoxAssets_KeyDown(object sender, KeyEventArgs e)
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
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Control)
            {
                if (buttonEditAsset.Enabled)
                    buttonEditAsset_Click(null, null);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (buttonRemoveAsset.Enabled)
                    ButtonRemoveAsset_Click(null, null);
            }
        }

        private void checkedListBoxAssets_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                toolStripMenuItem_Add.Enabled = buttonAddAsset.Enabled;
                toolStripMenuItem_Duplicate.Enabled = buttonDuplicate.Enabled;
                toolStripMenuItem_Copy.Enabled = buttonCopy.Enabled;
                toolStripMenuItem_Paste.Enabled = buttonPaste.Enabled;
                toolStripMenuItem_Remove.Enabled = buttonRemoveAsset.Enabled;
                toolStripMenuItem_View.Enabled = buttonView.Enabled;
                toolStripMenuItem_Export.Enabled = buttonExportRaw.Enabled;
                toolStripMenuItem_EditHeader.Enabled = buttonEditAsset.Enabled;
                toolStripMenuItem_EditData.Enabled = buttonInternalEdit.Enabled;
                addTemplateToolStripMenuItem.Enabled = buttonAddAsset.Enabled;

                contextMenuStrip_ListBoxAssets.Show(listViewAssets.PointToScreen(e.Location));
            }
        }

        private void TemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = ((ToolStripItem)sender).Text;
            foreach (AssetTemplate template in Enum.GetValues(typeof(AssetTemplate)))
            {
                if (text == template.ToString())
                {
                    Vector3 Position = Program.MainForm.renderer.Camera.Position + 3 * Program.MainForm.renderer.Camera.Forward;
                    PlaceTemplate(Position, template);
                    return;
                }
            }

            MessageBox.Show("There was a problem setting your template for placement");
        }

        public void PlaceTemplate(Vector3 position, AssetTemplate template = AssetTemplate.Null)
        {
            if (comboBoxLayers.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a layer to place your asset in!");
                return;
            }

            List<uint> assetIDs = new List<uint>();

            archive.PlaceTemplate(position, comboBoxLayers.SelectedIndex, out bool success, ref assetIDs, template: template);
            
            if (success)
            {
                archive.UnsavedChanges = true;

                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                //PopulateAssetListAndComboBox();

                SetSelectedIndices(assetIDs, true);
            }
        }

        public bool TemplateFocus { get; private set; } = false;

        public void TemplateFocusOff()
        {
            TemplateFocus = false;

            labelTemplateFocus.Text = "Template Focus\nOFF";
            labelTemplateFocus.ForeColor = System.Drawing.Color.Red;
        }

        public void TemplateFocusOn()
        {
            TemplateFocus = true;

            labelTemplateFocus.Text = "Template Focus\nON";
            labelTemplateFocus.ForeColor = System.Drawing.Color.Green;
        }

        private void labelTemplateFocus_Click(object sender, EventArgs e)
        {
            if (TemplateFocus)
            {
                TemplateFocusOff();
            }
            else
            {
                Program.MainForm.ClearTemplateFocus();
                TemplateFocusOn();
            }
        }

        private void hideButtonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideButtonsToolStripMenuItem.Checked = !hideButtonsToolStripMenuItem.Checked;

            buttonAddAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonDuplicate.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonCopy.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonPaste.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonRemoveAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonView.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonEditAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonExportRaw.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonInternalEdit.Visible = !hideButtonsToolStripMenuItem.Checked;

            if (hideButtonsToolStripMenuItem.Checked)
            {
                listViewAssets.Size = new System.Drawing.Size(listViewAssets.Size.Width + 81, listViewAssets.Size.Height);
            }
            else
            {
                listViewAssets.Size = new System.Drawing.Size(listViewAssets.Size.Width - 81, listViewAssets.Size.Height);
            }
        }

        private void listViewAssets_SizeChanged(object sender, EventArgs e)
        {
            columnHeader1.Width = listViewAssets.Width - 28;
        }

        public void SetAllTopMost(bool value)
        {
            TopMost = value;
            archive.SetAllTopMost(value);
        }

        private void exportRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTXD(true);
        }

        private void importRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTXD(true);
        }

        private void exportNoRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTXD(false);
        }

        private void importNoRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTXD(false);
        }

        private void ImportTXD(bool RW3)
        {
            OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };

            if (openTXD.ShowDialog() == DialogResult.OK)
            {
                archive.AddTextureDictionary(openTXD.FileName, RW3);
                PopulateLayerComboBox();
            }
        }

        private void ExportTXD(bool RW3)
        {
            SaveFileDialog saveTXD = new SaveFileDialog() { Filter = "TXD archives|*.txd" };

            if (saveTXD.ShowDialog() == DialogResult.OK)
                archive.ExportTextureDictionary(saveTXD.FileName, RW3);
        }
    }
}
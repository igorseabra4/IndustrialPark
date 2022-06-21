using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace IndustrialPark
{
    public partial class ArchiveEditor : Form
    {
        public static ArchiveEditor Standalone
        {
            get
            {
                SharpRenderer.cubeVertices = new List<Vector3>();
                SharpRenderer.cylinderVertices = new List<Vector3>();
                SharpRenderer.pyramidVertices = new List<Vector3>();
                SharpRenderer.sphereVertices = new List<Vector3>();
                SharpRenderer.planeVertices = new List<Vector3>();
                SharpRenderer.torusVertices = new List<Vector3>();
                SharpRenderer.cubeTriangles = new List<Models.Triangle>();
                SharpRenderer.cylinderTriangles = new List<Models.Triangle>();
                SharpRenderer.pyramidTriangles = new List<Models.Triangle>();
                SharpRenderer.sphereTriangles = new List<Models.Triangle>();
                SharpRenderer.planeTriangles = new List<Models.Triangle>();
                SharpRenderer.torusTriangles = new List<Models.Triangle>();
                HexUIntTypeConverter.Legacy = true;
                var ae = new ArchiveEditor(true);
                ae.Begin(null, Platform.Unknown);
                return ae;
            }
        }

        public ArchiveEditorFunctions archive;

        public ArchiveEditor(bool standalone = false)
        {
            InitializeComponent();
            TopMost = true;

            this.standalone = standalone;

            defaultColor = textBoxFindAsset.BackColor;
            if (standalone)
                checkBoxTemplateFocus.Enabled = false;

            ArchiveEditorFunctions.PopulateTemplateMenusAt(addTemplateToolStripMenuItem, TemplateToolStripMenuItem_Click);
        }

        public void Begin(string filePath, Platform scoobyPlatform)
        {
            archive = new ArchiveEditorFunctions
            {
                standalone = standalone
            };

            textBoxFindAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
            archive.SetTextboxForAutocomplete(textBoxFindAsset);

            if (!string.IsNullOrWhiteSpace(filePath))
                OpenFile(filePath, scoobyPlatform);

            _updateFilesizeStatusBarItem();
        }

        private readonly bool standalone = false;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!standalone)
            {
                if (e.CloseReason == CloseReason.WindowsShutDown) return;
                if (e.CloseReason == CloseReason.FormOwnerClosing) return;

                e.Cancel = true;
                Hide();
            }
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
                archive.autoCompleteSource.Clear();
                EnableToolStripMenuItems();

                PopulateLayerTypeComboBox();
                PopulateLayerComboBox();
                PopulateAssetList();
                SetupAssetVisibilityButtons();
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
                bool shouldOpenFile = true;

                if (!standalone)
                {
                    foreach (var archiveEditor in Program.MainForm.archiveEditors)
                    {
                        if (Path.GetFileName(archiveEditor.GetCurrentlyOpenFileName()) ==
                            Path.GetFileName(openFile.FileName))
                        {
                            var result = MessageBox.Show(
                                            $"A file named {Path.GetFileName(openFile.FileName)} is already open. Would you still like to open it?",
                                            "Duplicate file detected",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Warning);

                            if (result != DialogResult.Yes)
                            {
                                shouldOpenFile = false;
                            }
                        }
                    }
                }

                if (shouldOpenFile)
                {
                    OpenFile(openFile.FileName);
                }
                _updateFilesizeStatusBarItem();
            }
        }

        /// <summary>
        /// Opens a HIP file from the specified path.
        /// </summary>
        /// <param name="fileName">The HIP file to open</param>
        /// <param name="scoobyPlatform">The </param>
        private void OpenFile(string fileName, Platform scoobyPlatform = Platform.Unknown)
        {
            archive.autoCompleteSource.Clear();
            //new Thread(() =>
            //{
            archive.OpenFile(fileName, true, scoobyPlatform, out string[] autoComplete);
            //    Invoke(new Action(() =>
            //   {
            archive.autoCompleteSource.AddRange(autoComplete);
            OpenFileDone(fileName);
            //   }));
            //}).Start();
        }

        private void OpenFileDone(string fileName)
        {
            toolStripStatusLabelCurrentFilename.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            archive.UnsavedChanges = false;

            EnableToolStripMenuItems();

            PopulateLayerTypeComboBox();
            PopulateLayerComboBox();
            PopulateAssetList();

            SetupAssetVisibilityButtons();

            if (!standalone)
            {
                Program.MainForm.SetToolStripItemName(this, Text);
                Show();
            }
        }

        private void SetupAssetVisibilityButtons()
        {
            if (!standalone)
                Program.MainForm.SetupAssetVisibilityButtons();
        }

        private void EnableToolStripMenuItems()
        {
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            hipHopToolExportToolStripMenuItem.Enabled = true;
            exportAudioToolStripMenuItem.Enabled = true;
            importHIPArchiveToolStripMenuItem.Enabled = true;
            tXDArchiveToolStripMenuItem.Enabled = true;
            buttonAddLayer.Enabled = true;
            editPACKToolStripMenuItem.Enabled = true;
            collapseLayersToolStripMenuItem.Enabled = true;
            mergeSimilarAssetsToolStripMenuItem.Enabled = true;
            verifyArchiveToolStripMenuItem.Enabled = true;
            applyScaleToolStripMenuItem.Enabled = true;
        }

        private void PopulateLayerTypeComboBox()
        {
            programIsChangingStuff = true;

            comboBoxLayerTypes.Items.Clear();
            if (archive.game == Game.Incredibles)
                comboBoxLayerTypes.Items.AddRange(Enum.GetValues(typeof(LayerType_TSSM)).Cast<object>().ToArray());
            else
                comboBoxLayerTypes.Items.AddRange(Enum.GetValues(typeof(LayerType_BFBB)).Cast<object>().ToArray());

            programIsChangingStuff = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Updates the status bar to display the archive's filsize.
        /// </summary>
        private void _updateFilesizeStatusBarItem()
        {
            if (GetCurrentlyOpenFileName() == "Empty")
            {
                return;
            }

            long filesizeBytes = new FileInfo(GetCurrentlyOpenFileName()).Length;

            if (filesizeBytes < 1000) // 1 B - 999 B
            {
                toolStripStatusLabelFileSize.Text = filesizeBytes + " B";
            } else if (filesizeBytes < 1_000_000) // 1 kB - 999 kB
            {
                toolStripStatusLabelFileSize.Text = Math.Round(filesizeBytes / 1000.0, 2) + " kB";
            }
            else // 1+ MB
            {
                toolStripStatusLabelFileSize.Text = Math.Round(filesizeBytes / 1_000_000.0, 2) + " MB";
            }
        }

        public void Save()
        {
            Program.StopSound();

            if (archive.currentlyOpenFilePath == null)
                saveAsToolStripMenuItem_Click(null, null);
            else
                new Thread(archive.Save).Start();

            _updateFilesizeStatusBarItem();
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
                if (!standalone)
                    Program.MainForm.SetToolStripItemName(this, Text);
                toolStripStatusLabelCurrentFilename.Text = "File: " + saveFileDialog.FileName;
                archive.UnsavedChanges = false;
            }

            _updateFilesizeStatusBarItem();
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

        public event Action EditorClosed;

        protected virtual void OnEditorClosed()
        {
            EditorClosed?.Invoke();
        }

        public void CloseArchiveEditor()
        {
            if (verifyResult != null)
                if (!verifyResult.IsDisposed)
                    verifyResult.Close();

            archive.autoCompleteSource.Clear();
            archive.Dispose();

            if (!standalone)
            {
                Program.MainForm.CloseArchiveEditor(this);
                Program.MainForm.UpdateTitleBar();
            }
            Close();
            OnEditorClosed();
        }

        private bool programIsChangingStuff = false;

        public bool HasAsset(uint assetID)
        {
            return archive.ContainsAsset(assetID);
        }

        public string GetAssetNameFromID(uint assetID)
        {
            return archive.GetFromAssetID(assetID).assetName;
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
            for (int i = 0; i < archive.LayerCount; i++)
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
                importTexturesToolStripMenuItem.Enabled = false;
                addTemplateToolStripMenuItem.Enabled = false;
            }
            else
            {
                if (archive.game == Game.Incredibles)
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
                importTexturesToolStripMenuItem.Enabled = true;
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
                comboBoxLayers.Items.Add(archive.LayerToString(archive.LayerCount - 1));
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
            int cnt = archive.GetAssetIDsOnLayer(comboBoxLayers.SelectedIndex).Count;
            if (cnt > 0 &&
                MessageBox.Show($"Are you sure you want to delete this layer with {cnt} assets?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

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
                importTexturesToolStripMenuItem.Enabled = false;
                addTemplateToolStripMenuItem.Enabled = false;

                buttonCopy.Enabled = false;
                buttonDuplicate.Enabled = false;
                buttonRemoveAsset.Enabled = false;
                buttonExportRaw.Enabled = false;
                buttonInternalEdit.Enabled = false;
                buttonMultiEdit.Enabled = false;
            }
            SetupAssetVisibilityButtons();
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
            comboBoxAssetTypes.Items.AddRange(archive.AssetTypesOnLayer(comboBoxLayers.SelectedIndex).Cast<object>().OrderBy(f => f.ToString()).ToArray());

            comboBoxAssetTypes.SelectedIndex = 0;
            PopulateAssetList();

            programIsChangingStuff = false;
        }

        AssetType curType = AssetType.Null;

        private void PopulateAssetList(AssetType type = AssetType.Null, List<uint> assetIDs = null, bool select = false, List<uint> selectionAssetIDs = null)
        {
            curType = type;
            listViewAssets.BeginUpdate();
            listViewAssets.Items.Clear();

            if (comboBoxLayers.SelectedItem != null)
            {
                if (assetIDs == null)
                {
                    assetIDs = archive.GetAssetIDsOnLayer(comboBoxLayers.SelectedIndex);
                }

                List<ListViewItem> items = new List<ListViewItem>(assetIDs.Count());

                for (int i = 0; i < assetIDs.Count(); i++)
                {
                    Asset asset = archive.GetFromAssetID(assetIDs[i]);
                    if (type == AssetType.Null || asset.assetType == type)
                        items.Add(ListViewItemFromAsset(asset, (select == true) && selectionAssetIDs.Contains(asset.assetID)));
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

        private ListViewItem ListViewItemFromAsset(Asset asset, bool selected)
        {
            ListViewItem item = new ListViewItem(asset.assetName)
            {
                Checked = !asset.isInvisible,
                Selected = selected
            };
            item.SubItems.AddRange(new ListViewItem.ListViewSubItem[]
            {
                new ListViewItem.ListViewSubItem(item, asset.assetID.ToString("X8")),
                new ListViewItem.ListViewSubItem(item, asset.assetType.ToString()),
                new ListViewItem.ListViewSubItem(item, asset.AssetInfo)
            });
            return item;
        }

        private void checkedListBoxAssets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            archive.GetFromAssetID(GetAssetIDFromName(listViewAssets.Items[e.Index])).isInvisible = e.NewValue != CheckState.Checked;
        }

        private class AssetListViewSorter : System.Collections.IComparer
        {
            public int Column { get; set; }
            public bool reverseSorting { get; set; }

            public AssetListViewSorter(int Column, bool reverseSorting)
            {
                this.Column = Column;
                this.reverseSorting = reverseSorting;
            }

            public int Compare(ListViewItem x, ListViewItem y)
            {
                if (Column == 1)
                    return GetAssetIDFromName(x).CompareTo(GetAssetIDFromName(y));

                if (Column == 3)
                {
                    if (x.SubItems[Column].Text.EndsWith(" bytes") && y.SubItems[Column].Text.EndsWith(" bytes"))
                    {
                        int bytes1 = Convert.ToInt32(x.SubItems[Column].Text.Replace(" bytes", ""));
                        int bytes2 = Convert.ToInt32(y.SubItems[Column].Text.Replace(" bytes", ""));

                        return bytes1.CompareTo(bytes2);
                    }
                }

                return x.SubItems[Column].Text.CompareTo(y.SubItems[Column].Text);
            }

            public int Compare(object x, object y)
            {
                return Compare((ListViewItem)x, (ListViewItem)y) * (reverseSorting ? -1 : 1);
            }
        }

        private bool reverseSorting = false;
        private int prevColumn = 0;

        private void listViewAssets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == prevColumn)
                reverseSorting = !reverseSorting;
            else
                reverseSorting = false;
            prevColumn = e.Column;

            listViewAssets.ListViewItemSorter = new AssetListViewSorter(e.Column, reverseSorting);
            listViewAssets.Sort();
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
                PopulateAssetList(comboBoxAssetTypes.SelectedIndex == 0 ? AssetType.Null : (AssetType)comboBoxAssetTypes.SelectedItem);
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            uint? assetID = archive.CreateNewAsset(comboBoxLayers.SelectedIndex);

            if (assetID.HasValue)
            {
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                SetSelectedIndices(new List<uint>() { assetID.Value }, true);
                SetupAssetVisibilityButtons();
            }
        }

        private void importMultipleAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (List<Section_AHDR> AHDRs, bool overwrite) = AddMultipleAssets.GetAssets();

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(comboBoxLayers.SelectedIndex, AHDRs, overwrite);
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                Program.MainForm.RefreshTexturesAndModels();
                SetSelectedIndices(assetIDs, true);
                SetupAssetVisibilityButtons();
            }
        }

        private void importModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (List<Section_AHDR> AHDRs, bool overwrite, bool makeSimps, bool ledgeGrabSimps, bool piptVcolors, bool solidSimps) = ImportModel.GetModels(archive.game);

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(comboBoxLayers.SelectedIndex, AHDRs, overwrite);
                if (piptVcolors)
                    archive.MakePiptVcolors(assetIDs);
                if (makeSimps)
                    assetIDs.AddRange(archive.MakeSimps(assetIDs, solidSimps, ledgeGrabSimps));
                PopulateLayerComboBox();
                Program.MainForm.RefreshTexturesAndModels();
                SetupAssetVisibilityButtons();
                SetSelectedIndices(assetIDs, true);
            }
        }

        private void ImportTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var (AHDRs, overwrite) = ImportTextures.GetAssets(archive.game, archive.platform);

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(comboBoxLayers.SelectedIndex, AHDRs, overwrite);
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                Program.MainForm.RefreshTexturesAndModels();
                SetupAssetVisibilityButtons();
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
            SetupAssetVisibilityButtons();
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
            listViewAssets.BeginUpdate();
            foreach (ListViewItem v in listViewAssets.SelectedItems)
                listViewAssets.Items.Remove(v);
            listViewAssets.EndUpdate();
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
            SetupAssetVisibilityButtons();
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0 || standalone) return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is AssetCAM cam)
                Program.MainForm.renderer.Camera.SetPositionCamera(cam);
            else if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset a)
                Program.MainForm.renderer.Camera.SetPosition(a.GetBoundingBox().Center - (10 + a.GetBoundingBox().Size) * Program.MainForm.renderer.Camera.Forward);
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint oldAssetID = CurrentlySelectedAssetIDs()[0];

                var asset = archive.GetFromAssetID(oldAssetID);

                Section_AHDR AHDR = AssetHeader.GetAsset(asset.BuildAHDR());

                if (AHDR != null)
                {
                    archive.UnsavedChanges = true;

                    archive.RemoveAsset(oldAssetID, false);

                    while (archive.ContainsAsset(AHDR.assetID))
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID:X8}]. Will change it to [{++AHDR.assetID:X8}].");

                    archive.AddAsset(comboBoxLayers.SelectedIndex, AHDR, asset.game, asset.endianness, true);

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

        private void buttonMultiEdit_Click(object sender, EventArgs e)
        {
            archive.OpenInternalEditorMulti(archive.GetCurrentlySelectedAssetIDs());
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
                    FileName = archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]).assetName
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    try
                    {
                        var AHDR = archive.GetAHDRFromAssetID(CurrentlySelectedAssetIDs()[0]);
                        File.WriteAllBytes(saveFileDialog.FileName, AHDR.data);
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
                            var AHDR = archive.GetAHDRFromAssetID(u);
                            File.WriteAllBytes(saveFileDialog.FileName + "/" + AHDR.ADBG.assetName, AHDR.data);
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
                list.Add(GetAssetIDFromName(v));
            return list;
        }

        private static uint GetAssetIDFromName(ListViewItem v)
        {
            return Convert.ToUInt32(v.SubItems[1].Text, 16);
        }

        private void checkedListBoxAssets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff)
                return;

            toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";

            List<uint> selected = new List<uint>(listViewAssets.SelectedItems.Count);
            foreach (ListViewItem v in listViewAssets.SelectedItems)
                selected.Add(GetAssetIDFromName(v));

            archive.SelectAssets(selected);

            if (listViewAssets.SelectedItems.Count == 0)
            {
                buttonCopy.Enabled = false;
                buttonDuplicate.Enabled = false;
                buttonRemoveAsset.Enabled = false;
                buttonExportRaw.Enabled = false;
                buttonInternalEdit.Enabled = false;
                buttonMultiEdit.Enabled = false;
            }
            else
            {
                buttonCopy.Enabled = true;
                buttonDuplicate.Enabled = true;
                buttonRemoveAsset.Enabled = true;
                buttonExportRaw.Enabled = true;
                buttonInternalEdit.Enabled = true;
                buttonMultiEdit.Enabled = true;
            }

            if (listViewAssets.SelectedItems.Count == 1)
            {
                buttonEditAsset.Enabled = true;
                buttonView.Enabled = archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset;
            }
            else
            {
                buttonEditAsset.Enabled = false;
                buttonView.Enabled = false;
            }
        }

        public void MouseMoveGeneric(Matrix viewProjection, int deltaX, int deltaY, bool grid)
        {
            archive.MouseMoveForPosition(viewProjection, deltaX, deltaY, grid);
            archive.MouseMoveForRotation(viewProjection, deltaX, grid);//, deltaY);
            archive.MouseMoveForScale(viewProjection, deltaX, deltaY, grid);
            archive.MouseMoveForPositionLocal(viewProjection, deltaX, deltaY, grid);
        }

        public void SetSelectedIndices(List<uint> assetIDs, bool newlyAddedObjects, bool add = false)
        {
            if (assetIDs.Contains(0) && !add)
            {
                listViewAssets.SelectedIndices.Clear();
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
                assetType = archive.GetFromAssetID(u).assetType;
                if (archive.GetLayerFromAssetID(u) != comboBoxLayers.SelectedIndex)
                    comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(u);
                break;
            }

            foreach (uint u in assetIDs)
                if (archive.GetFromAssetID(u).assetType != assetType)
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

                PopulateAssetList(assetType, null, true, assetIDs);
                return;
            }
            else
            {
                listViewAssets.SelectedIndices.Clear();
                int last = 0;
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    if (assetIDs.Contains(GetAssetIDFromName(listViewAssets.Items[i])))
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
                assetID = HexUIntTypeConverter.AssetIDFromString(textBoxFindAsset.Text);
            }
            catch
            {
                textBoxFindAsset.BackColor = System.Drawing.Color.Red;
            }

            var assetIDs = new List<uint>();

            if (assetID != 0 && archive.ContainsAsset(assetID))
                assetIDs.Add(assetID);
            else
                foreach (Asset a in archive.GetAllAssets())
                    if (a.assetName.ToLower().Contains(textBoxFindAsset.Text.ToLower()))
                        assetIDs.Add(a.assetID);

            foreach (uint u in assetIDs)
            {
                if (archive.GetLayerFromAssetID(u) != comboBoxLayers.SelectedIndex || comboBoxLayers.SelectedIndex == -1)
                    comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(u);
                break;
            }

            if (comboBoxAssetTypes.Items.Count > 0)
                comboBoxAssetTypes.SelectedIndex = 0;
            PopulateAssetList(AssetType.Null, assetIDs);
        }

        private void EditPACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.EditPack())
            {
                PopulateLayerTypeComboBox();
                PopulateLayerComboBox();
                PopulateAssetList();
                archive.SetupTextureDisplay();
                SetupAssetVisibilityButtons();
            }
        }

        private void collapseLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.CollapseLayers();
            PopulateLayerComboBox();
            comboBoxLayers.SelectedIndex = -1;
            comboBoxLayers_SelectedIndexChanged(sender, e);
            buttonDuplicate.Enabled = false;
            buttonCopy.Enabled = false;
            buttonRemoveAsset.Enabled = false;
            buttonExportRaw.Enabled = false;
            buttonEditAsset.Enabled = false;
            buttonInternalEdit.Enabled = false;
            buttonMultiEdit.Enabled = false;
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
            var scale = ApplyScale.GetScale();

            if (scale.HasValue)
            {
                string question = $"Are you sure you want to scale all assets by a factor of [{scale.Value.X}, {scale.Value.Y}, {scale.Value.Z}]? " +
                    $"To undo this, you must scale it by a factor of [{1 / scale.Value.X}, {1 / scale.Value.Y}, {1 / scale.Value.Z}], and precision might be lost in the process, resulting in some objects slighly off from their intented placement.";
                DialogResult dialogResult = MessageBox.Show(question, "Apply Scale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                    archive.ApplyScale(scale.Value);
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
            {
                Filter = "All supported file types|*.hip;*.hop;*.ini|HIP archives|*.hip|HOP archives|*.hop|HipHopTool INI|*.ini",
                Multiselect = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                archive.ImportHip(openFile.FileNames, false);
                SetupAssetVisibilityButtons();
            }
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
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                listViewAssets.BeginUpdate();
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    listViewAssets.Items[i].Selected = true;
                listViewAssets.EndUpdate();
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
                toolStripMenuItem_MultiEdit.Enabled = buttonMultiEdit.Enabled;

                addTemplateToolStripMenuItem.Enabled = buttonAddAsset.Enabled;
                ToolStripMenuItem_CreateGroup.Enabled = buttonAddAsset.Enabled;

                contextMenuStrip_ListBoxAssets.Show(listViewAssets.PointToScreen(e.Location));
            }
        }

        private void TemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var template = (AssetTemplate)((ToolStripItem)sender).Tag;
            Vector3 Position = standalone ? new Vector3() : (Program.MainForm.renderer.Camera.Position + 3 * Program.MainForm.renderer.Camera.Forward);
            PlaceTemplate(Position, template);
            SetupAssetVisibilityButtons();
        }

        public List<uint> PlaceTemplate(Vector3 position, AssetTemplate template = AssetTemplate.Null)
        {
            if (comboBoxLayers.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a layer to place your asset in!");
                return null;
            }

            List<uint> assetIDs = new List<uint>();

            archive.PlaceTemplate(position, comboBoxLayers.SelectedIndex, ref assetIDs, template: template);

            if (assetIDs.Count != 0)
            {
                archive.UnsavedChanges = true;
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(comboBoxLayers.SelectedIndex);
                SetSelectedIndices(assetIDs, true);
            }

            return assetIDs;
        }

        private void ToolStripMenuItem_CreateGroup_Click(object sender, EventArgs e)
        {
            var assetIDs = PlaceTemplate(new Vector3(), AssetTemplate.Group);
            if (assetIDs != null && assetIDs.Count > 0)
                ((AssetGRUP)archive.GetFromAssetID(assetIDs[0])).AddItems(CurrentlySelectedAssetIDs());
            SetupAssetVisibilityButtons();
        }

        public bool TemplateFocus => checkBoxTemplateFocus.Checked;

        public void TemplateFocusOff()
        {
            checkBoxTemplateFocus.Checked = false;
        }

        private void checkBoxTemplateFocus_Click(object sender, EventArgs e)
        {
            if (!standalone)
                Program.MainForm.ClearTemplateFocus();
            checkBoxTemplateFocus.Checked = true;
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
            buttonMultiEdit.Visible = !hideButtonsToolStripMenuItem.Checked;

            if (hideButtonsToolStripMenuItem.Checked)
            {
                listViewAssets.Size = new System.Drawing.Size(listViewAssets.Size.Width + 81, listViewAssets.Size.Height);
            }
            else
            {
                listViewAssets.Size = new System.Drawing.Size(listViewAssets.Size.Width - 81, listViewAssets.Size.Height);
            }
        }

        public void SetAllTopMost(bool value)
        {
            TopMost = value;
            archive.SetAllTopMost(value);
        }

        private void exportRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTXD(CheckState.Checked);
        }

        private void importRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportTXD(true);
        }

        private void exportNoRW3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTXD(CheckState.Unchecked);
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
                archive.ImportTextureDictionary(openTXD.FileName, RW3);
                Program.MainForm.RefreshTexturesAndModels();
                PopulateLayerComboBox();
                SetupAssetVisibilityButtons();
            }
        }

        private void ExportTXD(CheckState RW3)
        {
            SaveFileDialog saveTXD = new SaveFileDialog() { Filter = "TXD archives|*.txd" };

            if (saveTXD.ShowDialog() == DialogResult.OK)
                archive.ExportTextureDictionary(saveTXD.FileName, RW3);
        }

        internal void RefreshHop(SharpRenderer renderer)
        {
            archive.ResetModels(renderer);
            archive.SetupTextureDisplay();
            archive.RecalculateAllMatrices();
        }

        private void exportAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!archive.ContainsAssetWithType(AssetType.Sound) && !archive.ContainsAssetWithType(AssetType.StreamingSound))
                return;

            CommonOpenFileDialog saveFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                foreach (var asset in archive.GetAllAssets().OfType<AssetWithData>().Where(asset => asset.assetType == AssetType.Sound || asset.assetType == AssetType.StreamingSound))
                    try
                    {
                        string extension =
                            (archive.platform == Platform.GameCube && asset.game != Game.Incredibles) ? ".DSP" :
                            (archive.platform == Platform.Xbox) ? ".WAV" :
                            (archive.platform == Platform.PS2) ? ".VAG" :
                            "";

                        string filename = (extension != "" && asset.assetName.ToLower().Contains(extension.ToLower())) ?
                            asset.assetName :
                            asset.assetName + extension;

                        File.WriteAllBytes(Path.Combine(saveFileDialog.FileName, filename),
                            archive.GetSoundData(asset.assetID, asset.Data));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to export " + asset + ": " + ex.Message);
                    }
            }
        }

        private HashSet<Keys> PressedKeys = new HashSet<Keys>();

        private void ArchiveEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PressedKeys.Contains(e.KeyCode))
                PressedKeys.Add(e.KeyCode);

            if (PressedKeys.Contains(Keys.ControlKey) 
                && PressedKeys.Contains(Keys.S))
            {
                Save();
            }

            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.N))
            {
                newToolStripMenuItem_Click(sender, e);
            }

            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.O))
            {
                openToolStripMenuItem_Click(sender, e);
            }

            if (PressedKeys.Contains(Keys.ControlKey)
                && PressedKeys.Contains(Keys.W))
            {
                closeToolStripMenuItem_Click(sender, e);
            }
        }

        private void ArchiveEditor_KeyUp(object sender, KeyEventArgs e)
        {
            PressedKeys.Remove(e.KeyCode);
        }

        private void ArchiveEditor_Deactivate(object sender, EventArgs e)
        {
            PressedKeys.Clear();
        }

        /// <summary>
        /// Given the filepath to a hip file, returns an integer representing the game the file is for, 
        /// based on the filename of the configuration ini.
        /// <para>
        /// This method checks the same directory, and then the parent directory. This assumes 
        /// the filename has not been changed. Note this cannot guarantee which game the file is for.
        /// </para>
        /// </summary>
        /// <param name="filepath">The filepath of the HIP file</param>
        /// <returns>0: Unknown, 1: Scooby, 2: BFBB , 3: TSSM, 4: Incredibles, 5: ROTU, 6: Ratatouille Prototype</returns>
        public static int GetGameFromGameConfigIni(string filepath)
        {
            const string scoobyIniFilename = "sd2.ini";
            const string bfbbIniFilename = "sb.ini";
            const string tssmIniFilename = "sb04.ini";
            const string incrediblesIniFilename = "in.ini";
            const string rotuIniFilename = "in2.ini";
            const string ratProtoIniFilename = "rats.ini";

            // Checks in same directory for a specific ini and then checks the parent directory
            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), scoobyIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, scoobyIniFilename)))
                return 1;

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), bfbbIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, bfbbIniFilename)))
                return 2;

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), tssmIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, tssmIniFilename)))
                return 3;

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), incrediblesIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, incrediblesIniFilename)))
                return 4;

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), rotuIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, rotuIniFilename)))
                return 5;

            if (File.Exists(Path.Combine(Path.GetDirectoryName(filepath), ratProtoIniFilename))
                || File.Exists(Path.Combine(Directory.GetParent(Path.GetDirectoryName(filepath)).FullName, ratProtoIniFilename)))
                return 6;

            return 0;
        }

        /// <summary>
        /// Generates a report for the HIP/HOP archive to a .txt in the same directory.
        /// </summary>
        /// 
        private void _generateReportTxt()
        {
            bool generationWasSuccessful = true;
            bool isDebug;
            var timeOfGeneration = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            string currentFilepath = GetCurrentlyOpenFileName();
            var stopwatch = new Stopwatch();

            // TODO: Change filetype to LIP or LOP
            string filepath = Path.Combine(
                Path.GetDirectoryName(currentFilepath),
                Path.GetFileName(currentFilepath) + "_IP_Report.txt");

            using (StreamWriter output = new StreamWriter(filepath))
            {
                stopwatch.Start();
                output.WriteLine("This report file was generated by Industrial Park.");
                output.WriteLine("Please visit https://github.com/igorseabra4/IndustrialPark for more information.");
                output.WriteLine("---------------------------");
                output.WriteLine("#### START OF REPORT ####\n");
                output.WriteLine($"Industrial Park Version: {new IPversion().version}");
#if DEBUG
                isDebug = true;
#else
                isDebug = false;
#endif
                output.WriteLine($"Is Debug Build: {isDebug}");
                output.WriteLine($"Time of Report Generation: {timeOfGeneration}");

                output.WriteLine("\nFILE INFORMATION");
                output.WriteLine("----------------");
                output.WriteLine($"File path: {currentFilepath}");
                output.WriteLine($"File size: {new FileInfo(currentFilepath).Length} Bytes");
                output.WriteLine($"Platform: {archive.platform}");
                string game = archive.game == Game.Incredibles ? "Incredible/TSSM/ROTU" : archive.game.ToString();
                output.WriteLine($"Game: {game}");
                output.WriteLine($"Number of Layers: {archive.LayerCount}");
                output.WriteLine($"Number of Assets: {archive.AssetCount}");
                output.WriteLine($"Has unsaved changes: {archive.UnsavedChanges}");
                output.WriteLine($"Environment Name: {archive.environmentName}");

                Dictionary<uint, Asset>.ValueCollection allAssets = archive.GetAllAssets();
                Dictionary<AssetType, int> assetCount = new Dictionary<AssetType, int>();

                output.WriteLine("\nTYPE TOTALS");
                output.WriteLine("-------------");

                foreach (AssetType assetType in Enum.GetValues(typeof(AssetType)))
                {
                    assetCount[assetType] = 0;
                }

                foreach (var asset in allAssets)
                {
                    assetCount[asset.assetType] += 1;
                }

                foreach (KeyValuePair<AssetType, int> entry in assetCount)
                {
                    if (entry.Value != 0)
                    {
                        output.WriteLine($"{entry.Key.ToString().PadRight(30, '.')}{entry.Value}");
                    }
                }

                output.WriteLine("\nLAYER INFORMATION");
                output.WriteLine("-------------");

                List<uint> assetIDsOnLayer;
                List<AssetType> assetTypesOnLayer;
                int numOfInvisibleAssets = 0;


                for (int i = 0; i < archive.LayerCount; i++)
                {
                    assetTypesOnLayer = archive.AssetTypesOnLayer(i);
                    output.WriteLine($"{archive.LayerToString(i)}");
                    output.Write($"{assetTypesOnLayer.Count} asset type(s) [");
                    foreach (var assetType in assetTypesOnLayer)
                    {
                        output.Write($"{assetType} ");
                    }
                    output.WriteLine("]");

                    assetIDsOnLayer = archive.GetAssetIDsOnLayer(i);


                    for (int j = 0; j < assetIDsOnLayer.Count(); j++)
                    {
                        Asset asset = archive.GetFromAssetID(assetIDsOnLayer[j]);

                        output.WriteLine($"    {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                        if (asset.isInvisible)
                        {
                            numOfInvisibleAssets++;
                        }
                    }
                    output.WriteLine("");
                    

                }
                output.WriteLine($"Number of invisible assets: {numOfInvisibleAssets}");
                output.WriteLine("");

                output.WriteLine("\nASSET LINKING INFO");
                output.WriteLine("-------------");


                foreach (var asset in allAssets)
                {
                    // If Base asset, get list of links.
                    if (asset is BaseAsset)
                    {
                        Link[] links = ((BaseAsset)asset).Links;
                        if (links.Length > 0)
                        {
                            output.WriteLine($"{asset.assetName}: {links.Length} links");

                            foreach (var link in links) 
                            {
                                string eventSendIDName = "";
                                string eventReceiveIDName = "";

                                if (archive.game == Game.BFBB) // BFBB
                                {
                                    eventSendIDName = ((EventBFBB)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventBFBB)link.EventReceiveID).ToString();
                                } else if (archive.game == Game.Scooby) // Scooby
                                {
                                    // TODO: Scooby events
                                }
                                else if (archive.game == Game.Incredibles) // TSSM/Incredibles/ROTU
                                {
                                    switch (GetGameFromGameConfigIni(GetCurrentlyOpenFileName()))
                                    {
                                        case 3: // TSSM
                                            eventSendIDName = ((EventTSSM)link.EventSendID).ToString();
                                            eventReceiveIDName = ((EventTSSM)link.EventReceiveID).ToString();
                                            break;
                                        case 4: // Incredibles
                                            // TODO: Incredibles events
                                            break;
                                        case 5: // ROTU
                                            // TODO: ROTU events
                                            break;
                                    }
                                }
                                
                                // If event name not supplied, event ID used instead.
                                eventSendIDName = eventSendIDName == "" ? link.EventSendID.ToString() : eventSendIDName;
                                eventReceiveIDName = eventReceiveIDName == "" ? link.EventReceiveID.ToString() : eventReceiveIDName;


                                string targetAssetName = "";
                                try
                                {
                                    // FIXME: Only searches current archive
                                    targetAssetName = archive.GetFromAssetID(link.TargetAsset).assetName;
                                }
                                catch
                                {
                                    targetAssetName = link.TargetAsset.ToString();
                                }

                                output.WriteLine($"  {eventSendIDName} => {eventReceiveIDName} => {targetAssetName}");
                            }
                        }
                    }
                }



                output.WriteLine("\nUNUSED MODELS");
                output.WriteLine("-------------");

                int numOfModels = 0;
                int numOfUnusedModels = 0;

                //foreach(var asset in allAssets)
                //{
                //    if (asset.assetType == AssetType.Model)
                //    {
                //        numOfModels++;

                //        List<uint> whoTargets = archive.FindWhoTargets(asset.assetID);
                //        if (whoTargets.Count == 0)
                //        {
                //            output.WriteLine($"  {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                //            numOfUnusedModels++;
                //        }

                //    }
                //}

                //output.WriteLine($"{numOfUnusedModels} unused models out of {numOfModels} total.");

                output.WriteLine("* Not yet implemented");
                output.WriteLine();
                // Find who Targets Me throws an exception sometimes :(


                output.WriteLine("\n#### END OF REPORT ####");
                stopwatch.Stop();
                output.WriteLine($"Report generated in {Math.Round(stopwatch.Elapsed.TotalMilliseconds,2)} ms.");
            }


            if (generationWasSuccessful)
            {
                if (MessageBox.Show($"Report \"{Path.GetFileName(filepath)}\" generated. Open file?",
                    "Report generated",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(filepath);
                }
            }
            else
            {
                MessageBox.Show("File could not be generated successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _generateReportTxt();
        }
    }
}
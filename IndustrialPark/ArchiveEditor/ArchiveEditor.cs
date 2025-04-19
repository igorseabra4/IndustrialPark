﻿using HipHopFile;
using Microsoft.WindowsAPICodePack.Dialogs;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

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
                if (e.CloseReason == CloseReason.WindowsShutDown)
                    return;
                if (e.CloseReason == CloseReason.FormOwnerClosing)
                    return;

                e.Cancel = true;
                Hide();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    archive.Save();
            }

            if (archive.New())
            {
                archive.autoCompleteSource.Clear();
                SetMenuItemsEnabled();
                SetNoLayers();
                SetupAssetVisibilityButtons();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.UnsavedChanges)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    archive.Save();
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
            //new Thread(() =>
            //{
            archive.OpenFile(fileName, true, scoobyPlatform);
            //    Invoke(new Action(() =>
            //   {
            OpenFileDone(fileName);
            //   }));
            //}).Start();
        }

        private void OpenFileDone(string fileName)
        {
            toolStripStatusLabelCurrentFilename.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);
            archive.UnsavedChanges = false;

            SetMenuItemsEnabled();

            SetNoLayers();

            SetupAssetVisibilityButtons();

            if (!standalone)
            {
                Program.MainForm.SetToolStripItemName(this, Text);
                Program.MainForm.SetRecentOpenedArchives(fileName);
            }
        }

        private void SetupAssetVisibilityButtons()
        {
            if (!standalone)
                Program.MainForm.SetupAssetVisibilityButtons();
        }

        private void SetMenuItemsEnabled()
        {
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            convertArchiveToolStripMenuItem.Enabled = true;
            layersToolStripMenuItem.Enabled = true;
            noLayersToolStripMenuItem.Enabled = true;
            organizeLayersToolStripMenuItem.Enabled = true;
            organizeLegacyToolStripMenuItem.Enabled = true;
            mergeSimilarAssetsToolStripMenuItem.Enabled = true;
            applyScaleToolStripMenuItem.Enabled = true;
            verifyArchiveToolStripMenuItem.Enabled = true;
            hipHopFileToolStripMenuItem.Enabled = true;
            importHipArchiveToolStripMenuItem.Enabled = true;
            importHipArchiveForceOverwriteToolStripMenuItem.Enabled = true;
            exportAssetsIniToolStripMenuItem.Enabled = true;
            texturesToolStripMenuItem.Enabled = true;
            soundsToolStripMenuItem.Enabled = true;
            buttonAddLayer.Enabled = true;
            importSoundsToolStripMenuItem.Enabled = true;
            importRawSoundsToolStripMenuItem.Enabled = true;

            var canImportAssetsToLayer = archive.NoLayers || (archive.SelectedLayerIndex != -1);
            importTexturesToolStripMenuItem.Enabled = canImportAssetsToLayer;
            importRW3ToolStripMenuItem.Enabled = canImportAssetsToLayer;
            importNoRW3ToolStripMenuItem.Enabled = canImportAssetsToLayer;
            importModelsToolStripMenuItem.Enabled = canImportAssetsToLayer;
            importMultipleAssetsToolStripMenuItem.Enabled = canImportAssetsToLayer;

            toolStripMenuItem_Import.Enabled = buttonImportAsset.Enabled = canImportAssetsToLayer;
            toolStripMenuItem_Paste.Enabled = buttonPasteAssets.Enabled = canImportAssetsToLayer;
            addTemplateToolStripMenuItem.Enabled = canImportAssetsToLayer;

            var canExportTextures = archive.ContainsAssetWithType(AssetType.Texture);
            exportAllTexturesToolStripMenuItem.Enabled = canExportTextures;
            exportRW3ToolStripMenuItem.Enabled = canExportTextures;
            exportNoRW3ToolStripMenuItem.Enabled = canExportTextures;

            var canExportSounds = archive.ContainsAssetWithType(AssetType.Sound) || archive.ContainsAssetWithType(AssetType.SoundStream);
            exportAllSoundsToolStripMenuItem.Enabled = canExportSounds;
            exportAllSoundsRawToolStripMenuItem.Enabled = canExportSounds;

            buttonRemoveLayer.Enabled = archive.SelectedLayerIndex != -1;
            buttonArrowUp.Enabled = archive.LayerCount > 1 && archive.SelectedLayerIndex > 0;
            buttonArrowDown.Enabled = archive.LayerCount > 1 && archive.SelectedLayerIndex != -1 && archive.SelectedLayerIndex < archive.LayerCount - 1;

            var canEditAsset = listViewAssets.SelectedItems.Count != 0;
            toolStripMenuItem_Duplicate.Enabled = buttonDuplicateAsset.Enabled = canEditAsset;
            toolStripMenuItem_Copy.Enabled = buttonCopyAsset.Enabled = canEditAsset;
            toolStripMenuItem_Remove.Enabled = buttonRemoveAsset.Enabled = canEditAsset;
            toolStripMenuItem_ExportRaw.Enabled = buttonExportRawAsset.Enabled = canEditAsset;
            toolStripMenuItem_EditData.Enabled = buttonEditDataAsset.Enabled = canEditAsset;
            toolStripMenuItem_MultiEdit.Enabled = buttonMultiEditAsset.Enabled = canEditAsset;
            toolStripMenuItem_CreateGroup.Enabled = canEditAsset;

            var canEditSingleAsset = listViewAssets.SelectedItems.Count == 1;
            toolStripMenuItem_EditHeader.Enabled = buttonEditAsset.Enabled = canEditSingleAsset;
            toolStripMenuItem_View.Enabled = buttonView.Enabled = canEditSingleAsset && archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset;
        }

        private void PopulateLayerTypeComboBox()
        {
            if (archive.NoLayers)
                return;

            programIsChangingStuff = true;

            comboBoxLayerTypes.Items.Clear();
            if (archive.game >= Game.Incredibles)
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

            toolStripStatusLabelFileSize.Text = ConvertSize((int)filesizeBytes);
        }

        public static string ConvertSize(int size)
        {
            if (size <= 1024)
                return $"{size} B";

            string[] name = new string[] { "B", "KiB", "MiB", "GiB" };

            double i = Math.Floor(Math.Log(size, 1024));
            double s = Math.Round(size / Math.Pow(1024, i), 2);

            return $"{s} {name[(int)i]}";
        }

        public void Save()
        {
            SoundUtility_vgmstream.StopSound();

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
                {
                    Program.MainForm.SetToolStripItemName(this, Text);
                    Program.MainForm.SetRecentOpenedArchives(saveFileDialog.FileName);
                }
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

                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                    archive.Save();
            }

            CloseArchiveEditor();
        }

        public event Action EditorUpdate;

        protected virtual void OnEditorUpdate()
        {
            EditorUpdate?.Invoke();
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
            OnEditorUpdate();
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
            return archive.currentlyOpenFilePath;
        }

        private void PopulateLayerComboBox()
        {
            if (archive.NoLayers)
                return;

            programIsChangingStuff = true;

            comboBoxLayers.Items.Clear();
            for (int i = 0; i < archive.LayerCount; i++)
                comboBoxLayers.Items.Add(archive.LayerToString(i));

            programIsChangingStuff = false;
        }

        private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff || archive.NoLayers)
                return;

            programIsChangingStuff = true;

            archive.SelectedLayerIndex = comboBoxLayers.SelectedIndex;

            if (archive.SelectedLayerIndex == -1 && !archive.NoLayers)
            {
                comboBoxLayerTypes.SelectedItem = null;

                comboBoxAssetTypes.Items.Clear();
                comboBoxAssetTypes.SelectedIndex = -1;
                PopulateAssetList();
            }
            else
            {
                if (!archive.NoLayers)
                {
                    if (archive.game >= Game.Incredibles)
                        comboBoxLayerTypes.SelectedItem = (LayerType_TSSM)archive.GetLayerType();
                    else
                        comboBoxLayerTypes.SelectedItem = (LayerType_BFBB)archive.GetLayerType();
                    if (!archive.LegacySave)
                        renameLayerToolStripMenuItem.Enabled = true;
                }
                else
                    renameLayerToolStripMenuItem.Enabled = false;

                PopulateAssetListAndComboBox();
            }

            programIsChangingStuff = false;
        }

        private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programIsChangingStuff || archive.SelectedLayerIndex == -1)
                return;

            archive.SetLayerType((int)comboBoxLayerTypes.SelectedItem);
            comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();
            archive.UnsavedChanges = true;
        }

        private void buttonAddLayer_Click(object sender, EventArgs e)
        {
            try
            {
                archive.AddLayer();
                comboBoxLayers.Items.Add(archive.LayerToString());
                comboBoxLayers.SelectedIndex = archive.SelectedLayerIndex;
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
            int cnt = archive.GetAssetIDsOnLayer().Count;
            if (cnt > 0 &&
                MessageBox.Show($"Are you sure you want to delete this layer with {cnt} assets?", "Warning",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            int previndex = archive.SelectedLayerIndex;

            archive.RemoveLayer();

            PopulateLayerComboBox();

            if (comboBoxLayers.Items.Count > 0)
                comboBoxLayers.SelectedIndex = Math.Max(previndex - 1, 0);
            else
            {
                comboBoxLayers.SelectedItem = null;
                comboBoxLayerTypes.SelectedItem = null;

                comboBoxAssetTypes.Items.Clear();
                listViewAssets.Items.Clear();
            }
            SetMenuItemsEnabled();
            SetupAssetVisibilityButtons();
        }

        private void buttonArrowUp_Click(object sender, EventArgs e)
        {
            int previndex = archive.SelectedLayerIndex;
            archive.MoveLayerUp();
            PopulateLayerComboBox();
            comboBoxLayers.SelectedIndex = Math.Max(previndex - 1, 0);
            SetMenuItemsEnabled();
        }

        private void buttonArrowDown_Click(object sender, EventArgs e)
        {
            int previndex = archive.SelectedLayerIndex;
            archive.MoveLayerDown();
            PopulateLayerComboBox();
            comboBoxLayers.SelectedIndex = Math.Min(previndex + 1, comboBoxLayers.Items.Count - 1);
            SetMenuItemsEnabled();
        }

        public void PopulateAssetListAndComboBox()
        {
            programIsChangingStuff = true;

            comboBoxAssetTypes.Items.Clear();
            if (archive.NoLayers || archive.SelectedLayerIndex != -1)
            {
                comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.Null));
                comboBoxAssetTypes.Items.AddRange(archive.AssetTypesOnLayer().Select(f => new AssetTypeContainer(f)).OrderBy(f => f.ToString()).ToArray());
                comboBoxAssetTypes.SelectedIndex = 0;
            }
            else
                comboBoxAssetTypes.SelectedIndex = -1;

            PopulateAssetList();

            programIsChangingStuff = false;
        }

        private AssetType curType = AssetType.Null;

        private void PopulateAssetList(AssetType type = AssetType.Null, List<uint> assetIDs = null, bool select = false, List<uint> selectionAssetIDs = null)
        {
            curType = type;
            listViewAssets.BeginUpdate();
            listViewAssets.Items.Clear();

            if (archive.NoLayers || comboBoxLayers.SelectedItem != null)
            {
                if (assetIDs == null)
                    assetIDs = archive.GetAssetIDsOnLayer();

                List<ListViewItem> items = new List<ListViewItem>(assetIDs.Count());

                for (int i = 0; i < assetIDs.Count(); i++)
                {
                    Asset asset = archive.GetFromAssetID(assetIDs[i]);
                    if (type == AssetType.Null || asset.assetType == type)
                        items.Add(ListViewItemFromAsset(asset, (select == true) && selectionAssetIDs.Contains(asset.assetID)));
                }
                var itemsArray = items.ToArray();
                listViewAssets.Items.AddRange(itemsArray);
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

            SetMenuItemsEnabled();
        }

        private ListViewItem ListViewItemFromAsset(Asset asset, bool selected)
        {
            ListViewItem item = new ListViewItem(asset.assetName)
            {
                Checked = !asset.isInvisible,
                Selected = selected,
                Tag = asset.assetID
            };

            item.SubItems.AddRange(new ListViewItem.ListViewSubItem[]
            {
                new ListViewItem.ListViewSubItem(item, asset.assetID.ToString("X8")),
                new ListViewItem.ListViewSubItem(item, asset.TypeString),
                new ListViewItem.ListViewSubItem(item, TreatInfo(asset.AssetInfo)),
                new ListViewItem.ListViewSubItem(item, asset.AssetInfoLinks),
            });
            return item;
        }

        private string TreatInfo(string info)
        {
            if (info.Length > 50)
                info = info.Substring(0, 47) + "...";
            return info;
        }

        private void UpdateCurrentListView(Asset asset)
        {
            foreach (ListViewItem item in listViewAssets.Items)
            {
                if ((uint)item.Tag == asset.assetID)
                {
                    item.SubItems[3].Text = TreatInfo(asset.AssetInfo);
                    item.SubItems[4].Text = asset.AssetInfoLinks;
                    return;
                }
            }
        }

        private void checkedListBoxAssets_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            archive.GetFromAssetID((uint)listViewAssets.Items[e.Index].Tag).isInvisible = e.NewValue != CheckState.Checked;
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
                    return ((uint)x.Tag).CompareTo((uint)y.Tag);

                if (Column == 4)
                {
                    int links1 = (x.SubItems[Column].Text == "-") ? -1 : Convert.ToInt32(x.SubItems[Column].Text);
                    int links2 = (y.SubItems[Column].Text == "-") ? -1 : Convert.ToInt32(y.SubItems[Column].Text);
                    return links1.CompareTo(links2);
                }

                return x.SubItems[Column].Text.CompareTo(y.SubItems[Column].Text);
            }

            public int Compare(object x, object y)
            {
                var comp = Compare((ListViewItem)x, (ListViewItem)y);
                if (reverseSorting)
                    return -comp;
                return comp;
            }
        }

        private bool reverseSorting = false;
        private int prevColumn = -1;

        private void listViewAssets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            reverseSorting = (e.Column == prevColumn) && !reverseSorting;

            prevColumn = e.Column;

            listViewAssets.ListViewItemSorter = new AssetListViewSorter(e.Column, reverseSorting);
            listViewAssets.Sort();

            foreach (ColumnHeader col in listViewAssets.Columns)
                col.Text = TreatColumnName(col.Text);
            listViewAssets.Columns[prevColumn].Text = TreatColumnName(listViewAssets.Columns[prevColumn].Text) + (reverseSorting ? " ▼" : " ▲");
        }

        private string TreatColumnName(string name)
        {
            if (name.EndsWith("▲") || name.EndsWith("▼"))
                return name.Substring(0, name.Length - 2);
            return name;
        }

        private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!programIsChangingStuff)
                PopulateAssetList(((AssetTypeContainer)comboBoxAssetTypes.SelectedItem).assetType);
        }

        private void buttonAddAsset_Click(object sender, EventArgs e)
        {
            uint? assetID = archive.CreateNewAsset();

            if (assetID.HasValue)
            {
                if (!archive.NoLayers)
                    comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();
                SetSelectedIndex(assetID.Value, true);
                SetupAssetVisibilityButtons();
                SetMenuItemsEnabled();
            }
        }

        private void importMultipleAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (List<Section_AHDR> AHDRs, bool overwrite) = AddMultipleAssets.GetAssets();

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(AHDRs, overwrite);
                if (!archive.NoLayers)
                    comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();
                OnEditorUpdate();
                SetSelectedIndices(assetIDs, true);
                SetMenuItemsEnabled();
            }
        }

        private void importModelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (List<Section_AHDR> AHDRs,
                bool overwrite, 
                bool makeSimps, 
                bool ledgeGrabSimps, 
                bool piptVcolors, 
                bool solidSimps, 
                bool jsp, 
                bool placeOnExistingDefaultLayer) = ImportModel.GetModels(archive.game, archive.NoLayers);

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(AHDRs, overwrite);
                if (piptVcolors)
                    archive.MakePiptVcolors(assetIDs);
                if (makeSimps)
                    assetIDs.AddRange(archive.MakeSimps(assetIDs, solidSimps, ledgeGrabSimps, placeOnExistingDefaultLayer));
                PopulateLayerComboBox();
                OnEditorUpdate();
                SetSelectedIndices(assetIDs, true);
                SetMenuItemsEnabled();
            }
        }

        private void importTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var (AHDRs, overwrite) = ImportTextures.GetAssets(archive.game, archive.platform);

            if (AHDRs != null)
            {
                List<uint> assetIDs = archive.ImportMultipleAssets(AHDRs, overwrite);
                if (!archive.NoLayers)
                    comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString();
                OnEditorUpdate();
                SetSelectedIndices(assetIDs, true);
                SetMenuItemsEnabled();
            }
            Enabled = true;
        }

        private void exportAllTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog saveFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var textures = archive.GetAllTexturesAsBitmaps();

                RenderWareFile.ReadFileMethods.treatStuffAsByteArray = false;

                foreach ((string textureName, Bitmap texture) in textures)
                    texture.Save(Path.Combine(saveFileDialog.FileName, textureName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void buttonDuplicate_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0)
                return;

            archive.DuplicateSelectedAssets(out List<uint> finalIndices);

            if (!archive.NoLayers)
                comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();
            SetSelectedIndices(finalIndices, true);
            SetMenuItemsEnabled();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0)
                return;

            archive.CopyAssetsToClipboard();
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            if (!archive.PasteAssetsFromClipboard(out List<uint> finalIndices))
                return;

            if (!archive.NoLayers)
                comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();

            PopulateAssetListAndComboBox();
            SetSelectedIndices(finalIndices, true);
            SetupAssetVisibilityButtons();
        }

        private void ButtonRemoveAsset_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0)
                return;

            programIsChangingStuff = true;

            AssetType a = AssetType.Null;
            if (comboBoxAssetTypes.SelectedIndex > 0)
                a = ((AssetTypeContainer)comboBoxAssetTypes.SelectedItem).assetType;
            var prevIndex = listViewAssets.SelectedIndices[0];

            archive.RemoveAsset(CurrentlySelectedAssetIDs());

            if (!archive.NoLayers)
                comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();

            archive.UnsavedChanges = true;
            listViewAssets.BeginUpdate();
            foreach (ListViewItem v in listViewAssets.SelectedItems)
                listViewAssets.Items.Remove(v);
            listViewAssets.EndUpdate();
            programIsChangingStuff = false;

            if (a != curType)
                SelectAssetTypeOnBox(a);
            else
            {
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    listViewAssets.Items[i].Selected = false;

                if (listViewAssets.Items.Count > 0)
                    try
                    { listViewAssets.Items[prevIndex].Selected = true; }
                    catch
                    {
                        try
                        { listViewAssets.Items[prevIndex - 1].Selected = true; }
                        catch { }
                    }
            }

            if (listViewAssets.Items.Count == 0)
                PopulateAssetListAndComboBox();
            else
                SetMenuItemsEnabled();
            SetupAssetVisibilityButtons();
        }

        private void buttonView_Click(object sender, EventArgs e)
        {
            if (listViewAssets.SelectedItems.Count == 0 || standalone)
                return;

            if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is AssetCAM cam)
                Program.Renderer.Camera.SetPositionCamera(cam);
            else if (archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]) is IClickableAsset a)
                Program.Renderer.Camera.SetPosition(a.GetBoundingBox().Center - (10 + a.GetBoundingBox().Size) * Program.MainForm.renderer.Camera.Forward);
        }

        private void buttonEditAsset_Click(object sender, EventArgs e)
        {
            try
            {
                uint oldAssetID = CurrentlySelectedAssetIDs()[0];

                var asset = archive.GetFromAssetID(oldAssetID);
                var oldLayer = archive.GetLayerFromAssetID(oldAssetID);

                Section_AHDR AHDR = AssetHeader.GetAsset(asset.BuildAHDR(archive.platform.Endianness()));

                if (AHDR != null)
                {
                    archive.UnsavedChanges = true;

                    archive.RemoveAsset(oldAssetID, false);

                    while (archive.ContainsAsset(AHDR.assetID))
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID:X8}]. Will change it to [{++AHDR.assetID:X8}].");

                    archive.AddAsset(AHDR, asset.game, archive.platform.Endianness(), true, oldLayer);

                    if (ArchiveEditorFunctions.updateReferencesOnCopy)
                    {
                        if (standalone)
                            archive.ReplaceReferences(oldAssetID, AHDR.assetID);
                        else
                            foreach (var ae in Program.MainForm.archiveEditors)
                                ae.archive.ReplaceReferences(oldAssetID, AHDR.assetID);
                    }

                    PopulateAssetListAndComboBox();
                    SetSelectedIndex(AHDR.assetID, true);
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
            archive.OpenInternalEditorMulti(archive.GetCurrentlySelectedAssetIDs(), UpdateCurrentListView);
        }

        public void OpenInternalEditors()
        {
            archive.OpenInternalEditor(archive.GetCurrentlySelectedAssetIDs(), false, UpdateCurrentListView);
        }

        public void OpenInternalEditors(List<uint> whoTargets, bool openAnyway)
        {
            archive.OpenInternalEditor(whoTargets, openAnyway, UpdateCurrentListView);
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

            uint selectedAssetID = CurrentlySelectedAssetIDs()[0];

            if (CurrentlySelectedAssetIDs().Count == 1)
            {
                var finalAssetName = archive.GetFromAssetID(selectedAssetID).assetName;
                var assetFileExtension = Path.GetExtension(archive.GetFromAssetID(selectedAssetID).assetFileName).ToLower();
                // Check to see if the extension is '.anm' and use 'assetFileName'
                if (assetFileExtension == ".anm")
                {
                    finalAssetName = Path.GetFileName(archive.GetFromAssetID(selectedAssetID).assetFileName);
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    //FileName = archive.GetFromAssetID(CurrentlySelectedAssetIDs()[0]).assetName
                    FileName = finalAssetName
                };
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    try
                    {
                        var AHDR = archive.GetFromAssetID(selectedAssetID).BuildAHDR(archive.platform.Endianness());
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
                            var asset = archive.GetFromAssetID(u);
                            var AHDR = asset.BuildAHDR(archive.platform.Endianness());

                            // Check to see if the file is an '.anm' before exporting raw file(s)
                            if (Path.GetExtension(asset.assetFileName) == ".anm")
                            {
                                // Combine the selected folder path with the filename
                                string fullPath = Path.Combine(saveFileDialog.FileName, Path.GetFileName(asset.assetFileName));
                                File.WriteAllBytes(fullPath, AHDR.data);
                            }
                            else
                            {
                                File.WriteAllBytes(saveFileDialog.FileName + "/" + AHDR.ADBG.assetName, AHDR.data);
                            }
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

            SetMenuItemsEnabled();
        }

        public void MouseMoveGeneric(Matrix viewProjection, int deltaX, int deltaY, bool grid)
        {
            archive.MouseMoveForPosition(viewProjection, deltaX, deltaY, grid);
            archive.MouseMoveForRotation(viewProjection, deltaX, grid);//, deltaY);
            archive.MouseMoveForScale(viewProjection, deltaX, deltaY, grid);
            archive.MouseMoveForPositionLocal(viewProjection, deltaX, deltaY, grid);
        }

        public void SetSelectedIndex(uint assetID, bool newlyAddedObject, bool addToSelected = false)
        {
            var assetIDs = new List<uint> { assetID };
            if (addToSelected)
                assetIDs.AddRange(CurrentlySelectedAssetIDs());
            SetSelectedIndices(assetIDs, newlyAddedObject);
        }

        public void SetSelectedIndices(List<uint> assetIDs, bool newlyAddedObjects)
        {
            foreach (uint u in assetIDs)
                if (!archive.ContainsAsset(u))
                {
                    archive.ClearSelectedAssets();
                    listViewAssets.SelectedIndices.Clear();
                    listViewAssets.EndUpdate();
                    return;
                }

            var firstOrDefault = assetIDs.FirstOrDefault();
            if (firstOrDefault != 0)
                PopulateAssetListAndComboBox();
            AssetType assetType = archive.GetFromAssetID(firstOrDefault).assetType;

            if (!archive.NoLayers && archive.GetLayerFromAssetID(firstOrDefault) != archive.SelectedLayerIndex)
                comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(firstOrDefault);

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
                    SelectAssetTypeOnBox(assetType);

                PopulateAssetList(assetType, null, true, assetIDs);
            }
            else
            {
                listViewAssets.SelectedIndices.Clear();
                int first = -1;
                for (int i = 0; i < listViewAssets.Items.Count; i++)
                    if (assetIDs.Contains((uint)listViewAssets.Items[i].Tag))
                    {
                        listViewAssets.SelectedIndices.Add(i);
                        if (first == -1)
                            first = i;
                    }
                listViewAssets.EnsureVisible(first);
            }
        }

        private void SelectAssetTypeOnBox(AssetType assetType)
        {
            foreach (AssetTypeContainer atc in comboBoxAssetTypes.Items)
                if (atc.assetType == assetType)
                {
                    comboBoxAssetTypes.SelectedItem = atc;
                    break;
                }
        }

        private readonly System.Drawing.Color defaultColor;

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

            if (!archive.NoLayers && assetIDs.Any())
            {
                var firstOrDef = assetIDs.FirstOrDefault();
                if (archive.GetLayerFromAssetID(firstOrDef) != comboBoxLayers.SelectedIndex || comboBoxLayers.SelectedIndex == -1)
                    comboBoxLayers.SelectedIndex = archive.GetLayerFromAssetID(firstOrDef);
            }

            if (comboBoxAssetTypes.Items.Count > 0)
                comboBoxAssetTypes.SelectedIndex = 0;
            PopulateAssetList(AssetType.Null, assetIDs, assetIDs.Count == 1, assetIDs);
        }

        private void EditPACKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (archive.EditPack())
            {
                PopulateLayerTypeComboBox();
                PopulateLayerComboBox();
                PopulateAssetList();
                OnEditorUpdate();
            }
        }

        private void organizeLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrganizeLayersClick(false, sender, e);
        }

        private void organizeLegacyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrganizeLayersClick(true, sender, e);
        }

        private void OrganizeLayersClick(bool legacy, object sender, EventArgs e)
        {
            if (archive.NoLayers)
                return;

            if (archive.OrganizeLayers(legacy))
            {
                PopulateLayerComboBox();
                comboBoxLayers.SelectedIndex = -1;
                comboBoxLayers_SelectedIndexChanged(sender, e);
            }
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
            var applyScale = ApplyScale.GetScaleWithAssets(archive.ScalableAssetTypesOnArchive().Select(a => new AssetTypeContainer(a)).OrderBy(t => t.ToString()));
            if (applyScale.HasValue)
                archive.ApplyScale(applyScale.Value.Item1, applyScale.Value.Item2, applyScale.Value.Item3, applyScale.Value.Item4);
        }

        private void MergeSimilarAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            archive.MergeSimilar();
            comboBoxLayers_SelectedIndexChanged(sender, e);
        }

        private void exportAssetsIniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
                archive.ExportHip(openFile.FileName);
        }

        private void importHIPArchiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportHipClick(false);
        }

        private void importForceOverwriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportHipClick(true);
        }

        private void ImportHipClick(bool forceOverwrite)
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "All supported file types|*.hip;*.hop;*.ini|HIP archives|*.hip|HOP archives|*.hop|HipHopTool INI|*.ini",
                Multiselect = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Enabled = false;
                archive.ImportHip(openFile.FileNames, forceOverwrite);
                OnEditorUpdate();
                Enabled = true;
            }
            PopulateLayerComboBox();
        }

        private void checkedListBoxAssets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I && e.Modifiers == Keys.Control)
            {
                if (buttonImportAsset.Enabled)
                    buttonAddAsset_Click(null, null);
            }
            else if (e.KeyCode == Keys.D && e.Modifiers == Keys.Control)
            {
                if (buttonDuplicateAsset.Enabled)
                    buttonDuplicate_Click(null, null);
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                if (buttonCopyAsset.Enabled)
                    buttonCopy_Click(null, null);
            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                if (buttonPasteAssets.Enabled)
                    buttonPaste_Click(null, null);
            }
            else if (e.KeyCode == Keys.G && e.Modifiers == Keys.Control)
            {
                if (buttonEditDataAsset.Enabled)
                    buttonInternalEdit_Click(null, null);
            }
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Control)
            {
                if (buttonEditAsset.Enabled)
                    buttonEditAsset_Click(null, null);
            }
            else
            if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
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
            if (!archive.NoLayers && comboBoxLayers.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a layer to place your asset in!");
                return null;
            }

            List<uint> assetIDs = new List<uint>();

            archive.PlaceTemplate(position, ref assetIDs, template: template);

            if (assetIDs.Count != 0)
            {
                archive.UnsavedChanges = true;
                if (!archive.NoLayers)
                    comboBoxLayers.Items[archive.SelectedLayerIndex] = archive.LayerToString();
                PopulateAssetListAndComboBox();
                SetSelectedIndices(assetIDs, true);
            }

            return assetIDs;
        }

        private void ToolStripMenuItem_CreateGroup_Click(object sender, EventArgs e)
        {
            var ids = CurrentlySelectedAssetIDs();
            var assetIDs = PlaceTemplate(new Vector3(), AssetTemplate.Group);
            if (assetIDs != null && assetIDs.Count > 0)
            {
                var asset = (AssetGRUP)archive.GetFromAssetID(assetIDs[0]);
                asset.AddItems(ids);
                PopulateAssetListAndComboBox();
                SetSelectedIndex(asset.assetID, true);
            }
        }

        public bool TemplateFocus => checkBoxTemplateFocus.Checked;

        public void ClearTemplateFocus()
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

            buttonImportAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonDuplicateAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonCopyAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonPasteAssets.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonRemoveAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonView.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonEditAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonExportRawAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonEditDataAsset.Visible = !hideButtonsToolStripMenuItem.Checked;
            buttonMultiEditAsset.Visible = !hideButtonsToolStripMenuItem.Checked;

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
            Enabled = false;
            OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };

            if (openTXD.ShowDialog() == DialogResult.OK)
            {
                archive.ImportTextureDictionary(openTXD.FileName, RW3);
                PopulateLayerComboBox();
                OnEditorUpdate();
            }
            Enabled = true;
        }

        private void ExportTXD(CheckState RW3)
        {
            SaveFileDialog saveTXD = new SaveFileDialog() { Filter = "TXD archives|*.txd" };

            if (saveTXD.ShowDialog() == DialogResult.OK)
                archive.ExportTextureDictionary(saveTXD.FileName, RW3);
        }

        internal void RefreshHop(SharpRenderer renderer)
        {
            archive.SetupTextureDisplay();
            archive.ResetModels(renderer);
            archive.RecalculateAllMatrices();
        }

        private HashSet<Keys> PressedKeys = new HashSet<Keys>();

        private void ArchiveEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!PressedKeys.Contains(e.KeyCode))
                PressedKeys.Add(e.KeyCode);

            if (PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.S))
                Save();

            if (PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.N))
                newToolStripMenuItem_Click(sender, e);

            if (PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.O))
                openToolStripMenuItem_Click(sender, e);

            if (PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.W))
                closeToolStripMenuItem_Click(sender, e);
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
            if (GetCurrentlyOpenFileName().Equals("Empty"))
            {
                // TODO: Disable the menu button if no file is open
                MessageBox.Show("No file is currently open.");
                return;
            }

            bool generationWasSuccessful = true;
            bool isDebug;
            var timeOfGeneration = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
            string currentFilepath = GetCurrentlyOpenFileName();
            var stopwatch = new Stopwatch();

            // TODO: Change filetype to LIP or LOP
            string filepath = Path.Combine(
                Path.GetDirectoryName(currentFilepath),
                Path.GetFileName(currentFilepath) + "_IP_Report.txt");

#if DEBUG
            HipFile hip = HipFile.FromPath(currentFilepath).Item1;
            if (!File.Exists(currentFilepath))
            {
                MessageBox.Show($"HIP File \"{currentFilepath}\" not found");
                return;
            }
#endif

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
                string game = archive.game == Game.Incredibles ? "Incredibles/TSSM" : archive.game.ToString();
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

#if DEBUG
                Dictionary<uint, Section_AHDR> ahdrList = new Dictionary<uint, Section_AHDR>();
                foreach (var ahdr in hip.DICT.ATOC.AHDRList)
                    ahdrList.Add(ahdr.assetID, ahdr);

                int padding = archive.platform == Platform.GameCube ? 0x20 : 0x800;

                output.WriteLine("\n\nVerbose Layer Information\n------------------\n");
                int i = 0;
                foreach (var lhdr in hip.DICT.LTOC.LHDRList)
                {
                    LayerType type = (archive.game >= Game.Incredibles || lhdr.layerType < 2) ? (LayerType)lhdr.layerType : (LayerType)(lhdr.layerType + 1);

                    output.WriteLine($"Layer #{i} is Type {lhdr.layerType} ({type})");
                    output.WriteLine("-------------------");
                    output.WriteLine("AssetName                       AssetID  Checksum Type Offset   Size     Plus Align");
                    output.WriteLine("---------                       -------  -------- ---- ------   ----     ---- -----");

                    int layerSizeTotal = 0, layerSizeDataAsset = 0, layerSizeDataLayer = 0, layerSizePaddingAsset = 0, layerSizePaddingLayer = 0;
                    int sndSize = 0, sndsSize = 0;

                    foreach (uint aid in lhdr.assetIDlist)
                    {
                        Section_AHDR ahdr = ahdrList[aid];
                        output.Write(ahdr.ADBG.assetName.PadRight(32, ' '));
                        output.Write(ahdr.assetID.ToString("X8") + ' ');
                        output.Write(ahdr.ADBG.checksum.ToString("X8") + ' ');
                        output.Write(Functions.GetCode(ahdr.assetType).PadRight(5, ' '));
                        output.Write(ahdr.fileOffset.ToString("X8").PadRight(9, ' '));
                        output.Write(ahdr.fileSize.ToString("X").PadRight(9, ' '));
                        output.Write(ahdr.plusValue.ToString().PadRight(5, ' '));
                        output.Write(ahdr.ADBG.alignment.ToString().PadRight(6, ' ') + "\n");

                        if (ahdr.assetType == AssetType.Sound)
                            sndSize += ahdr.fileSize + ahdr.plusValue;
                        else if (ahdr.assetType == AssetType.SoundStream)
                            sndsSize += ahdr.fileSize + ahdr.plusValue;

                        layerSizeTotal += ahdr.fileSize + ahdr.plusValue;
                        layerSizeDataAsset += ahdr.fileSize;
                        layerSizePaddingAsset += ahdr.plusValue;
                        layerSizeDataLayer += ahdr.fileSize + ahdr.plusValue;
                    }

                    layerSizePaddingLayer = (padding - (layerSizeTotal % padding)) % padding;
                    layerSizeTotal += layerSizePaddingLayer;

                    if (archive.game <= Game.BFBB && lhdr.layerType == 6 || archive.game >= Game.Incredibles && lhdr.layerType == 7) // SRAM
                    {
                        output.WriteLine($"\nSND Size (bytes): {sndSize} ({ConvertSize(sndSize)})");
                        output.WriteLine($"SNDS Size (bytes): {sndsSize} ({ConvertSize(sndsSize)})");
                    }

                    output.WriteLine($"\nLayer Size (Total):         {layerSizeTotal} ({ConvertSize(layerSizeTotal)})");
                    output.WriteLine($"Layer Size (Data_Layer):    {layerSizeDataLayer} ({ConvertSize(layerSizeDataLayer)})");
                    output.WriteLine($"Layer Size (Data_Asset):    {layerSizeDataAsset} ({ConvertSize(layerSizeDataAsset)})");
                    output.WriteLine($"Layer Size (Padding_Asset): {layerSizePaddingAsset} ({ConvertSize(layerSizePaddingAsset)})");
                    output.WriteLine($"Layer Size (Padding_Layer): {layerSizePaddingLayer} ({ConvertSize(layerSizePaddingLayer)})\n\n");
                    i++;
                }
#else
                output.WriteLine("\nLAYER INFORMATION");
                output.WriteLine("-------------");

                List<uint> assetIDsOnLayer;
                List<AssetType> assetTypesOnLayer;
                int numOfInvisibleAssets = 0;


                for (int i = 0; i < archive.LayerCount; i++)
                {
                    archive.SelectedLayerIndex = i;

                    assetTypesOnLayer = archive.AssetTypesOnLayer();
                    output.WriteLine($"{archive.LayerToString()}");
                    output.Write($"{assetTypesOnLayer.Count} asset type(s) [");
                    foreach (var assetType in assetTypesOnLayer)
                    {
                        output.Write($"{assetType} ");
                    }
                    output.WriteLine("]");

                    assetIDsOnLayer = archive.GetAssetIDsOnLayer();

                    for (int j = 0; j < assetIDsOnLayer.Count(); j++)
                    {
                        Asset asset = archive.GetFromAssetID(assetIDsOnLayer[j]);

                        output.WriteLine($"    {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                        if (asset.isInvisible)
                            numOfInvisibleAssets++;
                    }
                    output.WriteLine("");
                }
                output.WriteLine($"Number of invisible assets: {numOfInvisibleAssets}");
                output.WriteLine("");
#endif

                output.WriteLine("\nASSET LINKING INFO");
                output.WriteLine("-------------");


                foreach (var asset in allAssets)
                {
                    // If Base asset, get list of links.
                    if (asset is BaseAsset baseAsset)
                    {
                        Link[] links = baseAsset.Links;
                        if (links.Length > 0)
                        {
                            output.WriteLine($"{asset.assetName}: {links.Length} links");

                            foreach (var link in links)
                            {
                                string eventSendIDName = "";
                                string eventReceiveIDName = "";

                                if (archive.game == Game.BFBB)
                                {
                                    eventSendIDName = ((EventBFBB)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventBFBB)link.EventReceiveID).ToString();
                                }
                                else if (archive.game == Game.Scooby) 
                                {
                                    eventSendIDName = ((EventScooby)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventScooby)link.EventReceiveID).ToString();
                                }
                                else if (archive.game == Game.Incredibles) // TSSM/Incredibles
                                {
                                    eventSendIDName = ((EventTSSM)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventTSSM)link.EventReceiveID).ToString();
                                }
                                else if (archive.game == Game.ROTU)
                                {
                                    eventSendIDName = ((EventROTU)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventROTU)link.EventReceiveID).ToString();
                                }
                                else if (archive.game == Game.RatProto)
                                {
                                    eventSendIDName = ((EventRatProto)link.EventSendID).ToString();
                                    eventReceiveIDName = ((EventRatProto)link.EventReceiveID).ToString();
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

                foreach (var asset in allAssets)
                {
                    if (asset.assetType == AssetType.Model)
                    {
                        numOfModels++;

                        List<uint> whoTargets = archive.FindWhoTargets(asset.assetID);
                        if (whoTargets.Count == 0)
                        {
                            output.WriteLine($"  {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                            numOfUnusedModels++;
                        }
                    }
                }

                output.WriteLine($"{numOfUnusedModels} unused models out of {numOfModels} total.");

                // Find who Targets Me throws an exception sometimes :(

                output.WriteLine("\n#### END OF REPORT ####");
                stopwatch.Stop();
                output.WriteLine($"Report generated in {Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms.");
            }

            if (generationWasSuccessful)
            {
                if (MessageBox.Show($"Report \"{Path.GetFileName(filepath)}\" generated. Open file?",
                    "Report generated",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start(filepath);
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

        private void noLayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newNoLayers = !noLayersToolStripMenuItem.Checked;
            if (newNoLayers)
            {
                var dr = MessageBox.Show("This will convert the HIP/HOP archive into a format in which layers are virtually gone: " +
                    "assets will be placed automatically by Industrial Park in the correct layers upon saving and you do not have to worry about them anymore. " +
                    "Enabling this will erase your current layers. Proceed?",
                    "No Layers", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr != DialogResult.Yes)
                    return;
            }
            archive.NoLayers = newNoLayers;
            SetNoLayers();
        }

        private bool noLayersForGroupBox = false;

        private void SetNoLayers()
        {
            if (archive.NoLayers)
            {
                noLayersToolStripMenuItem.Checked = true;
                renameLayerToolStripMenuItem.Enabled = false;

                groupBoxLayers.Visible = false;

                if (!noLayersForGroupBox)
                {
                    groupBoxAssets.Location = new System.Drawing.Point(groupBoxAssets.Location.X, groupBoxAssets.Location.Y - 53);
                    groupBoxAssets.Size = new System.Drawing.Size(groupBoxAssets.Size.Width, groupBoxAssets.Size.Height + 53);
                    noLayersForGroupBox = true;
                }
            }
            else
            {
                noLayersToolStripMenuItem.Checked = false;

                groupBoxLayers.Visible = true;

                if (noLayersForGroupBox)
                {
                    groupBoxAssets.Location = new System.Drawing.Point(groupBoxAssets.Location.X, groupBoxAssets.Location.Y + 53);
                    groupBoxAssets.Size = new System.Drawing.Size(groupBoxAssets.Size.Width, groupBoxAssets.Size.Height - 53);
                    noLayersForGroupBox = false;
                }

                PopulateLayerTypeComboBox();
                PopulateLayerComboBox();
            }

            PopulateAssetListAndComboBox();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboBoxLayers.SelectedIndex != -1)
            {
                if (archive.RenameLayer(comboBoxLayers.SelectedIndex))
                {
                    PopulateLayerComboBox();
                    PopulateAssetListAndComboBox();
                }
            }
        }

        private void exportAllWavToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!archive.ContainsAssetWithType(AssetType.Sound) && !archive.ContainsAssetWithType(AssetType.SoundStream))
                return;

            CommonOpenFileDialog saveFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ProgressBar progressBar = new ProgressBar("Export All (Wav)");
                progressBar.SetProgressBar(0, archive.GetAllAssets().OfType<AssetSound>().Count(), 1);
                progressBar.Show();
                foreach (var asset in archive.GetAllAssets().OfType<AssetSound>())
                {
                    SoundUtility_vgmstream.ExportToFile(asset, archive, Path.Combine(saveFileDialog.FileName, asset.assetName + ".wav"));
                    progressBar.PerformStep(asset.assetName);
                }
                progressBar.Close();
            }
        }

        private void exportAllRawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!archive.ContainsAssetWithType(AssetType.Sound) && !archive.ContainsAssetWithType(AssetType.SoundStream))
                return;

            CommonOpenFileDialog saveFileDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true
            };
            if (saveFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var asset in archive.GetAllAssets().OfType<AssetWithData>().Where(asset => asset.assetType == AssetType.Sound || asset.assetType == AssetType.SoundStream))
                    try
                    {
                        string extension =
                            (archive.platform == Platform.GameCube && asset.game < Game.Incredibles) ? ".DSP" :
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

        private void ImportSounds(bool raw, AssetType assetType)
        {
            Enabled = false;
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "Audio files|*",
                Multiselect = true
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                archive.ImportSounds(raw, openFile.FileNames, assetType, overwriteOnImportToolStripMenuItem.Checked, out List<uint> assetIDs);
                if (assetIDs.Any())
                {
                    OnEditorUpdate();
                    PopulateLayerComboBox();
                    comboBoxLayers.SelectedIndex = archive.IndexOfLayerOfType(LayerType.SRAM);
                    SetSelectedIndices(assetIDs, true);
                    SetMenuItemsEnabled();
                }
            }
            Enabled = true;
        }

        private void importAsSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSounds(false, AssetType.Sound);
        }

        private void importAsSoundStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSounds(false, AssetType.SoundStream);
        }

        private void importRawSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSounds(true, AssetType.Sound);
        }

        private void importRawSoundStreamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportSounds(true, AssetType.SoundStream);
        }

        private void overwriteOnImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            overwriteOnImportToolStripMenuItem.Checked = !overwriteOnImportToolStripMenuItem.Checked;
        }

        private void legacySaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isLegacy = ((ToolStripMenuItem)sender).Checked;
            archive.LegacySave = isLegacy;

            if (isLegacy && archive.NoLayers)
            {
                archive.NoLayers = false;
                SetNoLayers();
            }

            noLayersToolStripMenuItem.Enabled = !isLegacy;
            renameLayerToolStripMenuItem.Enabled = !isLegacy;
        }
    }
}
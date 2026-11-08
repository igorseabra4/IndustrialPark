using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.Models.CollisionTree;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class ArchiveEditor : Form
{
    private readonly System.Drawing.Color defaultColor;
    private bool programIsChangingStuff = false;
    public bool Targeted => checkBoxTemplateFocus.Checked;

    public Archive.Archive archive;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool UnsavedChanges
    {
        get;
        set
        {
            field = value;
            ChangesMade?.Invoke();
        }
    }

    public event Action ChangesMade;
    
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectedLayerIndex
    {
        get => comboBoxLayers.SelectedIndex;
        set => comboBoxLayers.SelectedIndex = value;
    }

    private AssetType curSelectedType = AssetType.Null;

    protected AssetSelectionManager selectionManager;
    public IEnumerable<Asset> SelectedAssets => selectionManager.SelectedAssets;
    public IEnumerable<uint> SelectedAssetIDs => selectionManager.SelectedAssetIDs;
    private AssetHistoryManager historyManager;
    private ScrollableMessageBox verifyResult;
    public IEnumerable<string> LayerNames => comboBoxLayers.Items.Cast<object>().Select(o => o.ToString());

    public event Action<ArchiveEditor> ToolbarStateChanged;
    public event Action EditorUpdate;
    protected void OnEditorUpdate() => EditorUpdate?.Invoke();

    public ArchiveEditor(string filePath, Platform scoobyPlatform)
    {
        InitializeComponent();

        ArchiveStateChanged += HandleArchiveStateChanged;

        defaultColor = textBoxFindAsset.BackColor;

        AssetFactoryService.PopulateTemplateMenusAt(addTemplateToolStripMenuItem, TemplateToolStripMenuItem_Click, false);

        comboBoxLayers.SelectedIndexChanged += (s, e) => ToolbarStateChanged?.Invoke(this);
        checkBoxTemplateFocus.CheckedChanged += (s, e) => ToolbarStateChanged?.Invoke(this);

        textBoxFindAsset.AutoCompleteMode = AutoCompleteMode.Suggest;
        textBoxFindAsset.AutoCompleteSource = AutoCompleteSource.CustomSource;
        textBoxFindAsset.AutoCompleteCustomSource = new AutoCompleteStringCollection();

        selectionManager = new AssetSelectionManager();

        if (assetImageList != null && assetImageList.Images.Count > 0)
            listViewAssets.SmallImageList = assetImageList;

        if (!string.IsNullOrWhiteSpace(filePath))
            OpenFile(filePath, scoobyPlatform);

        _updateFilesizeStatusBarItem();
        checkBoxTemplateFocus_CheckedChanged(null, null);

        fileToolStripMenuItem1.DropDown = fileToolStripMenuItem.DropDown;
        editToolStripMenuItem1.DropDown = editToolStripMenuItem.DropDown;
        toolsToolStripMenuItem1.DropDown = toolsToolStripMenuItem.DropDown;

        SetDockedMenuItemsVisibility(false);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.WindowsShutDown)
            return;
        if (e.CloseReason == CloseReason.FormOwnerClosing)
            return;

        e.Cancel = true;
        Hide();
    }

    private HashSet<Keys> PressedKeys = new HashSet<Keys>();

    private void ArchiveEditor_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Handled || textBoxFindAsset.Focused)
            return;

        if (!PressedKeys.Contains(e.KeyCode))
            PressedKeys.Add(e.KeyCode);

        if ((PressedKeys.Contains(Keys.Y) && PressedKeys.Contains(Keys.ControlKey)) ||
            (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.ShiftKey)))
        {
            e.Handled = true;
            Program.UndoBuffer.Redo();
        }
        else if (PressedKeys.Contains(Keys.Z) && PressedKeys.Contains(Keys.ControlKey))
        {
            e.Handled = true;
            Program.UndoBuffer.Undo();
        }

        if (PressedKeys.Contains(Keys.Delete))
        {
            e.Handled = true;
            ButtonRemoveAsset_Click(sender, e);
        }

        if (PressedKeys.Contains(Keys.ControlKey))
        {
            if (PressedKeys.Contains(Keys.S))
            {
                e.Handled = true;
                Save();
            }
            else if (PressedKeys.Contains(Keys.N))
            {
                e.Handled = true;
                newToolStripMenuItem_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.O))
            {
                e.Handled = true;
                openToolStripMenuItem_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.ControlKey) && PressedKeys.Contains(Keys.W))
            {
                e.Handled = true;
                closeToolStripMenuItem_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.I))
            {
                e.Handled = true;
                buttonImportAsset_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.D))
            {
                e.Handled = true;
                buttonDuplicate_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.C))
            {
                e.Handled = true;
                buttonCopy_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.V))
            {
                e.Handled = true;
                PasteAssets();
            }
            else if (PressedKeys.Contains(Keys.G))
            {
                e.Handled = true;
                OpenInternalEditors();
            }
            else if (PressedKeys.Contains(Keys.H))
            {
                e.Handled = true;
                buttonEditHeader_Click(sender, e);
            }
            else if (PressedKeys.Contains(Keys.A))
            {
                e.Handled = true;
                var assetIDs = listViewAssets.Items.Cast<ListViewItem>().Select(i => (uint)i.Tag);
                SetSelectedAssets(assetIDs);
            }
        }
    }

    private void ArchiveEditor_KeyUp(object sender, KeyEventArgs e)
    {
        PressedKeys.Remove(e.KeyCode);
    }

    private void ArchiveEditor_Activated(object sender, EventArgs e)
    {
        Opacity = 1f;
        foreach (var child in OwnedForms)
            child.SendToBack();
        BringToFront();
    }

    private void ArchiveEditor_Deactivate(object sender, EventArgs e)
    {
        PressedKeys.Clear();
        if (Program.MainForm.TranslucentWhenOutOfFocus && CanFocus && !OwnedForms.Any())
            Opacity = 0.5f;
    }

    private void newToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (UnsavedChanges)
        {
            DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                return;
            if (result == DialogResult.Yes)
                Save();
        }

        var getNewArchive = NewArchive.GetNewArchive();

        if (getNewArchive.HasValue)
        {
            DisposeArchive(true);

            archive = new Archive.Archive(
                getNewArchive.Value.platform,
                getNewArchive.Value.game,
                getNewArchive.Value.fileDate,
                getNewArchive.Value.dateString,
                getNewArchive.Value.autoLayers,
                getNewArchive.Value.addDefaultAssets
            );

            historyManager = new AssetHistoryManager(archive);
            foreach (var (assetID, data) in archive.Assets.Select(a => (a.assetID, a.BuildAHDR(archive.Platform.Endianness()).data)))
                historyManager.AddAsset(assetID, data);

            ArchiveSubscriptions();
            SetMenuItemsEnabled();
            SetAutoLayers();
            SetupAssetVisibilityButtons();
        }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (UnsavedChanges)
        {
            DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                return;
            if (result == DialogResult.Yes)
                Save();
        }

        OpenFileDialog openFile = new OpenFileDialog()
        {
            Filter = "HIP/HOP Files|*.hip;*.hop",
            Title = "Please choose a HIP or HOP file"
        };
        if (openFile.ShowDialog() == DialogResult.OK)
        {
            bool shouldOpenFile = true;

            foreach (var archiveEditor in Program.ArchiveEditors)
            {
                if (Path.GetFileName(archiveEditor.GetCurrentFilePath()) ==
                    Path.GetFileName(openFile.FileName))
                {
                    var result = MessageBox.Show(
                                    $"A file named {Path.GetFileName(openFile.FileName)} is already open. Would you still like to open it?",
                                    "Duplicate file detected",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                        shouldOpenFile = false;
                }
            }

            if (shouldOpenFile)
                OpenFile(openFile.FileName);
            _updateFilesizeStatusBarItem();
        }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Save();
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SaveAs();
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (UnsavedChanges)
        {
            DialogResult result = MessageBox.Show("You have unsaved changes. Do you wish to save them before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (result == DialogResult.Cancel)
                return;
            if (result == DialogResult.Yes)
                SaveAndWait();
        }

        CloseArchiveEditor();
        Program.MainForm.Activate();
    }

    private void convertArchiveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BeginTransaction();
        if (archive.Transformer.EditPack(out IEnumerable<string> unconvertable))
        {
            EndTransaction();
            OnEditorUpdate();

            if (unconvertable != null && unconvertable.Any())
            {
                var text = "The following asset types could not be converted. You might need to re-create them or replace them with the appropriate version on the game and/or platform.\n\n" + string.Join("\n", unconvertable.OrderBy(x => x));
                new ScrollableMessageBox($"[{Path.GetFileName(archive.CurrentFilePath)}] Unconvertable assets", text).Show(this);
            }
        }
    }

    private void autoLayersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BeginTransaction();
        archive.Transformer.SetAutoLayers(!autoLayersToolStripMenuItem.Checked);
        EndTransaction();
    }

    private void buildLayersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BuildLayersClick(false);
    }

    private void buildLegacyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BuildLayersClick(true);
    }

    private void renameLayerToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (SelectedLayerIndex == -1)
            return;

        var index = SelectedLayerIndex;
        var renameLayer = new RenameLayer(archive.GetLayerName(index));
        if (renameLayer.ShowDialog() == DialogResult.OK)
        {
            var name = string.IsNullOrWhiteSpace(renameLayer.LayerName) ? null : renameLayer.LayerName;
            BeginTransaction();
            if (archive.RenameLayer(index, name))
            {
                EndTransaction();
            }
        }
    }

    private void legacySaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        bool isLegacy = ((ToolStripMenuItem)sender).Checked;
        archive.LegacySave = isLegacy;

        BeginTransaction();
        if (isLegacy && archive.AutoLayers)
        {
            archive.Transformer.SetAutoLayers(false);
            SetAutoLayers();
        }
        EndTransaction();

        autoLayersToolStripMenuItem.Enabled = !isLegacy;
        renameLayerToolStripMenuItem.Enabled = !isLegacy;
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
            Enabled = false;
            BeginTransaction();
            archive.Transformer.ImportHip(openFile.FileNames, overwriteAssetsOnImportToolStripMenuItem.Checked);
            EndTransaction();
            OnEditorUpdate();
            Enabled = true;
        }
    }

    private void exportAssetsIniToolStripMenuItem_Click(object sender, EventArgs e)
    {
        CommonOpenFileDialog openFile = new CommonOpenFileDialog() { IsFolderPicker = true };
        if (openFile.ShowDialog() == CommonFileDialogResult.Ok)
            archive.ExportToIni(openFile.FileName);
    }

    private void importTexturesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Enabled = false;
        var (AHDRs, overwrite) = ImportTextures.GetAssets(archive.Game, archive.Platform);

        if (AHDRs != null)
        {
            BeginTransaction();
            List<Asset> assets = archive.ImportAssets(AHDRs, SelectedLayerIndex, overwrite);
            EndTransaction();
            if (!archive.AutoLayers)
                comboBoxLayers.Items[comboBoxLayers.SelectedIndex] = archive.LayerToString(SelectedLayerIndex);
            OnEditorUpdate();
            SetSelectedAssets(assets);
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
            var textures = TextureConverter.GetTexturesAsBitmaps(archive.Assets.OfType<AssetRWTX>(), TextureIOHelper.currentTextureVersion(archive.Game, archive.Platform));

            RenderWareFile.ReadFileMethods.treatStuffAsByteArray = false;

            foreach ((string textureName, Bitmap texture) in textures)
                texture.Save(Path.Combine(saveFileDialog.FileName, textureName + ".png"), System.Drawing.Imaging.ImageFormat.Png);
        }
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
            progressBar.SetProgressBar(0, archive.Assets.OfType<AssetSound>().Count(), 1);
            progressBar.Show(this);
            foreach (var asset in archive.Assets.OfType<AssetSound>())
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
            foreach (var asset in archive.Assets.OfType<AssetWithData>().Where(asset => asset.assetType == AssetType.Sound || asset.assetType == AssetType.SoundStream))
                try
                {
                    string extension =
                        (archive.Platform == Platform.GameCube && asset.game < Game.Incredibles) ? ".DSP" :
                        (archive.Platform == Platform.Xbox) ? ".WAV" :
                        (archive.Platform == Platform.PS2) ? ".VAG" :
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

    private void importModelsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ImportModelResult result = ImportModel.GetImportData(archive.Game, archive.Platform);

        if (result == null)
            return;

        List<Asset> assets;
        BeginTransaction();

        if (result.AssetType == AssetType.JSP)
        {
            if (archive.Game == Game.BFBB)
            {
                var assetsToRemove = archive.Assets
                    .Where(a => a.assetType is AssetType.BSP or AssetType.JSP or AssetType.JSPInfo)
                    .Select(a => a.assetID);
                RemoveAssets(assetsToRemove);
                if (!archive.AutoLayers)
                {
                    archive.RemoveLayerOfType(LayerType.BSP);
                    archive.RemoveLayerOfType(LayerType.JSPINFO);
                }
            }
            assets = archive.Transformer.CreateJSPInfoAndBSPLayers(result.AHDRCollections, result.Overwrite, archive.ContainsAsset);
        }
        else
        {
            assets = archive.ImportAssets([.. result.AHDRCollections.SelectMany(list => list)], SelectedLayerIndex, result.Overwrite);

            if (result.AssetType == AssetType.Model)
            {
                if (result.PiptVColors)
                    archive.Transformer.MakePiptVcolors(assets);
                if (result.GenerateSimps)
                    assets.AddRange(archive.Transformer.MakeSimps(assets, result.SolidSimps, result.LedgeGrabSimps));
            }
        }

        EndTransaction();

        SetSelectedAssets(assets);
        OnEditorUpdate();
    }

    private void importMultipleAssetsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        (List<Section_AHDR> AHDRs, bool overwrite) = IndustrialPark.AddMultipleAssets.GetAssets();

        if (AHDRs != null)
        {
            BeginTransaction();
            var assets = archive.ImportAssets(AHDRs, SelectedLayerIndex, overwrite);
            EndTransaction();

            OnEditorUpdate();
            SetSelectedAssets(assets);
        }
    }

    private void generateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        ArchiveTransformer.GenerateReportTxt(archive, UnsavedChanges);
    }

    private void verifyArchiveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (verifyResult != null)
            if (!verifyResult.IsDisposed)
                verifyResult.Close();

        ProgressBar progressBar = new ProgressBar("Verify Archive");
        progressBar.Show(this);

        verifyResult = new ScrollableMessageBox("Verify results on " + Text, archive.Transformer.VerifyArchive(progressBar));
        verifyResult.Show(this);

        progressBar.Close();
    }

    private void applyScaleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var assetTypes = ArchiveModelTransformer.ScalableAssetTypesOnArchive(archive).Select(a => new AssetTypeContainer(a)).OrderBy(t => t.ToString());
        var applyScale = ApplyScale.GetScaleWithAssets(assetTypes);
        if (applyScale.HasValue)
        {
            BeginTransaction();
            ArchiveModelTransformer.ApplyScale(archive, applyScale.Value.Item1, applyScale.Value.Item2, applyScale.Value.Item3, applyScale.Value.Item4);
            EndTransaction();
            Program.MainForm.RefreshTexturesAndModels();
        }
    }

    private void mergeSimilarAssetsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        BeginTransaction();
        archive.TableManager.MergeTableAssets();
        EndTransaction();
    }

    private void BuildCollTree(object sender, EventArgs e)
    {
        Enum.TryParse(((ToolStripMenuItem)sender).Tag.ToString(), out CollTreeVersion collVer);
        List<string> skipped = new List<string>();

        ProgressBar progressBar = new ProgressBar("Building Collision PLG...");
        progressBar.Show(this);
        progressBar.SetProgressBar(0, archive.Assets.OfType<AssetMODL>().Count(), 1);

        BeginTransaction();

        foreach (var modl in archive.Assets.OfType<AssetMODL>())
        {
            try
            {
                modl.BuildCollisionTree(collVer);
                modl.InvokePropertyChanged(false);
            }
            catch (NotSupportedException ex)
            {
                progressBar.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            catch (Exception ex)
            {
                skipped.Add($"{modl.assetName} | Reason: {ex.Message}");
            }
            finally
            {
                progressBar.PerformStep(modl.assetName);
            }
        }

        EndTransaction();

        PopulateAssetListAndComboBox();
        if (skipped.Any())
            MessageBox.Show($"Following models were skipped:\n{string.Join("\n", skipped)}", $"{skipped.Count} models skipped", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

    private void comboBoxLayers_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (programIsChangingStuff || archive.AutoLayers)
            return;

        programIsChangingStuff = true;

        if (SelectedLayerIndex == -1 && !archive.AutoLayers)
        {
            comboBoxLayerTypes.SelectedItem = null;

            comboBoxAssetTypes.Items.Clear();
            comboBoxAssetTypes.SelectedIndex = -1;
            PopulateAssetList();
        }
        else
        {
            ShowSelectedLayerType();
            PopulateAssetListAndComboBox();
        }

        programIsChangingStuff = false;
    }

    private void comboBoxLayerTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (programIsChangingStuff || SelectedLayerIndex == -1)
            return;

        BeginTransaction();

        archive.SetGameSpecificLayerType(SelectedLayerIndex, Convert.ToInt32(comboBoxLayerTypes.SelectedItem));
        comboBoxLayers.Items[SelectedLayerIndex] = archive.LayerToString(SelectedLayerIndex);

        EndTransaction();
    }

    private void buttonAddLayer_Click(object sender, EventArgs e)
    {
        try
        {
            BeginTransaction();

            archive.AddLayer();

            EndTransaction();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to add layer: " + ex.Message);
        }
    }

    private void buttonRemoveLayer_Click(object sender, EventArgs e)
    {
        if (SelectedLayerIndex < 0 || SelectedLayerIndex >= archive.LayerCount)
            return;

        int cnt = archive.GetLayerAssetIDs(SelectedLayerIndex).Count();
        if (cnt > 0 &&
            MessageBox.Show($"Are you sure you want to delete this layer with {cnt} assets?", "Warning",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            return;

        BeginTransaction();

        archive.RemoveLayer(SelectedLayerIndex);

        EndTransaction();
    }

    private void buttonArrowUp_Click(object sender, EventArgs e)
    {
        BeginTransaction();

        if (archive.MoveLayerUp(SelectedLayerIndex))
        {
            EndTransaction();
        }
    }

    private void buttonArrowDown_Click(object sender, EventArgs e)
    {
        BeginTransaction();

        if (archive.MoveLayerDown(SelectedLayerIndex))
        {
            EndTransaction();
        }
    }

    private void comboBoxAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!programIsChangingStuff && comboBoxAssetTypes.SelectedIndex != -1)
            PopulateAssetList(((AssetTypeContainer)comboBoxAssetTypes.SelectedItem).assetType);
    }

    private void textBoxFindAsset_TextChanged(object sender, EventArgs e)
    {
        uint assetID = 0;
        if (textBoxFindAsset.Text == "")
        {
            PopulateAssetList();
            return;
        }
        try
        {
            textBoxFindAsset.BackColor = defaultColor;
            assetID = HexUIntTypeConverter.AssetIDFromString(textBoxFindAsset.Text);
        }
        catch
        {
            textBoxFindAsset.BackColor = System.Drawing.Color.Red;
        }

        var assets = new List<Asset>();

        foreach (Asset a in archive.Assets)
            if (a.assetName.Contains(textBoxFindAsset.Text, StringComparison.CurrentCultureIgnoreCase) || a.assetType.ToString().Contains(textBoxFindAsset.Text, StringComparison.CurrentCultureIgnoreCase))
                assets.Add(a);
        if (assetID != 0 && archive.ContainsAsset(assetID) && !assets.Any(a => a.assetID == assetID))
            assets.Add(archive.GetAsset(assetID));

        if (!archive.AutoLayers && assets.Count != 0)
            comboBoxLayers.SelectedIndex = -1;

        if (comboBoxAssetTypes.Items.Count > 0)
            comboBoxAssetTypes.SelectedIndex = 0;
        PopulateAssetList(AssetType.Null, assets);
    }

    private void checkBoxTemplateFocus_Click(object sender, EventArgs e)
    {
        SetTargetedOn();
    }

    private void checkBoxTemplateFocus_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBoxTemplateFocus.Checked)
        {
            if (Program.IsDarkMode)
            {
                checkBoxTemplateFocus.BackColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                checkBoxTemplateFocus.BackColor = System.Drawing.Color.LightGreen;
            }
        }
        else
        {
            if (Program.IsDarkMode)
            {
                checkBoxTemplateFocus.BackColor = System.Drawing.SystemColors.Control;
            }
            else
            {
                checkBoxTemplateFocus.BackColor = System.Drawing.SystemColors.ActiveBorder;
            }
        }
    }

    private void buttonImportAsset_Click(object sender, EventArgs e)
    {
        var layerIndex = SelectedLayerIndex;

        if (!archive.AutoLayers && SelectedLayerIndex == -1)
            return;

        Section_AHDR AHDR = AssetHeader.GetAsset();

        if (AHDR != null)
        {
            while (archive.ContainsAsset(AHDR.assetID))
            {
                var result = MessageBox.Show($"Archive already contains asset with id [{AHDR.assetID:X8}]. Do you wish to overwrite it? Select 'No' to try again, or 'Cancel' to discard.", "Asset already exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                    break;
                else if (result == DialogResult.No)
                    AHDR = AssetHeader.GetAsset(AHDR);
                else if (result == DialogResult.Cancel)
                    return;
            }

            BeginTransaction();

            var asset = archive.AddNewAsset(AHDR, archive.Game, archive.Platform, layerIndex);
            AssetTransformer.SetAssetPositionToView(asset);

            EndTransaction();

            SetSelectedAsset(asset);
        }
    }

    private void buttonDuplicate_Click(object sender, EventArgs e)
    {
        if (selectionManager.SelectedAssetCount == 0)
            return;

        var duplicated = AssetService.DuplicateAssets(
            selectionManager.SelectedAssets,
            archive.Platform,
            archive.GetLayerFromAssetID,
            archive.ContainsAsset,
            UpdateReferencesOnCopy);

        BeginTransaction();

        foreach (var (asset, layerIndex) in duplicated)
            archive.AddNewAsset(asset, layerIndex);

        EndTransaction();
        SetSelectedAssets(duplicated.Select(d => d.asset));
    }

    private void buttonCopy_Click(object sender, EventArgs e)
    {
        if (!HasSelectedAssets)
            return;

        var assets = selectionManager.SelectedAssets.ToList();

        if (PressedKeys.Contains(Keys.ShiftKey))
            assets = [.. AssetService.GetAssetsAndHierarchy(assets)];

        var clipboard = AssetService.AssetsToClipboard(assets, archive.Platform, archive.GetSoundData);

        Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
    }

    private void buttonPaste_Click(object sender, EventArgs e)
    {
        PasteAssets();
    }

    private void ButtonRemoveAsset_Click(object sender, EventArgs e)
    {
        BeginTransaction();
        RemoveAssets(selectionManager.SelectedAssetIDs);
        EndTransaction();
    }

    private int currentView = -1;

    private void buttonView_Click(object sender, EventArgs e)
    {
        var assets = selectionManager.SelectedAssets.OfType<IClickableAsset>()
            .Where(asset => asset is not AssetSFX { PlayIn3D: false })
            .ToArray();
        if (assets.Length == 0)
            return;
        currentView++;
        if (currentView >= assets.Length)
            currentView = 0;
        Program.Renderer.Camera.ShowAssetOnViewport(assets[currentView]);
    }

    private void buttonExportRaw_Click(object sender, EventArgs e)
    {
        if (listViewAssets.SelectedItems.Count == 0)
            return;

        var selected = selectionManager.SelectedAssets;
        if (selected.Count == 1)
        {
            var asset = selected[0];
            var assetFileName = asset.assetName;

            // Check to see if the extension is '.anm' and use 'assetFileName'
            if (Path.GetExtension(asset.assetFileName).ToLower().Equals(".anm") && !string.IsNullOrWhiteSpace(asset.assetFileName))
                assetFileName = Path.GetFileName(asset.assetFileName);

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = assetFileName
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                try
                {
                    var AHDR = asset.BuildAHDR(archive.Platform.Endianness());
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
                foreach (var asset in selected)
                    try
                    {
                        var assetFileName = asset.assetName;

                        // Check to see if the extension is '.anm' and use 'assetFileName'
                        if (Path.GetExtension(asset.assetFileName).ToLower().Equals(".anm") && !string.IsNullOrWhiteSpace(asset.assetFileName))
                            assetFileName = Path.GetFileName(asset.assetFileName);

                        var AHDR = asset.BuildAHDR(archive.Platform.Endianness());
                        File.WriteAllBytes(Path.Combine(saveFileDialog.FileName, assetFileName), AHDR.data);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unable to export asset raw data: " + ex.Message);
                    }
        }
    }

    private void buttonEditHeader_Click(object sender, EventArgs e)
    {
        var asset = selectionManager.SelectedAssets.FirstOrDefault();
        if (asset != null)
            EditHeader(asset);
    }

    private void buttonEditData_Click(object sender, EventArgs e)
    {
        OpenInternalEditors();
    }

    private void buttonMultiEdit_Click(object sender, EventArgs e)
    {
        Program.EditorManager.OpenMultiAssetEditor(selectionManager.SelectedAssets, MousePosition);
    }

    private void listViewAssets_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (programIsChangingStuff)
            return;

        toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";

        if (!PressedKeys.Contains(Keys.ControlKey) && !PressedKeys.Contains(Keys.ShiftKey))
            foreach (var ae in Program.ArchiveEditors.Where(ae => ae != this))
                ae.ClearSelectedAssets();

        selectionManager.SetSelected(listViewAssets.SelectedItems.Cast<ListViewItem>().Select(item => archive.GetAsset((uint)item.Tag)));

        SetMenuItemsEnabled();
    }

    private void listViewAssets_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        if (archive.TryGetAsset((uint)listViewAssets.Items[e.Index].Tag, out Asset asset))
            asset.isInvisible = e.NewValue != CheckState.Checked;
    }

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

    private void listViewAssets_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            contextMenuStrip_ListBoxAssets.Show(listViewAssets.PointToScreen(e.Location));
        }
    }

    private void TemplateToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var template = (AssetTemplate)((ToolStripItem)sender).Tag;
        Vector3 Position = Program.Renderer.Camera.Position + 3 * Program.Renderer.Camera.Forward;
        PlaceTemplate(template, Position);
        SetupAssetVisibilityButtons();
    }

    private void ToolStripMenuItem_CreateGroup_Click(object sender, EventArgs e)
    {
        BeginTransaction();
        var ids = selectionManager.SelectedAssetIDs.ToList();
        var group = (AssetGRUP)GenerateTemplateForArchive(AssetTemplate.Group, new Vector3())[0];
        group.AddItems([.. ids]);
        archive.AddNewAsset(group, SelectedLayerIndex);
        EndTransaction();

        if (archive.ContainsNewlyAddedAssetType([group.assetID]))
            PopulateAssetListAndComboBox();
        SetSelectedAsset(group);
        UpdateListView(group);
    }

    private void copyAssetNamesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var names = SelectedAssets.Select(a => a.assetName);
        Clipboard.SetText($"\"{string.Join("\", \"", names)}\"");
    }


    public event Action TransactionBegin;
    public void BeginTransaction() => TransactionBegin?.Invoke();
    public event Action TransactionEnd;
    public void EndTransaction() => TransactionEnd?.Invoke();
    private void AddToCustomSource(Asset asset) => textBoxFindAsset.AutoCompleteCustomSource.Add(asset.assetName);
    private void RemoveFromCustomSource(Asset asset) => textBoxFindAsset.AutoCompleteCustomSource.Remove(asset.assetName);

    private bool archiveRefreshPending = false;
    private ArchiveRefreshKind pendingArchiveRefresh;
    private readonly HashSet<uint> pendingAddedAssetIDs = [];
    private readonly HashSet<uint> pendingRemovedAssetIDs = [];
    private bool skipNextArchiveSelectionRefresh;
    private int? requestedLayerIndex;
    internal event Action<ArchiveRefreshKind> ArchiveStateChanged;

    private void ArchiveSubscriptions()
    {
        textBoxFindAsset.AutoCompleteCustomSource = [.. archive.Assets.Select(a => a.assetName)];

        archive.Subscriber.SubscribeToArchiveHeaderSet((_, _, _, _) => NotifyArchiveStateChanged(ArchiveRefreshKind.None));
        archive.Subscriber.SubscribeToModDateSet((_) => NotifyArchiveStateChanged(ArchiveRefreshKind.None));

        archive.Subscriber.SubscribeToAutoLayersChanged((_) => NotifyArchiveStateChanged(ArchiveRefreshKind.Layout | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerAdded((_) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerList | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerRemoved((_) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerList | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerSwapped((_, _) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerList | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerTypeChanged((_, _) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerMetadata | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerRenamed((_, _) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerMetadata | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerIndicesSet((_) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerList | ArchiveRefreshKind.Menus));
        archive.Subscriber.SubscribeToLayerAssetAdded((_, assetID) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerMetadata, assetID, true));
        archive.Subscriber.SubscribeToLayerAssetRemoved((_, assetID) => NotifyArchiveStateChanged(ArchiveRefreshKind.LayerMetadata, assetID, false));

        archive.Subscriber.SubscribeToAssetAdded(AddToCustomSource);
        archive.Subscriber.SubscribeToAssetRemoved(RemoveFromCustomSource);

        archive.Subscriber.SubscribeToAssetAdded((asset) =>
        {
            Program.EditorManager.TryRebindAssetEditor(archive, asset);
            NotifyArchiveStateChanged(ArchiveRefreshKind.AssetMutations | ArchiveRefreshKind.Menus, asset.assetID, true);
            historyManager.AddAsset(asset);
            if (asset is AssetRWTX rwtx)
                TextureConverter.EnableTextureForDisplay(rwtx);
            else if (asset is AssetRenderWareModel model)
                model.SetupModel(Program.Renderer);
            if (asset is IClickableAsset clickableAsset)
                RefreshReferencedTransformMatrices(clickableAsset);
            asset.PropertyChanged += (hard) => AssetPropertyChanged(asset, hard);
        });

        archive.Subscriber.SubscribeToAssetRemoved((asset) =>
        {
            NotifyArchiveStateChanged(ArchiveRefreshKind.AssetMutations | ArchiveRefreshKind.Menus, asset.assetID, false);
            historyManager.RemoveAsset(asset.assetID);
            if (asset is AssetRWTX rwtx)
                Program.TextureManager.RemoveTexture(rwtx.Name, archive, rwtx.assetID);
            selectionManager.Unselect(asset);
            if (!Program.EditorManager.IsAssetDataSwap(archive, asset))
                Program.EditorManager.CloseEditor(asset);
        });

        foreach (var asset in archive.Assets)
            asset.PropertyChanged += (hard) => AssetPropertyChanged(asset, hard);

        TransactionBegin = null;
        TransactionEnd = null;
        TransactionBegin += () => Program.UndoBuffer.BeginTransaction(this);
        TransactionEnd += () => Program.UndoBuffer.EndTransaction(this);

        archive.Subscriber.SetupUndoBuffer(this, Program.UndoBuffer);
    }

    internal void NotifyArchiveStateChanged(ArchiveRefreshKind refreshKind = ArchiveRefreshKind.Menus, uint? assetID = null, bool? added = null)
    {
        if (assetID.HasValue && added.HasValue)
        {
            if (added.Value)
                pendingAddedAssetIDs.Add(assetID.Value);
            else
                pendingRemovedAssetIDs.Add(assetID.Value);
        }

        ArchiveStateChanged?.Invoke(refreshKind);
    }

    private void HandleArchiveStateChanged(ArchiveRefreshKind refreshKind)
    {
        if (InvokeRequired)
        {
            if (!IsDisposed)
                BeginInvoke(() => HandleArchiveStateChanged(refreshKind));
            return;
        }

        UnsavedChanges = true;
        pendingArchiveRefresh |= refreshKind;

        if (refreshKind == ArchiveRefreshKind.None || archiveRefreshPending || IsDisposed || !IsHandleCreated)
            return;

        archiveRefreshPending = true;
        BeginInvoke(() =>
        {
            archiveRefreshPending = false;
            var pendingRefresh = pendingArchiveRefresh;
            var addedAssetIDs = pendingAddedAssetIDs.ToList();
            var removedAssetIDs = pendingRemovedAssetIDs.ToList();
            pendingArchiveRefresh = ArchiveRefreshKind.None;
            pendingAddedAssetIDs.Clear();
            pendingRemovedAssetIDs.Clear();
            RefreshArchiveView(pendingRefresh, addedAssetIDs, removedAssetIDs);
        });
    }

    private void RefreshArchiveView(ArchiveRefreshKind refreshKind, IEnumerable<uint> addedAssetIDs, IEnumerable<uint> removedAssetIDs)
    {
        if (archive == null || IsDisposed)
            return;

        bool refreshSelection = refreshKind.HasFlag(ArchiveRefreshKind.LayerList)
            || refreshKind.HasFlag(ArchiveRefreshKind.AssetMutations)
            || refreshKind.HasFlag(ArchiveRefreshKind.Layout);
        var selectedAssetIDs = refreshSelection ? SelectedAssetIDs.ToList() : null;
        int selectedLayer = requestedLayerIndex ?? SelectedLayerIndex;
        requestedLayerIndex = null;
        bool layoutChanged = refreshKind.HasFlag(ArchiveRefreshKind.Layout)
            || autoLayersToolStripMenuItem.Checked != archive.AutoLayers;

        if (layoutChanged)
        {
            SetAutoLayers();
        }
        else if (!archive.AutoLayers && refreshKind.HasFlag(ArchiveRefreshKind.LayerList))
        {
            PopulateLayerComboBox();
            if (selectedLayer >= 0 && selectedLayer < comboBoxLayers.Items.Count)
                SelectedLayerIndex = selectedLayer;
            else
                SelectFirstLayer();
            if (SelectedLayerIndex >= 0)
                ShowSelectedLayerType();
        }

        if (!archive.AutoLayers && refreshKind.HasFlag(ArchiveRefreshKind.LayerMetadata))
            RefreshLayerLabels();

        if (!archive.AutoLayers && refreshKind.HasFlag(ArchiveRefreshKind.AssetMutations) && !refreshKind.HasFlag(ArchiveRefreshKind.LayerList))
            RefreshLayerLabels();

        if (!layoutChanged && refreshKind.HasFlag(ArchiveRefreshKind.LayerList))
            PopulateAssetListAndComboBox();
        else if (!layoutChanged && refreshKind.HasFlag(ArchiveRefreshKind.AssetMutations))
            RefreshAssetMutations(addedAssetIDs, removedAssetIDs);

        if (refreshSelection && !skipNextArchiveSelectionRefresh)
            SetSelectedAssets(selectedAssetIDs);
        skipNextArchiveSelectionRefresh = false;
        if (refreshSelection || refreshKind.HasFlag(ArchiveRefreshKind.Layout))
            SetupAssetVisibilityButtons();
        if (refreshKind.HasFlag(ArchiveRefreshKind.Menus) || refreshSelection || refreshKind.HasFlag(ArchiveRefreshKind.Layout))
            SetMenuItemsEnabled();
    }

    public void AssetPropertyChanged(Asset asset, bool hard)
    {
        historyManager.FlushChangesToUndoBuffer(this, asset, hard);
        switch (asset)
        {
            case IRenderableAsset ira:
                ira.CreateTransformMatrix(Program.Renderer);
                break;
            case AssetRWTX rwtx:
                TextureConverter.EnableTextureForDisplay(rwtx);
                break;
            case AssetRenderWareModel model:
                model.SetupModel(Program.Renderer);
                break;
        }
        if (asset is IClickableAsset clickableAsset)
            RefreshReferencedTransformMatrices(clickableAsset);
        UpdateListView(asset);
        NotifyArchiveStateChanged();
    }

    internal void RefreshReferencedTransformMatrices(IClickableAsset asset)
    {
        foreach (var sfx in archive.Assets.OfType<AssetSFX>())
            if (sfx.UseAttach && ReferenceEquals(sfx.GetAttach?.Invoke(sfx.Attach), asset))
                sfx.CreateTransformMatrix(Program.Renderer);

        if (asset is AssetMRKR marker)
            foreach (var teleport in archive.Assets.OfType<DynaGObjectTeleport>())
                if (ReferenceEquals(teleport.GetMRKR?.Invoke(teleport.Marker), marker))
                    teleport.CreateTransformMatrix(Program.Renderer);
    }

    public void SubscribeToSelectedAssetsCollectionChanged(Action action) =>
        selectionManager.SelectedAssets.CollectionChanged += (_, _) => action();

    /// <summary>
    /// Opens a HIP file from the specified path.
    /// </summary>
    /// <param name="fileName">The HIP file to open</param>
    /// <param name="knownPlatform">The plataform which should be used in case one cannot be detected from the HIP file</param>
    private void OpenFile(string fileName, Platform knownPlatform = Platform.Unknown)
    {
        DisposeArchive(true);
        var progressBar = new ProgressBar("Opening " + Path.GetFileName(fileName));
        try
        {
            progressBar.Show(this);

            Program.Renderer.Assets.PauseMatrixRecalculation = true;

            HipFile hipFile = HipFile.FromPath(fileName);

            if (hipFile.HIPB.VersionMismatch)
                MessageBox.Show($"Expected: {Section_HIPB.CurrentVersion}\nGot: {hipFile.HIPB.Version}\nAdditional data will be skipped", "Newer HIPB version detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            while (hipFile.game == Game.Unknown)
                hipFile.game = ChooseGame.GetGame();

            if (hipFile.platform == Platform.Unknown && knownPlatform != Platform.Unknown)
                hipFile.platform = knownPlatform;
            while (hipFile.platform == Platform.Unknown)
                hipFile.platform = ChoosePlatformDialog.GetPlatform();

            progressBar.SetProgressBar(0, hipFile.DICT.ATOC.AHDRList.Count, 1);

            archive = new Archive.Archive(hipFile, fileName, true, out List<AssetError> errors, progressBar);

            if (errors.Count != 0)
                MessageBox.Show("There was an error loading the following assets and editing may have been disabled for them:\n" + string.Join("\n", errors),
                    $"{errors.Count} Asset(s) with errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            historyManager = new AssetHistoryManager(archive);
            foreach (var (assetID, data) in hipFile.DICT.ATOC.AHDRList.Select(ahdr => (ahdr.assetID, ahdr.data)))
                historyManager.AddAsset(assetID, data);

            ArchiveSubscriptions();

            toolStripStatusLabelCurrentFilename.Text = "File: " + fileName;
            Text = Path.GetFileName(fileName);

            SetMenuItemsEnabled();
            SetAutoLayers();
            SelectFirstLayer();

            Program.MainForm.SetToolStripItemName(this, Text);
            Program.MainForm.SetRecentOpenedArchives(fileName);
        }
        catch (Exception e)
        {
            MessageBox.Show($"There was an error opening the archive: {e.Message}");
        }
        finally
        {
            Program.Renderer.Assets.PauseMatrixRecalculation = false;
            OnEditorUpdate();
            progressBar.Close();
        }
    }

    public void Save()
    {
        SoundUtility_vgmstream.StopSound();

        if (archive.CurrentFilePath == null)
            SaveAs();
        else
        {
            var archiveToSave = archive;
            Task.Run(() =>
            {
                try
                {
                    archiveToSave.Save();
                    return (Exception)null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }).ContinueWith(task =>
            {
                if (IsDisposed)
                    return;

                if (task.Result != null)
                {
                    MessageBox.Show($"Error saving archive: {task.Result.Message}");
                    return;
                }

                UnsavedChanges = false;
                _updateFilesizeStatusBarItem();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    /// <summary>
    /// Saves on the calling thread (unlike Save, which writes the file in the background).
    /// </summary>
    /// <returns>Whether the archive is saved</returns>
    public bool SaveAndWait()
    {
        SoundUtility_vgmstream.StopSound();

        if (archive.CurrentFilePath == null)
            SaveAs();
        else
        {
            try
            {
                archive.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving archive: {ex.Message}");
                return false;
            }

            UnsavedChanges = false;
            _updateFilesizeStatusBarItem();
        }

        return !UnsavedChanges;
    }

    private void SaveAs()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog()
        {
            FileName = archive.CurrentFilePath,
            Filter = "HIP/HOP Files|*.hip;*.hop",
            Title = "Please choose a HIP or HOP file to save as"
        };
        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                archive.Save(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving archive: {ex.Message}");
                return;
            }

            UnsavedChanges = false;

            Text = Path.GetFileName(saveFileDialog.FileName);
            Program.MainForm.SetToolStripItemName(this, Text);
            Program.MainForm.SetRecentOpenedArchives(saveFileDialog.FileName);
            toolStripStatusLabelCurrentFilename.Text = "File: " + saveFileDialog.FileName;
        }

        _updateFilesizeStatusBarItem();
    }

    public void DisposeArchive(bool showProgressBar = false)
    {
        if (archive == null)
            return;
        ProgressBar progressBar = null;
        if (showProgressBar)
        {
            progressBar = new ProgressBar("Closing Archive");
            progressBar.SetProgressBar(0, archive.AssetCount, 1);
            progressBar.Show(this);
            archive.Subscriber.SubscribeToAssetRemoved((assetID) => progressBar.PerformStep());
        }

        archive.Subscriber.UnsubscribeFromAssetAdded(AddToCustomSource);
        archive.Subscriber.UnsubscribeFromAssetRemoved(RemoveFromCustomSource);
        textBoxFindAsset.AutoCompleteCustomSource.Clear();

        Program.UndoBuffer.RemoveActionsOfArchive(this);

        archive.Dispose();
        archive = null;
        progressBar?.Close();
    }

    public void CloseArchiveEditor()
    {
        if (AssetListDocked)
            Program.MainForm.UndockAssetList();

        if (verifyResult != null)
            if (!verifyResult.IsDisposed)
                verifyResult.Close();

        DisposeArchive(true);

        Program.MainForm.CloseArchiveEditor(this);
        Program.MainForm.UpdateTitleBar();
        Close();
        OnEditorUpdate();
    }

    public string GetCurrentFilePath()
    {
        if (string.IsNullOrWhiteSpace(archive?.CurrentFilePath))
            return "Empty";
        return archive.CurrentFilePath;
    }

    /// <summary>
    /// Updates the status bar to display the archive's filsize.
    /// </summary>
    private void _updateFilesizeStatusBarItem()
    {
        if (GetCurrentFilePath() == "Empty")
            return;

        long filesizeBytes = new FileInfo(GetCurrentFilePath()).Length;

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

    internal void SetMenuItemsEnabled()
    {
        if (archive == null)
            return;

        legacySaveToolStripMenuItem.Enabled = true;
        saveToolStripMenuItem.Enabled = true;
        saveAsToolStripMenuItem.Enabled = true;
        convertArchiveToolStripMenuItem.Enabled = true;
        layersToolStripMenuItem.Enabled = true;
        autoLayersToolStripMenuItem.Enabled = true;
        buildLayersToolStripMenuItem.Enabled = true;
        buildLegacyToolStripMenuItem.Enabled = true;
        mergeSimilarAssetsToolStripMenuItem.Enabled = true;
        applyScaleToolStripMenuItem.Enabled = true;
        verifyArchiveToolStripMenuItem.Enabled = true;
        hipHopFileToolStripMenuItem.Enabled = true;
        importHipArchiveToolStripMenuItem.Enabled = true;
        exportAssetsIniToolStripMenuItem.Enabled = true;
        texturesToolStripMenuItem.Enabled = true;
        soundsToolStripMenuItem.Enabled = true;
        addLayerToolStripMenuItem.Enabled = buttonAddLayer.Enabled = true;
        importSoundsToolStripMenuItem.Enabled = true;
        importRawSoundsToolStripMenuItem.Enabled = true;

        var canImportAssetsToLayer = archive.AutoLayers || (SelectedLayerIndex != -1);
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

        removeLayerToolStripMenuItem.Enabled = buttonRemoveLayer.Enabled = !archive.AutoLayers && SelectedLayerIndex != -1;
        moveLayerUpToolStripMenuItem.Enabled = buttonArrowUp.Enabled = !archive.AutoLayers && archive.LayerCount > 1 && SelectedLayerIndex > 0;
        moveLayerDownToolStripMenuItem.Enabled = buttonArrowDown.Enabled = !archive.AutoLayers && archive.LayerCount > 1 && SelectedLayerIndex != -1 && SelectedLayerIndex < archive.LayerCount - 1;

        var canEditAsset = listViewAssets.SelectedItems.Count != 0;
        toolStripMenuItem_Duplicate.Enabled = buttonDuplicateAsset.Enabled = canEditAsset;
        toolStripMenuItem_Copy.Enabled = buttonCopyAsset.Enabled = canEditAsset;
        toolStripMenuItem_Remove.Enabled = buttonRemoveAsset.Enabled = canEditAsset;
        toolStripMenuItem_ExportRaw.Enabled = buttonExportRawAsset.Enabled = canEditAsset;
        toolStripMenuItem_EditData.Enabled = buttonEditDataAsset.Enabled = canEditAsset;
        toolStripMenuItem_MultiEdit.Enabled = buttonMultiEditAsset.Enabled = canEditAsset;
        toolStripMenuItem_CreateGroup.Enabled = canEditAsset;
        toolStripMenuItem_View.Enabled = buttonView.Enabled = canEditAsset && selectionManager.SelectedAssets.Any(asset => asset is not AssetSFX { PlayIn3D: false });

        var canEditSingleAsset = listViewAssets.SelectedItems.Count == 1;
        toolStripMenuItem_EditHeader.Enabled = buttonEditAsset.Enabled = canEditSingleAsset;

        buildCollisionTreeForAllModelsToolStripMenuItem.Enabled = archive.ContainsAssetWithType(AssetType.Model);
        coll36toolStripMenuItem.Enabled = archive.Game >= Game.Incredibles;
        coll36sortTrianglesToolStripMenuItem.Enabled = archive.Game >= Game.Incredibles;
    }

    private void SetupAssetVisibilityButtons()
    {
        Program.MainForm.SetupAssetVisibilityButtons();
    }

    private void PopulateLayerTypeComboBox()
    {
        if (archive.AutoLayers)
            return;

        programIsChangingStuff = true;

        comboBoxLayerTypes.Items.Clear();
        if (archive.Game >= Game.Incredibles)
            comboBoxLayerTypes.Items.AddRange(Enum.GetValues(typeof(LayerType_TSSM)).Cast<object>().ToArray());
        else
            comboBoxLayerTypes.Items.AddRange(Enum.GetValues(typeof(LayerType_BFBB)).Cast<object>().ToArray());

        programIsChangingStuff = false;
    }

    public void PopulateLayerComboBox()
    {
        if (archive.AutoLayers)
            return;

        programIsChangingStuff = true;

        comboBoxLayers.BeginUpdate();
        comboBoxLayers.Items.Clear();
        for (int i = 0; i < archive.LayerCount; i++)
            comboBoxLayers.Items.Add(archive.LayerToString(i));
        comboBoxLayers.EndUpdate();

        programIsChangingStuff = false;
    }

    private void RefreshLayerLabels()
    {
        if (archive.AutoLayers)
            return;

        programIsChangingStuff = true;
        for (int i = 0; i < archive.LayerCount && i < comboBoxLayers.Items.Count; i++)
            comboBoxLayers.Items[i] = archive.LayerToString(i);
        programIsChangingStuff = false;

        if (SelectedLayerIndex >= 0)
            ShowSelectedLayerType();
    }

    private void ShowSelectedLayerType()
    {
        if (archive.Game >= Game.Incredibles)
            comboBoxLayerTypes.SelectedItem = (LayerType_TSSM)archive.GetLayerType(SelectedLayerIndex);
        else
            comboBoxLayerTypes.SelectedItem = (LayerType_BFBB)archive.GetLayerType(SelectedLayerIndex);
        if (!archive.LegacySave)
            renameLayerToolStripMenuItem.Enabled = true;
    }

    private void SelectFirstLayer()
    {
        comboBoxLayers.SelectedIndex = archive.GetFirstActiveLayerIndex();
    }

    public void PopulateAssetListAndComboBox()
    {
        programIsChangingStuff = true;

        PopulateAssetTypeComboBox(true);
        PopulateAssetList();

        programIsChangingStuff = false;
    }

    private void PopulateAssetTypeComboBox(bool selectAllOption)
    {
        comboBoxAssetTypes.Items.Clear();
        comboBoxAssetTypes.SelectedIndex = -1;
        if (archive.AutoLayers || SelectedLayerIndex != -1)
        {
            comboBoxAssetTypes.Items.Add(new AssetTypeContainer(AssetType.Null));
            comboBoxAssetTypes.Items.AddRange([.. archive.AssetTypesOnLayer(SelectedLayerIndex).Select(f => new AssetTypeContainer(f)).OrderBy(f => f.ToString())]);
            if (selectAllOption)
                comboBoxAssetTypes.SelectedIndex = 0;
        }
    }

    private void PopulateAssetList(AssetType type = AssetType.Null, IEnumerable<Asset> assets = null, bool select = false, IEnumerable<Asset> selectionAssets = null)
    {
        curSelectedType = type;
        programIsChangingStuff = true;
        listViewAssets.BeginUpdate();
        listViewAssets.ListViewItemSorter = null;
        listViewAssets.Items.Clear();

        assets ??= (archive.AutoLayers || comboBoxLayers.SelectedItem != null) ?
            [.. archive.GetLayerAssetIDs(SelectedLayerIndex).Select(archive.GetAsset).Where(asset => type == AssetType.Null || asset.assetType == type)] :
            [];

        var items = new ListViewItem[assets.Count()];
        int k = 0;
        foreach (var asset in assets)
            items[k++] = ListViewItemFromAsset(asset, select && selectionAssets.Contains(asset));

        listViewAssets.Items.AddRange(items);

        if (select)
        {
            int ensureVisible = -1;
            for (int i = 0; i < listViewAssets.Items.Count; i++)
                if (listViewAssets.Items[i].Selected)
                    ensureVisible = i;
            if (ensureVisible != -1)
                listViewAssets.EnsureVisible(ensureVisible);

            selectionManager.SetSelected(selectionAssets);
            Program.EditorManager.OpenAutoDockedEditors(selectionAssets, this);
        }

        listViewAssets.EndUpdate();
        programIsChangingStuff = false;

        toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";

        SetMenuItemsEnabled();
    }

    private ListViewItem ListViewItemFromAsset(Asset asset, bool selected)
    {
        ListViewItem item = new ListViewItem(asset.assetName)
        {
            Checked = !asset.isInvisible,
            Selected = selected,
            Tag = asset.assetID,
            ImageKey = ImageKey(asset),
        };

        item.SubItems.AddRange(
        [
            new ListViewItem.ListViewSubItem(item, asset.assetID.ToString("X8")),
            new ListViewItem.ListViewSubItem(item, asset.TypeString),
            new ListViewItem.ListViewSubItem(item, TreatInfo(asset.AssetInfo)),
            new ListViewItem.ListViewSubItem(item, asset.AssetInfoLinks),
        ]);

        return item;
    }

    private string TreatInfo(string info)
    {
        if (info.Length > 50)
            info = info.Substring(0, 47) + "...";
        return info;
    }

    public void ClearListItems(IEnumerable<uint> assetIDs)
    {
        programIsChangingStuff = true;
        var ids = assetIDs.ToHashSet();
        foreach (ListViewItem v in listViewAssets.Items)
            if (ids.Contains((uint)v.Tag))
                listViewAssets.Items.Remove(v);
        programIsChangingStuff = false;

        toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";
        SetMenuItemsEnabled();
    }

    public void UpdateListView(Asset asset)
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

    private string TreatColumnName(string name)
    {
        if (name.EndsWith("▲") || name.EndsWith("▼"))
            return name.Substring(0, name.Length - 2);
        return name;
    }

    private static ImageList assetImageList = null;

    public static void InitializeAssetIcons()
    {
        var path = Path.Combine(Application.StartupPath, "Resources", "Icons", "Assets");
        if (!Directory.Exists(path))
            return;

        assetImageList = new ImageList
        {
            ImageSize = new Size(20, 20),
            ColorDepth = ColorDepth.Depth32Bit
        };

        foreach (string file in Directory.GetFiles(path, "*.png"))
        {
            string key = Path.GetFileNameWithoutExtension(file);
            var image = Image.FromFile(file);
            if (Program.IsDarkMode)
                assetImageList.Images.Add(key, InvertImage(image));
            else
                assetImageList.Images.Add(key, image);
        }

        var keysConfig = Path.Combine(path, "Config.ini");
        if (File.Exists(keysConfig))
            ParseCustomImageKeys(File.ReadAllLines(keysConfig));
     }

    private static Bitmap InvertImage(Image source)
    {
        var result = new Bitmap(source.Width, source.Height);
        using var graphics = Graphics.FromImage(result);
        using var attributes = new ImageAttributes();
        var matrix = new ColorMatrix([
            [-1,  0,  0,  0, 0],
            [0, -1,  0,  0, 0],
            [0,  0, -1,  0, 0],
            [0,  0,  0,  1, 0],
            [1,  1,  1,  0, 1]]);

        attributes.SetColorMatrix(matrix);
        graphics.DrawImage(source, new System.Drawing.Rectangle(0, 0, result.Width, result.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
        return result;
    }

    private static string ImageKey(Asset asset)
    {
        if (assetImageList == null)
            return null;

        if (asset is AssetPLAT plat && SubtypeImageKeys.TryGetValue((asset.assetType, plat.PlatformType.ToString()), out string key))
            return key;
        if (asset is AssetTRIG trig && SubtypeImageKeys.TryGetValue((asset.assetType, trig.Shape.ToString()), out key))
            return key;
        if (asset is AssetBUTN butn && SubtypeImageKeys.TryGetValue((asset.assetType, butn.ActMethod.ToString()), out key))
            return key;

        if (CustomImageKeys.TryGetValue(asset.assetType, out key))
            return key;

        key = asset.assetType.ToString();

        return assetImageList.Images.ContainsKey(key) ? key : (assetImageList.Images.ContainsKey("Default") ? "Default" : null);
    }

    private static Dictionary<AssetType, string> CustomImageKeys = new Dictionary<AssetType, string>();
    private static Dictionary<(AssetType, string), string> SubtypeImageKeys = new Dictionary<(AssetType, string), string>();

    private static void ParseCustomImageKeys(string[] strings)
    {
        var customImageKeys = new Dictionary<AssetType, string>();
        var subtypeImageKeys = new Dictionary<(AssetType, string), string>();
        foreach (var line in strings)
        {
            var content = line.Split('#', 2)[0].Trim();
            if (content.Length == 0)
                continue;

            var separator = content.IndexOf('=');
            if (separator < 0)
                continue;

            var assetTypeName = content[..separator].Trim();
            var imageKey = content[(separator + 1)..].Trim();
            if (assetTypeName.Length == 0 || imageKey.Length == 0)
                continue;

            if (!assetImageList.Images.ContainsKey(imageKey))
                continue;

            var subtypeSeparator = assetTypeName.IndexOf('_');
            string subtype = null;
            if (subtypeSeparator >= 0)
            {
                subtype = assetTypeName[(subtypeSeparator + 1)..].Trim();
                assetTypeName = assetTypeName[..subtypeSeparator].Trim();
                if (subtype.Length == 0)
                    continue;
            }

            if (!Enum.TryParse(assetTypeName, out AssetType assetType) || !Enum.IsDefined(assetType))
                continue;

            if (subtype is null)
                customImageKeys[assetType] = imageKey;
            else
                subtypeImageKeys[(assetType, subtype)] = imageKey;
        }

        CustomImageKeys = customImageKeys;
        SubtypeImageKeys = subtypeImageKeys;
    }

    public static bool UpdateReferencesOnCopy = true;

    public static bool ReplaceAssetsOnPaste = false;

    public void PasteAssets()
    {
        if (!archive.AutoLayers && SelectedLayerIndex == -1)
        {
            MessageBox.Show("Please select a layer to paste your asset in!");
            return;
        }

        List<AssetClipboard> clipboard;
        try
        {
            clipboard = JsonConvert.DeserializeObject<List<AssetClipboard>>(Clipboard.GetText());
            if (clipboard == null || clipboard.Count == 0)
                throw new InvalidDataException("no assets found");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error pasting assets from clipboard: {ex.Message}. Are you sure you have assets copied?");
            return;
        }

        var assets = AssetService.ClipboardToAssets(archive.Platform, archive.Game, UpdateReferencesOnCopy, ReplaceAssetsOnPaste, archive.ContainsAsset, false, clipboard);
        AddMultipleAssets(assets);
    }

    private void AddMultipleAssets(IEnumerable<Asset> assets, bool isTemplate = false)
    {
        var containsNewlyAddedAssetType = !assets.Select(a => a.assetType).Distinct().All(archive.ContainsAssetWithType);

        BeginTransaction();

        var result = archive.AddMultipleAssets(assets, SelectedLayerIndex, !isTemplate && ReplaceAssetsOnPaste);

        EndTransaction();

        if (containsNewlyAddedAssetType)
            SetupAssetVisibilityButtons();

        SetSelectedAssets(result);
        skipNextArchiveSelectionRefresh = result.Count != 0;
    }

    public void RemoveAssets(IEnumerable<uint> assetIDs)
    {
        programIsChangingStuff = true;
        var types = assetIDs.Select(u => archive.GetAsset(u).assetType).ToHashSet();

        archive.RemoveAssets(assetIDs);

        if (!archive.AutoLayers && SelectedLayerIndex != -1)
            comboBoxLayers.Items[SelectedLayerIndex] = archive.LayerToString(SelectedLayerIndex);

        listViewAssets.BeginUpdate();
        foreach (ListViewItem v in listViewAssets.SelectedItems)
            listViewAssets.Items.Remove(v);
        listViewAssets.EndUpdate();
        programIsChangingStuff = false;

        if (types.Any(t => !archive.ContainsAssetWithType(t)))
        {
            SetupAssetVisibilityButtons();
            PopulateAssetListAndComboBox();
        }
        else
            SetMenuItemsEnabled();
    }

    public Asset EditHeader(Asset asset)
    {
        try
        {
            var oldLayer = archive.GetLayerFromAssetID(asset.assetID);
            var oldAssetID = asset.assetID;

            Section_AHDR AHDR = AssetHeader.GetAsset(asset.BuildAHDR(archive.Platform.Endianness()));

            if (AHDR == null)
                return null;

            while (oldAssetID != AHDR.assetID && archive.ContainsAsset(AHDR.assetID))
            {
                var dr = MessageBox.Show($"Archive already contains asset with id [{AHDR.assetID:X8}]: {archive.GetAsset(AHDR.assetID).assetName}. Do you wish to overwrite it? Select 'Yes' to overwrite, 'No' to return to the previous dialog, or 'Cancel' to discard the change.", "Asset already exists", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                    break;
                else if (dr == DialogResult.No)
                    AHDR = AssetHeader.GetAsset(AHDR);
                else if (dr == DialogResult.Cancel)
                    return null;
            }

            BeginTransaction();

            archive.RemoveAsset(oldAssetID, false);

            var result = archive.AddNewAsset(AHDR, asset.game, archive.Platform, oldLayer);

            if (UpdateReferencesOnCopy && oldAssetID != result.assetID)
                foreach (var ae in Program.ArchiveEditors)
                    ae.archive.Transformer.ReplaceReferences(oldAssetID, result.assetID);

            EndTransaction();

            for (int i = 0; i < listViewAssets.Items.Count; i++)
                if ((uint)listViewAssets.Items[i].Tag == oldAssetID)
                {
                    listViewAssets.Items.RemoveAt(i);
                    break;
                }

            AddToAssetList(result);
            SetSelectedAsset(result);

            return result;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to edit asset: " + ex.Message);
        }

        return null;
    }

    public void OpenInternalEditors()
    {
        Program.EditorManager.OpenAssetEditors(selectionManager.SelectedAssets, false, MousePosition);
    }

    public void OpenInternalEditors(IEnumerable<Asset> assets, bool openAnyway)
    {
        Program.EditorManager.OpenAssetEditors(assets, openAnyway, MousePosition);
    }

    public void DeleteSelectedAssets()
    {
        ButtonRemoveAsset_Click(null, null);
        foreach (ListViewItem v in listViewAssets.Items)
            if (v.Selected)
                v.Selected = false;
    }

    public void ClearSelectedAssets()
    {
        programIsChangingStuff = true;
        listViewAssets.SelectedIndices.Clear();
        selectionManager.UnselectAll();
        programIsChangingStuff = false;
    }

    public bool HasSelectedAssets => selectionManager.SelectedAssets.Any();

    public void SetSelectedAsset(Asset asset, bool addToSelection = false, bool toggleSelection = false)
    {
        var selected = new List<Asset>();
        if (addToSelection || toggleSelection)
            selected.AddRange(selectionManager.SelectedAssets);
        if (asset != null)
        {
            if (toggleSelection && selected.Contains(asset))
                selected.Remove(asset);
            else
                selected.Add(asset);
        }
        SetSelectedAssets(selected);
    }

    public void RemoveFromSelection(uint? assetID)
    {
        if (!assetID.HasValue || !archive.ContainsAsset(assetID.Value))
            return;
        var assets = selectionManager.SelectedAssets.ToList();
        if (assets.RemoveAll(a => a.assetID == assetID.Value) > 0)
            SetSelectedAssets(assets);
    }

    private void RefreshAssetMutations(IEnumerable<uint> addedAssetIDs, IEnumerable<uint> removedAssetIDs)
    {
        var removedIDs = removedAssetIDs.ToHashSet();
        if (removedIDs.Count != 0)
            ClearListItems(removedIDs);

        RefreshAssetTypeComboBoxIfNeeded();

        var addedAssets = addedAssetIDs
            .Distinct()
            .Where(archive.ContainsAsset)
            .Select(archive.GetAsset)
            .Where(IsAssetVisibleInCurrentList)
            .ToList();

        if (addedAssets.Count != 0)
        {
            var selectedIDs = SelectedAssetIDs.ToHashSet();
            AddAssetRows(addedAssets, selectedIDs);
        }

        toolStripStatusLabelSelectionCount.Text = $"{listViewAssets.SelectedItems.Count}/{listViewAssets.Items.Count} assets selected";
        SetMenuItemsEnabled();
    }

    private bool IsAssetVisibleInCurrentList(Asset asset)
    {
        if (curSelectedType != AssetType.Null && asset.assetType != curSelectedType)
            return false;

        if (archive.AutoLayers)
            return true;

        if (SelectedLayerIndex == -1)
            return false;

        return archive.GetLayerFromAssetID(asset.assetID) == SelectedLayerIndex;
    }

    private void RefreshAssetTypeComboBoxIfNeeded()
    {
        var expectedTypes = archive.AssetTypesOnLayer(SelectedLayerIndex).ToHashSet();
        var currentTypes = comboBoxAssetTypes.Items
            .Cast<AssetTypeContainer>()
            .Select(container => container.assetType)
            .Where(type => type != AssetType.Null)
            .ToHashSet();

        if (expectedTypes.SetEquals(currentTypes))
            return;

        var selectedType = curSelectedType;
        PopulateAssetTypeComboBox(false);

        if (selectedType != AssetType.Null && expectedTypes.Contains(selectedType))
        {
            comboBoxAssetTypes.SelectedItem = comboBoxAssetTypes.Items
                .Cast<AssetTypeContainer>()
                .FirstOrDefault(container => container.assetType == selectedType);
        }
        else if (selectedType != AssetType.Null)
        {
            curSelectedType = AssetType.Null;
            comboBoxAssetTypes.SelectedIndex = 0;
            PopulateAssetList();
        }
    }

    public void AddToAssetList(Asset asset)
    {
        AddToAssetList([asset]);
    }

    public void AddToAssetList(IEnumerable<Asset> assets)
    {
        var assetList = assets.ToList();
        AddAssetRows(assetList, assetList.Select(asset => asset.assetID).ToHashSet());
    }

    private void AddAssetRows(IEnumerable<Asset> assets, HashSet<uint> selectedIDs)
    {
        listViewAssets.BeginUpdate();
        listViewAssets.Items.AddRange([.. assets
            .Where(asset => !listViewAssets.Items.Cast<ListViewItem>().Any(item => (uint)item.Tag == asset.assetID))
            .Select(asset => ListViewItemFromAsset(asset, selectedIDs.Contains(asset.assetID)))]);
        listViewAssets.EndUpdate();
    }

    public static bool AlwaysListAllAssets { get; internal set; }

    public void SetSelectedAssets(IEnumerable<Asset> assets, bool clearIfEmpty = true)
    {
        var assetList = assets?.ToList() ?? [];

        programIsChangingStuff = true;
        if (clearIfEmpty && (assetList.Count == 0 || assetList.Any(a => !archive.ContainsAsset(a.assetID))))
        {
            selectionManager.UnselectAll();
            listViewAssets.SelectedIndices.Clear();
            programIsChangingStuff = false;
            SetMenuItemsEnabled();
            return;
        }

        if (assetList.Count == 0)
        {
            programIsChangingStuff = false;
            return;
        }

        AssetType assetType = AssetType.Null;
        var first = assetList[0];

        if (!AlwaysListAllAssets && assetList.All(a => a.assetType == first.assetType))
            assetType = first.assetType;

        int layerIndex = archive.AutoLayers ? -1 : archive.GetLayerFromAssetID(first.assetID);
        bool layerChanged = !archive.AutoLayers && layerIndex != SelectedLayerIndex;

        if (!layerChanged && curSelectedType == assetType)
        {
            SelectListedAssets(assetList, addMissingAssets: true);

            programIsChangingStuff = false;
            return;
        }

        if (layerChanged)
        {
            comboBoxLayers.SelectedIndex = layerIndex;
            ShowSelectedLayerType();
            PopulateAssetTypeComboBox(false);
        }

        comboBoxAssetTypes.SelectedItem = comboBoxAssetTypes.Items.Cast<AssetTypeContainer>().FirstOrDefault(atc => atc.assetType == assetType);

        PopulateAssetList(assetType, null, true, assetList);
        SetMenuItemsEnabled();
        programIsChangingStuff = false;
    }

    public void SetSelectedAssets(IEnumerable<uint> assetIDs)
    {
        var assets = assetIDs.Where(archive.ContainsAsset).Select(archive.GetAsset);
        SetSelectedAssets(assets);
    }

    /// <summary>
    /// Selects <paramref name="assets"/> among the rows in the asset list.
    /// </summary>
    /// <param name="addMissingAssets">Adds rows for assets that are not currently listed before selecting them.</param>
    /// <returns>False, leaving the selection untouched, if any asset has no row in the list.</returns>
    private bool SelectListedAssets(IReadOnlyList<Asset> assets, bool addMissingAssets = false)
    {
        var wanted = assets.Select(asset => asset.assetID).ToHashSet();

        if (addMissingAssets)
        {
            var listedAssetIDs = listViewAssets.Items
                .Cast<ListViewItem>()
                .Select(item => (uint)item.Tag)
                .ToHashSet();
            var missingAssets = assets.Where(asset => !listedAssetIDs.Contains(asset.assetID)).ToList();

            if (missingAssets.Count != 0)
                AddAssetRows(missingAssets, new HashSet<uint>());
        }

        var indices = new List<int>(wanted.Count);
        for (int i = 0; i < listViewAssets.Items.Count; i++)
            if (wanted.Contains((uint)listViewAssets.Items[i].Tag))
                indices.Add(i);

        if (indices.Count != wanted.Count)
            return false;

        programIsChangingStuff = true;

        listViewAssets.BeginUpdate();
        listViewAssets.SelectedIndices.Clear();
        foreach (int i in indices)
            listViewAssets.SelectedIndices.Add(i);
        listViewAssets.EndUpdate();
        listViewAssets.EnsureVisible(indices[0]);

        programIsChangingStuff = false;
        selectionManager.SetSelected(assets);
        Program.EditorManager.OpenAutoDockedEditors(assets, this);
        SetMenuItemsEnabled();

        return true;
    }

    private void BuildLayersClick(bool legacy)
    {
        if (archive.AutoLayers)
            return;

        BeginTransaction();
        if (archive.Transformer.BuildLayers(legacy))
        {
            EndTransaction();
            PopulateLayerComboBox();
            SelectFirstLayer();
            PopulateAssetListAndComboBox();
        }
    }

    public List<Asset> PlaceTemplate(AssetTemplate template, Vector3 position)
    {
        if (!archive.AutoLayers && comboBoxLayers.SelectedIndex == -1)
        {
            MessageBox.Show("Please select a layer to place your asset in!");
            return null;
        }

        var assets = GenerateTemplateForArchive(template, position);
        if (assets == null)
            return null;

        AddMultipleAssets(assets, true);
        return assets;
    }

    private List<Asset> GenerateTemplateForArchive(AssetTemplate template, Vector3 position)
    {
        List<Asset> assets;

        if (template == AssetTemplate.Paste_Clipboard || template == AssetTemplate.User_Template)
        {
            List<AssetClipboard> clipboard;
            try
            {
                string text = template switch
                {
                    AssetTemplate.Paste_Clipboard => Clipboard.GetText(),
                    AssetTemplate.User_Template => File.ReadAllText(Path.Combine(UserTemplateManager.userTemplatesFolder, Program.MainForm.CurrentUserTemplate)),
                };
                clipboard = JsonConvert.DeserializeObject<List<AssetClipboard>>(text);
                if (clipboard == null || clipboard.Count == 0)
                    throw new InvalidDataException("no assets found");
            }
            catch (Exception ex)
            {
                var errorMessage = template switch
                {
                    AssetTemplate.Paste_Clipboard => $"Error pasting assets from clipboard: {ex.Message}",
                    AssetTemplate.User_Template => $"Error placing user template: {ex.Message}",
                };
                MessageBox.Show(errorMessage);
                return null;
            }

            assets = AssetService.ClipboardToAssets(archive.Platform, archive.Game, true, false, archive.ContainsAsset, true, clipboard);
            AssetTransformer.SetAssetPositionsPreserveRelative(assets.OfType<IClickableAsset>(), position);
        }
        else
        {
            assets = AssetFactory.GenerateTemplate(archive.Game, archive.Platform, archive.ContainsAsset, template, position: position);
        }
        return assets;
    }

    public void SetTargetedOff()
    {
        checkBoxTemplateFocus.Checked = false;
    }

    public void SetTargetedOn()
    {
        Program.ArchiveManager.ClearTemplateFocus();
        checkBoxTemplateFocus.Checked = true;
        if (Program.MainForm.AutoDockArchiveEditors && Program.MainForm.ShowToolbar)
            Program.MainForm.DockAssetList(this);
    }

    private void ImportTXD(bool RW3)
    {
        OpenFileDialog openTXD = new OpenFileDialog() { Filter = "TXD archives|*.txd" };

        if (openTXD.ShowDialog() == DialogResult.OK)
        {
            BeginTransaction();

            var AHDRs = TextureConverter.GetAssetsFromTextureDictionary(openTXD.FileName, RW3, archive.Game);
            archive.ImportAssets(AHDRs, SelectedLayerIndex, true);

            EndTransaction();

            PopulateLayerComboBox();
            SelectFirstLayer();
            PopulateAssetListAndComboBox();
            OnEditorUpdate();
        }
    }

    private void ExportTXD(CheckState RW3)
    {
        SaveFileDialog saveTXD = new SaveFileDialog() { Filter = "TXD archives|*.txd" };

        if (saveTXD.ShowDialog() == DialogResult.OK)
            TextureConverter.ExportTextureDictionary(archive.Assets.OfType<AssetRWTX>(), saveTXD.FileName, RW3);
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

    private bool autoLayersForGroupBox = false;

    internal void SetAutoLayers()
    {
        if (archive.AutoLayers)
        {
            autoLayersToolStripMenuItem.Checked = true;
            renameLayerToolStripMenuItem.Enabled = false;

            groupBoxLayers.Visible = false;

            if (!autoLayersForGroupBox)
            {
                groupBoxAssets.Location = new System.Drawing.Point(groupBoxAssets.Location.X, groupBoxAssets.Location.Y - 53);
                groupBoxAssets.Size = new System.Drawing.Size(groupBoxAssets.Size.Width, groupBoxAssets.Size.Height + 53);
                autoLayersForGroupBox = true;
            }
        }
        else
        {
            autoLayersToolStripMenuItem.Checked = false;

            groupBoxLayers.Visible = true;

            if (autoLayersForGroupBox)
            {
                groupBoxAssets.Location = new System.Drawing.Point(groupBoxAssets.Location.X, groupBoxAssets.Location.Y + 53);
                groupBoxAssets.Size = new System.Drawing.Size(groupBoxAssets.Size.Width, groupBoxAssets.Size.Height - 53);
                autoLayersForGroupBox = false;
            }

            PopulateLayerTypeComboBox();
            PopulateLayerComboBox();
            SelectFirstLayer();
        }

        PopulateAssetListAndComboBox();
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
            var progressBar = new ProgressBar("Import Sounds");
            progressBar.SetProgressBar(0, openFile.FileNames.Count(), 1);
            progressBar.Show(this);

            try
            {
                BeginTransaction();
                var assets = archive.ImportSounds(raw, openFile.FileNames, assetType, overwriteSoundsOnImportToolStripMenuItem.Checked, progressBar.PerformStep);
                if (assets.Any())
                {
                    EndTransaction();
                    OnEditorUpdate();
                    PopulateLayerComboBox();
                    comboBoxLayers.SelectedIndex = archive.GetOrCreateLayerOfType(LayerType.SRAM);
                    SetSelectedAssets(assets);
                    SetMenuItemsEnabled();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error importing sounds: {e.Message}");
            }
            finally
            {
                progressBar.Close();
            }
        }
        Enabled = true;
    }

    public void HideSelectedAssets()
    {
        foreach (ListViewItem item in listViewAssets.SelectedItems)
            if (item.Checked)
            {
                item.Checked = false;
                item.Selected = false;
            }
    }

    public void HideAssets(IEnumerable<uint> assetIDs)
    {
        foreach (var u in assetIDs)
            if (archive.TryGetAsset(u, out Asset asset) && !asset.isInvisible)
                asset.isInvisible = true;
    }

    public bool UnhideAllAssets()
    {
        bool unhid = false;
        foreach (var asset in archive.Assets)
            if (asset.isInvisible)
            {
                asset.isInvisible = false;
                unhid = true;
            }
        foreach (ListViewItem item in listViewAssets.Items)
            if (!item.Checked)
                item.Checked = true;
        return unhid;
    }

    private sealed class ControlLayout
    {
        public required Control Parent { get; init; }
        public required int Index { get; init; }
        public required DockStyle Dock { get; init; }
        public required System.Drawing.Rectangle Bounds { get; init; }
        public required AnchorStyles Anchor { get; init; }
        public required Padding Margin { get; init; }
    }

    private readonly Dictionary<Control, ControlLayout> originalLayouts = new();
    private TableLayoutPanel assetListContainer = new TableLayoutPanel
    {
        Dock = DockStyle.Fill,
        ColumnCount = 1,
        RowCount = 3,
        Margin = Padding.Empty,
        Padding = Padding.Empty
    };

    public void MoveAssetListToControl(Panel destination)
    {
        if (AssetListDocked)
            return;

        Control[] controls = [comboBoxAssetTypes, textBoxFindAsset, listViewAssets];

        originalLayouts.Clear();

        foreach (Control control in controls)
            originalLayouts[control] = new ControlLayout
            {
                Parent = control.Parent,
                Index = control.Parent.Controls.GetChildIndex(control),
                Dock = control.Dock,
                Bounds = control.Bounds,
                Anchor = control.Anchor,
                Margin = control.Margin
            };

        assetListContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        assetListContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        assetListContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        assetListContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        foreach (Control control in controls)
            control.Parent.Controls.Remove(control);

        destination.Controls.Add(assetListContainer);

        comboBoxAssetTypes.Dock = DockStyle.Fill;
        textBoxFindAsset.Dock = DockStyle.Fill;
        listViewAssets.Dock = DockStyle.Fill;

        comboBoxAssetTypes.Margin = new Padding(0);
        textBoxFindAsset.Margin = new Padding(0);
        listViewAssets.Margin = new Padding(0);

        assetListContainer.Controls.Add(comboBoxAssetTypes, 0, 0);
        assetListContainer.Controls.Add(textBoxFindAsset, 0, 1);
        assetListContainer.Controls.Add(listViewAssets, 0, 2);

        listViewAssets.KeyDown += ArchiveEditor_KeyDown;
        listViewAssets.KeyUp += ArchiveEditor_KeyUp;
        listViewAssets.LostFocus += ArchiveEditor_Deactivate;

        AssetListDocked = true;
        Hide();
    }

    public void MoveAssetListBack(Control origin)
    {
        if (!AssetListDocked)
            return;

        Control[] controls = [comboBoxAssetTypes, textBoxFindAsset, listViewAssets];

        foreach (Control control in controls)
            assetListContainer.Controls.Remove(control);

        origin.Controls.Remove(assetListContainer);

        foreach (Control control in controls)
        {
            ControlLayout layout = originalLayouts[control];
            layout.Parent.Controls.Add(control);
            control.Dock = layout.Dock;
            control.Anchor = layout.Anchor;
            control.Margin = layout.Margin;
            control.Bounds = layout.Bounds;
        }

        foreach (Control control in controls.OrderBy(c => originalLayouts[c].Index))
        {
            ControlLayout layout = originalLayouts[control];
            layout.Parent.Controls.SetChildIndex(control, layout.Index);
        }

        originalLayouts.Clear();

        listViewAssets.KeyDown -= ArchiveEditor_KeyDown;
        listViewAssets.KeyUp -= ArchiveEditor_KeyUp;
        listViewAssets.LostFocus -= ArchiveEditor_Deactivate;

        AssetListDocked = false;
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AssetListDocked
    {
        get;
        set
        {
            field = value;
            SetDockedMenuItemsVisibility(value);
        }
    } = false;

    private void SetDockedMenuItemsVisibility(bool value)
    {
        fileToolStripMenuItem1.Visible = value;
        editToolStripMenuItem1.Visible = value;
        toolsToolStripMenuItem1.Visible = value;
        toolStripSeparatorExtra1.Visible = value;

        addLayerToolStripMenuItem.Visible = value;
        removeLayerToolStripMenuItem.Visible = value;
        moveLayerUpToolStripMenuItem.Visible = value;
        moveLayerDownToolStripMenuItem.Visible = value;
        toolStripSeparatorLayerEditBox.Visible = value;
    }
}
using HipHopFile;
using IndustrialPark.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class InternalAssetEditor : InternalEditor
{
    public InternalAssetEditor(Asset asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
    {
        InitializeComponent();

        this.asset = asset;
        this.archive = archive;
        this.updateListView = updateListView;

        propertyGridAsset.SelectedObject = DynamicTypeDescriptor.Create(asset);

        Text = asset.assetName;

        viewToolStripMenuItem.Visible = asset is IClickableAsset;

        switch (asset)
        {
            case AssetCAM cam:
                SetupForCam(cam);
                break;
            case AssetCSN csn:
                SetupForCsn(csn);
                break;
            case IAssetAddSelected aas:
                SetupForAddSelected(aas);
                break;
            case AssetSCRP scrp:
                SetupForScrp(scrp);
                break;
            case AssetWIRE wire:
                SetupForWire(wire);
                break;
            case AssetFOG fog:
                SetupForFog(fog);
                break;
            case AssetJSP_INFO info:
                SetupForJSPInfo(info);
                break;
        }

        if (asset is IAssetCopyPasteTransformation iacpt)
            SetupForCopyPasteTransformation(iacpt);

        if (asset is EntityAsset entity && !new AssetType[] {
            AssetType.Trigger,
            AssetType.Pickup,
            AssetType.Player,
            AssetType.UserInterface,
            AssetType.UserInterfaceFont
        }.Contains(asset.assetType))
            SetupForBakeScaleRot(entity);

        AddRow();

        AddTool("Find who targets me", (object sender, EventArgs e) => Program.MainForm.FindWhoTargets(GetAssetID()));

        AddTool("Open wiki page", (object sender, EventArgs e) =>
        {
            ArchiveEditorFunctions.OpenWikiPage(asset);
        });

        for (int i = 1; i < tableLayoutPanel1.RowCount; i++)
        {
            var rs = new RowStyle(SizeType.Absolute, RowSizes[i]);
            if (i < tableLayoutPanel1.RowStyles.Count)
                tableLayoutPanel1.RowStyles[i] = rs;
            else
                tableLayoutPanel1.RowStyles.Add(rs);
        }

        SelectFirstProperty(propertyGridAsset.SelectedGridItem);
    }

    public override bool CanBeDocked => true;

    private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
    {
        archive.CloseInternalEditor(this);
    }

    private readonly Asset asset;

    public override uint GetAssetID()
    {
        return asset.assetID;
    }

    public override void RefreshPropertyGrid()
    {
        Task.Run(() =>
        {
            Invoke(() =>
            {
                propertyGridAsset.Refresh();
                updateListView(asset);
            });
        });
    }

    private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        archive.UnsavedChanges = true;
        RefreshPropertyGrid();
        Program.UndoBuffer.AddAction(GetPropertyChangedAction(archive, asset, e.OldValue, e.ChangedItem.Value, e.ChangedItem));
    }

    private void editHeaderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var ae = archive.GetArchiveEditor();
        if (ae.EditHeader(asset.assetID, out uint newAssetID))
            ae.OpenInternalEditor(newAssetID);
    }

    private void viewToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (asset is IClickableAsset ica)
            Program.Renderer.Camera.ShowAssetOnViewport(ica);
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        archive.CopyAssetsToClipboard(asset.assetID);
    }

    private void removeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show($"Are you sure you want to delete asset [{asset.assetID:X8}] {asset.assetName} ({AssetTypeContainer.AssetTypeToString(asset.assetType)})?", "Confirm asset removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.Yes)
        {
            var actions = new List<IReversibleAction>();
            archive.GetArchiveEditor().RemoveAssets([asset.assetID], actions);
            Program.UndoBuffer.AddAction(actions);
        }
    }

    private readonly List<int> RowSizes = new List<int>() { -1 };
    private const int ButtonSize = 28;

    private int AddRow()
    {
        if (!EnableAssetToolButtons)
            return -1;
        tableLayoutPanel1.RowCount += 1;
        RowSizes.Add(ButtonSize);
        return tableLayoutPanel1.RowCount - 1;
    }

    private (Button button, ToolStripMenuItem menuItem) AddTool(string text, EventHandler click, bool doubleSpan = false)
    {
        Button button = null;
        if (EnableAssetToolButtons)
        {
            button = new Button() { Dock = DockStyle.Fill, Text = text, AutoSize = true };
            button.Click += click;
            tableLayoutPanel1.Controls.Add(button);
            if (doubleSpan)
                tableLayoutPanel1.SetColumnSpan(button, 2);
        }

        var menuItem = new ToolStripMenuItem(text);
        menuItem.Click += click;
        this.toolsToolStripMenuItem.DropDownItems.Add(menuItem);

        return (button, menuItem);
    }

    private void SetupForJSPInfo(AssetJSP_INFO asset)
    {
        AddRow();
        AddTool("Generate JSPINFO", (s, e) =>
        {
            archive.GenerateJSPInfo(asset);
            RefreshPropertyGrid();
            archive.UnsavedChanges = true;
        });
    }

    private void SetupForCam(AssetCAM asset)
    {
        AddRow();

        AddTool("Get View Position", (object sender, EventArgs e) =>
        {
            asset.SetPosition(Program.MainForm.renderer.Camera.Position);
            RefreshPropertyGrid();
            archive.UnsavedChanges = true;
        });

        AddTool("Get View Direction", (object sender, EventArgs e) =>
        {
            asset.SetNormalizedForward(Program.MainForm.renderer.Camera.Forward);
            asset.SetNormalizedUp(Program.MainForm.renderer.Camera.Up);
            asset.SetNormalizedLeft(Program.MainForm.renderer.Camera.Right);

            RefreshPropertyGrid();
            archive.UnsavedChanges = true;
        });
    }

    private void SetupForAddSelected(IAssetAddSelected asset)
    {
        AddRow();
        AddTool("Add selected to " + asset.GetItemsText, (object sender, EventArgs e) =>
        {
            asset.AddItems([.. archive.CurrentlySelectedAssetIDs]);
            RefreshPropertyGrid();
            archive.UnsavedChanges = true;
        }, true);
    }

    private void SetupForFog(AssetFOG asset)
    {
        AddRow();
        AddTool("Preview Fog", (object sender, EventArgs e) =>
        {
            SharpRenderer.Fog = asset;
        }, true);
    }

    private void SetupForScrp(AssetSCRP asset)
    {
        AddRow();
        AddTool("Convert to group of timers", (object sender, EventArgs e) =>
        {
            var newAssets = archive.ConvertScriptToGroupOfTimers(asset);
            archive.UnsavedChanges = true;
            Program.MainForm.SetSelectedIndices(newAssets);
        }, true);
    }

    private void SetupForCsn(AssetCSN asset)
    {
        AddRow();
        AddTool("Export All", (object sender, EventArgs e) =>
        {
            using var openFolder = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFolder.ShowDialog() == CommonFileDialogResult.Ok)
                asset.ExtractToFolder(openFolder.FileName, archive.platform.Endianness());
        }, true);
    }

    private void SetupForWire(AssetWIRE asset)
    {
        AddRow();

        AddTool("Import", (object sender, EventArgs e) =>
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                asset.FromObj(openFile.FileName);
                archive.UnsavedChanges = true;
            }
        });

        AddTool("Export", (object sender, EventArgs e) =>
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
                FileName = asset.assetName
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
                asset.ToObj(saveFile.FileName);
        });
    }

    private void SetupForCopyPasteTransformation(IAssetCopyPasteTransformation asset)
    {
        AddRow();

        AddTool("Copy Transformation", (object sender, EventArgs e) =>
        {
            asset.CopyTransformation();
        });

        AddTool("Paste Transformation", (object sender, EventArgs e) =>
        {
            asset.PasteTransformation();
            propertyGridAsset.Refresh();
        });
    }

    private void SetupForBakeScaleRot(EntityAsset asset)
    {
        AddRow();

        var toolTip = new ToolTip();
        (var button, var menuItem) = AddTool("Bake Rotation", (object sender, EventArgs e) =>
        {
            var result = MessageBox.Show(Resources.BakeRotationDescription, "Bake Rotation", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                asset.ApplyBakeRotation();
                propertyGridAsset.Refresh();
            }
        });
        void showToolTipBakeRotation(object sender, EventArgs e) => toolTip.Show(Resources.BakeRotationDescription, button);
        button?.MouseHover += showToolTipBakeRotation;
        menuItem?.MouseHover += showToolTipBakeRotation;

        (button, menuItem) = AddTool("Bake Scale", (object sender, EventArgs e) =>
        {
            var result = MessageBox.Show(Resources.BakeScaleDescription, "Bake Scale", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                asset.ApplyBakeScale();
                propertyGridAsset.Refresh();
            }
        });
        void showToolTipBakeScale(object sender, EventArgs e) => toolTip.Show(Resources.BakeScaleDescription, button);
        button?.MouseHover += showToolTipBakeScale;
        menuItem?.MouseHover += showToolTipBakeScale;
    }
}

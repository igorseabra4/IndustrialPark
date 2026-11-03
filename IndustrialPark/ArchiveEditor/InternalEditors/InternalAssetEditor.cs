using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark;

public partial class InternalAssetEditor : InternalEditor
{
    public InternalAssetEditor(Asset asset, ArchiveEditor archive) : base(asset, archive)
    {
        InitializeComponent();

        SetAsset(asset);

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

        AddTool("Find who targets me", (_, _) => Program.EditorManager.OpenWhoTargetsAssetEditors(GetAssetID()));
        AddTool("Open wiki page", (_, _) => OpenWikiPage());

        for (int i = 1; i < tableLayoutPanel1.RowCount; i++)
        {
            var rs = new RowStyle(SizeType.Absolute, RowSizes[i]);
            if (i < tableLayoutPanel1.RowStyles.Count)
                tableLayoutPanel1.RowStyles[i] = rs;
            else
                tableLayoutPanel1.RowStyles.Add(rs);
        }

        viewToolStripMenuItem.Visible = asset is IClickableAsset;

        SelectFirstProperty(propertyGridAsset.SelectedGridItem);
    }

    public void SetAsset(Asset asset)
    {
        if (this.asset != null && this.asset.assetType != asset.assetType)
            throw new ArgumentException("Cannot change the asset type of an InternalAssetEditor.");

        this.asset = asset;
        propertyGridAsset.SelectedObject = DynamicTypeDescriptor.Create(asset);
    }

    public override void RefreshPropertyGrid()
    {
        Invoke(propertyGridAsset.Refresh);
    }

    private void propertyGridAsset_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        asset.InvokePropertyChanged(true);
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
        AddTool("Generate JSPInfo", (s, e) =>
        {
            var jsps = archiveEditor.archive.Assets.OfType<AssetJSP>().Where(a => asset.JSP_AssetIDs.Contains(a.assetID)).ToList();
            JSPService.GenerateJSPInfo(archiveEditor.archive.Game, archiveEditor.archive.Platform, asset, jsps);
            asset.InvokePropertyChanged(true);
        });
    }

    private void SetupForCam(AssetCAM asset)
    {
        AddRow();

        AddTool("Get View Position", (_, _) =>
        {
            asset.SetPosition(Program.Renderer.Camera.Position);
            asset.InvokePropertyChanged(true);
        });

        AddTool("Get View Direction", (_, _) =>
        {
            asset.SetNormalizedForward(Program.Renderer.Camera.Forward);
            asset.SetNormalizedUp(Program.Renderer.Camera.Up);
            asset.SetNormalizedLeft(Program.Renderer.Camera.Right);
            asset.InvokePropertyChanged(true);
        });
    }

    private void SetupForAddSelected(IAssetAddSelected asset)
    {
        AddRow();
        AddTool($"Select {asset.GetItemsText} assets", (_, _) =>
        {
            Program.ArchiveManager.SetSelectedAssets(asset.GetItems(), false);
        });
        AddTool($"Add selected to {asset.GetItemsText}", (_, _) =>
        {
            asset.AddItems([.. archiveEditor.SelectedAssetIDs]);
            (asset as Asset).InvokePropertyChanged(true);
        });
    }

    private void SetupForFog(AssetFOG asset)
    {
        AddRow();
        AddTool("Preview Fog", (_, _) =>
        {
            Program.Renderer.Fog = asset;
        }, true);
    }

    private void SetupForScrp(AssetSCRP asset)
    {
        AddRow();
        AddTool("Convert to group of timers", (_, _) =>
        {
            var newAssets = AssetFactory.ConvertScriptToGroupOfTimers(archiveEditor.archive.Platform, asset, archiveEditor.archive.ContainsAsset);

            archiveEditor.BeginTransaction();
            var layer = archiveEditor.archive.GetOrCreateLayerOfType(LayerType.DEFAULT);
            archiveEditor.archive.AddMultipleAssets(newAssets, layer, false);
            archiveEditor.EndTransaction();

            Program.ArchiveManager.SetSelectedAssets(newAssets);
        }, true);
    }

    private void SetupForCsn(AssetCSN asset)
    {
        AddRow();
        AddTool("Export All", (_, _) =>
        {
            using var openFolder = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFolder.ShowDialog() == CommonFileDialogResult.Ok)
                asset.ExtractToFolder(openFolder.FileName, archiveEditor.archive.Platform.Endianness());
        }, true);
    }

    private void SetupForWire(AssetWIRE asset)
    {
        AddRow();

        AddTool("Import", (_, _) =>
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
            };

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                asset.FromObj(openFile.FileName);
                asset.InvokePropertyChanged(true);
            }
        });

        AddTool("Export", (_, _) =>
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

        AddTool("Copy Transformation", (_, _) =>
        {
            asset.CopyTransformation();
        });

        AddTool("Paste Transformation", (_, _) =>
        {
            asset.PasteTransformation();
            (asset as Asset).InvokePropertyChanged(true);
        });
    }

    private void SetupForBakeScaleRot(EntityAsset asset)
    {
        AddRow();

        var toolTip = new ToolTip();
        (var button, var menuItem) = AddTool("Bake Rotation", (_, _) =>
        {
            var result = MessageBox.Show(Resources.BakeRotationDescription, "Bake Rotation", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                asset.ApplyBakeRotation();
                asset.InvokePropertyChanged(true);
            }
        });
        button?.MouseHover += (_, _) => toolTip.Show(Resources.BakeRotationDescription, button);
        menuItem?.MouseHover += (_, _) => toolTip.Show(Resources.BakeRotationDescription, button);

        (button, menuItem) = AddTool("Bake Scale", (_, _) =>
        {
            var result = MessageBox.Show(Resources.BakeScaleDescription, "Bake Scale", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                asset.ApplyBakeScale();
                asset.InvokePropertyChanged(true);
            }
        });
        button?.MouseHover += (_, _) => toolTip.Show(Resources.BakeScaleDescription, button);
        menuItem?.MouseHover += (_, _) => toolTip.Show(Resources.BakeScaleDescription, button);
    }
}

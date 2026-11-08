using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.Properties;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
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
            case AssetCAM:
                SetupForCam();
                break;
            case AssetCSN:
                SetupForCsn();
                break;
            case IAssetAddSelected:
                SetupForAddSelected();
                break;
            case AssetSCRP:
                SetupForScrp();
                break;
            case AssetWIRE:
                SetupForWire();
                break;
            case AssetFOG:
                SetupForFog();
                break;
            case AssetJSP_INFO:
                SetupForJSPInfo();
                break;
        }

        if (asset is IAssetCopyPasteTransformation)
            SetupForCopyPasteTransformation();

        if (asset is EntityAsset entity && !new AssetType[] {
            AssetType.Trigger,
            AssetType.Pickup,
            AssetType.Player,
            AssetType.UserInterface,
            AssetType.UserInterfaceFont
        }.Contains(asset.assetType))
            SetupForBakeScaleRot();

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

        SelectFirstProperty(propertyGridAsset.SelectedGridItem);
    }

    protected override void OnAssetReplaced()
    {
        propertyGridAsset.SelectedObject = DynamicTypeDescriptor.Create(asset);
        viewToolStripMenuItem.Visible = asset is IClickableAsset && asset is not AssetSFX { PlayIn3D: false };
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

    private void SetupForJSPInfo()
    {
        AddRow();
        AddTool("Generate JSPInfo", (s, e) =>
        {
            var currentAsset = GetCurrentAsset<AssetJSP_INFO>();
            var jsps = archiveEditor.archive.Assets.OfType<AssetJSP>().Where(a => currentAsset.JSP_AssetIDs.Contains(a.assetID)).ToList();
            JSPService.GenerateJSPInfo(archiveEditor.archive.Game, archiveEditor.archive.Platform, currentAsset, jsps);
            currentAsset.InvokePropertyChanged(true);
        });
    }

    private void SetupForCam()
    {
        AddRow();

        AddTool("Get View Position", (_, _) =>
        {
            var currentAsset = GetCurrentAsset<AssetCAM>();
            currentAsset.SetPosition(Program.Renderer.Camera.Position);
            currentAsset.InvokePropertyChanged(true);
        });

        AddTool("Get View Direction", (_, _) =>
        {
            var currentAsset = GetCurrentAsset<AssetCAM>();
            currentAsset.SetNormalizedForward(Program.Renderer.Camera.Forward);
            currentAsset.SetNormalizedUp(Program.Renderer.Camera.Up);
            currentAsset.SetNormalizedLeft(Program.Renderer.Camera.Right);
            currentAsset.InvokePropertyChanged(true);
        });
    }

    private void SetupForAddSelected()
    {
        AddRow();
        AddTool($"Select {((IAssetAddSelected)this.asset).GetItemsText} assets", (_, _) =>
        {
            Program.ArchiveManager.SetSelectedAssets(((IAssetAddSelected)this.asset).GetItems(), false);
        });
        AddTool($"Add selected to {((IAssetAddSelected)this.asset).GetItemsText}", (_, _) =>
        {
            var currentAsset = (IAssetAddSelected)this.asset;
            currentAsset.AddItems([.. archiveEditor.SelectedAssetIDs]);
            ((Asset)currentAsset).InvokePropertyChanged(true);
        });
    }

    private void SetupForFog()
    {
        AddRow();
        AddTool("Preview Fog", (_, _) =>
        {
            Program.Renderer.Fog = GetCurrentAsset<AssetFOG>();
        }, true);
    }

    private void SetupForScrp()
    {
        AddRow();
        AddTool("Convert to group of timers", (_, _) =>
        {
            var currentAsset = GetCurrentAsset<AssetSCRP>();
            var newAssets = AssetFactory.ConvertScriptToGroupOfTimers(archiveEditor.archive.Platform, currentAsset, archiveEditor.archive.ContainsAsset);

            archiveEditor.BeginTransaction();
            var layer = archiveEditor.archive.GetOrCreateLayerOfType(LayerType.DEFAULT);
            archiveEditor.archive.AddMultipleAssets(newAssets, layer, false);
            archiveEditor.EndTransaction();

            Program.ArchiveManager.SetSelectedAssets(newAssets);
        }, true);
    }

    private void SetupForCsn()
    {
        AddRow();
        AddTool("Export All", (_, _) =>
        {
            using var openFolder = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (openFolder.ShowDialog() == CommonFileDialogResult.Ok)
                GetCurrentAsset<AssetCSN>().ExtractToFolder(openFolder.FileName, archiveEditor.archive.Platform.Endianness());
        }, true);
    }

    private void SetupForWire()
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
                var currentAsset = GetCurrentAsset<AssetWIRE>();
                currentAsset.FromObj(openFile.FileName);
                currentAsset.InvokePropertyChanged(true);
            }
        });

        AddTool("Export", (_, _) =>
        {
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                Filter = "OBJ Files|*.obj|All files|*.*",
                FileName = GetCurrentAsset<AssetWIRE>().assetName
            };

            if (saveFile.ShowDialog() == DialogResult.OK)
                GetCurrentAsset<AssetWIRE>().ToObj(saveFile.FileName);
        });
    }

    private void SetupForCopyPasteTransformation()
    {
        AddRow();

        AddTool("Copy Transformation", (_, _) =>
        {
            if (((IAssetCopyPasteTransformation)this.asset).GetTransformation() is Transformation transformation)
                Clipboard.SetText(JsonConvert.SerializeObject(transformation));
        });

        AddTool("Paste Transformation", (_, _) =>
        {
            Transformation transformation;
            try
            {
                transformation = JsonConvert.DeserializeObject<Transformation>(Clipboard.GetText()) ?? throw new Exception("no transformation found");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error pasting the transformation from clipboard: ${ex.Message}. Are you sure you have a transformation copied?");
                return;
            }
            var currentAsset = (IAssetCopyPasteTransformation)this.asset;
            currentAsset.SetTransformation(transformation);
            ((Asset)currentAsset).InvokePropertyChanged(true);
        });
    }

    private void SetupForBakeScaleRot()
    {
        AddRow();

        var toolTip = new ToolTip();
        (var button, var menuItem) = AddTool("Bake Rotation", (_, _) =>
        {
            var result = MessageBox.Show(Resources.BakeRotationDescription, "Bake Rotation", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                var currentAsset = GetCurrentAsset<EntityAsset>();
                currentAsset.ApplyBakeRotation();
                currentAsset.InvokePropertyChanged(true);
            }
        });
        button?.MouseHover += (_, _) => toolTip.Show(Resources.BakeRotationDescription, button);
        menuItem?.MouseHover += (_, _) => toolTip.Show(Resources.BakeRotationDescription, button);

        (button, menuItem) = AddTool("Bake Scale", (_, _) =>
        {
            var result = MessageBox.Show(Resources.BakeScaleDescription, "Bake Scale", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                var currentAsset = GetCurrentAsset<EntityAsset>();
                currentAsset.ApplyBakeScale();
                currentAsset.InvokePropertyChanged(true);
            }
        });
        button?.MouseHover += (_, _) => toolTip.Show(Resources.BakeScaleDescription, button);
        menuItem?.MouseHover += (_, _) => toolTip.Show(Resources.BakeScaleDescription, button);
    }

    private T GetCurrentAsset<T>() where T : Asset => (T)asset;
}

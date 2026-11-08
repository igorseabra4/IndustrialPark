using IndustrialPark.Archive;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static HipHopFile.Functions;

namespace IndustrialPark;

public abstract class InternalEditor : Form
{
    public static bool EnableAssetToolButtons = false;

    protected readonly ArchiveEditor archiveEditor;
    protected Asset asset;

    public bool IsDocked = false;
    public bool CanBeDocked => CanDockAssetEditor(asset);
    public static bool CanDockAssetEditor(Asset asset) => asset is not (AssetSound or AssetRenderWareModel);

    public InternalEditor(Asset asset, ArchiveEditor archiveEditor)
    {
        this.asset = asset;
        this.archiveEditor = archiveEditor;

        asset.PropertyChanged += RefreshPropertyGridDelegate;
    }

    public Asset GetAsset() => asset;
    public uint GetAssetID() => asset.assetID;
    public bool IsForArchive(Archive.Archive archive) => ReferenceEquals(archiveEditor.archive, archive);

    public virtual void SetAsset(Asset newAsset)
    {
        if (asset.assetID != newAsset.assetID || asset.assetType != newAsset.assetType)
            throw new ArgumentException("Cannot change the asset identity or type of an InternalEditor.");

        asset.PropertyChanged -= RefreshPropertyGridDelegate;
        asset = newAsset;
        asset.PropertyChanged += RefreshPropertyGridDelegate;
        Text = newAsset.assetName;
        OnAssetReplaced();
        if (IsHandleCreated)
            RefreshPropertyGrid();
    }

    protected virtual void OnAssetReplaced()
    {
    }

    public virtual void RefreshPropertyGrid()
    {
    }

    protected void RefreshPropertyGridDelegate(bool propertyChanged) => RefreshPropertyGrid();

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        asset.PropertyChanged -= RefreshPropertyGridDelegate;
        base.OnFormClosing(e);
    }

    public event EventHandler FormDropped;
    private const int WM_MOVING = 0x0216;
    private const int WM_EXITSIZEMOVE = 0x0232;
    private bool isInDockingZone;
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        if (IsDocked || this.Owner is not MainForm mainForm)
            return;

        if (m.Msg == WM_MOVING)
        {
            isInDockingZone = mainForm.IsPointerInDockingZone();
        }
        else if (m.Msg == WM_EXITSIZEMOVE)
        {
            if (isInDockingZone)
            {
                isInDockingZone = false;
                FormDropped?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public static string[] GetPropertyPath(GridItem item)
    {
        var path = new List<string>();
        while (item != null)
        {
            if (item.GridItemType == GridItemType.Property)
                path.Add(item.PropertyDescriptor.Name);
            item = item.Parent;
        }
        path.Reverse();
        return path.ToArray();
    }

    public static void SelectFirstProperty(GridItem gridItem)
    {
        var root = gridItem;
        while (root.Parent != null)
            root = root.Parent;
        FindFirstPropertyRecursive(root)?.Select();
    }

    public static GridItem FindFirstPropertyRecursive(GridItem item)
    {
        foreach (GridItem child in item.GridItems)
        {
            if (child.GridItemType == GridItemType.Property)
                return child;
            if (child.GridItemType == GridItemType.Category && child.GridItems.Count > 0)
            {
                var nested = FindFirstPropertyRecursive(child);
                if (nested != null)
                    return nested;
            }
        }
        return null;
    }

    public void OpenWikiPage()
    {
        // todo: add more options of wiki pages
        var code = asset.assetType.GetCode();
        if (asset.assetType.IsDyna())
            code += $"/{asset.TypeString}";
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = AboutBox.WikiLink + code, UseShellExecute = true });
    }

    protected void buttonFindCallers_Click(object sender, EventArgs e)
    {
        Program.EditorManager.OpenWhoTargetsAssetEditors(GetAssetID());
    }

    protected void buttonHelp_Click(object sender, EventArgs e)
    {
        OpenWikiPage();
    }

    protected void editHeaderToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (archiveEditor.EditHeader(asset) is Asset newAsset)
            Program.EditorManager.OpenAssetEditor(newAsset, MousePosition);
    }

    protected void viewToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (asset is IClickableAsset ica && asset is not AssetSFX { PlayIn3D: false })
            Program.Renderer.Camera.ShowAssetOnViewport(ica);
    }

    protected void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var clipboard = AssetService.AssetsToClipboard([asset], archiveEditor.archive.Platform, archiveEditor.archive.GetSoundData);
        Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
    }

    protected void copyHierarchyToolStripMenuItem_Click(object sender, EventArgs e)
    {
        List<Asset> assets = [asset];
        assets = [.. AssetService.GetAssetsAndHierarchy(assets)];
        var clipboard = AssetService.AssetsToClipboard(assets, archiveEditor.archive.Platform, archiveEditor.archive.GetSoundData);
        Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
    }

    protected void removeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show($"Are you sure you want to delete asset [{asset.assetID:X8}] {asset.assetName} ({AssetTypeContainer.AssetTypeToString(asset.assetType)})?", "Confirm asset removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (result == DialogResult.Yes)
        {
            archiveEditor.BeginTransaction();
            archiveEditor.RemoveAssets([asset.assetID]);
            archiveEditor.EndTransaction();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark;

public abstract class InternalEditor : Form
{
    protected readonly ArchiveEditorFunctions archive;
    protected readonly Asset asset;
    protected readonly Action<Asset> updateListView;

    public bool IsDocked = false;
    public bool CanBeDocked => CanDockAssetEditor(asset);
    public static bool CanDockAssetEditor(Asset asset) => asset is not (AssetSound or AssetRenderWareModel);

    public InternalEditor(Asset asset, ArchiveEditorFunctions archive, Action<Asset> updateListView)
    {
        this.asset = asset;
        this.archive = archive;
        this.updateListView = updateListView;
    }

    public uint GetAssetID() => asset.assetID;

    public abstract void RefreshPropertyGrid();

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

    public static bool EnableAssetToolButtons = false;

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

    public static AssetPropertyChangedAction GetPropertyChangedAction(ArchiveEditorFunctions archive, Asset asset, object oldValue, object newValue, GridItem changedItem)
    {
        var propertyPath = GetPropertyPath(changedItem);

        if (changedItem.Parent != null && changedItem.Parent.Value is FlagBitmask flags && !changedItem.PropertyDescriptor.Name.Equals("FlagsValue"))
        {
            var flagBit = int.Parse(changedItem.PropertyDescriptor.Name.Split('_')[1]);
            var flagsValue = flags.FlagValueInt;

            oldValue = flagsValue ^ (1 << flagBit);
            newValue = flagsValue;
            propertyPath = [.. propertyPath.SkipLast(1), "FlagsValue"];
        }

        var action = new AssetPropertyChangedAction(archive, asset, propertyPath, oldValue, newValue);
        return action;
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
}
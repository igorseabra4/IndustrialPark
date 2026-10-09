using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark;

public interface IInternalEditor
{
    bool TopMost { get; set; }
    uint GetAssetID();
    void Close();
    void Show();
    FormStartPosition StartPosition { get; set; }
    System.Drawing.Point Location { get; set; }

    void RefreshPropertyGrid();

    double Opacity { get; set; }
    Form[] OwnedForms { get; }
    System.Drawing.Size Size { get; set; }

    void BringToFront();

    event EventHandler Activated;
    event EventHandler Deactivate;

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
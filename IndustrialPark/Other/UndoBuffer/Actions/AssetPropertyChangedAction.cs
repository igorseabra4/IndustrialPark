using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark;

public class AssetPropertyChangedAction : IReversibleAction
{
    private ArchiveEditorFunctions archive;
    private uint assetID;
    private string[] propertyPath;
    private object oldValue;
    private object newValue;
    private List<uint> selection;

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, uint assetID, string[] propertyPath, object oldValue, object newValue, List<uint> selection = null)
    {
        this.archive = archive;
        this.assetID = assetID;
        this.propertyPath = propertyPath;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.selection = selection;
    }

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, Asset asset, string propertyName, object oldValue, object newValue, List<uint> selection = null) :
        this(archive, asset.assetID, [propertyName], oldValue, newValue, selection)
    { }

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, Asset asset, string[] propertyName, object oldValue, object newValue, List<uint> selection = null) :
        this(archive, asset.assetID, propertyName, oldValue, newValue, selection)
    { }

    private Asset asset => archive.GetFromAssetID(assetID);

    private void SetValue(object value)
    {
        object current = asset;
        for (int i = 0; i < propertyPath.Length - 1; i++)
        {
            var path = propertyPath[i];
            current = path.StartsWith("[")
                ? ((IList)current)[int.Parse(path[1..^1])]
                : TypeDescriptor.GetProperties(current)[path].GetValue(current);
        }
        var property = TypeDescriptor.GetProperties(current)[propertyPath[^1]];
        property.SetValue(current, IReversibleAction.ConvertValue(value, property.PropertyType));

        // TODO: fix this for flags fields

        if (this.asset is Asset a)
            archive.RefreshAssetEditor(a.assetID);
        if (selection != null)
            Program.MainForm.SetSelectedIndices(selection);
    }

    public void Undo()
    {
        SetValue(oldValue);
    }

    public void Redo()
    {
        SetValue(newValue);
    }

    public bool ContainsArchive(ArchiveEditorFunctions archive) => this.archive == archive;
}

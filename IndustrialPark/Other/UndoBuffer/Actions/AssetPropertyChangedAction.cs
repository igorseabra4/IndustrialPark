using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark;

public class AssetPropertyChangedAction(
    ArchiveEditorFunctions archive,
    uint assetID,
    string[] propertyPath,
    object oldValue,
    object newValue,
    List<uint> selection = null)
    : IReversibleAction
{
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
        property.SetValue(current, ConvertValue(value, property.PropertyType));

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

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        if (targetType.IsInstanceOfType(value))
            return value;

        Type sourceType = value.GetType();

        // Look for implicit/explicit conversion on the target type.
        MethodInfo? conversion = targetType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                (m.Name == "op_Implicit" || m.Name == "op_Explicit") &&
                m.ReturnType == targetType &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == sourceType);

        if (conversion != null)
            return conversion.Invoke(null, [value]);

        // Normal conversions for types such as int, float, string, etc.
        return Convert.ChangeType(value, targetType);
    }
}

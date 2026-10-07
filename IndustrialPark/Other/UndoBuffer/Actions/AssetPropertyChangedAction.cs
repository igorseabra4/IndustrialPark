using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark;

public class AssetPropertyChangedAction(
    ArchiveEditorFunctions archive,
    uint assetID,
    PropertyDescriptor property,
    object oldValue,
    object newValue,
    List<uint> selection = null)
    : IReversibleAction
{
    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, Asset asset, PropertyDescriptor property, object oldValue, object newValue, List<uint> selection = null) :
        this(archive, asset.assetID, property, oldValue, newValue, selection)
    { }

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, Asset asset, string propertyName, object oldValue, object newValue, List<uint> selection = null) :
        this(archive, asset.assetID, TypeDescriptor.GetProperties(asset)[propertyName], oldValue, newValue, selection)
    { }

    private Asset asset => archive.GetFromAssetID(assetID);

    public void Undo()
    {
        property.SetValue(this.asset, ConvertValue(oldValue, property.PropertyType));
        if (this.asset is Asset a)
            archive.RefreshAssetEditor(a.assetID);
        if (selection != null)
            Program.MainForm.SetSelectedIndices(selection);
    }

    public void Redo()
    {
        property.SetValue(this.asset, ConvertValue(newValue, property.PropertyType));
        if (this.asset is Asset a)
            archive.RefreshAssetEditor(a.assetID);
        if (selection != null)
            Program.MainForm.SetSelectedIndices(selection);
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

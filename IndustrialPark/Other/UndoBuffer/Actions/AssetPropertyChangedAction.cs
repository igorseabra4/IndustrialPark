using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IndustrialPark;

public class AssetPropertyChangedAction : IReversibleAction
{
    private ArchiveEditorFunctions archive;
    private GenericAssetDataContainer asset;
    private PropertyDescriptor property;
    private object oldValue;
    private object newValue;

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, GenericAssetDataContainer asset, PropertyDescriptor property, object oldValue, object newValue)
    {
        this.archive = archive;
        this.asset = asset;
        this.property = property;
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public AssetPropertyChangedAction(ArchiveEditorFunctions archive, GenericAssetDataContainer asset, string propertyName, object oldValue, object newValue)
    {
        this.archive = archive;
        this.asset = asset;
        this.property = TypeDescriptor.GetProperties(asset)[propertyName];
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public void Undo()
    {
        this.property.SetValue(this.asset, ConvertValue(this.oldValue, this.property.PropertyType));
        if (this.asset is Asset a)
            this.archive.RefreshAssetEditor(a.assetID);
    }

    public void Redo()
    {
        this.property.SetValue(this.asset, ConvertValue(this.newValue, this.property.PropertyType));
        if (this.asset is Asset a)
            this.archive.RefreshAssetEditor(a.assetID);
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

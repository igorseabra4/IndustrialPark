using System;
using System.Linq;
using System.Reflection;

namespace IndustrialPark;

public interface IReversibleAction
{
    bool ContainsArchive(ArchiveEditorFunctions archive);

    void Undo();
    void Redo();

    protected static object ConvertValue(object value, Type targetType)
    {
        if (value == null)
            return null;

        if (targetType.IsInstanceOfType(value))
            return value;

        Type sourceType = value.GetType();

        MethodInfo conversion = targetType.GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(m =>
                (m.Name == "op_Implicit" || m.Name == "op_Explicit") &&
                m.ReturnType == targetType &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == sourceType);

        if (conversion != null)
            return conversion.Invoke(null, [value]);

        return Convert.ChangeType(value, targetType);
    }
}

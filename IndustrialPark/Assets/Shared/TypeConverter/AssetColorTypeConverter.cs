using System;
using System.ComponentModel;
using System.Globalization;

namespace IndustrialPark.AssetEditorColors;

public class AssetColorTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string text)
            return new AssetColor(text);
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
    {
        if ((destType == typeof(string)) && (value is AssetColor color))
            return color.ToString();

        return base.ConvertTo(context, culture, value, destType);
    }
}
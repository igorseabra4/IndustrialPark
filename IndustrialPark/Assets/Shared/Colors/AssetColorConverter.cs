using System;
using System.ComponentModel;
using System.Globalization;

namespace AssetEditorColors
{
    public class AssetColorConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
                return new AssetColor((string)value);
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if ((destType == typeof(string)) && (value is AssetColor color))
            {
                return color.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace IndustrialPark
{
    public class AssetIDTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return new AssetID(Convert.ToUInt32(s, 16));
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AssetID);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return ((AssetID)value).ToString("X8");

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
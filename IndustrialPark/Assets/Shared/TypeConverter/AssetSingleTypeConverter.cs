using System;
using System.ComponentModel;
using System.Globalization;

namespace IndustrialPark
{
    public class AssetSingleTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return new AssetSingle(0);
                if (float.TryParse(s, out float result))
                    return new AssetSingle(result);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AssetSingle);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return string.Format("{0:0.000000}", (float)(AssetSingle)value);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
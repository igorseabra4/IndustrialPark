using System;
using System.ComponentModel;
using System.Globalization;

namespace IndustrialPark
{
    public class HexByteTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
                return new AssetByte(Convert.ToByte(s, 16));

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(byte);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value is uint valueUint)
                    return valueUint.ToString("X2");
                if (value is ushort valueUshort)
                    return valueUshort.ToString("X2");
                if (value is byte valuebyte)
                    return valuebyte.ToString("X2");
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
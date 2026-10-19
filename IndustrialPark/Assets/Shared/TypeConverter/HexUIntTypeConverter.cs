using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace IndustrialPark
{
    public class HexUIntTypeConverter : TypeConverter
    {
        public static bool Legacy;

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                if (Legacy)
                    return new AssetID(Convert.ToUInt32(s, 16));

                return AssetIDFromString(s);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public static AssetID AssetIDFromString(string s)
        {
            if (s.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase))
                return Convert.ToUInt32(s, 16);

            if (s.StartsWith("{") && s.EndsWith("}"))
            {
                try
                {
                    var assets = JsonConvert.DeserializeObject<List<AssetClipboard>>(s);
                    return assets[0].AHDR.assetID;
                }
                catch { }
            }

            return new AssetID(s);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AssetID);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value != null)
                return StringFromAssetID((AssetID)value);

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public static string StringFromAssetID(AssetID value)
        {
            if (Legacy)
                return value.ToString("X8");

            return Program.MainForm.GetAssetNameFromID(value);
        }
    }
}
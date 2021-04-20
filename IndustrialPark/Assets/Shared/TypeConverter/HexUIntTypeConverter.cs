using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;

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
            if (s.ToLower().StartsWith("0x"))
                return Convert.ToUInt32(s, 16);

            if (s.StartsWith("{") && s.EndsWith("}"))
            {
                try
                {
                    var assets = JsonConvert.DeserializeObject<AssetClipboard>(s);
                    return assets.assets[0].assetID;
                }
                catch { }
            }

            return s;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(AssetID);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (Legacy)
                    return ((AssetID)value).ToString("X8");

                return Program.MainForm.GetAssetNameFromID((AssetID)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
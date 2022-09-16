using HipHopFile;
using System.Text.RegularExpressions;

namespace IndustrialPark
{
    public class AssetTypeContainer
    {
        private static readonly Regex r = new Regex(
            @"(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])",
            RegexOptions.IgnorePatternWhitespace);

        public AssetType assetType;

        public AssetTypeContainer(AssetType assetType)
        {
            this.assetType = assetType;
        }

        public override string ToString() =>
            AssetTypeToString(assetType);

        public static bool LegacyAssetNameFormat = false;

        public static string AssetTypeToString(AssetType assetType) =>
            (assetType == AssetType.Null) ? "All" :
            (!LegacyAssetNameFormat) ? AssetTypeToString(assetType.ToString()) :
            assetType.IsDyna() ? $"DYNA [{AssetTypeToString(assetType.ToString())}]" :
            Functions.GetCode(assetType);

        public static string AssetTypeToString(string assetType) => r.Replace(assetType, " ");
    }
}
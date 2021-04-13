using HipHopFile;
using System.Collections.Generic;

namespace IndustrialPark
{
    public class GenericAssetDataContainer
    {
        public virtual byte[] Serialize(Game game, Platform platform) => new byte[0];

        public virtual bool HasReference(uint assetID) => false;

        public virtual void Verify(ref List<string> result) { }

        public static void Verify(uint assetID, ref List<string> result)
        {
            if (assetID != 0 && !Program.MainForm.AssetExists(assetID))
                result.Add("Referenced asset 0x" + assetID.ToString("X8") + " was not found in any open archive.");
        }

        protected static FlagBitmask ByteFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(8, flagNames);

        protected static FlagBitmask ShortFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(16, flagNames);

        protected static FlagBitmask IntFlagsDescriptor(params string[] flagNames) => FlagsDescriptor(32, flagNames);

        private static FlagBitmask FlagsDescriptor(int bitSize, params string[] flagNames)
        {
            var dt = new DynamicTypeDescriptor(typeof(FlagsField));
            var ff = new FlagsField(bitSize, flagNames, dt);
            return dt.DFD_FromComponent(ff);
        }
    }
}

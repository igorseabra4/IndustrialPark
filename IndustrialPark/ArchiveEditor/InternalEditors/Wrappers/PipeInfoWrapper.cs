using System.ComponentModel;

namespace IndustrialPark
{
    public class PipeInfoWrapper
    {
        public PipeInfo Entry;

        public PipeInfoWrapper(PipeInfo entry)
        {
            Entry = entry;
        }

        private const string categoryPipeInfo = "Pipe Info";

        [Category(categoryPipeInfo)]
        public PiptPreset PipeFlags_Preset
        {
            get => Entry.PipeFlags_Preset;
            set => Entry.PipeFlags_Preset = value;
        }

        [Category(categoryPipeInfo)]
        public FlagBitmask SubObjectBits
        {
            get => Entry.SubObjectBits;
            set => Entry.SubObjectBits = value;
        }

        [Category(categoryPipeInfo)]
        public int PipeFlags
        {
            get => Entry.PipeFlags;
            set => Entry.PipeFlags = value;
        }

        private const string categoryFlags = "Flags";

        [Category(categoryFlags), DisplayName("AlphaCompareValue (0 - 255)")]
        public byte AlphaCompareValue
        {
            get => Entry.AlphaCompareValue;
            set => Entry.AlphaCompareValue = value;
        }

        [Category(categoryFlags), DisplayName("UnknownFlagB (0 - 15)")]
        public byte UnknownFlagB
        {
            get => Entry.UnknownFlagB;
            set => Entry.UnknownFlagB = value;
        }

        [Category(categoryFlags), DisplayName("UnknownFlagC (0 - 7)")]
        public byte UnknownFlagC
        {
            get => Entry.UnknownFlagC;
            set => Entry.UnknownFlagC = value;
        }

        [Category(categoryFlags)]
        public bool IgnoreFog
        {
            get => Entry.IgnoreFog;
            set => Entry.IgnoreFog = value;
        }

        [Category(categoryFlags)]
        public BlendFactorType DestinationBlend
        {
            get => Entry.DestinationBlend;
            set => Entry.DestinationBlend = value;
        }

        [Category(categoryFlags)]
        public BlendFactorType SourceBlend
        {
            get => Entry.SourceBlend;
            set => Entry.SourceBlend = value;
        }

        [Category(categoryFlags)]
        public LightingMode LightingMode
        {
            get => Entry.LightingMode;
            set => Entry.LightingMode = value;
        }

        [Category(categoryFlags)]
        public PiptCullMode CullMode
        {
            get => Entry.CullMode;
            set => Entry.CullMode = value;
        }

        [Category(categoryFlags)]
        public ZWriteMode ZWriteMode
        {
            get => Entry.ZWriteMode;
            set => Entry.ZWriteMode = value;
        }

        [Category(categoryFlags), DisplayName("UnknownFlagJ (0 - 3)")]
        public byte UnknownFlagJ
        {
            get => Entry.UnknownFlagJ;
            set => Entry.UnknownFlagJ = value;
        }

        [Category(categoryFlags), DisplayName("Unknown (Movie/Incredibles only)")]
        public AssetID Unknown
        {
            get => Entry.Unknown;
            set => Entry.Unknown = value;
        }
    }
}

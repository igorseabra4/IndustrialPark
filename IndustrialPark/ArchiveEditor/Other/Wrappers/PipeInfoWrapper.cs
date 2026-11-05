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
        public uint PipeFlags
        {
            get => Entry.PipeFlags;
            set => Entry.PipeFlags = value;
        }

        private const string categoryFlags = "Flags";

        [Category(categoryFlags + " (Movie/Incredibles only)")]
        public bool HDR_Brightening
        {
            get => Entry.HDR_Brightening;
            set => Entry.HDR_Brightening = value;
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

        [Category(categoryPipeInfo)]
        public PIPTLayerType Layer
        {
            get => Entry.Layer;
            set => Entry.Layer = value;
        }

        [Category(categoryPipeInfo)]
        public byte AlphaDiscard
        {
            get => Entry.AlphaDiscard;
            set => Entry.AlphaDiscard = value;
        }

        [Category(categoryPipeInfo)]
        public uint UnknownFlags
        {
            get => Entry.UnknownFlags;
            set => Entry.UnknownFlags = value;
        }
    }
}

using HipHopFile;

namespace IndustrialPark
{
    public class AssetCOND : ObjectAsset
    {
        public AssetCOND(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset
        {
            get => 0x18;
        }

        public int EvaluationAmount
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        public AssetID VariableCategory
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        public int EvaluationMethod
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        public AssetID Variable
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
    }
}
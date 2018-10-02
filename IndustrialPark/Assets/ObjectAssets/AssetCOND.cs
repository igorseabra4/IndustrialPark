using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum CONDOperation
    {
        EQUAL_TO = 0,
        GREATER_THAN = 1,
        LESS_THAN = 2,
        GREATER_THAN_OR_EQUAL_TO = 3,
        LESS_THAN_OR_EQUAL_TO = 4,
        NOT_EQUAL_TO = 5
    }

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

        [TypeConverter(typeof(HexIntTypeConverter))]
        public int Variable
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        public CONDOperation Operation
        {
            get => (CONDOperation)ReadInt(0x10);
            set => Write(0x10, (int)value);
        }

        public AssetID SubVariable
        {
            get => ReadUInt(0x14);
            set => Write(0x14, value);
        }
    }
}
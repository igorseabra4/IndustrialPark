using System.ComponentModel;

namespace IndustrialPark
{
    public class FlagsField_UInt : FlagsField
    {
        public FlagsField_UInt(EndianConvertibleWithData asset, int flagsLoc, DynamicTypeDescriptor dt, string[] flagNames)
            : base(asset, flagsLoc, dt)
        {
            for (uint i = 0; i < 32; i++)
                AddPropertyAt(i, flagNames);
        }

        public override string ToString()
        {
            return Flags.ToString("X8");
        }

        [TypeConverter(typeof(HexUIntTypeConverter))]
        public override uint Flags
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
    }
}
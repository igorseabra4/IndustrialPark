using System.ComponentModel;

namespace IndustrialPark
{
    public class FlagsField_UShort : FlagsField
    {
        public FlagsField_UShort(Asset asset, int flagsLoc, DynamicTypeDescriptor dt, string[] flagNames)
            : base(asset, flagsLoc, dt)
        {
            for (uint i = 0; i < 16; i++)
                AddPropertyAt(i, flagNames);
        }

        public override string ToString()
        {
            return Flags.ToString("X4");
        }

        [TypeConverter(typeof(HexUShortTypeConverter))]
        public override uint Flags
        {
            get => ReadUShort(0x00);
            set => Write(0x00, (ushort)value);
        }
    }
}
using System.ComponentModel;

namespace IndustrialPark
{
    public class FlagsField_Byte_Dyna : FlagsField
    {
        public FlagsField_Byte_Dyna(DynaBase asset, int flagsLoc, DynamicTypeDescriptor dt, string[] flagNames)
            : base(asset, flagsLoc + 0x10, dt)
        {
            for (uint i = 0; i < 8; i++)
                AddPropertyAt(i, flagNames);
        }

        public override string ToString()
        {
            return Flags.ToString("X2");
        }

        [TypeConverter(typeof(HexByteTypeConverter))]
        public override uint Flags
        {
            get => ReadByte(0x00);
            set => Write(0x00, (byte)value);
        }

        protected override void Write(int j, byte value)
        {
            ((DynaBase)asset).Data[j + specificStart] = value;
        }
    }
}
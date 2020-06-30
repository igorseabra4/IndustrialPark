using System;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class FlagsField_UInt_Dyna : FlagsField
    {
        public FlagsField_UInt_Dyna(EndianConvertibleWithData asset, int flagsLoc, DynamicTypeDescriptor dt, string[] flagNames)
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

        protected override void Write(int j, uint value)
        {
            byte[] split = BitConverter.GetBytes(value);

            if (endianness == Endianness.Big)
                split = split.Reverse().ToArray();

            for (int i = 0; i < 4; i++)
                ((DynaBase)asset).asset.Data[j + specificStart + i + 0x10] = split[i];
        }
    }
}
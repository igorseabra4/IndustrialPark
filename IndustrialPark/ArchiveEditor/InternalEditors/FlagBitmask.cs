using IndustrialPark;
using System;
using System.ComponentModel;

public class FlagBitmask : DynamicTypeDescriptor
{
    public FlagBitmask() : base() { }

    public FlagBitmask(Type type) : base(type) { }

    private FlagsField _flags_Field;
    [Browsable(false)]
    public uint FlagValueInt
    {
        get => _flags_Field.Flags;
        set => _flags_Field.Flags = value;
    }
    [Browsable(false)]
    public ushort FlagValueShort
    {
        get => (ushort)_flags_Field.Flags;
        set => _flags_Field.Flags = value;
    }
    [Browsable(false)]
    public byte FlagValueByte
    {
        get => (byte)_flags_Field.Flags;
        set => _flags_Field.Flags = value;
    }

    public override FlagBitmask DFD_FromComponent(object component)
    {
        if (component is FlagsField ff)
            _flags_Field = ff;
        else throw new ArgumentException();

        return base.DFD_FromComponent(component);
    }
}
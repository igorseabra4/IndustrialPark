using IndustrialPark;
using System;
using System.ComponentModel;

public class FlagBitmask : DynamicTypeDescriptor
{
    public FlagBitmask(FlagField ff) : base() { _flagField = ff; }

    public FlagBitmask(Type type) : base(type) { }

    private FlagField _flagField;
    [Browsable(false)]
    public uint FlagValueInt
    {
        get => _flagField.Flags;
        set => _flagField.Flags = value;
    }
    [Browsable(false)]
    public ushort FlagValueShort
    {
        get => (ushort)_flagField.Flags;
        set => _flagField.Flags = value;
    }
    [Browsable(false)]
    public byte FlagValueByte
    {
        get => (byte)_flagField.Flags;
        set => _flagField.Flags = value;
    }

    public override FlagBitmask DFD_FromComponent(object component)
    {
        if (component is FlagField ff)
            _flagField = ff;
        else
            throw new ArgumentException();

        return base.DFD_FromComponent(ff);
    }
}
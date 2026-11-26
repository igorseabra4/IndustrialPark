using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class DynaNpcGate : AssetDYNA
{
    private const string dynaCategoryName = "Enemy:NPC Gate";
    public override string TypeString => dynaCategoryName;
    public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(NpcWallsGroup);
    protected override short constVersion => 3;

    [Category(dynaCategoryName)]
    public AssetID NpcWallsGroup { get; set; }
    [Category(dynaCategoryName)]
    public uint GateFacing { get; set; }
    [Category(dynaCategoryName)]
    public AssetID NpcsGroup { get; set; }
    [Category(dynaCategoryName)]
    public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

    public DynaNpcGate(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__NPC_Gate, reader)
    {
        NpcWallsGroup = reader.ReadUInt32();
        GateFacing = reader.ReadUInt32();
        NpcsGroup = reader.ReadUInt32();
        Flags.FlagValueInt = reader.ReadUInt32();
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        writer.Write(NpcWallsGroup);
        writer.Write(GateFacing);
        writer.Write(NpcsGroup);
        writer.Write(Flags.FlagValueInt);
    }
}
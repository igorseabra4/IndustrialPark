using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class DynaNpcWalls : AssetDYNA
{
    private const string dynaCategoryName = "Enemy:NPC Walls";
    public override string TypeString => dynaCategoryName;
    public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(npcWallsGroup);
    protected override short constVersion => 1;

    [Category(dynaCategoryName)]
    public AssetID npcWallsGroup { get; set; }
    [Category(dynaCategoryName)]
    public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();

    public DynaNpcWalls(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__NPC_Walls, reader)
    {
        npcWallsGroup = reader.ReadUInt32();
        Flags.FlagValueInt = reader.ReadUInt32();
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        writer.Write(npcWallsGroup);
        writer.Write(Flags.FlagValueInt);
    }
}
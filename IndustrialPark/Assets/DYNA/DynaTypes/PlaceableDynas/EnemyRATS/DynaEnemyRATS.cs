using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public abstract class DynaEnemyRATS : DynaEnemy
{
    private const string dynaCategoryName = "Enemy:RATS";

    [Category(dynaCategoryName)]
    public AssetID LightkitID { get; set; }
    [Category(dynaCategoryName)]
    public AssetID npcWalls { get; set; }
    [Category(dynaCategoryName)]
    public AssetID npcPerception { get; set; }
    [Category(dynaCategoryName)]
    public AssetSingle UpdateDistance { get; set; }
    [Category(dynaCategoryName)]
    public FlagBitmask Flags { get; set; } = IntFlagsDescriptor("Enabled");

    public DynaEnemyRATS(string assetName, DynaType type, Vector3 position) : base(assetName, type, position)
    {
        UpdateDistance = 250f;
        Flags.FlagValueInt = 1;
    }

    public DynaEnemyRATS(Section_AHDR AHDR, DynaType type, EndianBinaryReader reader) : base(AHDR, type, reader)
    {
        LightkitID = reader.ReadUInt32();
        npcWalls = reader.ReadUInt32();
        npcPerception = reader.ReadUInt32();
        UpdateDistance = reader.ReadSingle();
        Flags.FlagValueInt = reader.ReadUInt32();
    }

    protected void SerializeDynaEnemyRATS(EndianBinaryWriter writer)
    {
        SerializeEntityDyna(writer);
        writer.Write(LightkitID);
        writer.Write(npcWalls);
        writer.Write(npcPerception);
        writer.Write(UpdateDistance);
        writer.Write(Flags.FlagValueInt);
    }
}
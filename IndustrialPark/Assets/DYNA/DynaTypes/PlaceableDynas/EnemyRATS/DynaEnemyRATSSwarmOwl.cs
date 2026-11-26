using HipHopFile;
using SharpDX;

namespace IndustrialPark.Assets;

public class DynaEnemyRATSSwarmOwl : DynaEnemyRATSSwarm
{
    public override string TypeString => "Enemy:RATS:Swarm:Owl";
    protected override short constVersion => 1;

    public DynaEnemyRATSSwarmOwl(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__RATS__Swarm__Owl, position) { }
    public DynaEnemyRATSSwarmOwl(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__RATS__Swarm__Owl, reader) { }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}
using HipHopFile;
using SharpDX;

namespace IndustrialPark.Assets;

public class DynaEnemyRATSSwarmBug : DynaEnemyRATSSwarm
{
    public override string TypeString => "Enemy:RATS:Swarm:Bug";
    protected override short constVersion => 1;

    public DynaEnemyRATSSwarmBug(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__RATS__Swarm__Bug, position) { }
    public DynaEnemyRATSSwarmBug(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__RATS__Swarm__Bug, reader) { }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}
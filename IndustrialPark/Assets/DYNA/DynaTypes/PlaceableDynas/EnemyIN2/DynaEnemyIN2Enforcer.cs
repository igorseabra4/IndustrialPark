using HipHopFile;
using SharpDX;

namespace IndustrialPark.Assets;

public class DynaEnemyIN2Enforcer : DynaEnemyIN2
{
    public override string TypeString => "Enemy:IN2:Enforcer";
    protected override short constVersion => 1;

    public DynaEnemyIN2Enforcer(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__Enforcer, position) { }
    public DynaEnemyIN2Enforcer(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__IN2__Enforcer, reader) { }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        SerializeDynaEnemyIN2(writer);
    }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}

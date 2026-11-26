using HipHopFile;
using SharpDX;

namespace IndustrialPark.Assets;

public class DynaEnemyIN2BossUnderminerUM : DynaEnemyIN2
{
    public override string TypeString => "Enemy:IN2:BossUnderminerUM";
    protected override short constVersion => 1;

    public DynaEnemyIN2BossUnderminerUM(string assetName, Vector3 position) : base(assetName, DynaType.Enemy__IN2__BossUnderminerUM, position) { }
    public DynaEnemyIN2BossUnderminerUM(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__IN2__BossUnderminerUM, reader) { }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        SerializeDynaEnemyIN2(writer);
    }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}

using HipHopFile;

namespace IndustrialPark.Assets;

public class DynaEnemyRATSLeftArm : DynaEnemyRATS
{
    public override string TypeString => "Enemy:RATS:LeftArm";
    protected override short constVersion => 1;

    public DynaEnemyRATSLeftArm(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__RATS__LeftArm, reader) { }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        SerializeDynaEnemyRATS(writer);
    }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}
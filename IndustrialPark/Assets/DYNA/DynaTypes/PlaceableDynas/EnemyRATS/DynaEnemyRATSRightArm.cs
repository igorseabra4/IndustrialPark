using HipHopFile;

namespace IndustrialPark.Assets;

public class DynaEnemyRATSRightArm : DynaEnemyRATS
{
    public override string TypeString => "Enemy:RATS:RightArm";
    protected override short constVersion => 1;

    public DynaEnemyRATSRightArm(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__RATS__RightArm, reader) { }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        SerializeDynaEnemyRATS(writer);
    }

    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}
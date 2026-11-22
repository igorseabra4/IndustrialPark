using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark.Assets;

public class DynaEnemyRATSWaiter : DynaEnemyRATS
{
    private const string dynaCategoryName = "Enemy:RATS:Waiter";
    public override string TypeString => dynaCategoryName;
    protected override short constVersion => 2;

    [Category(dynaCategoryName)]
    public AssetID FirstMovePoint { get; set; }

    public DynaEnemyRATSWaiter(string assetName, Vector3 position, uint mvptAssetID) : base(assetName, DynaType.Enemy__RATS__Waiter, position)
    {
        FirstMovePoint = mvptAssetID;
    }

    public DynaEnemyRATSWaiter(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__RATS__Waiter, reader)
    {
        FirstMovePoint = reader.ReadUInt32();
    }

    protected override void SerializeDyna(EndianBinaryWriter writer)
    {
        SerializeDynaEnemyRATS(writer);
        writer.Write(FirstMovePoint);
    }


    public static bool dontRender = false;
    public override bool DontRender => dontRender;
}
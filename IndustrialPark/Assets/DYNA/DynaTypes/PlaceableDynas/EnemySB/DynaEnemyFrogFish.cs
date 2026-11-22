using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public enum EnemyFrogFishType : uint
    {
        frog_bind = 0x2AA1822C,
    }

    public class DynaEnemyFrogFish : DynaEnemy
    {
        private const string dynaCategoryName = "Enemy:SB:FrogFish";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => FrogFishType.ToString();

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemyFrogFishType FrogFishType
        {
            get => (EnemyFrogFishType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Unknown { get; set; }

        public DynaEnemyFrogFish(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__SB__FrogFish, reader)
        {
            Unknown = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(Unknown);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
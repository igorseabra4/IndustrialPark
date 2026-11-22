using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark.Assets
{
    public enum EnemyNeptuneType : uint
    {
        neptune_bind = 0x8F85AF53
    }

    public class DynaEnemyNeptune : DynaEnemy
    {
        private const string dynaCategoryName = "Enemy:SB:Neptune";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => NeptuneType.ToString();

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyNeptuneType NeptuneType
        {
            get => (EnemyNeptuneType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Unknown50 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }

        public DynaEnemyNeptune(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR, DynaType.Enemy__SB__Neptune, reader)
        {
            Unknown50 = reader.ReadUInt32();
            Unknown54 = reader.ReadUInt32();
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(Unknown50);
            writer.Write(Unknown54);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
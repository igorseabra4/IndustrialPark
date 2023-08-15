using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyDennisType : uint
    {
        dennis_junk_bind = 0xCB1BBC20,
        dennis_hoff_bind = 0x3D6C5895
    }

    public class DynaEnemyDennis : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Dennis";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => DennisType.ToString();

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public EnemyDennisType DennisType
        {
            get => (EnemyDennisType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Unknown50 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown58 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown5C { get; set; }

        public DynaEnemyDennis(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Dennis, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Unknown50 = reader.ReadUInt32();
                Unknown54 = reader.ReadUInt32();
                Unknown58 = reader.ReadUInt32();
                Unknown5C = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(Unknown50);
            writer.Write(Unknown54);
            writer.Write(Unknown58);
            writer.Write(Unknown5C);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
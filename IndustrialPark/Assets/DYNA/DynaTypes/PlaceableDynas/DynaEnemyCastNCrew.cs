using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public enum EnemyCastNCrewType : uint
    {
        gram_bind = 0x470806A1,
        sb_bat_bind = 0xFF2ABE7F
    }

    public class DynaEnemyCastNCrew : DynaEnemySB
    {
        protected override int constVersion => 1;

        [Category("Enemy:SB:CastNCrew")]
        public EnemyCastNCrewType CastNCrewType
        {
            get => (EnemyCastNCrewType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        public DynaEnemyCastNCrew(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.Enemy__SB__CastNCrew, game, platform) { }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntityDyna(platform));
            return writer.ToArray();
        }
    }
}
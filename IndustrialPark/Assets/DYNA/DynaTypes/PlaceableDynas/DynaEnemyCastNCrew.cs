using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyCastNCrewType : uint
    {
        gram_bind = 0x470806A1,
        sb_bat_bind = 0xFF2ABE7F
    }

    public class DynaEnemyCastNCrew : DynaEnemySB
    {
        protected override short constVersion => 1;

        [Category("Enemy:SB:CastNCrew")]
        public EnemyCastNCrewType CastNCrewType
        {
            get => (EnemyCastNCrewType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        public DynaEnemyCastNCrew(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__CastNCrew, game, endianness) { }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                return writer.ToArray();
            }
        }
    }
}
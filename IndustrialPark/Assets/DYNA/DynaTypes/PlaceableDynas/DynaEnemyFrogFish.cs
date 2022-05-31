using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyFrogFishType : uint
    {
        frog_bind = 0x2AA1822C,
    }

    public class DynaEnemyFrogFish : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:FrogFish";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemyFrogFishType FrogFishType
        {
            get => (EnemyFrogFishType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Unknown { get; set; }

        public DynaEnemyFrogFish(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__FrogFish, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Unknown = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(Unknown);
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Unknown, ref result);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
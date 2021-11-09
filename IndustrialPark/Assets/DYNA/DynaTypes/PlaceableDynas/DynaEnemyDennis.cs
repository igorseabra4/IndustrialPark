using HipHopFile;
using System.Collections.Generic;
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

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public EnemyDennisType DennisType
        {
            get => (EnemyDennisType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
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

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(Unknown50);
                writer.Write(Unknown54);
                writer.Write(Unknown58);
                writer.Write(Unknown5C);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown50 == assetID)
                return true;
            if (Unknown54 == assetID)
                return true;
            if (Unknown58 == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Unknown50, ref result);
            Verify(Unknown54, ref result);
            Verify(Unknown58, ref result);
            Verify(Unknown5C, ref result);
        }
    }
}
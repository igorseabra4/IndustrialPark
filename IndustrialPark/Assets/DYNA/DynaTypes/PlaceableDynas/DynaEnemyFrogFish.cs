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

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public EnemyFrogFishType FrogFishType
        {
            get => (EnemyFrogFishType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Player_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public byte UnknownByte54 { get; set; }
        [Category(dynaCategoryName)]
        public byte UnknownByte55 { get; set; }
        [Category(dynaCategoryName)]
        public byte UnknownByte56 { get; set; }
        [Category(dynaCategoryName)]
        public byte UnknownByte57 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown58 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown5C { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown60 { get; set; }

        public DynaEnemyFrogFish(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.Enemy__SB__FrogFish, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityDynaEndPosition;

            Player_AssetID = reader.ReadUInt32();
            UnknownByte54 = reader.ReadByte();
            UnknownByte55 = reader.ReadByte();
            UnknownByte56 = reader.ReadByte();
            UnknownByte57 = reader.ReadByte();
            Unknown58 = reader.ReadUInt32();
            Unknown5C = reader.ReadUInt32();
            Unknown60 = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeEntityDyna(platform));

            writer.Write(Player_AssetID);
            writer.Write(UnknownByte54);
            writer.Write(UnknownByte55);
            writer.Write(UnknownByte56);
            writer.Write(UnknownByte57);
            writer.Write(Unknown58);
            writer.Write(Unknown5C);
            writer.Write(Unknown60);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (Player_AssetID == assetID)
                return true;
            if (Unknown58 == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Player_AssetID, ref result);
            Verify(Unknown58, ref result);
            Verify(Unknown5C, ref result);
            Verify(Unknown60, ref result);
        }
    }
}
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyNeptuneType : uint
    {
        neptune_bind = 0x8F85AF53
    }

    public class DynaEnemyNeptune : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Neptune";

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyNeptuneType NeptuneType
        {
            get => (EnemyNeptuneType)(uint)Model_AssetID;
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
        [Category(dynaCategoryName)]
        public AssetID Unknown60 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown64 { get; set; }

        public DynaEnemyNeptune(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Neptune, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = entityDynaEndPosition;

            Unknown50 = reader.ReadUInt32();
            Unknown54 = reader.ReadUInt32();
            Unknown58 = reader.ReadUInt32();
            Unknown5C = reader.ReadUInt32();
            Unknown60 = reader.ReadUInt32();
            Unknown64 = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeEntityDyna(endianness));

            writer.Write(Unknown50);
            writer.Write(Unknown54);
            writer.Write(Unknown58);
            writer.Write(Unknown5C);
            writer.Write(Unknown60);
            writer.Write(Unknown64);

            return writer.ToArray();
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
            if (Unknown60 == assetID)
                return true;
            if (Unknown64 == assetID)
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
            Verify(Unknown60, ref result);
            Verify(Unknown64, ref result);
        }
    }
}
using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyTurretType : uint
    {
        turret_v1_bind = 0xE7A67E0E,
        turret_v2_bind = 0xE32AC981,
        turret_v3_bind = 0xDEAF14F4
    }

    public class DynaEnemyTurret : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:Turret";

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyTurretType TurretType
        {
            get => (EnemyTurretType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat50 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt58 { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown5C { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown60 { get; set; }

        public DynaEnemyTurret(string assetName, AssetTemplate template, Vector3 position) : base(assetName, DynaType.Enemy__SB__Turret, 4, position)
        {
            UnknownFloat50 = 30f;
            UnknownInt58 = 1;
            
            TurretType =
            template == AssetTemplate.Turret_v1 ? EnemyTurretType.turret_v1_bind :
            template == AssetTemplate.Turret_v2 ? EnemyTurretType.turret_v2_bind :
            template == AssetTemplate.Turret_v3 ? EnemyTurretType.turret_v3_bind : 0;
        }

        public DynaEnemyTurret(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Turret, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = entityDynaEndPosition;

            UnknownFloat50 = reader.ReadSingle();
            Unknown54 = reader.ReadUInt32();
            UnknownInt58 = reader.ReadInt32();
            Unknown5C = reader.ReadUInt32();
            Unknown60 = reader.ReadUInt32();
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeEntityDyna(endianness));

            writer.Write(UnknownFloat50);
            writer.Write(Unknown54);
            writer.Write(UnknownInt58);
            writer.Write(Unknown5C);
            writer.Write(Unknown60);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (Unknown54 == assetID)
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

            Verify(Unknown54, ref result);
            Verify(Unknown5C, ref result);
            Verify(Unknown60, ref result);
        }
    }
}
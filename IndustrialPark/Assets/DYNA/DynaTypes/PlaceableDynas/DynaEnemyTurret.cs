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
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => TurretType.ToString();

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyTurretType TurretType
        {
            get => (EnemyTurretType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetSingle Rotation { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown54 { get; set; }
        [Category(dynaCategoryName)]
        public int TargetPlayer { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown5C { get; set; }
        [Category(dynaCategoryName)]
        public AssetID Unknown60 { get; set; }

        public DynaEnemyTurret(string assetName, AssetTemplate template, Vector3 position) : base(assetName, DynaType.Enemy__SB__Turret, 4, position)
        {
            Rotation = 30f;
            TargetPlayer = 1;

            TurretType =
            template == AssetTemplate.Turret_v1 ? EnemyTurretType.turret_v1_bind :
            template == AssetTemplate.Turret_v2 ? EnemyTurretType.turret_v2_bind :
            template == AssetTemplate.Turret_v3 ? EnemyTurretType.turret_v3_bind : 0;
        }

        public DynaEnemyTurret(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__Turret, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Rotation = reader.ReadSingle();
                Unknown54 = reader.ReadUInt32();
                TargetPlayer = reader.ReadInt32();
                Unknown5C = reader.ReadUInt32();
                Unknown60 = reader.ReadUInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(Rotation);
                writer.Write(Unknown54);
                writer.Write(TargetPlayer);
                writer.Write(Unknown5C);
                writer.Write(Unknown60);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(Unknown54, ref result);
            Verify(Unknown5C, ref result);
            Verify(Unknown60, ref result);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
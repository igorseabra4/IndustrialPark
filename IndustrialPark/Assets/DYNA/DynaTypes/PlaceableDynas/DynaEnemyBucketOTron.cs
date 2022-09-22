using HipHopFile;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum EnemyBucketOTronType : uint
    {
        buckotron_bb_bind = 0x224770CB,
        buckotron_de_bind = 0xDEB029B9,
        buckotron_gg_bind = 0xF3D0942A,
        buckotron_tr_bind = 0xEF91BC20,
        buckotron_jk_bind = 0xFFF99581,
        buckotron_pt_bind = 0x13CFE3A2,
    }

    public class DynaEnemyBucketOTron : DynaEnemySB
    {
        private const string dynaCategoryName = "Enemy:SB:BucketOTron";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => BucketOTronType.ToString();

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyBucketOTronType BucketOTronType
        {
            get => (EnemyBucketOTronType)(uint)Model;
            set => Model = (uint)value;
        }
        [Category(dynaCategoryName), ValidReferenceRequired]
        public AssetID Group { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt54 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpawnSpeed { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt5C { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt60 { get; set; }

        public DynaEnemyBucketOTron(string assetName, AssetTemplate template, Vector3 position, uint groupAssetID) : base(assetName, DynaType.Enemy__SB__BucketOTron, position)
        {
            BucketOTronType =
                        template == AssetTemplate.Spawner_BB ? EnemyBucketOTronType.buckotron_bb_bind :
                        template == AssetTemplate.Spawner_DE ? EnemyBucketOTronType.buckotron_de_bind :
                        template == AssetTemplate.Spawner_GG ? EnemyBucketOTronType.buckotron_gg_bind :
                        template == AssetTemplate.Spawner_TR ? EnemyBucketOTronType.buckotron_tr_bind :
                        template == AssetTemplate.Spawner_JK ? EnemyBucketOTronType.buckotron_jk_bind :
                        template == AssetTemplate.Spawner_PT ? EnemyBucketOTronType.buckotron_pt_bind : 0;

            Group = groupAssetID;
        }

        public DynaEnemyBucketOTron(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__BucketOTron, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Group = reader.ReadUInt32();
                UnknownInt54 = reader.ReadInt32();
                SpawnSpeed = reader.ReadSingle();
                UnknownInt5C = reader.ReadInt32();
                UnknownInt60 = reader.ReadInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeEntityDyna(writer);
            writer.Write(Group);
            writer.Write(UnknownInt54);
            writer.Write(SpawnSpeed);
            writer.Write(UnknownInt5C);
            writer.Write(UnknownInt60);
        }

        public static bool dontRender = false;
        public override bool DontRender => dontRender;
    }
}
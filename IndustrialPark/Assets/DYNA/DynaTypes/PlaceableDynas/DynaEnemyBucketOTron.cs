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

        protected override short constVersion => 4;

        [Category(dynaCategoryName)]
        public EnemyBucketOTronType BucketOTronType
        {
            get => (EnemyBucketOTronType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [Category(dynaCategoryName)]
        public AssetID Group_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt54 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle SpawnSpeed { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt5C { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt60 { get; set; }

        public DynaEnemyBucketOTron(string assetName, AssetTemplate template, Vector3 position, uint groupAssetID) : base(assetName, DynaType.Enemy__SB__BucketOTron, 4, position)
        {
            BucketOTronType =
                        template == AssetTemplate.BucketOTron_BB ? EnemyBucketOTronType.buckotron_bb_bind :
                        template == AssetTemplate.BucketOTron_DE ? EnemyBucketOTronType.buckotron_de_bind :
                        template == AssetTemplate.BucketOTron_GG ? EnemyBucketOTronType.buckotron_gg_bind :
                        template == AssetTemplate.BucketOTron_TR ? EnemyBucketOTronType.buckotron_tr_bind :
                        template == AssetTemplate.BucketOTron_JK ? EnemyBucketOTronType.buckotron_jk_bind :
                        template == AssetTemplate.BucketOTron_PT ? EnemyBucketOTronType.buckotron_pt_bind : 0;

            Group_AssetID = groupAssetID;
        }

        public DynaEnemyBucketOTron(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Enemy__SB__BucketOTron, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityDynaEndPosition;

                Group_AssetID = reader.ReadUInt32();
                UnknownInt54 = reader.ReadInt32();
                SpawnSpeed = reader.ReadSingle();
                UnknownInt5C = reader.ReadInt32();
                UnknownInt60 = reader.ReadInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntityDyna(endianness));
                writer.Write(Group_AssetID);
                writer.Write(UnknownInt54);
                writer.Write(SpawnSpeed);
                writer.Write(UnknownInt5C);
                writer.Write(UnknownInt60);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (Group_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (Group_AssetID == 0)
                result.Add("DYNA BucketOTron with GRUP Asset ID set to 0");

            Verify(Group_AssetID, ref result);
        }

        public static bool dontRender = false;
        public override bool DontRender { get => dontRender; }
    }
}
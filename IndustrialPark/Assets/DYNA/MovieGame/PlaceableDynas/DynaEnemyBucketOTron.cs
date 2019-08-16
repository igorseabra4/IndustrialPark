using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class DynaEnemyBucketOTron : DynaPlaceableBase
    {
        public override string Note => "Version is always 4";

        public DynaEnemyBucketOTron(Platform platform) : base(platform)
        {
            GRUP_AssetID = 0;
        }

        public DynaEnemyBucketOTron(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            GRUP_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            UnknownInt54 = Switch(BitConverter.ToInt32(Data, 0x54));
            UnknownFloat58 = Switch(BitConverter.ToSingle(Data, 0x58));
            UnknownInt5C = Switch(BitConverter.ToInt32(Data, 0x5C));
            UnknownInt60 = Switch(BitConverter.ToInt32(Data, 0x60));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (GRUP_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (GRUP_AssetID == 0)
                result.Add("DYNA BucketOTron with GRUP Asset ID set to 0");

            Asset.Verify(GRUP_AssetID, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(GRUP_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt54)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat58)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt5C)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt60)));

            return list.ToArray();
        }

        [Category("Enemy BucketOTron")]
        public EnemyBucketOTronType Type
        {
            get => (EnemyBucketOTronType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy BucketOTron")]
        public AssetID GRUP_AssetID { get; set; }

        [Category("Enemy BucketOTron")]
        public int UnknownInt54 { get; set; }

        [Category("Enemy BucketOTron")]
        public float UnknownFloat58 { get; set; }

        [Category("Enemy BucketOTron")]
        public int UnknownInt5C { get; set; }

        [Category("Enemy BucketOTron")]
        public int UnknownInt60 { get; set; }
    }
}
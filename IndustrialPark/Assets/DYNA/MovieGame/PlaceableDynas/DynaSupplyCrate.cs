using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaSupplyCrate : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaSupplyCrate(Platform platform) : base(platform)
        {
            MVPT_AssetID = 0;
        }

        public DynaSupplyCrate(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            MVPT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Asset.Verify(MVPT_AssetID, ref result);
        }


        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));

            return list.ToArray();
        }

        [Category("Supply Crate")]
        public EnemySupplyCrateType Type
        {
            get => (EnemySupplyCrateType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Supply Crate")]
        public AssetID MVPT_AssetID { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyStandard : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaEnemyStandard() : base()
        {
            MVPT_AssetID = 0;
            MVPT_Group_AssetID = 0;
            Unknown5C = 0;
            Unknown60 = 0;
            Unknown64 = 0;
            Unknown68 = 0;
        }

        public DynaEnemyStandard(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MVPT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            MVPT_Group_AssetID = Switch(BitConverter.ToUInt32(Data, 0x54));
            MaybeFlags = Switch(BitConverter.ToInt32(Data, 0x58));
            Unknown5C = Switch(BitConverter.ToUInt32(Data, 0x5C));
            Unknown60 = Switch(BitConverter.ToUInt32(Data, 0x60));
            Unknown64 = Switch(BitConverter.ToUInt32(Data, 0x64));
            Unknown68 = Switch(BitConverter.ToUInt32(Data, 0x68));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;
            if (MVPT_Group_AssetID == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;
            if (Unknown64 == assetID)
                return true;
            if (Unknown68 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(MVPT_Group_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(MaybeFlags)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown5C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown60)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown64)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown68)));

            return list.ToArray();
        }

        [Category("Enemy Standard")]
        public EnemyStandardType Type
        {
            get => (EnemyStandardType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy Standard")]
        public AssetID MVPT_AssetID { get; set; }

        [Category("Enemy Standard")]
        public AssetID MVPT_Group_AssetID { get; set; }

        [Category("Enemy Standard")]
        public int MaybeFlags { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown5C { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown60 { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown64 { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown68 { get; set; }
    }
}
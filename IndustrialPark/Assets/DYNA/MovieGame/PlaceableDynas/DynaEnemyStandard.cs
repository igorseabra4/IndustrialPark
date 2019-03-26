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
            Unknown54 = 0;
            Unknown58 = 0;
            Unknown5C = 0;
            Unknown60 = 0;
            Unknown64 = 0;
            Unknown68 = 0;
        }

        public DynaEnemyStandard(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MVPT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            Unknown54 = Switch(BitConverter.ToUInt32(Data, 0x54));
            Unknown58 = Switch(BitConverter.ToUInt32(Data, 0x58));
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
            if (Unknown68 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown54)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown58)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown5C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown60)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown64)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown68)));

            return list.ToArray();
        }

        [Category("Enemy Standard")]
        public AssetID MVPT_AssetID { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown54 { get; set; }

        [Category("Enemy Standard")]
        public AssetID Unknown58 { get; set; }

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
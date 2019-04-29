using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyCritter : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaEnemyCritter() : base()
        {
            MVPT_AssetID = 0;
            Unknown54 = 0;
        }

        public DynaEnemyCritter(IEnumerable<byte> enumerable) : base (enumerable)
        {
            MVPT_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            Unknown54 = Switch(BitConverter.ToUInt32(Data, 0x54));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;
            if (Unknown54 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (MVPT_AssetID == 0)
                result.Add("DYNA Critter with MVPT Asset ID set to 0");

            Asset.Verify(MVPT_AssetID, ref result);
            Asset.Verify(Unknown54, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown54)));

            return list.ToArray();
        }

        [Category("Enemy Critter")]
        public EnemyCritterType Type
        {
            get => (EnemyCritterType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy Critter")]
        public AssetID MVPT_AssetID { get; set; }

        [Category("Enemy Critter")]
        public AssetID Unknown54 { get; set; }
    }
}
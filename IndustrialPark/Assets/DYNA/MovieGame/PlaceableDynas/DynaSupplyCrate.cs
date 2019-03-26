using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaSupplyCrate : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaSupplyCrate() : base()
        {
        }

        public DynaSupplyCrate(IEnumerable<byte> enumerable) : base (enumerable)
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

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(MVPT_AssetID)));

            return list.ToArray();
        }
        
        [Category("Supply Crate")]
        public AssetID MVPT_AssetID { get; set; }
    }
}
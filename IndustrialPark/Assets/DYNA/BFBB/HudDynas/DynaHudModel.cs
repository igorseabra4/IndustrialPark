using System;
using System.Collections.Generic;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaHudModel : DynaHudBase
    {
        public override string Note => "Version is always 1";

        public DynaHudModel(Platform platform) : base(platform)
        {
            Model_AssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (Model_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(Model_AssetID, ref result);
        }

        public DynaHudModel(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            Model_AssetID = Switch(BitConverter.ToUInt32(Data, 0x18));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();
            list.AddRange(BitConverter.GetBytes(Switch(Model_AssetID)));
            return list.ToArray();
        }

        public AssetID Model_AssetID { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudText : DynaHudBase
    {
        public override string Note => "Version is always 1";

        public DynaHudText() : base()
        {
            TextboxID = 0;
            TextID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (TextboxID == assetID || TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(TextboxID, ref result);
            Asset.Verify(TextID, ref result);
        }

        public DynaHudText(IEnumerable<byte> enumerable) : base (enumerable)
        {
            TextboxID = Switch(BitConverter.ToUInt32(Data, 0x18));
            TextID = Switch(BitConverter.ToUInt32(Data, 0x1C));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(TextboxID)));
            list.AddRange(BitConverter.GetBytes(Switch(TextID)));

            return list.ToArray();
        }

        public AssetID TextboxID { get; set; }
        public AssetID TextID { get; set; }
    }
}
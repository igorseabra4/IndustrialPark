using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaHudModel : DynaHud
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x1C;

        public DynaHudModel(AssetDYNA asset) : base(asset) { }

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
                
        public AssetID Model_AssetID
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }
    }
}
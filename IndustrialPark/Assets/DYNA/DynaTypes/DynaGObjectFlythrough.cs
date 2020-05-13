using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaGObjectFlythrough : DynaBase
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x4;

        public DynaGObjectFlythrough(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID) => FLY_ID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            if (FLY_ID == 0)
                result.Add("Flythrough with no FLY reference");
            Asset.Verify(FLY_ID, ref result);
        }
        
        public AssetID FLY_ID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }
    }
}
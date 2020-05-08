using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaHudText : DynaHud
    {
        public string Note => "Version is always 1";

        public override int StructSize => 0x20;

        public DynaHudText(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (TextBoxID == assetID || TextID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(TextBoxID, ref result);
            Asset.Verify(TextID, ref result);
        }
        
        public AssetID TextBoxID
        {
            get => ReadUInt(0x18);
            set => Write(0x18, value);
        }
        public AssetID TextID
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }
    }
}
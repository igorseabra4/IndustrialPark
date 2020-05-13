using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaEnemySupplyCrate : DynaEnemySB
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x54;

        public DynaEnemySupplyCrate(AssetDYNA asset) : base(asset) { }
        
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

        public EnemySupplyCrateType Type
        {
            get => (EnemySupplyCrateType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        public AssetID MVPT_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
    }
}
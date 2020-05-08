using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEnemyCritter : DynaPlaceableBase
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x58;

        public DynaEnemyCritter(AssetDYNA asset) : base(asset) { }
        
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

        public EnemyCritterType Type
        {
            get => (EnemyCritterType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy Critter")]
        public AssetID MVPT_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }

        [Category("Enemy Critter")]
        public AssetID Unknown54
        {
            get => ReadUInt(0x54);
            set => Write(0x54, value);
        }
        }
}
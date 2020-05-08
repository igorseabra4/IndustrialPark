using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaEnemyBucketOTron : DynaPlaceableBase
    {
        public string Note => "Version is always 4";

        public override int StructSize => 0x64;

        public DynaEnemyBucketOTron(AssetDYNA asset) : base(asset) { }
                
        public override bool HasReference(uint assetID)
        {
            if (GRUP_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (GRUP_AssetID == 0)
                result.Add("DYNA BucketOTron with GRUP Asset ID set to 0");

            Asset.Verify(GRUP_AssetID, ref result);
        }

        public EnemyBucketOTronType Type
        {
            get => (EnemyBucketOTronType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        public AssetID GRUP_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }
        public float UnknownFloat58
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        public int UnknownInt5C
        {
            get => ReadInt(0x5C);
            set => Write(0x5C, value);
        }
        public int UnknownInt60
        {
            get => ReadInt(0x60);
            set => Write(0x60, value);
        }
    }
}
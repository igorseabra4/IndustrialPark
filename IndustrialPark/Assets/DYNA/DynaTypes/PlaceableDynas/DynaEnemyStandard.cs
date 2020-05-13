using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaEnemyStandard : DynaEnemySB
    {
        public string Note => "Version is always 2";

        public override int StructSize => 0x6C;

        public DynaEnemyStandard(AssetDYNA asset) : base(asset) { }

        public override bool HasReference(uint assetID)
        {
            if (MVPT_AssetID == assetID)
                return true;
            if (MVPT_Group_AssetID == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;
            if (Unknown64 == assetID)
                return true;
            if (Unknown68 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (MVPT_AssetID == 0 && MVPT_Group_AssetID == 0)
                result.Add("DYNA Enemy Standard without set MVPT");

            Asset.Verify(MVPT_AssetID, ref result);
            Asset.Verify(MVPT_Group_AssetID, ref result);
            Asset.Verify(Unknown5C, ref result);
            Asset.Verify(Unknown60, ref result);
            Asset.Verify(Unknown64, ref result);
            Asset.Verify(Unknown68, ref result);
        }

        public EnemyStandardType Type
        {
            get => (EnemyStandardType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        public AssetID MVPT_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
        public AssetID MVPT_Group_AssetID
        {
            get => ReadUInt(0x54);
            set => Write(0x54, value);
        }
        public int MaybeFlags
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }
        public AssetID Unknown5C
        {
            get => ReadUInt(0x5C);
            set => Write(0x5C, value);
        }
        public AssetID Unknown60
        {
            get => ReadUInt(0x60);
            set => Write(0x60, value);
        }
        public AssetID Unknown64
        {
            get => ReadUInt(0x64);
            set => Write(0x64, value);
        }
        public AssetID Unknown68
        {
            get => ReadUInt(0x68);
            set => Write(0x68, value);
        }
    }
}
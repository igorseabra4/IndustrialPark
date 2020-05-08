using System.Collections.Generic;

namespace IndustrialPark
{
    public class DynaEnemyNeptune : DynaPlaceableBase
    {
        public string Note => "Version is always 4";

        public override int StructSize => 0x68;

        public DynaEnemyNeptune(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (Unknown50 == assetID)
                return true;
            if (Unknown54 == assetID)
                return true;
            if (Unknown58 == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;
            if (Unknown64 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            
            Asset.Verify(Unknown50, ref result);
            Asset.Verify(Unknown54, ref result);
            Asset.Verify(Unknown58, ref result);
            Asset.Verify(Unknown5C, ref result);
            Asset.Verify(Unknown60, ref result);
            Asset.Verify(Unknown64, ref result);
        }

        public EnemyNeptuneType Type
        {
            get => (EnemyNeptuneType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        public AssetID Unknown50
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
        public AssetID Unknown54
        {
            get => ReadUInt(0x54);
            set => Write(0x54, value);
        }
        public AssetID Unknown58
        {
            get => ReadUInt(0x58);
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
    }
}
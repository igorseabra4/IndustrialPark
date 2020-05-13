using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEnemyTurret : DynaEnemySB
    {
        public string Note => "Version is always 4";

        public override int StructSize => 0x64;

        public DynaEnemyTurret(AssetDYNA asset) : base(asset) { }
        
        public override bool HasReference(uint assetID)
        {
            if (Unknown54 == assetID)
                return true;
            if (Unknown5C == assetID)
                return true;
            if (Unknown60 == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Asset.Verify(Unknown54, ref result);
            Asset.Verify(Unknown5C, ref result);
            Asset.Verify(Unknown60, ref result);
        }
        public EnemyTurretType Type
        {
            get => (EnemyTurretType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat50
        {
            get => ReadFloat(0x50);
            set => Write(0x50, value);
        }
        public AssetID Unknown54
        {
            get => ReadUInt(0x54);
            set => Write(0x54, value);
        }
        public int UnknownInt58
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
    }
}
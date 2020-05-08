using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEnemyMindy : DynaPlaceableBase
    {
        public string Note => "Version is always 3";

        public override int StructSize => 0x68;

        public DynaEnemyMindy(AssetDYNA asset) : base(asset) { }
                
        public override bool HasReference(uint assetID)
        {
            if (TaskBox1_AssetID == assetID)
                return true;
            if (TaskBox2_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Asset.Verify(TaskBox1_AssetID, ref result);
            Asset.Verify(TaskBox2_AssetID, ref result);
        }

        public EnemyMindyType Type
        {
            get => (EnemyMindyType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }
        public AssetID TaskBox1_AssetID
        {
            get => ReadUInt(0x50);
            set => Write(0x50, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat54
        {
            get => ReadFloat(0x54);
            set => Write(0x54, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat58
        {
            get => ReadFloat(0x58);
            set => Write(0x58, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5C
        {
            get => ReadFloat(0x5C);
            set => Write(0x5C, value);
        }
        public int UnknownInt60
        {
            get => ReadInt(0x60);
            set => Write(0x60, value);
        }
        public AssetID TaskBox2_AssetID
        {
            get => ReadUInt(0x64);
            set => Write(0x64, value);
        }
    }
}
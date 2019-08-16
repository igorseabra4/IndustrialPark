using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class DynaEnemyMindy : DynaPlaceableBase
    {
        public override string Note => "Version is always 3";

        public DynaEnemyMindy(Platform platform) : base(platform)
        {
            TaskBox1_AssetID = 0;
            TaskBox2_AssetID = 0;
        }

        public DynaEnemyMindy(IEnumerable<byte> enumerable, Platform platform) : base (enumerable, platform)
        {
            TaskBox1_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            UnknownFloat54 = Switch(BitConverter.ToSingle(Data, 0x54));
            UnknownFloat58 = Switch(BitConverter.ToSingle(Data, 0x58));
            UnknownFloat5C = Switch(BitConverter.ToSingle(Data, 0x5C));
            UnknownInt60 = Switch(BitConverter.ToInt32(Data, 0x60));
            TaskBox2_AssetID = Switch(BitConverter.ToUInt32(Data, 0x64));

            CreateTransformMatrix();
        }
        
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

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(TaskBox1_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat54)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat58)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat5C)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt60)));
            list.AddRange(BitConverter.GetBytes(Switch(TaskBox2_AssetID)));

            return list.ToArray();
        }

        [Category("Enemy Mindy")]
        public EnemyMindyType Type
        {
            get => (EnemyMindyType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy Mindy")]
        public AssetID TaskBox1_AssetID { get; set; }

        [Category("Enemy Mindy"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat54 { get; set; }

        [Category("Enemy Mindy"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat58 { get; set; }

        [Category("Enemy Mindy"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat5C { get; set; }

        [Category("Enemy Mindy")]
        public int UnknownInt60 { get; set; }

        [Category("Enemy Mindy")]
        public AssetID TaskBox2_AssetID { get; set; }
    }
}
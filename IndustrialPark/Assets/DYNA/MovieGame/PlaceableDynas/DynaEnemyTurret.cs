using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyTurret : DynaPlaceableBase
    {
        public override string Note => "Version is always 4";

        public DynaEnemyTurret() : base()
        {
            Unknown54 = 0;
            Unknown5C = 0;
            Unknown60 = 0;
        }

        public DynaEnemyTurret(IEnumerable<byte> enumerable) : base (enumerable)
        {
            UnknownFloat50 = Switch(BitConverter.ToSingle(Data, 0x50));
            Unknown54 = Switch(BitConverter.ToUInt32(Data, 0x54));
            UnknownInt58 = Switch(BitConverter.ToInt32(Data, 0x58));
            Unknown5C = Switch(BitConverter.ToUInt32(Data, 0x5C));
            Unknown60 = Switch(BitConverter.ToUInt32(Data, 0x60));

            CreateTransformMatrix();
        }
        
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

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat50)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown54)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt58)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown5C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown60)));

            return list.ToArray();
        }

        [Category("Enemy Turret")]
        public EnemyTurretType Type
        {
            get => (EnemyTurretType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy Turret"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat50 { get; set; }

        [Category("Enemy Turret")]
        public AssetID Unknown54 { get; set; }

        [Category("Enemy Turret")]
        public int UnknownInt58 { get; set; }

        [Category("Enemy Turret")]
        public AssetID Unknown5C { get; set; }

        [Category("Enemy Turret")]
        public AssetID Unknown60 { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaEnemyFrogFish : DynaPlaceableBase
    {
        public override string Note => "Version is always 2";

        public DynaEnemyFrogFish() : base()
        {
            Player_AssetID = 0;
            Unknown58 = 0;
            Unknown5C = 0;
            Unknown60 = 0;
        }

        public DynaEnemyFrogFish(IEnumerable<byte> enumerable) : base (enumerable)
        {
            Player_AssetID = Switch(BitConverter.ToUInt32(Data, 0x50));
            UnknownByte54 = Data[0x54];
            UnknownByte55 = Data[0x55];
            UnknownByte56 = Data[0x56];
            UnknownByte57 = Data[0x57];
            Unknown58 = Switch(BitConverter.ToUInt32(Data, 0x58));
            Unknown5C = Switch(BitConverter.ToUInt32(Data, 0x5C));
            Unknown60 = Switch(BitConverter.ToUInt32(Data, 0x60));

            CreateTransformMatrix();
        }
        
        public override bool HasReference(uint assetID)
        {
            if (Player_AssetID == assetID)
                return true;
            if (Unknown58 == assetID)
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

            Asset.Verify(Player_AssetID, ref result);
            Asset.Verify(Unknown58, ref result);
            Asset.Verify(Unknown5C, ref result);
            Asset.Verify(Unknown60, ref result);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(Player_AssetID)));
            list.Add(UnknownByte54);
            list.Add(UnknownByte55);
            list.Add(UnknownByte56);
            list.Add(UnknownByte57);
            list.AddRange(BitConverter.GetBytes(Switch(Unknown58)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown5C)));
            list.AddRange(BitConverter.GetBytes(Switch(Unknown60)));

            return list.ToArray();
        }

        [Category("Enemy FrogFish")]
        public EnemyFrogFishType Type
        {
            get => (EnemyFrogFishType)(uint)Model_AssetID;
            set => Model_AssetID = (uint)value;
        }

        [Category("Enemy FrogFish")]
        public AssetID Player_AssetID { get; set; }
        [Category("Enemy FrogFish")]
        public byte UnknownByte54 { get; set; }
        [Category("Enemy FrogFish")]
        public byte UnknownByte55 { get; set; }
        [Category("Enemy FrogFish")]
        public byte UnknownByte56 { get; set; }
        [Category("Enemy FrogFish")]
        public byte UnknownByte57 { get; set; }

        [Category("Enemy FrogFish")]
        public AssetID Unknown58 { get; set; }

        [Category("Enemy FrogFish")]
        public AssetID Unknown5C { get; set; }

        [Category("Enemy FrogFish")]
        public AssetID Unknown60 { get; set; }
    }
}
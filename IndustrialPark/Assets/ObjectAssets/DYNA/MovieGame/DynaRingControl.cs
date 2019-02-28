using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaRingControl : DynaBase
    {
        public static uint RingModelAssetID = 0;

        public DynaRingControl() : base()
        {
            RingModel_AssetID = 0;
            RingSoundGroup_AssetID = 0;
            Rings_AssetIDs = new AssetID[0];
        }

        public DynaRingControl(IEnumerable<byte> enumerable) : base (enumerable)
        {
            PlayerType = (DynaRingControlPlayerType)Switch(BitConverter.ToInt32(Data, 0x0));
            RingModel_AssetID = Switch(BitConverter.ToUInt32(Data, 0x4));
            UnknownFloat1 = Switch(BitConverter.ToSingle(Data, 0x8));
            RingCount = Switch(BitConverter.ToInt32(Data, 0xC));
            UnknownInt1 = Switch(BitConverter.ToInt32(Data, 0x10));
            RingSoundGroup_AssetID = Switch(BitConverter.ToUInt32(Data, 0x14));
            UnknownInt2 = Switch(BitConverter.ToInt32(Data, 0x18));
            UnknownInt3 = Switch(BitConverter.ToInt32(Data, 0x1C));
            UnknownInt4 = Switch(BitConverter.ToInt32(Data, 0x20));
            RingsAreVisible = Switch(BitConverter.ToInt32(Data, 0x24));

            List<AssetID> rings = new List<AssetID>();
            for (int i = 0; i < RingCount; i++)
                rings.Add(Switch(BitConverter.ToUInt32(Data, 0x28 + 4 * i)));
            Rings_AssetIDs = rings.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            if (RingModel_AssetID == assetID)
                return true;
            if (RingSoundGroup_AssetID == assetID)
                return true;
            foreach (AssetID ring in Rings_AssetIDs)
                if (ring == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = new List<byte>();
            list.AddRange(BitConverter.GetBytes(Switch((int)PlayerType)));
            list.AddRange(BitConverter.GetBytes(Switch(RingModel_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownFloat1)));
            list.AddRange(BitConverter.GetBytes(Switch(RingCount)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt1)));
            list.AddRange(BitConverter.GetBytes(Switch(RingSoundGroup_AssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt2)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt3)));
            list.AddRange(BitConverter.GetBytes(Switch(UnknownInt4)));
            list.AddRange(BitConverter.GetBytes(Switch(RingsAreVisible)));

            foreach (AssetID ring in Rings_AssetIDs)
                list.AddRange(BitConverter.GetBytes(Switch(ring)));
            
            return list.ToArray();
        }

        public enum DynaRingControlPlayerType
        {
            Drive = 0,
            SpongebobPatrick = 1,
            Spongeball = 2,
            Unknown3 = 3,
            Slide = 4,
            SonicWaveGuitar = 5
        }

        [Category("Ring Control")]
        public DynaRingControlPlayerType PlayerType { get; set; }

        private uint _ringModelAssetID;
        [Category("Ring Control")]
        public AssetID RingModel_AssetID
        {
            get => _ringModelAssetID;
            set
            {
                _ringModelAssetID = value;
                RingModelAssetID = value;
            }
        }

        [Category("Ring Control"), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat1 { get; set; }

        [Category("Ring Control"), ReadOnly(true)]
        public int RingCount { get; set; }
        [Category("Ring Control")]
        public int UnknownInt1 { get; set; }

        [Category("Ring Control")]
        public AssetID RingSoundGroup_AssetID { get; set; }

        [Category("Ring Control")]
        public int UnknownInt2 { get; set; }
        [Category("Ring Control")]
        public int UnknownInt3 { get; set; }
        [Category("Ring Control")]
        public int UnknownInt4 { get; set; }
        [Category("Ring Control")]
        public int RingsAreVisible { get; set; }

        [Category("Ring Control")]
        public AssetID[] Rings_AssetIDs { get; set; }
    }
}
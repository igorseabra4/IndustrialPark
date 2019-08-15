using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class DynaHudMeterBase : DynaHudBase
    {
        public override string Note => "Version is always 3";

        public DynaHudMeterBase() : base()
        {
            StartIncrement_SoundAssetID = 0;
            Increment_SoundAssetID = 0;
            StartDecrement_SoundAssetID = 0;
            Decrement_SoundAssetID = 0;
        }

        public override bool HasReference(uint assetID)
        {
            if (StartIncrement_SoundAssetID == assetID)
                return true;
            if (Increment_SoundAssetID == assetID)
                return true;
            if (StartDecrement_SoundAssetID == assetID)
                return true;
            if (Decrement_SoundAssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            Asset.Verify(StartIncrement_SoundAssetID, ref result);
            Asset.Verify(Increment_SoundAssetID, ref result);
            Asset.Verify(StartDecrement_SoundAssetID, ref result);
            Asset.Verify(Decrement_SoundAssetID, ref result);
        }

        public DynaHudMeterBase(IEnumerable<byte> enumerable) : base (enumerable)
        {
            StartValue = Switch(BitConverter.ToSingle(Data, 0x18));
            MinValue = Switch(BitConverter.ToSingle(Data, 0x1C));
            MaxValue = Switch(BitConverter.ToSingle(Data, 0x20));
            IncrementTime = Switch(BitConverter.ToSingle(Data, 0x24));
            DecrementTime = Switch(BitConverter.ToSingle(Data, 0x28));
            StartIncrement_SoundAssetID = Switch(BitConverter.ToUInt32(Data, 0x2C));
            Increment_SoundAssetID = Switch(BitConverter.ToUInt32(Data, 0x30));
            StartDecrement_SoundAssetID = Switch(BitConverter.ToUInt32(Data, 0x34));
            Decrement_SoundAssetID = Switch(BitConverter.ToUInt32(Data, 0x38));
        }

        public override byte[] ToByteArray()
        {
            List<byte> list = base.ToByteArray().ToList();

            list.AddRange(BitConverter.GetBytes(Switch(StartValue)));
            list.AddRange(BitConverter.GetBytes(Switch(MinValue)));
            list.AddRange(BitConverter.GetBytes(Switch(MaxValue)));
            list.AddRange(BitConverter.GetBytes(Switch(IncrementTime)));
            list.AddRange(BitConverter.GetBytes(Switch(DecrementTime)));
            list.AddRange(BitConverter.GetBytes(Switch(StartIncrement_SoundAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Increment_SoundAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(StartDecrement_SoundAssetID)));
            list.AddRange(BitConverter.GetBytes(Switch(Decrement_SoundAssetID)));

            return list.ToArray();
        }

        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), Browsable(true)]
        public override float PositionZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleX { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleY { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public override float ScaleZ { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float StartValue { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float MinValue { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float MaxValue { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float IncrementTime { get; set; }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DecrementTime { get; set; }
        public AssetID StartIncrement_SoundAssetID { get; set; }
        public AssetID Increment_SoundAssetID { get; set; }
        public AssetID StartDecrement_SoundAssetID { get; set; }
        public AssetID Decrement_SoundAssetID { get; set; }
    }
}
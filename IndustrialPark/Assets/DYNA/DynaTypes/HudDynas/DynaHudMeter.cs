using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class DynaHudMeter : DynaHud
    {
        public string Note => "Version is always 3";

        public DynaHudMeter(AssetDYNA asset) : base(asset) { }
        
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
        
        [TypeConverter(typeof(FloatTypeConverter))]
        public float StartValue
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float MinValue
        {
            get => ReadFloat(0x1C);
            set => Write(0x1C, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float MaxValue
        {
            get => ReadFloat(0x20);
            set => Write(0x20, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float IncrementTime
        {
            get => ReadFloat(0x24);
            set => Write(0x24, value);
        }
        [TypeConverter(typeof(FloatTypeConverter))]
        public float DecrementTime
        {
            get => ReadFloat(0x28);
            set => Write(0x28, value);
        }
        public AssetID StartIncrement_SoundAssetID
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }
        public AssetID Increment_SoundAssetID
        {
            get => ReadUInt(0x30);
            set => Write(0x30, value);
        }
        public AssetID StartDecrement_SoundAssetID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }
        public AssetID Decrement_SoundAssetID
        {
            get => ReadUInt(0x38);
            set => Write(0x38, value);
        }
    }
}
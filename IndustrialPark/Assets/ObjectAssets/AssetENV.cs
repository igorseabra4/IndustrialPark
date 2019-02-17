using HipHopFile;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV : ObjectAsset
    {
        public AssetENV(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x44;

        public override bool HasReference(uint assetID)
        {
            if (BSP_AssetID == assetID)
                return true;
            if (StartCameraAssetID == assetID)
                return true;
            if (BSP_LKIT_AssetID == assetID)
                return true;
            if (Object_LKIT_AssetID == assetID)
                return true;
            if (BSP_Collision_AssetID == assetID)
                return true;
            if (BSP_FX_AssetID == assetID)
                return true;
            if (BSP_Camera_AssetID == assetID)
                return true;
            if (BSP_MAPR_AssetID == assetID)
                return true;
            if (BSP_MAPR_Collision_AssetID == assetID)
                return true;
            if (BSP_MAPR_FX_AssetID == assetID)
                return true;

            return base.HasReference(assetID);
        }

        [Category("Environment")]
        public AssetID BSP_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Environment")]
        public AssetID StartCameraAssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Environment")]
        public int ClimateFlags
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category("Environment"), TypeConverter(typeof(FloatTypeConverter))]
        public float ClimateStrengthMin
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category("Environment"), TypeConverter(typeof(FloatTypeConverter))]
        public float ClimateStrengthMax
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category("Environment")]
        public AssetID BSP_LKIT_AssetID
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category("Environment")]
        public AssetID Object_LKIT_AssetID
        {
            get => ReadUInt(0x20);
            set => Write(0x20, value);
        }

        [Category("Environment")]
        public int Padding24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category("Environment")]
        public AssetID BSP_Collision_AssetID
        {
            get => ReadUInt(0x28);
            set => Write(0x28, value);
        }	
	
        [Category("Environment")]
        public AssetID BSP_FX_AssetID 	
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category("Environment")]
        public AssetID BSP_Camera_AssetID
        {
            get => ReadUInt(0x30);
            set => Write(0x30, value);
        }

        [Category("Environment")]
        public AssetID BSP_MAPR_AssetID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }

        [Category("Environment")]
        public AssetID BSP_MAPR_Collision_AssetID
        {
            get => ReadUInt(0x38);
            set => Write(0x38, value);
        }

        [Category("Environment")]
        public AssetID BSP_MAPR_FX_AssetID
        {
            get => ReadUInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category("Environment"), TypeConverter(typeof(FloatTypeConverter))]
        public float LoldHeight
        {
            get => BitConverter.ToSingle(AHDR.data, 0x40);
            set
            {
                for (int i = 0; i < 4; i++)
                    AHDR.data[0x40 + i] = BitConverter.GetBytes(value)[i];
            }
        }
    }
}
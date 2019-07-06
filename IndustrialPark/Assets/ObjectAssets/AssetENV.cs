using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV : ObjectAsset
    {
        public AssetENV(Section_AHDR AHDR) : base(AHDR) { }

        protected override int EventStartOffset => 0x44;

        public override bool HasReference(uint assetID) => BSP_AssetID == assetID || StartCameraAssetID == assetID || BSP_LKIT_AssetID == assetID ||
            Object_LKIT_AssetID == assetID || BSP_Collision_AssetID == assetID || BSP_FX_AssetID == assetID || BSP_Camera_AssetID == assetID ||
            BSP_MAPR_AssetID == assetID || BSP_MAPR_Collision_AssetID == assetID || BSP_MAPR_FX_AssetID == assetID || base.HasReference(assetID);
        
        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            Verify(BSP_AssetID, ref result);
            if (StartCameraAssetID == 0)
                result.Add("ENV with StartCameraAssetID set to 0");
            Verify(StartCameraAssetID, ref result);
            Verify(BSP_LKIT_AssetID, ref result);
            Verify(Object_LKIT_AssetID, ref result);
            Verify(BSP_Collision_AssetID, ref result);
            Verify(BSP_FX_AssetID, ref result);
            Verify(BSP_Camera_AssetID, ref result);
            Verify(BSP_MAPR_AssetID, ref result);
            Verify(BSP_MAPR_Collision_AssetID, ref result);
            Verify(BSP_MAPR_FX_AssetID, ref result);
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
        [Description("Not present in Scooby.")]
        public float LoldHeight
        {
            get
            {
                if (Functions.currentGame == Game.Scooby)
                    return 0;

                return BitConverter.ToSingle(Data, 0x40);
            }
            set
            {
                if (Functions.currentGame != Game.Scooby)
                    for (int i = 0; i < 4; i++)
                        Data[0x40 + i] = BitConverter.GetBytes(value)[i];
            }
        }
    }
}
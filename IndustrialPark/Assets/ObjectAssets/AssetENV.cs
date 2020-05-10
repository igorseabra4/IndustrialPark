using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV : BaseAsset
    {
        public AssetENV(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }
        
        protected override int EventStartOffset => game == Game.Scooby ? 0x40 : game == Game.BFBB ? 0x44 : 0x5C;

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

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("LoldHeight");
            if (game != Game.Incredibles)
            {
                dt.RemoveProperty("UnknownInt44");
                dt.RemoveProperty("UnknownInt48");
                dt.RemoveProperty("UnknownInt4C");
                dt.RemoveProperty("UnknownInt50");
                dt.RemoveProperty("UnknownInt54");
                dt.RemoveProperty("UnknownInt58");
            }
            base.SetDynamicProperties(dt);
        }

        private const string categoryName = "Environment";

        [Category(categoryName)]
        public AssetID BSP_AssetID
        {
            get => ReadUInt(0x8);
            set => Write(0x8, value);
        }

        [Category(categoryName)]
        public AssetID StartCameraAssetID
        {
            get => ReadUInt(0xC);
            set => Write(0xC, value);
        }

        [Category(categoryName)]
        public int ClimateFlags
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ClimateStrengthMin
        {
            get => ReadFloat(0x14);
            set => Write(0x14, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float ClimateStrengthMax
        {
            get => ReadFloat(0x18);
            set => Write(0x18, value);
        }

        [Category(categoryName)]
        public AssetID BSP_LKIT_AssetID
        {
            get => ReadUInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category(categoryName)]
        public AssetID Object_LKIT_AssetID
        {
            get => ReadUInt(0x20);
            set => Write(0x20, value);
        }

        [Category(categoryName)]
        public int Padding24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category(categoryName)]
        public AssetID BSP_Collision_AssetID
        {
            get => ReadUInt(0x28);
            set => Write(0x28, value);
        }	
	
        [Category(categoryName)]
        public AssetID BSP_FX_AssetID 	
        {
            get => ReadUInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category(categoryName)]
        public AssetID BSP_Camera_AssetID
        {
            get => ReadUInt(0x30);
            set => Write(0x30, value);
        }

        [Category(categoryName)]
        public AssetID BSP_MAPR_AssetID
        {
            get => ReadUInt(0x34);
            set => Write(0x34, value);
        }

        [Category(categoryName)]
        public AssetID BSP_MAPR_Collision_AssetID
        {
            get => ReadUInt(0x38);
            set => Write(0x38, value);
        }

        [Category(categoryName)]
        public AssetID BSP_MAPR_FX_AssetID
        {
            get => ReadUInt(0x3C);
            set => Write(0x3C, value);
        }

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float LoldHeight
        {
            get => BitConverter.ToSingle(Data, 0x40);
            set
            {
                for (int i = 0; i < 4; i++)
                    Data[0x40 + i] = BitConverter.GetBytes(value)[i];
            }
        }

        [Category(categoryName)]
        public int UnknownInt44
        {
            get => ReadInt(0x44);
            set => Write(0x44, value);
        }

        [Category(categoryName)]
        public int UnknownInt48
        {
            get => ReadInt(0x48);
            set => Write(0x48, value);
        }

        [Category(categoryName)]
        public int UnknownInt4C
        {
            get => ReadInt(0x4C);
            set => Write(0x4C, value);
        }

        [Category(categoryName)]
        public int UnknownInt50
        {
            get => ReadInt(0x50);
            set => Write(0x50, value);
        }

        [Category(categoryName)]
        public int UnknownInt54
        {
            get => ReadInt(0x54);
            set => Write(0x54, value);
        }

        [Category(categoryName)]
        public int UnknownInt58
        {
            get => ReadInt(0x58);
            set => Write(0x58, value);
        }
    }
}
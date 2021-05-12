using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetENV : BaseAsset
    {
        private const string categoryName = "Environment";

        [Category(categoryName)]
        public AssetID BSP_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID StartCameraAssetID { get; set; }
        [Category(categoryName)]
        public int ClimateFlags { get; set; }
        [Category(categoryName)]
        public AssetSingle ClimateStrengthMin { get; set; }
        [Category(categoryName)]
        public AssetSingle ClimateStrengthMax { get; set; }
        [Category(categoryName)]
        public AssetID BSP_LKIT_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID Object_LKIT_AssetID { get; set; }
        [Category(categoryName)]
        public int Padding24 { get; set; }
        [Category(categoryName)]
        public AssetID BSP_Collision_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID BSP_FX_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID BSP_Camera_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID BSP_MAPR_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID BSP_MAPR_Collision_AssetID { get; set; }
        [Category(categoryName)]
        public AssetID BSP_MAPR_FX_AssetID { get; set; }
        [Category(categoryName)]
        public AssetSingle LoldHeight { get; set; }
        [Category(categoryName)]
        public int UnknownInt44 { get; set; }
        [Category(categoryName)]
        public int UnknownInt48 { get; set; }
        [Category(categoryName)]
        public int UnknownInt4C { get; set; }
        [Category(categoryName)]
        public int UnknownInt50 { get; set; }
        [Category(categoryName)]
        public int UnknownInt54 { get; set; }
        [Category(categoryName)]
        public int UnknownInt58 { get; set; }

        public AssetENV(string assetName, string startCamName) : base(assetName, AssetType.ENV, BaseAssetType.Env)
        {
            StartCameraAssetID = startCamName;
            LoldHeight = 10f;
        }

        public AssetENV(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                BSP_AssetID = reader.ReadUInt32();
                StartCameraAssetID = reader.ReadUInt32();
                ClimateFlags = reader.ReadInt32();
                ClimateStrengthMin = reader.ReadSingle();
                ClimateStrengthMax = reader.ReadSingle();
                BSP_LKIT_AssetID = reader.ReadUInt32();
                Object_LKIT_AssetID = reader.ReadUInt32();
                Padding24 = reader.ReadInt32();
                BSP_Collision_AssetID = reader.ReadUInt32();
                BSP_FX_AssetID = reader.ReadUInt32();
                BSP_Camera_AssetID = reader.ReadUInt32();
                BSP_MAPR_AssetID = reader.ReadUInt32();
                BSP_MAPR_Collision_AssetID = reader.ReadUInt32();
                BSP_MAPR_FX_AssetID = reader.ReadUInt32();
                if (game != Game.Scooby)
                {
                    reader.ReadInt32();
                    LoldHeight = BitConverter.ToSingle(AHDR.data, 0x40);
                }
                if (game == Game.Incredibles)
                {
                    UnknownInt44 = reader.ReadInt32();
                    UnknownInt48 = reader.ReadInt32();
                    UnknownInt4C = reader.ReadInt32();
                    UnknownInt50 = reader.ReadInt32();
                    UnknownInt54 = reader.ReadInt32();
                    UnknownInt58 = reader.ReadInt32();
                }
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {

                writer.Write(SerializeBase(endianness));
                writer.Write(BSP_AssetID);
                writer.Write(StartCameraAssetID);
                writer.Write(ClimateFlags);
                writer.Write(ClimateStrengthMin);
                writer.Write(ClimateStrengthMax);
                writer.Write(BSP_LKIT_AssetID);
                writer.Write(Object_LKIT_AssetID);
                writer.Write(Padding24);
                writer.Write(BSP_Collision_AssetID);
                writer.Write(BSP_FX_AssetID);
                writer.Write(BSP_Camera_AssetID);
                writer.Write(BSP_MAPR_AssetID);
                writer.Write(BSP_MAPR_Collision_AssetID);
                writer.Write(BSP_MAPR_FX_AssetID);
                if (game != Game.Scooby)
                    writer.Write(BitConverter.GetBytes(LoldHeight));
                if (game == Game.Incredibles)
                {
                    writer.Write(UnknownInt44);
                    writer.Write(UnknownInt48);
                    writer.Write(UnknownInt4C);
                    writer.Write(UnknownInt50);
                    writer.Write(UnknownInt54);
                    writer.Write(UnknownInt58);
                }

                writer.Write(SerializeLinks(endianness));

                return writer.ToArray();
            }
        }

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
    }
}
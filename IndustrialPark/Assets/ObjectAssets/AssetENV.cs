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
        public AssetID BSP { get; set; }
        [Category(categoryName), ValidReferenceRequired]
        public AssetID StartCamera { get; set; }
        [Category(categoryName)]
        public int ClimateFlags { get; set; }
        [Category(categoryName)]
        public AssetSingle ClimateStrengthMin { get; set; }
        [Category(categoryName)]
        public AssetSingle ClimateStrengthMax { get; set; }
        [Category(categoryName)]
        public AssetID BSP_LightKit { get; set; }
        [Category(categoryName)]
        public AssetID Object_LightKit { get; set; }
        [Category(categoryName)]
        public int Padding24 { get; set; }
        [Category(categoryName)]
        public AssetID BSP_Collision { get; set; }
        [Category(categoryName)]
        public AssetID BSP_FX { get; set; }
        [Category(categoryName)]
        public AssetID BSP_Camera { get; set; }
        [Category(categoryName)]
        public AssetID BSP_SurfaceMapper { get; set; }
        [Category(categoryName)]
        public AssetID BSP_Collision_SurfaceMapper { get; set; }
        [Category(categoryName)]
        public AssetID BSP_FX_SurfaceMapper { get; set; }
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

        public AssetENV(string assetName, string startCamName) : base(assetName, AssetType.Environment, BaseAssetType.Env)
        {
            StartCamera = startCamName;
            LoldHeight = 10f;
        }

        public AssetENV(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                BSP = reader.ReadUInt32();
                StartCamera = reader.ReadUInt32();
                ClimateFlags = reader.ReadInt32();
                ClimateStrengthMin = reader.ReadSingle();
                ClimateStrengthMax = reader.ReadSingle();
                BSP_LightKit = reader.ReadUInt32();
                Object_LightKit = reader.ReadUInt32();
                Padding24 = reader.ReadInt32();
                BSP_Collision = reader.ReadUInt32();
                BSP_FX = reader.ReadUInt32();
                BSP_Camera = reader.ReadUInt32();
                BSP_SurfaceMapper = reader.ReadUInt32();
                BSP_Collision_SurfaceMapper = reader.ReadUInt32();
                BSP_FX_SurfaceMapper = reader.ReadUInt32();
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
                writer.Write(BSP);
                writer.Write(StartCamera);
                writer.Write(ClimateFlags);
                writer.Write(ClimateStrengthMin);
                writer.Write(ClimateStrengthMax);
                writer.Write(BSP_LightKit);
                writer.Write(Object_LightKit);
                writer.Write(Padding24);
                writer.Write(BSP_Collision);
                writer.Write(BSP_FX);
                writer.Write(BSP_Camera);
                writer.Write(BSP_SurfaceMapper);
                writer.Write(BSP_Collision_SurfaceMapper);
                writer.Write(BSP_FX_SurfaceMapper);
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
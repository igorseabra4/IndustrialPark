using HipHopFile;
using System;
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

        private AssetID _object_lightkit;
        [Category(categoryName)]
        public AssetID Object_LightKit
        {
            get => _object_lightkit;
            set
            {
                _object_lightkit = value;
                updateLightKit(value);
            }
        }
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
        public AssetSingle MinBounds_X { get; set; }
        [Category(categoryName)]
        public AssetSingle MinBounds_Y { get; set; }
        [Category(categoryName)]
        public AssetSingle MinBounds_Z { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxBounds_X { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxBounds_Y { get; set; }
        [Category(categoryName)]
        public AssetSingle MaxBounds_Z { get; set; }

        public AssetENV(string assetName, string startCamName, UpdateLightKit updateLightKit) : base(assetName, AssetType.Environment, BaseAssetType.Env)
        {
            this.updateLightKit = updateLightKit;
            StartCamera = startCamName;
            LoldHeight = 10f;
        }

        public delegate void UpdateLightKit(AssetID lkitid);
        public readonly UpdateLightKit updateLightKit;
        public AssetENV(Section_AHDR AHDR, Game game, Endianness endianness, UpdateLightKit updateLightKit) : base(AHDR, game, endianness)
        {
            this.updateLightKit = updateLightKit;
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
                    LoldHeight = reader.ReadSingle();
                }
                if (game >= Game.Incredibles)
                {
                    MinBounds_X = reader.ReadSingle();
                    MinBounds_Y = reader.ReadSingle();
                    MinBounds_Z = reader.ReadSingle();
                    MaxBounds_X = reader.ReadSingle();
                    MaxBounds_Y = reader.ReadSingle();
                    MaxBounds_Z = reader.ReadSingle();
                }
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {


            base.Serialize(writer);
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
                writer.Write(LoldHeight);
            if (game >= Game.Incredibles)
            {
                writer.Write(MinBounds_X);
                writer.Write(MinBounds_Y);
                writer.Write(MinBounds_Z);
                writer.Write(MaxBounds_X);
                writer.Write(MaxBounds_Y);
                writer.Write(MaxBounds_Z);
            }

            SerializeLinks(writer);


        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("LoldHeight");
            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("MinBounds_X");
                dt.RemoveProperty("MinBounds_Y");
                dt.RemoveProperty("MinBounds_Z");
                dt.RemoveProperty("MaxBounds_X");
                dt.RemoveProperty("MaxBounds_Y");
                dt.RemoveProperty("MaxBounds_Z");
            }
            base.SetDynamicProperties(dt);
        }
    }
}
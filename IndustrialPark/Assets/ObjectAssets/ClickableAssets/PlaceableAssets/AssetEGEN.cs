﻿using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetEGEN : EntityAsset
    {
        protected const string categoryName = "Electric Arc";

        [Category(categoryName)]
        public AssetSingle Src_dpos_X { get; set; }
        [Category(categoryName)]
        public AssetSingle Src_dpos_Y { get; set; }
        [Category(categoryName)]
        public AssetSingle Src_dpos_Z { get; set; }
        [Category(categoryName)]
        public byte DamageType { get; set; }
        [Category(categoryName)]
        public FlagBitmask EgenFlags { get; set; } = ByteFlagsDescriptor("Start on");
        [Category(categoryName)]
        public AssetSingle ActiveTimeSeconds { get; set; }
        [Category(categoryName)]
        public AssetID OnAnimation { get; set; }

        public AssetEGEN(string assetName, Vector3 position) : base(assetName, AssetType.ElectricArcGenerator, BaseAssetType.EGenerator, position)
        {
            OnAnimation = 0xCE7F8131;
        }

        public AssetEGEN(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                Src_dpos_X = reader.ReadSingle();
                Src_dpos_Y = reader.ReadSingle();
                Src_dpos_Z = reader.ReadSingle();
                DamageType = reader.ReadByte();
                EgenFlags.FlagValueByte = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                ActiveTimeSeconds = reader.ReadSingle();
                OnAnimation = reader.ReadUInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Src_dpos_X);
            writer.Write(Src_dpos_Y);
            writer.Write(Src_dpos_Z);
            writer.Write(DamageType);
            writer.Write(EgenFlags.FlagValueByte);
            writer.Write((short)0);
            writer.Write(ActiveTimeSeconds);
            writer.Write(OnAnimation);

            SerializeLinks(writer);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}
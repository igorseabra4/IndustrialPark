﻿using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;
using AssetEditorColors;

namespace IndustrialPark
{
    public class AssetFOG : BaseAsset
    {
        private const string categoryName = "Fog";

        [Category(categoryName)]
        public AssetColor StartColor { get; set; }
        [Category(categoryName)]
        public AssetColor EndColor { get; set; }
        [Category(categoryName)]
        public AssetSingle FogDensity { get; set; }
        [Category(categoryName)]
        public AssetSingle StartDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle EndDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle TransitionTime { get; set; }
        [Category(categoryName)]
        public byte FogType { get; set; }

        public AssetFOG(string assetName) : base(assetName, AssetType.FOG, BaseAssetType.Fog)
        {
            EndColor = new AssetColor();
            StartColor = new AssetColor();
            FogDensity = 1;
            StartDistance = 100;
            EndDistance = 400;
        }

        public AssetFOG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                EndColor = reader.ReadColor();
                StartColor = reader.ReadColor();
                FogDensity = reader.ReadSingle();
                StartDistance = reader.ReadSingle();
                EndDistance = reader.ReadSingle();
                TransitionTime = reader.ReadSingle();
                FogType = reader.ReadByte();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

                writer.Write(EndColor);
                writer.Write(StartColor);
                writer.Write(FogDensity);
                writer.Write(StartDistance);
                writer.Write(EndDistance);
                writer.Write(TransitionTime);
                writer.Write(FogType);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }
    }
}
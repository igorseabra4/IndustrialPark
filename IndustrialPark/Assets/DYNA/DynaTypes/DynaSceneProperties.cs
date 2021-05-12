using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaSceneProperties : AssetDYNA
    {
        private const string dynaCategoryName = "Scene Properties";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public int idle03ExtraCount { get; set; }
        [Category(dynaCategoryName)]
        public int idle03Extras { get; set; }
        [Category(dynaCategoryName)]
        public int idle04ExtraCount { get; set; }
        [Category(dynaCategoryName)]
        public int idle04Extras { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte bombCount { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte extraIdleDelay { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte hdrGlow { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte hdrDarken { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackgroundMusic_SoundAssetID { get; set; }
        [Category(dynaCategoryName)]
        public int scenePropertiesFlags { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle waterTileWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle lodFadeDistance { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt24 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt28 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt2C { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt30 { get; set; }

        public DynaSceneProperties(string assetName) : base(assetName, DynaType.SceneProperties, 1)
        {
            idle03ExtraCount = 0;
            idle03Extras = 52;
            idle04ExtraCount = 0;
            idle04Extras = 52;
            bombCount = 0x14;
            extraIdleDelay = 0x05;
            hdrGlow = 0x03;
            hdrDarken = 0x1E;
            BackgroundMusic_SoundAssetID = 0;
            scenePropertiesFlags = 1;
            waterTileWidth = 0;
            lodFadeDistance = 4;
            UnknownInt24 = 0;
            UnknownInt28 = 0;
            UnknownInt2C = 0;
            UnknownInt30 = 0;
        }

        public DynaSceneProperties(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.SceneProperties, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                idle03ExtraCount = reader.ReadInt32();
                idle03Extras = reader.ReadInt32();
                idle04ExtraCount = reader.ReadInt32();
                idle04Extras = reader.ReadInt32();
                bombCount = reader.ReadByte();
                extraIdleDelay = reader.ReadByte();
                hdrGlow = reader.ReadByte();
                hdrDarken = reader.ReadByte();
                BackgroundMusic_SoundAssetID = reader.ReadUInt32();
                scenePropertiesFlags = reader.ReadInt32();
                waterTileWidth = reader.ReadSingle();
                lodFadeDistance = reader.ReadSingle();
                UnknownInt24 = reader.ReadInt32();
                UnknownInt28 = reader.ReadInt32();
                UnknownInt2C = reader.ReadInt32();
                UnknownInt30 = reader.ReadInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(idle03ExtraCount);
                writer.Write(idle03Extras);
                writer.Write(idle04ExtraCount);
                writer.Write(idle04Extras);
                writer.Write(bombCount);
                writer.Write(extraIdleDelay);
                writer.Write(hdrGlow);
                writer.Write(hdrDarken);
                writer.Write(BackgroundMusic_SoundAssetID);
                writer.Write(scenePropertiesFlags);
                writer.Write(waterTileWidth);
                writer.Write(lodFadeDistance);
                writer.Write(UnknownInt24);
                writer.Write(UnknownInt28);
                writer.Write(UnknownInt2C);
                writer.Write(UnknownInt30);

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID) => BackgroundMusic_SoundAssetID == assetID || base.HasReference(assetID);

        public override void Verify(ref List<string> result)
        {
            if (BackgroundMusic_SoundAssetID == 0)
                result.Add("Scene Properties with no song reference");
            Verify(BackgroundMusic_SoundAssetID, ref result);
        }
    }
}
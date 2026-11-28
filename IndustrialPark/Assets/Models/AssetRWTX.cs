using HipHopFile;
using IndustrialPark.Archives;
using RenderWareFile;
using RenderWareFile.Enums;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace IndustrialPark.Assets
{
    public class AssetRWTX : Asset
    {
        public override string AssetInfo => $"{platformFormat} {RenderWareVersion} {Size}";
        private const string categoryName = "\t\t\tTexture";

        private TextureDictionary_0016 dictionary;

        public AssetRWTX(Section_AHDR AHDR, EndianBinaryReader reader) : base(AHDR)
        {
            ReadDictionary(reader.BaseStream, AHDR.data.Length);
        }

        public AssetRWTX(string assetName, byte[] data) : base(assetName, AssetType.Texture)
        {
            using var stream = new MemoryStream(data, false);
            ReadDictionary(stream, data.Length);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (!AssetService.HasWhoTargets(assetID))
                result.Add("Texture appears unused; no references found.");
        }

        [Category(categoryName)]
        public string Name { get => Path.GetFileNameWithoutExtension(assetName); }

        [Browsable(false)]
        public TextureNative_0015 Native
        {
            get => dictionary.textureNativeList[0];
            set => dictionary.textureNativeList[0] = value ?? throw new ArgumentNullException(nameof(value));
        }

        [Browsable(false)]
        public IReadOnlyList<TextureNative_0015> Natives => dictionary.textureNativeList;

        [Description("Present on the header of the TXD when version > 3.5.")]
        public short DeviceId
        {
            get => dictionary.textureDictionaryStruct.deviceId;
            set => dictionary.textureDictionaryStruct.deviceId = value;
        }

        private string platformFormat => GetPlatform();
        private RWVersion RenderWareVersion => dictionary.renderWareVersion;

        public void ReplaceDictionary(byte[] data)
        {
            using var stream = new MemoryStream(data, false);
            ReadDictionary(stream, data.Length);
        }

        private void ReadDictionary(Stream stream, int length)
        {
            long start = stream.Position;
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);
            if ((RenderWareFile.Section)reader.ReadInt32() != RenderWareFile.Section.TextureDictionary)
                throw new InvalidDataException("Texture asset does not contain a texture dictionary.");
            var parsed = new TextureDictionary_0016(reader);
            if (parsed.textureNativeList.Count == 0 || stream.Position - start != length)
                throw new InvalidDataException("Texture asset must contain one non-empty texture dictionary.");
            dictionary = parsed;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(dictionary.GetBytes(RenderWareVersion));
        }

        [Category(categoryName)]
        public TextureFilterMode FilterMode
        {
            get => Native.textureNativeStruct.filterMode;
            set => Native.textureNativeStruct.filterMode = value;
        }

        [Category(categoryName)]
        public TextureAddressMode AddressModeU
        {
            get => Native.textureNativeStruct.addressModeU;
            set => Native.textureNativeStruct.addressModeU = value;
        }

        [Category(categoryName)]
        public TextureAddressMode AddressModeV
        {
            get => Native.textureNativeStruct.addressModeV;
            set => Native.textureNativeStruct.addressModeV = value;
        }

        [Category(categoryName), ReadOnly(true)]
        public string Size
        {
            get
            {
                if (Native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                    return $"{Native.textureNativeStruct.width}x{Native.textureNativeStruct.height}";
                else
                    return $"{(short)Native.PS2RasterFormat.Width}x{(short)Native.PS2RasterFormat.Height}";
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public TextureRasterFormat RasterFormat
        {
            get
            {
                if (Native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                    return (TextureRasterFormat)((int)Native.textureNativeStruct.rasterFormatFlags & 0xF00);
                else
                    return (TextureRasterFormat)(((int)Native.PS2RasterFormat.RasterFormat & 0xF) << 8);
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public TextureRasterFormat PALFormat
        {
            get
            {
                if (Native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                    return (TextureRasterFormat)((int)Native.textureNativeStruct.rasterFormatFlags & 0x6000);
                else
                    return (TextureRasterFormat)(((int)Native.PS2RasterFormat.RasterFormat & 0x60) << 8);
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public bool HasMipMaps
        {
            get
            {
                if (Native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                    return ((int)Native.textureNativeStruct.rasterFormatFlags & 0x8000) != 0;
                else
                    return ((int)Native.PS2RasterFormat.RasterFormat & 0x80) != 0;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public int MipMapCount => Native.textureNativeStruct.mipMapCount;

        [Category(categoryName), ReadOnly(true)]
        public bool AutoGenerateMipMaps
        {
            get
            {
                if (Native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                    return ((int)Native.textureNativeStruct.rasterFormatFlags & 0x1000) != 0;
                else
                    return ((int)Native.PS2RasterFormat.RasterFormat & 0x10) != 0;
            }
        }

        #region GameCube
        [Category(GameCubeCategory), ReadOnly(true)]
        public GXTexFmt Format => (GXTexFmt)Native.textureNativeStruct.type;

        [Category(GameCubeCategory), ReadOnly(true)]
        public GXTexFmt TLUTFormat => (GXTexFmt)Native.textureNativeStruct.compression;

        [Category(GameCubeCategory)]
        public uint MaxAniso
        {
            get => Native.textureNativeStruct.maxAniso;
            set => Native.textureNativeStruct.maxAniso = value;
        }

        [Category(GameCubeCategory)]
        public int BiasClamp
        {
            get => Native.textureNativeStruct.biasClamp;
            set => Native.textureNativeStruct.biasClamp = value;
        }

        [Category(GameCubeCategory)]
        public int EdgeLod
        {
            get => Native.textureNativeStruct.edgeLod;
            set => Native.textureNativeStruct.edgeLod = value;
        }

        [Category(GameCubeCategory)]
        public float LodBias
        {
            get => Native.textureNativeStruct.lodBias;
            set => Native.textureNativeStruct.lodBias = value;
        }
        #endregion

        #region Xbox
        [Category(XboxCategory), ReadOnly(true)]
        public DXTFormat DXTFormat => (DXTFormat)Native.textureNativeStruct.compression;
        #endregion

        public string GetPlatform() => Native.textureNativeStruct.platformType switch
        {
            TexturePlatformID.Xbox => "Xbox",
            TexturePlatformID.PCD3D8 or TexturePlatformID.PCD3D9 => "PC",
            TexturePlatformID.GameCube => "GC",
            TexturePlatformID.PS2 => "PS2",
            _ => "Unknown Format",
        };

        private const string GameCubeCategory = "GameCube";
        private const string XboxCategory = "Xbox";
        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            TexturePlatformID platform = Native.textureNativeStruct.platformType;

            if (platform != TexturePlatformID.GameCube)
                dt.RemovePropertiesByCategory(GameCubeCategory);
            if (platform != TexturePlatformID.Xbox)
                dt.RemovePropertiesByCategory(XboxCategory);
            if (platform == TexturePlatformID.PS2)
                dt.RemoveProperty("MipMapCount");
        }
    }
}
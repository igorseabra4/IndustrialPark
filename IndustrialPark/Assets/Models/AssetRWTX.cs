using HipHopFile;
using RenderWareFile;
using RenderWareFile.Enums;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace IndustrialPark
{
    public class AssetRWTX : AssetWithData
    {
        public override string AssetInfo => $"{platformFormat} {renderWareVersion} {base.AssetInfo}";
        private const string categoryName = "\t\t\tTexture";

        private string platformFormat;

        public AssetRWTX(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            platformFormat = GetPlatform();
            try
            {
                using (var reader = new BinaryReader(new MemoryStream(Data)))
                {
                    reader.BaseStream.Position = 0x8;
                    renderWareVersion = reader.ReadInt32();
                }
            }
            catch { }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            if (TextureAsRWSections.Length == 0)
                result.Add("Failed to read RWTX asset. This might be just a library error and does not necessarily mean the texture is broken.");

            if (Program.MainForm.WhoTargets(assetID).Count == 0)
                result.Add("Texture appears to be unused, as no other asset references it. This might just mean I haven't looked properly for an asset which does does, though.");
        }

        [Category(categoryName)]
        public string Name { get => Path.GetFileNameWithoutExtension(assetName); }

        private RWVersion renderWareVersion;

        private RWSection[] _textureAsRWSections;
        private RWSection[] TextureAsRWSections
        {
            get
            {
                if (_textureAsRWSections != null)
                    return _textureAsRWSections;
                try
                {
                    ReadFileMethods.treatStuffAsByteArray = true;
                    _textureAsRWSections = ReadFileMethods.ReadRenderWareFile(Data);
                    renderWareVersion = _textureAsRWSections[0].renderWareVersion;
                    ReadFileMethods.treatStuffAsByteArray = false;
                    return _textureAsRWSections;
                }
                catch
                {
                    return new RWSection[0];
                }
            }
            set
            {
                ReadFileMethods.treatStuffAsByteArray = true;
                _textureAsRWSections = value;
                Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
                ReadFileMethods.treatStuffAsByteArray = false;
            }
        }

        [Category(categoryName)]
        public TextureFilterMode FilterMode
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.filterMode;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.filterMode = value;

                TextureAsRWSections = file;
            }
        }

        [Category(categoryName)]
        public TextureAddressMode AddressModeU
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.addressModeU;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.addressModeU = value;

                TextureAsRWSections = file;
            }
        }

        [Category(categoryName)]
        public TextureAddressMode AddressModeV
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.addressModeV;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.addressModeV = value;

                TextureAsRWSections = file;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public string Size
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            if (native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                                return $"{native.textureNativeStruct.width}x{native.textureNativeStruct.height}";
                            else
                                return $"{(short)native.PS2RasterFormat.Width}x{(short)native.PS2RasterFormat.Height}";
                return "0x0";
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public TextureRasterFormat RasterFormat
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            if (native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                                return (TextureRasterFormat)((int)native.textureNativeStruct.rasterFormatFlags & 0xF00);
                            else
                                return (TextureRasterFormat)(((int)native.PS2RasterFormat.RasterFormat & 0xF) << 8);
                return 0;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public TextureRasterFormat PALFormat
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            if (native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                                return (TextureRasterFormat)((int)native.textureNativeStruct.rasterFormatFlags & 0x6000);
                            else
                                return (TextureRasterFormat)(((int)native.PS2RasterFormat.RasterFormat & 0x60) << 8);
                return 0;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public bool HasMipMaps
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            if (native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                                return ((int)native.textureNativeStruct.rasterFormatFlags & 0x8000) != 0;
                            else
                                return ((int)native.PS2RasterFormat.RasterFormat & 0x80) != 0;
                return false;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public int MipMapCount
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return (int)native.textureNativeStruct.mipMapCount;
                return 0;
            }
        }

        [Category(categoryName), ReadOnly(true)]
        public bool AutoGenerateMipMaps
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            if (native.textureNativeStruct.platformType != TexturePlatformID.PS2)
                                return ((int)native.textureNativeStruct.rasterFormatFlags & 0x1000) != 0;
                            else
                                return ((int)native.PS2RasterFormat.RasterFormat & 0x10) != 0;
                return false;
            }
        }

        #region GameCube
        [Category(GameCubeCategory), ReadOnly(true)]
        public GXTexFmt Format
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return (GXTexFmt)native.textureNativeStruct.type;
                return GXTexFmt.None;
            }
        }

        [Category(GameCubeCategory), ReadOnly(true)]
        public GXTexFmt TLUTFormat
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return (GXTexFmt)native.textureNativeStruct.compression;
                return GXTexFmt.None;
            }
        }

        [Category(GameCubeCategory)]
        public uint MaxAniso
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.maxAniso;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.maxAniso = value;

                TextureAsRWSections = file;
            }
        }

        [Category(GameCubeCategory)]
        public int BiasClamp
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.biasClamp;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.biasClamp = value;

                TextureAsRWSections = file;
            }
        }

        [Category(GameCubeCategory)]
        public int EdgeLod
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.edgeLod;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.edgeLod = value;

                TextureAsRWSections = file;
            }
        }

        [Category(GameCubeCategory)]
        public float LodBias
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.lodBias;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.lodBias = value;

                TextureAsRWSections = file;
            }
        }
        #endregion

        #region Xbox
        [Category(XboxCategory), ReadOnly(true)]
        public DXTFormat DXTFormat
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return (DXTFormat)native.textureNativeStruct.compression;
                return DXTFormat.None;
            }
        }
        #endregion

        public string GetPlatform()
        {
            foreach (var rws in TextureAsRWSections)
                if (rws is TextureDictionary_0016 textD)
                    foreach (TextureNative_0015 native in textD.textureNativeList)
                        switch (native.textureNativeStruct.platformType)
                        {
                            case TexturePlatformID.Xbox:
                                return "Xbox";
                            case TexturePlatformID.PCD3D8:
                            case TexturePlatformID.PCD3D9:
                                return "PC";
                            case TexturePlatformID.GameCube:
                                return "GC";
                            case TexturePlatformID.PS2:
                                return "PS2";
                        }
            return "Unknown Format";
        }

        public TexturePlatformID GetPlatformEnum()
        {
            foreach (var rws in TextureAsRWSections)
                if (rws is TextureDictionary_0016 textD)
                    foreach (TextureNative_0015 native in textD.textureNativeList)
                        return native.textureNativeStruct.platformType;
            return TexturePlatformID.ANY;
        }

        private const string GameCubeCategory = "GameCube";
        private const string XboxCategory = "Xbox";
        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            TexturePlatformID platform = GetPlatformEnum();

            if (platform != TexturePlatformID.GameCube)
                dt.RemovePropertiesByCategory(GameCubeCategory);
            if (platform != TexturePlatformID.Xbox)
                dt.RemovePropertiesByCategory(XboxCategory);
            if (platform == TexturePlatformID.PS2)
                dt.RemoveProperty("MipMapCount");
        }
    }
}
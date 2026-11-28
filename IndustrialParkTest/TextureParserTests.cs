using HipHopFile;
using IndustrialPark;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
using RenderWareFile;
using RenderWareFile.Sections;
using RenderWareFile.Textures;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace IndustrialParkTest;

public class TextureParserTests
{
    [Theory]
    [InlineData(13)]
    [InlineData(16)]
    public void PremultipliedXboxDxtRequiresFallback(byte compression)
    {
        var texture = new TextureNativeStruct_0001
        {
            platformType = TexturePlatformID.Xbox,
            width = 4,
            height = 4,
            compression = compression,
            mipMaps = new[] { new MipMapEntry(16, new byte[16]) }
        };
        Assert.False(TextureDecoder.CanDecode(texture));
        Assert.Null(TextureDecoder.DecodeAll(new TextureNative_0015 { textureNativeStruct = texture }, out _, out _));
    }

    [Fact]
    public void DecodesPs2TrueColorMipmapsAndRejectsInvalidDisplayChain()
    {
        var header = new byte[96];
        using var reader = new BinaryReader(new MemoryStream(header));
        byte[] basePixels = { 0x12, 0x34, 0x56, 0x80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] mipPixels = { 0xAB, 0xCD, 0xEF, 0x40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        byte[] first = MakeGifUpload(2, 2, basePixels);
        byte[] second = MakeGifUpload(1, 1, mipPixels);
        var raster = new Ps2TextureRasterFormatStruct_0001(reader)
        {
            Width = 2, Height = 2, Depth = 32, FullRasterFormat = 0x20504,
            sectionData = first.Concat(second).ToArray(),
            gsMipmapDataSize = first.Length + second.Length
        };
        var native = new TextureNative_0015
        {
            textureNativeStruct = new TextureNativeStruct_0001 { platformType = TexturePlatformID.PS2, textureName = "truecolor" },
            PS2RasterFormat = raster
        };
        var mipmaps = TextureDecoder.DecodeAll(native, out int width, out int height);
        Assert.Equal(2, width);
        Assert.Equal(2, height);
        Assert.Equal(2, mipmaps.Length);
        Assert.Equal(new byte[] { 0x56, 0x34, 0x12, 255 }, mipmaps[0][..4]);
        Assert.Equal(new byte[] { 0xEF, 0xCD, 0xAB, 127 }, mipmaps[1]);
        Assert.Equal(mipmaps[0], TextureDecoder.DecodeBase(native, out int baseWidth, out int baseHeight));
        Assert.Equal(2, baseWidth);
        Assert.Equal(2, baseHeight);
        using (var bitmap = TextureConverter.GetTextureAsBitmap(native))
            Assert.Equal(System.Drawing.Color.FromArgb(255, 0x12, 0x34, 0x56), bitmap.GetPixel(0, 0));

        second = MakeGifUpload(2, 1, mipPixels);
        raster.sectionData = first.Concat(second).ToArray();
        raster.gsMipmapDataSize = raster.sectionData.Length;
        Assert.Null(TextureDecoder.DecodeAll(native, out _, out _));
        Assert.Equal(mipmaps[0], TextureDecoder.DecodeBase(native, out _, out _));

        raster.gsMipmapDataSize--;
        raster.sectionData = raster.sectionData[..^1];
        Assert.Null(TextureDecoder.DecodeAll(native, out _, out _));
        Assert.Null(TextureDecoder.DecodeBase(native, out _, out _));
    }

    [Fact]
    public void BaseLevelDecodeDoesNotDecodeDamagedPcMipmaps()
    {
        var texture = MakePcTexture("mips");
        texture.width = 2;
        texture.height = 2;
        texture.mipMaps = new[]
        {
            new MipMapEntry(16, new byte[] { 1, 2, 3, 255, 4, 5, 6, 255, 7, 8, 9, 255, 10, 11, 12, 255 }),
            new MipMapEntry(1, new byte[] { 0 })
        };
        var native = new TextureNative_0015 { textureNativeStruct = texture };

        Assert.Null(TextureDecoder.DecodeAll(native, out _, out _));
        var pixels = TextureDecoder.DecodeBase(native, out int width, out int height);
        Assert.Equal(2, width);
        Assert.Equal(2, height);
        Assert.Equal(texture.mipMaps[0].data, pixels);
        using var bitmap = TextureConverter.GetTextureAsBitmap(native);
        Assert.Equal(System.Drawing.Color.FromArgb(255, 3, 2, 1), bitmap.GetPixel(0, 0));

        texture.mipMaps[1] = new MipMapEntry(4, new byte[] { 13, 14, 15, 255 });
        var mipmaps = TextureDecoder.DecodeAll(native, out _, out _);
        Assert.Equal(2, mipmaps.Length);
        Assert.Equal(texture.mipMaps[1].data, mipmaps[1]);
        Assert.Equal(mipmaps[1], TextureDecoder.DecodeMip(texture, 1));
    }

    [Fact]
    public void DisplayRejectsMipChainsThatCannotBeUploadedCompletely()
    {
        var pc = MakePcTexture("too-many-mips");
        pc.mipMaps = new[] { pc.mipMaps[0], new MipMapEntry(4, new byte[4]) };
        var pcNative = new TextureNative_0015 { textureNativeStruct = pc };
        Assert.Null(TextureDecoder.DecodeAll(pcNative, out _, out _));
        Assert.NotNull(TextureDecoder.DecodeBase(pcNative, out _, out _));

        var xbox = new TextureNativeStruct_0001
        {
            platformType = TexturePlatformID.Xbox, width = 8, height = 8, compression = 12,
            mipMaps = new[]
            {
                new MipMapEntry(32, new byte[32]),
                new MipMapEntry(0, Array.Empty<byte>()),
                new MipMapEntry(8, new byte[8])
            }
        };
        var xboxNative = new TextureNative_0015 { textureNativeStruct = xbox };
        Assert.Null(TextureDecoder.DecodeAll(xboxNative, out _, out _));
        Assert.NotNull(TextureDecoder.DecodeBase(xboxNative, out _, out _));
        xbox.mipMaps[2] = new MipMapEntry(0, Array.Empty<byte>());
        Assert.Single(TextureDecoder.DecodeAll(xboxNative, out _, out _));
    }

    [Fact]
    public void Ps2GifParserPublicUploadsOwnTheirPayload()
    {
        byte[] source = MakeGifUpload(2, 2, new byte[16]);
        source[source.Length - 16] = 17;
        var upload = Assert.Single(Ps2GifParser.Parse(source));
        source[source.Length - 16] = 99;
        Assert.Equal(17, upload.Data[0]);
    }

    [Fact]
    public void EmptyBitmapImportDoesNotInvokeConverter()
    {
        Assert.Empty(TextureIOHelper.CreateRWTXsFromBitmaps(Game.BFBB, Platform.GameCube,
            new System.Collections.Generic.List<string>(), false, false, false, false, false));
    }

    [Fact]
    public void DecodesPcIndexedRowsWithFourBytePitch()
    {
        var palette = new RenderWareFile.Color[256];
        palette[1] = new RenderWareFile.Color(1, 2, 3, 255);
        palette[2] = new RenderWareFile.Color(4, 5, 6, 255);
        var texture = new TextureNativeStruct_0001
        {
            platformType = TexturePlatformID.PCD3D8,
            width = 3,
            height = 2,
            rasterFormatFlags = TextureRasterFormat.RASTER_C8888 | TextureRasterFormat.RASTER_PAL8,
            palette = palette,
            mipMaps = new[] { new MipMapEntry(8, new byte[] { 1, 1, 1, 0, 2, 2, 2, 0 }) }
        };
        byte[] pixels = TextureDecoder.DecodeAll(texture)[0];
        Assert.Equal(new byte[] { 6, 5, 4, 255 }, pixels[12..16]);
        var native = new TextureNative_0015 { textureNativeStruct = texture };
        var decoded = TextureDecoder.DecodeAll(native, out int width, out int height);
        Assert.Equal(3, width);
        Assert.Equal(2, height);
        Assert.Equal(pixels, Assert.Single(decoded));
    }

    [Fact]
    public void PreservesPs2NamePaddingAndWritesEditedNames()
    {
        var header = new byte[96];
        BitConverter.GetBytes(new RWVersion(3, 5, 0, 0).Write()).CopyTo(header, 4);
        using var reader = new BinaryReader(new MemoryStream(header));
        var native = new TextureNative_0015
        {
            textureNativeStruct = new TextureNativeStruct_0001
            {
                platformType = TexturePlatformID.PS2,
                textureName = "a",
                alphaName = "bc"
            },
            PS2RasterFormat = new Ps2TextureRasterFormatStruct_0001(reader) { sectionData = Array.Empty<byte>() },
            textureNativeExtension = new Extension_0003()
        };
        byte[] source = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(native), new RWVersion(3, 5, 0, 0));
        source[74] = 0xA7;
        source[75] = 0xFF;
        source[91] = 0x9F;
        var sections = ReadFileMethods.ReadRenderWareFile(source);
        var parsed = ((TextureDictionary_0016)sections[0]).textureNativeList[0].textureNativeStruct;

        Assert.Equal(source, ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion));
        parsed.textureName = "new";
        byte[] edited = ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion);
        var reparsed = ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(edited)[0]).textureNativeList[0].textureNativeStruct;
        Assert.Equal("new", reparsed.textureName);
        Assert.Equal("bc", reparsed.alphaName);
        Assert.Equal(0x9F, edited[91]);
        Assert.NotEqual(0xA7, edited[74]);

        parsed.alphaName = "mask";
        byte[] bothEdited = ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion);
        var bothReparsed = ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(bothEdited)[0]).textureNativeList[0].textureNativeStruct;
        Assert.Equal("new", bothReparsed.textureName);
        Assert.Equal("mask", bothReparsed.alphaName);
    }

    [Theory]
    [InlineData(TexturePlatformID.PCD3D8)]
    [InlineData(TexturePlatformID.Xbox)]
    public void PreservesNoncanonicalAlphaWordUntilEdited(TexturePlatformID platform)
    {
        var texture = new TextureNativeStruct_0001
        {
            platformType = platform,
            textureName = "a",
            alphaName = "",
            width = 4,
            height = 4,
            bitDepth = 32,
            mipMapCount = 1,
            mipMaps = new[] { new MipMapEntry(64, new byte[64]) }
        };
        byte[] source = MakeDictionary(texture);
        BitConverter.GetBytes(0x12345678u).CopyTo(source, 128);
        var sections = ReadFileMethods.ReadRenderWareFile(source);
        var parsed = ((TextureDictionary_0016)sections[0]).textureNativeList[0].textureNativeStruct;
        Assert.Equal(source, ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion));
        parsed.hasAlpha = false;
        byte[] edited = ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion);
        Assert.Equal(0u, BitConverter.ToUInt32(edited, 128));
    }

    [Fact]
    public void RecodesAndEditsD3D9Format()
    {
        var texture = new TextureNativeStruct_0001
        {
            platformType = TexturePlatformID.PCD3D9,
            textureName = "dxt",
            alphaName = "",
            d3dFormat = 0x31545844,
            width = 4,
            height = 4,
            bitDepth = 16,
            mipMapCount = 1,
            mipMaps = new[] { new MipMapEntry(8, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }) }
        };
        byte[] source = MakeDictionary(texture);
        var sections = ReadFileMethods.ReadRenderWareFile(source);
        var parsed = ((TextureDictionary_0016)sections[0]).textureNativeList[0].textureNativeStruct;
        Assert.Equal(source, ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion));
        parsed.d3dFormat = 0x35545844;
        byte[] edited = ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion);
        Assert.Equal(0x35545844u, BitConverter.ToUInt32(edited, 128));
    }

    [Theory]
    [InlineData(0x310)]
    [InlineData(0x350)]
    [InlineData(0x0400FFFF)]
    [InlineData(0x1003FFFF)]
    public void PreservesLegacyAndPackedVersionStamps(int stamp)
    {
        Assert.Equal(stamp, new RWVersion(stamp).Write());
        Assert.Equal(0, default(RWVersion).Write());
        Assert.Equal(0x310, new RWVersion(3, 1, 0, 0).Write());
        Assert.False(new RWVersion(unchecked((int)0xF0000000)).LegacyStamp);
    }

    [Fact]
    public void RecodesPackedVersion31Dictionary()
    {
        var texture = new TextureNativeStruct_0001
        {
            platformType = TexturePlatformID.PCD3D8,
            textureName = "a",
            alphaName = "",
            width = 1,
            height = 1,
            bitDepth = 32,
            mipMapCount = 1,
            mipMaps = new[] { new MipMapEntry(4, new byte[4]) }
        };
        var native = new TextureNative_0015
        {
            textureNativeStruct = texture,
            textureNativeExtension = new Extension_0003()
        };
        var version = new RWVersion(0x0400FFFF);
        byte[] source = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(native), version);
        var sections = ReadFileMethods.ReadRenderWareFile(source);
        Assert.Equal(source, ReadFileMethods.ExportRenderWareFile(sections, sections[0].renderWareVersion));
    }

    [Fact]
    public void TextureAssetPreservesDictionaryAndWritesNativeEdits()
    {
        var pluginBytes = new byte[12];
        BitConverter.GetBytes(4).CopyTo(pluginBytes, 0);
        using var pluginReader = new BinaryReader(new MemoryStream(pluginBytes));
        var dictionary = new TextureDictionary_0016(new TextureNative_0015
        {
            textureNativeStruct = MakePcTexture("original"),
            textureNativeExtension = new Extension_0003()
        }, new Extension_0003(new RWSection[] { new GenericSection(pluginReader, RenderWareFile.Section.SkyMipmapVal) }));
        dictionary.textureDictionaryStruct.deviceId = 6;
        byte[] source = ReadFileMethods.ExportRenderWareFile(dictionary, new RWVersion(3, 7, 0, 2));
        var header = new Section_AHDR("asset.RW3", AssetType.Texture, 0, source, Game.BFBB, Platform.Xbox);
        using var reader = new EndianBinaryReader(source, Platform.Xbox);
        var asset = new AssetRWTX(header, reader);

        Assert.True(reader.BaseStream.CanRead);
        Assert.Equal("PC", asset.GetPlatform());
        Assert.Equal(6, asset.DeviceId);
        Assert.Equal(source, asset.Serialize(Platform.Xbox.Endianness()));
        asset.FilterMode = TextureFilterMode.FILTERLINEAR;
        var sections = ReadFileMethods.ReadRenderWareFile(asset.Serialize(Platform.Xbox.Endianness()));
        var native = ((TextureDictionary_0016)sections[0]).textureNativeList[0];
        Assert.Equal(TextureFilterMode.FILTERLINEAR, native.textureNativeStruct.filterMode);
    }

    [Fact]
    public void TextureAssetRetainsMultipleNativeTextures()
    {
        var textures = new[] { "one", "two" }.Select(name => new TextureNative_0015
        {
            textureNativeStruct = MakePcTexture(name),
            textureNativeExtension = new Extension_0003()
        }).ToList();
        byte[] source = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(textures), new RWVersion(3, 5, 0, 0));
        var header = new Section_AHDR("asset", AssetType.Texture, 0, source, Game.BFBB, Platform.Xbox);
        using var reader = new EndianBinaryReader(source, Platform.Xbox);
        var asset = new AssetRWTX(header, reader);
        Assert.Equal(2, asset.Natives.Count);
        Assert.Equal(source, asset.Serialize(Platform.Xbox.Endianness()));
    }

    [Fact]
    public void ReplacingTextureDictionaryIsAtomic()
    {
        byte[] source = MakeDictionary(MakePcTexture("original"));
        var header = new Section_AHDR("asset", AssetType.Texture, 0, source, Game.BFBB, Platform.Xbox);
        using var reader = new EndianBinaryReader(source, Platform.Xbox);
        var asset = new AssetRWTX(header, reader);
        Assert.Throws<EndOfStreamException>(() => asset.ReplaceDictionary(new byte[] { 1 }));
        Assert.Equal(source, asset.Serialize(Platform.Xbox.Endianness()));
        byte[] replacement = MakeDictionary(MakePcTexture("replacement"));
        asset.ReplaceDictionary(replacement);
        Assert.Equal("replacement", asset.Native.textureNativeStruct.textureName);
        Assert.Equal(replacement, asset.Serialize(Platform.Xbox.Endianness()));
    }

    private static TextureNativeStruct_0001 MakePcTexture(string name) => new()
    {
        platformType = TexturePlatformID.PCD3D8,
        textureName = name,
        alphaName = "",
        width = 1,
        height = 1,
        bitDepth = 32,
        mipMapCount = 1,
        type = 4,
        rasterFormatFlags = TextureRasterFormat.RASTER_C8888,
        mipMaps = new[] { new MipMapEntry(4, new byte[] { 1, 2, 3, 255 }) }
    };

    private static byte[] MakeGifUpload(int width, int height, byte[] payload)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write(3UL | 1UL << 60);
        writer.Write(0xEUL);
        writer.Write(0UL);
        writer.Write(0x51UL);
        writer.Write((ulong)(uint)width | (ulong)(uint)height << 32);
        writer.Write(0x52UL);
        writer.Write(0UL);
        writer.Write(0x53UL);
        writer.Write((ulong)(uint)(payload.Length / 16) | 2UL << 58);
        writer.Write(0UL);
        writer.Write(payload);
        return stream.ToArray();
    }

    private static byte[] MakeDictionary(TextureNativeStruct_0001 texture) =>
        ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(new TextureNative_0015
        {
            textureNativeStruct = texture,
            textureNativeExtension = new Extension_0003()
        }), new RWVersion(3, 5, 0, 0));
}

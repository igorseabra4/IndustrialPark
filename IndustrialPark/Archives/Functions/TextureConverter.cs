using HipHopFile;
using IndustrialPark.Assets;
using RenderWareFile;
using RenderWareFile.Sections;
using RenderWareFile.Textures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using static IndustrialPark.Archives.TextureIOHelper;

namespace IndustrialPark.Archives;

public static class TextureConverter
{
    private readonly record struct NamedTexture(string Name, TextureNative_0015 Native);
    private readonly record struct DecodedTexture(int Width, int Height, byte[][] Mipmaps);

    public static void SetupTextureDisplay(IEnumerable<AssetRWTX> assets)
    {
        Program.TextureManager.BeginTextureUpdates();
        try
        {
            foreach (var rwtx in assets)
                EnableTextureForDisplay(rwtx);
        }
        finally
        {
            Program.TextureManager.EndTextureUpdates();
        }
    }

    public static void EnableTextureForDisplay(AssetRWTX RWTX)
    {
        lock (locker)
        {
            try
            {
                var texture = new NamedTexture(DisplayTextureName(RWTX.assetName, out _), RWTX.Native);
                var decoded = DecodeNative(texture.Native, true);
                if (!decoded.HasValue)
                    DecodeFallback([texture], true, (_, value) => decoded = value);
                if (!decoded.HasValue)
                    throw new InvalidDataException($"Unable to decode texture '{texture.Name}'.");
                Program.TextureManager.AddTextureMipmaps(texture.Name, texture.Native.textureNativeStruct,
                    decoded.Value.Width, decoded.Value.Height, decoded.Value.Mipmaps);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add {RWTX.assetName} to texture display: {ex.Message}");
            }
        }
    }

    private static string DisplayTextureName(string name, out bool isRw3)
    {
        isRw3 = name.EndsWith(".RW3");
        if (name.Length == 31 && !isRw3)
        {
            if (name.EndsWith(".RW"))
            {
                isRw3 = true;
                return name.Substring(0, name.Length - 3);
            }
            if (name.EndsWith(".R"))
            {
                isRw3 = true;
                return name.Substring(0, name.Length - 2);
            }
            if (name.EndsWith("."))
            {
                isRw3 = true;
                return name.Substring(0, name.Length - 1);
            }
        }
        return isRw3 ? name.Substring(0, name.Length - 4) : name;
    }
    
    public static void ExportTXD(IEnumerable<AssetRWTX> assets, string fileName)
    {
        List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

        int fileVersion = 0;

        foreach (var asset in assets)
            try
            {
                foreach (var native in asset.Natives)
                {
                    var texture = CloneTexture(native);
                    fileVersion = texture.renderWareVersion;
                    texture.textureNativeStruct.textureName = DisplayTextureName(asset.assetName, out bool isRw3);
                    if (isRw3)
                        texture.textureNativeStruct.textureName += ".RW3";
                    textNativeList.Add(texture);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to add {asset.assetName} to TXD archive: {ex.Message}");
            }

        TextureDictionary_0016 rws = new TextureDictionary_0016(textNativeList);

        rws.textureNativeList = rws.textureNativeList.OrderBy(f => f.textureNativeStruct.textureName).ToList();

        File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(rws, fileVersion));
    }

    public static List<AssetRWTX> ImportTXD(string fileName, Game game)
    {
        List<AssetRWTX> assets = new List<AssetRWTX>();

        foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(fileName))
        {
            if (rw is TextureDictionary_0016 td)
            {
                // For each texture in the dictionary...
                foreach (TextureNative_0015 tn in td.textureNativeList)
                {
                    string textureName = tn.textureNativeStruct.textureName;

                    // Create a new dictionary that has only that texture.
                    byte[] data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(tn), tn.renderWareVersion);

                    if (game == Game.Scooby)
                        FixTextureForScooby(ref data);

                    // And add the new dictionary as an asset.
                    assets.Add(new AssetRWTX(textureName, data));
                }
            }
        }
        return assets;
    }

    public static Bitmap GetTextureAsBitmap(TextureNative_0015 texture)
    {
        lock (locker)
        {
            var named = new NamedTexture(texture.textureNativeStruct.textureName, texture);
            var decoded = DecodeNative(texture, false);
            if (!decoded.HasValue)
                DecodeFallback([named], false, (_, value) => decoded = value);
            if (!decoded.HasValue)
                throw new InvalidDataException($"Unable to decode texture '{named.Name}'.");
            return CreateBitmap(decoded.Value);
        }
    }

    private static TextureNative_0015 CloneTexture(TextureNative_0015 texture) =>
        new TextureNative_0015(texture.GetBytes(texture.renderWareVersion));

    private static DecodedTexture? DecodeNative(TextureNative_0015 texture, bool allMipmaps)
    {
        if (allMipmaps)
        {
            var mipmaps = TextureDecoder.DecodeAll(texture, out int width, out int height);
            return mipmaps == null ? null : new DecodedTexture(width, height, mipmaps);
        }
        var pixels = TextureDecoder.DecodeBase(texture, out int baseWidth, out int baseHeight);
        return pixels == null ? null : new DecodedTexture(baseWidth, baseHeight, [pixels]);
    }

    private static void DecodeFallback(List<NamedTexture> fallback, bool allMipmaps, Action<string, DecodedTexture> receive)
    {
        foreach (var group in fallback.GroupBy(t => t.Native.renderWareVersion.Write()))
        {
            var originals = group.ToList();
            var copies = originals.Select((item, index) =>
            {
                var copy = CloneTexture(item.Native);
                copy.textureNativeStruct.textureName = $"ip_{index:X8}";
                return copy;
            }).ToList();
            byte[] txd = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(copies), group.Key);
            ExportTXDExternal(txd, converted =>
            {
                for (int i = 0; i < originals.Count; i++)
                {
                    string convertedName = copies[i].textureNativeStruct.textureName;
                    if (!converted.TryGetValue(convertedName, out var native))
                        throw new InvalidDataException($"External conversion did not produce texture '{originals[i].Name}'.");
                    var texture = DecodeNative(native, allMipmaps);
                    if (!texture.HasValue)
                        throw new InvalidDataException($"Unable to decode converted PC texture '{originals[i].Name}'.");
                    receive(originals[i].Name, texture.Value);
                }
            });
        }
    }

    private static Bitmap CreateBitmap(DecodedTexture texture)
    {
        int width = texture.Width, height = texture.Height;
        byte[] pixels = texture.Mipmaps[0];

        var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        try
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            try
            {
                for (int y = 0; y < height; y++)
                    Marshal.Copy(pixels, y * width * 4, IntPtr.Add(data.Scan0, y * data.Stride), width * 4);
            }
            finally { bitmap.UnlockBits(data); }
            return bitmap;
        }
        catch
        {
            bitmap.Dispose();
            throw;
        }
    }

    private static void ExportTXDExternal(byte[] txdFile, Action<Dictionary<string, TextureNative_0015>> receive)
    {
        lock (locker)
        {
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);

            var converted = new Dictionary<string, TextureNative_0015>();
            try
            {
                File.Delete(pathToPcTXD);
                File.WriteAllBytes(pathToGcTXD, txdFile);
                PerformTXDDecodingExternal();
                if (!File.Exists(pathToPcTXD))
                    throw new InvalidDataException("External texture conversion did not produce an output dictionary.");
                foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(pathToPcTXD))
                    if (rw is TextureDictionary_0016 td)
                        foreach (TextureNative_0015 tn in td.textureNativeList)
                        {
                            converted.TryAdd(tn.textureNativeStruct.textureName, tn);
                        }
                receive(converted);
            }
            finally
            {
                File.Delete(pathToGcTXD);
                File.Delete(pathToPcTXD);
            }
        }
    }

    public static List<(string, Bitmap)> GetTexturesAsBitmaps(IEnumerable<AssetRWTX> assets)
    {
        var textures = assets.Select(asset => new NamedTexture(asset.assetName, asset.Native)).ToList();
        return CreateBitmaps(textures).Select(b => (b.Key, b.Value)).ToList();
    }

    public static Dictionary<string, Bitmap> GetTexturesAsBitmaps(Archive archive, string[] textureNames)
    {
        return CreateBitmaps(FindTextures(archive, textureNames));
    }

    private static List<NamedTexture> FindTextures(Archive archive, string[] textureNames)
    {
        List<NamedTexture> textures = new List<NamedTexture>();

        foreach (string t in textureNames)
        {
            AssetRWTX RWTX;

            uint assetID = new AssetID(t + ".RW3");
            if (archive.TryGetAsset(assetID, out Asset asset) && asset is AssetRWTX rWTX1)
                RWTX = rWTX1;
            else
            {
                assetID = new AssetID(t);
                if (archive.TryGetAsset(assetID, out asset) && asset is AssetRWTX rWTX2)
                    RWTX = rWTX2;
                else
                    continue;
            }

            textures.Add(new NamedTexture(t, RWTX.Native));
        }
        return textures;
    }

    private static Dictionary<string, Bitmap> CreateBitmaps(List<NamedTexture> textures)
    {
        lock (locker)
        {
            var result = new Dictionary<string, Bitmap>();
            try
            {
                var names = new HashSet<string>();
                var fallback = new List<NamedTexture>();
                foreach (var texture in textures)
                {
                    if (!names.Add(texture.Name))
                        continue;
                    var decoded = DecodeNative(texture.Native, false);
                    if (decoded.HasValue)
                        result.Add(texture.Name, CreateBitmap(decoded.Value));
                    else
                        fallback.Add(texture);
                }
                DecodeFallback(fallback, false, (name, texture) => result.Add(name, CreateBitmap(texture)));
                return result;
            }
            catch
            {
                foreach (var bitmap in result.Values)
                    bitmap.Dispose();
                throw;
            }
        }
    }

    public static void ExportTexturesAsPng(IEnumerable<AssetRWTX> assets, string folder) =>
        ExportTexturesAsPng(assets.Select(asset => new NamedTexture(asset.assetName, asset.Native)), folder);

    public static void ExportTexturesAsPng(IEnumerable<Archive> archives, string[] textureNames, string folder)
    {
        var textures = new List<NamedTexture>();
        foreach (var archive in archives)
            foreach (var texture in FindTextures(archive, textureNames))
                textures.Add(texture);
        ExportTexturesAsPng(textures, folder);
    }

    private static void ExportTexturesAsPng(IEnumerable<NamedTexture> textures, string folder)
    {
        lock (locker)
        {
            var names = new HashSet<string>();
            var fallback = new List<NamedTexture>();
            foreach (var texture in textures)
            {
                if (!names.Add(texture.Name))
                    continue;
                var decoded = DecodeNative(texture.Native, false);
                if (decoded.HasValue)
                    SavePng(texture.Name, decoded.Value, folder);
                else
                    fallback.Add(texture);
            }

            DecodeFallback(fallback, false, (name, texture) => SavePng(name, texture, folder));
        }
    }

    private static void SavePng(string name, DecodedTexture texture, string folder)
    {
        using var bitmap = CreateBitmap(texture);
        bitmap.Save(Path.Combine(folder, name + ".png"), ImageFormat.Png);
    }
}

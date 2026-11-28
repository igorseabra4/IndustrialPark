using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static HipHopFile.Functions;

namespace IndustrialPark.Archives;

public static class TextureIOHelper
{
    public static readonly object locker = new object();

    public static string txdGenFolder => Path.Combine(Program.ResourcesFolder, "txdgen_1.0");
    public static string tempGcTxdsDir => Path.Combine(txdGenFolder, "Temp", "txds_gc");
    public static string tempPcTxdsDir => Path.Combine(txdGenFolder, "Temp", "txds_pc");
    public static string pathToGcTXD => Path.Combine(tempGcTxdsDir, "temp.txd");
    public static string pathToPcTXD => Path.Combine(tempPcTxdsDir, "temp.txd");

    public static void PerformTXDConversionExternal(bool compress = false, bool generateMipmaps = false, string custom_output = null) =>
        PerformTXDConversionExternalCore(compress, generateMipmaps, custom_output, 300000);

    internal static void PerformTXDDecodingExternal() =>
        PerformTXDConversionExternalCore(false, false, null, 60000);

    private static void PerformTXDConversionExternalCore(bool compress, bool generateMipmaps, string custom_output,
        int waitMilliseconds)
    {
        lock (locker)
        {
            if (!converterInitialized || converterStarted)
                initConverter();

            string ini =
                "[Main]\r\n" +
                (custom_output ??
                $"gameRoot={tempGcTxdsDir}\r\n" +
                $"outputRoot={tempPcTxdsDir}\r\n" +
                "targetVersion=VC\r\n" +
                "targetPlatform=PC\r\n"
                ) +
                "clearMipmaps=false\r\n" +
                $"generateMipmaps={generateMipmaps.ToString().ToLower()}\r\n" +
                "mipGenMode=default\r\n" +
                "mipGenMaxLevel=10\r\n" +
                "improveFiltering=true\r\n" +
                $"compressTextures={compress.ToString().ToLower()}\r\n" +
                "compressionQuality=1.0\r\n" +
                "palRuntimeType=PNGQUANT\r\n" +
                "dxtRuntimeType=SQUISH\r\n" +
                "warningLevel=0\r\n" +
                "ignoreSecureWarnings=true\r\n" +
                "reconstructIMGArchives=false\r\n" +
                "fixIncompatibleRasters=true\r\n" +
                "dxtPackedDecompression=false\r\n" +
                "imgArchivesCompressed=false\r\n" +
                "ignoreSerializationRegions=true";

            File.WriteAllText(Path.Combine(txdGenFolder, "txdgen.ini"), ini);

            var errors = new StringBuilder();
            DataReceivedEventHandler onError = (_, e) =>
            {
                if (e.Data != null && errors.Length < 8192)
                    errors.AppendLine(e.Data.Substring(0, System.Math.Min(e.Data.Length, 8192 - errors.Length)));
            };
            txdgenProcess.ErrorDataReceived += onError;
            try
            {
                txdgenProcess.Start();
                converterStarted = true;
                txdgenProcess.BeginOutputReadLine();
                txdgenProcess.BeginErrorReadLine();
                if (!txdgenProcess.WaitForExit(waitMilliseconds))
                {
                    if (!txdgenProcess.HasExited)
                    {
                        try { txdgenProcess.Kill(); }
                        catch (System.InvalidOperationException) when (txdgenProcess.HasExited) { }
                    }
                    txdgenProcess.WaitForExit();
                    throw new System.TimeoutException("Texture conversion timed out.");
                }
                txdgenProcess.WaitForExit();
                if (txdgenProcess.ExitCode != 0)
                    throw new IOException($"Texture conversion failed with exit code {txdgenProcess.ExitCode}: {errors}");
            }
            catch
            {
                if (converterStarted && !txdgenProcess.HasExited)
                {
                    try { txdgenProcess.Kill(); }
                    catch (System.InvalidOperationException) when (txdgenProcess.HasExited) { }
                    txdgenProcess.WaitForExit();
                }
                throw;
            }
            finally
            {
                txdgenProcess.ErrorDataReceived -= onError;
            }
        }
    }

    private static Process txdgenProcess;
    private static bool converterInitialized = false;
    private static bool converterStarted = false;

    public static void initConverter()
    {
        txdgenProcess?.Dispose();
        txdgenProcess = new Process();
        txdgenProcess.StartInfo.FileName = Path.Combine(txdGenFolder, "txdgen.exe");
        txdgenProcess.StartInfo.WorkingDirectory = txdGenFolder;
        txdgenProcess.StartInfo.CreateNoWindow = true;
        txdgenProcess.StartInfo.RedirectStandardOutput = true;
        txdgenProcess.StartInfo.RedirectStandardError = true;
        txdgenProcess.StartInfo.UseShellExecute = false;
        txdgenProcess.EnableRaisingEvents = true;
        converterInitialized = true;
        converterStarted = false;
    }

    public static void closeConverter()
    {
        if (converterInitialized && converterStarted && txdgenProcess != null && !txdgenProcess.HasExited)
            txdgenProcess.Kill();
    }

    public static void CheckTXDConversionExternal()
    {
        if (!txdgenProcess.HasExited)
        {
            txdgenProcess.Kill();
            txdgenProcess.WaitForExit();
            throw new System.TimeoutException("Texture conversion timed out.");
        }
        if (txdgenProcess.ExitCode != 0)
            throw new System.IO.IOException($"Texture conversion failed with exit code {txdgenProcess.ExitCode}.");
    }

    public static RWVersion currentTextureVersion(Game game, Platform platform)
    {
        switch (game)
        {
            case Game.Scooby when platform == Platform.PS2:
                return (3, 1, 0, 0);
            case Game.Scooby when platform == Platform.GameCube:
                return (3, 3, 0, 2);
            case Game.BFBB when platform == Platform.GameCube:
                return (3, 4, 0, 3);
            case Game.Scooby when platform == Platform.Xbox:
                return (3, 4, 0, 5);
            case Game.BFBB:
            case Game.Incredibles when platform == Platform.PS2:
                return (3, 5, 0, 0);
            default:
                return (3, 7, 0, 2, 0xA);
        }
    }

    public static void ExportSingleTextureToRWTEX(TextureNative_0015 texture, string fileName) =>
        File.WriteAllBytes(fileName, texture.GetBytes(texture.renderWareVersion));

    public static Section_AHDR CreateRWTXFromBitmap(Game game, Platform platform, string fileName, bool appendRW3, bool flip, bool mipmaps, bool compress, bool transFix)
    {
        return CreateRWTXsFromBitmaps(game, platform, new List<string>() { fileName }, appendRW3, flip, mipmaps, compress, transFix)[0];
    }

    public static List<Section_AHDR> CreateRWTXsFromBitmaps
        (Game game, Platform platform, List<string> fileNames, bool appendRW3, bool flip, bool mipmaps, bool compress, bool transFix)
    {
        if (fileNames.Count == 0)
            return new List<Section_AHDR>();
        lock (locker)
        {
            if (transFix)
                compress = false;

            List<TextureNative_0015> textureNativeList = new List<TextureNative_0015>();

            foreach (string fileName in fileNames)
            {
                string textureName = Path.GetFileNameWithoutExtension(fileName);
                Bitmap bitmap = new Bitmap(fileName);

                List<byte> bitmapData = new List<byte>(bitmap.Width * bitmap.Height * 4);

                if (flip)
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

                for (int j = 0; j < bitmap.Height; j++)
                    for (int i = 0; i < bitmap.Width; i++)
                    {
                        bitmapData.Add(bitmap.GetPixel(i, j).B);
                        bitmapData.Add(bitmap.GetPixel(i, j).G);
                        bitmapData.Add(bitmap.GetPixel(i, j).R);
                        bitmapData.Add(bitmap.GetPixel(i, j).A);
                    }

                textureNativeList.Add(
                    new TextureNative_0015()
                    {
                        textureNativeStruct = new TextureNativeStruct_0001()
                        {
                            textureName = textureName + (appendRW3 && game != Game.Scooby ? ".RW3" : ""),
                            alphaName = "",
                            height = (short)bitmap.Height,
                            width = (short)bitmap.Width,
                            mipMapCount = 1,
                            addressModeU = TextureAddressMode.TEXTUREADDRESSWRAP,
                            addressModeV = TextureAddressMode.TEXTUREADDRESSWRAP,
                            filterMode = TextureFilterMode.FILTERLINEAR,
                            bitDepth = 32,
                            platformType = TexturePlatformID.PCD3D8,
                            compression = 0,
                            hasAlpha = false,
                            rasterFormatFlags = TextureRasterFormat.RASTER_C8888,
                            type = 4,
                            mipMaps = new MipMapEntry[] { new MipMapEntry(bitmapData.Count, bitmapData.ToArray()) },
                        },
                        textureNativeExtension = new Extension_0003()
                    }
                );

                bitmap.Dispose();
            }

            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);

            var platformText = (platform == Platform.GameCube ? "Gamecube" :
                platform == Platform.Xbox ? "XBOX" :
                platform == Platform.PS2 ? "PS2" : "PC");

            try
            {
                File.Delete(pathToGcTXD);
                File.WriteAllBytes(pathToPcTXD, ReadFileMethods.ExportRenderWareFile(
                    new TextureDictionary_0016(textureNativeList), currentTextureVersion(game, platform)));
                PerformTXDConversionExternal(compress, mipmaps,
                    $"gameRoot={tempPcTxdsDir}\r\n" +
                    $"outputRoot={tempGcTxdsDir}\r\n" +
                    "targetVersion=VC\r\n" +
                    $"targetPlatform={platformText}\r\n");

                if (!File.Exists(pathToGcTXD))
                    throw new InvalidDataException("External texture conversion did not produce an output dictionary.");

                List<Section_AHDR> AHDRs = new List<Section_AHDR>();

                foreach (TextureNative_0015 texture in ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(pathToGcTXD)[0]).textureNativeList)
                {
                    string sanitizedTexture = game == Game.Scooby && appendRW3 ? texture.textureNativeStruct.textureName + ".RW3" : texture.textureNativeStruct.textureName;
                    AHDRs.Add(new Section_AHDR(BKDRHash(sanitizedTexture), AssetType.Texture, AssetService.AHDRFlagsFromAssetType(AssetType.Texture),
                        new Section_ADBG(0, sanitizedTexture, "", 0),
                        ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016(texture), currentTextureVersion(game, platform)), game, platform));
                }

                // fix for apparent transparency issue
                if (transFix && platform == Platform.GameCube)
                    foreach (var AHDR in AHDRs)
                        AHDR.data[0x9B] = 0x01;

                return AHDRs;
            }
            finally
            {
                File.Delete(pathToGcTXD);
                File.Delete(pathToPcTXD);
            }
        }
    }

    public static void FixTextureForScooby(ref byte[] data)
    {
        for (int j = 0x6D; j < 0x8C; j++)
            data[j] = 0xCD;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private static string txdGenFolder => Application.StartupPath + "\\Resources\\txdgen_1.0\\";
        private static string tempGcTxdsDir => txdGenFolder + "Temp\\txds_gc\\";
        private static string tempPcTxdsDir => txdGenFolder + "Temp\\txds_pc\\";
        private static string pathToGcTXD => tempGcTxdsDir + "temp.txd";
        private static string pathToPcTXD => tempPcTxdsDir + "temp.txd";

        public void SetupTextureDisplay()
        {
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);

            ExportTextureDictionary(pathToGcTXD, true);

            PerformTXDConversionExternal();

            TextureManager.LoadTexturesFromTXD(pathToPcTXD, false);
            TextureManager.ReapplyTextures();

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);
        }

        public void EnableTextureForDisplay(AssetRWTX RWTX)
        {
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);

            ExportSingleTextureToDictionary(pathToGcTXD, RWTX.Data, RWTX.AHDR.ADBG.assetName);

            PerformTXDConversionExternal();

            TextureManager.LoadTexturesFromTXD(pathToPcTXD);

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);
        }

        private static void PerformTXDConversionExternal(bool toPC = true, bool compress = false, bool generateMipmaps = false, string custom_output = null)
        {
            string ini =
                "[Main]\r\n" +
                (custom_output ?? (toPC ?
                "gameRoot=" + tempGcTxdsDir + "\r\n" +
                "outputRoot=" + tempPcTxdsDir + "\r\n" +
                "targetVersion=VC\r\n" +
                "targetPlatform=PC\r\n"
                :
                "gameRoot=" + tempPcTxdsDir + "\r\n" +
                "outputRoot=" + tempGcTxdsDir + "\r\n" +
                "targetVersion=VC\r\n" +
                "targetPlatform=" +
                (currentPlatform == Platform.GameCube ? "Gamecube" :
                currentPlatform == Platform.Xbox ? "XBOX" :
                currentPlatform == Platform.PS2 ? "PS2" : "PC")
                + "\r\n")) +
                "clearMipmaps=false\r\n" +
                "generateMipmaps=" + generateMipmaps.ToString().ToLower() + "\r\n" +
                "mipGenMode=default\r\n" +
                "mipGenMaxLevel=10\r\n" +
                "improveFiltering=true\r\n" +
                "compressTextures=" + compress.ToString().ToLower() + "\r\n" +
                "compressionQuality=1.0\r\n" +
                "palRuntimeType=PNGQUANT\r\n" +
                "dxtRuntimeType=SQUISH\r\n" +
                "warningLevel=1\r\n" +
                "ignoreSecureWarnings=true\r\n" +
                "reconstructIMGArchives=false\r\n" +
                "fixIncompatibleRasters=true\r\n" +
                "dxtPackedDecompression=false\r\n" +
                "imgArchivesCompressed=false\r\n" +
                "ignoreSerializationRegions=true";

            string curr = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(txdGenFolder);

            File.WriteAllText("txdgen.ini", ini);

            System.Diagnostics.Process.Start("txdgen.exe").WaitForExit();

            Directory.SetCurrentDirectory(curr);
        }

        public static int scoobyTextureVersion => 0x0C02FFFF;
        public static int bfbbTextureVersion => 0x1003FFFF;
        public static int tssmTextureVersion => 0x1C02000A;

        public static int currentTextureVersion
        {
            get
            {
                if (currentGame == Game.Scooby)
                    return scoobyTextureVersion;
                if (currentGame == Game.BFBB)
                    return bfbbTextureVersion; // VC
                if (currentGame == Game.Incredibles)
                    return tssmTextureVersion; // Bully
                return 0;
            }
        }

        public static void ExportSingleTextureToRWTEX(byte[] data, string fileName)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(data))
                if (rw is TextureDictionary_0016 td)
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                        File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(tn, tn.renderWareVersion));

            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public static void ExportSingleTextureToDictionary(string fileName, byte[] data, string textureName)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(data))
                if (rw is TextureDictionary_0016 td)
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        fileVersion = tn.renderWareVersion;
                        tn.textureNativeStruct.textureName = textureName.Replace(".RW3", "");
                        textNativeList.Add(tn);
                    }

            TextureDictionary_0016 rws = new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001()
                {
                    textureCount = (short)textNativeList.Count(),
                    unknown = 0
                },
                textureNativeList = textNativeList,
                textureDictionaryExtension = new Extension_0003()
                {
                    extensionSectionList = new List<RWSection>()
                }
            };

            rws.textureNativeList = rws.textureNativeList.OrderBy(f => f.textureNativeStruct.textureName).ToList();

            File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(rws, fileVersion));

            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public void ExportTextureDictionary(string fileName, bool RW3)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetRWTX RWTX)
                    if ((RW3 && RWTX.AHDR.ADBG.assetName.Contains(".RW3")) || (!RW3 && !RWTX.AHDR.ADBG.assetName.Contains(".RW3")))
                        foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(RWTX.Data))
                            if (rw is TextureDictionary_0016 td)
                                foreach (TextureNative_0015 tn in td.textureNativeList)
                                {
                                    fileVersion = tn.renderWareVersion;
                                    tn.textureNativeStruct.textureName = RWTX.AHDR.ADBG.assetName.Replace(".RW3", "");
                                    textNativeList.Add(tn);
                                }

            TextureDictionary_0016 rws = new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001()
                {
                    textureCount = (short)textNativeList.Count(),
                    unknown = 0
                },
                textureNativeList = textNativeList,
                textureDictionaryExtension = new Extension_0003()
                {
                    extensionSectionList = new List<RWSection>()
                }
            };

            rws.textureNativeList = rws.textureNativeList.OrderBy(f => f.textureNativeStruct.textureName).ToList();

            File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(rws, fileVersion));

            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public void ImportTextureDictionary(string fileName, bool RW3)
        {
            UnsavedChanges = true;
            int layerIndex = 0;

            List<Section_LHDR> LHDRs = new List<Section_LHDR>
            {
                new Section_LHDR()
                {
                    layerType = 1,
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                }
            };
            LHDRs.AddRange(DICT.LTOC.LHDRList);
            DICT.LTOC.LHDRList = LHDRs;

            ReadFileMethods.treatStuffAsByteArray = true;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(fileName))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        string textureName = tn.textureNativeStruct.textureName;
                        if (RW3 && !textureName.Contains(".RW3"))
                            textureName += ".RW3";

                        // Create a new dictionary that has only that texture.
                        byte[] data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                        {
                            textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                            textureDictionaryExtension = new Extension_0003(),
                            textureNativeList = new List<TextureNative_0015>() { tn }
                        }, tn.renderWareVersion);
                        
                        // And add the new dictionary as an asset.
                        Section_ADBG ADBG = new Section_ADBG(0, textureName, "", 0);
                        Section_AHDR AHDR = new Section_AHDR(BKDRHash(textureName), AssetType.RWTX, AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM, ADBG, data);

                        if (ContainsAsset(AHDR.assetID))
                            RemoveAsset(AHDR.assetID);

                        AddAsset(layerIndex, AHDR);
                    }
                }
            }
            
            ReadFileMethods.treatStuffAsByteArray = false;
        }

        public static Section_AHDR CreateRWTXFromBitmap(string fileName, bool appendRW3, bool flip, bool mipmaps, bool compress)
        {
            return CreateRWTXsFromBitmaps(new List<string>() { fileName }, appendRW3, flip, mipmaps, compress)[0];
        }

        public static List<Section_AHDR> CreateRWTXsFromBitmaps(List<string> fileNames, bool appendRW3, bool flip, bool mipmaps, bool compress)
        {
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
                            textureName = textureName + (appendRW3 ? ".RW3" : ""),
                            alphaName = "",
                            height = (short)bitmap.Height,
                            width = (short)bitmap.Width,
                            mipMapCount = 1,
                            addressModeU = TextureAddressMode.TEXTUREADDRESSWRAP,
                            addressModeV = TextureAddressMode.TEXTUREADDRESSWRAP,
                            filterMode = TextureFilterMode.FILTERLINEAR,
                            bitDepth = 32,
                            platformType = 8,
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

            File.WriteAllBytes(pathToPcTXD, ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = (short)textureNativeList.Count, unknown = 0 },
                textureNativeList = textureNativeList,
                textureDictionaryExtension = new Extension_0003()
            }, currentTextureVersion));

            PerformTXDConversionExternal(false, compress, mipmaps);
            
            ReadFileMethods.treatStuffAsByteArray = true;

            List<Section_AHDR> AHDRs = new List<Section_AHDR>();

            foreach (TextureNative_0015 texture in ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(pathToGcTXD)[0]).textureNativeList)
                AHDRs.Add(new Section_AHDR(BKDRHash(texture.textureNativeStruct.textureName), AssetType.RWTX, AHDRFlagsFromAssetType(AssetType.RWTX),
                    new Section_ADBG(0, texture.textureNativeStruct.textureName, "", 0), 
                    ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                    {
                        textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                        textureNativeList = new List<TextureNative_0015>() { texture },
                        textureDictionaryExtension = new Extension_0003()
                    }, currentTextureVersion)));
            
            ReadFileMethods.treatStuffAsByteArray = false;

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);

            return AHDRs;
        }

        public static Dictionary<string, Bitmap> ExportRWTXToBitmap(byte[] txdFile)
        {
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);

            File.WriteAllBytes(pathToPcTXD, txdFile);

            PerformTXDConversionExternal(false, false, false,
            "gameRoot=" + tempPcTxdsDir + "\r\n" +
            "outputRoot=" + tempGcTxdsDir + "\r\n" +
            "targetVersion=VC\r\n" +
            "targetPlatform=uncompressed_mobile\r\n"
            );

            PerformTXDConversionExternal();

            Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(pathToPcTXD))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        List<System.Drawing.Color> bitmapData = new List<System.Drawing.Color>();

                        byte[] imageData = tn.textureNativeStruct.mipMaps[0].data;
                       
                        //if (tn.textureNativeStruct.compression == 0)
                        if (tn.textureNativeStruct.mipMaps[0].dataSize == tn.textureNativeStruct.width * tn.textureNativeStruct.height * 4)
                        {
                            for (int i = 0; i < imageData.Length; i += 4)
                                bitmapData.Add(System.Drawing.Color.FromArgb(
                                    imageData[i + 3],
                                    imageData[i + 2],
                                    imageData[i + 1],
                                    imageData[i]));
                        }
                        else
                        {
                            for (int i = 0; i < imageData.Length; i += 2)
                            {
                                short value = BitConverter.ToInt16(imageData, i);
                                byte R = (byte)((value >> 11) & 0x1F);
                                byte G = (byte)((value >> 5) & 0x3F);
                                byte B = (byte)((value) & 0x1F);

                                System.Drawing.Color color = System.Drawing.Color.FromArgb(0xFF,
                                    (R << 3) | (R >> 2),
                                    (G << 2) | (G >> 4),
                                    (B << 3) | (B >> 2));

                                bitmapData.Add(color);
                            }
                        }

                        Bitmap b = new Bitmap(tn.textureNativeStruct.width, tn.textureNativeStruct.height);

                        int k = 0;
                        for (int j = 0; j < b.Height; j++)
                            for (int i = 0; i < b.Width; i++)
                            {
                                int v = k;
                                int v2 = bitmapData.Count;
                                System.Drawing.Color c = bitmapData[k];
                                b.SetPixel(i, j, c);
                                k++;
                            }

                        if (!bitmaps.ContainsKey(tn.textureNativeStruct.textureName))
                            bitmaps.Add(tn.textureNativeStruct.textureName, b);
                    }
                }
            }

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);

            return bitmaps;
        }

        public Dictionary<string, Bitmap> GetTexturesAsBitmaps(string[] textureNames)
        {
            List<TextureNative_0015> textures = new List<TextureNative_0015>();

            foreach (string t in textureNames)
            {
                AssetRWTX RWTX;

                uint assetID = BKDRHash(t + ".RW3");
                if (ContainsAsset(assetID))
                    RWTX = (AssetRWTX)GetFromAssetID(assetID);
                else
                {
                    assetID = BKDRHash(t);
                    if (ContainsAsset(assetID))
                        RWTX = (AssetRWTX)GetFromAssetID(assetID);
                    else continue;
                }

                foreach (TextureNative_0015 texture in ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(RWTX.Data)[0]).textureNativeList)
                {
                    texture.textureNativeStruct.textureName = t;
                    textures.Add(texture);
                }
            }

            return ExportRWTXToBitmap(ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = (short)textures.Count, unknown = 0 },
                textureNativeList = textures,
                textureDictionaryExtension = new Extension_0003()
            }, currentTextureVersion));
        }
    }
}
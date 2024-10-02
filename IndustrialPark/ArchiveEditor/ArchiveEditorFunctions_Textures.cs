using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static HipHopFile.Functions;
using static IndustrialPark.TextureIOHelper;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public bool SkipTextureDisplay = false;

        public void SetupTextureDisplay()
        {
            if (standalone || SkipTextureDisplay || !ContainsAssetWithType(AssetType.Texture))
                return;

            lock (locker)
            {
                if (!Directory.Exists(tempGcTxdsDir))
                    Directory.CreateDirectory(tempGcTxdsDir);
                if (!Directory.Exists(tempPcTxdsDir))
                    Directory.CreateDirectory(tempPcTxdsDir);

                try
                {
                    ExportTextureDictionary(pathToGcTXD, CheckState.Indeterminate);
                    PerformTXDConversionExternal(platform);
                    TextureManager.LoadTexturesFromTXD(pathToPcTXD);

                    if (File.Exists(pathToGcTXD))
                        File.Delete(pathToGcTXD);
                    if (File.Exists(pathToPcTXD))
                        File.Delete(pathToPcTXD);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to setup texture display: " + ex.Message);
                }
            }
        }

        public void EnableTextureForDisplay(AssetRWTX RWTX)
        {
            lock (locker)
            {
                if (!Directory.Exists(tempGcTxdsDir))
                    Directory.CreateDirectory(tempGcTxdsDir);
                if (!Directory.Exists(tempPcTxdsDir))
                    Directory.CreateDirectory(tempPcTxdsDir);

                ExportSingleTextureToDictionary(pathToGcTXD, RWTX.Data, RWTX.assetName);

                PerformTXDConversionExternal(platform);

                TextureManager.LoadTexturesFromTXD(pathToPcTXD);

                if (File.Exists(pathToGcTXD))
                    File.Delete(pathToGcTXD);
                if (File.Exists(pathToPcTXD))
                    File.Delete(pathToPcTXD);
            }
        }

        public void ExportTextureDictionary(string fileName, CheckState RW3)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (var asset in GetAllAssets().OfType<AssetRWTX>())
                if ((RW3 == CheckState.Indeterminate) || ((RW3 == CheckState.Checked) && asset.assetName.Contains(".RW3")) || ((RW3 == CheckState.Unchecked) && !asset.assetName.Contains(".RW3")))
                    try
                    {
                        foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(asset.Data))
                            if (rw is TextureDictionary_0016 td)
                                foreach (TextureNative_0015 tn in td.textureNativeList)
                                {
                                    fileVersion = tn.renderWareVersion;

                                    var name = asset.assetName;
                                    // Fix for trimmed texture file names
                                    // For example, track textures in SpongeBall challenges
                                    // Length indicates that the file may have been trimmed (TODO: check if trim length is always 31?)
                                    int trimLength = 31;

                                    if (name.Length == trimLength && !name.EndsWith(".RW3"))
                                    {
                                        if (name.EndsWith(".RW"))
                                            tn.textureNativeStruct.textureName = name.Substring(0, name.Length - 3);
                                        else if (asset.assetName.EndsWith(".R"))
                                            tn.textureNativeStruct.textureName = name.Substring(0, name.Length - 2);
                                        else if (asset.assetName.EndsWith("."))
                                            tn.textureNativeStruct.textureName = name.Substring(0, name.Length - 1);
                                    }
                                    else if (name.EndsWith(".RW3"))
                                        tn.textureNativeStruct.textureName = name.Substring(0, name.Length - 4);
                                    else
                                        tn.textureNativeStruct.textureName = name;

                                    textNativeList.Add(tn);
                                }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to add RWTX asset {GetFromAssetID(asset.assetID)} to TXD archive: {ex.Message}");
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
            List<Section_AHDR> AHDRs = GetAssetsFromTextureDictionary(fileName, RW3);
            ImportMultipleAssets(AHDRs, true);
        }

        public List<Section_AHDR> GetAssetsFromTextureDictionary(string fileName, bool RW3)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<Section_AHDR> AHDRs = new List<Section_AHDR>();

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

                        if (game == Game.Scooby)
                            FixTextureForScooby(ref data);

                        // And add the new dictionary as an asset.
                        Section_ADBG ADBG = new Section_ADBG(0, textureName, "", 0);
                        Section_AHDR AHDR = new Section_AHDR(BKDRHash(textureName), AssetType.Texture, AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM, ADBG, data);


                        AHDRs.Add(AHDR);
                    }
                }
            }
            ReadFileMethods.treatStuffAsByteArray = false;

            return AHDRs;
        }

        public Dictionary<string, Bitmap> ExportTXDToBitmap(byte[] txdFile, bool hasTransparency = false)
        {
            lock (locker)
            {
                if (!Directory.Exists(tempPcTxdsDir))
                    Directory.CreateDirectory(tempPcTxdsDir);
                if (!Directory.Exists(tempGcTxdsDir))
                    Directory.CreateDirectory(tempGcTxdsDir);

                File.WriteAllBytes(pathToPcTXD, txdFile);

                PerformTXDConversionExternal(platform, false, false, false,
                "gameRoot=" + tempPcTxdsDir + "\r\n" +
                "outputRoot=" + tempGcTxdsDir + "\r\n" +
                "targetVersion=VC\r\n" +
                (hasTransparency ?
                "targetPlatform=PC\r\n" :
                "targetPlatform=uncompressed_mobile\r\n")
                );

                PerformTXDConversionExternal(platform);

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
                            Console.WriteLine(tn.textureNativeStruct.mipMaps[0].dataSize);

                            if (tn.textureNativeStruct.hasAlpha)
                            {
                                if ((tn.textureNativeStruct.rasterFormatFlags & TextureRasterFormat.RASTER_C4444) != 0)
                                {
                                    for (int i = 0; i < imageData.Length; i += 2)
                                    {
                                        short value = BitConverter.ToInt16(imageData, i);

                                        byte B = (byte)(value & 0x0F);
                                        byte G = (byte)((value >> 4) & 0x0F);
                                        byte R = (byte)((value >> 8) & 0x0F);
                                        byte A = (byte)((value >> 12) & 0x0F);

                                        B = (byte)(B * 0x11);
                                        G = (byte)(G * 0x11);
                                        R = (byte)(R * 0x11);
                                        A = (byte)(A * 0x11);

                                        bitmapData.Add(System.Drawing.Color.FromArgb(A, R, G, B));
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < imageData.Length; i += 4)
                                        bitmapData.Add(System.Drawing.Color.FromArgb(
                                            imageData[i + 3],
                                            imageData[i + 2],
                                            imageData[i + 1],
                                            imageData[i]));
                                }
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
        }

        public Dictionary<string, Bitmap> GetTexturesAsBitmaps(string[] textureNames)
        {
            List<TextureNative_0015> textures = new List<TextureNative_0015>();

            ReadFileMethods.treatStuffAsByteArray = true;

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
                    else
                        continue;
                }

                foreach (TextureNative_0015 texture in ((TextureDictionary_0016)ReadFileMethods.ReadRenderWareFile(RWTX.Data)[0]).textureNativeList)
                {
                    texture.textureNativeStruct.textureName = t;
                    textures.Add(texture);
                }
            }

            return ExportTXDToBitmap(ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = (short)textures.Count, unknown = 0 },
                textureNativeList = textures,
                textureDictionaryExtension = new Extension_0003()
            }, currentTextureVersion(game, platform)));
        }

        public List<(string, Bitmap)> GetAllTexturesAsBitmaps()
        {
            List<TextureNative_0015> textures = new List<TextureNative_0015>();

            ReadFileMethods.treatStuffAsByteArray = true;

            foreach (AssetRWTX RWTX in GetAllAssets().OfType<AssetRWTX>())
            {
                var data = RWTX.Data;
                
                // Some RWTX files seem to have no data, causing a crash when exporting as PNG
                // e.g. bubble_wand.RW3 in JF01.hop
                if (data.Length == 0)
                    continue;
                
                var textureDictionary = ReadFileMethods.ReadRenderWareFile(data)[0] as TextureDictionary_0016;
                var texNativeList = textureDictionary.textureNativeList;
                
                foreach (TextureNative_0015 texture in texNativeList)
                {
                    texture.textureNativeStruct.textureName = RWTX.assetName;
                    textures.Add(texture);
                }
            }

            return ExportTXDToBitmap(ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = (short)textures.Count, unknown = 0 },
                textureNativeList = textures,
                textureDictionaryExtension = new Extension_0003()
            }, currentTextureVersion(game, platform))).Select(b => (b.Key, b.Value)).ToList();
        }
    }
}
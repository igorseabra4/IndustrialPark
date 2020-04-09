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
using static IndustrialPark.TextureIOHelper;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public void SetupTextureDisplay()
        {
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);

            try
            {
                ExportTextureDictionary(pathToGcTXD, true);

                PerformTXDConversionExternal(platform);

                TextureManager.LoadTexturesFromTXD(pathToPcTXD);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to setup texture display: " + ex.Message);
            }

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

            PerformTXDConversionExternal(platform);

            TextureManager.LoadTexturesFromTXD(pathToPcTXD);

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);
        }

        public void ExportTextureDictionary(string fileName, bool RW3)
        {
            ReadFileMethods.treatStuffAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (Section_AHDR AHDR in GetAssetsOfType(AssetType.RWTX))
                if ((RW3 && AHDR.ADBG.assetName.Contains(".RW3")) || (!RW3 && !AHDR.ADBG.assetName.Contains(".RW3")))
                    try
                    {
                        foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(AHDR.data))
                            if (rw is TextureDictionary_0016 td)
                                foreach (TextureNative_0015 tn in td.textureNativeList)
                                {
                                    fileVersion = tn.renderWareVersion;
                                    tn.textureNativeStruct.textureName = AHDR.ADBG.assetName.Replace(".RW3", "");
                                    textNativeList.Add(tn);
                                }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to add RWTX asset {GetFromAssetID(AHDR.assetID).ToString()} to TXD archive: {ex.Message}");
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

            List<Section_AHDR> AHDRs = GetAssetsFromTextureDictionary(fileName, RW3);

            ImportMultipleAssets(0, AHDRs, out _, true);
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
                        Section_AHDR AHDR = new Section_AHDR(BKDRHash(textureName), AssetType.RWTX, AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM, ADBG, data);

                        AHDRs.Add(AHDR);
                    }
                }
            }
            ReadFileMethods.treatStuffAsByteArray = false;

            return AHDRs;
        }

        public Dictionary<string, Bitmap> ExportRWTXToBitmap(byte[] txdFile)
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
            "targetPlatform=uncompressed_mobile\r\n"
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
            }, currentTextureVersion(game)));
        }

        private bool PerformTextureConversion()
        {
            Dictionary<uint, Section_AHDR> dataDict = new Dictionary<uint, Section_AHDR>();

            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);
            
            try
            {
                ExportTextureDictionary(pathToPcTXD, true);
                PerformTXDConversionExternal(platform, false);
                foreach (var AHDR in GetAssetsFromTextureDictionary(pathToGcTXD, true))
                    dataDict.Add(AHDR.assetID, AHDR);

                ExportTextureDictionary(pathToPcTXD, false);
                PerformTXDConversionExternal(platform, false);
                foreach (var AHDR in GetAssetsFromTextureDictionary(pathToGcTXD, false))
                    dataDict.Add(AHDR.assetID, AHDR);
            }
            catch
            {
                File.Delete(pathToGcTXD);
                File.Delete(pathToPcTXD);
                return false;
            }

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);

            ReadFileMethods.treatStuffAsByteArray = true;

            foreach (var a in assetDictionary.Values)
                if (a is AssetRWTX rwtx)
                {
                    rwtx.Data =
                            ReadFileMethods.ExportRenderWareFile(
                            ReadFileMethods.ReadRenderWareFile(
                                dataDict[rwtx.AHDR.assetID].data),
                            Models.BSP_IO_Shared.modelRenderWareVersion(game));
                    rwtx.game = game;
                    rwtx.platform = platform;
                }

            ReadFileMethods.treatStuffAsByteArray = false;
            return true;
        }
    }
}
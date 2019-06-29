using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private class LHDRComparer : IComparer<int>
        {
            private static readonly List<int> layerOrderBFBB = new List<int> {
                (int)LayerType_BFBB.TEXTURE,
                (int)LayerType_BFBB.BSP,
                (int)LayerType_BFBB.JSPINFO,
                (int)LayerType_BFBB.MODEL,
                (int)LayerType_BFBB.ANIMATION,
                (int)LayerType_BFBB.DEFAULT,
                (int)LayerType_BFBB.CUTSCENE,
                (int)LayerType_BFBB.SRAM,
                (int)LayerType_BFBB.SNDTOC
            };
            private static readonly List<int> layerOrderTSSM = new List<int> {
                (int)LayerType_TSSM.TEXTURE,
                (int)LayerType_TSSM.TEXTURE_STRM,
                (int)LayerType_TSSM.BSP,
                (int)LayerType_TSSM.JSPINFO,
                (int)LayerType_TSSM.MODEL,
                (int)LayerType_TSSM.ANIMATION,
                (int)LayerType_TSSM.DEFAULT,
                (int)LayerType_TSSM.CUTSCENE,
                (int)LayerType_TSSM.SRAM,
                (int)LayerType_TSSM.SNDTOC,
                (int)LayerType_TSSM.CUTSCENETOC
            };

            public int Compare(int l1, int l2)
            {
                if (l1 == l2)
                    return 0;

                if (currentGame == Game.Scooby && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                    return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

                if (currentGame == Game.BFBB && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                    return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

                if (currentGame == Game.Incredibles)
                {
                    if ((l1 == 3 && l2 == 11) || (l1 == 11 && l2 == 3))
                        return 0;

                    if (layerOrderTSSM.Contains(l1) && layerOrderTSSM.Contains(l2))
                        return layerOrderTSSM.IndexOf(l1) > layerOrderTSSM.IndexOf(l2) ? 1 : -1;
                }

                return 0;
            }
        }

        public static List<uint> hiddenAssets = new List<uint>();

        public List<uint> GetHiddenAssets()
        {
            return (from asset in assetDictionary.Values where asset.isInvisible select asset.AHDR.assetID).ToList();
        }

        private List<IInternalEditor> internalEditors = new List<IInternalEditor>();

        public void CloseInternalEditor(IInternalEditor i)
        {
            internalEditors.Remove(i);
        }

        public void CloseInternalEditor(uint assetID)
        {
            for (int i = 0; i < internalEditors.Count; i++)
                if (internalEditors[i].GetAssetID() == assetID)
                    internalEditors[i].Close();
        }

        public void OpenInternalEditor(List<uint> list, bool openAnyway)
        {
            bool willOpen = true;
            if (list.Count > 15 && !openAnyway)
            {
                willOpen = MessageBox.Show($"Warning: you're going to open {list.Count} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }
                
            if (willOpen)
                foreach (uint u in list)
                    if (assetDictionary.ContainsKey(u))
                        OpenInternalEditor(assetDictionary[u]);
        }

        private void OpenInternalEditor(Asset asset)
        {
            CloseInternalEditor(asset.AHDR.assetID);

            switch (asset.AHDR.assetType)
            {
                case AssetType.BUTN:
                    internalEditors.Add(new InternalButtonEditor((AssetBUTN)asset, this));
                    break;
                case AssetType.CAM:
                    internalEditors.Add(new InternalCamEditor((AssetCAM)asset, this));
                    break;
                case AssetType.DYNA:
                    internalEditors.Add(new InternalDynaEditor((AssetDYNA)asset, this));
                    break;
                case AssetType.FLY:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this));
                    break;
                case AssetType.GRUP:
                    internalEditors.Add(new InternalGrupEditor((AssetGRUP)asset, this));
                    break;
                case AssetType.BSP:
                case AssetType.JSP:
                case AssetType.MODL:
                    internalEditors.Add(new InternalModelEditor((AssetRenderWareModel)asset, this));
                    break;
                case AssetType.PLAT:
                    internalEditors.Add(new InternalPlatEditor((AssetPLAT)asset, this));
                    break;
                case AssetType.RWTX:
                    internalEditors.Add(new InternalTextureEditor((AssetRWTX)asset, this));
                    break;
                case AssetType.SHRP:
                    internalEditors.Add(new InternalShrapnelEditor((AssetSHRP)asset, this));
                    break;
                case AssetType.SND:
                case AssetType.SNDS:
                    internalEditors.Add(new InternalSoundEditor(asset, this));
                    break;
                case AssetType.TEXT:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this));
                    break;
            }

            internalEditors.Last().Show();
        }

        public void SetAllTopMost(bool value)
        {
            foreach (var ie in internalEditors)
                ie.TopMost = value;
        }

        public static Vector3 GetRayInterserctionPosition(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
                l.AddRange(renderableAssetSetJSP);
            }
            catch { return Vector3.Zero; }

            float smallerDistance = 2000f;
            bool found = false;

            foreach (IRenderableAsset ra in l)
            {
                float? distance = ra.IntersectsWith(ray);
                if (distance != null && distance < smallerDistance)
                {
                    found = true;
                    smallerDistance = (float)distance;
                }
            }

            return ray.Position + Vector3.Normalize(ray.Direction) * (found ? smallerDistance : 2f);
        }

        public static uint GetClickedAssetID(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
            }
            catch { return 0; }

            float smallerDistance = 1000f;
            uint assetID = 0;

            foreach (Asset ra in l)
            {
                if (!ra.isSelected && ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
                }
            }

            return assetID;
        }

        public static uint GetClickedAssetID2D(Ray ray, float farPlane)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                foreach (IRenderableAsset a in renderableAssetSetCommon)
                    if (a is AssetUI ui)
                        l.Add(ui);
                    else if (a is AssetUIFT uift)
                        l.Add(uift);
            }
            catch { return 0; }

            float smallerDistance = 3 * farPlane;
            uint assetID = 0;

            foreach (Asset ra in l)
            {
                if (!ra.isSelected && ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
                }
            }

            return assetID;
        }

        public List<uint> FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = new List<uint>();
            foreach (Asset asset in assetDictionary.Values)
                if (asset.HasReference(assetID))
                    whoTargets.Add(asset.AHDR.assetID);

            return whoTargets;
        }

        public void ExportHip(string fileName)
        {
            HipSection[] hipFile = SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM);
            HipArrayToIni(hipFile, fileName, true, true);
        }

        public void ImportHip(string[] fileNames)
        {
            foreach (string fileName in fileNames)
                ImportHip(fileName);
        }

        public void ImportHip(string fileName)
        {
            if (Path.GetExtension(fileName).ToLower() == ".hip" || Path.GetExtension(fileName).ToLower() == ".hop")
                ImportHip(HipFileToHipArray(fileName));
            else if (Path.GetExtension(fileName).ToLower() == ".ini")
                ImportHip(IniToHipArray(fileName));
            else
                MessageBox.Show("Invalid file: " + fileName);
        }

        public void ImportHip(HipSection[] hipSections)
        {
            UnsavedChanges = true;

            foreach (HipSection i in hipSections)
            {
                if (i is Section_DICT dict)
                {
                    foreach (Section_AHDR AHDR in dict.ATOC.AHDRList)
                    {
                        if (AHDR.assetType == AssetType.COLL && ContainsAssetWithType(AssetType.COLL))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeCOLL(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.JAW && ContainsAssetWithType(AssetType.JAW))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeJAW(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.LODT && ContainsAssetWithType(AssetType.LODT))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeLODT(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.PIPT && ContainsAssetWithType(AssetType.PIPT))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergePIPT(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.SHDW && ContainsAssetWithType(AssetType.SHDW))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeSHDW(AHDR);
                            continue;
                        }
                        else if (AHDR.assetType == AssetType.SNDI && ContainsAssetWithType(AssetType.SNDI))
                        {
                            foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                LHDR.assetIDlist.Remove(AHDR.assetID);

                            MergeSNDI(AHDR);
                            continue;
                        }

                        if (ContainsAsset(AHDR.assetID))
                        {
                            DialogResult result = MessageBox.Show($"Asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                RemoveAsset(AHDR.assetID);
                                DICT.ATOC.AHDRList.Add(AHDR);
                                AddAssetToDictionary(AHDR);
                            }
                            else
                            {
                                foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                                    LHDR.assetIDlist.Remove(AHDR.assetID);
                            }
                        }
                        else
                        {
                            DICT.ATOC.AHDRList.Add(AHDR);
                            AddAssetToDictionary(AHDR);
                        }
                    }

                    foreach (Section_LHDR LHDR in dict.LTOC.LHDRList)
                        if (LHDR.assetIDlist.Count != 0)
                            DICT.LTOC.LHDRList.Add(LHDR);

                    break;
                }
            }

            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer()).ToList();

            RecalculateAllMatrices();
        }

        public void MergeSimilar()
        {
            UnsavedChanges = true;

            List<Section_AHDR> COLLs = GetAssetsOfType(AssetType.COLL);
            for (int i = 1; i < COLLs.Count; i++)
                RemoveAsset(COLLs[i].assetID);
            for (int i = 1; i < COLLs.Count; i++)
                MergeCOLL(COLLs[i]);
            
            List<Section_AHDR> JAWs = GetAssetsOfType(AssetType.JAW);
            for (int i = 1; i < JAWs.Count; i++)
                RemoveAsset(JAWs[i].assetID);
            for (int i = 1; i < JAWs.Count; i++)
                MergeJAW(JAWs[i]);
            
            List<Section_AHDR> LODTs = GetAssetsOfType(AssetType.LODT);
            for (int i = 1; i < LODTs.Count; i++)            
                RemoveAsset(LODTs[i].assetID);
            for (int i = 1; i < LODTs.Count; i++)
                MergeLODT(LODTs[i]);
            
            List<Section_AHDR> PIPTs = GetAssetsOfType(AssetType.PIPT);
            for (int i = 1; i < PIPTs.Count; i++)
                RemoveAsset(PIPTs[i].assetID);
            for (int i = 1; i < PIPTs.Count; i++)
                MergePIPT(PIPTs[i]);
            
            List<Section_AHDR> SHDWs = GetAssetsOfType(AssetType.SHDW);
            for (int i = 1; i < SHDWs.Count; i++)            
                RemoveAsset(SHDWs[i].assetID);
            for (int i = 1; i < SHDWs.Count; i++)
                MergeSHDW(SHDWs[i]);
            
            List<Section_AHDR> SNDIs = GetAssetsOfType(AssetType.SNDI);
            for (int i = 1; i < SNDIs.Count; i++)            
                RemoveAsset(SNDIs[i].assetID);
            for (int i = 1; i < SNDIs.Count; i++)
                MergeSNDI(SNDIs[i]);
        }

        private void MergeCOLL(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetCOLL COLL)
                {
                    COLL.Merge(new AssetCOLL(AHDR));
                    return;
                }
            }
        }

        private void MergeJAW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.Merge(new AssetJAW(AHDR));
                    return;
                }
            }
        }

        private void MergeLODT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetLODT LODT)
                {
                    LODT.Merge(new AssetLODT(AHDR));
                    return;
                }
            }
        }

        private void MergePIPT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetPIPT PIPT)
                {
                    PIPT.Merge(new AssetPIPT(AHDR));
                    return;
                }
            }
        }

        private void MergeSHDW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSHDW SHDW)
                {
                    SHDW.Merge(new AssetSHDW(AHDR));
                    return;
                }
            }
        }

        private void MergeSNDI(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.Merge(new AssetSNDI_GCN_V1(AHDR));
                    return;
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    SNDI_G2.Merge(new AssetSNDI_GCN_V2(AHDR));
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.Merge(new AssetSNDI_XBOX(AHDR));
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    SNDI_P.Merge(new AssetSNDI_PS2(AHDR));
                    return;
                }
            }
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
        
        public static void ExportSingleTextureToDictionary(string fileName, Section_AHDR RWTX)
        {
            ExportSingleTextureToDictionary(fileName, RWTX.data, RWTX.ADBG.assetName.Replace(".RW3", ""));
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
                        tn.textureNativeStruct.textureName = textureName;
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

        public void AddTextureDictionary(string fileName, bool RW3)
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
            string textureName = Path.GetFileNameWithoutExtension(fileName);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fileName);

            List<byte> bitmapData = new List<byte>(bitmap.Width * bitmap.Height * 4);

            if (flip)
                bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

            for (int j = 0; j < bitmap.Height; j++)
                for (int i = 0; i < bitmap.Width; i++)
                {
                    bitmapData.Add(bitmap.GetPixel(i, j).B);
                    bitmapData.Add(bitmap.GetPixel(i, j).G);
                    bitmapData.Add(bitmap.GetPixel(i, j).R);
                    bitmapData.Add(bitmap.GetPixel(i, j).A);
                }

            TextureDictionary_0016 td = new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                textureNativeList = new List<TextureNative_0015>()
                {
                    new TextureNative_0015()
                    {
                        textureNativeStruct = new TextureNativeStruct_0001(){
                            textureName = textureName,
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
                },
                textureDictionaryExtension = new Extension_0003()
            };

            bitmap.Dispose();

            // created PC txd, now will convert to gamecube.
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);

            ExportSingleTextureToDictionary(pathToPcTXD, ReadFileMethods.ExportRenderWareFile(td, currentTextureVersion), textureName);

            PerformTXDConversionExternal(false, compress, mipmaps);
            
            string assetName = textureName + (appendRW3 ? ".RW3" : "");

            ReadFileMethods.treatStuffAsByteArray = true;

            Section_AHDR AHDR = new Section_AHDR(BKDRHash(assetName), AssetType.RWTX, AHDRFlagsFromAssetType(AssetType.RWTX),
                new Section_ADBG(0, assetName, "", 0), ReadFileMethods.ExportRenderWareFile(ReadFileMethods.ReadRenderWareFile(pathToGcTXD), currentTextureVersion));

            ReadFileMethods.treatStuffAsByteArray = false;

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);

            return AHDR;
        }

        public static System.Drawing.Bitmap ExportRWTXToBitmap(byte[] textureData)
        {
            if (!Directory.Exists(tempPcTxdsDir))
                Directory.CreateDirectory(tempPcTxdsDir);
            if (!Directory.Exists(tempGcTxdsDir))
                Directory.CreateDirectory(tempGcTxdsDir);

            File.WriteAllBytes(pathToGcTXD, textureData);

            PerformTXDConversionExternal(true);

            System.Drawing.Bitmap bitmap = null;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(pathToPcTXD))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        List<System.Drawing.Color> bitmapData = new List<System.Drawing.Color>();

                        byte[] imageData = tn.textureNativeStruct.mipMaps[0].data;

                        if (tn.textureNativeStruct.compression == 0)
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

                        bitmap = new System.Drawing.Bitmap(tn.textureNativeStruct.width, tn.textureNativeStruct.height);

                        int k = 0;
                        for (int i = 0; i < bitmap.Width; i++)
                            for (int j = 0; j < bitmap.Height; j++)
                            {
                                int v = k;
                                int v2 = bitmapData.Count;
                                System.Drawing.Color c = bitmapData[k];
                                bitmap.SetPixel(i, j, c);
                                k++;
                            }
                    }
                }
            }

            File.Delete(pathToGcTXD);
            File.Delete(pathToPcTXD);

            return bitmap;
        }
        
        public void AddSoundToSNDI(byte[] soundData, uint assetID, AssetType assetType, out byte[] finalData)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    SNDI_G2.AddEntry(soundData, assetID);
                    finalData = new byte[0];
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    SNDI_P.AddEntry(soundData, assetID, assetType, out finalData);
                    return;
                }
            }

            throw new Exception("Unable to add sound: SNDI asset not found");
        }

        public void RemoveSoundFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        SNDI_G1.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    if (SNDI_G2.HasReference(assetID))
                        SNDI_G2.RemoveEntry(assetID);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        SNDI_X.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        SNDI_P.RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
            }
        }

        public byte[] GetHeaderFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    if (SNDI_G1.HasReference(assetID))
                        return SNDI_G1.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    if (SNDI_G2.HasReference(assetID))
                        return SNDI_G2.GetHeader(assetID);
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    if (SNDI_X.HasReference(assetID))
                        return SNDI_X.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    if (SNDI_P.HasReference(assetID))
                        return SNDI_P.GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);
                }
            }

            throw new Exception("Error: could not find SNDI asset which contains this sound in this archive.");
        }

        public void AddJawDataToJAW(byte[] jawData, uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.AddEntry(jawData, assetID);
                    return;
                }
            }

            throw new Exception("Unable to add jaw data: JAW asset not found");
        }

        public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.ALST:
                case AssetType.BOUL:
                case AssetType.BUTN:
                case AssetType.CAM:
                case AssetType.CNTR:
                case AssetType.COLL:
                case AssetType.COND:
                case AssetType.CSNM:
                case AssetType.CTOC:
                case AssetType.DPAT:
                case AssetType.DSCO:
                case AssetType.DSTR:
                case AssetType.DYNA:
                case AssetType.EGEN:
                case AssetType.ENV:
                case AssetType.FOG:
                case AssetType.HANG:
                case AssetType.GRUP:
                case AssetType.JAW:
                case AssetType.LODT:
                case AssetType.MAPR:
                case AssetType.MINF:
                case AssetType.MRKR:
                case AssetType.MVPT:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PEND:
                case AssetType.PICK:
                case AssetType.PIPT:
                case AssetType.PKUP:
                case AssetType.PLAT:
                case AssetType.PLYR:
                case AssetType.PORT:
                case AssetType.SFX:
                case AssetType.SHDW:
                case AssetType.SHRP:
                case AssetType.SIMP:
                case AssetType.SNDI:
                case AssetType.SURF:
                case AssetType.TEXT:
                case AssetType.TIMR:
                case AssetType.TRIG:
                case AssetType.UI:
                case AssetType.UIFT:
                case AssetType.VIL:
                case AssetType.VILP:
                    return AHDRFlags.SOURCE_VIRTUAL;
                case AssetType.CSN:
                case AssetType.FLY:
                case AssetType.RAW:
                    return AHDRFlags.SOURCE_FILE;
                case AssetType.ANIM:
                case AssetType.CRDT:
                case AssetType.SND:
                case AssetType.SNDS:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM;
                case AssetType.MODL:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM;
                case AssetType.ATBL:
                case AssetType.JSP:
                case AssetType.RWTX:
                    return AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM;
                case AssetType.LKIT:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM;
                default:
                    return 0;
            }
        }

        public void CollapseLayers()
        {
            Dictionary<int, Section_LHDR> layers = new Dictionary<int, Section_LHDR>();
            List<Section_LHDR> bspLayers = new List<Section_LHDR>();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                if (currentGame == Game.Incredibles && (LHDR.layerType == (int)LayerType_TSSM.BSP || LHDR.layerType == (int)LayerType_TSSM.JSPINFO))
                {
                    if (LHDR.assetIDlist.Count != 0)
                        bspLayers.Add(LHDR);
                }
                else if (layers.ContainsKey(LHDR.layerType))
                {
                    layers[LHDR.layerType].assetIDlist.AddRange(LHDR.assetIDlist);
                }
                else if (LHDR.assetIDlist.Count != 0)
                {
                    layers[LHDR.layerType] = LHDR;
                }
            }

            UnsavedChanges = true;
            
            DICT.LTOC.LHDRList = new List<Section_LHDR>();
            DICT.LTOC.LHDRList.AddRange(layers.Values.ToList());
            DICT.LTOC.LHDRList.AddRange(bspLayers);
            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer()).ToList();
        }

        public string VerifyArchive()
        {
            string result = "";
            char endl = '\n';

            ProgressBar progressBar = new ProgressBar("Verify Archive");
            progressBar.SetProgressBar(0, DICT.ATOC.AHDRList.Count, 1);
            progressBar.Show();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                foreach (uint assetID in LHDR.assetIDlist)
                    if (!ContainsAsset(assetID))
                        result += $"Archive: Asset 0x{assetID.ToString("X8")} appears to be present in a layer, but it's not in the AHDR dictionary. This archive is likely unusable." + endl;

            List<Asset> ordered = assetDictionary.Values.OrderBy(f => f.AHDR.ADBG.assetName).ToList();
            ordered = ordered.OrderBy(f => f.AHDR.ADBG.assetName).ToList();

            if (!ContainsAssetWithType(AssetType.JSP))
                result += $"Archive: Does not contain any JSP asset." + endl;

            foreach (Asset asset in ordered)
            {
                bool found = false;

                foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    foreach (uint assetID in LHDR.assetIDlist)
                        if (assetID == asset.AHDR.assetID)
                        {
                            if (found == false)
                                found = true;
                            else
                                result += $"Archive: Asset {asset.ToString()} is present in more than one layer. This is unexpected." + endl;
                        }

                if (found == false)
                    result += $"Archive: Asset {asset.ToString()} appears to not be present in the AHDR dictionary, but it's not in any layer. This archive is likely unusable." + endl;

                List<string> resultParam = new List<string>();
                try
                {
                    asset.Verify(ref resultParam);

                    foreach (string s in resultParam)
                        result += $"[{asset.AHDR.assetType.ToString()}] {asset.AHDR.ADBG.assetName}: " + s + endl;
                }
                catch (Exception e)
                {
                    result += $"Failed verification on [{asset.AHDR.assetType.ToString()}] {asset.AHDR.ADBG.assetName}: " + e.Message + endl;
                }

                progressBar.PerformStep();
            }

            progressBar.Close();

            return result;
        }

        public void ApplyScale(Vector3 factor)
        {
            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetTRIG TRIG)
                {
                    if (TRIG.Shape == TriggerShape.Sphere)
                    {
                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Radius *= singleFactor;
                    }
                    else
                    {
                        Vector3 TrigCenter = new Vector3(TRIG.Position0X + TRIG.Radius, TRIG.Position0Y + TRIG.Position1Y, TRIG.Position0Z + TRIG.Position1Z) / 2f;

                        TRIG.Position0X -= TrigCenter.X;
                        TRIG.Position0Y -= TrigCenter.Y;
                        TRIG.Position0Z -= TrigCenter.Z;
                        TRIG.Position0X -= TrigCenter.X;
                        TRIG.Position1Y -= TrigCenter.Y;
                        TRIG.Position1Z -= TrigCenter.Z;

                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Position0X *= factor.X;
                        TRIG.Position1Y *= factor.Y;
                        TRIG.Position1Z *= factor.Z;

                        TRIG.Position0X += TrigCenter.X * factor.X;
                        TRIG.Position0Y += TrigCenter.Y * factor.Y;
                        TRIG.Position0Z += TrigCenter.Z * factor.Z;
                        TRIG.Position0X += TrigCenter.X * factor.X;
                        TRIG.Position1Y += TrigCenter.Y *factor.Y;
                        TRIG.Position1Z += TrigCenter.Z * factor.Z;
                    }

                    TRIG._position.X = TRIG.Position0X;
                    TRIG._position.Y = TRIG.Position0Y;
                    TRIG._position.Z = TRIG.Position0Z;
                }
                else if (a is IClickableAsset clickableAsset)
                {
                    ((IClickableAsset)a).PositionX *= factor.X;
                    ((IClickableAsset)a).PositionY *= factor.Y;
                    ((IClickableAsset)a).PositionZ *= factor.Z;

                    if (a is AssetMVPT MVPT)
                    {
                        if (MVPT.ZoneRadius != -1)
                            MVPT.ZoneRadius *= singleFactor;
                        if (MVPT.ArenaRadius != -1)
                            MVPT.ArenaRadius *= singleFactor;
                    }
                    else if (a is AssetSFX SFX)
                    {
                        SFX.OuterRadius *= singleFactor;
                        SFX.InnerRadius *= singleFactor;
                    }
                    else if (a is PlaceableAsset placeable && !(a is AssetPLYR || a is AssetPKUP || a is AssetUI || a is AssetUIFT || a is AssetVIL || (a is AssetDYNA DYNA && DYNA.Type_BFBB == DynaType_BFBB.game_object__Teleport)))
                    {
                        placeable.ScaleX *= factor.X;
                        placeable.ScaleY *= factor.Y;
                        placeable.ScaleZ *= factor.Z;
                    }
                }
            }

            UnsavedChanges = true;
            RecalculateAllMatrices();
        }
    }
}
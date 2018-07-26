using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class ArchiveEditorFunctions
    {
        public static HashSet<RenderableAsset> renderableAssetSet = new HashSet<RenderableAsset>();
        public static Dictionary<int, AssetWithModel> renderingDictionary = new Dictionary<int, AssetWithModel>();

        public static void AddToRenderingDictionary(int key, AssetWithModel value)
        {
            if (!renderingDictionary.ContainsKey(key))
                renderingDictionary.Add(key, value);
            else
                renderingDictionary[key] = value;
        }

        private Dictionary<int, Asset> assetDictionary = new Dictionary<int, Asset>();

        public bool DictionaryHasKey(int key)
        {
            return assetDictionary.ContainsKey(key);
        }

        public Asset GetFromAssetID(int key)
        {
            if (DictionaryHasKey(key))
                return assetDictionary[key];
            else
                throw new KeyNotFoundException();
        }

        public Dictionary<int, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }
        
        public static void ExportTextureAsset(AssetRWTX asset, string fileNamePrefix)
        {
            Directory.CreateDirectory(Application.StartupPath + "\\Export\\" + fileNamePrefix);
            File.WriteAllBytes(Application.StartupPath + "\\Export\\" + fileNamePrefix + "\\" + Path.GetFileNameWithoutExtension(asset.AHDR.ADBG.assetName) + ".txd", asset.AHDR.containedFile);
        }

        public string fileNamePrefix;
        public string currentlyOpenFilePath;
        public Section_HIPA HIPA;
        public Section_PACK PACK;
        public Section_DICT DICT;
        public Section_STRM STRM;

        public void OpenFile(string fileName)
        {
            Dispose();

            currentlyOpenFilePath = fileName;
                        
            fileNamePrefix = Path.GetFileNameWithoutExtension(fileName);

            HipSection[] HipFile = HipFileToHipArray(fileName);

            foreach (HipSection i in HipFileToHipArray(fileName))
            {
                if (i is Section_HIPA hipa) HIPA = hipa;
                else if (i is Section_PACK pack) PACK = pack;
                else if (i is Section_DICT dict) DICT = dict;
                else if (i is Section_STRM strm) STRM = strm;
                else throw new Exception();
            }
                        
            SharpRenderer.LoadTextures(fileNamePrefix);

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                AddAssetToDictionary(AHDR);

            foreach (RenderableAsset a in renderableAssetSet)
                a.Setup();
        }

        public void Dispose()
        {
            foreach (int key in assetDictionary.Keys)
            {
                if (assetDictionary[key] is RenderableAsset ra)
                    if (renderableAssetSet.Contains(ra))
                    {
                        renderableAssetSet.Remove(ra);
                    }
                if (renderingDictionary.ContainsKey(key))
                {
                    renderingDictionary.Remove(key);
                }
            }
            assetDictionary.Clear();

            if (DICT == null) return;
            HIPA = null;
            PACK = null;
            DICT = null;
            STRM = null;
            fileNamePrefix = null;
            currentlyOpenFilePath = null;
        }

        private void AddAssetToDictionary(Section_AHDR AHDR)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            switch (AHDR.assetType)
            {
                case AssetType.BSP:
                case AssetType.JSP:
                    {
                        AssetLevelModel newAsset = new AssetLevelModel(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.RWTX:
                    {
                        AssetRWTX newAsset = new AssetRWTX(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BUTN:
                case AssetType.PLAT:
                case AssetType.SIMP:
                case AssetType.VIL:
                    {
                        RenderableAsset newAsset = new RenderableAsset(AHDR); ;
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PKUP:
                    {
                        AssetPKUP newAsset = new AssetPKUP(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MINF:
                    {
                        AssetMINF newAsset = new AssetMINF(AHDR); ;
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MODL:
                    {
                        AssetMODL newAsset = new AssetMODL(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MVPT:
                    {
                        AssetMVPT newAsset = new AssetMVPT(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PICK:
                    {
                        AssetPICK newAsset = new AssetPICK(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    {
                        AssetGeneric newAsset = new AssetGeneric(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
            }
        }

        public void RemoveLayer(int index)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList[index].assetIDlist.Count(); i++)
                RemoveAsset(index, DICT.LTOC.LHDRList[index].assetIDlist[i]);

            DICT.LTOC.LHDRList.RemoveAt(index);
        }

        public void AddAsset(int layerIndex, Section_AHDR AHDR)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            DICT.ATOC.AHDRList.Add(AHDR);
            AddAssetToDictionary(AHDR);
        }

        public void RemoveAsset(int layerIndex, int assetID)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Remove(assetID);

            if (renderingDictionary.ContainsKey(assetID))
                renderingDictionary.Remove(assetID);
            if (renderableAssetSet.Contains(assetDictionary[assetID]))
                renderableAssetSet.Remove(assetDictionary[assetID] as RenderableAsset);

            assetDictionary.Remove(assetID);

            for (int i = 0; i < DICT.ATOC.AHDRList.Count; i++)
            {
                if (DICT.ATOC.AHDRList[i].assetID == assetID)
                {
                    DICT.ATOC.AHDRList.RemoveAt(i);
                    break;
                }
            }
        }

        private int currentlySelectedAssetID = 0;

        public int getCurrentlySelectedAssetID()
        {
            return currentlySelectedAssetID;
        }

        public void SelectAsset(int assetID)
        {
            if (assetDictionary.ContainsKey(currentlySelectedAssetID))
                assetDictionary[currentlySelectedAssetID].isSelected = false;
            currentlySelectedAssetID = assetID;
            if (currentlySelectedAssetID != 0)
                assetDictionary[currentlySelectedAssetID].isSelected = true;
        }

        public int GetSelectedLayerIndex()
        {
            if (currentlySelectedAssetID == 0)
                throw new Exception();

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
            {
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(currentlySelectedAssetID))
                    return i;
            }
            throw new Exception();
        }

        public int ScreenClicked(Ray ray)
        {
            int assetID = 0;

            float smallerDistance = 1000f;
            foreach (RenderableAsset ra in renderableAssetSet)
            {
                if (ra.isSelected) continue;

                float? distance = ra.IntersectsWith(ray);
                if (distance != null)
                    if (distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
            }

            if (assetID != 0 & assetDictionary.ContainsKey(assetID))
                SelectAsset(assetID);
            return assetID;
        }

        public void Save()
        {
            List<byte> newStream = new List<byte>();
            for (int j = 0; j < STRM.DPAK.firstPadding; j++)
                newStream.Add(0x33);

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                AHDR.fileOffset = newStream.Count();
                AHDR.fileSize = AHDR.containedFile.Length;
                newStream.AddRange(AHDR.containedFile);

                AHDR.plusValue = 0;

                int alignment = 16;
                if (currentGame == Game.BFBB)
                {
                    if (AHDR.assetType == AssetType.CSN |
                        AHDR.assetType == AssetType.SND |
                        AHDR.assetType == AssetType.SNDS)
                        alignment = 32;
                    else if (AHDR.assetType == AssetType.CRDT)
                        alignment = 4;
                }

                int value = AHDR.fileSize % alignment;
                if (value != 0)
                    AHDR.plusValue = alignment - value;
                for (int j = 0; j < AHDR.plusValue; j++)
                    newStream.Add(0x33);
            }

            int value2 = (newStream.Count - STRM.DPAK.firstPadding) % 0x20;
            if (value2 != 0)
                for (int j = 0; j < 0x20 - value2; j++)
                    newStream.Add(0x33);

            STRM.DPAK.data = newStream.ToArray();
            PACK.PCNT = new Section_PCNT(DICT.ATOC.AHDRList.Count, DICT.LTOC.LHDRList.Count, 0, 0, 0);
            
            File.WriteAllBytes(currentlyOpenFilePath, HipArrayToFile(new HipSection[] { HIPA, PACK, DICT, STRM }));
        }
    }
}
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
    public class ArchiveEditorFunctions
    {
        public static HashSet<IRenderableAsset> renderableAssetSetCommon = new HashSet<IRenderableAsset>();
        public static HashSet<IRenderableAsset> renderableAssetSetTrans = new HashSet<IRenderableAsset>();
        public static HashSet<AssetJSP> renderableAssetSetJSP = new HashSet<AssetJSP>();
        public static Dictionary<uint, IAssetWithModel> renderingDictionary = new Dictionary<uint, IAssetWithModel>();

        public static void AddToRenderingDictionary(uint key, IAssetWithModel value)
        {
            if (!renderingDictionary.ContainsKey(key))
                renderingDictionary.Add(key, value);
            else
                renderingDictionary[key] = value;
        }

        public bool UnsavedChanges { get; set; } = false;

        public ArchiveEditorFunctions()
        {
            gizmos = new Gizmo[3];
            gizmos[0] = new Gizmo(GizmoType.X);
            gizmos[1] = new Gizmo(GizmoType.Y);
            gizmos[2] = new Gizmo(GizmoType.Z);
        }
        
        public string currentlyOpenFilePath;
        public Section_HIPA HIPA;
        public Section_PACK PACK;
        public Section_DICT DICT;
        public Section_STRM STRM;

        public bool New()
        {
            HipSection[] hipFile = NewArchive.GetNewArchive(out bool OK, out Platform platform, out Game game);

            if (OK)
            {
                Dispose();

                currentlySelectedAssets = new List<Asset>();
                currentlyOpenFilePath = null;

                foreach (HipSection i in hipFile)
                {
                    if (i is Section_HIPA hipa) HIPA = hipa;
                    else if (i is Section_PACK pack) PACK = pack;
                    else if (i is Section_DICT dict) DICT = dict;
                    else if (i is Section_STRM strm) STRM = strm;
                    else throw new Exception();
                }

                currentPlatform = platform;
                currentGame = game;

                if (currentPlatform == Platform.Unknown)
                    new ChoosePlatformDialog().ShowDialog();

                foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR);
                RecalculateAllMatrices();
            }

            return OK;
        }

        public void OpenFile(string fileName)
        {
            Dispose();

            currentlySelectedAssets = new List<Asset>();
            currentlyOpenFilePath = fileName;                        

            HipSection[] HipFile = HipFileToHipArray(fileName);

            foreach (HipSection i in HipFileToHipArray(fileName))
            {
                if (i is Section_HIPA hipa) HIPA = hipa;
                else if (i is Section_PACK pack) PACK = pack;
                else if (i is Section_DICT dict) DICT = dict;
                else if (i is Section_STRM strm) STRM = strm;
                else throw new Exception();
            }

            if (currentPlatform == Platform.Unknown)
                new ChoosePlatformDialog().ShowDialog();

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                AddAssetToDictionary(AHDR);
            RecalculateAllMatrices();
        }

        public void Save()
        {
            HipSection[] hipFile = SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM);
            byte[] file = HipArrayToFile(hipFile);
            File.WriteAllBytes(currentlyOpenFilePath, file);
            UnsavedChanges = false;
        }

        private Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public bool ContainsAsset(uint key)
        {
            return assetDictionary.ContainsKey(key);
        }

        public Asset GetFromAssetID(uint key)
        {
            if (ContainsAsset(key))
                return assetDictionary[key];
            throw new KeyNotFoundException("Asset not present in dictionary.");
        }

        public Dictionary<uint, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }

        public void Dispose()
        {
            List<uint> assetList = new List<uint>();
            assetList.AddRange(assetDictionary.Keys);

            foreach (uint assetID in assetList)
                RemoveAsset(assetID);

            if (DICT == null) return;
            HIPA = null;
            PACK = null;
            DICT = null;
            STRM = null;
            currentlyOpenFilePath = null;
        }

        public static bool allowRender = true;

        private void AddAssetToDictionary(Section_AHDR AHDR)
        {
            allowRender = false;

            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            switch (AHDR.assetType)
            {
                case AssetType.ANIM:
                    {
                        AssetANIM newAsset = new AssetANIM(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ALST:
                    {
                        AssetALST newAsset = new AssetALST(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ATBL:
                    {
                        AssetATBL newAsset = new AssetATBL(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BSP:
                case AssetType.JSP:
                    {
                        AssetJSP newAsset = new AssetJSP(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BOUL:
                    {
                        AssetBOUL newAsset = new AssetBOUL(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BUTN:
                    {
                        AssetBUTN newAsset = new AssetBUTN(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CAM:
                    {
                        AssetCAM newAsset = new AssetCAM(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CNTR:
                    {
                        AssetCNTR newAsset = new AssetCNTR(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.COLL:
                    {
                        AssetCOLL newAsset = new AssetCOLL(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.COND:
                    {
                        AssetCOND newAsset = new AssetCOND(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DPAT:
                    {
                        AssetDPAT newAsset = new AssetDPAT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DSCO:
                    {
                        AssetDSCO newAsset = new AssetDSCO(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DSTR:
                    {
                        AssetDSTR newAsset = new AssetDSTR(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DYNA:
                    {
                        AssetDYNA newAsset = new AssetDYNA(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.EGEN:
                    {
                        AssetEGEN newAsset = new AssetEGEN(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ENV:
                    {
                        AssetENV newAsset = new AssetENV(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.FOG:
                    {
                        AssetFOG newAsset = new AssetFOG(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.GRUP:
                    {
                        AssetGRUP newAsset = new AssetGRUP(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.HANG:
                    {
                        AssetHANG newAsset = new AssetHANG(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.JAW:
                    {
                        AssetJAW newAsset = new AssetJAW(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.LODT:
                    {
                        AssetLODT newAsset = new AssetLODT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MAPR:
                    {
                        AssetMAPR newAsset = new AssetMAPR(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MINF:
                    {
                        AssetMINF newAsset = new AssetMINF(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MODL:
                    {
                        AssetMODL newAsset = new AssetMODL(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MRKR:
                    {
                        AssetMRKR newAsset = new AssetMRKR(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MVPT:
                    {
                        if (currentGame == Game.BFBB)
                        {
                            AssetMVPT newAsset = new AssetMVPT(AHDR);
                            newAsset.Setup();
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                        else
                        {
                            Asset newAsset = new Asset(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                    }
                    break;
                case AssetType.PEND:
                    {
                        AssetPEND newAsset = new AssetPEND(AHDR);
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
                case AssetType.PIPT:
                    {
                        AssetPIPT newAsset = new AssetPIPT(AHDR);
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
                case AssetType.PLAT:
                    {
                        AssetPLAT newAsset = new AssetPLAT(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PLYR:
                    {
                        AssetPLYR newAsset = new AssetPLYR(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PORT:
                    {
                        AssetPORT newAsset = new AssetPORT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.RWTX:
                    {
                        AssetRWTX newAsset = new AssetRWTX(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SFX:
                    {
                        AssetSFX newAsset = new AssetSFX(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SIMP:
                    {
                        AssetSIMP newAsset = new AssetSIMP(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SHDW:
                    {
                        AssetSHDW newAsset = new AssetSHDW(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SNDI:
                    {
                        if (currentPlatform == Platform.GameCube && (currentGame == Game.BFBB || currentGame == Game.Scooby))
                        {
                            AssetSNDI_GCN_V1 newAsset = new AssetSNDI_GCN_V1(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                        else if (currentPlatform == Platform.Xbox)
                        {
                            AssetSNDI_XBOX newAsset = new AssetSNDI_XBOX(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                        else if (currentPlatform == Platform.PS2)
                        {
                            AssetSNDI_PS2 newAsset = new AssetSNDI_PS2(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                        else
                        {
                            Asset newAsset = new Asset(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                    }
                    break;
                case AssetType.TEXT:
                    {
                        AssetTEXT newAsset = new AssetTEXT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TRIG:
                    {
                        AssetTRIG newAsset = new AssetTRIG(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TIMR:
                    {
                        AssetTIMR newAsset = new AssetTIMR(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.UI:
                    {
                        AssetUI newAsset = new AssetUI(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.UIFT:
                    {
                        AssetUIFT newAsset = new AssetUIFT(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.VIL:
                    {
                        AssetVIL newAsset = new AssetVIL(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CCRV:
                case AssetType.CSNM:
                case AssetType.DTRK:
                case AssetType.DUPC:
                case AssetType.GRSM:
                case AssetType.GUST:
                case AssetType.LITE:
                case AssetType.LOBM:
                case AssetType.NGMS:
                case AssetType.NPC:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PGRS:
                case AssetType.PRJT:
                case AssetType.RANM:
                case AssetType.SCRP:
                case AssetType.SDFX:
                case AssetType.SGRP:
                case AssetType.SLID:
                case AssetType.SPLN:
                case AssetType.SSET:
                case AssetType.SUBT:
                case AssetType.SURF:
                case AssetType.TPIK:
                case AssetType.TRWT:
                case AssetType.UIM:
                case AssetType.VOLU:
                case AssetType.ZLIN:
                    {
                        ObjectAsset newAsset = new ObjectAsset(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ATKT:
                case AssetType.BINK:
                case AssetType.CRDT:
                case AssetType.CSN:
                case AssetType.CSSS:
                case AssetType.CTOC:
                case AssetType.DEST:
                case AssetType.FLY:
                case AssetType.LKIT:
                case AssetType.MPHT:
                case AssetType.NPCS:
                case AssetType.ONEL:
                case AssetType.RAW:
                case AssetType.SHRP:
                case AssetType.SND:
                case AssetType.SNDS:
                case AssetType.SPLP:
                case AssetType.TEXS:
                case AssetType.UIFN:
                case AssetType.VILP:
                case AssetType.WIRE:
                    {
                        Asset newAsset = new Asset(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    throw new Exception("Unknown asset type: " + AHDR.assetType);
            }

            allowRender = true;
        }

        public void RemoveLayer(int index)
        {
            RemoveAsset(DICT.LTOC.LHDRList[index].assetIDlist);

            DICT.LTOC.LHDRList.RemoveAt(index);

            UnsavedChanges = true;
        }

        public void MoveLayerUp(int selectedIndex)
        {
            if (selectedIndex > 0)
            {
                Section_LHDR previous = DICT.LTOC.LHDRList[selectedIndex - 1];
                DICT.LTOC.LHDRList[selectedIndex - 1] = DICT.LTOC.LHDRList[selectedIndex];
                DICT.LTOC.LHDRList[selectedIndex] = previous;
                UnsavedChanges = true;
            }
        }

        public void MoveLayerDown(int selectedIndex)
        {
            if (selectedIndex < DICT.LTOC.LHDRList.Count - 1)
            {
                Section_LHDR post = DICT.LTOC.LHDRList[selectedIndex + 1];
                DICT.LTOC.LHDRList[selectedIndex + 1] = DICT.LTOC.LHDRList[selectedIndex];
                DICT.LTOC.LHDRList[selectedIndex] = post;
                UnsavedChanges = true;
            }
        }

        public uint AddAsset(int layerIndex, Section_AHDR AHDR)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            DICT.ATOC.AHDRList.Add(AHDR);
            AddAssetToDictionary(AHDR);

            return AHDR.assetID;
        }

        public uint AddAssetWithUniqueID(int layerIndex, Section_AHDR AHDR, string stringToAdd = "_COPY", bool giveIDregardless = false)
        {
            int numCopies = -1;

            while (ContainsAsset(AHDR.assetID) | giveIDregardless)
            {
                giveIDregardless = false;
                numCopies++;

                if (AHDR.ADBG.assetName.Contains(stringToAdd))
                    AHDR.ADBG.assetName = AHDR.ADBG.assetName.Substring(0, AHDR.ADBG.assetName.LastIndexOf(stringToAdd));

                AHDR.ADBG.assetName += stringToAdd + numCopies.ToString("D3");
                AHDR.assetID = BKDRHash(AHDR.ADBG.assetName);
            }

            return AddAsset(layerIndex, AHDR);
        }

        public void RemoveAsset(IEnumerable<uint> assetIDs)
        {
            List<uint> assets = assetIDs.ToList();
            foreach (uint u in assets)
                RemoveAsset(u);
        }

        public void RemoveAsset(uint assetID)
        {
            UnselectAsset(assetID);
            CloseInternalEditor(assetID);

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                DICT.LTOC.LHDRList[i].assetIDlist.Remove(assetID);

            renderingDictionary.Remove(assetID);

            if (GetFromAssetID(assetID).AHDR.assetType == AssetType.SND | GetFromAssetID(assetID).AHDR.assetType == AssetType.SNDS)
                RemoveSoundFromSNDI(assetID);

            if (assetDictionary[assetID] is IRenderableAsset ra)
            {
                if (renderableAssetSetCommon.Contains(ra))
                    renderableAssetSetCommon.Remove(ra);
                else if (renderableAssetSetTrans.Contains(ra))
                    renderableAssetSetTrans.Remove(ra);
                else if (renderableAssetSetJSP.Contains(ra))
                    renderableAssetSetJSP.Remove((AssetJSP)ra);
            }

            if (assetDictionary[assetID] is AssetJSP jsp)
                jsp.model.Dispose();

            if (assetDictionary[assetID] is AssetMODL modl)
                modl.GetRenderWareModelFile().Dispose();

            DICT.ATOC.AHDRList.Remove(assetDictionary[assetID].AHDR);

            assetDictionary.Remove(assetID);
        }

        private List<Asset> currentlySelectedAssets = new List<Asset>();

        public bool AssetIsSelected(uint assetID)
        {
            for (int i = 0; i < currentlySelectedAssets.Count; i++)
                if (currentlySelectedAssets[i].AHDR.assetID == assetID)
                    return true;

            return false;
        }

        public void UnselectAsset(uint assetID)
        {
            for (int i = 0; i < currentlySelectedAssets.Count; i++)
            {
                if (currentlySelectedAssets[i].AHDR.assetID == assetID)
                {
                    currentlySelectedAssets[i].isSelected = false;
                    currentlySelectedAssets.RemoveAt(i);
                    return;
                }
            }
        }

        public void ClearSelectedAssets()
        {
            for (int i = 0; i < currentlySelectedAssets.Count; i++)
                currentlySelectedAssets[i].isSelected = false;

            currentlySelectedAssets.Clear();
        }

        public List<uint> GetCurrentlySelectedAssetIDs()
        {
            List<uint> selectedAssetIDs = new List<uint>();
            foreach (Asset a in currentlySelectedAssets)
                selectedAssetIDs.Add(a.AHDR.assetID);

            return selectedAssetIDs;
        }

        public void SelectAsset(uint assetID, bool add)
        {
            if (!add)
                ClearSelectedAssets();

            if (!assetDictionary.ContainsKey(assetID))
                return;

            assetDictionary[assetID].isSelected = true;
            currentlySelectedAssets.Add(assetDictionary[assetID]);

            bool updateGizmos = false;

            if (assetDictionary[assetID] is IClickableAsset ra)
            {
                if (ra is AssetDYNA dyna)
                    updateGizmos = dyna.IsRenderableClickable;
                else
                    updateGizmos = true;
            }

            if (updateGizmos)
                UpdateGizmoPosition();
            else
                ClearGizmos();
        }

        public int GetLayerFromAssetID(uint assetID)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(assetID))
                    return i;

            throw new Exception($"Asset ID {assetID.ToString("X8")} is not present in any layer.");
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

        public void OpenInternalEditor(List<uint> list)
        {
            foreach (uint u in list)
                if (assetDictionary.ContainsKey(u))
                    OpenInternalEditor(assetDictionary[u]);
        }

        private void OpenInternalEditor(Asset asset)
        {
            CloseInternalEditor(asset.AHDR.assetID);

            switch (asset.AHDR.assetType)
            {
                case AssetType.CAM:
                    internalEditors.Add(new InternalCamEditor((AssetCAM)asset, this));
                    break;
                case AssetType.DYNA:
                    internalEditors.Add(new InternalDynaEditor((AssetDYNA)asset, this));
                    break;
                case AssetType.TEXT:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                case AssetType.SND:
                case AssetType.SNDS:
                    internalEditors.Add(new InternalSoundEditor(asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this));
                    break;
            }

            internalEditors.Last().Show();
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

        public void FindWhoTargets(uint assetID)
        {
            foreach (Asset asset in assetDictionary.Values)
                if (asset.HasReference(assetID))
                    OpenInternalEditor(asset);
        }

        public void RecalculateAllMatrices()
        {
            foreach (IRenderableAsset a in renderableAssetSetCommon)
                a.CreateTransformMatrix();
            foreach (IRenderableAsset a in renderableAssetSetTrans)
                a.CreateTransformMatrix();
            foreach (AssetJSP a in renderableAssetSetJSP)
                a.CreateTransformMatrix();
        }
        
        // Gizmos
        private static Gizmo[] gizmos = new Gizmo[0];
        private static bool DrawGizmos = false;
        public static bool FinishedMovingGizmo = false;

        public static void RenderGizmos(SharpRenderer renderer)
        {
            if (DrawGizmos)
                foreach (Gizmo g in gizmos)
                    g.Draw(renderer);
        }

        public void UpdateGizmoPosition()
        {
            if (currentlySelectedAssets.Count == 0)
                UpdateGizmoPosition(new BoundingSphere());
            else
            {
                bool found = false;
                BoundingSphere bs = new BoundingSphere();

                foreach (Asset a in currentlySelectedAssets)
                {
                    if (a is IClickableAsset ica)
                    {
                        if (!found)
                        {
                            found = true;
                            bs = ica.GetGizmoCenter();
                        }
                        else
                            bs = BoundingSphere.Merge(bs, ica.GetGizmoCenter());
                    }
                }

                UpdateGizmoPosition(bs);
            }
        }
        
        private static void UpdateGizmoPosition(BoundingSphere position)
        {
            DrawGizmos = true;
            foreach (Gizmo g in gizmos)
                g.SetPosition(position);
        }

        private static void ClearGizmos()
        {
            DrawGizmos = false;
        }

        public static void GizmoSelect(Ray r)
        {
            if (!DrawGizmos)
                return;

            float dist = 1000f;
            int index = -1;

            for (int g = 0; g < gizmos.Length; g++)
            {
                float? distance = gizmos[g].IntersectsWith(r);
                if (distance != null)
                {
                    if (distance < dist)
                    {
                        dist = (float)distance;
                        index = g;
                    }
                }
            }

            if (index == -1)
                return;

            gizmos[index].isSelected = true;
        }

        public static void ScreenUnclicked()
        {
            foreach (Gizmo g in gizmos)
                g.isSelected = false;
        }

        public void MouseMoveX(SharpCamera camera, int distance)
        {
            foreach (Asset a in currentlySelectedAssets)
                if (a is IClickableAsset ra)
                {
                    if (gizmos[0].isSelected)
                    {
                        ra.PositionX += (
                            (camera.Yaw >= -360 & camera.Yaw < -270) |
                            (camera.Yaw >= -90 & camera.Yaw < 90) |
                            (camera.Yaw >= 270)) ? distance / 10f : -distance / 10f;
                        UpdateGizmoPosition();
                        FinishedMovingGizmo = true;
                    }
                    else if (gizmos[2].isSelected)
                    {
                        ra.PositionZ += (
                            (camera.Yaw >= -180 & camera.Yaw < 0) |
                            (camera.Yaw >= 180)) ? distance / 10f : -distance / 10f;
                        UpdateGizmoPosition();
                        FinishedMovingGizmo = true;
                    }
                }
        }

        public void MouseMoveY(SharpCamera camera, int distance)
        {
            foreach (Asset a in currentlySelectedAssets)
                if (a is IClickableAsset ra)
                    if (gizmos[1].isSelected)
                    {
                        ra.PositionY -= distance / 10f;
                        UpdateGizmoPosition();
                        FinishedMovingGizmo = true;
                    }
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
                        if (AHDR.assetType == AssetType.SNDI && ContainsAsset(BKDRHash("sound_info")))
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

            List<Section_LHDR> newList = new List<Section_LHDR>();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                if (LHDR.layerType == LayerType.TEXTURE)
                    newList.Add(LHDR);

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                if (LHDR.layerType != LayerType.TEXTURE)
                    newList.Add(LHDR);

            DICT.LTOC.LHDRList = newList;

            RecalculateAllMatrices();
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

        public void ExportTextureDictionary(string fileName)
        {
            ReadFileMethods.treatTexturesAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetRWTX RWTX && RWTX.AHDR.ADBG.assetName.Contains(".RW3"))
                {
                    foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(RWTX.Data))
                        if (rw is TextureDictionary_0016 td)
                            foreach (TextureNative_0015 tn in td.textureNativeList)
                            {
                                fileVersion = tn.renderWareVersion;
                                tn.textureNativeStruct.textureName = RWTX.AHDR.ADBG.assetName;
                                textNativeList.Add(tn);
                            }
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

            ReadFileMethods.treatTexturesAsByteArray = false;
        }

        public void AddTextureDictionary(string fileName)
        {
            UnsavedChanges = true;
            int layerIndex = 0;

            List<Section_LHDR> LHDRs = new List<Section_LHDR>
            {
                new Section_LHDR()
                {
                    layerType = LayerType.TEXTURE,
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                }
            };
            LHDRs.AddRange(DICT.LTOC.LHDRList);
            DICT.LTOC.LHDRList = LHDRs;

            ReadFileMethods.treatTexturesAsByteArray = true;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(fileName))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        string textureName = tn.textureNativeStruct.textureName;
                        if (!textureName.Contains(".RW3"))
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

            ReadFileMethods.treatTexturesAsByteArray = false;
        }

        public void SetAssetPositionToView(uint assetID)
        {
            Vector3 Position = Program.MainForm.renderer.Camera.Position + 2 * Program.MainForm.renderer.Camera.GetForward();

            if (GetFromAssetID(assetID) is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
            }
            else if (GetFromAssetID(assetID) is PlaceableAsset ir)
            {
                ir.PositionX = Position.X;
                ir.PositionY = Position.Y;
                ir.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.GetForward());
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.GetUp());
            }
            else if (GetFromAssetID(assetID) is AssetMRKR mrkr)
            {
                mrkr.PositionX = Position.X;
                mrkr.PositionY = Position.Y;
                mrkr.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetMVPT mvpt)
            {
                mvpt.PositionX = Position.X;
                mvpt.PositionY = Position.Y;
                mvpt.PositionZ = Position.Z;
            }
            else if (GetFromAssetID(assetID) is AssetSFX sfx)
            {
                sfx.PositionX = Position.X;
                sfx.PositionY = Position.Y;
                sfx.PositionZ = Position.Z;
            }
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

            throw new Exception("Error: could not find SNDI asset in this archive.");
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

        public static byte[] GetTemplate(AssetType assetType)
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "\\Resources\\Templates\\" + currentGame.ToString() + "\\");
            foreach (string s in files)
                if (Path.GetFileName(s) == assetType.ToString())
                    return File.ReadAllBytes(s);

            return null;
        }

        public static AssetTemplate CurrentAssetTemplate { get; set; } = AssetTemplate.None;
        public static string CurrentUserTemplate { get; set; } = "";

        public uint PlaceTemplate(Vector3 position, int layerIndex, out bool success, ref List<uint> assetIDs, string customName = "", AssetTemplate template = AssetTemplate.None)
        {
            AssetType newAssetType = AssetType.Null;

            if (template == AssetTemplate.None)
                template = CurrentAssetTemplate;

            if (template == AssetTemplate.UserTemplate)
            {
                return PlaceUserTemplate(position, layerIndex, out success, ref assetIDs);
            }

            switch (template)
            {
                case AssetTemplate.Shiny_Red:
                case AssetTemplate.Shiny_Yellow:
                case AssetTemplate.Shiny_Green:
                case AssetTemplate.Shiny_Blue:
                case AssetTemplate.Shiny_Purple:
                case AssetTemplate.Underwear:
                case AssetTemplate.Spatula:
                case AssetTemplate.Sock:
                case AssetTemplate.Spongeball:
                    newAssetType = AssetType.PKUP;
                    break;
                case AssetTemplate.WoodenTiki:
                case AssetTemplate.FloatingTiki:
                case AssetTemplate.ThunderTiki:
                case AssetTemplate.ShhhTiki:
                case AssetTemplate.StoneTiki:
                case AssetTemplate.Fodder:
                case AssetTemplate.Hammer:
                case AssetTemplate.TarTar:
                case AssetTemplate.ChompBot:
                case AssetTemplate.GLove:
                case AssetTemplate.Chuck:
                case AssetTemplate.Chuck_Trigger:
                case AssetTemplate.Monsoon:
                case AssetTemplate.Monsoon_Trigger:
                case AssetTemplate.Sleepytime:
                case AssetTemplate.Sleepytime_Moving:
                case AssetTemplate.BombBot:
                case AssetTemplate.BzztBot:
                case AssetTemplate.Slick:
                case AssetTemplate.Slick_Trigger:
                case AssetTemplate.Jellyfish_Pink:
                case AssetTemplate.Jellyfish_Blue:
                case AssetTemplate.Duplicatotron:
                    newAssetType = AssetType.VIL;
                    break;
                case AssetTemplate.Button:
                case AssetTemplate.PressurePlateWithBase:
                    newAssetType = AssetType.BUTN;
                    break;
                case AssetTemplate.TaxiStand:
                case AssetTemplate.TexasHitch:
                case AssetTemplate.PressurePlateBaseOnly:
                    newAssetType = AssetType.SIMP;
                    break;
                case AssetTemplate.EnemyMVPT:
                    newAssetType = AssetType.MVPT;
                    break;
                case AssetTemplate.SphereTrigger:
                    newAssetType = AssetType.TRIG;
                    break;
                default:
                    if (template != AssetTemplate.None)
                        MessageBox.Show("Unsupported asset template");
                    success = false;
                    return 0;
            }

            Section_AHDR newAsset = new Section_AHDR
            {
                assetType = newAssetType,
                flags = AHDRFlagsFromAssetType(newAssetType),
                data = GetTemplate(newAssetType)
            };

            if (string.IsNullOrWhiteSpace(customName))
                newAsset.ADBG = new Section_ADBG(0, template.ToString().ToUpper() + "_T001", "", 0);
            else
                newAsset.ADBG = new Section_ADBG(0, customName + "_T001", "", 0);

            Asset asset = GetFromAssetID(AddAssetWithUniqueID(layerIndex, newAsset, "_T", true));

            success = true;

            if (asset is PlaceableAsset placeableAsset)
            {
                placeableAsset.PositionX = position.X;
                placeableAsset.PositionY = position.Y;
                placeableAsset.PositionZ = position.Z;
            }

            switch (template)
            {
                case AssetTemplate.Shiny_Red:
                    ((AssetPKUP)asset).Shape = 0x3E;
                    ((AssetPKUP)asset).PickReferenceID = 0x7C8AC53E;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Yellow:
                    ((AssetPKUP)asset).Shape = 0x3B;
                    ((AssetPKUP)asset).PickReferenceID = 0xB3D6283B;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Green:
                    ((AssetPKUP)asset).Shape = 0x34;
                    ((AssetPKUP)asset).PickReferenceID = 0x079A0734;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Blue:
                    ((AssetPKUP)asset).Shape = 0x81;
                    ((AssetPKUP)asset).PickReferenceID = 0x6D4A4181;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Shiny_Purple:
                    ((AssetPKUP)asset).Shape = 0xCB;
                    ((AssetPKUP)asset).PickReferenceID = 0xFA607BCB;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Underwear:
                    ((AssetPKUP)asset).Shape = 0x13;
                    ((AssetPKUP)asset).PickReferenceID = 0x28F55613;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Spatula:
                    ((AssetPKUP)asset).Shape = 0xDD;
                    ((AssetPKUP)asset).PickReferenceID = 0x8BDFE8DD;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Sock:
                    ((AssetPKUP)asset).Shape = 0x24;
                    ((AssetPKUP)asset).PickReferenceID = 0x74B46F24;
                    ((AssetPKUP)asset).UnknownShort58 = 2;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.Spongeball:
                    ((AssetPKUP)asset).Shape = 0x15;
                    ((AssetPKUP)asset).PickReferenceID = 0xF09A1415;
                    ((AssetPKUP)asset).UnknownShort58 = 3;
                    ((AssetPKUP)asset).UnknownShort5A = 4;
                    break;
                case AssetTemplate.WoodenTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_wooden_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_wooden_bind;
                    break;
                case AssetTemplate.FloatingTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_lovey_dovey_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_lovey_dovey_bind;
                    break;
                case AssetTemplate.ThunderTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_thunder_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_thunder_bind;
                    break;
                case AssetTemplate.ShhhTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_shhhh_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_shhhh_bind;
                    break;
                case AssetTemplate.StoneTiki:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("tiki_stone_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.tiki_stone_bind;
                    break;
                case AssetTemplate.Fodder:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_fodder_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_fodder_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Hammer:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("ham_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.ham_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.TarTar:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_tar_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_tar_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.ChompBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_chomper_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_chomper_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.GLove:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("g_love_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.g_love_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Chuck:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_chuck_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Chuck_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_chuck_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_chuck_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };
                    AssetTRIG chuckTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_TRIG", AssetTemplate.SphereTrigger));
                    chuckTrigger.Position1X_Radius = 15f;
                    chuckTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Monsoon:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_4a_monsoon_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Monsoon_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_4a_monsoon_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_4a_monsoon_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG monsoonTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_TRIG", AssetTemplate.SphereTrigger));
                    monsoonTrigger.Position1X_Radius = 15f;
                    monsoonTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Sleepytime_Moving:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_sleepy-time_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Sleepytime:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_sleepy-time_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_sleepytime_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    ((AssetMVPT)GetFromAssetID(((AssetVIL)asset).AssetID_MVPT)).MovementRadius = -1;
                    break;
                case AssetTemplate.BombBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_bomb_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bomb_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.BzztBot:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_0a_bzzt_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_0a_bzzt_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Slick:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_9a_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Slick_Trigger:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("robot_9a_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.robot_9a_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    ((AssetVIL)asset).EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ScenePrepare,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    AssetTRIG slickTrigger = (AssetTRIG)GetFromAssetID(PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_TRIG", AssetTemplate.SphereTrigger));
                    slickTrigger.Position1X_Radius = 15f;
                    slickTrigger.EventsBFBB = new AssetEventBFBB[] {
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.EnterPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOn
                        },
                        new AssetEventBFBB
                        {
                            Arguments_Float = new float[6],
                            TargetAssetID = asset.AHDR.assetID,
                            EventReceiveID = EventTypeBFBB.ExitPlayer,
                            EventSendID = EventTypeBFBB.DetectPlayerOff
                        }
                    };

                    break;
                case AssetTemplate.Jellyfish_Pink:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("jellyfish_pink_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.jellyfish_pink_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Jellyfish_Blue:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("jellyfish_blue_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.jellyfish_blue_bind;
                    ((AssetVIL)asset).AssetID_MVPT = PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_MP", AssetTemplate.EnemyMVPT);
                    break;
                case AssetTemplate.Duplicatotron:
                    ((AssetVIL)asset).ModelAssetID = BKDRHash("duplicatotron1000_bind.MINF");
                    ((AssetVIL)asset).VilType = VilType.duplicatotron1000_bind;
                    break;
                case AssetTemplate.Button:
                    ((AssetBUTN)asset).RotationX = -90f;
                    ((AssetBUTN)asset).ModelAssetID = BKDRHash("button");
                    ((AssetBUTN)asset).PressedModelAssetID = BKDRHash("button_grn");
                    ((AssetBUTN)asset).UnknownByte6C = 4;
                    ((AssetBUTN)asset).UnknownByte6F = 4;
                    ((AssetBUTN)asset).UnknownByte70 = 2;
                    ((AssetBUTN)asset).UnknownByte71 = 1;
                    ((AssetBUTN)asset).UnknownByte72 = 1;
                    ((AssetBUTN)asset).PressedOffset = -0.2f;
                    ((AssetBUTN)asset).TransitionTime = 0.5f;
                    ((AssetBUTN)asset).TransitionEaseOut = 0.2f;
                    ((AssetBUTN)asset).BubbleSpin = true;
                    ((AssetBUTN)asset).BubbleBowl = true;
                    ((AssetBUTN)asset).CruiseBubble = true;
                    ((AssetBUTN)asset).Throwable = true;
                    ((AssetBUTN)asset).PatrickBelly = true;
                    ((AssetBUTN)asset).SandyMelee = true;
                    break;
                case AssetTemplate.PressurePlateWithBase:
                    ((AssetBUTN)asset).ButtonType = AssetBUTN.ButnHitMode.PressurePlate;
                    ((AssetBUTN)asset).ModelAssetID = BKDRHash("plate_pressure");
                    ((AssetBUTN)asset).PressedModelAssetID = 0xCE7F8131;
                    ((AssetBUTN)asset).UnknownByte6C = 4;
                    ((AssetBUTN)asset).UnknownByte6F = 4;
                    ((AssetBUTN)asset).UnknownByte70 = 2;
                    ((AssetBUTN)asset).UnknownByte71 = 1;
                    ((AssetBUTN)asset).UnknownByte72 = 1;
                    ((AssetBUTN)asset).PressedOffset = -0.15f;
                    ((AssetBUTN)asset).TransitionTime = 0.15f;
                    ((AssetBUTN)asset).PlayerOnPressurePlate = true;
                    ((AssetBUTN)asset).AnyThrowableOnPressurePlate = true;
                    ((AssetBUTN)asset).ThrowFruitOnPressurePlate = true;
                    ((AssetBUTN)asset).BubbleBowlOnPressurePlate = true;

                    PlaceTemplate(position, layerIndex, out success, ref assetIDs, asset.AHDR.ADBG.assetName + "_BASE", AssetTemplate.PressurePlateBaseOnly);
                    break;
                case AssetTemplate.PressurePlateBaseOnly:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("plate_pressure_base");
                    break;
                case AssetTemplate.TaxiStand:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("taxi_stand");
                    break;
                case AssetTemplate.TexasHitch:
                    ((AssetSIMP)asset).ModelAssetID = BKDRHash("trailer_hitch");
                    break;
                case AssetTemplate.EnemyMVPT:
                    ((AssetMVPT)asset).PositionX = position.X;
                    ((AssetMVPT)asset).PositionY = position.Y;
                    ((AssetMVPT)asset).PositionZ = position.Z;
                    ((AssetMVPT)asset).UnknownByte14 = 0x27;
                    ((AssetMVPT)asset).UnknownByte15 = 0x10;
                    ((AssetMVPT)asset).UnknownByte16 = 0x01;
                    ((AssetMVPT)asset).PointType = 0x00;
                    ((AssetMVPT)asset).MovementAngle = 360;
                    ((AssetMVPT)asset).MovementRadius = 4;
                    ((AssetMVPT)asset).DistanceICanSeeYou = 8;
                    break;
                case AssetTemplate.SphereTrigger:
                    ((AssetTRIG)asset).PositionX = position.X;
                    ((AssetTRIG)asset).PositionY = position.Y;
                    ((AssetTRIG)asset).PositionZ = position.Z;
                    ((AssetTRIG)asset).Position1X_Radius = 10f;
                    break;
            }

            assetIDs.Add(asset.AHDR.assetID);

            return asset.AHDR.assetID;
        }

        private uint PlaceUserTemplate(Vector3 Position, int layerIndex, out bool success, ref List<uint> assetIDs)
        {
            if (!File.Exists(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate)))
            {
                success = false;
                return 0;
            }

            string assetTypeName = CurrentUserTemplate.Substring(CurrentUserTemplate.IndexOf('[') + 1, CurrentUserTemplate.IndexOf(']') - CurrentUserTemplate.IndexOf('[') - 1);
            AssetType assetType = AssetType.Null;

            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString() == assetTypeName.Trim().ToUpper())
                {
                    assetType = o;
                    break;
                }
            }
            if (assetType == AssetType.Null) throw new Exception("Unknown asset type: " + assetType);

            Section_AHDR newAsset = new Section_AHDR
            {
                assetType = assetType,
                flags = AHDRFlagsFromAssetType(assetType),
                data = File.ReadAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, CurrentUserTemplate))
            };

            newAsset.ADBG = new Section_ADBG(0, CurrentUserTemplate.Substring(CurrentUserTemplate.IndexOf(']') + 2) + "_T001", "", 0);

            Asset asset = GetFromAssetID(AddAssetWithUniqueID(layerIndex, newAsset, "_T", true));

            success = true;

            if (asset is AssetTRIG trig)
            {
                trig.PositionX = Position.X;
                trig.PositionY = Position.Y;
                trig.PositionZ = Position.Z;

                trig.Position0X = Position.X;
                trig.Position0Y = Position.Y;
                trig.Position0Z = Position.Z;
            }
            if (asset is PlaceableAsset placeableAsset)
            {
                placeableAsset.PositionX = Position.X;
                placeableAsset.PositionY = Position.Y;
                placeableAsset.PositionZ = Position.Z;
            }
            else if (asset is AssetCAM cam)
            {
                cam.SetPosition(Program.MainForm.renderer.Camera.Position);
                cam.SetNormalizedForward(Program.MainForm.renderer.Camera.GetForward());
                cam.SetNormalizedUp(Program.MainForm.renderer.Camera.GetUp());
            }
            else if (asset is AssetMRKR mrkr)
            {
                mrkr.PositionX = Position.X;
                mrkr.PositionY = Position.Y;
                mrkr.PositionZ = Position.Z;
            }
            else if (asset is AssetMVPT mvpt)
            {
                mvpt.PositionX = Position.X;
                mvpt.PositionY = Position.Y;
                mvpt.PositionZ = Position.Z;
            }
            else if (asset is AssetSFX sfx)
            {
                sfx.PositionX = Position.X;
                sfx.PositionY = Position.Y;
                sfx.PositionZ = Position.Z;
            }

            assetIDs.Add(asset.AHDR.assetID);

            return asset.AHDR.assetID;
        }
    }
}
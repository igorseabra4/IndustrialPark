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

        public bool ContainsAssetWithType(AssetType assetType)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a.AHDR.assetType == assetType)
                    return true;

            return false;
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
                case AssetType.CRDT:
                    {
                        AssetCRDT newAsset = new AssetCRDT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CSNM:
                    {
                        AssetCSNM newAsset = new AssetCSNM(AHDR);
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
                case AssetType.FLY:
                    {
                        AssetFLY newAsset = new AssetFLY(AHDR);
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
                case AssetType.LKIT:
                    {
                        AssetLKIT newAsset = new AssetLKIT(AHDR);
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
                case AssetType.PARE:
                    {
                        AssetPARE newAsset = new AssetPARE(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PARP:
                    {
                        AssetPARP newAsset = new AssetPARP(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PARS:
                    {
                        AssetPARS newAsset = new AssetPARS(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
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
                case AssetType.DTRK:
                case AssetType.DUPC:
                case AssetType.GRSM:
                case AssetType.GUST:
                case AssetType.LITE:
                case AssetType.LOBM:
                case AssetType.NGMS:
                case AssetType.NPC:
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
                case AssetType.CSN:
                case AssetType.CSSS:
                case AssetType.CTOC:
                case AssetType.DEST:
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
    }
}
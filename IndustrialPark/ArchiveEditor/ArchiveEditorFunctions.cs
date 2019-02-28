using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using Newtonsoft.Json;
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
                    AddAssetToDictionary(AHDR, true);
                RecalculateAllMatrices();
            }

            return OK;
        }

        public void OpenFile(string fileName)
        {
            allowRender = false;

            Dispose();
            ProgressBar progressBar = new ProgressBar("Opening Archive");
            progressBar.Show();

            assetDictionary = new Dictionary<uint, Asset>();

            currentlySelectedAssets = new List<Asset>();
            currentlyOpenFilePath = fileName;

            foreach (HipSection i in HipFileToHipArray(fileName))
            {
                if (i is Section_HIPA hipa) HIPA = hipa;
                else if (i is Section_PACK pack) PACK = pack;
                else if (i is Section_DICT dict) DICT = dict;
                else if (i is Section_STRM strm) STRM = strm;
                else
                {
                    progressBar.Close();
                    throw new Exception();
                }
            }

            progressBar.SetProgressBar(0, DICT.ATOC.AHDRList.Count, 1);

            if (currentPlatform == Platform.Unknown)
                new ChoosePlatformDialog().ShowDialog();

            List<string> autoComplete = new List<string>();

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                AddAssetToDictionary(AHDR, true);

                autoComplete.Add(AHDR.ADBG.assetName);

                progressBar.PerformStep();
            }

            autoCompleteSource.AddRange(autoComplete.ToArray());

            RecalculateAllMatrices();

            progressBar.Close();

            allowRender = true;
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
            autoCompleteSource.Clear();

            List<uint> assetList = new List<uint>();
            assetList.AddRange(assetDictionary.Keys);

            if (assetList.Count == 0)
                return;

            ProgressBar progressBar = new ProgressBar("Closing Archive");
            progressBar.Show();
            progressBar.SetProgressBar(0, assetList.Count, 1);

            foreach (uint assetID in assetList)
            {
                DisposeOfAsset(assetID, true);
                progressBar.PerformStep();
            }

            HIPA = null;
            PACK = null;
            DICT = null;
            STRM = null;
            currentlyOpenFilePath = null;

            progressBar.Close();
        }

        public void DisposeForClosing()
        {
            List<uint> assetList = new List<uint>();
            assetList.AddRange(assetDictionary.Keys);

            ProgressBar progressBar = new ProgressBar("Closing Archive");
            progressBar.Show();
            progressBar.SetProgressBar(0, assetList.Count, 1);

            foreach (uint assetID in assetList)
            {
                if (assetDictionary[assetID] is AssetJSP jsp)
                    jsp.GetRenderWareModelFile().Dispose();
                else if (assetDictionary[assetID] is AssetMODL modl)
                    modl.GetRenderWareModelFile().Dispose();
                
                progressBar.PerformStep();
            }
            
            progressBar.Close();
        }

        public void DisposeOfAsset(uint assetID, bool fast = false)
        {
            currentlySelectedAssets.Remove(assetDictionary[assetID]);
            CloseInternalEditor(assetID);

            renderingDictionary.Remove(assetID);

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
                jsp.GetRenderWareModelFile().Dispose();
            if (assetDictionary[assetID] is AssetMODL modl)
                modl.GetRenderWareModelFile().Dispose();
        }

        public static bool allowRender = true;

        private void AddAssetToDictionary(Section_AHDR AHDR, bool fast = false)
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
                case AssetType.SCRP:
                    {
                        AssetSRCP newAsset = new AssetSRCP(AHDR);
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
                case AssetType.SHRP:
                    {
                        AssetSHRP newAsset = new AssetSHRP(AHDR);
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
                case AssetType.SURF:
                    {
                        AssetSURF newAsset = new AssetSURF(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
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
                case AssetType.VILP:
                    {
                        AssetVILP newAsset = new AssetVILP(AHDR);
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
                case AssetType.SDFX:
                case AssetType.SGRP:
                case AssetType.SLID:
                case AssetType.SPLN:
                case AssetType.SSET:
                case AssetType.SUBT:
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
                case AssetType.SND:
                case AssetType.SNDS:
                case AssetType.SPLP:
                case AssetType.TEXS:
                case AssetType.UIFN:
                case AssetType.WIRE:
                    {
                        Asset newAsset = new Asset(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    throw new Exception("Unknown asset type: " + AHDR.assetType);
            }

            if (hiddenAssets.Contains(AHDR.assetID))
                assetDictionary[AHDR.assetID].isInvisible = true;

            autoCompleteSource.Add(AHDR.ADBG.assetName);
            
            allowRender = true;
        }
        
        private AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();
        private TextBox textBoxFindAsset;

        public void SetTextboxForAutocomplete(TextBox textBoxFindAsset)
        {
            this.textBoxFindAsset = textBoxFindAsset;
            this.textBoxFindAsset.AutoCompleteCustomSource = autoCompleteSource;
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

        public void CreateNewAsset(int layerIndex, out bool success, out uint assetID)
        {
            Section_AHDR AHDR = AddAssetDialog.GetAsset(new AddAssetDialog(), out success, out bool setPosition);

            if (success)
            {
                try
                {
                    while (ContainsAsset(AHDR.assetID))
                    {
                        MessageBox.Show($"Archive already contains asset id [{AHDR.assetID.ToString("X8")}]. Will change it to [{(AHDR.assetID + 1).ToString("X8")}].");
                        AHDR.assetID++;
                    }
                    UnsavedChanges = true;
                    AddAsset(layerIndex, AHDR);
                    if (setPosition)
                        SetAssetPositionToView(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }
            }

            assetID = AHDR.assetID;
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
            int numCopies = 0;

            while (ContainsAsset(AHDR.assetID) | giveIDregardless)
            {
                if (numCopies > 1000)
                {
                    MessageBox.Show("Something went wrong: the asset your're trying to duplicate, paste or create a template of's name is too long. Due to that, I'll have to give it a new name myself.");
                    numCopies = 0;
                    AHDR.ADBG.assetName = "TOO_LONG";
                }

                giveIDregardless = false;
                numCopies++;

                if (AHDR.ADBG.assetName.Contains(stringToAdd))
                    AHDR.ADBG.assetName = AHDR.ADBG.assetName.Substring(0, AHDR.ADBG.assetName.LastIndexOf(stringToAdd));

                AHDR.ADBG.assetName += stringToAdd + numCopies.ToString("D2");
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
            DisposeOfAsset(assetID);
            autoCompleteSource.Remove(assetDictionary[assetID].AHDR.ADBG.assetName);

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                DICT.LTOC.LHDRList[i].assetIDlist.Remove(assetID);

            if (GetFromAssetID(assetID).AHDR.assetType == AssetType.SND | GetFromAssetID(assetID).AHDR.assetType == AssetType.SNDS)
                RemoveSoundFromSNDI(assetID);

            DICT.ATOC.AHDRList.Remove(assetDictionary[assetID].AHDR);

            assetDictionary.Remove(assetID);
        }

        public void DuplicateSelectedAssets(int layerIndex, out List<uint> finalIndices)
        {
            UnsavedChanges = true;

            finalIndices = new List<uint>();
            foreach (Asset asset in currentlySelectedAssets)
            {
                string serializedObject = JsonConvert.SerializeObject(asset.AHDR);
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                AddAssetWithUniqueID(layerIndex, AHDR);

                finalIndices.Add(AHDR.assetID);
            }
        }

        public void CopyAssetsToClipboard()
        {
            List<Section_AHDR> copiedAHDRs = new List<Section_AHDR>();

            foreach (Asset asset in currentlySelectedAssets)
            {
                Section_AHDR AHDR = asset.AHDR;

                if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                {
                    List<byte> file = new List<byte>();
                    file.AddRange(GetHeaderFromSNDI(AHDR.assetID));
                    file.AddRange(AHDR.data);

                    if (new string(new char[] { (char)file[0], (char)file[1], (char)file[2], (char)file[3] }) == "RIFF")
                    {
                        byte[] chunkSizeArr = BitConverter.GetBytes(file.Count - 8);

                        file[4] = chunkSizeArr[0];
                        file[5] = chunkSizeArr[1];
                        file[6] = chunkSizeArr[2];
                        file[7] = chunkSizeArr[3];
                    }

                    AHDR.data = file.ToArray();
                }

                copiedAHDRs.Add(AHDR);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(copiedAHDRs));
        }

        public void PasteAssetsFromClipboard(int layerIndex, out List<uint> finalIndices)
        {
            List<Section_AHDR> AHDRs;
            finalIndices = new List<uint>();

            try
            {
                AHDRs = JsonConvert.DeserializeObject<List<Section_AHDR>>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting objects from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            UnsavedChanges = true;

            foreach (Section_AHDR AHDR in AHDRs)
            {
                if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                {
                    try
                    {
                        AddSoundToSNDI(AHDR.data, AHDR.assetID, AHDR.assetType, out byte[] soundData);
                        AHDR.data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                AddAssetWithUniqueID(layerIndex, AHDR);
                finalIndices.Add(AHDR.assetID);
            }
        }

        private List<Asset> currentlySelectedAssets = new List<Asset>();

        private static List<Asset> allCurrentlySelectedAssets
        {
            get
            {
                List<Asset> currentlySelectedAssets = new List<Asset>();
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                    currentlySelectedAssets.AddRange(ae.archive.currentlySelectedAssets);
                return currentlySelectedAssets;
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

            UpdateGizmoPosition();
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
    }
}
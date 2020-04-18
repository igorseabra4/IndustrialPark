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
        public static Dictionary<uint, string> nameDictionary = new Dictionary<uint, string>();

        public static void AddToRenderingDictionary(uint key, IAssetWithModel value)
        {
            renderingDictionary[key] = value;
        }

        public static void AddToNameDictionary(uint key, string value)
        {
            nameDictionary[key] = value;
        }

        private AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();

        public void SetTextboxForAutocomplete(TextBox textBoxFindAsset)
        {
            textBoxFindAsset.AutoCompleteCustomSource = autoCompleteSource;
        }

        public bool UnsavedChanges { get; set; } = false;
        public string currentlyOpenFilePath { get; private set; }

        protected HipFile hipFile;
        protected Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public Game game => hipFile.game;
        public Platform platform => hipFile.platform;
        protected Section_DICT DICT => hipFile.DICT;

        public bool standalone;

        public bool New()
        {
            HipFile hipFile = NewArchive.GetNewArchive(out bool OK);

            if (OK)
            {
                Dispose();

                currentlySelectedAssets = new List<Asset>();
                currentlyOpenFilePath = null;
                autoCompleteSource.Clear();
                assetDictionary.Clear();

                this.hipFile = hipFile;

                if (platform == Platform.Unknown)
                    new ChoosePlatformDialog().ShowDialog();

                foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR, true);

                UnsavedChanges = true;
                RecalculateAllMatrices();
            }

            return OK;
        }

        public void OpenFile(string fileName, bool displayProgressBar, Platform platform, bool skipTexturesAndModels = false)
        {
            allowRender = false;

            Dispose();

            ProgressBar progressBar = new ProgressBar("Opening Archive");

            if (displayProgressBar)
                progressBar.Show();

            assetDictionary = new Dictionary<uint, Asset>();

            currentlySelectedAssets = new List<Asset>();
            currentlyOpenFilePath = fileName;

            try
            {
                hipFile = new HipFile(fileName);
            }
            catch (Exception e)
            {
                progressBar.Close();
                throw e;
            }

            progressBar.SetProgressBar(0, DICT.ATOC.AHDRList.Count, 1);

            if (this.platform == Platform.Unknown)
                hipFile.platform = platform;
            while (this.platform == Platform.Unknown)
                hipFile.platform = ChoosePlatformDialog.GetPlatform();
            
            List<string> autoComplete = new List<string>(DICT.ATOC.AHDRList.Count);

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                AddAssetToDictionary(AHDR, true, skipTexturesAndModels || standalone);

                autoComplete.Add(AHDR.ADBG.assetName);

                progressBar.PerformStep();
            }

            autoCompleteSource.AddRange(autoComplete.ToArray());

            if (!(skipTexturesAndModels || standalone) && ContainsAssetWithType(AssetType.RWTX))
                SetupTextureDisplay();

            RecalculateAllMatrices();

            progressBar.Close();

            allowRender = true;
        }

        public void Save(string path)
        {
            currentlyOpenFilePath = path;
            Save();
        }

        public void Save()
        {
            File.WriteAllBytes(currentlyOpenFilePath, hipFile.ToBytes());
            UnsavedChanges = false;
        }

        public bool EditPack(out HashSet<uint> unsupported)
        {
            Platform previousPlatform = platform;
            Game previousGame = game;

            NewArchive.GetExistingArchive(platform, game, hipFile.PACK.PCRT.fileDate, hipFile.PACK.PCRT.dateString,
                out bool OK, out Section_PACK PACK, out Platform newPlatform, out Game newGame);

            unsupported = new HashSet<uint>();
            
            if (OK)
            {
                hipFile.PACK = PACK;

                hipFile.platform = newPlatform;
                hipFile.game = newGame;

                if (platform == Platform.Unknown)
                    new ChoosePlatformDialog().ShowDialog();

                for (int i = 0; i < internalEditors.Count; i++)
                {
                    internalEditors[i].Close();
                    i--;
                }

                if (previousPlatform != platform || previousGame != game)
                    ConvertAllAssetTypes(previousPlatform, previousGame, out unsupported);
                UnsavedChanges = true; 
            }

            return OK;
        }

        public int GetLayerCount() => DICT.LTOC.LHDRList.Count;

        public int GetLayerType(int index) => DICT.LTOC.LHDRList[index].layerType;

        public void SetLayerType(int index, int type) => DICT.LTOC.LHDRList[index].layerType = type;

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + (game == Game.Incredibles ?
            ((LayerType_TSSM)DICT.LTOC.LHDRList[index].layerType).ToString() :
            ((LayerType_BFBB)DICT.LTOC.LHDRList[index].layerType).ToString())
            + " [" + DICT.LTOC.LHDRList[index].assetIDlist.Count() + "]";

        public List<uint> GetAssetIDsOnLayer(int index) => DICT.LTOC.LHDRList[index].assetIDlist;

        public List<Section_AHDR> GetAssetsOfType(AssetType assetType) => (from asset in assetDictionary.Values where asset.AHDR.assetType == assetType select asset.AHDR).ToList();
        
        public void AddLayer()
        {
            DICT.LTOC.LHDRList.Add(new Section_LHDR()
            {
                assetIDlist = new List<uint>(),
                LDBG = new Section_LDBG(-1)
            });
        }

        public void RemoveLayer(int index)
        {
            RemoveAsset(DICT.LTOC.LHDRList[index].assetIDlist);

            DICT.LTOC.LHDRList.RemoveAt(index);

            UnsavedChanges = true;
        }

        public void MoveLayerUp(int index)
        {
            if (index > 0)
            {
                Section_LHDR previous = DICT.LTOC.LHDRList[index - 1];
                DICT.LTOC.LHDRList[index - 1] = DICT.LTOC.LHDRList[index];
                DICT.LTOC.LHDRList[index] = previous;
                UnsavedChanges = true;
            }
        }

        public void MoveLayerDown(int index)
        {
            if (index < DICT.LTOC.LHDRList.Count - 1)
            {
                Section_LHDR post = DICT.LTOC.LHDRList[index + 1];
                DICT.LTOC.LHDRList[index + 1] = DICT.LTOC.LHDRList[index];
                DICT.LTOC.LHDRList[index] = post;
                UnsavedChanges = true;
            }
        }

        public int GetLayerFromAssetID(uint assetID)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(assetID))
                    return i;

            throw new Exception($"Asset ID {assetID.ToString("X8")} is not present in any layer.");
        }

        public void Dispose(bool showProgress = true)
        {
            autoCompleteSource.Clear();

            List<uint> assetList = new List<uint>();
            assetList.AddRange(assetDictionary.Keys);

            if (assetList.Count == 0)
                return;

            ProgressBar progressBar = new ProgressBar("Closing Archive");
            if (showProgress)
                progressBar.Show();
            progressBar.SetProgressBar(0, assetList.Count, 1);

            foreach (uint assetID in assetList)
            {
                DisposeOfAsset(assetID, true);
                progressBar.PerformStep();
            }

            hipFile = null;
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
                if (renderableAssetSetTrans.Contains(ra))
                    renderableAssetSetTrans.Remove(ra);
                if (renderableAssetSetJSP.Contains(ra))
                    renderableAssetSetJSP.Remove((AssetJSP)ra);
            }

            if (assetDictionary[assetID] is AssetJSP jsp)
                jsp.GetRenderWareModelFile().Dispose();
            else if (assetDictionary[assetID] is AssetMODL modl && modl.HasRenderWareModelFile())
                modl.GetRenderWareModelFile().Dispose();
            else if (assetDictionary[assetID] is IAssetWithModel iawm)
                iawm.MovieRemoveFromDictionary();
            else if (assetDictionary[assetID] is AssetPICK pick)
                pick.ClearDictionary();
            else if (assetDictionary[assetID] is AssetLODT lodt)
                lodt.ClearDictionary();
            else if (assetDictionary[assetID] is AssetPIPT pipt)
                pipt.ClearDictionary();
            else if (assetDictionary[assetID] is AssetRWTX rwtx)
                TextureManager.RemoveTexture(rwtx.Name);
        }

        public bool ContainsAsset(uint key)
        {
            return assetDictionary.ContainsKey(key);
        }

        public List<AssetType> AssetTypesOnArchive()
        {
            var assetTypeList = new List<AssetType>();

            foreach (Asset asset in assetDictionary.Values)
                if (!assetTypeList.Contains(asset.AHDR.assetType))
                    assetTypeList.Add(asset.AHDR.assetType);

            assetTypeList.Sort();
            return assetTypeList;
        }

        public List<AssetType> AssetTypesOnLayer(int index)
        {
            var assetTypeSet = new HashSet<AssetType>();
            foreach (uint i in DICT.LTOC.LHDRList[index].assetIDlist)
                assetTypeSet.Add(assetDictionary[i].AHDR.assetType);
            var assetTypeList = assetTypeSet.ToList();

            assetTypeList.Sort();
            return assetTypeList;
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

        public int AssetCount => assetDictionary.Values.Count;

        public static bool allowRender = true;

        private void AddAssetToDictionary(Section_AHDR AHDR, bool fast, bool skipTexturesAndModels = false)
        {
            allowRender = false;
            
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;
            try
            { 
                switch (AHDR.assetType)
                {
                    case AssetType.ANIM: newAsset = AHDR.ADBG.assetName.Contains("ATBL") ? new Asset(AHDR, game, platform) : newAsset = new AssetANIM(AHDR, game, platform); break;
                    case AssetType.ALST: newAsset = new AssetALST(AHDR, game, platform); break;
                    case AssetType.ATBL: newAsset = game == Game.Scooby ? new Asset(AHDR, game, platform) : new AssetATBL(AHDR, game, platform); break;
                    case AssetType.BSP: case AssetType.JSP:
                        if (DICT.LTOC.LHDRList[GetLayerFromAssetID(AHDR.assetID)].layerType > 9)
                            newAsset = new AssetJSP_INFO(AHDR, game, platform);
                        else
                            newAsset = skipTexturesAndModels ? new Asset(AHDR, game, platform) : new AssetJSP(AHDR, game, platform, Program.MainForm.renderer); 
                        break;
                    case AssetType.BOUL: newAsset = new AssetBOUL(AHDR, game, platform); break;
                    case AssetType.BUTN: newAsset = new AssetBUTN(AHDR, game, platform); break;
                    case AssetType.CAM: newAsset = new AssetCAM(AHDR, game, platform); break;
                    case AssetType.CNTR: newAsset = new AssetCNTR(AHDR, game, platform); break;
                    case AssetType.COLL: newAsset = new AssetCOLL(AHDR, game, platform); break;
                    case AssetType.COND: if (game == Game.Scooby) newAsset = new AssetCOND_Scooby(AHDR, game, platform); else newAsset = new AssetCOND(AHDR, game, platform); break;
                    case AssetType.CRDT: newAsset = new AssetCRDT(AHDR, game, platform); break;
                    case AssetType.CSNM: newAsset = new AssetCSNM(AHDR, game, platform); break;
                    case AssetType.DPAT: newAsset = new AssetDPAT(AHDR, game, platform); break;
                    case AssetType.DSCO: newAsset = new AssetDSCO(AHDR, game, platform); break;
                    case AssetType.DSTR: newAsset = game == Game.Scooby ? new AssetDSTR_Scooby(AHDR, game, platform) : new AssetDSTR(AHDR, game, platform); break;
                    case AssetType.DYNA: newAsset = new AssetDYNA(AHDR, game, platform); break;
                    case AssetType.EGEN: newAsset = new AssetEGEN(AHDR, game, platform); break;
                    case AssetType.ENV: newAsset = game == Game.Incredibles ? new AssetENV_TSSM(AHDR, game, platform) : new AssetENV(AHDR, game, platform); break;
                    case AssetType.FLY: newAsset = new AssetFLY(AHDR, game, platform); break;
                    case AssetType.FOG: newAsset = new AssetFOG(AHDR, game, platform); break;
                    case AssetType.GRUP: newAsset = new AssetGRUP(AHDR, game, platform); break;
                    case AssetType.GUST: newAsset = new AssetGUST(AHDR, game, platform); break;
                    case AssetType.HANG: newAsset = new AssetHANG(AHDR, game, platform); break;
                    case AssetType.JAW: newAsset = new AssetJAW(AHDR, game, platform); break;
                    case AssetType.LITE: newAsset = new AssetLITE(AHDR, game, platform); break;
                    case AssetType.LKIT: newAsset = new AssetLKIT(AHDR, game, platform); break;
                    case AssetType.LODT: newAsset = new AssetLODT(AHDR, game, platform); break;
                    case AssetType.MAPR: newAsset = new AssetMAPR(AHDR, game, platform); break;
                    case AssetType.MINF: newAsset = new AssetMINF(AHDR, game, platform); break;
                    case AssetType.MODL:
                        newAsset = skipTexturesAndModels ? new Asset(AHDR, game, platform) : new AssetMODL(AHDR, game, platform, Program.MainForm.renderer); break;
                    case AssetType.MRKR: newAsset = new AssetMRKR(AHDR, game, platform); break;
                    case AssetType.MVPT: newAsset = game == Game.Scooby ? new AssetMVPT_Scooby(AHDR, game, platform) : new AssetMVPT(AHDR, game, platform); break;
                    case AssetType.NPC: newAsset = new AssetNPC(AHDR, game, platform); break;
                    case AssetType.PARE: newAsset = new AssetPARE(AHDR, game, platform); break;
                    case AssetType.PARP: newAsset = new AssetPARP(AHDR, game, platform); break;
                    case AssetType.PARS: newAsset = new AssetPARS(AHDR, game, platform); break;
                    case AssetType.PEND: newAsset = new AssetPEND(AHDR, game, platform); break;
                    case AssetType.PICK: newAsset = new AssetPICK(AHDR, game, platform); break;
                    case AssetType.PIPT: newAsset = new AssetPIPT(AHDR, game, platform); break;
                    case AssetType.PKUP: newAsset = new AssetPKUP(AHDR, game, platform); break;
                    case AssetType.PLAT: newAsset = new AssetPLAT(AHDR, game, platform); break;
                    case AssetType.PLYR: newAsset = new AssetPLYR(AHDR, game, platform); break;
                    case AssetType.PORT: newAsset = new AssetPORT(AHDR, game, platform); break;
                    case AssetType.PRJT: newAsset = new AssetPRJT(AHDR, game, platform); break;
                    case AssetType.RWTX:
                        newAsset = skipTexturesAndModels ? new Asset(AHDR, game, platform) : new AssetRWTX(AHDR, game, platform); break;
                    case AssetType.SCRP: newAsset = new AssetSCRP(AHDR, game, platform); break;
                    case AssetType.SFX: newAsset = new AssetSFX(AHDR, game, platform); break;
                    case AssetType.SGRP: newAsset = new AssetSGRP(AHDR, game, platform); break;
                    case AssetType.SIMP: newAsset = new AssetSIMP(AHDR, game, platform); break;
                    case AssetType.SHDW: newAsset = new AssetSHDW(AHDR, game, platform); break;
                    case AssetType.SHRP: newAsset = new AssetSHRP(AHDR, game, platform); break;
                    case AssetType.SNDI:
                        if (platform == Platform.GameCube && (game == Game.BFBB || game == Game.Scooby))
                            newAsset = new AssetSNDI_GCN_V1(AHDR, game, platform);
                        else if (platform == Platform.GameCube)
                            newAsset = new AssetSNDI_GCN_V2(AHDR, game, platform);
                        else if (platform == Platform.Xbox)
                            newAsset = new AssetSNDI_XBOX(AHDR, game, platform);
                        else if (platform == Platform.PS2)
                            newAsset = new AssetSNDI_PS2(AHDR, game, platform);
                        else
                            newAsset = new Asset(AHDR, game, platform);
                        break;
                    case AssetType.SURF: newAsset = new AssetSURF(AHDR, game, platform); break;
                    case AssetType.TEXT: newAsset = new AssetTEXT(AHDR, game, platform); break;
                    case AssetType.TRIG: newAsset = new AssetTRIG(AHDR, game, platform); break;
                    case AssetType.TIMR: newAsset = new AssetTIMR(AHDR, game, platform); break;
                    case AssetType.UI: newAsset = new AssetUI(AHDR, game, platform); break;
                    case AssetType.UIFT: newAsset = new AssetUIFT(AHDR, game, platform); break;
                    case AssetType.VIL: newAsset = new AssetVIL(AHDR, game, platform); break;
                    case AssetType.VILP: newAsset = new AssetVILP(AHDR, game, platform); break;
                    case AssetType.VOLU: newAsset = new AssetVOLU(AHDR, game, platform); break;
                    case AssetType.WIRE: newAsset = new AssetWIRE(AHDR, game, platform); break;
                    case AssetType.CCRV:
                    case AssetType.DTRK:
                    case AssetType.DUPC:
                    case AssetType.GRSM:
                    case AssetType.LOBM:
                    case AssetType.NGMS:
                    case AssetType.PGRS:
                    case AssetType.RANM:
                    case AssetType.SDFX:
                    case AssetType.SLID:
                    case AssetType.SPLN:
                    case AssetType.SSET:
                    case AssetType.SUBT:
                    case AssetType.TPIK:
                    case AssetType.TRWT:
                    case AssetType.UIM:
                    case AssetType.ZLIN:
                        newAsset = new ObjectAsset(AHDR, game, platform);
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
                        newAsset = new Asset(AHDR, game, platform);
                        break;
                    default:
                        throw new Exception($"Unknown asset type ({AHDR.assetType.ToString()})");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error loading asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName}: " + ex.Message + ". Industrial Park will not be able to edit this asset.");
                newAsset = new Asset(AHDR, game, platform);
            }

            assetDictionary[AHDR.assetID] = newAsset;
            
            if (hiddenAssets.Contains(AHDR.assetID))
                assetDictionary[AHDR.assetID].isInvisible = true;

            if (!fast)
                autoCompleteSource.Add(AHDR.ADBG.assetName);
            
            allowRender = true;
        }

        public void CreateNewAsset(int layerIndex, out bool success, out uint assetID)
        {
            Section_AHDR AHDR = AssetHeader.GetAsset(new AssetHeader(), out success);
            assetID = 0;

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
                    AddAsset(layerIndex, AHDR, true);
                    SetAssetPositionToView(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }

                assetID = AHDR.assetID;
            }
        }

        public uint AddAsset(int layerIndex, Section_AHDR AHDR, bool setTextureDisplay)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            DICT.ATOC.AHDRList.Add(AHDR);
            AddAssetToDictionary(AHDR, false);

            if (setTextureDisplay && GetFromAssetID(AHDR.assetID) is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return AHDR.assetID;
        }

        public uint AddAssetWithUniqueID(int layerIndex, Section_AHDR AHDR, bool giveIDregardless = false, bool setTextureDisplay = false)
        {
            int numCopies = 0;
            char stringToAdd = '_';

            while (ContainsAsset(AHDR.assetID) || giveIDregardless)
            {
                if (numCopies > 1000)
                {
                    MessageBox.Show("Something went wrong: the asset you're trying to duplicate, paste or create a template of's name is too long. Due to that, I'll have to give it a new name myself.");
                    numCopies = 0;
                    AHDR.ADBG.assetName = AHDR.assetType.ToString();
                }

                giveIDregardless = false;
                numCopies++;

                if (AHDR.ADBG.assetName.Contains(stringToAdd))
                    try
                    {
                        int a = Convert.ToInt32(AHDR.ADBG.assetName.Split(stringToAdd).Last());
                        AHDR.ADBG.assetName = AHDR.ADBG.assetName.Substring(0, AHDR.ADBG.assetName.LastIndexOf(stringToAdd));
                    }
                    catch { }

                AHDR.ADBG.assetName += stringToAdd + numCopies.ToString("D2");
                AHDR.assetID = BKDRHash(AHDR.ADBG.assetName);
            }

            return AddAsset(layerIndex, AHDR, setTextureDisplay);
        }

        public void RemoveAsset(IEnumerable<uint> assetIDs)
        {
            List<uint> assets = assetIDs.ToList();
            foreach (uint u in assets)
                RemoveAsset(u);
        }

        public void RemoveAsset(uint assetID, bool removeSound = true)
        {
            DisposeOfAsset(assetID);
            autoCompleteSource.Remove(assetDictionary[assetID].AHDR.ADBG.assetName);

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                DICT.LTOC.LHDRList[i].assetIDlist.Remove(assetID);

            if (GetFromAssetID(assetID).AHDR.assetType == AssetType.SND || GetFromAssetID(assetID).AHDR.assetType == AssetType.SNDS)
                if (removeSound)
                    RemoveSoundFromSNDI(assetID);

            DICT.ATOC.AHDRList.Remove(assetDictionary[assetID].AHDR);

            assetDictionary.Remove(assetID);
        }

        public void DuplicateSelectedAssets(int layerIndex, out List<uint> finalIndices)
        {
            UnsavedChanges = true;

            finalIndices = new List<uint>();
            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();

            foreach (Asset asset in currentlySelectedAssets)
            {
                string serializedObject = JsonConvert.SerializeObject(asset.AHDR);
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                var previousAssetID = AHDR.assetID;

                AddAssetWithUniqueID(layerIndex, AHDR);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

                finalIndices.Add(AHDR.assetID);
            }

            if (updateReferencesOnCopy)
                UpdateReferencesOnCopy(referenceUpdate, (from Asset asset in currentlySelectedAssets select asset.AHDR).ToList());
        }

        public void CopyAssetsToClipboard()
        {
            List<Section_AHDR> copiedAHDRs = new List<Section_AHDR>();

            foreach (Asset asset in currentlySelectedAssets)
            {
                Section_AHDR AHDR = asset.AHDR;

                if (AHDR.assetType == AssetType.SND || AHDR.assetType == AssetType.SNDS)
                {
                    try
                    {
                        AHDR.data = GetSoundData(AHDR.assetID, AHDR.data);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + " The asset will be copied as it is.");
                    }
                }

                copiedAHDRs.Add(AHDR);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(new AssetClipboard(game, EndianConverter.PlatformEndianness(platform), copiedAHDRs), Formatting.None));
        }

        public static bool updateReferencesOnCopy = true;

        public void PasteAssetsFromClipboard(int layerIndex, out List<uint> finalIndices)
        {
            AssetClipboard clipboard;
            finalIndices = new List<uint>();

            try
            {
                clipboard = JsonConvert.DeserializeObject<AssetClipboard>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting assets from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            UnsavedChanges = true;

            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();

            foreach (Section_AHDR section in clipboard.assets)
            {
                Section_AHDR AHDR = ConvertAssetType(section, clipboard.endianness, EndianConverter.PlatformEndianness(platform), clipboard.game, game);
                
                uint previousAssetID = AHDR.assetID;

                AddAssetWithUniqueID(layerIndex, AHDR);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

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

                finalIndices.Add(AHDR.assetID);
            }

            if (updateReferencesOnCopy) 
                UpdateReferencesOnCopy(referenceUpdate, clipboard.assets);
        }

        public void UpdateReferencesOnCopy(Dictionary<uint, uint> referenceUpdate, List<Section_AHDR> assets)
        {
            AssetType[] dontUpdate = new AssetType[] {
                    AssetType.BSP,
                    AssetType.JSP,
                    AssetType.MODL,
                    AssetType.RWTX,
                    AssetType.SND,
                    AssetType.SNDI,
                    AssetType.SNDS,
                    AssetType.TEXT
                };

            Dictionary<uint, uint> newReferenceUpdate;

            if (EndianConverter.PlatformEndianness(platform) == Endianness.Big)
            {
                newReferenceUpdate = new Dictionary<uint, uint>();
                foreach (var key in referenceUpdate.Keys)
                {
                    newReferenceUpdate.Add(
                        BitConverter.ToUInt32(BitConverter.GetBytes(key).Reverse().ToArray(), 0),
                        BitConverter.ToUInt32(BitConverter.GetBytes(referenceUpdate[key]).Reverse().ToArray(), 0));
                }
            }
            else
                newReferenceUpdate = referenceUpdate;

            foreach (Section_AHDR section in assets)
                if (!dontUpdate.Contains(section.assetType))
                    section.data = ReplaceReferences(section.data, newReferenceUpdate);
        }

        public void ImportMultipleAssets(int layerIndex, List<Section_AHDR> AHDRs, out List<uint> assetIDs, bool overwrite)
        {
            UnsavedChanges = true;
            assetIDs = new List<uint>();

            foreach (Section_AHDR AHDR in AHDRs)
            {
                try
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

                    if (overwrite)
                    {
                        if (ContainsAsset(AHDR.assetID))
                            RemoveAsset(AHDR.assetID);
                        AddAsset(layerIndex, AHDR, setTextureDisplay: false);
                    }
                    else
                        AddAssetWithUniqueID(layerIndex, AHDR, setTextureDisplay: true);

                    assetIDs.Add(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to import asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName}: " + ex.Message);
                }
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

        public void SelectAssets(List<uint> assetIDs)
        {
            ClearSelectedAssets();

            foreach (uint assetID in assetIDs)
            {
                if (!assetDictionary.ContainsKey(assetID))
                    continue;

                assetDictionary[assetID].isSelected = true;
                currentlySelectedAssets.Add(assetDictionary[assetID]);
            }
        }

        public List<uint> GetCurrentlySelectedAssetIDs()
        {
            List<uint> selectedAssetIDs = new List<uint>();
            foreach (Asset a in currentlySelectedAssets)
                selectedAssetIDs.Add(a.AHDR.assetID);

            return selectedAssetIDs;
        }

        public void ClearSelectedAssets()
        {
            for (int i = 0; i < currentlySelectedAssets.Count; i++)
                currentlySelectedAssets[i].isSelected = false;

            currentlySelectedAssets.Clear();
        }

        public void ResetModels(SharpRenderer renderer)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetRenderWareModel model)
                    model.Setup(renderer);
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
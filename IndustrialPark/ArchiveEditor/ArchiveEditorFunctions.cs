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
            if (!renderingDictionary.ContainsKey(key))
                renderingDictionary.Add(key, value);
            else
                renderingDictionary[key] = value;
        }

        public static void AddToNameDictionary(uint key, string value)
        {
            if (!nameDictionary.ContainsKey(key))
                nameDictionary.Add(key, value);
            else
                nameDictionary[key] = value;
        }

        private AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();
        private TextBox textBoxFindAsset;

        public void SetTextboxForAutocomplete(TextBox textBoxFindAsset)
        {
            this.textBoxFindAsset = textBoxFindAsset;
            this.textBoxFindAsset.AutoCompleteCustomSource = autoCompleteSource;
        }

        public bool UnsavedChanges { get; set; } = false;
        public string currentlyOpenFilePath { get; private set; }

        private Section_HIPA HIPA;
        private Section_PACK PACK;
        private Section_DICT DICT;
        private Section_STRM STRM;
        private Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

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

        public void Save(string path)
        {
            currentlyOpenFilePath = path;
            Save();
        }

        public void Save()
        {
            HipSection[] hipFile = SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM);
            byte[] file = HipArrayToFile(hipFile);
            File.WriteAllBytes(currentlyOpenFilePath, file);
            UnsavedChanges = false;
        }

        public int GetLayerCount() => DICT.LTOC.LHDRList.Count;

        public int GetLayerType(int index) => DICT.LTOC.LHDRList[index].layerType;

        public void SetLayerType(int index, int type) => DICT.LTOC.LHDRList[index].layerType = type;

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + (currentGame == Game.Incredibles ?
            ((LayerType_TSSM)DICT.LTOC.LHDRList[index].layerType).ToString() :
            ((LayerType_BFBB)DICT.LTOC.LHDRList[index].layerType).ToString())
            + " [" + DICT.LTOC.LHDRList[index].assetIDlist.Count() + "]";

        public List<uint> GetAssetIDsOnLayer(int index) => DICT.LTOC.LHDRList[index].assetIDlist;

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
                if (renderableAssetSetTrans.Contains(ra))
                    renderableAssetSetTrans.Remove(ra);
                if (renderableAssetSetJSP.Contains(ra))
                    renderableAssetSetJSP.Remove((AssetJSP)ra);
            }

            if (assetDictionary[assetID] is AssetJSP jsp)
                jsp.GetRenderWareModelFile().Dispose();
            if (assetDictionary[assetID] is AssetMODL modl)
                if (modl.HasRenderWareModelFile())
                    modl.GetRenderWareModelFile().Dispose();
            if (assetDictionary[assetID] is IAssetWithModel iawm)
                iawm.MovieRemoveFromDictionary();
            if (assetDictionary[assetID] is AssetLODT lodt)
                lodt.ClearDictionary();
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
            List<uint> assetIDs = GetAssetIDsOnLayer(index);
            var assetTypeList = new List<AssetType>();

            for (int i = 0; i < assetIDs.Count(); i++)
                if (!assetTypeList.Contains(assetDictionary[assetIDs[i]].AHDR.assetType))
                    assetTypeList.Add(assetDictionary[assetIDs[i]].AHDR.assetType);

            assetTypeList.Sort();
            return assetTypeList;
        }

        public bool ContainsAssetWithType(AssetType assetType) => AssetTypesOnArchive().Contains(assetType);
        
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

        private void AddAssetToDictionary(Section_AHDR AHDR, bool fast = false)
        {
            allowRender = false;
            
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;

            switch (AHDR.assetType)
            {
                case AssetType.ANIM: newAsset = new AssetANIM(AHDR); break;
                case AssetType.ALST: newAsset = new AssetALST(AHDR); break;
                case AssetType.ATBL: newAsset = new AssetATBL(AHDR); break;
                case AssetType.BSP:
                case AssetType.JSP:
                    newAsset = new AssetJSP(AHDR);
                    ((AssetJSP)newAsset).Setup(Program.MainForm.renderer);
                    break;
                case AssetType.BOUL: newAsset = new AssetBOUL(AHDR); break;
                case AssetType.BUTN: newAsset = new AssetBUTN(AHDR); break;
                case AssetType.CAM: newAsset = new AssetCAM(AHDR); break;
                case AssetType.CNTR: newAsset = new AssetCNTR(AHDR); break;
                case AssetType.COLL: newAsset = new AssetCOLL(AHDR); break;
                case AssetType.COND:
                    if (currentGame == Game.Scooby)
                        newAsset = new AssetCOND_Scooby(AHDR);
                    else
                        newAsset = new AssetCOND(AHDR);
                    break;
                case AssetType.CRDT: newAsset = new AssetCRDT(AHDR); break;
                case AssetType.CSNM: newAsset = new AssetCSNM(AHDR); break;
                case AssetType.DPAT: newAsset = new AssetDPAT(AHDR); break;
                case AssetType.DSCO: newAsset = new AssetDSCO(AHDR); break;
                case AssetType.DSTR:
                    if (currentGame == Game.Scooby)
                        newAsset = new AssetDSTR_Scooby(AHDR);
                    else
                        newAsset = new AssetDSTR(AHDR);
                    break;
                case AssetType.DYNA: newAsset = new AssetDYNA(AHDR); break;
                case AssetType.EGEN: newAsset = new AssetEGEN(AHDR); break;
                case AssetType.ENV: newAsset = new AssetENV(AHDR); break;
                case AssetType.FLY: newAsset = new AssetFLY(AHDR); break;
                case AssetType.FOG: newAsset = new AssetFOG(AHDR); break;
                case AssetType.GRUP: newAsset = new AssetGRUP(AHDR); break;
                case AssetType.GUST: newAsset = new AssetGUST(AHDR); break;
                case AssetType.HANG: newAsset = new AssetHANG(AHDR); break;
                case AssetType.JAW: newAsset = new AssetJAW(AHDR); break;
                case AssetType.LITE: newAsset = new AssetLITE(AHDR); break;
                case AssetType.LKIT: newAsset = new AssetLKIT(AHDR); break;
                case AssetType.LODT: newAsset = new AssetLODT(AHDR); break;
                case AssetType.MAPR: newAsset = new AssetMAPR(AHDR); break;
                case AssetType.MINF: newAsset = new AssetMINF(AHDR); break;
                case AssetType.MODL:
                    newAsset = new AssetMODL(AHDR);
                    ((AssetMODL)newAsset).Setup(Program.MainForm.renderer);
                    break;
                case AssetType.MRKR: newAsset = new AssetMRKR(AHDR); break;
                case AssetType.MVPT:
                    if (currentGame == Game.Scooby)
                        newAsset = new AssetMVPT_Scooby(AHDR);
                    else
                        newAsset = new AssetMVPT(AHDR);
                    break;
                case AssetType.NPC: newAsset = new AssetNPC(AHDR); break;
                case AssetType.PARE: newAsset = new AssetPARE(AHDR); break;
                case AssetType.PARP: newAsset = new AssetPARP(AHDR); break;
                case AssetType.PARS: newAsset = new AssetPARS(AHDR); break;
                case AssetType.PEND: newAsset = new AssetPEND(AHDR); break;
                case AssetType.PICK: newAsset = new AssetPICK(AHDR); break;
                case AssetType.PIPT: newAsset = new AssetPIPT(AHDR); break;
                case AssetType.PKUP: newAsset = new AssetPKUP(AHDR); break;
                case AssetType.PLAT: newAsset = new AssetPLAT(AHDR); break;
                case AssetType.PLYR: newAsset = new AssetPLYR(AHDR); break;
                case AssetType.PORT: newAsset = new AssetPORT(AHDR); break;
                case AssetType.PRJT: newAsset = new AssetPRJT(AHDR); break;
                case AssetType.RWTX: newAsset = new AssetRWTX(AHDR); break;
                case AssetType.SCRP: newAsset = new AssetSCRP(AHDR); break;
                case AssetType.SFX: newAsset = new AssetSFX(AHDR); break;
                case AssetType.SGRP: newAsset = new AssetSGRP(AHDR); break;
                case AssetType.SIMP: newAsset = new AssetSIMP(AHDR); break;
                case AssetType.SHDW: newAsset = new AssetSHDW(AHDR); break;
                case AssetType.SHRP: newAsset = new AssetSHRP(AHDR); break;
                case AssetType.SNDI:
                    if (currentPlatform == Platform.GameCube && (currentGame == Game.BFBB || currentGame == Game.Scooby))
                        newAsset = new AssetSNDI_GCN_V1(AHDR);
                    else if (currentPlatform == Platform.GameCube)
                        newAsset = new AssetSNDI_GCN_V2(AHDR);
                    else if (currentPlatform == Platform.Xbox)
                        newAsset = new AssetSNDI_XBOX(AHDR);
                    else if (currentPlatform == Platform.PS2)
                        newAsset = new AssetSNDI_PS2(AHDR);
                    else
                        newAsset = new Asset(AHDR);
                    break;
                case AssetType.SURF: newAsset = new AssetSURF(AHDR); break;
                case AssetType.TEXT: newAsset = new AssetTEXT(AHDR); break;
                case AssetType.TRIG: newAsset = new AssetTRIG(AHDR); break;
                case AssetType.TIMR: newAsset = new AssetTIMR(AHDR); break;
                case AssetType.UI: newAsset = new AssetUI(AHDR); break;
                case AssetType.UIFT: newAsset = new AssetUIFT(AHDR); break;
                case AssetType.VIL: newAsset = new AssetVIL(AHDR); break;
                case AssetType.VILP: newAsset = new AssetVILP(AHDR); break;
                case AssetType.VOLU: newAsset = new AssetVOLU(AHDR); break;
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
                        newAsset = new ObjectAsset(AHDR);
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
                        newAsset = new Asset(AHDR);
                    break;
                default:
                    throw new Exception("Unknown asset type: " + AHDR.assetType);
            }

            assetDictionary.Add(AHDR.assetID, newAsset);

            // THIS IS TEMPORARY, REMOVE AFTER FINISHING ENEMY HIPS
            //if (newAsset is AssetMINF minf)
            //{
            //    uint hash = BKDRHash(AHDR.ADBG.assetName.Replace(".MINF", ""));
            //    if (assetDictionary.ContainsKey(hash))
            //        assetDictionary[hash] = newAsset;
            //    else
            //        assetDictionary.Add(hash, newAsset);
            //}
            //if (newAsset is AssetMODL modl)
            //{
            //    uint hash = BKDRHash(AHDR.ADBG.assetName.ToLower().Replace(".dff", ""));
            //    if (assetDictionary.ContainsKey(hash))
            //        assetDictionary[hash] = newAsset;
            //    else
            //        assetDictionary.Add(hash, newAsset);
            //}
            // ...

            if (hiddenAssets.Contains(AHDR.assetID))
                assetDictionary[AHDR.assetID].isInvisible = true;

            autoCompleteSource.Add(AHDR.ADBG.assetName);
            
            allowRender = true;
        }

        public void CreateNewAsset(int layerIndex, out bool success, out uint assetID)
        {
            Section_AHDR AHDR = AssetHeader.GetAsset(new AssetHeader(), out success, out bool setPosition);
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
                    AddAsset(layerIndex, AHDR);
                    if (setPosition)
                        SetAssetPositionToView(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                }

                assetID = AHDR.assetID;
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
            int numCopies = 0;

            while (ContainsAsset(AHDR.assetID) || giveIDregardless)
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

            if (GetFromAssetID(assetID).AHDR.assetType == AssetType.SND || GetFromAssetID(assetID).AHDR.assetType == AssetType.SNDS)
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
                    try
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
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + " The asset will be copied as it is.");
                    }
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
                AddAssetWithUniqueID(layerIndex, AHDR);

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
        }

        public bool ImportMultipleAssets(int layerIndex, List<Section_AHDR> AHDRs, out List<uint> assetIDs)
        {
            UnsavedChanges = true;
            assetIDs = new List<uint>();

            try
            {
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
                    assetIDs.Add(AHDR.assetID);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add asset: " + ex.Message);
                return false;
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
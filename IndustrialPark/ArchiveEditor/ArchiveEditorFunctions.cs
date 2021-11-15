using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static HashSet<IRenderableAsset> renderableAssets = new HashSet<IRenderableAsset>();
        public static void AddToRenderableAssets(IRenderableAsset ira)
        {
            lock (renderableAssets)
            {
                renderableAssets.Remove(ira);
                renderableAssets.Add(ira);
            }
        }

        public static HashSet<AssetJSP> renderableJSPs = new HashSet<AssetJSP>();
        public static void AddToRenderableJSPs(AssetJSP jsp)
        {
            lock (renderableJSPs)
            {
                renderableJSPs.Remove(jsp);
                renderableJSPs.Add(jsp);
            }
        }

        public static Dictionary<uint, IAssetWithModel> renderingDictionary = new Dictionary<uint, IAssetWithModel>();
        public static void AddToRenderingDictionary(uint assetID, IAssetWithModel value)
        {
            lock (renderingDictionary)
                renderingDictionary[assetID] = value;
        }
        public static void RemoveFromRenderingDictionary(uint assetID)
        {
            lock (renderingDictionary)
                renderingDictionary.Remove(assetID);
        }
        public static RenderWareModelFile GetFromRenderingDictionary(uint assetID)
        {
            lock (renderingDictionary)
                return renderingDictionary.ContainsKey(assetID) ? renderingDictionary[assetID].GetRenderWareModelFile() : null;
        }

        private static Dictionary<uint, string> nameDictionary = new Dictionary<uint, string>();
        public static void AddToNameDictionary(uint assetID, string value)
        {
            lock (nameDictionary)
                nameDictionary[assetID] = value;
        }
        public static void RemoveFromNameDictionary(uint assetID)
        {
            lock (nameDictionary)
                nameDictionary.Remove(assetID);
        }
        public static string GetFromNameDictionary(uint assetID)
        {
            lock (nameDictionary)
                return nameDictionary.ContainsKey(assetID) ? nameDictionary[assetID] : null;
        }
        public static void ClearNameDictionary()
        {
            lock (nameDictionary)
                nameDictionary.Clear();
        }

        public AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();

        public void SetTextboxForAutocomplete(TextBox textBoxFindAsset) =>
            textBoxFindAsset.AutoCompleteCustomSource = autoCompleteSource;

        public bool UnsavedChanges { get; set; } = false;
        public string currentlyOpenFilePath { get; private set; }
        public bool IsNull => hipFile == null;

        protected HipFile hipFile;
        protected Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public Game game;
        public Platform platform;
        protected Section_DICT DICT => hipFile.DICT;

        public bool standalone;

        public bool New()
        {
            var (hipFile, platform, game, addDefaultAssets) = NewArchive.GetNewArchive();

            if (hipFile != null)
            {
                Dispose();

                currentlySelectedAssets = new List<Asset>();
                currentlyOpenFilePath = null;
                assetDictionary.Clear();

                this.hipFile = hipFile;
                this.platform = platform;
                this.game = game;

                if (addDefaultAssets)
                    PlaceDefaultAssets();

                UnsavedChanges = true;
                RecalculateAllMatrices();

                return true;
            }

            return false;
        }

        public void OpenFile(string fileName, bool displayProgressBar, Platform platform, out string[] autoCompleteSource, bool skipTexturesAndModels = false)
        {
            Dispose();

            ProgressBar progressBar = new ProgressBar("Opening Archive");

            if (displayProgressBar)
                progressBar.Show();

            assetDictionary = new Dictionary<uint, Asset>();

            currentlySelectedAssets = new List<Asset>();
            currentlyOpenFilePath = fileName;

            try
            {
                (hipFile, game, this.platform) = HipFile.FromPath(fileName);
            }
            catch (Exception e)
            {
                progressBar.Close();
                throw e;
            }

            progressBar.SetProgressBar(0, DICT.ATOC.AHDRList.Count, 1);

            if (platform != Platform.Unknown)
                this.platform = platform;
            while (this.platform == Platform.Unknown)
                this.platform = ChoosePlatformDialog.GetPlatform();

            string assetsWithError = "";

            List<string> autoComplete = new List<string>(DICT.ATOC.AHDRList.Count);

#if DEBUG
            var tempAhdrUglyDict = new Dictionary<uint, Section_AHDR>();
#endif

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                string error = AddAssetToDictionary(AHDR, game, this.platform.Endianness(), true, skipTexturesAndModels || standalone, false);

                if (error != null)
                    assetsWithError += error + "\n";

                autoComplete.Add(AHDR.ADBG.assetName);

                progressBar.PerformStep();

#if DEBUG
                tempAhdrUglyDict[AHDR.assetID] = AHDR;
#endif
            }

            DICT.ATOC.AHDRList.Clear();

            if (assetsWithError != "")
                MessageBox.Show("There was an error loading the following assets and editing has been disabled for them:\n" + assetsWithError);

            if (!(skipTexturesAndModels || standalone) && ContainsAssetWithType(AssetType.RWTX))
                SetupTextureDisplay();

            RecalculateAllMatrices();

            autoCompleteSource = autoComplete.ToArray();

            if (!skipTexturesAndModels && ContainsAssetWithType(AssetType.PIPT) && ContainsAssetWithType(AssetType.MODL))
                foreach (var asset in assetDictionary.Values)
                    if (asset is AssetPIPT PIPT)
                        PIPT.UpdateDictionary();

            progressBar.Close();

#if DEBUG
            LogAssetOrder(tempAhdrUglyDict);
#endif
        }

        private void LogAssetOrder(Dictionary<uint, Section_AHDR> tempAhdrUglyDict)
        {
            var tempLog = new List<string>();
            var diff = false;
            foreach (var layer in DICT.LTOC.LHDRList)
            {
                tempLog.Add(layer.layerType.ToString());
                foreach (var u in layer.assetIDlist)
                {
                    var asset = GetFromAssetID(u);
                    string line = asset.assetType.ToString();
                    if (asset is AssetPLAT plat)
                        line += " " + plat.PlatformType.ToString();
                    else if (asset is AssetDYNA dyna)
                        line += " " + dyna.Type.ToString();
                    if (!tempLog.Contains(line))
                        tempLog.Add(line);
                }

                var list = layer.assetIDlist;
                var sortedList = layer.assetIDlist.OrderBy(u => tempAhdrUglyDict[u].GetCompareValue(game, this.platform)).ToList();
                if (ListsAreDifferent(list, sortedList))
                {
                    diff = true;
                    tempLog.Add("sorting failed");
                    var exp = "";
                    foreach (var l in list)
                        exp += l.ToString("X8") + " ";
                    var res = "";
                    foreach (var l in sortedList)
                        res += l.ToString("X8") + " ";
                    tempLog.Add(exp);
                    tempLog.Add(res);
                }

                tempLog.Add("");
            }

            if (diff)
                File.WriteAllLines(Path.GetFileName(currentlyOpenFilePath) + "_orderlog.txt", tempLog.ToArray());
        }

        private bool ListsAreDifferent(List<uint> list1, List<uint> list2)
        {
            if (list1.Count != list2.Count)
                return true;
            for (int i = 0; i < list1.Count; i++)
                if (list1[i] != list2[i])
                    return true;
            return false;
        }

        public void Save(string path)
        {
            currentlyOpenFilePath = path;
            Save();
        }

        public void Save()
        {
            foreach (var asset in assetDictionary.Values)
                DICT.ATOC.AHDRList.Add(asset.BuildAHDR(game, platform.Endianness()));
            File.WriteAllBytes(currentlyOpenFilePath, hipFile.ToBytes(game, platform));
            DICT.ATOC.AHDRList.Clear();
            UnsavedChanges = false;
        }

        public bool EditPack()
        {
            var (PACK, newPlatform, newGame) = NewArchive.GetExistingArchive(platform, game, hipFile.PACK.PCRT.fileDate, hipFile.PACK.PCRT.dateString);

            if (PACK != null)
            {
                hipFile.PACK = PACK;

                platform = newPlatform;
                game = newGame;

                if (platform == Platform.Unknown)
                    new ChoosePlatformDialog().ShowDialog();

                for (int i = 0; i < internalEditors.Count; i++)
                {
                    internalEditors[i].Close();
                    i--;
                }

                UnsavedChanges = true;

                return true;
            }

            return false;
        }

        public int LayerCount => DICT.LTOC.LHDRList.Count;

        public int GetLayerType(int index) => DICT.LTOC.LHDRList[index].layerType;

        public void SetLayerType(int index, int type) => DICT.LTOC.LHDRList[index].layerType = type;

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + (game == Game.Incredibles ?
            ((LayerType_TSSM)DICT.LTOC.LHDRList[index].layerType).ToString() :
            ((LayerType_BFBB)DICT.LTOC.LHDRList[index].layerType).ToString())
            + " [" + DICT.LTOC.LHDRList[index].assetIDlist.Count() + "]";

        public List<uint> GetAssetIDsOnLayer(int index) => DICT.LTOC.LHDRList[index].assetIDlist;

        public void AddLayer(int layerType = 0)
        {
            DICT.LTOC.LHDRList.Add(new Section_LHDR()
            {
                layerType = layerType,
                assetIDlist = new List<uint>(),
                LDBG = new Section_LDBG(-1)
            });

            UnsavedChanges = true;
        }

        public void RemoveLayer(int index)
        {
            foreach (uint u in DICT.LTOC.LHDRList[index].assetIDlist.ToArray())
                RemoveAsset(u);

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

            throw new Exception($"Asset ID {assetID:X8} is not present in any layer.");
        }

        public void Dispose(bool showProgress = true)
        {
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
                DisposeOfAsset(assetID);
                progressBar.PerformStep();
            }

            hipFile = null;
            currentlyOpenFilePath = null;

            progressBar.Close();
        }

        public void DisposeOfAsset(uint assetID)
        {
            var asset = assetDictionary[assetID];
            currentlySelectedAssets.Remove(asset);
            CloseInternalEditor(assetID);
            CloseInternalEditorMulti(assetID);

            renderingDictionary.Remove(assetID);

            if (asset is IRenderableAsset ra)
            {
                lock (renderableAssets)
                    renderableAssets.Remove(ra);
                if (ra is AssetJSP bsp)
                    renderableJSPs.Remove(bsp);
                if (Program.MainForm != null)
                    lock (Program.MainForm.renderer.renderableAssets)
                        Program.MainForm.renderer.renderableAssets.Remove(ra);
            }

            if (asset is AssetRenderWareModel jsp)
                jsp.GetRenderWareModelFile()?.Dispose();
            else if (asset is IAssetWithModel iawm)
                iawm.MovieRemoveFromDictionary();
            else if (asset is AssetPICK pick)
                pick.ClearDictionary();
            else if (asset is AssetTPIK tpik)
                tpik.ClearDictionary();
            else if (asset is AssetLODT lodt)
                lodt.ClearDictionary();
            else if (asset is AssetPIPT pipt)
                pipt.ClearDictionary();
            else if (asset is AssetSPLN spln)
                spln.Dispose();
            else if (asset is AssetWIRE wire)
                wire.Dispose();
            else if (asset is AssetDTRK dtrk)
                dtrk.Dispose();
            else if (asset is AssetGRSM grsm)
                grsm.Dispose();
            else if (asset is AssetRWTX rwtx)
                TextureManager.RemoveTexture(rwtx.Name, this, rwtx.assetID);
        }

        public bool ContainsAsset(uint key)
        {
            return assetDictionary.ContainsKey(key);
        }

        public List<AssetType> AssetTypesOnLayer(int index) =>
            (from uint i in DICT.LTOC.LHDRList[index].assetIDlist select assetDictionary[i].assetType).Distinct().OrderBy(f => f).ToList();

        public bool ContainsAssetWithType(AssetType assetType)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a.assetType.Equals(assetType))
                    return true;
            return false;
        }

        public Asset GetFromAssetID(uint key)
        {
            if (ContainsAsset(key))
                return assetDictionary[key];
            throw new KeyNotFoundException("Asset not present in dictionary.");
        }

        public Section_AHDR GetAHDRFromAssetID(uint key)
        {
            if (ContainsAsset(key))
                return assetDictionary[key].BuildAHDR();
            throw new KeyNotFoundException("Asset not present in dictionary.");
        }

        public Dictionary<uint, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }

        public int AssetCount => assetDictionary.Values.Count;

        private string AddAssetToDictionary(Section_AHDR AHDR, Game game, Endianness endianness, bool fast, bool skipTexturesAndModels = false, bool showMessageBox = true)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;
            string error = null;

            newAsset = CreateAsset(AHDR, game, endianness, skipTexturesAndModels, showMessageBox, ref error);

            // testing if build works
            if (fast)
            {
                var built = newAsset.BuildAHDR().data;
                if (!Enumerable.SequenceEqual(AHDR.data, built))
                {
                    error = $"[{ AHDR.assetID:X8}] { AHDR.ADBG.assetName} (unsupported format)";
                    if (showMessageBox)
                        MessageBox.Show($"There was an error loading asset " + error + " and editing has been disabled for it.");

                    newAsset = new AssetGeneric(AHDR, game, endianness);
#if DEBUG
                    string folder = "build_test" + Path.GetFileName(currentlyOpenFilePath) + "_out\\";
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    string assetName = $"[{AHDR.assetType}] {AHDR.ADBG.assetName}";
                    File.WriteAllBytes(folder + assetName + " ok", AHDR.data);
                    File.WriteAllBytes(folder + assetName + " bad", built);
#endif
                }
            }

            assetDictionary[AHDR.assetID] = newAsset;

            if (hiddenAssets.Contains(AHDR.assetID))
                assetDictionary[AHDR.assetID].isInvisible = true;

            if (!fast)
                autoCompleteSource.Add(AHDR.ADBG.assetName);

            return error;
        }

        private void AddAssetToDictionary(Asset asset, bool fast)
        {
            if (assetDictionary.ContainsKey(asset.assetID))
            {
                assetDictionary.Remove(asset.assetID);
                MessageBox.Show("Duplicate asset ID found: " + asset.assetID.ToString("X8"));
            }

            assetDictionary[asset.assetID] = asset;

            if (hiddenAssets.Contains(asset.assetID))
                assetDictionary[asset.assetID].isInvisible = true;

            if (!fast)
                autoCompleteSource.Add(asset.assetName);
        }

        private Asset CreateAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool skipTexturesAndModels, bool showMessageBox, ref string error)
        {
            try
            {
                switch (AHDR.assetType)
                {
                    case AssetType.ANIM:
                        {
                            if (AHDR.data.Length == 0)
                                return new AssetGeneric(AHDR, game, endianness);
                            var magic = AHDR.data.Take(4).ToArray();
                            if (endianness == Endianness.Big)
                                magic = magic.Reverse().ToArray();
                            if (magic[0] == 'S' && magic[1] == 'K' && magic[2] == 'B' && magic[3] == '1')
                                return new AssetANIM(AHDR, game, endianness);
                            return new AssetGeneric(AHDR, game, endianness);
                        }
                    case AssetType.BSP:
                    case AssetType.JSP:
                        if (DICT.LTOC.LHDRList[GetLayerFromAssetID(AHDR.assetID)].layerType > 9)
                            return new AssetJSP_INFO(AHDR, game, endianness);
                        if (skipTexturesAndModels)
                            return new AssetWithData(AHDR, game, endianness);
                        return new AssetJSP(AHDR, game, endianness, Program.MainForm.renderer);
                    case AssetType.MODL:
                        if (skipTexturesAndModels)
                            return new AssetWithData(AHDR, game, endianness);
                        return new AssetMODL(AHDR, game, endianness, Program.MainForm.renderer);
                    case AssetType.RWTX:
                        if (skipTexturesAndModels)
                            return new AssetWithData(AHDR, game, endianness);
                        return new AssetRWTX(AHDR, game, endianness);
                    case AssetType.SNDI:
                        if (platform == Platform.GameCube && (game == Game.BFBB || game == Game.Scooby))
                            return new AssetSNDI_GCN_V1(AHDR, game, endianness);
                        if (platform == Platform.GameCube)
                            return new AssetSNDI_GCN_V2(AHDR, game, endianness);
                        if (platform == Platform.Xbox)
                            return new AssetSNDI_XBOX(AHDR, game, endianness);
                        if (platform == Platform.PS2)
                            return new AssetSNDI_PS2(AHDR, game, endianness);
                        return new AssetGeneric(AHDR, game, endianness);
                    case AssetType.SPLN:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, endianness);
                        return new AssetSPLN(AHDR, game, endianness, Program.MainForm.renderer);
                    case AssetType.WIRE:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, endianness);
                        return new AssetWIRE(AHDR, game, endianness, Program.MainForm.renderer);
                    case AssetType.ALST: return new AssetALST(AHDR, game, endianness);
                    case AssetType.ATBL: return new AssetATBL(AHDR, game, endianness);
                    case AssetType.BOUL: return new AssetBOUL(AHDR, game, endianness);
                    case AssetType.BUTN: return new AssetBUTN(AHDR, game, endianness);
                    case AssetType.CAM: return new AssetCAM(AHDR, game, endianness);
                    case AssetType.CNTR: return new AssetCNTR(AHDR, game, endianness);
                    case AssetType.COLL: return new AssetCOLL(AHDR, game, endianness);
                    case AssetType.COND: return new AssetCOND(AHDR, game, endianness);
                    case AssetType.CRDT:
                        if (game == Game.BFBB)
                            return new AssetCRDT(AHDR, game, endianness);
                        return new AssetGeneric(AHDR, game, endianness); // unsupported CRDT for non bfbb
                    // case AssetType.CSN: return new AssetCSN(AHDR, game, platform);
                    case AssetType.CSNM: return new AssetCSNM(AHDR, game, endianness);
                    case AssetType.DEST: return new AssetDEST(AHDR, game, endianness);
                    case AssetType.DPAT: return new AssetDPAT(AHDR, game, endianness);
                    case AssetType.DSCO: return new AssetDSCO(AHDR, game, endianness);
                    case AssetType.DSTR: return new AssetDSTR(AHDR, game, endianness);
                    case AssetType.DTRK: return new AssetDTRK(AHDR, game, endianness);
                    case AssetType.DYNA: return CreateDYNA(AHDR, game, endianness);
                    case AssetType.DUPC: return new AssetDUPC(AHDR, game, endianness);
                    case AssetType.EGEN: return new AssetEGEN(AHDR, game, endianness);
                    case AssetType.ENV: return new AssetENV(AHDR, game, endianness);
                    case AssetType.FLY: return new AssetFLY(AHDR, game, endianness);
                    case AssetType.FOG: return new AssetFOG(AHDR, game, endianness);
                    case AssetType.GRUP: return new AssetGRUP(AHDR, game, endianness);
                    case AssetType.GRSM: return new AssetGRSM(AHDR, game, endianness);
                    case AssetType.GUST: return new AssetGUST(AHDR, game, endianness);
                    case AssetType.HANG: return new AssetHANG(AHDR, game, endianness);
                    case AssetType.JAW: return new AssetJAW(AHDR, game, endianness);
                    case AssetType.LITE: return new AssetLITE(AHDR, game, endianness);
                    case AssetType.LKIT: return new AssetLKIT(AHDR, game, endianness);
                    case AssetType.LOBM: return new AssetLOBM(AHDR, game, endianness);
                    case AssetType.LODT: return new AssetLODT(AHDR, game, endianness);
                    case AssetType.MAPR: return new AssetMAPR(AHDR, game, endianness);
                    case AssetType.MINF: return new AssetMINF(AHDR, game, endianness);
                    case AssetType.MRKR: return new AssetMRKR(AHDR, game, endianness);
                    case AssetType.MVPT: return new AssetMVPT(AHDR, game, endianness);
                    case AssetType.NPC: return new AssetNPC(AHDR, game, endianness);
                    case AssetType.ONEL: return new AssetONEL(AHDR, game, endianness);
                    case AssetType.PARE:
                        if (game != Game.Scooby)
                            return new AssetPARE(AHDR, game, endianness);
                        return new AssetGeneric(AHDR, game, endianness); // unsupported pare for scooby
                    case AssetType.PARP: return new AssetPARP(AHDR, game, endianness);
                    case AssetType.PARS: return new AssetPARS(AHDR, game, endianness);
                    case AssetType.PEND: return new AssetPEND(AHDR, game, endianness);
                    case AssetType.PGRS: return new AssetPGRS(AHDR, game, endianness);
                    case AssetType.PICK: return new AssetPICK(AHDR, game, endianness);
                    case AssetType.PIPT: return new AssetPIPT(AHDR, game, endianness, UpdateModelBlendModes);
                    case AssetType.PKUP: return new AssetPKUP(AHDR, game, endianness);
                    case AssetType.PLAT: return new AssetPLAT(AHDR, game, endianness);
                    case AssetType.PLYR: return new AssetPLYR(AHDR, game, endianness);
                    case AssetType.PORT: return new AssetPORT(AHDR, game, endianness);
                    case AssetType.PRJT: return new AssetPRJT(AHDR, game, endianness);
                    case AssetType.SCRP: return new AssetSCRP(AHDR, game, endianness);
                    case AssetType.SDFX: return new AssetSDFX(AHDR, game, endianness);
                    case AssetType.SFX: return new AssetSFX(AHDR, game, endianness);
                    case AssetType.SGRP: return new AssetSGRP(AHDR, game, endianness);
                    case AssetType.TRCK:
                    case AssetType.SIMP: return new AssetSIMP(AHDR, game, endianness);
                    case AssetType.SHDW: return new AssetSHDW(AHDR, game, endianness);
                    case AssetType.SHRP: return new AssetSHRP(AHDR, game, endianness);
                    case AssetType.SURF: return new AssetSURF(AHDR, game, endianness);
                    case AssetType.TEXT: return new AssetTEXT(AHDR, game, endianness);
                    case AssetType.TRIG: return new AssetTRIG(AHDR, game, endianness);
                    case AssetType.TIMR: return new AssetTIMR(AHDR, game, endianness);
                    case AssetType.TPIK: return new AssetTPIK(AHDR, game, endianness);
                    case AssetType.UI: return new AssetUI(AHDR, game, endianness);
                    case AssetType.UIFT: return new AssetUIFT(AHDR, game, endianness);
                    case AssetType.VIL: return new AssetVIL(AHDR, game, endianness);
                    case AssetType.VILP: return new AssetVILP(AHDR, game, endianness);
                    case AssetType.VOLU: return new AssetVOLU(AHDR, game, endianness);

                    case AssetType.SND:
                    case AssetType.SNDS:
                        return new AssetSound(AHDR, game, platform);

                    case AssetType.CCRV:
                    case AssetType.NGMS:
                    case AssetType.RANM:
                    case AssetType.SLID:
                    case AssetType.SSET:
                    case AssetType.TRWT:
                    case AssetType.UIM:
                    case AssetType.ZLIN:
                        return new AssetGenericBase(AHDR, game, endianness);

                    case AssetType.ATKT:
                    case AssetType.BINK:
                    case AssetType.CSSS:
                    case AssetType.CTOC:
                    case AssetType.MPHT:
                    case AssetType.NPCS:
                    case AssetType.RAW:
                    case AssetType.SPLP:
                    case AssetType.SUBT:
                    case AssetType.TEXS:
                    case AssetType.UIFN:
                        return new AssetGeneric(AHDR, game, endianness);
                    case AssetType.CSN:
                        return new AssetGeneric(AHDR, game, endianness);
                    default:
                        throw new Exception($"Unknown asset type ({AHDR.assetType})");
                }
            }
            catch (Exception ex)
            {
                error = $"[{ AHDR.assetID:X8}] {AHDR.ADBG.assetName} ({ex.Message})";

                if (showMessageBox)
                    MessageBox.Show($"There was an error loading asset {error}:" + ex.Message + " and editing has been disabled for it.");

                return new AssetGeneric(AHDR, game, endianness);
            }
        }

        private AssetDYNA CreateDYNA(Section_AHDR AHDR, Game game, Endianness endianness)
        {
            EndianBinaryReader reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = 8;
            DynaType type = (DynaType)reader.ReadUInt32();

            switch (type)
            {
                case DynaType.camera__preset: return new DynaCameraPreset(AHDR, game, endianness);
                case DynaType.Enemy__SB__BucketOTron: return new DynaEnemyBucketOTron(AHDR, game, endianness);
                case DynaType.Enemy__SB__CastNCrew: return new DynaEnemyCastNCrew(AHDR, game, endianness);
                case DynaType.Enemy__SB__Critter: return new DynaEnemyCritter(AHDR, game, endianness);
                case DynaType.Enemy__SB__Dennis: return new DynaEnemyDennis(AHDR, game, endianness);
                case DynaType.Enemy__SB__FrogFish: return new DynaEnemyFrogFish(AHDR, game, endianness);
                case DynaType.Enemy__SB__Mindy: return new DynaEnemyMindy(AHDR, game, endianness);
                case DynaType.Enemy__SB__Neptune: return new DynaEnemyNeptune(AHDR, game, endianness);
                case DynaType.Enemy__SB__Standard: return new DynaEnemyStandard(AHDR, game, endianness);
                case DynaType.Enemy__SB__SupplyCrate: return new DynaEnemySupplyCrate(AHDR, game, endianness);
                case DynaType.Enemy__SB__Turret: return new DynaEnemyTurret(AHDR, game, endianness);
                case DynaType.Incredibles__Icon: return new DynaIncrediblesIcon(AHDR, game, endianness);
                case DynaType.JSPExtraData: return new DynaJSPExtraData(AHDR, game, endianness);
                case DynaType.SceneProperties: return new DynaSceneProperties(AHDR, game, endianness);
                case DynaType.effect__Flamethrower: return new DynaEffectFlamethrower(AHDR, game, endianness);
                case DynaType.effect__LensFlareElement: return new DynaEffectLensFlare(AHDR, game, endianness);
                case DynaType.Unknown_LensFlareSomething: return new DynaEffectLensFlareSomething(AHDR, game, endianness);
                case DynaType.effect__Lightning: return new DynaEffectLightning(AHDR, game, endianness);
                case DynaType.effect__Rumble: return new DynaEffectRumble(AHDR, game, endianness);
                case DynaType.effect__RumbleSphericalEmitter: return new DynaEffectRumbleSphere(AHDR, game, endianness);
                case DynaType.effect__ScreenFade: return new DynaEffectScreenFade(AHDR, game, endianness);
                case DynaType.effect__Splash: return new DynaEffectSplash(AHDR, game, endianness);
                case DynaType.effect__grass: return new DynaEffectGrass(AHDR, game, endianness);
                case DynaType.effect__smoke_emitter: return new DynaEffectSmokeEmitter(AHDR, game, endianness);
                case DynaType.effect__spotlight: return new DynaEffectSpotlight(AHDR, game, endianness);
                case DynaType.effect__uber_laser: return new DynaEffectUberLaser(AHDR, game, endianness);
                case DynaType.effect__water_body: return new DynaEffectWaterBody(AHDR, game, endianness);
                case DynaType.Effect__particle_generator: return new DynaEffectParticleGenerator(AHDR, game, endianness);
                case DynaType.game_object__BoulderGenerator: return new DynaGObjectBoulderGen(AHDR, game, endianness);
                case DynaType.game_object__BusStop: return new DynaGObjectBusStop(AHDR, game, endianness);
                case DynaType.game_object__Camera_Tweak: return new DynaGObjectCamTweak(AHDR, game, endianness);
                case DynaType.game_object__Flythrough: return new DynaGObjectFlythrough(AHDR, game, endianness);
                case DynaType.game_object__Grapple: return new DynaGObjectGrapple(AHDR, game, endianness);
                case DynaType.game_object__Hangable: return new DynaGObjectHangable(AHDR, game, endianness);
                case DynaType.game_object__IN_Pickup: return new DynaGObjectInPickup(AHDR, game, endianness);
                case DynaType.game_object__NPCSettings: return new DynaGObjectNPCSettings(AHDR, game, endianness);
                case DynaType.game_object__RaceTimer: return new DynaGObjectRaceTimer(AHDR, game, endianness);
                case DynaType.game_object__Ring: return new DynaGObjectRing(AHDR, game, endianness);
                case DynaType.game_object__RingControl: return new DynaGObjectRingControl(AHDR, game, endianness);
                case DynaType.game_object__Taxi: return new DynaGObjectTaxi(AHDR, game, endianness);
                case DynaType.game_object__Teleport: return new DynaGObjectTeleport(AHDR, game, endianness);
                case DynaType.game_object__Turret: return new DynaGObjectTurret(AHDR, game, endianness);
                case DynaType.game_object__Vent: return new DynaGObjectVent(AHDR, game, endianness);
                case DynaType.game_object__VentType: return new DynaGObjectVentType(AHDR, game, endianness);
                case DynaType.game_object__bungee_drop: return new DynaGObjectBungeeDrop(AHDR, game, endianness);
                case DynaType.game_object__bungee_hook: return new DynaGObjectBungeeHook(AHDR, game, endianness);
                case DynaType.game_object__camera_param_asset: return new DynaGObjectCameraParamAsset(AHDR, game, endianness);
                case DynaType.game_object__dash_camera_spline: return new DynaGObjectDashCameraSpline(AHDR, game, endianness);
                case DynaType.game_object__flame_emitter: return new DynaGObjectFlameEmitter(AHDR, game, endianness);
                case DynaType.game_object__laser_beam: return new DynaGObjectLaserBeam(AHDR, game, endianness);
                case DynaType.game_object__talk_box: return new DynaGObjectTalkBox(AHDR, game, endianness);
                case DynaType.game_object__task_box: return new DynaGObjectTaskBox(AHDR, game, endianness);
                case DynaType.game_object__text_box: return new DynaGObjectTextBox(AHDR, game, endianness);
                case DynaType.game_object__train_car: return new DynaGObjectTrainCar(AHDR, game, endianness);
                case DynaType.game_object__train_junction: return new DynaGObjectTrainJunction(AHDR, game, endianness);
                case DynaType.hud__image: return new DynaHudImage(AHDR, game, endianness);
                case DynaType.hud__meter__font: return new DynaHudMeterFont(AHDR, game, endianness);
                case DynaType.hud__meter__unit: return new DynaHudMeterUnit(AHDR, game, endianness);
                case DynaType.hud__model: return new DynaHudModel(AHDR, game, endianness);
                case DynaType.hud__text: return new DynaHudText(AHDR, game, endianness);
                case DynaType.interaction__Launch: return new DynaInteractionLaunch(AHDR, game, endianness);
                case DynaType.interaction__Lift: return new DynaInteractionLift(AHDR, game, endianness);
                case DynaType.interaction__Turn: return new DynaInteractionTurn(AHDR, game, endianness);
                case DynaType.logic__reference: return new DynaLogicReference(AHDR, game, endianness);
                case DynaType.logic__FunctionGenerator: return new DynaLogicFunctionGenerator(AHDR, game, endianness);
                case DynaType.npc__group: return new DynaNPCGroup(AHDR, game, endianness);
                case DynaType.pointer: return new DynaPointer(AHDR, game, endianness);
                case DynaType.ui__box: return new DynaUIBox(AHDR, game, endianness);
                case DynaType.ui__controller: return new DynaUIController(AHDR, game, endianness);
                case DynaType.ui__image: return new DynaUIImage(AHDR, game, endianness);
                case DynaType.ui__model: return new DynaUIModel(AHDR, game, endianness);
                case DynaType.ui__text: return new DynaUIText(AHDR, game, endianness);
                case DynaType.ui__text__userstring: return new DynaUITextUserString(AHDR, game, endianness);
                case DynaType.Checkpoint:
                case DynaType.Enemy__SB:
                case DynaType.Interest_Pointer:
                case DynaType.audio__conversation:
                case DynaType.camera__binary_poi:
                case DynaType.camera__transition_path:
                case DynaType.camera__transition_time:
                case DynaType.effect__BossBrain:
                case DynaType.effect__LightEffectFlicker:
                case DynaType.effect__LightEffectStrobe:
                case DynaType.effect__RumbleBoxEmitter:
                case DynaType.effect__ScreenWarp:
                case DynaType.effect__Waterhose:
                case DynaType.effect__light:
                case DynaType.effect__spark_emitter:
                case DynaType.game_object__FreezableObject:
                case DynaType.game_object__RubbleGenerator: // incredibles
                case DynaType.game_object__bullet_mark: // incredibles
                case DynaType.game_object__bullet_time: // incredibles
                case DynaType.game_object__rband_camera_asset: // incredibles
                case DynaType.interaction__IceBridge:
                case DynaType.interaction__SwitchLever:
                case DynaType.npc__CoverPoint:
                case DynaType.npc__NPC_Custom_AV:
                case DynaType.Enemy__IN2__BossUnderminerDrill:
                case DynaType.Enemy__IN2__Rat:
                case DynaType.Enemy__IN2__Chicken:
                case DynaType.Enemy__IN2__Humanoid:
                case DynaType.Enemy__IN2__RobotTank:
                case DynaType.Enemy__IN2__Bomber:
                case DynaType.Enemy__IN2__BossUnderminerUM:
                case DynaType.Enemy__IN2__Driller:
                case DynaType.Enemy__IN2__Enforcer:
                case DynaType.Enemy__IN2__Scientist:
                case DynaType.Enemy__IN2__Shooter:
                case DynaType.Unknown_EBC04E7B: // incredibles
                case DynaType.Null:
                    return new DynaGeneric(AHDR, type, game, endianness);
                default:
                    throw new Exception("Unknown DYNA type: " + type.ToString("X8"));
            }
        }

        public uint? CreateNewAsset(int layerIndex)
        {
            Section_AHDR AHDR = AssetHeader.GetAsset(new AssetHeader());

            if (AHDR != null)
            {
#if !DEBUG
                try
                {
#endif
                while (ContainsAsset(AHDR.assetID))
                    MessageBox.Show($"Archive already contains asset id [{AHDR.assetID:X8}]. Will change it to [{++AHDR.assetID:X8}].");

                UnsavedChanges = true;
                AddAsset(layerIndex, AHDR, game, platform.Endianness(), true);
                SetAssetPositionToView(AHDR.assetID);
#if !DEBUG
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to add asset: " + ex.Message);
                    return null;
                }
#endif
                return AHDR.assetID;
            }

            return null;
        }

        public uint AddAsset(int layerIndex, Section_AHDR AHDR, Game game, Endianness endianness, bool setTextureDisplay)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            AddAssetToDictionary(AHDR, game, endianness, false);

            if (setTextureDisplay && GetFromAssetID(AHDR.assetID) is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return AHDR.assetID;
        }

        public uint AddAsset(int layerIndex, Asset asset, bool setTextureDisplay)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(asset.assetID);
            AddAssetToDictionary(asset, false);

            if (setTextureDisplay && asset is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return asset.assetID;
        }

        public uint AddAssetWithUniqueID(int layerIndex, Section_AHDR AHDR, Game game, Endianness endianness, bool giveIDregardless = false, bool setTextureDisplay = false, bool ignoreNumber = false)
        {
            var assetName = GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, giveIDregardless, ignoreNumber);

            AHDR.ADBG.assetName = assetName;
            AHDR.assetID = BKDRHash(assetName);

            return AddAsset(layerIndex, AHDR, game, endianness, setTextureDisplay);
        }

        private string GetUniqueAssetName(string assetName, uint assetID, bool giveIDregardless, bool ignoreNumber)
        {
            int numCopies = 0;
            char stringToAdd = '_';

            while (ContainsAsset(assetID) || giveIDregardless)
            {
                if (numCopies > 10000)
                {
                    MessageBox.Show("There was a problem naming the placed template. It will be given a generic name (ASSET_XX)");
                    numCopies = 0;
                    ignoreNumber = false;
                    assetName = "ASSET_01";
                }

                giveIDregardless = false;
                numCopies++;

                if (!ignoreNumber)
                    assetName = FindNewAssetName(assetName, stringToAdd, numCopies);

                assetID = BKDRHash(assetName);
            }

            return assetName;
        }

        private string FindNewAssetName(string previousName, char stringToAdd, int numCopies)
        {
            if (previousName.Contains(stringToAdd))
                try
                {
                    int a = Convert.ToInt32(previousName.Split(stringToAdd).Last());
                    previousName = previousName.Substring(0, previousName.LastIndexOf(stringToAdd));
                }
                catch { }

            previousName += stringToAdd + numCopies.ToString("D2");
            return previousName;
        }

        public void RemoveAsset(IEnumerable<uint> assetIDs)
        {
            foreach (uint u in assetIDs)
                RemoveAsset(u);
        }

        public void RemoveAsset(Asset asset)
        {
            RemoveAsset(asset.assetID);
        }

        public void RemoveAsset(uint assetID, bool removeSound = true)
        {
            DisposeOfAsset(assetID);
            autoCompleteSource.Remove(assetDictionary[assetID].assetName);

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                DICT.LTOC.LHDRList[i].assetIDlist.Remove(assetID);

            if (removeSound && (GetFromAssetID(assetID).assetType.ToString().Contains("SND")))
                RemoveSoundFromSNDI(assetID);

            assetDictionary.Remove(assetID);
        }

        public void DuplicateSelectedAssets(int layerIndex, out List<uint> finalIndices)
        {
            UnsavedChanges = true;

            finalIndices = new List<uint>();
            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();
            var newAHDRs = new List<Section_AHDR>();

            foreach (var asset in currentlySelectedAssets)
            {
                string serializedObject = JsonConvert.SerializeObject(asset.BuildAHDR());
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                var previousAssetID = AHDR.assetID;

                AddAssetWithUniqueID(layerIndex, AHDR, asset.game, asset.endianness);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

                finalIndices.Add(AHDR.assetID);
                newAHDRs.Add(AHDR);
            }

            if (updateReferencesOnCopy)
                UpdateReferencesOnCopy(referenceUpdate, newAHDRs);
        }

        public void CopyAssetsToClipboard()
        {
            List<Game> copiedGames = new List<Game>();
            List<Endianness> copiedEndiannesses = new List<Endianness>();
            List<Section_AHDR> copiedAHDRs = new List<Section_AHDR>();

            foreach (Asset asset in currentlySelectedAssets)
            {
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(JsonConvert.SerializeObject(asset.BuildAHDR()));

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

                copiedGames.Add(asset.game);
                copiedEndiannesses.Add(asset.endianness);
                copiedAHDRs.Add(AHDR);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(new AssetClipboard(copiedGames, copiedEndiannesses, copiedAHDRs), Formatting.Indented));
        }

        public static bool updateReferencesOnCopy = true;
        public static bool replaceAssetsOnPaste = false;

        public void PasteAssetsFromClipboard(int layerIndex, out List<uint> finalIndices, AssetClipboard clipboard = null, bool forceRefUpdate = false, bool dontReplace = false)
        {
            finalIndices = new List<uint>();

            try
            {
                if (clipboard == null)
                    clipboard = JsonConvert.DeserializeObject<AssetClipboard>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting assets from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            UnsavedChanges = true;

            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();

            for (int i = 0; i < clipboard.assets.Count; i++)
            {
                Section_AHDR AHDR = clipboard.assets[i];

                uint previousAssetID = AHDR.assetID;

                if (replaceAssetsOnPaste && !dontReplace && ContainsAsset(AHDR.assetID))
                    RemoveAsset(AHDR.assetID);

                AddAssetWithUniqueID(layerIndex, clipboard.assets[i], clipboard.games[i], clipboard.endiannesses[i]);

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

            if (updateReferencesOnCopy || forceRefUpdate)
                UpdateReferencesOnCopy(referenceUpdate, clipboard.assets);

            for (int i = 0; i < clipboard.assets.Count; i++)
            {
                var AHDR = clipboard.assets[i];
                string error = "";
                assetDictionary[AHDR.assetID] = CreateAsset(AHDR, clipboard.games[i], clipboard.endiannesses[i], false, true, ref error);
            }
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

            if (platform.Endianness() == Endianness.Big)
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

        public List<uint> ImportMultipleAssets(int layerIndex, List<Section_AHDR> AHDRs, bool overwrite)
        {
            UnsavedChanges = true;
            var assetIDs = new List<uint>();

            foreach (Section_AHDR AHDR in AHDRs)
            {
                try
                {
                    if (overwrite)
                    {
                        if (ContainsAsset(AHDR.assetID))
                            RemoveAsset(AHDR.assetID);
                        AddAsset(layerIndex, AHDR, game, platform.Endianness(), setTextureDisplay: false);
                    }
                    else
                        AddAssetWithUniqueID(layerIndex, AHDR, game, platform.Endianness(), setTextureDisplay: true);

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

                    assetIDs.Add(AHDR.assetID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to import asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName}: " + ex.Message);
                }
            }

            return assetIDs;
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
                selectedAssetIDs.Add(a.assetID);

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
            lock (renderableAssets)
                foreach (IRenderableAsset a in renderableAssets)
                    a.CreateTransformMatrix();
            lock (renderableJSPs)
                foreach (AssetJSP a in renderableJSPs)
                    a.CreateTransformMatrix();
        }

        public void UpdateModelBlendModes(Dictionary<uint, (uint, BlendFactorType, BlendFactorType)[]> blendModes)
        {
            foreach (var asset in assetDictionary.Values)
                if (asset is AssetMODL MODL)
                    MODL.ResetBlendModes();

            if (blendModes != null)
            {
                foreach (var k in blendModes.Keys)
                    if (renderingDictionary.ContainsKey(k) && renderingDictionary[k] is AssetMODL MODL)
                        MODL.SetBlendModes(blendModes[k]);
            }
        }
    }
}
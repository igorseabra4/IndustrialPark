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

        private static readonly Dictionary<uint, string> nameDictionary = new Dictionary<uint, string>();
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

        private bool _unsavedChanges;
        public bool UnsavedChanges {
            get
            {
                return _unsavedChanges;
            }
            set { 
                _unsavedChanges = value;
                OnChangesMade();
            } 
        }

        public event Action ChangesMade;

        protected virtual void OnChangesMade()
        {
            ChangesMade?.Invoke();
        }

        public string currentlyOpenFilePath { get; private set; }

        protected Section_PACK PACK;

        protected List<Layer> Layers;

        protected Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public Game game;
        public Platform platform;

        public bool standalone;

        public bool New()
        {
            var getNewArchive = NewArchive.GetNewArchive();

            if (getNewArchive.HasValue)
            {
                Dispose();

                currentlySelectedAssets = new List<Asset>();
                currentlyOpenFilePath = null;
                assetDictionary.Clear();

                PACK = getNewArchive.Value.PACK;

                Layers = new List<Layer>();
                platform = getNewArchive.Value.platform;
                game = getNewArchive.Value.game;

                if (getNewArchive.Value.addDefaultAssets)
                    PlaceDefaultAssets();

                if (getNewArchive.Value.noLayers)
                    NoLayers = true;

                UnsavedChanges = true;
                RecalculateAllMatrices();

                return true;
            }

            return false;
        }

        public void OpenFile(string fileName, bool displayProgressBar, Platform scoobyPlatform, out string[] autoCompleteSource, bool skipTexturesAndModels = false)
        {
            Dispose();

            ProgressBar progressBar = new ProgressBar("Opening " + Path.GetFileName(fileName));

            if (displayProgressBar)
                progressBar.Show();

            assetDictionary = new Dictionary<uint, Asset>();

            currentlySelectedAssets = new List<Asset>();
            currentlyOpenFilePath = fileName;

            HipFile hipFile;
            Game game;
            Platform platformFromFile;

            try
            {
                (hipFile, game, platformFromFile) = HipFile.FromPath(fileName);
            }
            catch (Exception e)
            {
                progressBar.Close();
                throw e;
            }

            if (hipFile.HIPB != null && hipFile.HIPB.HasNoLayers != 0)
                NoLayers = true;

            progressBar.SetProgressBar(0, hipFile.DICT.ATOC.AHDRList.Count, 1);

            this.game = game;            
            platform = (scoobyPlatform != Platform.Unknown) ? scoobyPlatform : platformFromFile;

            while (platform == Platform.Unknown)
                platform = ChoosePlatformDialog.GetPlatform();

            string assetsWithError = "";

            List<string> autoComplete = new List<string>(hipFile.DICT.ATOC.AHDRList.Count);

#if DEBUG
            var tempAhdrUglyDict = new Dictionary<uint, Section_AHDR>();
#endif

            PACK = hipFile.PACK;

            if (!NoLayers)
            {
                Layers = new List<Layer>();
                foreach (Section_LHDR LHDR in hipFile.DICT.LTOC.LHDRList)
                    Layers.Add(LHDRToLayer(LHDR));
            }

            foreach (Section_AHDR AHDR in hipFile.DICT.ATOC.AHDRList)
            {
                string error = AddAssetToDictionary(AHDR, game, platform.Endianness(), true, false);

                if (error != null)
                    assetsWithError += error + "\n";

                autoComplete.Add(AHDR.ADBG.assetName);

                progressBar.PerformStep();

#if DEBUG
                tempAhdrUglyDict[AHDR.assetID] = AHDR;
#endif
            }

            if (assetsWithError != "")
                MessageBox.Show("There was an error loading the following assets and editing has been disabled for them:\n" + assetsWithError);

            if (!(skipTexturesAndModels || standalone) && ContainsAssetWithType(AssetType.Texture))
            {
                SetupTextureDisplay();
            }

            RecalculateAllMatrices();

            autoCompleteSource = autoComplete.ToArray();

            if (!skipTexturesAndModels && ContainsAssetWithType(AssetType.PipeInfoTable) && ContainsAssetWithType(AssetType.Model))
                foreach (var asset in assetDictionary.Values)
                    if (asset is AssetPIPT PIPT)
                        PIPT.UpdateDictionary();

            progressBar.Close();

#if DEBUG
            LogAssetOrder(hipFile.DICT, tempAhdrUglyDict);
#endif
        }

        private Layer LHDRToLayer(Section_LHDR LHDR)
        {
            var layer = new Layer(LayerTypeSpecificToGeneric(LHDR.layerType));
            foreach (var u in LHDR.assetIDlist)
                layer.AssetIDs.Add(u);
            return layer;
        }

        private void LogAssetOrder(Section_DICT DICT, Dictionary<uint, Section_AHDR> tempAhdrUglyDict)
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
                var sortedList = layer.assetIDlist.OrderBy(u => tempAhdrUglyDict[u].GetCompareValue(game, platform)).ToList();
                if (!Enumerable.SequenceEqual(list, sortedList))
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

        public void Save(string path)
        {
            currentlyOpenFilePath = path;
            Save();
        }

        public void Save()
        {
            File.WriteAllBytes(currentlyOpenFilePath, BuildHipFile().ToBytes(game, platform));
            UnsavedChanges = false;
        }

        private HipFile BuildHipFile()
        {
            var DICT = new Section_DICT();

            foreach (var asset in assetDictionary.Values)
                DICT.ATOC.AHDRList.Add(asset.BuildAHDR(game, platform.Endianness()));

            foreach (var layer in NoLayers ? BuildLayers() : Layers)
                DICT.LTOC.LHDRList.Add(new Section_LHDR()
                {
                    layerType = LayerTypeGenericToSpecific(layer.Type),
                    assetIDlist = layer.AssetIDs,
                    LDBG = new Section_LDBG(-1)
                });

            return new HipFile(new Section_HIPA(), PACK, DICT, new Section_STRM())
            {
                HIPB = NoLayers ? new Section_HIPB() { HasNoLayers = 1 } : null
            };
        }

        private int LayerTypeGenericToSpecific(LayerType layerType)
        {
            if (game == Game.Incredibles || layerType < LayerType.BSP)
                return (int)layerType;
            return (int)layerType - 1;
        }

        private LayerType LayerTypeSpecificToGeneric(int layerType)
        {
            if (game == Game.Incredibles || layerType < 2)
                return (LayerType)layerType;
            return (LayerType)(layerType + 1);
        }

        public bool EditPack()
        {
            var (PACK, newPlatform, newGame) = NewArchive.GetExistingArchive(platform, game, this.PACK.PCRT.fileDate, this.PACK.PCRT.dateString);

            if (PACK != null)
            {
                this.PACK = PACK;

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

        private bool _noLayers = false;
        public bool NoLayers
        {
            get => _noLayers;
            set
            {
                _noLayers = value;
                if (_noLayers)
                {
                    Layers = new List<Layer>();
                }
                else
                {
                    Layers = BuildLayers();
                }
                UnsavedChanges = true;
            }
        }

        public int SelectedLayerIndex = -1;

        public int LayerCount => Layers.Count;

        public int GetLayerType() => LayerTypeGenericToSpecific(Layers[SelectedLayerIndex].Type);

        public void SetLayerType(int type) => Layers[SelectedLayerIndex].Type = LayerTypeSpecificToGeneric(type);

        public string LayerToString() => LayerToString(SelectedLayerIndex);

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + Layers[index].Type.ToString()
            + " [" + Layers[index].AssetIDs.Count() + "]";

        public List<uint> GetAssetIDsOnLayer() => NoLayers ?
            (from Asset a in assetDictionary.Values select a.assetID).ToList() :
            Layers[SelectedLayerIndex].AssetIDs;

        public void AddLayer(LayerType layerType = LayerType.DEFAULT)
        {
            if (NoLayers)
                return;

            Layers.Add(new Layer(layerType));

            SelectedLayerIndex = Layers.Count - 1;

            UnsavedChanges = true;
        }

        public void RemoveLayer()
        {
            if (NoLayers)
                return;

            foreach (uint u in Layers[SelectedLayerIndex].AssetIDs.ToArray())
                RemoveAsset(u);

            Layers.RemoveAt(SelectedLayerIndex);

            SelectedLayerIndex--;

            UnsavedChanges = true;
        }

        public void MoveLayerUp()
        {
            if (NoLayers)
                return;

            if (SelectedLayerIndex > 0)
            {
                var previous = Layers[SelectedLayerIndex - 1];
                Layers[SelectedLayerIndex - 1] = Layers[SelectedLayerIndex];
                Layers[SelectedLayerIndex] = previous;
                UnsavedChanges = true;
            }
        }

        public void MoveLayerDown()
        {
            if (NoLayers)
                return;

            if (SelectedLayerIndex < Layers.Count - 1)
            {
                var post = Layers[SelectedLayerIndex + 1];
                Layers[SelectedLayerIndex + 1] = Layers[SelectedLayerIndex];
                Layers[SelectedLayerIndex] = post;
                UnsavedChanges = true;
            }
        }

        public int GetLayerFromAssetID(uint assetID)
        {
            if (NoLayers)
                return -1;

            for (int i = 0; i < Layers.Count; i++)
                if (Layers[i].AssetIDs.Contains(assetID))
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

            Layers.Clear();
            assetDictionary.Clear();

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
                if (Program.Renderer != null)
                    lock (Program.Renderer.renderableAssets)
                        Program.Renderer.renderableAssets.Remove(ra);
            }

            if (asset is AssetRenderWareModel jsp)
                jsp.GetRenderWareModelFile()?.Dispose();
            else if (asset is IAssetWithModel iawm)
                iawm.RemoveFromDictionary();
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

        public bool ContainsAsset(uint key) => assetDictionary.ContainsKey(key);

        public IEnumerable<AssetType> AssetTypesOnArchive() =>
            (from Asset asset in assetDictionary.Values select asset.assetType).Distinct();

        public IEnumerable<AssetType> ScalableAssetTypesOnArchive() =>
            (from Asset asset in assetDictionary.Values where
            asset is IClickableAsset ||
            asset is AssetJSP ||
            asset is AssetJSP_INFO ||
            asset is AssetLODT ||
            asset is AssetFLY
            select asset.assetType).Distinct();

        public List<AssetType> AssetTypesOnLayer() => NoLayers ?
            (from Asset a in assetDictionary.Values select a.assetType).Distinct().ToList():
            (from uint a in Layers[SelectedLayerIndex].AssetIDs select assetDictionary[a].assetType).Distinct().ToList();

        public bool ContainsAssetWithType(AssetType assetType) =>
            assetDictionary.Values.Any(a => a.assetType.Equals(assetType));

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

        private string AddAssetToDictionary(Section_AHDR AHDR, Game game, Endianness endianness, bool fast, bool showMessageBox)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;
            string error = null;

            newAsset = TryCreateAsset(AHDR, game, endianness, showMessageBox, ref error);

            // testing if build works
            if (fast)
            {
                var built = newAsset.BuildAHDR().data;
                if (!Enumerable.SequenceEqual(AHDR.data, built))
                {
                    error = $"[{AHDR.assetID:X8}] {AHDR.ADBG.assetName} (unsupported format)";
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

        private Asset TryCreateAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool showMessageBox, ref string error)
        {
            try
            {
                return CreateAsset(AHDR, game, endianness, showMessageBox, ref error);
            }
            catch (Exception ex)
            {
                //#if DEBUG
                //                throw ex;
                //#endif
                error = $"[{AHDR.assetID:X8}] {AHDR.ADBG.assetName} ({ex.Message})";

                if (showMessageBox)
                    MessageBox.Show($"There was an error loading asset {error}:" + ex.Message + " and editing has been disabled for it.");

                return new AssetGeneric(AHDR, game, endianness);
            }
        }

        private Asset CreateAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool showMessageBox, ref string error)
        {
            if (AHDR.IsDyna)
                return CreateDYNA(AHDR, game, endianness);

            switch (AHDR.assetType)
            {
                case AssetType.Animation:
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
                    return new AssetJSP(AHDR, game, endianness, Program.Renderer);
                case AssetType.JSPInfo:
                    return new AssetJSP_INFO(AHDR, game, endianness);
                case AssetType.Model:
                    return new AssetMODL(AHDR, game, endianness, Program.Renderer);
                case AssetType.Texture:
                    return new AssetRWTX(AHDR, game, endianness);
                case AssetType.SoundInfo:
                    if (platform == Platform.GameCube && (game == Game.BFBB || game == Game.Scooby))
                        return new AssetSNDI_GCN_V1(AHDR, game, endianness);
                    if (platform == Platform.GameCube)
                        return new AssetSNDI_GCN_V2(AHDR, game, endianness);
                    if (platform == Platform.Xbox)
                        return new AssetSNDI_XBOX(AHDR, game, endianness);
                    if (platform == Platform.PS2)
                        return new AssetSNDI_PS2(AHDR, game, endianness);
                    return new AssetGeneric(AHDR, game, endianness);
                case AssetType.Spline:
                    return new AssetSPLN(AHDR, game, endianness, Program.Renderer);
                case AssetType.WireframeModel:
                    return new AssetWIRE(AHDR, game, endianness, Program.Renderer);
                case AssetType.AnimationList:
                    return new AssetALST(AHDR, game, endianness);
                case AssetType.AnimationTable:
                    return new AssetATBL(AHDR, game, endianness);
                case AssetType.AttackTable:
                    return new AssetATKT(AHDR, game, endianness);
                case AssetType.Boulder:
                    return new AssetBOUL(AHDR, game, endianness);
                case AssetType.Button:
                    return new AssetBUTN(AHDR, game, endianness);
                case AssetType.Camera:
                    return new AssetCAM(AHDR, game, endianness);
                case AssetType.Counter:
                    return new AssetCNTR(AHDR, game, endianness);
                case AssetType.CollisionTable:
                    return new AssetCOLL(AHDR, game, endianness);
                case AssetType.Conditional:
                    return new AssetCOND(AHDR, game, endianness);
                case AssetType.Credits:
                    if (game == Game.BFBB)
                        return new AssetCRDT(AHDR, game, endianness);
                    return new AssetGeneric(AHDR, game, endianness); // unsupported CRDT for non bfbb
                case AssetType.CutsceneManager:
                    return new AssetCSNM(AHDR, game, endianness);
                case AssetType.Destructible:
                    return new AssetDEST(AHDR, game, endianness);
                case AssetType.Dispatcher:
                    return new AssetDPAT(AHDR, game, endianness);
                case AssetType.DiscoFloor:
                    return new AssetDSCO(AHDR, game, endianness);
                case AssetType.DestructibleObject:
                    return new AssetDSTR(AHDR, game, endianness);
                case AssetType.DashTrack:
                    return new AssetDTRK(AHDR, game, endianness);
                case AssetType.Duplicator:
                    return new AssetDUPC(AHDR, game, endianness);
                case AssetType.ElectricArcGenerator:
                    return new AssetEGEN(AHDR, game, endianness);
                case AssetType.Environment:
                    return new AssetENV(AHDR, game, endianness);
                case AssetType.Flythrough:
                    return new AssetFLY(AHDR, game, endianness);
                case AssetType.Fog:
                    return new AssetFOG(AHDR, game, endianness);
                case AssetType.Group:
                    return new AssetGRUP(AHDR, game, endianness);
                case AssetType.GrassMesh:
                    return new AssetGRSM(AHDR, game, endianness);
                case AssetType.Gust:
                    return new AssetGUST(AHDR, game, endianness);
                case AssetType.Hangable:
                    return new AssetHANG(AHDR, game, endianness);
                case AssetType.JawDataTable:
                    return new AssetJAW(AHDR, game, endianness);
                case AssetType.Light:
                    return new AssetLITE(AHDR, game, endianness);
                case AssetType.LightKit:
                    if (AHDR.data.Length == 0)
                        return new AssetGeneric(AHDR, game, endianness);
                    return new AssetLKIT(AHDR, game, endianness);
                case AssetType.LobMaster:
                    return new AssetLOBM(AHDR, game, endianness);
                case AssetType.LevelOfDetailTable:
                    return new AssetLODT(AHDR, game, endianness);
                case AssetType.SurfaceMapper:
                    return new AssetMAPR(AHDR, game, endianness);
                case AssetType.ModelInfo:
                    return new AssetMINF(AHDR, game, endianness);
                case AssetType.Marker:
                    return new AssetMRKR(AHDR, game, endianness);
                case AssetType.MovePoint:
                    return new AssetMVPT(AHDR, game, endianness);
                case AssetType.Villain:
                    return new AssetNPC(AHDR, game, endianness);
                case AssetType.OneLiner:
                    return new AssetONEL(AHDR, game, endianness);
                case AssetType.ParticleEmitter:
                    return new AssetPARE(AHDR, game, endianness);
                case AssetType.ParticleProperties:
                    return new AssetPARP(AHDR, game, endianness);
                case AssetType.ParticleSystem:
                    return new AssetPARS(AHDR, game, endianness);
                case AssetType.Pendulum:
                    return new AssetPEND(AHDR, game, endianness);
                case AssetType.ProgressScript:
                    return new AssetPGRS(AHDR, game, endianness);
                case AssetType.PickupTable:
                    return new AssetPICK(AHDR, game, endianness);
                case AssetType.PipeInfoTable:
                    return new AssetPIPT(AHDR, game, endianness, UpdateModelBlendModes);
                case AssetType.Pickup:
                    return new AssetPKUP(AHDR, game, endianness);
                case AssetType.Platform:
                    return new AssetPLAT(AHDR, game, endianness);
                case AssetType.Player:
                    return new AssetPLYR(AHDR, game, endianness);
                case AssetType.Portal:
                    return new AssetPORT(AHDR, game, endianness);
                case AssetType.Projectile:
                    return new AssetPRJT(AHDR, game, endianness);
                case AssetType.ReactiveAnimation:
                    return new AssetRANM(AHDR, game, endianness);
                case AssetType.Script:
                    return new AssetSCRP(AHDR, game, endianness);
                case AssetType.SDFX:
                    return new AssetSDFX(AHDR, game, endianness);
                case AssetType.SFX:
                    return new AssetSFX(AHDR, game, endianness);
                case AssetType.SoundGroup:
                    return new AssetSGRP(AHDR, game, endianness);
                case AssetType.Track:
                case AssetType.SimpleObject:
                    return new AssetSIMP(AHDR, game, endianness);
                case AssetType.ShadowTable:
                    return new AssetSHDW(AHDR, game, endianness);
                case AssetType.Shrapnel:
                    return new AssetSHRP(AHDR, game, endianness);
                case AssetType.Surface:
                    return new AssetSURF(AHDR, game, endianness);
                case AssetType.Text:
                    return new AssetTEXT(AHDR, game, endianness);
                case AssetType.Trigger:
                    return new AssetTRIG(AHDR, game, endianness);
                case AssetType.Timer:
                    return new AssetTIMR(AHDR, game, endianness);
                case AssetType.ThrowableTable:
                    return new AssetTRWT(AHDR, game, endianness);
                case AssetType.PickupTypes:
                    return new AssetTPIK(AHDR, game, endianness);
                case AssetType.UserInterface:
                    return new AssetUI(AHDR, game, endianness);
                case AssetType.UserInterfaceFont:
                    return new AssetUIFT(AHDR, game, endianness);
                case AssetType.UserInterfaceMotion:
                    return new AssetUIM(AHDR, game, endianness);
                case AssetType.NPC:
                    return new AssetVIL(AHDR, game, endianness);
                case AssetType.NPCProperties:
                    return new AssetVILP(AHDR, game, endianness);
                case AssetType.Volume:
                    return new AssetVOLU(AHDR, game, endianness);
                case AssetType.ZipLine:
                    return new AssetZLIN(AHDR, game, endianness);

                case AssetType.Sound:
                case AssetType.StreamingSound:
                    return new AssetSound(AHDR, game, platform);

                case AssetType.CameraCurve:
                case AssetType.NavigationMesh:
                case AssetType.SlideProperty:
                case AssetType.SceneSettings:
                    return new AssetGenericBase(AHDR, game, endianness);

                case AssetType.BinkVideo:
                case AssetType.CutsceneStreamingSound:
                case AssetType.MorphTarget:
                case AssetType.NPCSettings:
                case AssetType.RawImage:
                case AssetType.SplinePath:
                case AssetType.Subtitles:
                case AssetType.TEXS:
                case AssetType.UIFN:

                case AssetType.Null:

                case AssetType.Cutscene:
                case AssetType.CutsceneTableOfContents:

                    return new AssetGeneric(AHDR, game, endianness);
            }
            throw new Exception($"Unknown asset type ({AHDR.assetType})");
        }

        private AssetDYNA CreateDYNA(Section_AHDR AHDR, Game game, Endianness endianness)
        {
            DynaType type;
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = 8;
                type = (DynaType)reader.ReadUInt32();
            }

            switch (type)
            {
                case DynaType.Checkpoint: return new DynaCheckpoint(AHDR, game, endianness);
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
                case DynaType.effect__LensFlareSource: return new DynaEffectLensFlareSource(AHDR, game, endianness);
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
                case DynaType.game_object__RubbleGenerator: return new DynaGObjectRubbleGenerator(AHDR, game, endianness);
                case DynaType.game_object__Taxi: return new DynaGObjectTaxi(AHDR, game, endianness);
                case DynaType.game_object__Teleport: return new DynaGObjectTeleport(AHDR, game, endianness, GetMRKR);
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
                case DynaType.game_object__bullet_mark:
                case DynaType.game_object__bullet_time:
                case DynaType.game_object__rband_camera_asset:
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
                case DynaType.Unknown_EBC04E7B: // incredibles and rotu

                // rat proto
                case DynaType.AnalogDeflection:
                case DynaType.AnalogDirection:
                case DynaType.Enemy__NPC_Gate:
                case DynaType.Enemy__NPC_Walls:
                case DynaType.HUD_Compass_Object:
                case DynaType.HUD_Compass_System:
                case DynaType.logic__Mission:
                case DynaType.logic__Task:
                case DynaType.Pour_Widget:
                case DynaType.Twiddler:
                case DynaType.Unknown_0A21FFAD:
                case DynaType.Unknown_105DFF22:
                case DynaType.Unknown_1E175B3F:
                case DynaType.Unknown_284375FD:
                case DynaType.Unknown_2D0D198B:
                case DynaType.Unknown_35F3B22A:
                case DynaType.Unknown_45F261C6:
                case DynaType.Unknown_544AA34C:
                case DynaType.Unknown_544E0BCC:
                case DynaType.Unknown_89F5441A:
                case DynaType.Unknown_B34B0083:
                case DynaType.Unknown_D9CA96BC:
                case DynaType.Unknown_EF5FD10C:
                case DynaType.Unknown_F5B8CC9C:

                case DynaType.Null:
                    return new DynaGeneric(AHDR, type, game, endianness);
                default:
                    throw new Exception("Unknown DYNA type: " + type.ToString("X8"));
            }
        }

        public uint? CreateNewAsset()
        {
            Section_AHDR AHDR = AssetHeader.GetAsset();

            if (AHDR != null)
            {
#if !DEBUG
                try
                {
#endif
                while (ContainsAsset(AHDR.assetID))
                    MessageBox.Show($"Archive already contains asset id [{AHDR.assetID:X8}]. Will change it to [{++AHDR.assetID:X8}].");

                UnsavedChanges = true;
                AddAsset(AHDR, game, platform.Endianness(), true);
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

        public uint AddAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool setTextureDisplay)
        {
            if (!NoLayers)
                Layers[SelectedLayerIndex].AssetIDs.Add(AHDR.assetID);
            AddAssetToDictionary(AHDR, game, endianness, false, true);

            if (setTextureDisplay && GetFromAssetID(AHDR.assetID) is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return AHDR.assetID;
        }

        public uint AddAsset(Asset asset, bool setTextureDisplay)
        {
            if (!NoLayers)
                Layers[SelectedLayerIndex].AssetIDs.Add(asset.assetID);
            AddAssetToDictionary(asset, false);

            if (setTextureDisplay && asset is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return asset.assetID;
        }

        public uint AddAssetWithUniqueID(Section_AHDR AHDR, Game game, Endianness endianness, bool giveIDregardless = false, bool setTextureDisplay = false, bool ignoreNumber = false)
        {
            var assetName = GetUniqueAssetName(AHDR.ADBG.assetName, AHDR.assetID, giveIDregardless, ignoreNumber);

            if (assetName != AHDR.ADBG.assetName || AHDR.assetID == 0 || string.IsNullOrEmpty(AHDR.ADBG.assetName))
            {
                AHDR.assetID = BKDRHash(assetName);
                AHDR.ADBG.assetName = assetName;
            }

            return AddAsset(AHDR, game, endianness, setTextureDisplay);
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

            Layers.ForEach(l => l.AssetIDs.Remove(assetID));

            if (removeSound && GetFromAssetID(assetID).assetType.ToString().Contains("SND"))
                RemoveSoundFromSNDI(assetID);

            assetDictionary.Remove(assetID);
        }

        public void DuplicateSelectedAssets(out List<uint> finalIndices)
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

                AddAssetWithUniqueID(AHDR, asset.game, asset.endianness);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

                finalIndices.Add(AHDR.assetID);
                newAHDRs.Add(AHDR);
            }

            if (updateReferencesOnCopy)
                UpdateReferencesOnCopy(referenceUpdate, newAHDRs);
        }

        public void CopyAssetsToClipboard()
        {
            var clipboard = new AssetClipboard();

            foreach (Asset asset in currentlySelectedAssets)
            {
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(JsonConvert.SerializeObject(asset.BuildAHDR()));

                if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.StreamingSound)
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

                clipboard.Add(asset.game, asset.endianness, AHDR);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
        }

        public static bool updateReferencesOnCopy = true;
        public static bool replaceAssetsOnPaste = false;

        public void PasteAssetsFromClipboard(out List<uint> finalIndices, AssetClipboard clipboard = null, bool forceRefUpdate = false, bool dontReplace = false)
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

                AddAssetWithUniqueID(clipboard.assets[i], clipboard.games[i], clipboard.endiannesses[i]);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

                if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.StreamingSound)
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
                
                assetDictionary[AHDR.assetID] = TryCreateAsset(AHDR, clipboard.games[i], clipboard.endiannesses[i], true, ref error);
            }
        }

        public void UpdateReferencesOnCopy(Dictionary<uint, uint> referenceUpdate, List<Section_AHDR> assets)
        {
            AssetType[] dontUpdate = new AssetType[] {
                    AssetType.BSP,
                    AssetType.JSP,
                    AssetType.Model,
                    AssetType.Texture,
                    AssetType.Sound,
                    AssetType.SoundInfo,
                    AssetType.StreamingSound,
                    AssetType.Text
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

        public void ReplaceReferences(uint oldAssetId, uint newAssetId) =>
            FindWhoTargets(oldAssetId).ForEach(assetId => GetFromAssetID(assetId).ReplaceReferences(oldAssetId, newAssetId));            
        
        public List<uint> ImportMultipleAssets(List<Section_AHDR> AHDRs, bool overwrite)
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
                        AddAsset(AHDR, game, platform.Endianness(), setTextureDisplay: false);
                    }
                    else
                        AddAssetWithUniqueID(AHDR, game, platform.Endianness(), setTextureDisplay: true);

                    if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.StreamingSound)
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

        public IEnumerable<uint> GetCurrentlySelectedAssetIDs() => currentlySelectedAssets.Select(a => a.assetID);
        
        public void ClearSelectedAssets()
        {
            currentlySelectedAssets.ForEach(a => a.isSelected = false);
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

        public AssetMRKR GetMRKR(uint mrkr)
        {
            if (ContainsAsset(mrkr) && GetFromAssetID(mrkr) is AssetMRKR MRKR)
                return MRKR;
            return null;
        }
    }
}
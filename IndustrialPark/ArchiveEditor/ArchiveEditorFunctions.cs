using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public bool UnsavedChanges
        {
            get
            {
                return _unsavedChanges;
            }
            set
            {
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

                CurrentlySelectedAssets = new ObservableCollection<Asset>();
                currentlyOpenFilePath = null;
                assetDictionary.Clear();

                PACK = getNewArchive.Value.PACK;

                Layers = new List<Layer>();
                platform = getNewArchive.Value.platform;
                game = getNewArchive.Value.game;

                if (getNewArchive.Value.noLayers)
                    NoLayers = true;

                if (getNewArchive.Value.addDefaultAssets)
                    PlaceDefaultAssets();

                UnsavedChanges = true;
                RecalculateAllMatrices();

                return true;
            }

            return false;
        }

        public void OpenFile(string fileName, bool displayProgressBar, Platform scoobyPlatform)
        {
            Dispose();

            ProgressBar progressBar = new ProgressBar("Opening " + Path.GetFileName(fileName));

            if (displayProgressBar)
                progressBar.Show();

            assetDictionary = new Dictionary<uint, Asset>();

            CurrentlySelectedAssets = new ObservableCollection<Asset>();
            currentlyOpenFilePath = fileName;

            HipFile hipFile;
            Game game;
            Platform platform;

            try
            {
                (hipFile, game, platform) = HipFile.FromPath(fileName);
            }
            catch (Exception e)
            {
                progressBar.Close();
                throw e;
            }

            progressBar.SetProgressBar(0, hipFile.DICT.ATOC.AHDRList.Count, 1);

            if (hipFile.HIPB.VersionMismatch)
                MessageBox.Show($"Expected: {Section_HIPB.CurrentVersion}\nGot: {hipFile.HIPB.Version}\nAdditional data will be skipped", "Newer HIPB version detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            while (game == Game.Unknown)
                game = ChooseGame.GetGame();
            this.game = game;

            while (platform == Platform.Unknown)
                platform = ChoosePlatformDialog.GetPlatform();
            this.platform = platform;

            List<AssetError> assetsWithError = new List<AssetError>();

            List<string> autoComplete = new List<string>(hipFile.DICT.ATOC.AHDRList.Count);

#if DEBUG
            var tempAhdrUglyDict = new Dictionary<uint, Section_AHDR>();
#endif

            PACK = hipFile.PACK;

            Layers = new List<Layer>();
            for (int i = 0; i < hipFile.DICT.LTOC.LHDRList.Count; i++)
                Layers.Add(LHDRToLayer(hipFile.DICT.LTOC.LHDRList[i], game, hipFile.HIPB.GetLayerName(i)));

            foreach (var l in Layers)
                foreach (var u in l.AssetIDs)
                {
                    var AHDR = hipFile.DICT.ATOC.AHDRList.Where(ahdr => ahdr.assetID == u).First();
                    AssetError error = AddAssetToDictionary(AHDR, game, platform.Endianness(), true, false);

                    if (error.reason != ErrorReason.NoError)
                        assetsWithError.Add(error);

                    autoComplete.Add(AHDR.ADBG.assetName);

                    progressBar.PerformStep();
#if DEBUG
                    tempAhdrUglyDict[AHDR.assetID] = AHDR;
#endif
                }

            if (assetsWithError.Any())
                MessageBox.Show("There was an error loading the following assets and editing may have been disabled for them:\n" + string.Join("\n", assetsWithError), 
                    $"{assetsWithError.Count} Asset(s) with errors", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (hipFile.HIPB != null && hipFile.HIPB.HasNoLayers != 0)
                NoLayers = true;

            SetupTextureDisplay();
            RecalculateAllMatrices();

            autoCompleteSource.Clear();
            autoCompleteSource.AddRange(autoComplete.ToArray());

            if (ContainsAssetWithType(AssetType.PipeInfoTable) && ContainsAssetWithType(AssetType.Model))
                foreach (var PIPT in assetDictionary.Values.Where(a => a is AssetPIPT).Select(a => (AssetPIPT)a))
                    PIPT.UpdateDictionary();

            if (NoLayers)
                Layers = new List<Layer>();

            progressBar.Close();
#if DEBUG
            LogAssetOrder(hipFile.DICT, tempAhdrUglyDict);
#endif
        }

        private static Layer LHDRToLayer(Section_LHDR LHDR, Game game, string layerName)
        {
            var layer = new Layer(LayerTypeSpecificToGeneric(LHDR.layerType, game), LHDR.assetIDlist.Count, layerName);
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
            try
            {
                File.WriteAllBytes(currentlyOpenFilePath, BuildHipFile().ToBytes(game, platform));
                UnsavedChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool LegacySave = false;

        private HipFile BuildHipFile()
        {
            var DICT = new Section_DICT();

            foreach (var asset in assetDictionary.Values)
            {
                asset.SetGame(game);
                if (asset is AssetSNDI_PS2 sndips2)
                    sndips2.OrderEntries();
                DICT.ATOC.AHDRList.Add(asset.BuildAHDR(platform.Endianness()));
            }

            var HIPB = new Section_HIPB()
            {
                HasNoLayers = Convert.ToInt32(NoLayers),
                ScoobyPlatform = platform,
                IncrediblesGame = game
            };

            var layers = NoLayers ? BuildLayers() : Layers;
            for (int i = 0; i < layers.Count; i++)
            {
                DICT.LTOC.LHDRList.Add(new Section_LHDR()
                {
                    layerType = LayerTypeGenericToSpecific(layers[i].Type, game),
                    assetIDlist = layers[i].AssetIDs,
                    LDBG = new Section_LDBG(-1)
                });
                if (!string.IsNullOrWhiteSpace(layers[i].LayerName))
                    HIPB.LayerNames[i] = layers[i].LayerName;
            }

            PACK.PMOD.modDate = (int)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

            return new HipFile(new Section_HIPA(), PACK, DICT, new Section_STRM(), LegacySave ? null : HIPB);
        }

        private static int LayerTypeGenericToSpecific(LayerType layerType, Game game)
        {
            if (game >= Game.Incredibles || layerType < LayerType.BSP)
                return (int)layerType;
            return (int)layerType - 1;
        }

        private static LayerType LayerTypeSpecificToGeneric(int layerType, Game game)
        {
            if (game >= Game.Incredibles || layerType < 2)
                return (LayerType)layerType;
            return (LayerType)(layerType + 1);
        }

        public bool EditPack()
        {
            var (PACK, newPlatform, newGame) = NewArchive.GetExistingArchive(platform, game, this.PACK.PCRT.fileDate, this.PACK.PCRT.dateString);

            if (PACK != null)
            {
                this.PACK = PACK;

                if (platform != newPlatform || game != newGame)
                {
                    var notConverted = GetUnconvertableAssets();
                    if (!string.IsNullOrEmpty(notConverted))
                        new ScrollableMessageBox($"[{Path.GetFileName(currentlyOpenFilePath)}] Unconvertable assets", notConverted).Show();
                }

                platform = newPlatform;
                game = newGame;

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

        private string GetUnconvertableAssets()
        {
            var result = new List<string>();
            foreach (var asset in assetDictionary.Values)
            {
                if (asset.assetType == AssetType.SoundInfo ||
                    asset is AssetSound ||
                    asset is AssetGeneric ||
                    asset is AssetGenericBase ||
                    asset is DynaGeneric ||
                    asset is AssetJSP ||
                    asset is AssetJSP_INFO ||
                    asset is AssetMODL ||
                    asset is AssetRWTX)
                    result.Add($"[{AssetTypeContainer.AssetTypeToString(asset.assetType)}] {asset.assetName}");
            }
            if (result.Count > 0)
                return "The following asset types could not be converted. You might need to re-create them or replace them with the appropriate version on the game and/or platform.\n\n" + string.Join("\n", result.OrderBy(x => x));
            return null;
        }

        private bool _noLayers = false;
        public bool NoLayers
        {
            get => _noLayers;
            set
            {
                if (value)
                {
                    Layers = new List<Layer>();
                }
                else
                {
                    try
                    {
                        Layers = BuildLayers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                _noLayers = value;
                UnsavedChanges = true;
                SelectedLayerIndex = -1;
            }
        }

        public int SelectedLayerIndex = -1;

        public int LayerCount => Layers.Count;

        public int GetLayerType() => LayerTypeGenericToSpecific(Layers[SelectedLayerIndex].Type, game);

        public void SetLayerType(int type) => Layers[SelectedLayerIndex].Type = LayerTypeSpecificToGeneric(type, game);

        public string LayerToString() => LayerToString(SelectedLayerIndex);

        public string LayerToString(int index) => "Layer " + index.ToString("D2") + ": "
            + (string.IsNullOrWhiteSpace(Layers[index].LayerName) ? Layers[index].Type.ToString() : Layers[index].LayerName)
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

        /// <summary>
        /// Rename a layer
        /// </summary>
        /// <param name="selectedIndex"></param>
        /// <returns>True if layer has been successfully renamed, false otherwise</returns>
        public bool RenameLayer(int selectedIndex)
        {
            if (NoLayers)
                return false;

            var layer = Layers[selectedIndex];
            var rn = new RenameLayer(layer.LayerName);
            if (rn.ShowDialog() == DialogResult.OK)
            {
                layer.LayerName = string.IsNullOrWhiteSpace(rn.LayerName) ? null : rn.LayerName;
                UnsavedChanges = true;
                return true;
            }
            return false;
        }

        public void Dispose(bool showProgress = true)
        {
            List<uint> assetList = new List<uint>();
            assetList.AddRange(assetDictionary.Keys);

            if (assetList.Count == 0)
                return;

            ProgressBar progressBar = null;
            if (showProgress)
            {
                progressBar = new ProgressBar("Closing Archive");
                progressBar.Show();
                progressBar.SetProgressBar(0, assetList.Count, 1);
            }

            foreach (uint assetID in assetList)
            {
                DisposeOfAsset(assetID);
                if (showProgress)
                    progressBar.PerformStep();
            }

            Layers.Clear();
            assetDictionary.Clear();

            currentlyOpenFilePath = null;
            SelectedLayerIndex = -1;

            if (showProgress)
                progressBar.Close();
        }

        public void DisposeOfAsset(uint assetID)
        {
            var asset = assetDictionary[assetID];
            CurrentlySelectedAssets.Remove(asset);
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
            else if (asset is AssetRWTX rwtx && !SkipTextureDisplay)
                TextureManager.RemoveTexture(rwtx.Name, this, rwtx.assetID);
        }

        public bool ContainsAsset(uint key) => assetDictionary.ContainsKey(key);

        public IEnumerable<AssetType> AssetTypesOnArchive() =>
            (from Asset asset in assetDictionary.Values select asset.assetType).Distinct();

        public IEnumerable<AssetType> ScalableAssetTypesOnArchive() =>
            (from Asset asset in assetDictionary.Values
             where
            asset is IClickableAsset ||
            (game == Game.BFBB && asset is AssetJSP) ||
            (game == Game.BFBB && asset is AssetJSP_INFO) ||
            asset is AssetLODT ||
            asset is AssetFLY
             select asset.assetType).Distinct();

        public List<AssetType> AssetTypesOnLayer() => NoLayers ?
            (from Asset a in assetDictionary.Values select a.assetType).Distinct().ToList() :
            (from uint a in Layers[SelectedLayerIndex].AssetIDs select assetDictionary[a].assetType).Distinct().ToList();

        public bool ContainsAssetWithType(AssetType assetType) =>
            assetDictionary.Values.Any(a => a.assetType.Equals(assetType));

        public Asset GetFromAssetID(uint key)
        {
            if (ContainsAsset(key))
                return assetDictionary[key];
            throw new KeyNotFoundException($"Asset [{key:X8}] not present in dictionary.");
        }

        public Dictionary<uint, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }

        public int AssetCount => assetDictionary.Values.Count;

        public enum ErrorReason
        {
            NoError,
            SequenceNotEqual,
            SkipBuildTesting,
            Exception,
            NoData
        }
        public struct AssetError
        {
            public ErrorReason reason;
            public uint assetid;
            public string assetname;
            public string errorMessage;

            public override string ToString()
            {
                return $"-{reason}: [{assetid:X8}] {assetname} {string.Format("({0})", errorMessage) ?? ""}";
            }
        }
        private AssetError AddAssetToDictionary(Section_AHDR AHDR, Game game, Endianness endianness, bool fast, bool showMessageBox)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;
            AssetError err = new AssetError() { reason = ErrorReason.NoError, assetid = AHDR.assetID, assetname = AHDR.ADBG.assetName };

            newAsset = TryCreateAsset(AHDR, game, endianness, showMessageBox, ref err);

            if (newAsset.SkipBuildTesting)
            {
                err.reason = ErrorReason.SkipBuildTesting;
                err.errorMessage = "Intentional, editing has not been disabled";
            }

            // testing if build works
            if (fast && err.reason == ErrorReason.NoError)
            {
                var built = newAsset.BuildAHDR(platform.Endianness()).data;
                if (!Enumerable.SequenceEqual(AHDR.data, built))
                {
                    err.reason = ErrorReason.SequenceNotEqual;
                    err.errorMessage = "unsupported format";
                    if (showMessageBox)
                        MessageBox.Show($"There was an error loading asset " + err + " and editing has been disabled for it.");

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

            return err;
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

        protected virtual Asset TryCreateAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool showMessageBox, ref AssetError error)
        {
            try
            {
                return CreateAsset(AHDR, game, endianness);
            }
            catch (Exception ex)
            {
                if (AHDR.data.Length == 0)
                {
                    error.reason = ErrorReason.NoData;
                    error.errorMessage = "Asset is empty and does not contain any data";
                }
                else
                {
                    error.reason = ErrorReason.Exception;
                    error.errorMessage = ex.Message;
                }

                if (showMessageBox)
                    MessageBox.Show($"There was an error loading asset {error.assetname} and editing has been disabled for it.");

                return new AssetGeneric(AHDR, game, endianness);
            }
        }

        protected Asset CreateAsset(Section_AHDR AHDR, Game game, Endianness endianness)
        {
            if (AHDR.IsDyna)
                return CreateDYNA(AHDR, game, endianness);

            switch (AHDR.assetType)
            {
                case AssetType.Animation:
                    if (game >= Game.ROTU)
                        return new AssetANIM_V2(AHDR, game, endianness);
                    return new AssetANIM_V1(AHDR, game, endianness);
                case AssetType.BSP:
                case AssetType.JSP:
                    return new AssetJSP(AHDR, game, endianness, Program.Renderer);
                case AssetType.JSPInfo:
                    return new AssetJSP_INFO(AHDR, game, platform, defaultJspAssetIds ?? GetJspAssetIDs(AHDR.assetID));
                case AssetType.Model:
                    return new AssetMODL(AHDR, game, endianness, Program.Renderer);
                case AssetType.Texture:
                case AssetType.TextureStream:
                    return new AssetRWTX(AHDR, game, endianness);
                case AssetType.SoundInfo:
                    if (platform == Platform.GameCube)
                    {
                        if (game < Game.Incredibles)
                            return new AssetSNDI_GCN_V1(AHDR, game, endianness);
                        return new AssetSNDI_GCN_V2(AHDR, game);
                    }
                    if (platform == Platform.Xbox)
                        return new AssetSNDI_XBOX(AHDR, game, endianness);
                    if (platform == Platform.PS2)
                        return new AssetSNDI_PS2(AHDR, game, endianness);
                    return new AssetGeneric(AHDR, game, endianness);
                case AssetType.Spline:
                    return new AssetSPLN(AHDR, game, endianness, Program.Renderer);
                case AssetType.SplinePath:
                    return new AssetSPLP(AHDR, game, endianness);
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
                case AssetType.CameraCurve:
                    return new AssetCCRV(AHDR, game, endianness);
                case AssetType.Counter:
                    return new AssetCNTR(AHDR, game, endianness);
                case AssetType.CollisionTable:
                    return new AssetCOLL(AHDR, game, endianness);
                case AssetType.Conditional:
                    return new AssetCOND(AHDR, game, endianness);
                case AssetType.Credits:
                    return new AssetCRDT(AHDR, game, endianness);
                case AssetType.Cutscene:
                    return new AssetCSN(AHDR, game, endianness);
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
                    return new AssetFLY(AHDR, game);
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
                    return new AssetSDFX(AHDR, game, endianness, GetSGRP);
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
                case AssetType.SoundStream:
                    return new AssetSound(AHDR, game, platform);

                case AssetType.NavigationMesh:
                case AssetType.SlideProperty:
                case AssetType.SceneSettings:
                    return new AssetGenericBase(AHDR, game, endianness);

                case AssetType.BinkVideo:
                case AssetType.CutsceneStreamingSound:
                case AssetType.MorphTarget:
                case AssetType.NPCSettings:
                case AssetType.RawImage:
                case AssetType.Subtitles:
                case AssetType.UIFN:

                case AssetType.Null:

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
                case DynaType.audio__conversation:
                    return new DynaAudioConversation(AHDR, game, endianness);
                case DynaType.Checkpoint:
                    return new DynaCheckpoint(AHDR, game, endianness);
                case DynaType.camera__preset:
                    return new DynaCameraPreset(AHDR, game, endianness);
                case DynaType.Enemy__SB__BucketOTron:
                    return new DynaEnemyBucketOTron(AHDR, game, endianness);
                case DynaType.Enemy__SB__CastNCrew:
                    return new DynaEnemyCastNCrew(AHDR, game, endianness);
                case DynaType.Enemy__SB__Critter:
                    return new DynaEnemyCritter(AHDR, game, endianness);
                case DynaType.Enemy__SB__Dennis:
                    return new DynaEnemyDennis(AHDR, game, endianness);
                case DynaType.Enemy__SB__FrogFish:
                    return new DynaEnemyFrogFish(AHDR, game, endianness);
                case DynaType.Enemy__SB__Mindy:
                    return new DynaEnemyMindy(AHDR, game, endianness);
                case DynaType.Enemy__SB__Neptune:
                    return new DynaEnemyNeptune(AHDR, game, endianness);
                case DynaType.Enemy__SB__Standard:
                    return new DynaEnemyStandard(AHDR, game, endianness);
                case DynaType.Enemy__SB__SupplyCrate:
                    return new DynaEnemySupplyCrate(AHDR, game, endianness);
                case DynaType.Enemy__SB__Turret:
                    return new DynaEnemyTurret(AHDR, game, endianness);
                case DynaType.Incredibles__Icon:
                    return new DynaIncrediblesIcon(AHDR, game, endianness);
                case DynaType.JSPExtraData:
                    return new DynaJSPExtraData(AHDR, game, endianness);
                case DynaType.SceneProperties:
                    return new DynaSceneProperties(AHDR, game, endianness);
                case DynaType.effect__Flamethrower:
                    return new DynaEffectFlamethrower(AHDR, game, endianness);
                case DynaType.effect__LensFlareElement:
                    return new DynaEffectLensFlare(AHDR, game, endianness);
                case DynaType.effect__LensFlareSource:
                    return new DynaEffectLensFlareSource(AHDR, game, endianness);
                case DynaType.effect__Lightning:
                    return new DynaEffectLightning(AHDR, game, endianness);
                case DynaType.effect__Rumble:
                    return new DynaEffectRumble(AHDR, game, endianness);
                case DynaType.effect__RumbleSphericalEmitter:
                    return new DynaEffectRumbleSphere(AHDR, game, endianness);
                case DynaType.effect__ScreenFade:
                    return new DynaEffectScreenFade(AHDR, game, endianness);
                case DynaType.effect__Splash:
                    return new DynaEffectSplash(AHDR, game, endianness);
                case DynaType.effect__grass:
                    return new DynaEffectGrass(AHDR, game, endianness);
                case DynaType.effect__smoke_emitter:
                    return new DynaEffectSmokeEmitter(AHDR, game, endianness);
                case DynaType.effect__spark_emitter:
                    return new DynaEffectSparkEmitter(AHDR, game, endianness);
                case DynaType.effect__spotlight:
                    return new DynaEffectSpotlight(AHDR, game, endianness);
                case DynaType.effect__uber_laser:
                    return new DynaEffectUberLaser(AHDR, game, endianness);
                case DynaType.effect__water_body:
                    return new DynaEffectWaterBody(AHDR, game, endianness);
                case DynaType.Effect__particle_generator:
                    return new DynaEffectParticleGenerator(AHDR, game, endianness);
                case DynaType.game_object__BoulderGenerator:
                    return new DynaGObjectBoulderGen(AHDR, game, endianness);
                case DynaType.game_object__BusStop:
                    return new DynaGObjectBusStop(AHDR, game, endianness);
                case DynaType.game_object__Camera_Tweak:
                    return new DynaGObjectCamTweak(AHDR, game, endianness);
                case DynaType.game_object__Flythrough:
                    return new DynaGObjectFlythrough(AHDR, game, endianness);
                case DynaType.game_object__Grapple:
                    return new DynaGObjectGrapple(AHDR, game, endianness);
                case DynaType.game_object__Hangable:
                    return new DynaGObjectHangable(AHDR, game, endianness);
                case DynaType.game_object__IN_Pickup:
                    return new DynaGObjectInPickup(AHDR, game, endianness);
                case DynaType.game_object__NPCSettings:
                    return new DynaGObjectNPCSettings(AHDR, game, endianness);
                case DynaType.game_object__RaceTimer:
                    return new DynaGObjectRaceTimer(AHDR, game, endianness);
                case DynaType.game_object__Ring:
                    return new DynaGObjectRing(AHDR, game, endianness);
                case DynaType.game_object__RingControl:
                    return new DynaGObjectRingControl(AHDR, game, endianness);
                case DynaType.game_object__RubbleGenerator:
                    return new DynaGObjectRubbleGenerator(AHDR, game, endianness);
                case DynaType.game_object__Taxi:
                    return new DynaGObjectTaxi(AHDR, game, endianness);
                case DynaType.game_object__Teleport:
                    return new DynaGObjectTeleport(AHDR, game, endianness, GetMRKR);
                case DynaType.game_object__Turret:
                    return new DynaGObjectTurret(AHDR, game, endianness);
                case DynaType.game_object__Vent:
                    return new DynaGObjectVent(AHDR, game, endianness);
                case DynaType.game_object__VentType:
                    return new DynaGObjectVentType(AHDR, game, endianness);
                case DynaType.game_object__bungee_drop:
                    return new DynaGObjectBungeeDrop(AHDR, game, endianness);
                case DynaType.game_object__bungee_hook:
                    return new DynaGObjectBungeeHook(AHDR, game, endianness);
                case DynaType.game_object__camera_param_asset:
                    return new DynaGObjectCameraParamAsset(AHDR, game, endianness);
                case DynaType.game_object__dash_camera_spline:
                    return new DynaGObjectDashCameraSpline(AHDR, game, endianness);
                case DynaType.game_object__flame_emitter:
                    return new DynaGObjectFlameEmitter(AHDR, game, endianness);
                case DynaType.game_object__laser_beam:
                    return new DynaGObjectLaserBeam(AHDR, game, endianness);
                case DynaType.game_object__talk_box:
                    return new DynaGObjectTalkBox(AHDR, game, endianness);
                case DynaType.game_object__task_box:
                    return new DynaGObjectTaskBox(AHDR, game, endianness);
                case DynaType.game_object__text_box:
                    return new DynaGObjectTextBox(AHDR, game, endianness);
                case DynaType.game_object__train_car:
                    return new DynaGObjectTrainCar(AHDR, game, endianness);
                case DynaType.game_object__train_junction:
                    return new DynaGObjectTrainJunction(AHDR, game, endianness);
                case DynaType.hud__image:
                    return new DynaHudImage(AHDR, game, endianness);
                case DynaType.hud__meter__font:
                    return new DynaHudMeterFont(AHDR, game, endianness);
                case DynaType.hud__meter__unit:
                    return new DynaHudMeterUnit(AHDR, game, endianness);
                case DynaType.hud__model:
                    return new DynaHudModel(AHDR, game, endianness);
                case DynaType.hud__text:
                    return new DynaHudText(AHDR, game, endianness);
                case DynaType.interaction__Launch:
                    return new DynaInteractionLaunch(AHDR, game, endianness);
                case DynaType.interaction__Lift:
                    return new DynaInteractionLift(AHDR, game, endianness);
                case DynaType.interaction__Turn:
                    return new DynaInteractionTurn(AHDR, game, endianness);
                case DynaType.logic__reference:
                    return new DynaLogicReference(AHDR, game, endianness);
                case DynaType.logic__FunctionGenerator:
                    return new DynaLogicFunctionGenerator(AHDR, game, endianness);
                case DynaType.npc__group:
                    return new DynaNPCGroup(AHDR, game, endianness);
                case DynaType.pointer:
                    return new DynaPointer(AHDR, game, endianness);
                case DynaType.ui__box:
                    return new DynaUIBox(AHDR, game, endianness);
                case DynaType.ui__controller:
                    return new DynaUIController(AHDR, game, endianness);
                case DynaType.ui__image:
                    return new DynaUIImage(AHDR, game, endianness);
                case DynaType.ui__model:
                    return new DynaUIModel(AHDR, game, endianness);
                case DynaType.ui__text:
                    return new DynaUIText(AHDR, game, endianness);
                case DynaType.ui__text__userstring:
                    return new DynaUITextUserString(AHDR, game, endianness);
                case DynaType.Interest_Pointer:
                    return new DynaInterestPointer(AHDR, game, endianness);
                case DynaType.camera__binary_poi:
                    return new DynaCameraBinary(AHDR, game, endianness);
                case DynaType.effect__LightEffectFlicker:
                    return new DynaEffectLightFlicker(AHDR, game, endianness);
                case DynaType.effect__LightEffectStrobe:
                    return new DynaEffectLightStrobe(AHDR, game, endianness);
                case DynaType.effect__ScreenWarp:
                    return new DynaEffectScreenWarp(AHDR, game, endianness);
                case DynaType.effect__light:
                    return new DynaEffectLight(AHDR, game, endianness);
                case DynaType.game_object__bullet_mark:
                    return new DynaGObjectBulletMark(AHDR, game, endianness);
                case DynaType.game_object__bullet_time:
                    return new DynaGObjectBulletTime(AHDR, game, endianness);
                case DynaType.game_object__rband_camera_asset:
                    return new DynaGObjectCameraRband(AHDR, game, endianness);
                case DynaType.npc__CoverPoint:
                    return new DynaNPCCoverpoint(AHDR, game, endianness);
                case DynaType.npc__NPC_Custom_AV:
                    return new DynaNPCCustomAV(AHDR, game, endianness);
                case DynaType.AnalogDeflection:
                    return new DynaAnalogDeflection(AHDR, game, endianness);
                case DynaType.AnalogDirection:
                    return new DynaAnalogDirection(AHDR, game, endianness);
                case DynaType.camera__transition_time:
                    return new DynaCameraTransitionTime(AHDR, game, endianness);
                case DynaType.Carrying_CarryableProperty_GenericUseProperty:
                    return new DynaCarryablePropertyGeneric(AHDR, game, endianness);
                case DynaType.Carrying_CarryableProperty_UsePropertyAttract:
                    return new DynaCarryablePropertyAttract(AHDR, game, endianness);
                case DynaType.Carrying_CarryableProperty_UsePropertyRepel:
                    return new DynaCarryablePropertyRepel(AHDR, game, endianness);
                case DynaType.Carrying_CarryableProperty_UsePropertySwipe:
                    return new DynaCarryablePropertySwipe(AHDR, game, endianness);
                case DynaType.ContextObject_PoleSwing:
                    return new DynaCObjectPoleSwing(AHDR, game, endianness);
                case DynaType.ContextObject_Springboard:
                    return new DynaCObjectSpringBoard(AHDR, game, endianness);
                case DynaType.ContextObject_Tightrope:
                    return new DynaCObjectTightrope(AHDR, game, endianness);
                case DynaType.Enemy__NPC_Gate:
                    return new DynaNpcGate(AHDR, game, endianness);
                case DynaType.Enemy__NPC_Walls:
                    return new DynaNpcWalls(AHDR, game, endianness);
                case DynaType.Enemy__RATS__LeftArm:
                    return new DynaEnemyRATSLeftArm(AHDR, game, endianness);
                case DynaType.Enemy__RATS__RightArm:
                    return new DynaEnemyRATSRightArm(AHDR, game, endianness);
                case DynaType.Enemy__RATS__Swarm__Bug:
                    return new DynaEnemyRATSSwarmBug(AHDR, game, endianness);
                case DynaType.Enemy__RATS__Swarm__Owl:
                    return new DynaEnemyRATSSwarmOwl(AHDR, game, endianness);
                case DynaType.Enemy__RATS__Thief:
                    return new DynaEnemyRATSThief(AHDR, game, endianness);
                case DynaType.Enemy__RATS__Waiter:
                    return new DynaEnemyRATSWaiter(AHDR, game, endianness);
                case DynaType.HUD_Compass_Object:
                    return new DynaHudCompassObject(AHDR, game, endianness);
                case DynaType.HUD_Compass_System:
                    return new DynaHudCompassSystem(AHDR, game, endianness);
                case DynaType.logic__Mission:
                    return new DynaLogicMission(AHDR, game, endianness);
                case DynaType.logic__Task:
                    return new DynaLogicTask(AHDR, game, endianness);
                case DynaType.Pour_Widget:
                    return new DynaPourWidget(AHDR, game, endianness);
                case DynaType.Twiddler:
                    return new DynaTwiddler(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Bomber:
                    return new DynaEnemyIN2Bomber(AHDR, game, endianness);
                case DynaType.Enemy__IN2__BossUnderminerDrill:
                    return new DynaEnemyIN2BossUnderminerDrill(AHDR, game, endianness);
                case DynaType.Enemy__IN2__BossUnderminerUM:
                    return new DynaEnemyIN2BossUnderminerUM(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Chicken:
                    return new DynaEnemyIN2Chicken(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Driller:
                    return new DynaEnemyIN2Driller(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Enforcer:
                    return new DynaEnemyIN2Enforcer(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Humanoid:
                    return new DynaEnemyIN2Humanoid(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Rat:
                    return new DynaEnemyIN2Rat(AHDR, game, endianness);
                case DynaType.Enemy__IN2__RobotTank:
                    return new DynaEnemyIN2RobotTank(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Scientist:
                    return new DynaEnemyIN2Scientist(AHDR, game, endianness);
                case DynaType.Enemy__IN2__Shooter:
                    return new DynaEnemyIN2Shooter(AHDR, game, endianness);

                case DynaType.effect__RumbleBoxEmitter:
                case DynaType.effect__Waterhose:
                case DynaType.Enemy__SB:
                case DynaType.camera__transition_path:
                case DynaType.effect__BossBrain:
                case DynaType.game_object__FreezableObject:
                case DynaType.Carrying_CarryableObject:
                case DynaType.interaction__IceBridge:
                case DynaType.interaction__SwitchLever:
                case DynaType.Unknown_EBC04E7B:
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

        public Asset AddAsset(Section_AHDR AHDR, Game game, Endianness endianness, bool setTextureDisplay, int forceLayerIndex = -1)
        {
            if (!NoLayers)
                Layers[forceLayerIndex != -1 ? forceLayerIndex : SelectedLayerIndex].AssetIDs.Add(AHDR.assetID);
            AddAssetToDictionary(AHDR, game, endianness, false, true);

            var asset = GetFromAssetID(AHDR.assetID);

            if (setTextureDisplay && asset is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return asset;
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

        public Asset AddAssetWithUniqueID(Section_AHDR AHDR, Game game, Endianness endianness, bool giveIDregardless = false, bool setTextureDisplay = false, bool ignoreNumber = false)
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

        public void RemoveAsset(uint assetID, bool removeSound = true)
        {
            DisposeOfAsset(assetID);
            autoCompleteSource.Remove(assetDictionary[assetID].assetName);

            Layers.ForEach(l => l.AssetIDs.Remove(assetID));

            if (removeSound)
            {
                var assetType = GetFromAssetID(assetID).assetType;
                if (assetType == AssetType.Sound || assetType == AssetType.SoundStream)
                    RemoveSoundFromSNDI(assetID);
            }

            assetDictionary.Remove(assetID);
        }

        public void DuplicateSelectedAssets(out List<uint> finalIndices)
        {
            UnsavedChanges = true;

            finalIndices = new List<uint>();
            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();
            var newAHDRs = new List<Section_AHDR>();

            foreach (var asset in CurrentlySelectedAssets)
            {
                string serializedObject = JsonConvert.SerializeObject(asset.BuildAHDR(platform.Endianness()));
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                var previousAssetID = AHDR.assetID;

                AddAssetWithUniqueID(AHDR, asset.game, platform.Endianness());

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

            foreach (Asset asset in CurrentlySelectedAssets)
            {
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(JsonConvert.SerializeObject(asset.BuildAHDR(platform.Endianness())));

                if (asset is AssetSound)
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
                clipboard.Add(asset.game, platform.Endianness(), AHDR, asset is AssetJSP_INFO jspInfo ? jspInfo.JSP_AssetIDs : null);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(clipboard, Formatting.None));
        }

        public static bool updateReferencesOnCopy = true;
        public static bool replaceAssetsOnPaste = false;

        public bool PasteAssetsFromClipboard(out List<uint> finalIndices, AssetClipboard clipboard = null, bool forceRefUpdate = false, bool dontReplace = false)
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
                return false;
            }

            UnsavedChanges = true;

            Dictionary<uint, uint> referenceUpdate = new Dictionary<uint, uint>();

            for (int i = 0; i < clipboard.assets.Count; i++)
            {
                Section_AHDR AHDR = clipboard.assets[i];

                uint previousAssetID = AHDR.assetID;

                if (replaceAssetsOnPaste && !dontReplace && ContainsAsset(AHDR.assetID))
                    RemoveAsset(AHDR.assetID);

                var asset = AddAssetWithUniqueID(AHDR, clipboard.games[i], clipboard.endiannesses[i]);

                asset.SetGame(game);

                if (previousAssetID != 0)
                    referenceUpdate.Add(previousAssetID, asset.assetID);

                if (asset is AssetSound sound)
                {
                    try
                    {
                        AddSoundToSNDI(sound.Data, sound.assetID, sound.assetType, out byte[] soundData);
                        AHDR.data = soundData;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else if (asset is AssetJSP_INFO jspInfo)
                    jspInfo.JSP_AssetIDs = clipboard.jspExtraInfo[i];

                finalIndices.Add(AHDR.assetID);
            }

            if (updateReferencesOnCopy || forceRefUpdate)
                UpdateReferencesOnCopy(referenceUpdate, clipboard.assets);

            return true;
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
                    AssetType.SoundStream,
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

                    if (AHDR.assetType == AssetType.Sound || AHDR.assetType == AssetType.SoundStream)
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

                    UnsavedChanges = true;
                    assetIDs.Add(AHDR.assetID);

                    if (AHDR.assetType == AssetType.JSP)
                    {
                        AssetJSP_INFO jspInfo = (AssetJSP_INFO)PlaceTemplate(AHDR.ADBG.assetName + "_jspInfo", AssetTemplate.Jsp_Info);
                        jspInfo.CreateFromJsp((AssetJSP)GetFromAssetID(AHDR.assetID));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unable to import asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName}: " + ex.Message);
                }
            }

            return assetIDs;
        }

        public ObservableCollection<Asset> CurrentlySelectedAssets { get; private set; } = new ObservableCollection<Asset>();
        

        private static IList<Asset> allCurrentlySelectedAssets
        {
            get
            {
                IList<Asset> currentlySelectedAssets = new ObservableCollection<Asset>();
                foreach (ArchiveEditor ae in Program.MainForm.archiveEditors)
                {
                    foreach (Asset assetToAdd in ae.archive.CurrentlySelectedAssets)
                    {
                        if (!currentlySelectedAssets.Contains(assetToAdd))
                            currentlySelectedAssets.Add(assetToAdd);
                    }
                }
                    
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
                CurrentlySelectedAssets.Add(assetDictionary[assetID]);
            }
        }

        public IEnumerable<uint> GetCurrentlySelectedAssetIDs() => CurrentlySelectedAssets.Select(a => a.assetID);

        public int GetNumberOfSelectedAssets => CurrentlySelectedAssets.Count;

        public void ClearSelectedAssets()
        {
            foreach (var asset in CurrentlySelectedAssets)
                asset.isSelected = false;
            
            CurrentlySelectedAssets.Clear();
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

        public AssetSGRP GetSGRP(uint sgrp)
        {
            if (ContainsAsset(sgrp) && GetFromAssetID(sgrp) is AssetSGRP SGRP)
                return SGRP;
            return null;
        }

        protected AssetID[] defaultJspAssetIds = null;

        private AssetID[] GetJspAssetIDs(uint jspInfo)
        {
            var result = new List<AssetID>();
            var layerIndex = GetLayerFromAssetID(jspInfo);
            for (int i = layerIndex - 3; i < layerIndex; i++)
                if (i > 0 && i < Layers.Count)
                {
                    if (Layers[i].Type == LayerType.BSP)
                    {
                        foreach (var u in Layers[i].AssetIDs)
                            if (GetFromAssetID(u).assetType == AssetType.JSP)
                                result.Add(u);
                    }
                    else if (Layers[i].Type == LayerType.JSPINFO)
                        result.Clear();
                }
            return result.ToArray();
        }

        public Dictionary<LayerType, HashSet<AssetType>> AssetTypesPerLayer()
        {
            var result = new Dictionary<LayerType, HashSet<AssetType>>();
            foreach (var l in Layers)
                result[l.Type] = (from uint a in l.AssetIDs select assetDictionary[a].assetType).Distinct().ToHashSet();
            return result;
        }
    }
}
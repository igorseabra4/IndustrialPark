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
        public static HashSet<IRenderableAsset> renderableAssets = new HashSet<IRenderableAsset>();
        public static HashSet<AssetJSP> renderableJSPs = new HashSet<AssetJSP>();
        public static Dictionary<uint, IAssetWithModel> renderingDictionary = new Dictionary<uint, IAssetWithModel>();
        public static Dictionary<uint, string> nameDictionary = new Dictionary<uint, string>();

        public static void AddToRenderingDictionary(uint assetID, IAssetWithModel value) =>
            renderingDictionary[assetID] = value;

        public static RenderWareModelFile GetFromRenderingDictionary(uint assetID) =>
            renderingDictionary.ContainsKey(assetID) ? renderingDictionary[assetID].GetRenderWareModelFile() : null;
        
        public static void AddToNameDictionary(uint assetID, string value) =>
            nameDictionary[assetID] = value;

        private AutoCompleteStringCollection autoCompleteSource = new AutoCompleteStringCollection();

        public void SetTextboxForAutocomplete(TextBox textBoxFindAsset) =>
            textBoxFindAsset.AutoCompleteCustomSource = autoCompleteSource;

        public bool UnsavedChanges { get; set; } = false;
        public string currentlyOpenFilePath { get; private set; }
        public bool IsNull => hipFile == null;

        protected HipFile hipFile;
        protected Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public Game game => hipFile.game;
        public Platform platform => hipFile.platform;
        protected Section_DICT DICT => hipFile.DICT;

        public bool standalone;

        public bool New()
        {
            var (hipFile, addDefaultAssets) = NewArchive.GetNewArchive();

            if (hipFile != null)
            {
                Dispose();

                currentlySelectedAssets = new List<Asset>();
                currentlyOpenFilePath = null;
                assetDictionary.Clear();

                this.hipFile = hipFile;

                if (platform == Platform.Unknown)
                    new ChoosePlatformDialog().ShowDialog();

                foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR, true);

                DICT.ATOC.AHDRList.Clear();

                if (addDefaultAssets)
                    PlaceDefaultAssets();

                UnsavedChanges = true;
                RecalculateAllMatrices();

                return true;
            }

            return false;
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

            string assetsWithError = "";

            List<string> autoComplete = new List<string>(DICT.ATOC.AHDRList.Count);

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
            {
                string error = AddAssetToDictionary(AHDR, true, skipTexturesAndModels || standalone, false);

                if (error != null)
                    assetsWithError += error + "\n";

                autoComplete.Add(AHDR.ADBG.assetName);

                progressBar.PerformStep();
            }

            DICT.ATOC.AHDRList.Clear();

            if (assetsWithError != "")
                MessageBox.Show("There was an error loading the following assets and editing has been disabled for them:\n" + assetsWithError);

            autoCompleteSource.AddRange(autoComplete.ToArray());

            if (!(skipTexturesAndModels || standalone) && ContainsAssetWithType(AssetType.RWTX))
                SetupTextureDisplay();

            RecalculateAllMatrices();

            if (!skipTexturesAndModels && ContainsAssetWithType(AssetType.PIPT) && ContainsAssetWithType(AssetType.MODL))
                foreach (var asset in assetDictionary.Values)
                    if (asset is AssetPIPT PIPT)
                        PIPT.UpdateDictionary();

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
            foreach (var asset in assetDictionary.Values)
                DICT.ATOC.AHDRList.Add(asset.BuildAHDR());
            File.WriteAllBytes(currentlyOpenFilePath, hipFile.ToBytes());
            DICT.ATOC.AHDRList.Clear();
            UnsavedChanges = false;
        }

        public bool EditPack()
        {
            var (PACK, newPlatform, newGame) = NewArchive.GetExistingArchive(platform, game, hipFile.PACK.PCRT.fileDate, hipFile.PACK.PCRT.dateString);

            if (PACK != null)
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
                renderableAssets.Remove(ra);
                if (renderableJSPs.Contains(ra))
                    renderableJSPs.Remove((AssetJSP)ra);
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
            else if (asset is AssetRWTX rwtx)
                TextureManager.RemoveTexture(rwtx.Name);
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

        public Dictionary<uint, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }

        public int AssetCount => assetDictionary.Values.Count;

        public static bool allowRender = true;

        private string AddAssetToDictionary(Section_AHDR AHDR, bool fast, bool skipTexturesAndModels = false, bool showMessageBox = true)
        {
            allowRender = false;

            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            Asset newAsset;
            string error = null;

            newAsset = CreateAsset(AHDR, skipTexturesAndModels, showMessageBox, ref error);

            //
            var built = newAsset.BuildAHDR().data;
            if (!Enumerable.SequenceEqual(AHDR.data, built))
            {
                string folder = "build_test\\";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string assetName = $"[{AHDR.assetType}] {AHDR.ADBG.assetName}";
                File.WriteAllBytes(folder + assetName + " proper", AHDR.data);
                File.WriteAllBytes(folder + assetName + " wrong", built);
            }
            //

            assetDictionary[AHDR.assetID] = newAsset;

            if (hiddenAssets.Contains(AHDR.assetID))
                assetDictionary[AHDR.assetID].isInvisible = true;

            if (!fast)
                autoCompleteSource.Add(AHDR.ADBG.assetName);

            allowRender = true;

            return error;
        }

        private Asset CreateAsset(Section_AHDR AHDR, bool skipTexturesAndModels, bool showMessageBox, ref string error)
        {
            #if !DEBUG
            try
            {
                #endif
                switch (AHDR.assetType)
                {
                    case AssetType.ANIM:
                        if (AHDR.ADBG.assetName.Contains("ATBL"))
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetANIM(AHDR, game, platform);
                    case AssetType.ATBL:
                        //if (game == Game.Scooby)
                            return new AssetGeneric(AHDR, game, platform);
                        //return new AssetATBL(AHDR, game, platform);
                    case AssetType.BSP:
                    case AssetType.JSP:
                        if (DICT.LTOC.LHDRList[GetLayerFromAssetID(AHDR.assetID)].layerType > 9)
                            return new AssetJSP_INFO(AHDR, game, platform);
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetJSP(AHDR, game, platform, Program.MainForm.renderer);
                    case AssetType.MODL:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetMODL(AHDR, game, platform, Program.MainForm.renderer);
                    case AssetType.RWTX:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetRWTX(AHDR, game, platform);
                    case AssetType.SNDI:
                        if (platform == Platform.GameCube && (game == Game.BFBB || game == Game.Scooby))
                            return new AssetSNDI_GCN_V1(AHDR, game, platform);
                        if (platform == Platform.GameCube)
                            return new AssetSNDI_GCN_V2(AHDR, game, platform);
                        if (platform == Platform.Xbox)
                            return new AssetSNDI_XBOX(AHDR, game, platform);
                        if (platform == Platform.PS2)
                            return new AssetSNDI_PS2(AHDR, game, platform);
                        return new AssetGeneric(AHDR, game, platform);
                    case AssetType.SPLN:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetSPLN(AHDR, game, platform, Program.MainForm.renderer);
                    case AssetType.WIRE:
                        if (skipTexturesAndModels)
                            return new AssetGeneric(AHDR, game, platform);
                        return new AssetWIRE(AHDR, game, platform, Program.MainForm.renderer);
                    case AssetType.ALST: return new AssetALST(AHDR, game, platform);
                    case AssetType.BOUL: return new AssetBOUL(AHDR, game, platform);
                    case AssetType.BUTN: return new AssetBUTN(AHDR, game, platform);
                    case AssetType.CAM:  return new AssetCAM (AHDR, game, platform);
                    case AssetType.CNTR: return new AssetCNTR(AHDR, game, platform);
                    case AssetType.COLL: return new AssetCOLL(AHDR, game, platform);
                    case AssetType.COND: return new AssetCOND(AHDR, game, platform);
                    case AssetType.CRDT: return new AssetCRDT(AHDR, game, platform);
                    //case AssetType.CSN: return new AssetCSN(AHDR, game, platform);
                    case AssetType.CSNM: return new AssetCSNM(AHDR, game, platform);
                    case AssetType.DEST: return new AssetDEST(AHDR, game, platform);
                    case AssetType.DPAT: return new AssetDPAT(AHDR, game, platform);
                    case AssetType.DSCO: return new AssetDSCO(AHDR, game, platform);
                    case AssetType.DSTR: return new AssetDSTR(AHDR, game, platform);
                    case AssetType.DYNA: return CreateDYNA(AHDR);
                    case AssetType.DUPC: return new AssetDUPC(AHDR, game, platform);
                    case AssetType.EGEN: return new AssetEGEN(AHDR, game, platform);
                    case AssetType.ENV:  return new AssetENV (AHDR, game, platform);
                    case AssetType.FLY:  return new AssetFLY (AHDR, game, platform);
                    case AssetType.FOG:  return new AssetFOG (AHDR, game, platform);
                    case AssetType.GRUP: return new AssetGRUP(AHDR, game, platform);
                    case AssetType.GUST: return new AssetGUST(AHDR, game, platform);
                    case AssetType.HANG: return new AssetHANG(AHDR, game, platform);
                    case AssetType.JAW:  return new AssetJAW (AHDR, game, platform);
                    case AssetType.LITE: return new AssetLITE(AHDR, game, platform);
                    case AssetType.LKIT: return new AssetLKIT(AHDR, game, platform);
                    case AssetType.LOBM: return new AssetLOBM(AHDR, game, platform);
                    case AssetType.LODT: return new AssetLODT(AHDR, game, platform);
                    case AssetType.MAPR: return new AssetMAPR(AHDR, game, platform);
                    case AssetType.MINF: return new AssetMINF(AHDR, game, platform);
                    case AssetType.MRKR: return new AssetMRKR(AHDR, game, platform);
                    case AssetType.MVPT: return new AssetMVPT(AHDR, game, platform);
                    case AssetType.NPC:  return new AssetNPC (AHDR, game, platform);
                    case AssetType.PARE: return new AssetPARE(AHDR, game, platform);
                    case AssetType.PARP: return new AssetPARP(AHDR, game, platform);
                    case AssetType.PARS: return new AssetPARS(AHDR, game, platform);
                    case AssetType.PEND: return new AssetPEND(AHDR, game, platform);
                    case AssetType.PGRS: return new AssetPGRS(AHDR, game, platform);
                    case AssetType.PICK: return new AssetPICK(AHDR, game, platform);
                    case AssetType.PIPT: return new AssetPIPT(AHDR, game, platform, UpdateModelBlendModes);
                    case AssetType.PKUP: return new AssetPKUP(AHDR, game, platform);
                    case AssetType.PLAT: return new AssetPLAT(AHDR, game, platform);
                    case AssetType.PLYR: return new AssetPLYR(AHDR, game, platform);
                    case AssetType.PORT: return new AssetPORT(AHDR, game, platform);
                    case AssetType.PRJT: return new AssetPRJT(AHDR, game, platform);
                    case AssetType.SCRP: return new AssetSCRP(AHDR, game, platform);
                    case AssetType.SDFX: return new AssetSDFX(AHDR, game, platform);
                    case AssetType.SFX:  return new AssetSFX (AHDR, game, platform);
                    case AssetType.SGRP: return new AssetSGRP(AHDR, game, platform);
                    case AssetType.TRCK:
                    case AssetType.SIMP: return new AssetSIMP(AHDR, game, platform);
                    case AssetType.SHDW: return new AssetSHDW(AHDR, game, platform);
                    case AssetType.SHRP: return new AssetSHRP(AHDR, game, platform);
                    case AssetType.SURF: return new AssetSURF(AHDR, game, platform);
                    case AssetType.TEXT: return new AssetTEXT(AHDR, game, platform);
                    case AssetType.TRIG: return new AssetTRIG(AHDR, game, platform);
                    case AssetType.TIMR: return new AssetTIMR(AHDR, game, platform);
                    case AssetType.TPIK: return new AssetTPIK(AHDR, game, platform);
                    case AssetType.UI:   return new AssetUI  (AHDR, game, platform);
                    case AssetType.UIFT: return new AssetUIFT(AHDR, game, platform);
                    case AssetType.VIL:  return new AssetVIL (AHDR, game, platform);
                    case AssetType.VILP: return new AssetVILP(AHDR, game, platform);
                    case AssetType.VOLU: return new AssetVOLU(AHDR, game, platform);

                    case AssetType.SND:
                    case AssetType.SNDS:
                        return new AssetWithData(AHDR, game, platform);

                    case AssetType.CCRV:
                    case AssetType.DTRK:
                    case AssetType.GRSM:
                    case AssetType.NGMS:
                    case AssetType.RANM:
                    case AssetType.SLID:
                    case AssetType.SSET:
                    case AssetType.SUBT:
                    case AssetType.TRWT:
                    case AssetType.UIM:
                    case AssetType.ZLIN:
                        return new AssetGenericBase(AHDR, game, platform);

                    case AssetType.ATKT:
                    case AssetType.BINK:
                    case AssetType.CSSS:
                    case AssetType.CTOC:
                    case AssetType.MPHT:
                    case AssetType.NPCS:
                    case AssetType.ONEL:
                    case AssetType.RAW:
                    case AssetType.SPLP:
                    case AssetType.TEXS:
                    case AssetType.UIFN:
                        return new AssetGeneric(AHDR, game, platform);

                    case AssetType.CSN:
                        return new AssetGeneric(AHDR, game, platform);

                    default:
                        throw new Exception($"Unknown asset type ({AHDR.assetType})");
                }
                #if !DEBUG
            }
            catch (Exception ex)
            {
                error = $"[{ AHDR.assetID:X8}] {AHDR.ADBG.assetName}";

                if (showMessageBox)
                    MessageBox.Show($"There was an error loading asset {error}:" + ex.Message + " and editing has been disabled for it.");

                return new AssetGeneric(AHDR, game, platform);
            }
            #endif
        }

        private AssetDYNA CreateDYNA(Section_AHDR AHDR)
        {
            EndianBinaryReader reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = 8;
            DynaType type = (DynaType)reader.ReadUInt32();

            switch (type)
            {
                case DynaType.Enemy__SB__BucketOTron: return new DynaEnemyBucketOTron(AHDR, game, platform);
                case DynaType.Enemy__SB__CastNCrew: return new DynaEnemyCastNCrew(AHDR, game, platform);
                case DynaType.Enemy__SB__Critter: return new DynaEnemyCritter(AHDR, game, platform);
                case DynaType.Enemy__SB__Dennis: return new DynaEnemyDennis(AHDR, game, platform);
                case DynaType.Enemy__SB__FrogFish: return new DynaEnemyFrogFish(AHDR, game, platform);
                case DynaType.Enemy__SB__Mindy: return new DynaEnemyMindy(AHDR, game, platform);
                case DynaType.Enemy__SB__Neptune: return new DynaEnemyNeptune(AHDR, game, platform);
                case DynaType.Enemy__SB__Standard: return new DynaEnemyStandard(AHDR, game, platform);
                case DynaType.Enemy__SB__SupplyCrate: return new DynaEnemySupplyCrate(AHDR, game, platform);
                case DynaType.Enemy__SB__Turret: return new DynaEnemyTurret(AHDR, game, platform);
                case DynaType.Incredibles__Icon: return new DynaIncrediblesIcon(AHDR, game, platform);
                case DynaType.JSPExtraData: return new DynaJSPExtraData(AHDR, game, platform);
                case DynaType.SceneProperties: return new DynaSceneProperties(AHDR, game, platform);
                case DynaType.effect__Lightning: return new DynaEffectLightning(AHDR, game, platform);
                case DynaType.effect__Rumble: return new DynaEffectRumble(AHDR, game, platform);
                case DynaType.effect__RumbleSphericalEmitter: return new DynaEffectRumbleSphere(AHDR, game, platform);
                case DynaType.effect__ScreenFade: return new DynaEffectScreenFade(AHDR, game, platform);
                case DynaType.effect__smoke_emitter: return new DynaEffectSmokeEmitter(AHDR, game, platform);
                case DynaType.effect__spotlight: return new DynaEffectSpotlight(AHDR, game, platform);
                case DynaType.game_object__BoulderGenerator: return new DynaGObjectBoulderGen(AHDR, game, platform);
                case DynaType.game_object__BusStop: return new DynaGObjectBusStop(AHDR, game, platform);
                case DynaType.game_object__Camera_Tweak: return new DynaGObjectCamTweak(AHDR, game, platform);
                case DynaType.game_object__Flythrough: return new DynaGObjectFlythrough(AHDR, game, platform);
                case DynaType.game_object__IN_Pickup: return new DynaGObjectInPickup(AHDR, game, platform);
                case DynaType.game_object__NPCSettings: return new DynaGObjectNPCSettings(AHDR, game, platform);
                case DynaType.game_object__RaceTimer: return new DynaGObjectRaceTimer(AHDR, game, platform);
                case DynaType.game_object__Ring: return new DynaGObjectRing(AHDR, game, platform);
                case DynaType.game_object__RingControl: return new DynaGObjectRingControl(AHDR, game, platform);
                case DynaType.game_object__Taxi: return new DynaGObjectTaxi(AHDR, game, platform);
                case DynaType.game_object__Teleport: return new DynaGObjectTeleport(AHDR, game, platform);
                case DynaType.game_object__Vent: return new DynaGObjectVent(AHDR, game, platform);
                case DynaType.game_object__VentType: return new DynaGObjectVentType(AHDR, game, platform);
                case DynaType.game_object__bungee_drop: return new DynaGObjectBungeeDrop(AHDR, game, platform);
                case DynaType.game_object__bungee_hook: return new DynaGObjectBungeeHook(AHDR, game, platform);
                case DynaType.game_object__flame_emitter: return new DynaGObjectFlameEmitter(AHDR, game, platform);
                case DynaType.game_object__talk_box: return new DynaGObjectTalkBox(AHDR, game, platform);
                case DynaType.game_object__task_box: return new DynaGObjectTaskBox(AHDR, game, platform);
                case DynaType.game_object__text_box: return new DynaGObjectTextBox(AHDR, game, platform);
                case DynaType.hud__meter__font: return new DynaHudMeterFont(AHDR, game, platform);
                case DynaType.hud__meter__unit: return new DynaHudMeterUnit(AHDR, game, platform);
                case DynaType.hud__model: return new DynaHudModel(AHDR, game, platform);
                case DynaType.hud__text: return new DynaHudText(AHDR, game, platform);
                case DynaType.interaction__Launch: return new DynaInteractionLaunch(AHDR, game, platform);
                case DynaType.logic__reference: return new DynaLogicReference(AHDR, game, platform);
                case DynaType.pointer: return new DynaPointer(AHDR, game, platform);
                case DynaType.ui__box: return new DynaUIBox(AHDR, game, platform);
                case DynaType.ui__controller: return new DynaUIController(AHDR, game, platform);
                case DynaType.ui__image: return new DynaUIImage(AHDR, game, platform);
                case DynaType.ui__model: return new DynaUIModel(AHDR, game, platform);
                case DynaType.ui__text: return new DynaUIText(AHDR, game, platform);
                case DynaType.ui__text__userstring: return new DynaUITextUserString(AHDR, game, platform);
                case DynaType.Checkpoint:
                case DynaType.Effect__particle_generator:
                case DynaType.Enemy__SB:
                case DynaType.Interest_Pointer:
                case DynaType.audio__conversation:
                case DynaType.camera__binary_poi:
                case DynaType.camera__preset:
                case DynaType.camera__transition_path:
                case DynaType.camera__transition_time:
                case DynaType.effect__BossBrain:
                case DynaType.effect__Flamethrower:
                case DynaType.effect__LightEffectFlicker:
                case DynaType.effect__LightEffectStrobe:
                case DynaType.effect__RumbleBoxEmitter:
                case DynaType.effect__ScreenWarp:
                case DynaType.effect__Splash:
                case DynaType.effect__Waterhose:
                case DynaType.effect__grass:
                case DynaType.effect__light:
                case DynaType.effect__spark_emitter:
                case DynaType.effect__uber_laser:
                case DynaType.effect__water_body:
                case DynaType.game_object__FreezableObject:
                case DynaType.game_object__Grapple:
                case DynaType.game_object__Hangable:
                case DynaType.game_object__RubbleGenerator:
                case DynaType.game_object__Turret:
                case DynaType.game_object__bullet_mark:
                case DynaType.game_object__bullet_time:
                case DynaType.game_object__camera_param_asset:
                case DynaType.game_object__dash_camera_spline:
                case DynaType.game_object__laser_beam:
                case DynaType.game_object__rband_camera_asset:
                case DynaType.game_object__train_car:
                case DynaType.game_object__train_junction:
                case DynaType.hud__image:
                case DynaType.interaction__IceBridge:
                case DynaType.interaction__Lift:
                case DynaType.interaction__SwitchLever:
                case DynaType.interaction__Turn:
                case DynaType.logic__FunctionGenerator:
                case DynaType.npc__CoverPoint:
                case DynaType.npc__NPC_Custom_AV:
                case DynaType.npc__group:
                case DynaType.Unknown_2CD29541:
                case DynaType.Unknown_4EE03B24:
                case DynaType.Unknown_9F234F8E:
                case DynaType.Unknown_460F4FB2:
                case DynaType.Unknown_2743B85C:
                case DynaType.Unknown_A072A4DA:
                case DynaType.Unknown_AD7CB421:
                case DynaType.Unknown_C6C76EEE:
                case DynaType.Unknown_CDB57387:
                case DynaType.Unknown_CF21DB89:
                case DynaType.Unknown_E5D82D97:
                case DynaType.Unknown_E2301EA9:
                case DynaType.Unknown_EBC04E7B:
                case DynaType.Unknown_FC2951C1:
                case DynaType.Null:
                    return new DynaGeneric(AHDR, type, game, platform);
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
                    AddAsset(layerIndex, AHDR, true);
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

        public uint AddAsset(int layerIndex, Section_AHDR AHDR, bool setTextureDisplay)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            AddAssetToDictionary(AHDR, false);

            if (setTextureDisplay && GetFromAssetID(AHDR.assetID) is AssetRWTX rwtx)
                EnableTextureForDisplay(rwtx);

            return AHDR.assetID;
        }

        public uint AddAssetWithUniqueID(int layerIndex, Section_AHDR AHDR, bool giveIDregardless = false, bool setTextureDisplay = false, bool ignoreNumber = false)
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

                if (!ignoreNumber)
                    AHDR.ADBG.assetName = FindNewAssetName(AHDR.ADBG.assetName, stringToAdd, numCopies);

                AHDR.assetID = BKDRHash(AHDR.ADBG.assetName);
            }

            return AddAsset(layerIndex, AHDR, setTextureDisplay);
        }

        public string FindNewAssetName(string previousName, char stringToAdd, int numCopies)
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

            foreach (Asset asset in currentlySelectedAssets)
            {
                string serializedObject = JsonConvert.SerializeObject(asset.BuildAHDR());
                Section_AHDR AHDR = JsonConvert.DeserializeObject<Section_AHDR>(serializedObject);

                var previousAssetID = AHDR.assetID;

                AddAssetWithUniqueID(layerIndex, AHDR);

                referenceUpdate.Add(previousAssetID, AHDR.assetID);

                finalIndices.Add(AHDR.assetID);
                newAHDRs.Add(AHDR);
            }

            if (updateReferencesOnCopy)
                UpdateReferencesOnCopy(referenceUpdate, newAHDRs);
        }

        public void CopyAssetsToClipboard()
        {
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

                copiedAHDRs.Add(AHDR);
            }

            Clipboard.SetText(JsonConvert.SerializeObject(new AssetClipboard(game, platform.Endianness(), copiedAHDRs), Formatting.Indented));
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

                AddAssetWithUniqueID(layerIndex, clipboard.assets[i]);

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
                Section_AHDR AHDR = clipboard.assets[i];
                string error = "";
                assetDictionary[AHDR.assetID] = CreateAsset(AHDR, standalone, false, ref error);
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
                        AddAsset(layerIndex, AHDR, setTextureDisplay: false);
                    }
                    else
                        AddAssetWithUniqueID(layerIndex, AHDR, setTextureDisplay: true);

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
            foreach (IRenderableAsset a in renderableAssets)
                a.CreateTransformMatrix();
            foreach (AssetJSP a in renderableJSPs)
                a.CreateTransformMatrix();
        }
        
        public void UpdateModelBlendModes(Dictionary<uint, (int, BlendFactorType, BlendFactorType)[]> blendModes)
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
using HipHopFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark.Archive;

// TODO: add asset property changed invokes on all methods in this class, where applicable

public class ArchiveTransformer
{
    private readonly ArchiveSession session;
    private readonly AssetRepository assetRepository;
    private readonly LayerRepository layerRepository;

    public ArchiveTransformer(
        ArchiveSession session,
        AssetRepository assetRepository,
        LayerRepository layerRepository)
    {
        this.session = session;
        this.assetRepository = assetRepository;
        this.layerRepository = layerRepository;
    }

    public bool EditPack(out IEnumerable<string> unconvertable)
    {
        unconvertable = null;

        var PACK = NewArchive.GetExistingArchive(session.Platform, session.Game, session.PACK.PCRT.fileDate, session.PACK.PCRT.dateString);

        if (PACK != null)
        {
            if (session.Platform != PACK.Value.newPlatform || session.Game != PACK.Value.newGame)
                unconvertable = AssetService.GetUnconvertableAssets(assetRepository.Assets);

            session.SetArchiveHeader(PACK.Value.newPlatform, PACK.Value.newGame, PACK.Value.fileDate, PACK.Value.dateString);

            foreach (var asset in assetRepository.Assets)
                Program.EditorManager.CloseEditor(asset);

            return true;
        }

        return false;
    }

    public bool BuildLayers(bool singleLayerOfEachType)
    {
        try
        {
            layerRepository.SetLayers(LayerService.BuildLayers(assetRepository.Assets, session.Game, singleLayerOfEachType));
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to create a layer setup for your archive: {ex.Message}");
            return false;
        }
    }

    public void SetAutoLayers(bool autoLayers)
    {
        if (autoLayers)
        {
            layerRepository.AutoLayers = true;
            layerRepository.ClearLayers();
        }
        else
        {
            layerRepository.AutoLayers = false;
            BuildLayers(false);
        }
    }

    public void ImportHip(string[] fileNames, bool forceOverwrite)
    {
        foreach (string fileName in fileNames)
            ImportHip(fileName, forceOverwrite);
    }

    public IEnumerable<uint> ImportHip(string fileName, bool forceOverwrite)
    {
        if (Path.GetExtension(fileName).Equals(".hip", StringComparison.CurrentCultureIgnoreCase) ||
            Path.GetExtension(fileName).Equals(".hop", StringComparison.CurrentCultureIgnoreCase))
            return ImportHip(HipFile.FromPath(fileName), forceOverwrite);

        if (Path.GetExtension(fileName).Equals(".ini", StringComparison.CurrentCultureIgnoreCase))
            return ImportHip(HipFile.FromINI(fileName), forceOverwrite);

        MessageBox.Show("Invalid file: " + fileName);
        return null;
    }

    public IEnumerable<uint> ImportHip(HipFile hip, bool forceOverwrite)
    {
        if (hip.platform == Platform.Unknown)
            hip.platform = session.Platform;

        void RemoveAssetFromImportedLayers(uint assetID)
        {
            foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                LHDR.assetIDlist.Remove(assetID);
        }

        var assetIDs = new List<uint>();
        var tableManager = new TableAssetManager(session, assetRepository, layerRepository);

        foreach (Section_AHDR AHDR in hip.DICT.ATOC.AHDRList)
        {
            switch (AHDR.assetType)
            {
                case AssetType.CollisionTable when assetRepository.ContainsType(AssetType.CollisionTable):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    tableManager.MergeTableAsset<AssetCOLL, EntryCOLL>(new AssetCOLL(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                case AssetType.JawDataTable when assetRepository.ContainsType(AssetType.JawDataTable):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    tableManager.MergeTableAsset<AssetJAW, EntryJAW>(new AssetJAW(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                case AssetType.LevelOfDetailTable when assetRepository.ContainsType(AssetType.LevelOfDetailTable):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    tableManager.MergeTableAsset<AssetLODT, EntryLODT>(new AssetLODT(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                case AssetType.PipeInfoTable when assetRepository.ContainsType(AssetType.PipeInfoTable):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    tableManager.MergeTableAsset<AssetPIPT, PipeInfo>(new AssetPIPT(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                case AssetType.ShadowTable when assetRepository.ContainsType(AssetType.ShadowTable):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    tableManager.MergeTableAsset<AssetSHDW, EntrySHDW>(new AssetSHDW(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                case AssetType.SoundInfo when assetRepository.ContainsType(AssetType.SoundInfo):
                    RemoveAssetFromImportedLayers(AHDR.assetID);
                    if (hip.platform == Platform.GameCube)
                    {
                        if (hip.game >= Game.Incredibles)
                            tableManager.MergeSoundInfo(new AssetSNDI_GCN_V2(AHDR, hip.game));
                        else
                            tableManager.MergeSoundInfo(new AssetSNDI_GCN_V1(AHDR, hip.game));
                    }
                    else if (hip.platform == Platform.Xbox)
                        tableManager.MergeSoundInfo(new AssetSNDI_XBOX(AHDR, hip.game));
                    else if (hip.platform == Platform.PS2)
                        tableManager.MergeSoundInfo(new AssetSNDI_PS2(AHDR, hip.game));
                    continue;
            }

            if (assetRepository.Contains(AHDR.assetID))
            {
                DialogResult result = forceOverwrite ? DialogResult.Yes :
                MessageBox.Show($"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Asset already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    continue;
                }

                assetRepository.Remove(AHDR.assetID);
                layerRepository.RemoveAsset(AHDR.assetID);
            }

            Asset asset = AssetService.ConvertHeaderToAsset(AHDR, hip.game, hip.platform, true, out AssetError error);
            assetRepository.Add(asset);
            assetIDs.Add(asset.assetID);

            if (asset is AssetJSP_INFO jspInfo)
                jspInfo.JSP_AssetIDs = JSPService.GetDefaultJspAssetIds(hip.DICT, hip.DICT.LTOC.LHDRList.FindIndex(layer => layer.assetIDlist.Contains(AHDR.assetID)), hip.game);
        }

        if (!layerRepository.AutoLayers)
        {
            for (int i = 0; i < hip.DICT.LTOC.LHDRList.Count; i++)
                if (hip.DICT.LTOC.LHDRList[i].assetIDlist.Count != 0)
                {
                    var layer = LayerService.LHDRToLayer(hip.DICT.LTOC.LHDRList[i], hip.game, hip.HIPB.GetLayerName(i));
                    layerRepository.Add(layer);
                }
            layerRepository.SortLayers(session.Game);
        }
        return assetIDs;
    }

    public void ReplaceReferences(uint oldAssetId, uint newAssetId)
    {
        if (oldAssetId == newAssetId)
            return;

        foreach (var asset in assetRepository.Assets)
            if (asset.ReplaceReferences(oldAssetId, newAssetId))
                asset.InvokePropertyChanged(false);
    }

    public List<Asset> CreateJSPInfoAndBSPLayers(List<List<Section_AHDR>> ahdrLists, bool overwrite, Func<uint, bool> ContainsAsset)
    {
        var jspInfos = new List<Asset>();

        foreach (var AHDRs in ahdrLists)
        {
            bool skip = false;
            foreach (var ahdr in AHDRs)
            {
                if (ContainsAsset(ahdr.assetID))
                {
                    if (overwrite)
                    {
                        assetRepository.Remove(ahdr.assetID);
                        layerRepository.RemoveAsset(ahdr.assetID);
                    }
                    else skip = true;
                }
            }
            if (skip)
                continue;

            int layerIndex = layerRepository.GetJSPInfoInsertIndex();

            layerIndex = layerRepository.Add(LayerType.BSP, layerIndex);
            var jsp0 = (AssetJSP)AssetService.ConvertHeaderToAsset(AHDRs[0], session.Game, session.Platform, false, out _);
            assetRepository.Add(jsp0);
            layerRepository.AddAsset(layerIndex, jsp0.assetID);
            List<AssetJSP> bsps = [jsp0];

            layerIndex++;

            layerIndex = layerRepository.Add(LayerType.BSP, layerIndex);
            if (AHDRs.Count > 1)
            {
                var jsp1 = (AssetJSP)AssetService.ConvertHeaderToAsset(AHDRs[1], session.Game, session.Platform, false, out _);
                assetRepository.Add(jsp1);
                layerRepository.AddAsset(layerIndex, jsp1.assetID);
                bsps.Add(jsp1);
            }

            layerIndex++;

            layerIndex = layerRepository.Add(LayerType.BSP, layerIndex);
            if (AHDRs.Count > 2)
            {
                var jsp2 = (AssetJSP)AssetService.ConvertHeaderToAsset(AHDRs[2], session.Game, session.Platform, false, out _);
                assetRepository.Add(jsp2);
                layerRepository.AddAsset(layerIndex, jsp2.assetID);
                bsps.Add(jsp2);
            }

            layerIndex++;

            layerIndex = layerRepository.Add(LayerType.JSPINFO, layerIndex);

            AssetJSP_INFO info = new AssetJSP_INFO(AHDRs[0].ADBG.assetName[..^1], session.Game, session.Platform)
            {
                JSP_AssetIDs = [.. AHDRs.Select(a => (AssetID)a.assetID)]
            };
            JSPService.GenerateJSPInfo(session.Game, session.Platform, info, bsps);

            if (ContainsAsset(info.assetID))
            {
                assetRepository.Remove(info.assetID);
                layerRepository.RemoveAsset(info.assetID);
            }

            assetRepository.Add(info);
            layerRepository.AddAsset(layerIndex, info.assetID);

            jspInfos.Add(info);
        }

        return jspInfos;
    }

    public List<Asset> MakeSimps(List<Asset> models, bool solid, bool ledgeGrabSimps)
    {
        int layerIndex = layerRepository.GetOrCreateLayerOfType(LayerType.DEFAULT);
        var result = new List<Asset>();

        foreach (var model in models)
        {
            string simpName = "SIMP_" + model.assetName.Replace(".dff", "").ToUpper();

            if (assetRepository.Contains(new AssetID(simpName)))
                continue;

            var simp = (AssetSIMP)AssetFactory.GenerateTemplate(session.Game, session.Platform, assetRepository.Contains, AssetTemplate.Simple_Object, simpName, Vector3.Zero, true)[0];
            simp.Model = model.assetID;
            if (!solid)
            {
                simp.CollType.FlagValueByte = 0;
            }
            else if (ledgeGrabSimps)
            {
                simp.SolidityFlags.FlagValueByte = 0x82;
            }
            result.Add(simp);

            assetRepository.Add(simp);
            layerRepository.AddAsset(layerIndex, simp.assetID);
        }
        return result;
    }

    public void MakePiptVcolors(List<Asset> models)
    {
        bool containsPipt = assetRepository.ContainsType(AssetType.PipeInfoTable);
        var pipt = new TableAssetManager(session, assetRepository, layerRepository).GetPIPT(true);

        List<PipeInfo> entries = [.. pipt.Entries];

        models = [.. models.OfType<AssetMODL>().Where(asset => entries.All(e => e.Model != asset.assetID))];

        foreach (var asset in models)
            entries.Add(new PipeInfo(session.Game)
            {
                Model = asset.assetID,
                LightingMode = LightingMode.Prelight
            });
        pipt.Entries = [.. entries];
    }

    /// <summary>
    /// Generates a report for the HIP/HOP archive to a .txt in the same directory.
    /// </summary>
    /// 
    public static void GenerateReportTxt(Archive archive, bool UnsavedChanges)
    {
        if (archive == null)
        {
            // TODO: Disable the menu button if no file is open
            MessageBox.Show("No file is currently open.");
            return;
        }

        bool generationWasSuccessful = true;
        bool isDebug;
        var timeOfGeneration = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zzz");
        string currentFilepath = archive.CurrentFilePath;
        var stopwatch = new Stopwatch();
        var ConvertSize = ArchiveEditor.ConvertSize;

        // TODO: Change filetype to LIP or LOP
        string filepath = Path.Combine(
            Path.GetDirectoryName(currentFilepath),
            Path.GetFileName(currentFilepath) + "_IP_Report.txt");

#if DEBUG
        HipFile hip = HipFile.FromPath(currentFilepath);
        if (!File.Exists(currentFilepath))
        {
            MessageBox.Show($"HIP File \"{currentFilepath}\" not found");
            return;
        }
#endif

        using (StreamWriter output = new StreamWriter(filepath))
        {
            stopwatch.Start();
            output.WriteLine("This report file was generated by Industrial Park.");
            output.WriteLine("Please visit https://github.com/igorseabra4/IndustrialPark for more information.");
            output.WriteLine("---------------------------");
            output.WriteLine("#### START OF REPORT ####\n");
            output.WriteLine($"Industrial Park Version: {new IPversion().version}");
#if DEBUG
            isDebug = true;
#else
            isDebug = false;
#endif
            output.WriteLine($"Is Debug Build: {isDebug}");
            output.WriteLine($"Time of Report Generation: {timeOfGeneration}");

            output.WriteLine("\nFILE INFORMATION");
            output.WriteLine("----------------");
            output.WriteLine($"File path: {currentFilepath}");
            output.WriteLine($"File size: {new FileInfo(currentFilepath).Length} Bytes");
            output.WriteLine($"Platform: {archive.Platform}");
            string game = archive.Game == Game.Incredibles ? "Incredibles/TSSM" : archive.Game.ToString();
            output.WriteLine($"Game: {game}");
            output.WriteLine($"Number of Layers: {archive.LayerCount}");
            output.WriteLine($"Number of Assets: {archive.AssetCount}");
            output.WriteLine($"Has unsaved changes: {UnsavedChanges}");
            if (archive.Assets.OfType<AssetENV>().FirstOrDefault() is AssetENV env)
                output.WriteLine($"Environment Name: {env.assetName}");

            var allAssets = archive.Assets;
            Dictionary<AssetType, int> assetCount = new Dictionary<AssetType, int>();

            output.WriteLine("\nTYPE TOTALS");
            output.WriteLine("-------------");

            foreach (AssetType assetType in Enum.GetValues(typeof(AssetType)))
            {
                assetCount[assetType] = 0;
            }

            foreach (var asset in allAssets)
            {
                assetCount[asset.assetType] += 1;
            }

            foreach (KeyValuePair<AssetType, int> entry in assetCount)
            {
                if (entry.Value != 0)
                {
                    output.WriteLine($"{entry.Key.ToString().PadRight(30, '.')}{entry.Value}");
                }
            }

#if DEBUG
            Dictionary<uint, Section_AHDR> ahdrList = new Dictionary<uint, Section_AHDR>();
            foreach (var ahdr in hip.DICT.ATOC.AHDRList)
                ahdrList.Add(ahdr.assetID, ahdr);

            int padding = archive.Platform == Platform.GameCube ? 0x20 : 0x800;

            output.WriteLine("\n\nVerbose Layer Information\n------------------\n");
            int i = 0;
            foreach (var lhdr in hip.DICT.LTOC.LHDRList)
            {
                LayerType type = (archive.Game >= Game.Incredibles || lhdr.layerType < 2) ? (LayerType)lhdr.layerType : (LayerType)(lhdr.layerType + 1);

                output.WriteLine($"Layer #{i} is Type {lhdr.layerType} ({type})");
                output.WriteLine("-------------------");
                output.WriteLine("AssetName                       AssetID  Checksum Type Offset   Size     Plus Align");
                output.WriteLine("---------                       -------  -------- ---- ------   ----     ---- -----");

                int layerSizeTotal = 0, layerSizeDataAsset = 0, layerSizeDataLayer = 0, layerSizePaddingAsset = 0, layerSizePaddingLayer = 0;
                int sndSize = 0, sndsSize = 0;

                foreach (uint aid in lhdr.assetIDlist)
                {
                    Section_AHDR ahdr = ahdrList[aid];
                    output.Write(ahdr.ADBG.assetName.PadRight(32, ' '));
                    output.Write(ahdr.assetID.ToString("X8") + ' ');
                    output.Write(ahdr.ADBG.checksum.ToString("X8") + ' ');
                    output.Write(Functions.GetCode(ahdr.assetType).PadRight(5, ' '));
                    output.Write(ahdr.fileOffset.ToString("X8").PadRight(9, ' '));
                    output.Write(ahdr.fileSize.ToString("X").PadRight(9, ' '));
                    output.Write(ahdr.plusValue.ToString().PadRight(5, ' '));
                    output.Write(ahdr.ADBG.alignment.ToString().PadRight(6, ' ') + "\n");

                    if (ahdr.assetType == AssetType.Sound)
                        sndSize += ahdr.fileSize + ahdr.plusValue;
                    else if (ahdr.assetType == AssetType.SoundStream)
                        sndsSize += ahdr.fileSize + ahdr.plusValue;

                    layerSizeTotal += ahdr.fileSize + ahdr.plusValue;
                    layerSizeDataAsset += ahdr.fileSize;
                    layerSizePaddingAsset += ahdr.plusValue;
                    layerSizeDataLayer += ahdr.fileSize + ahdr.plusValue;
                }

                layerSizePaddingLayer = (padding - (layerSizeTotal % padding)) % padding;
                layerSizeTotal += layerSizePaddingLayer;

                if (archive.Game <= Game.BFBB && lhdr.layerType == 6 || archive.Game >= Game.Incredibles && lhdr.layerType == 7) // SRAM
                {
                    output.WriteLine($"\nSND Size (bytes): {sndSize} ({ConvertSize(sndSize)})");
                    output.WriteLine($"SNDS Size (bytes): {sndsSize} ({ConvertSize(sndsSize)})");
                }

                output.WriteLine($"\nLayer Size (Total):         {layerSizeTotal} ({ConvertSize(layerSizeTotal)})");
                output.WriteLine($"Layer Size (Data_Layer):    {layerSizeDataLayer} ({ConvertSize(layerSizeDataLayer)})");
                output.WriteLine($"Layer Size (Data_Asset):    {layerSizeDataAsset} ({ConvertSize(layerSizeDataAsset)})");
                output.WriteLine($"Layer Size (Padding_Asset): {layerSizePaddingAsset} ({ConvertSize(layerSizePaddingAsset)})");
                output.WriteLine($"Layer Size (Padding_Layer): {layerSizePaddingLayer} ({ConvertSize(layerSizePaddingLayer)})\n\n");
                i++;
            }
#else
            output.WriteLine("\nLAYER INFORMATION");
            output.WriteLine("-------------");

            List<uint> assetIDsOnLayer;
            List<AssetType> assetTypesOnLayer;
            int numOfInvisibleAssets = 0;


            for (int i = 0; i < archive.LayerCount; i++)
            {
                assetTypesOnLayer = archive.AssetTypesOnLayer(i).ToList();
                output.WriteLine($"{archive.LayerToString(i)}");
                output.Write($"{assetTypesOnLayer.Count} asset type(s) [");
                foreach (var assetType in assetTypesOnLayer)
                {
                    output.Write($"{assetType} ");
                }
                output.WriteLine("]");

                assetIDsOnLayer = archive.GetLayerAssetIDs(i).ToList();

                for (int j = 0; j < assetIDsOnLayer.Count; j++)
                {
                    Asset asset = archive.GetAsset(assetIDsOnLayer[j]);

                    output.WriteLine($"    {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                    if (asset.isInvisible)
                        numOfInvisibleAssets++;
                }
                output.WriteLine("");
            }
            output.WriteLine($"Number of invisible assets: {numOfInvisibleAssets}");
            output.WriteLine("");
#endif

            output.WriteLine("\nASSET LINKING INFO");
            output.WriteLine("-------------");


            foreach (var asset in allAssets)
            {
                // If Base asset, get list of links.
                if (asset is BaseAsset baseAsset)
                {
                    Link[] links = baseAsset.Links;
                    if (links.Length > 0)
                    {
                        output.WriteLine($"{asset.assetName}: {links.Length} links");

                        foreach (var link in links)
                        {
                            string eventSendIDName = "";
                            string eventReceiveIDName = "";

                            if (archive.Game == Game.BFBB)
                            {
                                eventSendIDName = ((EventBFBB)link.EventSendID).ToString();
                                eventReceiveIDName = ((EventBFBB)link.EventReceiveID).ToString();
                            }
                            else if (archive.Game == Game.Scooby)
                            {
                                eventSendIDName = ((EventScooby)link.EventSendID).ToString();
                                eventReceiveIDName = ((EventScooby)link.EventReceiveID).ToString();
                            }
                            else if (archive.Game == Game.Incredibles) // TSSM/Incredibles
                            {
                                eventSendIDName = ((EventTSSM)link.EventSendID).ToString();
                                eventReceiveIDName = ((EventTSSM)link.EventReceiveID).ToString();
                            }
                            else if (archive.Game == Game.ROTU)
                            {
                                eventSendIDName = ((EventROTU)link.EventSendID).ToString();
                                eventReceiveIDName = ((EventROTU)link.EventReceiveID).ToString();
                            }
                            else if (archive.Game == Game.RatProto)
                            {
                                eventSendIDName = ((EventRatProto)link.EventSendID).ToString();
                                eventReceiveIDName = ((EventRatProto)link.EventReceiveID).ToString();
                            }

                            // If event name not supplied, event ID used instead.
                            eventSendIDName = eventSendIDName == "" ? link.EventSendID.ToString() : eventSendIDName;
                            eventReceiveIDName = eventReceiveIDName == "" ? link.EventReceiveID.ToString() : eventReceiveIDName;

                            string targetAssetName = "";
                            try
                            {
                                // FIXME: Only searches current archive
                                targetAssetName = archive.GetAsset(link.TargetAsset).assetName;
                            }
                            catch
                            {
                                targetAssetName = link.TargetAsset.ToString();
                            }

                            output.WriteLine($"  {eventSendIDName} => {eventReceiveIDName} => {targetAssetName}");
                        }
                    }
                }
            }

            output.WriteLine("\nUNUSED MODELS");
            output.WriteLine("-------------");

            int numOfModels = 0;
            int numOfUnusedModels = 0;

            foreach (var asset in allAssets)
            {
                if (asset.assetType == AssetType.Model)
                {
                    numOfModels++;

                    var whoTargets = AssetService.WhoTargets(asset.assetID);
                    if (!whoTargets.Any())
                    {
                        output.WriteLine($"  {Convert.ToString(asset.assetID, 16)}: {asset.assetName}");
                        numOfUnusedModels++;
                    }
                }
            }

            output.WriteLine($"{numOfUnusedModels} unused models out of {numOfModels} total.");

            // Find who Targets Me throws an exception sometimes :(

            output.WriteLine("\n#### END OF REPORT ####");
            stopwatch.Stop();
            output.WriteLine($"Report generated in {Math.Round(stopwatch.Elapsed.TotalMilliseconds, 2)} ms.");
        }

        if (generationWasSuccessful)
        {
            if (MessageBox.Show($"Report \"{Path.GetFileName(filepath)}\" generated. Open file?",
                "Report generated",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo { FileName = filepath, UseShellExecute = true });
            }
        }
        else
        {
            MessageBox.Show("File could not be generated successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public string VerifyArchive(IProgressBar progressBar)
    {
        List<string> result = new List<string>();

        progressBar.SetProgressBar(0, assetRepository.Assets.Count() + 1, 1);

        List<Asset> ordered = assetRepository.Assets.OrderBy(f => f.assetName).ToList();
        ordered = ordered.OrderBy(f => f.assetType.ToString()).ToList();

        progressBar.PerformStep();

        foreach (Asset asset in ordered)
        {
            try
            {
                var resultAsset = new List<string>();
                asset.Verify(ref resultAsset);
                foreach (string s in resultAsset)
                    result.Add($"[{AssetTypeContainer.AssetTypeToString(asset.assetType)}] {asset.assetName}: " + s);
            }
            catch (Exception e)
            {
                result.Add($"Failed verification on [{asset.assetType}] {asset.assetName}: " + e.Message);
            }

            progressBar.PerformStep();
        }

        return string.Join("\n", result);
    }
}

using HipHopFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static string editorFilesFolder => Application.StartupPath +
            "/Resources/IndustrialPark-EditorFiles/IndustrialPark-EditorFiles-master/";

        public void ExportHip(string fileName)
        {
            try
            {
                BuildHipFile().ToIni(fileName, true, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ImportHip(string[] fileNames, bool forceOverwrite)
        {
            foreach (string fileName in fileNames)
                ImportHip(fileName, forceOverwrite);
        }

        public void ImportHip(string fileName, bool forceOverwrite)
        {
            if (Path.GetExtension(fileName).ToLower() == ".hip" || Path.GetExtension(fileName).ToLower() == ".hop")
                ImportHip(HipFile.FromPath(fileName), forceOverwrite);
            else if (Path.GetExtension(fileName).ToLower() == ".ini")
                ImportHip(HipFile.FromINI(fileName), forceOverwrite);
            else
                MessageBox.Show("Invalid file: " + fileName);
        }

        public void ImportHip(HipFile hip, bool forceOverwrite)
        {
            if (hip.platform == Platform.Unknown)
                hip.platform = platform;

            void RemoveAssetFromImportedLayers(uint assetID)
            {
                foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                    LHDR.assetIDlist.Remove(assetID);
            }

            UnsavedChanges = true;
            var actions = new List<IReversibleAction>();
            var addedAssets = new List<uint>();

            foreach (Section_AHDR AHDR in hip.DICT.ATOC.AHDRList)
            {
                switch (AHDR.assetType)
                {
                    case AssetType.CollisionTable when ContainsAssetWithType(AssetType.CollisionTable):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        MergeTableAsset<AssetCOLL, EntryCOLL>(new AssetCOLL(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                    case AssetType.JawDataTable when ContainsAssetWithType(AssetType.JawDataTable):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        MergeTableAsset<AssetJAW, EntryJAW>(new AssetJAW(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                    case AssetType.LevelOfDetailTable when ContainsAssetWithType(AssetType.LevelOfDetailTable):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        MergeTableAsset<AssetLODT, EntryLODT>(new AssetLODT(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                    case AssetType.PipeInfoTable when ContainsAssetWithType(AssetType.PipeInfoTable):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        MergeTableAsset<AssetPIPT, PipeInfo>(new AssetPIPT(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                    case AssetType.ShadowTable when ContainsAssetWithType(AssetType.ShadowTable):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        MergeTableAsset<AssetSHDW, EntrySHDW>(new AssetSHDW(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                    case AssetType.SoundInfo when ContainsAssetWithType(AssetType.SoundInfo):
                        RemoveAssetFromImportedLayers(AHDR.assetID);
                        if (hip.platform == Platform.GameCube)
                        {
                            if (hip.game >= Game.Incredibles)
                                MergeSoundInfo(new AssetSNDI_GCN_V2(AHDR, hip.game), actions);
                            else
                                MergeSoundInfo(new AssetSNDI_GCN_V1(AHDR, hip.game, hip.platform.Endianness()), actions);
                        }
                        else if (hip.platform == Platform.Xbox)
                            MergeSoundInfo(new AssetSNDI_XBOX(AHDR, hip.game, hip.platform.Endianness()), actions);
                        else if (hip.platform == Platform.PS2)
                            MergeSoundInfo(new AssetSNDI_PS2(AHDR, hip.game, hip.platform.Endianness()), actions);
                        continue;
                }

                bool added = false;
                if (ContainsAsset(AHDR.assetID))
                {
                    DialogResult result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        added = true;
                        actions.Add(GetAssetRemovedAction(AHDR.assetID));
                        RemoveAsset(AHDR.assetID, false);
                        AddAssetToDictionary(AHDR, hip.game, hip.platform.Endianness(), forceOverwrite, true);
                        addedAssets.Add(AHDR.assetID);
                    }
                    else
                        foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                }
                else
                {
                    added = true;
                    AddAssetToDictionary(AHDR, hip.game, hip.platform.Endianness(), forceOverwrite, true);
                    addedAssets.Add(AHDR.assetID);
                }

                if (added && GetFromAssetID(AHDR.assetID) is AssetJSP_INFO jspInfo)
                    jspInfo.JSP_AssetIDs = GetDefaultJspAssetIds(hip.DICT, hip.DICT.LTOC.LHDRList.FindIndex(layer => layer.assetIDlist.Contains(AHDR.assetID)), hip.game);
            }

            if (!NoLayers)
            {
                for (int i = 0; i < hip.DICT.LTOC.LHDRList.Count; i++)
                    if (hip.DICT.LTOC.LHDRList[i].assetIDlist.Count != 0)
                    {
                        var layer = LHDRToLayer(hip.DICT.LTOC.LHDRList[i], hip.game, hip.HIPB.GetLayerName(i));
                        Layers.Add(layer);
                        actions.Add(new LayerAddedAction(this, layer.Type, Layers.Count - 1));
                    }
                List<Layer> layersOld = [.. Layers];
                Layers = Layers.OrderBy(f => LayerTypeGenericToSpecific(f.Type, game), new LayerComparer(game)).ToList();
                actions.Add(new AllLayersMovedAction(this, layersOld, Layers));
            }

            foreach (var u in addedAssets)
                actions.Add(GetAssetAddedAction(u));

            Program.UndoBuffer.AddAction(actions);

            RecalculateAllMatrices();
        }

        private AssetID[] GetDefaultJspAssetIds(Section_DICT DICT, int jspInfoLayerIndex, Game game)
        {
            var result = new List<AssetID>();
            for (int j = jspInfoLayerIndex - 3; j < jspInfoLayerIndex; j++)
                if (j >= 0 && j < DICT.LTOC.LHDRList.Count)
                {
                    var layerType = LayerTypeSpecificToGeneric(DICT.LTOC.LHDRList[j].layerType, game);
                    if (layerType == LayerType.BSP)
                    {
                        foreach (var u in DICT.LTOC.LHDRList[j].assetIDlist)
                            foreach (var asset in DICT.ATOC.AHDRList.Where(a => a.assetID == u))
                                if (asset.assetType == AssetType.JSP)
                                    result.Add(u);
                    }
                    else
                        result.Clear();
                }
            return [.. result];
        }
    }
}
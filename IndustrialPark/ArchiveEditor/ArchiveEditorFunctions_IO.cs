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

            UnsavedChanges = true;

            foreach (Section_AHDR AHDR in hip.DICT.ATOC.AHDRList)
            {
                defaultJspAssetIds = null;

                if (AHDR.assetType == AssetType.CollisionTable && ContainsAssetWithType(AssetType.CollisionTable))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeCOLL(new AssetCOLL(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.JawDataTable && ContainsAssetWithType(AssetType.JawDataTable))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeJAW(new AssetJAW(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.LevelOfDetailTable && ContainsAssetWithType(AssetType.LevelOfDetailTable))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeLODT(new AssetLODT(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.PipeInfoTable && ContainsAssetWithType(AssetType.PipeInfoTable))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergePIPT(new AssetPIPT(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.ShadowTable && ContainsAssetWithType(AssetType.ShadowTable))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeSHDW(new AssetSHDW(AHDR, hip.game, hip.platform.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.SoundInfo && ContainsAssetWithType(AssetType.SoundInfo))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    if (hip.platform == Platform.GameCube)
                    {
                        if (hip.game >= Game.Incredibles)
                            MergeSNDI(new AssetSNDI_GCN_V2(AHDR, hip.game));
                        else
                            MergeSNDI(new AssetSNDI_GCN_V1(AHDR, hip.game, hip.platform.Endianness()));
                    }
                    else if (hip.platform == Platform.Xbox)
                        MergeSNDI(new AssetSNDI_XBOX(AHDR, hip.game, hip.platform.Endianness()));
                    else if (hip.platform == Platform.PS2)
                        MergeSNDI(new AssetSNDI_PS2(AHDR, hip.game, hip.platform.Endianness()));

                    continue;
                }
                else if (AHDR.assetType == AssetType.JSPInfo)
                {
                    for (int i = 0; i < hip.DICT.LTOC.LHDRList.Count; i++)
                        if (hip.DICT.LTOC.LHDRList[i].assetIDlist.Contains(AHDR.assetID))
                        {
                            setDefaultJspAssetIds(hip.DICT, i);
                            break;
                        }
                }

                if (ContainsAsset(AHDR.assetID))
                {
                    DialogResult result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        RemoveAsset(AHDR.assetID, false);
                        AddAssetToDictionary(AHDR, hip.game, hip.platform.Endianness(), forceOverwrite, true);
                    }
                    else
                        foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                }
                else
                {
                    AddAssetToDictionary(AHDR, hip.game, hip.platform.Endianness(), forceOverwrite, true);
                }
            }

            defaultJspAssetIds = null;

            if (!NoLayers)
            {
                for (int i = 0; i < hip.DICT.LTOC.LHDRList.Count; i++)
                    if (hip.DICT.LTOC.LHDRList[i].assetIDlist.Count != 0)
                        Layers.Add(LHDRToLayer(hip.DICT.LTOC.LHDRList[i], hip.game, hip.HIPB.GetLayerName(i)));
                Layers = Layers.OrderBy(f => (int)f.Type, new LayerComparer(game)).ToList();
            }

            if (!forceOverwrite)
                RecalculateAllMatrices();
        }

        private void setDefaultJspAssetIds(Section_DICT DICT, int jspInfoLayerIndex)
        {
            var result = new List<AssetID>();
            for (int j = jspInfoLayerIndex - 3; j < jspInfoLayerIndex; j++)
                if (j > 0 && j < DICT.LTOC.LHDRList.Count)
                {
                    LayerType layerType;
                    if (game >= Game.Incredibles || DICT.LTOC.LHDRList[j].layerType < 2)
                        layerType = (LayerType)DICT.LTOC.LHDRList[j].layerType;
                    else
                        layerType = (LayerType)(DICT.LTOC.LHDRList[j].layerType + 1);

                    if (layerType == LayerType.BSP)
                    {
                        foreach (var u in DICT.LTOC.LHDRList[j].assetIDlist)
                        {
                            foreach (var asset in DICT.ATOC.AHDRList.Where(a => a.assetID == u))
                                if (asset.assetType == AssetType.JSP)
                                    result.Add(u);
                        }
                    }
                    else if (layerType == LayerType.JSPINFO)
                        result.Clear();
                }
            defaultJspAssetIds = result.ToArray();
        }
    }
}
using HipHopFile;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static string editorFilesFolder => Application.StartupPath +
            "/Resources/IndustrialPark-EditorFiles/IndustrialPark-EditorFiles-master/";

        public void ExportHip(string fileName)
        {
            hipFile.ToIni(fileName, true, true);
        }
        
        public void ImportHip(string[] fileNames, bool forceOverwrite, List<uint> assetIDs = null)
        {
            foreach (string fileName in fileNames)
                ImportHip(fileName, forceOverwrite, assetIDs);
        }

        public void ImportHip(string fileName, bool forceOverwrite, List<uint> assetIDs = null)
        {
            if (Path.GetExtension(fileName).ToLower() == ".hip" || Path.GetExtension(fileName).ToLower() == ".hop")
                ImportHip(new HipFile(fileName), forceOverwrite, assetIDs);
            else if (Path.GetExtension(fileName).ToLower() == ".ini")
                ImportHip(HipFile.FromINI(fileName), forceOverwrite, assetIDs);
            else
                MessageBox.Show("Invalid file: " + fileName);
        }

        public void ImportHip(HipFile hip, bool forceOverwrite, List<uint> missingAssets)
        {
            UnsavedChanges = true;
            forceOverwrite |= missingAssets != null;

            foreach (Section_AHDR AHDR in hip.DICT.ATOC.AHDRList)
            {
                if (AHDR.assetType == AssetType.COLL && ContainsAssetWithType(AssetType.COLL))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeCOLL(new AssetCOLL(AHDR, hip.game, hip.platform));
                    continue;
                }
                else if (AHDR.assetType == AssetType.JAW && ContainsAssetWithType(AssetType.JAW))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeJAW(new AssetJAW(AHDR, hip.game, hip.platform));
                    continue;
                }
                else if (AHDR.assetType == AssetType.LODT && ContainsAssetWithType(AssetType.LODT))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeLODT(new AssetLODT(AHDR, hip.game, hip.platform));
                    continue;
                }
                else if (AHDR.assetType == AssetType.PIPT && ContainsAssetWithType(AssetType.PIPT))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergePIPT(new AssetPIPT(AHDR, hip.game, hip.platform));
                    continue;
                }
                else if (AHDR.assetType == AssetType.SHDW && ContainsAssetWithType(AssetType.SHDW))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeSHDW(new AssetSHDW(AHDR, hip.game, hip.platform));
                    continue;
                }
                else if (AHDR.assetType == AssetType.SNDI && ContainsAssetWithType(AssetType.SNDI))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    if (hip.platform == Platform.GameCube)
                    {
                        if (hip.game == Game.Incredibles)
                            MergeSNDI(new AssetSNDI_GCN_V2(AHDR, hip.game, hip.platform));
                        else
                            MergeSNDI(new AssetSNDI_GCN_V1(AHDR, hip.game, hip.platform));
                    }
                    else if (hip.platform == Platform.Xbox)
                        MergeSNDI(new AssetSNDI_XBOX(AHDR, hip.game, hip.platform));
                    else if (hip.platform == Platform.PS2)
                        MergeSNDI(new AssetSNDI_PS2(AHDR, hip.game, hip.platform));

                    continue;
                }

                if (missingAssets != null && !missingAssets.Contains(AHDR.assetID))
                {
                    foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);
                    continue;
                }

                if (ContainsAsset(AHDR.assetID) && (missingAssets == null || missingAssets.Contains(AHDR.assetID)))
                {
                    DialogResult result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        RemoveAsset(AHDR.assetID, false);
                        AddAssetToDictionary(AHDR, forceOverwrite, forceOverwrite);
                    }
                    else
                        foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                    if (missingAssets != null)
                        missingAssets.Remove(AHDR.assetID);
                }
                else if (missingAssets == null)
                {
                    AddAssetToDictionary(AHDR, forceOverwrite, forceOverwrite);
                }
            }

            foreach (Section_LHDR LHDR in hip.DICT.LTOC.LHDRList)
                if (LHDR.assetIDlist.Count != 0)
                    DICT.LTOC.LHDRList.Add(LHDR);

            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer(game)).ToList();

            if (!forceOverwrite)
                RecalculateAllMatrices();
        }

        public void ReplaceUnconvertableAssets(string fileName, ref List<uint> missing)
        {
            var hipHops = new List<string>();
            SearchOnFolder(fileName, ref hipHops);

            foreach (string s in hipHops)
            {
                ImportHip(s, true, missing);
                if (missing.Count == 0)
                    break;
            }

            CleanSNDI();
        }

        private void SearchOnFolder(string folderPath, ref List<string> hipHops)
        {
            foreach (string s in Directory.GetFiles(folderPath))
                if (Path.GetExtension(s).ToLower() == ".hip" || Path.GetExtension(s).ToLower() == ".hop")
                    hipHops.Add(s);
            foreach (string s in Directory.GetDirectories(folderPath))
                SearchOnFolder(s, ref hipHops);
        }
    }
}
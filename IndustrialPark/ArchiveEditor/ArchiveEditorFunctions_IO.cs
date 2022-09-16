using HipHopFile;
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
            BuildHipFile().ToIni(game, fileName, true, true);
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

        public void ImportHip((HipFile, Game, Platform) hip, bool forceOverwrite)
        {
            if (hip.Item3 == Platform.Unknown)
                hip.Item3 = platform;

            UnsavedChanges = true;

            foreach (Section_AHDR AHDR in hip.Item1.DICT.ATOC.AHDRList)
            {
                if (AHDR.assetType == AssetType.CollisionTable && ContainsAssetWithType(AssetType.CollisionTable))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeCOLL(new AssetCOLL(AHDR, hip.Item2, hip.Item3.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.JawDataTable && ContainsAssetWithType(AssetType.JawDataTable))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeJAW(new AssetJAW(AHDR, hip.Item2, hip.Item3.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.LevelOfDetailTable && ContainsAssetWithType(AssetType.LevelOfDetailTable))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeLODT(new AssetLODT(AHDR, hip.Item2, hip.Item3.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.PipeInfoTable && ContainsAssetWithType(AssetType.PipeInfoTable))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergePIPT(new AssetPIPT(AHDR, hip.Item2, hip.Item3.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.ShadowTable && ContainsAssetWithType(AssetType.ShadowTable))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeSHDW(new AssetSHDW(AHDR, hip.Item2, hip.Item3.Endianness()));
                    continue;
                }
                else if (AHDR.assetType == AssetType.SoundInfo && ContainsAssetWithType(AssetType.SoundInfo))
                {
                    foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    if (hip.Item3 == Platform.GameCube)
                    {
                        if (hip.Item2 == Game.Incredibles)
                            MergeSNDI(new AssetSNDI_GCN_V2(AHDR, hip.Item2, hip.Item3.Endianness()));
                        else
                            MergeSNDI(new AssetSNDI_GCN_V1(AHDR, hip.Item2, hip.Item3.Endianness()));
                    }
                    else if (hip.Item3 == Platform.Xbox)
                        MergeSNDI(new AssetSNDI_XBOX(AHDR, hip.Item2, hip.Item3.Endianness()));
                    else if (hip.Item3 == Platform.PS2)
                        MergeSNDI(new AssetSNDI_PS2(AHDR, hip.Item2, hip.Item3.Endianness()));

                    continue;
                }

                if (ContainsAsset(AHDR.assetID))
                {
                    DialogResult result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{AHDR.assetID:X8}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        RemoveAsset(AHDR.assetID, false);
                        AddAssetToDictionary(AHDR, hip.Item2, hip.Item3.Endianness(), forceOverwrite, true);
                    }
                    else
                        foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                }
                else
                {
                    AddAssetToDictionary(AHDR, hip.Item2, hip.Item3.Endianness(), forceOverwrite, true);
                }
            }

            if (!NoLayers)
            {
                foreach (Section_LHDR LHDR in hip.Item1.DICT.LTOC.LHDRList)
                    if (LHDR.assetIDlist.Count != 0)
                        Layers.Add(LHDRToLayer(LHDR));
                Layers = Layers.OrderBy(f => (int)f.Type, new LayerComparer(game)).ToList();
            }

            if (!forceOverwrite)
                RecalculateAllMatrices();
        }
    }
}
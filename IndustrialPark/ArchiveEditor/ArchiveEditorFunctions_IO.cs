using HipHopFile;
using System.Linq;
using System.IO;
using System.Windows.Forms;

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
        
        public void ImportHip(string[] fileNames, bool forceOverwrite)
        {
            foreach (string fileName in fileNames)
                ImportHip(fileName, forceOverwrite);
        }

        public void ImportHip(string fileName, bool forceOverwrite)
        {
            if (Path.GetExtension(fileName).ToLower() == ".hip" || Path.GetExtension(fileName).ToLower() == ".hop")
                ImportHip(new HipFile(fileName).DICT, forceOverwrite);
            else if (Path.GetExtension(fileName).ToLower() == ".ini")
                ImportHip(HipFile.FromINI(fileName).DICT, forceOverwrite);
            else
                MessageBox.Show("Invalid file: " + fileName);
        }

        public void ImportHip(Section_DICT dictToImport, bool forceOverwrite)
        {
            UnsavedChanges = true;
            bool redoTextures = false;

            foreach (Section_AHDR AHDR in dictToImport.ATOC.AHDRList)
            {
                if (AHDR.assetType == AssetType.COLL && ContainsAssetWithType(AssetType.COLL))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeCOLL(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.JAW && ContainsAssetWithType(AssetType.JAW))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeJAW(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.LODT && ContainsAssetWithType(AssetType.LODT))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeLODT(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.PIPT && ContainsAssetWithType(AssetType.PIPT))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergePIPT(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.SHDW && ContainsAssetWithType(AssetType.SHDW))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeSHDW(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.SNDI && ContainsAssetWithType(AssetType.SNDI))
                {
                    foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                        LHDR.assetIDlist.Remove(AHDR.assetID);

                    MergeSNDI(AHDR);
                    continue;
                }
                else if (AHDR.assetType == AssetType.RWTX)
                    redoTextures = true;

                if (ContainsAsset(AHDR.assetID))
                {
                    DialogResult result = forceOverwrite ? DialogResult.Yes :
                    MessageBox.Show($"Asset [{AHDR.assetID.ToString("X8")}] {AHDR.ADBG.assetName} already present in archive. Do you wish to overwrite it?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        RemoveAsset(AHDR.assetID, false);
                        DICT.ATOC.AHDRList.Add(AHDR);
                        AddAssetToDictionary(AHDR, forceOverwrite, forceOverwrite);
                    }
                    else
                    {
                        foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                            LHDR.assetIDlist.Remove(AHDR.assetID);
                    }
                }
                else
                {
                    DICT.ATOC.AHDRList.Add(AHDR);
                    AddAssetToDictionary(AHDR, forceOverwrite, forceOverwrite);
                }
            }

            foreach (Section_LHDR LHDR in dictToImport.LTOC.LHDRList)
                if (LHDR.assetIDlist.Count != 0)
                    DICT.LTOC.LHDRList.Add(LHDR);

            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer(game)).ToList();

            if (!forceOverwrite)
            {
                RecalculateAllMatrices();
                if (redoTextures)
                    SetupTextureDisplay();
            }
        }
    }
}
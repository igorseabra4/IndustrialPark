using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HipHopFile;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public static class HipHopFunctions
    {
        public static string currentlyOpenHip;
        public static Section_HIPA hipHIPA;
        public static Section_PACK hipPACK;
        public static Section_DICT hipDICT;
        public static Section_STRM hipSTRM;

        public static string currentlyOpenHop;
        public static Section_HIPA hopHIPA;
        public static Section_PACK hopPACK;
        public static Section_DICT hopDICT;
        public static Section_STRM hopSTRM;

        public static string currentlyOpenBoot;
        public static Section_HIPA bootHIPA;
        public static Section_PACK bootPACK;
        public static Section_DICT bootDICT;
        public static Section_STRM bootSTRM;

        public static string fileNamePrefix;
        public static Dictionary<int, Asset> assetDictionary;
        public static HashSet<RenderableAsset> renderableAssetList = new HashSet<RenderableAsset>();

        public static void openFilePair()
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                Filter = "HIP/HOP Files|*.hip;*.hop",
                Title = "Please choose a HIP or HOP file"
            };
            if (openFile.ShowDialog() != DialogResult.OK) return;

            if (Path.GetFileNameWithoutExtension(openFile.FileName).Contains("boot") |
                Path.GetFileNameWithoutExtension(openFile.FileName).Contains("font") |
                Path.GetFileNameWithoutExtension(openFile.FileName).Contains("plat"))
            {
                MessageBox.Show("Please only open level HIP/HOP files");
                return;
            }

            fileNamePrefix = Path.GetFileNameWithoutExtension(openFile.FileName);

            if (Path.GetExtension(openFile.FileName).ToLower() == ".hip")
            {
                currentlyOpenHip = openFile.FileName;
                currentlyOpenHop = Path.Combine(Path.GetDirectoryName(currentlyOpenHip), fileNamePrefix + ".HOP");
            }
            else if (Path.GetExtension(openFile.FileName).ToLower() == ".hop")
            {
                currentlyOpenHop = openFile.FileName;
                currentlyOpenHip = Path.Combine(Path.GetDirectoryName(currentlyOpenHop), fileNamePrefix + ".HIP");
            }
            currentlyOpenBoot = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(currentlyOpenHop)), "boot.HIP");

            if (File.Exists(currentlyOpenHip))
                foreach (HipSection i in HipFileToHipArray(currentlyOpenHip))
                {
                    if (i is Section_HIPA hipa) hipHIPA = hipa;
                    else if (i is Section_PACK pack) hipPACK = pack;
                    else if (i is Section_DICT dict) hipDICT = dict;
                    else if (i is Section_STRM strm) hipSTRM = strm;
                    else throw new Exception();
                }
            else
            {
                currentlyOpenHip = null;
                hipHIPA = null;
                hipPACK = null;
                hipDICT = null;
                hipSTRM = null;
            }

            if (File.Exists(currentlyOpenHop))
                foreach (HipSection i in HipFileToHipArray(currentlyOpenHop))
                {
                    if (i is Section_HIPA hipa) hopHIPA = hipa;
                    else if (i is Section_PACK pack) hopPACK = pack;
                    else if (i is Section_DICT dict) hopDICT = dict;
                    else if (i is Section_STRM strm) hopSTRM = strm;
                    else throw new Exception();
                }
            else
            {
                currentlyOpenHop = null;
                hopHIPA = null;
                hopPACK = null;
                hopDICT = null;
                hopSTRM = null;
            }

            if (File.Exists(currentlyOpenBoot))
                foreach (HipSection i in HipFileToHipArray(currentlyOpenBoot))
                {
                    if (i is Section_HIPA hipa) bootHIPA = hipa;
                    else if (i is Section_PACK pack) bootPACK = pack;
                    else if (i is Section_DICT dict) bootDICT = dict;
                    else if (i is Section_STRM strm) bootSTRM = strm;
                    else throw new Exception();
                }
            else
            {
                currentlyOpenBoot = null;
                bootHIPA = null;
                bootPACK = null;
                bootDICT = null;
                bootSTRM = null;
            }
            
            foreach (SharpMesh mesh in RenderWareModelFile.completeMeshList)
                mesh.Dispose();
            RenderWareModelFile.completeMeshList.Clear();

            assetDictionary = new Dictionary<int, Asset>();
            renderableAssetList = new HashSet<RenderableAsset>();

            SharpRenderer.LoadTextures(fileNamePrefix, false);

            if (hipDICT != null)
                foreach (Section_AHDR AHDR in hipDICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR);
            if (hopDICT != null)
                foreach (Section_AHDR AHDR in hopDICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR);
            if (bootDICT != null)
                foreach (Section_AHDR AHDR in bootDICT.ATOC.AHDRList)
                    AddAssetToDictionary(AHDR);
        }

        private static void AddAssetToDictionary(Section_AHDR AHDR)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
                assetDictionary.Remove(AHDR.assetID);

            switch (AHDR.fileType)
            {
                case "BSP ":
                    {
                        AssetBSP newAsset = new AssetBSP(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "JSP ":
                    {
                        AssetJSP newAsset = new AssetJSP(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "MINF":
                    {
                        AssetMINF newAsset = new AssetMINF(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile); ;
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "MODL":
                    {
                        AssetMODL newAsset = new AssetMODL(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "PICK":
                    {
                        AssetPICK newAsset = new AssetPICK(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "PKUP":
                    {
                        AssetPKUP newAsset = new AssetPKUP(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "PLAT":
                    {
                        AssetPLAT newAsset = new AssetPLAT(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "RWTX":
                    {
                        AssetRWTX newAsset = new AssetRWTX(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "SIMP":
                    {
                        AssetSIMP newAsset = new AssetSIMP(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case "VIL ":
                    {
                        AssetVIL newAsset = new AssetVIL(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    {
                        AssetGeneric newAsset = new AssetGeneric(AHDR.assetID, AHDR.ADBG.assetName, AHDR.containedFile);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
            }
        }

        public static void ExportAllTextures()
        {
            foreach (Asset asset in assetDictionary.Values)
            {
                if (asset is AssetRWTX texture)
                    ExportTextureAsset(texture);
            }
        }

        private static void ExportTextureAsset(AssetRWTX asset)
        {
            Directory.CreateDirectory(Application.StartupPath + "\\Export\\" + fileNamePrefix);
            File.WriteAllBytes(Application.StartupPath + "\\Export\\" + fileNamePrefix + "\\" + Path.GetFileNameWithoutExtension(asset.Name) + ".txd", asset.Data);
        }
    }
}
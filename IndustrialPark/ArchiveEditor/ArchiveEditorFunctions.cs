using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class ArchiveEditorFunctions
    {
        public static HashSet<IRenderableAsset> renderableAssetSet = new HashSet<IRenderableAsset>();
        public static Dictionary<uint, IAssetWithModel> renderingDictionary = new Dictionary<uint, IAssetWithModel>();

        public static void AddToRenderingDictionary(uint key, IAssetWithModel value)
        {
            if (!renderingDictionary.ContainsKey(key))
                renderingDictionary.Add(key, value);
            else
                renderingDictionary[key] = value;
        }

        public ArchiveEditorFunctions()
        {
            gizmos = new Gizmo[3];
            gizmos[0] = new Gizmo(GizmoType.X);
            gizmos[1] = new Gizmo(GizmoType.Y);
            gizmos[2] = new Gizmo(GizmoType.Z);
        }

        private Dictionary<uint, Asset> assetDictionary = new Dictionary<uint, Asset>();

        public bool DictionaryHasKey(uint key)
        {
            return assetDictionary.ContainsKey(key);
        }

        public Asset GetFromAssetID(uint key)
        {
            if (DictionaryHasKey(key))
                return assetDictionary[key];
            else
                throw new KeyNotFoundException();
        }

        public Dictionary<uint, Asset>.ValueCollection GetAllAssets()
        {
            return assetDictionary.Values;
        }
        
        public static void ExportTextureAsset(AssetRWTX asset, string fileNamePrefix)
        {
            Directory.CreateDirectory(Application.StartupPath + "\\Export\\" + fileNamePrefix);
            File.WriteAllBytes(Application.StartupPath + "\\Export\\" + fileNamePrefix + "\\" + Path.GetFileNameWithoutExtension(asset.AHDR.ADBG.assetName) + ".txd", asset.AHDR.containedFile);
        }

        public string fileNamePrefix;
        public string currentlyOpenFilePath;
        public Section_HIPA HIPA;
        public Section_PACK PACK;
        public Section_DICT DICT;
        public Section_STRM STRM;

        public void OpenFile(string fileName)
        {
            currentlySelectedAssetID = 0;

            Dispose();

            currentlyOpenFilePath = fileName;
                        
            fileNamePrefix = Path.GetFileNameWithoutExtension(fileName);

            HipSection[] HipFile = HipFileToHipArray(fileName);

            foreach (HipSection i in HipFileToHipArray(fileName))
            {
                if (i is Section_HIPA hipa) HIPA = hipa;
                else if (i is Section_PACK pack) PACK = pack;
                else if (i is Section_DICT dict) DICT = dict;
                else if (i is Section_STRM strm) STRM = strm;
                else throw new Exception();
            }

            foreach (Section_AHDR AHDR in DICT.ATOC.AHDRList)
                AddAssetToDictionary(AHDR);
            RecalculateAllMatrices();
        }

        public void Save()
        {
            HipSection[] hipFile = SetupStream(ref HIPA, ref PACK, ref DICT, ref STRM);
            byte[] file = HipArrayToFile(hipFile);
            File.WriteAllBytes(currentlyOpenFilePath, file);
        }

        public void Dispose()
        {
            foreach (uint key in assetDictionary.Keys)
            {
                if (assetDictionary[key] is IRenderableAsset ra)
                    if (renderableAssetSet.Contains(ra))
                    {
                        renderableAssetSet.Remove(ra);
                    }
                if (renderingDictionary.ContainsKey(key))
                {
                    renderingDictionary.Remove(key);
                }
            }
            assetDictionary.Clear();

            if (DICT == null) return;
            HIPA = null;
            PACK = null;
            DICT = null;
            STRM = null;
            fileNamePrefix = null;
            currentlyOpenFilePath = null;
        }

        private void AddAssetToDictionary(Section_AHDR AHDR)
        {
            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            switch (AHDR.assetType)
            {
                case AssetType.BSP:
                case AssetType.JSP:
                    {
                        AssetJSP newAsset = new AssetJSP(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BUTN:
                    {
                        AssetBUTN newAsset = new AssetBUTN(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.GRUP:
                    {
                        AssetGRUP newAsset = new AssetGRUP(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MINF:
                    {
                        AssetMINF newAsset = new AssetMINF(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MODL:
                    {
                        AssetMODL newAsset = new AssetMODL(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MRKR:
                    {
                        AssetMRKR newAsset = new AssetMRKR(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MVPT:
                    {
                        AssetMVPT newAsset = new AssetMVPT(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PICK:
                    {
                        AssetPICK newAsset = new AssetPICK(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PKUP:
                    {
                        AssetPKUP newAsset = new AssetPKUP(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PLAT:
                    {
                        AssetPLAT newAsset = new AssetPLAT(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PLYR:
                    {
                        AssetPLYR newAsset = new AssetPLYR(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.RWTX:
                    {
                        AssetRWTX newAsset = new AssetRWTX(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SIMP:
                    {
                        AssetSIMP newAsset = new AssetSIMP(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TRIG:
                    {
                        AssetTRIG newAsset = new AssetTRIG(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.VIL:
                    {
                        AssetVIL newAsset = new AssetVIL(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    {
                        AssetGeneric newAsset = new AssetGeneric(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
            }
        }

        public void RemoveLayer(int index)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList[index].assetIDlist.Count(); i++)
                RemoveAsset(index, DICT.LTOC.LHDRList[index].assetIDlist[i]);

            DICT.LTOC.LHDRList.RemoveAt(index);
        }

        public void AddAsset(int layerIndex, Section_AHDR AHDR)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            DICT.ATOC.AHDRList.Add(AHDR);
            AddAssetToDictionary(AHDR);
        }

        public void RemoveAsset(int layerIndex, uint assetID)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Remove(assetID);

            if (renderingDictionary.ContainsKey(assetID))
                renderingDictionary.Remove(assetID);
            if (assetDictionary[assetID] is IRenderableAsset ra)
                if (renderableAssetSet.Contains(ra))
                    renderableAssetSet.Remove(ra);

            assetDictionary.Remove(assetID);

            for (int i = 0; i < DICT.ATOC.AHDRList.Count; i++)
            {
                if (DICT.ATOC.AHDRList[i].assetID == assetID)
                {
                    DICT.ATOC.AHDRList.RemoveAt(i);
                    break;
                }
            }

            currentlySelectedAssetID = 0;
        }

        private uint currentlySelectedAssetID = 0;

        public uint getCurrentlySelectedAssetID()
        {
            return currentlySelectedAssetID;
        }

        public void SelectAsset(uint assetID)
        {
            if (assetDictionary.ContainsKey(currentlySelectedAssetID))
                assetDictionary[currentlySelectedAssetID].isSelected = false;
            currentlySelectedAssetID = assetID;
            if (currentlySelectedAssetID != 0)
            {
                assetDictionary[currentlySelectedAssetID].isSelected = true;
                if (assetDictionary[currentlySelectedAssetID] is IClickableAsset ra)
                    UpdateGizmoPosition();
                else
                    ClearGizmos();
            }
            else ClearGizmos();
        }

        public int GetSelectedLayerIndex()
        {
            if (currentlySelectedAssetID == 0)
                throw new Exception();

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
            {
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(currentlySelectedAssetID))
                    return i;
            }
            throw new Exception();
        }

        public uint ScreenClicked(Ray ray)
        {
            uint assetID = 0;

            float smallerDistance = 1000f;
            foreach (Asset ra in renderableAssetSet)
            {
                if (!ra.isSelected & ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null)
                        if (distance < smallerDistance)
                        {
                            smallerDistance = (float)distance;
                            assetID = ra.AHDR.assetID;
                        }
                }

            }

            if (assetID != 0 & assetDictionary.ContainsKey(assetID))
                SelectAsset(assetID);
            return assetID;
        }
        
        public void RecalculateAllMatrices()
        {
            foreach (IRenderableAsset a in renderableAssetSet)
                a.CreateTransformMatrix();
        }
        
        // Gizmos
        private static Gizmo[] gizmos;
        private static bool DrawGizmos = false;
        public bool FinishedMovingGizmo = false;

        public static void RenderGizmos(SharpRenderer renderer)
        {
            if (DrawGizmos)
                foreach (Gizmo g in gizmos)
                    g.Draw(renderer);
        }

        public void UpdateGizmoPosition()
        {
            IClickableAsset currentAsset = ((IClickableAsset)assetDictionary[currentlySelectedAssetID]);
            UpdateGizmoPosition(currentAsset.GetGizmoCenter(), currentAsset.GetGizmoRadius());
        }
        
        private void UpdateGizmoPosition(Vector3 position, float distance)
        {
            DrawGizmos = true;
            foreach (Gizmo g in gizmos)
                g.SetPosition(position, distance);
        }

        private void ClearGizmos()
        {
            DrawGizmos = false;
        }

        public void GizmoSelect(Ray r)
        {
            if (!DrawGizmos)
                return;

            float dist = 1000f;
            int index = -1;

            for (int g = 0; g < gizmos.Length; g++)
            {
                float? distance = gizmos[g].IntersectsWith(r);
                if (distance != null)
                {
                    if (distance < dist)
                    {
                        dist = (float)distance;
                        index = g;
                    }
                }
            }

            if (index == -1)
                return;

            gizmos[index].isSelected = true;
        }

        public void ScreenUnclicked()
        {
            foreach (Gizmo g in gizmos)
                g.isSelected = false;
        }

        public void MouseMoveX(SharpCamera camera, int distance)
        {
            if (currentlySelectedAssetID == 0) return;

            Asset currentAsset = assetDictionary[currentlySelectedAssetID];
            if (currentAsset is IClickableAsset ra)
            {
                if (gizmos[0].isSelected)
                {
                    ra.PositionX += (
                        (camera.Yaw >= -360 & camera.Yaw < -270) |
                        (camera.Yaw >= -90 & camera.Yaw < 90) |
                        (camera.Yaw >= 270)) ? distance / 10f : -distance / 10f;
                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                }
                else if (gizmos[2].isSelected)
                {
                    ra.PositionZ += (
                        (camera.Yaw >= -180 & camera.Yaw < 0) |
                        (camera.Yaw >= 180)) ? distance / 10f : -distance / 10f;
                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                }
            }
        }

        public void MouseMoveY(SharpCamera camera, int distance)
        {
            if (currentlySelectedAssetID == 0) return;

            Asset currentAsset = assetDictionary[currentlySelectedAssetID];
            if (currentAsset is IClickableAsset ra)
                if (gizmos[1].isSelected)
                {
                    ra.PositionY -= distance / 10f;
                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                }
        }
    }
}
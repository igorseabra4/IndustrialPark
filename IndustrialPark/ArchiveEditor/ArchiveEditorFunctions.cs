using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using static HipHopFile.Functions;

namespace IndustrialPark
{
    public class ArchiveEditorFunctions
    {
        public static HashSet<IRenderableAsset> renderableAssetSetCommon = new HashSet<IRenderableAsset>();
        public static HashSet<IRenderableAsset> renderableAssetSetTrans = new HashSet<IRenderableAsset>();
        public static HashSet<AssetJSP> renderableAssetSetJSP = new HashSet<AssetJSP>();
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

        public bool ContainsAsset(uint key)
        {
            return assetDictionary.ContainsKey(key);
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
        
        public string fileNamePrefix;
        public string currentlyOpenFilePath;
        public Section_HIPA HIPA;
        public Section_PACK PACK;
        public Section_DICT DICT;
        public Section_STRM STRM;
        
        public void OpenFile(string fileName)
        {
            Dispose();

            currentlySelectedAssetID = 0;
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
                DisposeAsset(key);

            assetDictionary.Clear();

            if (DICT == null) return;
            HIPA = null;
            PACK = null;
            DICT = null;
            STRM = null;
            fileNamePrefix = null;
            currentlyOpenFilePath = null;
        }

        public static bool allowRender = true;

        private void AddAssetToDictionary(Section_AHDR AHDR)
        {
            allowRender = false;

            if (assetDictionary.ContainsKey(AHDR.assetID))
            {
                assetDictionary.Remove(AHDR.assetID);
                MessageBox.Show("Duplicate asset ID found: " + AHDR.assetID.ToString("X8"));
            }

            switch (AHDR.assetType)
            {
                case AssetType.ANIM:
                    {
                        AssetANIM newAsset = new AssetANIM(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ALST:
                    {
                        AssetALST newAsset = new AssetALST(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BSP:
                case AssetType.JSP:
                    {
                        AssetJSP newAsset = new AssetJSP(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BOUL:
                    {
                        AssetBOUL newAsset = new AssetBOUL(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.BUTN:
                    {
                        AssetBUTN newAsset = new AssetBUTN(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CAM:
                    {
                        AssetCAM newAsset = new AssetCAM(AHDR);
                        newAsset.Setup(Program.MainForm.renderer);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CNTR:
                    {
                        AssetCNTR newAsset = new AssetCNTR(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.COND:
                    {
                        AssetCOND newAsset = new AssetCOND(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DPAT:
                    {
                        AssetDPAT newAsset = new AssetDPAT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DSTR:
                    {
                        AssetDSTR newAsset = new AssetDSTR(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.DYNA:
                    {
                        AssetDYNA newAsset = new AssetDYNA(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.ENV:
                    {
                        AssetENV newAsset = new AssetENV(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.FOG:
                    {
                        AssetFOG newAsset = new AssetFOG(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.GRUP:
                    {
                        AssetGRUP newAsset = new AssetGRUP(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MAPR:
                    {
                        AssetMAPR newAsset = new AssetMAPR(AHDR);
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
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.MVPT:
                    {
                        if (currentGame == Game.BFBB)
                        {
                            AssetMVPT newAsset = new AssetMVPT(AHDR);
                            newAsset.Setup();
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                        else
                        {
                            Asset newAsset = new Asset(AHDR);
                            assetDictionary.Add(AHDR.assetID, newAsset);
                        }
                    }
                    break;
                case AssetType.PICK:
                    {
                        AssetPICK newAsset = new AssetPICK(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PIPT:
                    {
                        AssetPIPT newAsset = new AssetPIPT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PKUP:
                    {
                        AssetPKUP newAsset = new AssetPKUP(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PLAT:
                    {
                        AssetPLAT newAsset = new AssetPLAT(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PLYR:
                    {
                        AssetPLYR newAsset = new AssetPLYR(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.PORT:
                    {
                        AssetPORT newAsset = new AssetPORT(AHDR);
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
                case AssetType.SFX:
                    {
                        AssetSFX newAsset = new AssetSFX(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SIMP:
                    {
                        AssetSIMP newAsset = new AssetSIMP(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.SNDI:
                    {
                        AssetSNDI newAsset = new AssetSNDI(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TEXT:
                    {
                        AssetTEXT newAsset = new AssetTEXT(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TRIG:
                    {
                        AssetTRIG newAsset = new AssetTRIG(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.TIMR:
                    {
                        AssetTIMR newAsset = new AssetTIMR(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.VIL:
                    {
                        AssetVIL newAsset = new AssetVIL(AHDR);
                        newAsset.Setup();
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                case AssetType.CCRV:
                case AssetType.CSNM:
                case AssetType.DSCO:
                case AssetType.DTRK:
                case AssetType.DUPC:
                case AssetType.EGEN:
                case AssetType.GRSM:
                case AssetType.GUST:
                case AssetType.HANG:
                case AssetType.LITE:
                case AssetType.LOBM:
                case AssetType.NGMS:
                case AssetType.NPC:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PEND:
                case AssetType.PGRS:
                case AssetType.PRJT:
                case AssetType.RANM:
                case AssetType.SCRP:
                case AssetType.SDFX:
                case AssetType.SGRP:
                case AssetType.SLID:
                case AssetType.SPLN:
                case AssetType.SSET:
                case AssetType.SUBT:
                case AssetType.SURF:
                case AssetType.TPIK:
                case AssetType.TRWT:
                case AssetType.UIFT:
                case AssetType.UI:
                case AssetType.UIM:
                case AssetType.VOLU:
                case AssetType.ZLIN:
                    {
                        ObjectAsset newAsset = new ObjectAsset(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
                default:
                    {
                        Asset newAsset = new Asset(AHDR);
                        assetDictionary.Add(AHDR.assetID, newAsset);
                    }
                    break;
            }

            allowRender = true;
        }

        public void RemoveLayer(int index)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList[index].assetIDlist.Count(); i++)
                RemoveAsset(DICT.LTOC.LHDRList[index].assetIDlist[i]);

            DICT.LTOC.LHDRList.RemoveAt(index);
        }

        public void AddAsset(int layerIndex, Section_AHDR AHDR)
        {
            DICT.LTOC.LHDRList[layerIndex].assetIDlist.Add(AHDR.assetID);
            DICT.ATOC.AHDRList.Add(AHDR);
            AddAssetToDictionary(AHDR);
        }

        public void AddAssetWithUniqueID(int layerIndex, Section_AHDR AHDR)
        {
            int numCopies = -1;

            while (ContainsAsset(AHDR.assetID))
            {
                numCopies++;

                if (AHDR.ADBG.assetName.Contains("_COPY"))
                    AHDR.ADBG.assetName = AHDR.ADBG.assetName.Substring(0, AHDR.ADBG.assetName.LastIndexOf("_COPY"));

                AHDR.ADBG.assetName += "_COPY" + numCopies.ToString();
                AHDR.assetID = BKDRHash(AHDR.ADBG.assetName);
            }

            AddAsset(layerIndex, AHDR);
        }

        public void RemoveAsset(uint assetID)
        {
            CloseInternalEditor(currentlySelectedAssetID);

            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(assetID))
                    DICT.LTOC.LHDRList[i].assetIDlist.Remove(assetID);

            DisposeAsset(assetID);

            DICT.ATOC.AHDRList.Remove(assetDictionary[assetID].AHDR);

            if (GetFromAssetID(assetID).AHDR.assetType == AssetType.SND | GetFromAssetID(assetID).AHDR.assetType == AssetType.SNDS)
                RemoveSoundFromSNDI(assetID);

            assetDictionary.Remove(assetID);

            currentlySelectedAssetID = 0;
        }

        private void DisposeAsset(uint key)
        {
            if (assetDictionary[key] is IRenderableAsset ra)
            {
                if (renderableAssetSetCommon.Contains(ra))
                    renderableAssetSetCommon.Remove(ra);
                else if (renderableAssetSetTrans.Contains(ra))
                    renderableAssetSetTrans.Remove(ra);
                else if (renderableAssetSetJSP.Contains(ra))
                    renderableAssetSetJSP.Remove((AssetJSP)ra);
            }

            if (renderingDictionary.ContainsKey(key))
                renderingDictionary.Remove(key);

            if (assetDictionary[key] is AssetJSP jsp)
                jsp.model.Dispose();

            if (assetDictionary[key] is AssetMODL modl)
                modl.GetRenderWareModelFile().Dispose();

            if (assetDictionary[key] is AssetRWTX texture)
                TextureManager.RemoveTexture(texture.Name);
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
        }

        public int GetLayerFromAssetID(uint assetID)
        {
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].assetIDlist.Contains(assetID))
                    return i;

            throw new Exception($"Asset ID {assetID.ToString("X8")} is not present in any layer.");
        }

        private List<IInternalEditor> internalEditors = new List<IInternalEditor>();

        public void CloseInternalEditor(IInternalEditor i)
        {
            internalEditors.Remove(i);
        }

        public void CloseInternalEditor(uint assetID)
        {
            for (int i = 0; i < internalEditors.Count; i++)
                if (internalEditors[i].GetAssetID() == assetID)
                    internalEditors[i].Close();
        }

        public void OpenInternalEditor(Asset asset = null)
        {
            if (asset == null)
                asset = GetFromAssetID(currentlySelectedAssetID);

            for (int i = 0; i < internalEditors.Count; i++)
                if (internalEditors[i].GetAssetID() == asset.AHDR.assetID)
                    internalEditors[i].Close();

            if (asset is AssetCAM CAM)
                internalEditors.Add(new InternalCamEditor(CAM, this));
            else if (asset is AssetDYNA DYNA)
                internalEditors.Add(new InternalDynaEditor(DYNA, this));
            else if (asset is AssetTEXT TEXT)
                internalEditors.Add(new InternalTextEditor(TEXT, this));
            else if (asset.AHDR.assetType == AssetType.SND | asset.AHDR.assetType == AssetType.SNDS)
                internalEditors.Add(new InternalSoundEditor(asset, this));
            else if (asset is ObjectAsset objectAsset)
                internalEditors.Add(new InternalObjectAssetEditor(objectAsset, this));
            else
                internalEditors.Add(new InternalAssetEditor(asset, this));

            internalEditors.Last().Show();
        }

        public static uint GetClickedAssetID(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
            }
            catch { return 0; }

            float smallerDistance = 1000f;
            uint assetID = 0;

            foreach (Asset ra in l)
            {
                if (!ra.isSelected && ra is IClickableAsset)
                {
                    float? distance = ((IClickableAsset)ra).IntersectsWith(ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.AHDR.assetID;
                    }
                }
            }

            return assetID;
        }

        public void FindWhoTargets(uint assetID)
        {
            foreach (Asset asset in assetDictionary.Values)
                if (asset is ObjectAsset objectAsset)
                    foreach (AssetEvent assetEvent in objectAsset.EventsBFBB)
                        if (assetEvent.TargetAssetID == assetID)
                        {
                            OpenInternalEditor(asset);
                            break;
                        }
        }

        public void RecalculateAllMatrices()
        {
            foreach (IRenderableAsset a in renderableAssetSetCommon)
                a.CreateTransformMatrix();
            foreach (IRenderableAsset a in renderableAssetSetTrans)
                a.CreateTransformMatrix();
            foreach (AssetJSP a in renderableAssetSetJSP)
                a.CreateTransformMatrix();
        }
        
        // Gizmos
        private static Gizmo[] gizmos = new Gizmo[0];
        private static bool DrawGizmos = false;
        public static bool FinishedMovingGizmo = false;

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
        
        private static void UpdateGizmoPosition(Vector3 position, float distance)
        {
            DrawGizmos = true;
            foreach (Gizmo g in gizmos)
                g.SetPosition(position, distance);
        }

        private static void ClearGizmos()
        {
            DrawGizmos = false;
        }

        public static void GizmoSelect(Ray r)
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

        public static void ScreenUnclicked()
        {
            foreach (Gizmo g in gizmos)
                g.isSelected = false;
        }

        public void MouseMoveX(SharpCamera camera, int distance)
        {
            if (currentlySelectedAssetID == 0) return;

            if (assetDictionary[currentlySelectedAssetID] is IClickableAsset ra)
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

            if (assetDictionary[currentlySelectedAssetID] is IClickableAsset ra)
                if (gizmos[1].isSelected)
                {
                    ra.PositionY -= distance / 10f;
                    UpdateGizmoPosition();
                    FinishedMovingGizmo = true;
                }
        }

        public void ExportTextureDictionary(string fileName)
        {
            ReadFileMethods.treatTexturesAsByteArray = true;

            List<TextureNative_0015> textNativeList = new List<TextureNative_0015>();

            int fileVersion = 0;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetRWTX RWTX && RWTX.AHDR.ADBG.assetName.Contains(".RW3"))
                {
                    foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(RWTX.Data))
                        if (rw is TextureDictionary_0016 td)
                            foreach (TextureNative_0015 tn in td.textureNativeList)
                            {
                                fileVersion = tn.renderWareVersion;
                                tn.textureNativeStruct.textureName = RWTX.AHDR.ADBG.assetName;
                                textNativeList.Add(tn);
                            }
                }

            TextureDictionary_0016 rws = new TextureDictionary_0016()
            {
                textureDictionaryStruct = new TextureDictionaryStruct_0001()
                {
                    textureCount = (short)textNativeList.Count(),
                    unknown = 0
                },
                textureNativeList = textNativeList,
                textureDictionaryExtension = new Extension_0003()
                {
                    extensionSectionList = new List<RWSection>()
                }
            };

            rws.textureNativeList = rws.textureNativeList.OrderBy(f => f.textureNativeStruct.textureName).ToList();

            File.WriteAllBytes(fileName, ReadFileMethods.ExportRenderWareFile(rws, fileVersion));

            ReadFileMethods.treatTexturesAsByteArray = false;
        }

        public void AddTextureDictionary(string fileName)
        {
            int layerIndex = 0;

            List<Section_LHDR> LHDRs = new List<Section_LHDR>
            {
                new Section_LHDR()
                {
                    layerType = LayerType.TEXTURE,
                    assetIDlist = new List<uint>(),
                    LDBG = new Section_LDBG(-1)
                }
            };
            LHDRs.AddRange(DICT.LTOC.LHDRList);
            DICT.LTOC.LHDRList = LHDRs;

            ReadFileMethods.treatTexturesAsByteArray = true;

            foreach (RWSection rw in ReadFileMethods.ReadRenderWareFile(fileName))
            {
                if (rw is TextureDictionary_0016 td)
                {
                    // For each texture in the dictionary...
                    foreach (TextureNative_0015 tn in td.textureNativeList)
                    {
                        string textureName = tn.textureNativeStruct.textureName;
                        if (!textureName.Contains(".RW3"))
                            textureName += ".RW3";

                        // Create a new dictionary that has only that texture.
                        byte[] data = ReadFileMethods.ExportRenderWareFile(new TextureDictionary_0016()
                        {
                            textureDictionaryStruct = new TextureDictionaryStruct_0001() { textureCount = 1, unknown = 0 },
                            textureDictionaryExtension = new Extension_0003(),
                            textureNativeList = new List<TextureNative_0015>() { tn }
                        }, tn.renderWareVersion);
                        
                        // And add the new dictionary as an asset.
                        Section_ADBG ADBG = new Section_ADBG(0, textureName, "", 0);
                        Section_AHDR AHDR = new Section_AHDR(BKDRHash(textureName), AssetType.RWTX, AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM, ADBG, data);

                        if (ContainsAsset(AHDR.assetID))
                            RemoveAsset(AHDR.assetID);

                        AddAsset(layerIndex, AHDR);
                    }
                }
            }

            ReadFileMethods.treatTexturesAsByteArray = false;
        }

        public void AddSoundToSNDI(byte[] headerData, uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a.AHDR.assetType == AssetType.SNDI)
                {
                    ((AssetSNDI)a).AddEntry(headerData, assetID, GetFromAssetID(assetID).AHDR.assetType);
                    break;
                }
        }

        public void RemoveSoundFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a.AHDR.assetType == AssetType.SNDI)
                {
                    ((AssetSNDI)a).RemoveEntry(assetID, GetFromAssetID(assetID).AHDR.assetType);
                    break;
                }
        }

        public byte[] GetHeaderFromSNDI(uint assetID)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a.AHDR.assetType == AssetType.SNDI)
                    return ((AssetSNDI)a).GetHeader(assetID, GetFromAssetID(assetID).AHDR.assetType);

            throw new Exception("Error: could not find SNDI asset in this archive.");
        }
    }
}
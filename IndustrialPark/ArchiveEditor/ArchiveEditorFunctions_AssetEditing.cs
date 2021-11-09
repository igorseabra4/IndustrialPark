using HipHopFile;
using IndustrialPark.Models;
using RenderWareFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        private class LHDRComparer : IComparer<int>
        {
            private Game game;

            public LHDRComparer(Game game)
            {
                this.game = game;
            }

            private static readonly List<int> layerOrderBFBB = new List<int> {
                (int)LayerType_BFBB.TEXTURE,
                (int)LayerType_BFBB.BSP,
                (int)LayerType_BFBB.JSPINFO,
                (int)LayerType_BFBB.MODEL,
                (int)LayerType_BFBB.ANIMATION,
                (int)LayerType_BFBB.DEFAULT,
                (int)LayerType_BFBB.CUTSCENE,
                (int)LayerType_BFBB.SRAM,
                (int)LayerType_BFBB.SNDTOC
            };
            private static readonly List<int> layerOrderTSSM = new List<int> {
                (int)LayerType_TSSM.TEXTURE,
                (int)LayerType_TSSM.TEXTURE_STRM,
                (int)LayerType_TSSM.BSP,
                (int)LayerType_TSSM.JSPINFO,
                (int)LayerType_TSSM.MODEL,
                (int)LayerType_TSSM.ANIMATION,
                (int)LayerType_TSSM.DEFAULT,
                (int)LayerType_TSSM.CUTSCENE,
                (int)LayerType_TSSM.SRAM,
                (int)LayerType_TSSM.SNDTOC,
                (int)LayerType_TSSM.CUTSCENETOC
            };

            public int Compare(int l1, int l2)
            {
                if (l1 == l2)
                    return 0;

                if (game == Game.Scooby && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                    return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

                if (game == Game.BFBB && layerOrderBFBB.Contains(l1) && layerOrderBFBB.Contains(l2))
                    return layerOrderBFBB.IndexOf(l1) > layerOrderBFBB.IndexOf(l2) ? 1 : -1;

                if (game == Game.Incredibles)
                {
                    if ((l1 == 3 && l2 == 11) || (l1 == 11 && l2 == 3))
                        return 0;

                    if (layerOrderTSSM.Contains(l1) && layerOrderTSSM.Contains(l2))
                        return layerOrderTSSM.IndexOf(l1) > layerOrderTSSM.IndexOf(l2) ? 1 : -1;
                }

                return 0;
            }
        }

        public static List<uint> hiddenAssets = new List<uint>();

        public List<uint> GetHiddenAssets()
        {
            return (from asset in assetDictionary.Values where asset.isInvisible select asset.assetID).ToList();
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

        public void OpenInternalEditor(List<uint> list, bool openAnyway)
        {
            bool willOpen = true;
            if (list.Count > 15 && !openAnyway)
            {
                willOpen = MessageBox.Show($"Warning: you're going to open {list.Count} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }

            if (willOpen)
                foreach (uint u in list)
                    if (assetDictionary.ContainsKey(u))
                        OpenInternalEditor(assetDictionary[u]);
        }

        private void OpenInternalEditor(Asset asset)
        {
            CloseInternalEditor(asset.assetID);

            switch (asset.assetType)
            {
                case AssetType.FLY:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this));
                    break;
                case AssetType.RWTX:
                    if (asset is AssetRWTX rwtx)
                        internalEditors.Add(new InternalTextureEditor(rwtx, this));
                    else
                        internalEditors.Add(new InternalAssetEditor(asset, this));
                    break;
                case AssetType.SND:
                case AssetType.SNDS:
                    internalEditors.Add(new InternalSoundEditor((AssetSound)asset, this));
                    break;
                case AssetType.TEXT:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this));
                    break;
            }

            internalEditors.Last().Show();
        }

        private List<InternalMultiAssetEditor> multiInternalEditors = new List<InternalMultiAssetEditor>();

        public void OpenInternalEditorMulti(List<uint> list)
        {
            var assets = new List<Asset>();
            foreach (var u in list)
                if (assetDictionary.ContainsKey(u))
                    assets.Add(assetDictionary[u]);

            multiInternalEditors.Add(new InternalMultiAssetEditor(assets.ToArray(), this));
            multiInternalEditors.Last().Show();
        }

        public void CloseInternalEditorMulti(uint assetID)
        {
            for (int i = 0; i < multiInternalEditors.Count; i++)
                if (multiInternalEditors[i].assetIDs.Contains(assetID))
                {
                    multiInternalEditors[i].Close();
                    multiInternalEditors.RemoveAt(i--);
                }
        }

        public void SetAllTopMost(bool value)
        {
            foreach (var ie in internalEditors)
                ie.TopMost = value;
        }

        public static Vector3 GetRayInterserctionPosition(SharpRenderer renderer, Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssets);
                l.AddRange(renderableJSPs);
            }
            catch { return Vector3.Zero; }

            float? smallerDistance = null;

            foreach (IRenderableAsset ra in l)
            {
                float? distance = ra.GetIntersectionPosition(renderer, ray);
                if (distance != null && (smallerDistance == null || distance < smallerDistance))
                    smallerDistance = distance;
            }

            return ray.Position + Vector3.Normalize(ray.Direction) * (smallerDistance ?? 2f);
        }

        public static uint GetClickedAssetID(SharpRenderer renderer, Ray ray)
        {
            float smallerDistance = 1000f;
            uint assetID = 0;

            foreach (Asset ra in renderableAssets)
            {
                if (!ra.isSelected && ra is IClickableAsset ica)
                {
                    float? distance = ica.GetIntersectionPosition(renderer, ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.assetID;
                    }
                }
            }

            return assetID;
        }

        public static uint GetClickedAssetID2D(SharpRenderer renderer, Ray ray, float farPlane)
        {
            float smallerDistance = 3 * farPlane;
            uint assetID = 0;

            foreach (Asset ra in from IRenderableAsset asset in renderableAssets
                                 where asset is AssetUI || asset is AssetUIFT
                                 select asset)
            {
                if (!ra.isSelected)
                {
                    float? distance = ((IClickableAsset)ra).GetIntersectionPosition(renderer, ray);
                    if (distance != null && distance < smallerDistance)
                    {
                        smallerDistance = (float)distance;
                        assetID = ra.assetID;
                    }
                }
            }

            return assetID;
        }

        public List<uint> FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = new List<uint>();
            foreach (Asset asset in assetDictionary.Values)
                if (asset.HasReference(assetID))
                    whoTargets.Add(asset.assetID);

            return whoTargets;
        }

        public void MergeSimilar()
        {
            UnsavedChanges = true;

            var COLLs = (from asset in assetDictionary.Values where asset.assetType == AssetType.COLL select (AssetCOLL)asset).ToList();
            for (int i = 1; i < COLLs.Count; i++)
                RemoveAsset(COLLs[i].assetID);
            for (int i = 1; i < COLLs.Count; i++)
                MergeCOLL(COLLs[i]);

            var JAWs = (from asset in assetDictionary.Values where asset.assetType == AssetType.JAW select (AssetJAW)asset).ToList();
            for (int i = 1; i < JAWs.Count; i++)
                RemoveAsset(JAWs[i].assetID);
            for (int i = 1; i < JAWs.Count; i++)
                MergeJAW(JAWs[i]);

            var LODTs = (from asset in assetDictionary.Values where asset.assetType == AssetType.LODT select (AssetLODT)asset).ToList();
            for (int i = 1; i < LODTs.Count; i++)
                RemoveAsset(LODTs[i].assetID);
            for (int i = 1; i < LODTs.Count; i++)
                MergeLODT(LODTs[i]);

            var PIPTs = (from asset in assetDictionary.Values where asset.assetType == AssetType.PIPT select (AssetPIPT)asset).ToList();
            for (int i = 1; i < PIPTs.Count; i++)
                RemoveAsset(PIPTs[i].assetID);
            for (int i = 1; i < PIPTs.Count; i++)
                MergePIPT(PIPTs[i]);

            var SHDWs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SHDW select (AssetSHDW)asset).ToList();
            for (int i = 1; i < SHDWs.Count; i++)
                RemoveAsset(SHDWs[i].assetID);
            for (int i = 1; i < SHDWs.Count; i++)
                MergeSHDW(SHDWs[i]);


            if (platform == Platform.GameCube)
            {
                if (game == Game.Incredibles)
                {
                    var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SNDI select (AssetSNDI_GCN_V1)asset).ToList();
                    for (int i = 1; i < SNDIs.Count; i++)
                        RemoveAsset(SNDIs[i].assetID);
                    for (int i = 1; i < SNDIs.Count; i++)
                        MergeSNDI(SNDIs[i]);
                }
                else
                {
                    var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SNDI select (AssetSNDI_GCN_V2)asset).ToList();
                    for (int i = 1; i < SNDIs.Count; i++)
                        RemoveAsset(SNDIs[i].assetID);
                    for (int i = 1; i < SNDIs.Count; i++)
                        MergeSNDI(SNDIs[i]);
                }
            }
            else if (platform == Platform.Xbox)
            {
                var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SNDI select (AssetSNDI_XBOX)asset).ToList();
                for (int i = 1; i < SNDIs.Count; i++)
                    RemoveAsset(SNDIs[i].assetID);
                for (int i = 1; i < SNDIs.Count; i++)
                    MergeSNDI(SNDIs[i]);
            }
            else if (platform == Platform.PS2)
            {
                var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SNDI select (AssetSNDI_PS2)asset).ToList();
                for (int i = 1; i < SNDIs.Count; i++)
                    RemoveAsset(SNDIs[i].assetID);
                for (int i = 1; i < SNDIs.Count; i++)
                    MergeSNDI(SNDIs[i]);
            }
        }

        private void MergeCOLL(AssetCOLL asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetCOLL COLL)
                {
                    COLL.Merge(asset);
                    return;
                }
        }

        private void MergeJAW(AssetJAW asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetJAW JAW)
                {
                    JAW.Merge(asset);
                    return;
                }
        }

        private void MergeLODT(AssetLODT asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetLODT LODT)
                {
                    LODT.Merge(asset);
                    return;
                }
        }

        private void MergePIPT(AssetPIPT asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPIPT PIPT)
                {
                    PIPT.Merge(asset);
                    return;
                }
        }

        private void MergeSHDW(AssetSHDW asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSHDW SHDW)
                {
                    SHDW.Merge(asset);
                    return;
                }
        }

        private void MergeSNDI(AssetSNDI_GCN_V1 asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V1 SNDI)
                {
                    SNDI.Merge(asset);
                    return;
                }
        }

        private void MergeSNDI(AssetSNDI_GCN_V2 asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V2 SNDI)
                {
                    SNDI.Merge(asset);
                    return;
                }
        }

        private void MergeSNDI(AssetSNDI_XBOX asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_XBOX SNDI)
                {
                    SNDI.Merge(asset);
                    return;
                }
        }

        private void MergeSNDI(AssetSNDI_PS2 asset)
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_PS2 SNDI)
                {
                    SNDI.Merge(asset);
                    return;
                }
        }

        private void CleanSNDI()
        {
            foreach (Asset a in assetDictionary.Values)
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                    SNDI_G1.Clean(from Asset a1 in assetDictionary.Values select a1.assetID);
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                    SNDI_G2.Clean(from Asset a2 in assetDictionary.Values select a2.assetID);
                else if (a is AssetSNDI_XBOX SNDI_X)
                    SNDI_X.Clean(from Asset a3 in assetDictionary.Values select a3.assetID);
                else if (a is AssetSNDI_PS2 SNDI_P)
                    SNDI_P.Clean(from Asset a4 in assetDictionary.Values select a4.assetID);
        }

        public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.ALST:
                case AssetType.BOUL:
                case AssetType.BUTN:
                case AssetType.CAM:
                case AssetType.CNTR:
                case AssetType.COLL:
                case AssetType.COND:
                case AssetType.CSNM:
                case AssetType.CTOC:
                case AssetType.DPAT:
                case AssetType.DSCO:
                case AssetType.DSTR:
                case AssetType.DYNA:
                case AssetType.EGEN:
                case AssetType.ENV:
                case AssetType.FOG:
                case AssetType.HANG:
                case AssetType.GRUP:
                case AssetType.JAW:
                case AssetType.LODT:
                case AssetType.MAPR:
                case AssetType.MINF:
                case AssetType.MRKR:
                case AssetType.MVPT:
                case AssetType.PARE:
                case AssetType.PARP:
                case AssetType.PARS:
                case AssetType.PEND:
                case AssetType.PICK:
                case AssetType.PIPT:
                case AssetType.PKUP:
                case AssetType.PLAT:
                case AssetType.PLYR:
                case AssetType.PORT:
                case AssetType.SFX:
                case AssetType.SHDW:
                case AssetType.SHRP:
                case AssetType.SIMP:
                case AssetType.SNDI:
                case AssetType.SURF:
                case AssetType.TEXT:
                case AssetType.TIMR:
                case AssetType.TRIG:
                case AssetType.UI:
                case AssetType.UIFT:
                case AssetType.VIL:
                case AssetType.VILP:
                    return AHDRFlags.SOURCE_VIRTUAL;
                case AssetType.CSN:
                case AssetType.FLY:
                case AssetType.RAW:
                    return AHDRFlags.SOURCE_FILE;
                case AssetType.ANIM:
                case AssetType.CRDT:
                case AssetType.SND:
                case AssetType.SNDS:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM;
                case AssetType.BSP:
                case AssetType.MODL:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM;
                case AssetType.ATBL:
                case AssetType.JSP:
                case AssetType.RWTX:
                    return AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM;
                case AssetType.LKIT:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM;
                default:
                    return 0;
            }
        }

        public void CollapseLayers()
        {
            Dictionary<int, Section_LHDR> layers = new Dictionary<int, Section_LHDR>();
            List<Section_LHDR> bspLayers = new List<Section_LHDR>();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            {
                if (game == Game.Incredibles && (LHDR.layerType == (int)LayerType_TSSM.BSP || LHDR.layerType == (int)LayerType_TSSM.JSPINFO))
                {
                    if (LHDR.assetIDlist.Count != 0)
                        bspLayers.Add(LHDR);
                }
                else if (layers.ContainsKey(LHDR.layerType))
                {
                    layers[LHDR.layerType].assetIDlist.AddRange(LHDR.assetIDlist);
                }
                else if (LHDR.assetIDlist.Count != 0)
                {
                    layers[LHDR.layerType] = LHDR;
                }
            }

            UnsavedChanges = true;

            DICT.LTOC.LHDRList = new List<Section_LHDR>();
            DICT.LTOC.LHDRList.AddRange(layers.Values.ToList());
            DICT.LTOC.LHDRList.AddRange(bspLayers);
            DICT.LTOC.LHDRList = DICT.LTOC.LHDRList.OrderBy(f => f.layerType, new LHDRComparer(game)).ToList();
        }

        public string VerifyArchive()
        {
            string result = "";
            char endl = '\n';

            ProgressBar progressBar = new ProgressBar("Verify Archive");
            progressBar.SetProgressBar(0, DICT.ATOC.AHDRList.Count, 1);
            progressBar.Show();

            foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                foreach (uint assetID in LHDR.assetIDlist)
                    if (!ContainsAsset(assetID))
                        result += $"Archive: Asset 0x{assetID.ToString("X8")} appears to be present in a layer, but it's not in the AHDR dictionary. This archive is likely unusable." + endl;

            List<Asset> ordered = assetDictionary.Values.OrderBy(f => f.assetName).ToList();
            ordered = ordered.OrderBy(f => f.assetType).ToList();

            if (!ContainsAssetWithType(AssetType.JSP))
                result += $"Archive: Does not contain any JSP asset." + endl;

            foreach (Asset asset in ordered)
            {
                bool found = false;

                foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    foreach (uint assetID in LHDR.assetIDlist)
                        if (assetID == asset.assetID)
                        {
                            if (found == false)
                                found = true;
                            else
                                result += $"Archive: Asset {asset} is present in more than one layer. This is unexpected." + endl;
                        }

                if (found == false)
                    result += $"Archive: Asset {asset} appears to not be present in the AHDR dictionary, but it's not in any layer. This archive is likely unusable." + endl;

                List<string> resultParam = new List<string>();
                try
                {
                    asset.Verify(ref resultParam);

                    foreach (string s in resultParam)
                        result += $"[{asset.assetType.ToString()}] {asset.assetName}: " + s + endl;
                }
                catch (Exception e)
                {
                    result += $"Failed verification on [{asset.assetType.ToString()}] {asset.assetName}: " + e.Message + endl;
                }

                progressBar.PerformStep();
            }

            progressBar.Close();

            return result;
        }

        public void ApplyScale(Vector3 factor)
        {
            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetTRIG TRIG)
                {
                    if (TRIG.Shape != TriggerShape.Box)
                    {
                        TRIG.MinimumX *= factor.X;
                        TRIG.MinimumY *= factor.Y;
                        TRIG.MinimumZ *= factor.Z;
                        TRIG.Radius *= singleFactor;
                        TRIG.Height *= singleFactor;
                    }
                    else
                    {
                        Vector3 TrigCenter = new Vector3(TRIG.MinimumX + TRIG.Radius, TRIG.MinimumY + TRIG.MaximumY, TRIG.MinimumZ + TRIG.MaximumZ) / 2f;

                        TRIG.MinimumX -= TrigCenter.X;
                        TRIG.MinimumY -= TrigCenter.Y;
                        TRIG.MinimumZ -= TrigCenter.Z;
                        TRIG.MinimumX -= TrigCenter.X;
                        TRIG.MaximumY -= TrigCenter.Y;
                        TRIG.MaximumZ -= TrigCenter.Z;

                        TRIG.MinimumX *= factor.X;
                        TRIG.MinimumY *= factor.Y;
                        TRIG.MinimumZ *= factor.Z;
                        TRIG.MinimumX *= factor.X;
                        TRIG.MaximumY *= factor.Y;
                        TRIG.MaximumZ *= factor.Z;

                        TRIG.MinimumX += TrigCenter.X * factor.X;
                        TRIG.MinimumY += TrigCenter.Y * factor.Y;
                        TRIG.MinimumZ += TrigCenter.Z * factor.Z;
                        TRIG.MinimumX += TrigCenter.X * factor.X;
                        TRIG.MaximumY += TrigCenter.Y * factor.Y;
                        TRIG.MaximumZ += TrigCenter.Z * factor.Z;
                    }

                    TRIG.FixPosition();
                }
                else if (a is IClickableAsset ica)
                {
                    ica.PositionX *= factor.X;
                    ica.PositionY *= factor.Y;
                    ica.PositionZ *= factor.Z;

                    if (a is AssetMVPT MVPT)
                    {
                        if (MVPT.ZoneRadius != -1)
                            MVPT.ZoneRadius *= singleFactor;
                        if (MVPT.ArenaRadius != -1)
                            MVPT.ArenaRadius *= singleFactor;
                    }
                    else if (a is AssetSFX SFX)
                    {
                        SFX.OuterRadius *= singleFactor;
                        SFX.InnerRadius *= singleFactor;
                    }
                    else if (a is AssetSGRP SGRP)
                    {
                        SGRP.OuterRadius *= singleFactor;
                        SGRP.InnerRadius *= singleFactor;
                    }
                    else if (a is EntityAsset placeable && !(a is AssetPLYR || a is AssetPKUP || a is AssetUI || a is AssetUIFT || a is AssetVIL || (a is AssetDYNA DYNA && DYNA.Type == DynaType.game_object__Teleport)))
                    {
                        placeable.ScaleX *= factor.X;
                        placeable.ScaleY *= factor.Y;
                        placeable.ScaleZ *= factor.Z;
                    }
                }
            }

            UnsavedChanges = true;
            RecalculateAllMatrices();
        }

        public List<uint> MakeSimps(List<uint> assetIDs, bool solid, bool ledgeGrabSimps)
        {
            int layerIndex = DICT.LTOC.LHDRList.Count;
            AddLayer();
            List<uint> outAssetIDs = new List<uint>();

            foreach (uint i in assetIDs)
                if (GetFromAssetID(i) is AssetMODL MODL)
                {
                    string simpName = "SIMP_" + MODL.assetName.Replace(".dff", "").ToUpper();
                    AssetSIMP simp = (AssetSIMP)PlaceTemplate(new Vector3(), layerIndex, ref outAssetIDs, simpName, AssetTemplate.SIMP_Generic);
                    simp.Model_AssetID = i;
                    if (!solid)
                    {
                        simp.SolidityFlags.FlagValueByte = 0;
                        simp.CollType.FlagValueByte = 0;
                    }
                    else if (ledgeGrabSimps)
                    {
                        simp.SolidityFlags.FlagValueByte = 0x82;
                    }
                }

            return outAssetIDs;
        }

        public int IndexOfLayerOfType(int layerType)
        {
            int layerIndex = -1;
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].layerType == layerType)
                {
                    layerIndex = i;
                    break;
                }

            if (layerIndex == -1)
            {
                AddLayer();
                DICT.LTOC.LHDRList.Last().layerType = layerType;
                layerIndex = DICT.LTOC.LHDRList.Count - 1;
            }

            return layerIndex;
        }

        public void MakePiptVcolors(List<uint> assetIDs)
        {
            AssetPIPT pipt = null;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPIPT PIPT)
                {
                    pipt = PIPT;
                    break;
                }
            if (pipt == null)
                pipt = (AssetPIPT)PlaceTemplate(new Vector3(), IndexOfLayerOfType((int)LayerType_BFBB.DEFAULT), template: AssetTemplate.PipeInfoTable);

            List<EntryPIPT> entries = pipt.PIPT_Entries.ToList();

            foreach (uint u in assetIDs)
                if (GetFromAssetID(u) is AssetMODL)
                    entries.Add(new EntryPIPT()
                    {
                        ModelAssetID = u,
                        LightingMode = 1
                    });

            pipt.PIPT_Entries = entries.ToArray();
        }

        private byte[] ReplaceReferences(byte[] data, Dictionary<uint, uint> referenceUpdate)
        {
            for (int i = 0; i < data.Length; i += 4)
                foreach (var key in referenceUpdate.Keys)
                    if (BitConverter.ToUInt32(data, i) == key)
                    {
                        byte[] nd = BitConverter.GetBytes(referenceUpdate[key]);
                        for (int j = 0; j < 4; j++)
                            data[i + j] = nd[j];
                    }

            return data;
        }

        public static void ExportScene(string folderName, Assimp.ExportFormatDescription format, string textureExtension, out string[] textureNames)
        {
            List<string> textureNamesList = new List<string>();

            lock (renderableJSPs)
                foreach (var v in renderableJSPs)
                    try
                    {
                        textureNamesList.AddRange(v.Textures);
                        Assimp_IO.ExportAssimp(
                        Path.Combine(folderName, v.assetName + "." + format.FileExtension),
                        ReadFileMethods.ReadRenderWareFile(v.Data), true, format, textureExtension, Matrix.Identity);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Unable to export asset {v}: {e.Message}");
                    }

            lock (renderableAssets)
                foreach (var v in renderableAssets)
                    try
                    {
                        Asset modelAsset;
                        string assetName;
                        Matrix world;

                        if (v is EntityAsset entity)
                        {
                            if (entity.isInvisible || entity.DontRender || entity is AssetTRIG)
                                continue;

                            if (entity is AssetPKUP pkup)
                            {
                                if (AssetPICK.pickEntries.ContainsKey(pkup.PickReferenceID))
                                    modelAsset = (Asset)renderingDictionary[pkup.PickReferenceID];
                                else continue;
                            }
                            else
                                modelAsset = (Asset)renderingDictionary[entity.Model_AssetID];

                            assetName = entity.assetName;
                            world = entity.world;
                        }
                        else if (v is AssetDYNA dyna && !AssetDYNA.dontRender && dyna is IRenderableAsset)
                        {
                            if (dyna.isInvisible)
                                continue;

                            if (dyna is DynaGObjectRing ring)
                            {
                                modelAsset = (Asset)renderingDictionary[DynaGObjectRingControl.RingModelAssetID];
                                world = ring.world;
                            }
                            else if (dyna is DynaEnemySB enemySb)
                            {
                                modelAsset = (Asset)renderingDictionary[enemySb.Model_AssetID];
                                world = enemySb.world;
                            }
                            else continue;

                            assetName = dyna.assetName;
                        }
                        else continue;

                        if (modelAsset is AssetMINF minf)
                            modelAsset = (Asset)renderingDictionary[minf.MinfReferences[0].Model_AssetID];

                        if (modelAsset is AssetMODL modl)
                            textureNamesList.AddRange(modl.Textures);
                        else continue;

                        Assimp_IO.ExportAssimp(
                            Path.Combine(folderName, assetName + "." + format.FileExtension),
                            ReadFileMethods.ReadRenderWareFile(modl.Data), true, format, textureExtension, world);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show($"Unable to export asset {v}: {e.Message}");
                    }

            textureNames = textureNamesList.ToArray();
        }
    }
}
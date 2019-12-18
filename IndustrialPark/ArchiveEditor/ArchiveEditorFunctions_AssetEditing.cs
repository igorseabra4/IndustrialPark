using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HipHopFile;
using SharpDX;
using static HipHopFile.Functions;

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
            return (from asset in assetDictionary.Values where asset.isInvisible select asset.AHDR.assetID).ToList();
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

        public static bool hideHelp { get; set; }

        private void OpenInternalEditor(Asset asset)
        {
            CloseInternalEditor(asset.AHDR.assetID);

            switch (asset.AHDR.assetType)
            {
                case AssetType.BUTN:
                    internalEditors.Add(new InternalButtonEditor((AssetBUTN)asset, this, hideHelp));
                    break;
                case AssetType.CAM:
                    internalEditors.Add(new InternalCamEditor((AssetCAM)asset, this));
                    break;
                case AssetType.DYNA:
                    internalEditors.Add(new InternalDynaEditor((AssetDYNA)asset, this, hideHelp));
                    break;
                case AssetType.FLY:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this));
                    break;
                case AssetType.GRUP:
                //case AssetType.ANIM:
                    internalEditors.Add(new InternalGrupEditor(asset, this));
                    break;
                case AssetType.BSP:
                case AssetType.JSP:
                case AssetType.MODL:
                    internalEditors.Add(new InternalModelEditor((AssetRenderWareModel)asset, this, hideHelp));
                    break;
                case AssetType.PLAT:
                    internalEditors.Add(new InternalPlatEditor((AssetPLAT)asset, this, hideHelp));
                    break;
                case AssetType.RWTX:
                    internalEditors.Add(new InternalTextureEditor((AssetRWTX)asset, this, hideHelp));
                    break;
                case AssetType.SHRP:
                    internalEditors.Add(new InternalShrapnelEditor((AssetSHRP)asset, this));
                    break;
                case AssetType.SND:
                case AssetType.SNDS:
                    internalEditors.Add(new InternalSoundEditor(asset, this));
                    break;
                case AssetType.TEXT:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this, hideHelp));
                    break;
            }

            internalEditors.Last().Show();
        }

        public void SetAllTopMost(bool value)
        {
            foreach (var ie in internalEditors)
                ie.TopMost = value;
        }

        internal void SetHideHelp(bool hideHelp)
        {
            foreach (var ie in internalEditors)
                ie.SetHideHelp(hideHelp);
        }

        public static Vector3 GetRayInterserctionPosition(Ray ray)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                l.AddRange(renderableAssetSetCommon);
                l.AddRange(renderableAssetSetTrans);
                l.AddRange(renderableAssetSetJSP);
            }
            catch { return Vector3.Zero; }

            float smallerDistance = 2000f;
            bool found = false;

            foreach (IRenderableAsset ra in l)
            {
                float? distance = ra.IntersectsWith(ray);
                if (distance != null && distance < smallerDistance)
                {
                    found = true;
                    smallerDistance = (float)distance;
                }
            }

            return ray.Position + Vector3.Normalize(ray.Direction) * (found ? smallerDistance : 2f);
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

        public static uint GetClickedAssetID2D(Ray ray, float farPlane)
        {
            List<IRenderableAsset> l = new List<IRenderableAsset>();
            try
            {
                foreach (IRenderableAsset a in renderableAssetSetCommon)
                    if (a is AssetUI ui)
                        l.Add(ui);
                    else if (a is AssetUIFT uift)
                        l.Add(uift);
            }
            catch { return 0; }

            float smallerDistance = 3 * farPlane;
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

        public List<uint> FindWhoTargets(uint assetID)
        {
            List<uint> whoTargets = new List<uint>();
            foreach (Asset asset in assetDictionary.Values)
                if (asset.HasReference(assetID))
                    whoTargets.Add(asset.AHDR.assetID);

            return whoTargets;
        }

        public void MergeSimilar()
        {
            UnsavedChanges = true;

            List<Section_AHDR> COLLs = GetAssetsOfType(AssetType.COLL);
            for (int i = 1; i < COLLs.Count; i++)
                RemoveAsset(COLLs[i].assetID);
            for (int i = 1; i < COLLs.Count; i++)
                MergeCOLL(COLLs[i]);
            
            List<Section_AHDR> JAWs = GetAssetsOfType(AssetType.JAW);
            for (int i = 1; i < JAWs.Count; i++)
                RemoveAsset(JAWs[i].assetID);
            for (int i = 1; i < JAWs.Count; i++)
                MergeJAW(JAWs[i]);
            
            List<Section_AHDR> LODTs = GetAssetsOfType(AssetType.LODT);
            for (int i = 1; i < LODTs.Count; i++)            
                RemoveAsset(LODTs[i].assetID);
            for (int i = 1; i < LODTs.Count; i++)
                MergeLODT(LODTs[i]);
            
            List<Section_AHDR> PIPTs = GetAssetsOfType(AssetType.PIPT);
            for (int i = 1; i < PIPTs.Count; i++)
                RemoveAsset(PIPTs[i].assetID);
            for (int i = 1; i < PIPTs.Count; i++)
                MergePIPT(PIPTs[i]);
            
            List<Section_AHDR> SHDWs = GetAssetsOfType(AssetType.SHDW);
            for (int i = 1; i < SHDWs.Count; i++)            
                RemoveAsset(SHDWs[i].assetID);
            for (int i = 1; i < SHDWs.Count; i++)
                MergeSHDW(SHDWs[i]);
            
            List<Section_AHDR> SNDIs = GetAssetsOfType(AssetType.SNDI);
            for (int i = 1; i < SNDIs.Count; i++)            
                RemoveAsset(SNDIs[i].assetID);
            for (int i = 1; i < SNDIs.Count; i++)
                MergeSNDI(SNDIs[i]);
        }

        private void MergeCOLL(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetCOLL COLL)
                {
                    COLL.Merge(new AssetCOLL(AHDR, game, platform));
                    return;
                }
            }
        }

        private void MergeJAW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetJAW JAW)
                {
                    JAW.Merge(new AssetJAW(AHDR, game, platform));
                    return;
                }
            }
        }

        private void MergeLODT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetLODT LODT)
                {
                    LODT.Merge(new AssetLODT(AHDR, game, platform));
                    return;
                }
            }
        }

        private void MergePIPT(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetPIPT PIPT)
                {
                    PIPT.Merge(new AssetPIPT(AHDR, game, platform));
                    return;
                }
            }
        }

        private void MergeSHDW(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSHDW SHDW)
                {
                    SHDW.Merge(new AssetSHDW(AHDR, game, platform));
                    return;
                }
            }
        }

        private void MergeSNDI(Section_AHDR AHDR)
        {
            foreach (Asset a in assetDictionary.Values)
            {
                if (a is AssetSNDI_GCN_V1 SNDI_G1)
                {
                    SNDI_G1.Merge(new AssetSNDI_GCN_V1(AHDR, game, platform));
                    return;
                }
                else if (a is AssetSNDI_GCN_V2 SNDI_G2)
                {
                    SNDI_G2.Merge(new AssetSNDI_GCN_V2(AHDR, game, platform));
                    return;
                }
                else if (a is AssetSNDI_XBOX SNDI_X)
                {
                    SNDI_X.Merge(new AssetSNDI_XBOX(AHDR, game, platform));
                    return;
                }
                else if (a is AssetSNDI_PS2 SNDI_P)
                {
                    SNDI_P.Merge(new AssetSNDI_PS2(AHDR, game, platform));
                    return;
                }
            }
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

            List<Asset> ordered = assetDictionary.Values.OrderBy(f => f.AHDR.ADBG.assetName).ToList();
            ordered = ordered.OrderBy(f => f.AHDR.assetType).ToList();

            if (!ContainsAssetWithType(AssetType.JSP))
                result += $"Archive: Does not contain any JSP asset." + endl;

            foreach (Asset asset in ordered)
            {
                bool found = false;

                foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
                    foreach (uint assetID in LHDR.assetIDlist)
                        if (assetID == asset.AHDR.assetID)
                        {
                            if (found == false)
                                found = true;
                            else
                                result += $"Archive: Asset {asset.ToString()} is present in more than one layer. This is unexpected." + endl;
                        }

                if (found == false)
                    result += $"Archive: Asset {asset.ToString()} appears to not be present in the AHDR dictionary, but it's not in any layer. This archive is likely unusable." + endl;

                List<string> resultParam = new List<string>();
                try
                {
                    asset.Verify(ref resultParam);

                    foreach (string s in resultParam)
                        result += $"[{asset.AHDR.assetType.ToString()}] {asset.AHDR.ADBG.assetName}: " + s + endl;
                }
                catch (Exception e)
                {
                    result += $"Failed verification on [{asset.AHDR.assetType.ToString()}] {asset.AHDR.ADBG.assetName}: " + e.Message + endl;
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
                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Radius *= singleFactor;
                        TRIG.Height *= singleFactor;
                    }
                    else
                    {
                        Vector3 TrigCenter = new Vector3(TRIG.Position0X + TRIG.Radius, TRIG.Position0Y + TRIG.Position1Y, TRIG.Position0Z + TRIG.Position1Z) / 2f;

                        TRIG.Position0X -= TrigCenter.X;
                        TRIG.Position0Y -= TrigCenter.Y;
                        TRIG.Position0Z -= TrigCenter.Z;
                        TRIG.Position0X -= TrigCenter.X;
                        TRIG.Position1Y -= TrigCenter.Y;
                        TRIG.Position1Z -= TrigCenter.Z;

                        TRIG.Position0X *= factor.X;
                        TRIG.Position0Y *= factor.Y;
                        TRIG.Position0Z *= factor.Z;
                        TRIG.Position0X *= factor.X;
                        TRIG.Position1Y *= factor.Y;
                        TRIG.Position1Z *= factor.Z;

                        TRIG.Position0X += TrigCenter.X * factor.X;
                        TRIG.Position0Y += TrigCenter.Y * factor.Y;
                        TRIG.Position0Z += TrigCenter.Z * factor.Z;
                        TRIG.Position0X += TrigCenter.X * factor.X;
                        TRIG.Position1Y += TrigCenter.Y * factor.Y;
                        TRIG.Position1Z += TrigCenter.Z * factor.Z;
                    }

                    TRIG.FixPosition();
                }
                else if (a is IClickableAsset clickableAsset)
                {
                    ((IClickableAsset)a).PositionX *= factor.X;
                    ((IClickableAsset)a).PositionY *= factor.Y;
                    ((IClickableAsset)a).PositionZ *= factor.Z;

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
                    else if (a is PlaceableAsset placeable && !(a is AssetPLYR || a is AssetPKUP || a is AssetUI || a is AssetUIFT || a is AssetVIL || (a is AssetDYNA DYNA && DYNA.Type_BFBB == DynaType_BFBB.game_object__Teleport)))
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

        private void ConvertAllAssetTypes(Platform previousPlatform, Game previousGame, out HashSet<AssetType> unsupported)
        {
            unsupported = new HashSet<AssetType>();
            HashSet<AssetType> unsupportedDefault = new HashSet<AssetType>() { AssetType.BSP, AssetType.JSP, AssetType.RWTX, AssetType.MODL };
            foreach (Asset asset in assetDictionary.Values)
                try
                {
                    asset.AHDR = ConvertAssetType(asset.AHDR, EndianConverter.PlatformEndianness(previousPlatform), EndianConverter.PlatformEndianness(platform), previousGame, game);

                    if (unsupportedDefault.Contains(asset.AHDR.assetType))
                        unsupported.Add(asset.AHDR.assetType);
                    else
                    {
                        asset.game = game;
                        asset.platform = platform;
                    }
                }
                catch
                {
                    unsupported.Add(asset.AHDR.assetType);
                }
        }

        private Section_AHDR ConvertAssetType(Section_AHDR AHDR, Endianness previousEndianness, Endianness currentEndianness, Game previousGame,  Game currentGame)
        {
            if (previousGame != currentGame)
            {
                AHDR = EndianConverter.GetReversedEndian(AHDR, previousGame, currentGame, previousEndianness);

                if (previousEndianness == currentEndianness)
                    AHDR = EndianConverter.GetReversedEndian(AHDR, currentGame, currentGame, currentEndianness == Endianness.Big ? Endianness.Little : Endianness.Big);
            }
            else if (previousEndianness != currentEndianness)
                AHDR = EndianConverter.GetReversedEndian(AHDR, previousGame, currentGame, previousEndianness);

            return AHDR;
        }

        public List<uint> MakeSimps(List<uint> assetIDs)
        {
            int layerIndex = DICT.LTOC.LHDRList.Count;
            AddLayer();
            List<uint> outAssetIDs = new List<uint>();

            foreach (uint i in assetIDs)
                if (GetFromAssetID(i) is AssetMODL MODL)
                {
                    string simpName = "SIMP_" + MODL.AHDR.ADBG.assetName.Replace(".dff", "").ToUpper();
                    uint simpAssetID = PlaceTemplate(new Vector3(), layerIndex, out bool success, ref outAssetIDs, simpName, AssetTemplate.SIMP_Generic);
                    ((AssetSIMP)GetFromAssetID(simpAssetID)).Model_AssetID = i;
                }

            return outAssetIDs;
        }

        private int IndexOfDefaultLayer()
        {
            int defaultLayerIndex = -1;
            for (int i = 0; i < DICT.LTOC.LHDRList.Count; i++)
                if (DICT.LTOC.LHDRList[i].layerType == 0)
                {
                    defaultLayerIndex = i;
                    break;
                }

            if (defaultLayerIndex == -1)
            {
                AddLayer();
                defaultLayerIndex = DICT.LTOC.LHDRList.Count - 1;
            }

            return defaultLayerIndex;
        }

        public void MakePiptVcolors(List<uint> assetIDs)
        {
            AssetPIPT pipt = null;

            foreach (Asset a in assetDictionary.Values)
                if (a is AssetPIPT PIPT)
                    pipt = PIPT;
            if (pipt == null)
            {
                List<uint> assetIDs2 = new List<uint>();
                pipt = (AssetPIPT)GetFromAssetID(PlaceTemplate(new Vector3(), IndexOfDefaultLayer(), out _, ref assetIDs2, template: AssetTemplate.PipeInfoTable));
            }

            List<EntryPIPT> entries = pipt.PIPT_Entries.ToList();

            foreach (uint u in assetIDs)
                if (GetFromAssetID(u) is AssetMODL)
                    entries.Add(new EntryPIPT()
                    {
                        Culling = 0x98,
                        MeshIndex = -1,
                        ModelAssetID = u,
                        OtherFlags = 0x42
                    });

            pipt.PIPT_Entries = entries.ToArray();
        }
    }
}
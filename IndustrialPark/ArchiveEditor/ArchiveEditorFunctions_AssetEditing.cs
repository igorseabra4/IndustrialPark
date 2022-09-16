using HipHopFile;
using IndustrialPark.Models;
using Newtonsoft.Json;
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

        public void OpenInternalEditor(IEnumerable<uint> list, bool openAnyway, Action<Asset> updateListView)
        {
            bool willOpen = true;
            if (list.Count() > 15 && !openAnyway)
            {
                willOpen = MessageBox.Show($"Warning: you're going to open {list.Count()} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }

            if (willOpen)
                foreach (uint u in list)
                    if (assetDictionary.ContainsKey(u))
                        OpenInternalEditor(assetDictionary[u], updateListView);
        }

        private void OpenInternalEditor(Asset asset, Action<Asset> updateListView)
        {
            CloseInternalEditor(asset.assetID);

            switch (asset.assetType)
            {
                case AssetType.Flythrough:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this));
                    break;
                case AssetType.Texture:
                    if (asset is AssetRWTX rwtx)
                        internalEditors.Add(new InternalTextureEditor(rwtx, this));
                    else
                        internalEditors.Add(new InternalAssetEditor(asset, this, updateListView));
                    break;
                case AssetType.Sound:
                case AssetType.StreamingSound:
                    internalEditors.Add(new InternalSoundEditor((AssetSound)asset, this));
                    break;
                case AssetType.Text:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this));
                    break;
                default:
                    internalEditors.Add(new InternalAssetEditor(asset, this, updateListView));
                    break;
            }

            internalEditors.Last().Show();
        }

        private List<InternalMultiAssetEditor> multiInternalEditors = new List<InternalMultiAssetEditor>();

        public void OpenInternalEditorMulti(IEnumerable<uint> list, Action<Asset> updateListView)
        {
            var assets = new List<Asset>();
            foreach (var u in list)
                if (assetDictionary.ContainsKey(u))
                    assets.Add(assetDictionary[u]);

            multiInternalEditors.Add(new InternalMultiAssetEditor(assets.ToArray(), this, updateListView));
            multiInternalEditors.Last().Show();
        }

        public void CloseInternalEditorMulti(uint assetID)
        {
            for (int i = 0; i < multiInternalEditors.Count; i++)
                if (multiInternalEditors[i].AssetIDs.Contains(assetID))
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

        public static uint? GetClickedAssetID(SharpRenderer renderer, Ray ray)
        {
            float smallerDistance = 1000f;
            uint? assetID = null;

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

        public static uint? GetClickedAssetID2D(SharpRenderer renderer, Ray ray, float farPlane)
        {
            float smallerDistance = 3 * farPlane;
            uint? assetID = null;
            
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

            var COLLs = (from asset in assetDictionary.Values where asset.assetType == AssetType.CollisionTable select (AssetCOLL)asset).ToList();
            for (int i = 1; i < COLLs.Count; i++)
                RemoveAsset(COLLs[i].assetID);
            for (int i = 1; i < COLLs.Count; i++)
                MergeCOLL(COLLs[i]);

            var JAWs = (from asset in assetDictionary.Values where asset.assetType == AssetType.JawDataTable select (AssetJAW)asset).ToList();
            for (int i = 1; i < JAWs.Count; i++)
                RemoveAsset(JAWs[i].assetID);
            for (int i = 1; i < JAWs.Count; i++)
                MergeJAW(JAWs[i]);

            var LODTs = (from asset in assetDictionary.Values where asset.assetType == AssetType.LevelOfDetailTable select (AssetLODT)asset).ToList();
            for (int i = 1; i < LODTs.Count; i++)
                RemoveAsset(LODTs[i].assetID);
            for (int i = 1; i < LODTs.Count; i++)
                MergeLODT(LODTs[i]);

            var PIPTs = (from asset in assetDictionary.Values where asset.assetType == AssetType.PipeInfoTable select (AssetPIPT)asset).ToList();
            for (int i = 1; i < PIPTs.Count; i++)
                RemoveAsset(PIPTs[i].assetID);
            for (int i = 1; i < PIPTs.Count; i++)
                MergePIPT(PIPTs[i]);

            var SHDWs = (from asset in assetDictionary.Values where asset.assetType == AssetType.ShadowTable select (AssetSHDW)asset).ToList();
            for (int i = 1; i < SHDWs.Count; i++)
                RemoveAsset(SHDWs[i].assetID);
            for (int i = 1; i < SHDWs.Count; i++)
                MergeSHDW(SHDWs[i]);


            if (platform == Platform.GameCube)
            {
                if (game == Game.Incredibles)
                {
                    var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SoundInfo select (AssetSNDI_GCN_V2)asset).ToList();
                    for (int i = 1; i < SNDIs.Count; i++)
                        RemoveAsset(SNDIs[i].assetID);
                    for (int i = 1; i < SNDIs.Count; i++)
                        MergeSNDI(SNDIs[i]);
                }
                else
                {
                    var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SoundInfo select (AssetSNDI_GCN_V1)asset).ToList();
                    for (int i = 1; i < SNDIs.Count; i++)
                        RemoveAsset(SNDIs[i].assetID);
                    for (int i = 1; i < SNDIs.Count; i++)
                        MergeSNDI(SNDIs[i]);
                }
            }
            else if (platform == Platform.Xbox)
            {
                var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SoundInfo select (AssetSNDI_XBOX)asset).ToList();
                for (int i = 1; i < SNDIs.Count; i++)
                    RemoveAsset(SNDIs[i].assetID);
                for (int i = 1; i < SNDIs.Count; i++)
                    MergeSNDI(SNDIs[i]);
            }
            else if (platform == Platform.PS2)
            {
                var SNDIs = (from asset in assetDictionary.Values where asset.assetType == AssetType.SoundInfo select (AssetSNDI_PS2)asset).ToList();
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

        public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
        {
            if (assetType.IsDyna())
                return AHDRFlags.SOURCE_VIRTUAL;
            switch (assetType)
            {
                case AssetType.AnimationList:
                case AssetType.Boulder:
                case AssetType.Button:
                case AssetType.Camera:
                case AssetType.Counter:
                case AssetType.CollisionTable:
                case AssetType.Conditional:
                case AssetType.CutsceneManager:
                case AssetType.CutsceneTableOfContents:
                case AssetType.Dispatcher:
                case AssetType.DiscoFloor:
                case AssetType.DestructibleObject:
                case AssetType.ElectricArcGenerator:
                case AssetType.Environment:
                case AssetType.Fog:
                case AssetType.Hangable:
                case AssetType.Group:
                case AssetType.JawDataTable:
                case AssetType.LevelOfDetailTable:
                case AssetType.SurfaceMapper:
                case AssetType.ModelInfo:
                case AssetType.Marker:
                case AssetType.MovePoint:
                case AssetType.ParticleEmitter:
                case AssetType.ParticleProperties:
                case AssetType.ParticleSystem:
                case AssetType.Pendulum:
                case AssetType.PickupTable:
                case AssetType.PipeInfoTable:
                case AssetType.Pickup:
                case AssetType.Platform:
                case AssetType.Player:
                case AssetType.Portal:
                case AssetType.SFX:
                case AssetType.ShadowTable:
                case AssetType.Shrapnel:
                case AssetType.SimpleObject:
                case AssetType.SoundInfo:
                case AssetType.Surface:
                case AssetType.Text:
                case AssetType.Timer:
                case AssetType.Trigger:
                case AssetType.UserInterface:
                case AssetType.UserInterfaceFont:
                case AssetType.NPC:
                case AssetType.NPCProperties:
                    return AHDRFlags.SOURCE_VIRTUAL;
                case AssetType.Cutscene:
                case AssetType.Flythrough:
                case AssetType.RawImage:
                    return AHDRFlags.SOURCE_FILE;
                case AssetType.Animation:
                case AssetType.Credits:
                case AssetType.Sound:
                case AssetType.StreamingSound:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM;
                case AssetType.BSP:
                case AssetType.Model:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM;
                case AssetType.AnimationTable:
                case AssetType.JSP:
                case AssetType.JSPInfo:
                case AssetType.Texture:
                    return AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM;
                case AssetType.LightKit:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM;
                default:
                    return 0;
            }
        }

        public void OrganizeLayers()
        {
            Layers = BuildLayers();
            UnsavedChanges = true;
        }

        private List<Layer> BuildLayers()
        {
            Dictionary<int, Layer> layers = new Dictionary<int, Layer>();

            void AddToLayerOfType(uint assetId, int type)
            {
                if (!layers.ContainsKey(type))
                    layers[type] = new Layer(type);
                layers[type].AssetIDs.Add(assetId);
            }

            foreach (Asset a in assetDictionary.Values)
                AddToLayerOfType(a.assetID, GetLayerTypeOfAsset(a.assetType));

            return layers.Values.OrderBy(f => f.Type, new LayerComparer(game)).ToList();
        }

        private int GetLayerTypeOfAsset(AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Texture:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.TEXTURE : (int)LayerType_BFBB.TEXTURE;
                case AssetType.BSP:
                case AssetType.JSP:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.BSP : (int)LayerType_BFBB.BSP;
                case AssetType.JSPInfo:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.JSPINFO : (int)LayerType_BFBB.JSPINFO;
                case AssetType.Model:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.MODEL : (int)LayerType_BFBB.MODEL;
                case AssetType.Animation:
                    if (game == Game.BFBB)
                        return (int)LayerType_BFBB.ANIMATION;
                    break;
                case AssetType.Cutscene:
                case AssetType.CutsceneStreamingSound:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.CUTSCENE : (int)LayerType_BFBB.CUTSCENE;
                case AssetType.Sound:
                case AssetType.StreamingSound:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.SRAM : (int)LayerType_BFBB.SRAM;
                case AssetType.SoundInfo:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.SNDTOC : (int)LayerType_BFBB.SNDTOC;
                case AssetType.CutsceneTableOfContents:
                    return game == Game.Incredibles ? (int)LayerType_TSSM.CUTSCENETOC : (int)LayerType_BFBB.SNDTOC;
            }
            return game == Game.Incredibles ? (int)LayerType_TSSM.DEFAULT : (int)LayerType_BFBB.DEFAULT;
        }

        public string VerifyArchive()
        {
            string result = "";
            char endl = '\n';

            ProgressBar progressBar = new ProgressBar("Verify Archive");
            progressBar.SetProgressBar(0, assetDictionary.Values.Count + 1, 1);
            progressBar.Show();

            //foreach (Section_LHDR LHDR in DICT.LTOC.LHDRList)
            //    foreach (uint assetID in LHDR.assetIDlist)
            //        if (!ContainsAsset(assetID))
            //            result += $"Archive: Asset 0x{assetID.ToString("X8")} appears to be present in a layer, but it's not in the AHDR dictionary. This archive is likely unusable." + endl;

            List<Asset> ordered = assetDictionary.Values.OrderBy(f => f.assetName).ToList();
            ordered = ordered.OrderBy(f => f.assetType).ToList();

            if (!ContainsAssetWithType(AssetType.JSP))
                result += $"Archive: Does not contain any JSP asset." + endl;

            progressBar.PerformStep();

            foreach (Asset asset in ordered)
            {
                bool found = false;

                List<string> resultParam = new List<string>();
                try
                {
                    asset.Verify(ref resultParam);

                    foreach (string s in resultParam)
                        result += $"[{asset.assetType}] {asset.assetName}: " + s + endl;
                }
                catch (Exception e)
                {
                    result += $"Failed verification on [{asset.assetType}] {asset.assetName}: " + e.Message + endl;
                }

                progressBar.PerformStep();
            }

            progressBar.Close();

            return result;
        }

        public bool ApplyScale(Vector3 factor, IEnumerable<AssetType> assetTypes = null)
        {
            bool applied = false;
            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values)
            {
                if (assetTypes != null && !assetTypes.Contains(a.assetType))
                    continue;

                if (a is AssetTRIG TRIG)
                {
                    TRIG.ApplyScale(factor, singleFactor);
                    applied = true;
                }
                else if (a is IClickableAsset ica && !(a is DynaGObjectTeleport))
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
                    applied = true;
                }
                else if (a is AssetJSP jsp)
                {
                    jsp.ApplyScale(factor);
                    applied = true;
                }
                else if (a is AssetJSP_INFO jspinfo)
                {
                    jspinfo.ApplyScale(factor);
                    applied = true;
                }
                else if (a is AssetLODT lodt && singleFactor > 1.0)
                {
                    var entries = lodt.Entries;
                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i].MaxDistance *= singleFactor;
                        entries[i].LOD1_MinDistance *= singleFactor;
                        entries[i].LOD2_MinDistance *= singleFactor;
                        entries[i].LOD3_MinDistance *= singleFactor;
                    }
                    lodt.Entries = entries;
                    applied = true;
                }
                else if (a is AssetFLY fly)
                {
                    var entries = fly.Frames;
                    for (int i = 0; i < entries.Length; i++)
                    {
                        entries[i].CameraPosition.X *= factor.X;
                        entries[i].CameraPosition.Y *= factor.Y;
                        entries[i].CameraPosition.Z *= factor.Z;
                    }
                    fly.Frames = entries;
                    applied = true;
                }
            }

            UnsavedChanges = true;
            RecalculateAllMatrices();
            return applied;
        }

        public List<uint> MakeSimps(List<uint> assetIDs, bool solid, bool ledgeGrabSimps)
        {
            if (!NoLayers)
            {
                AddLayer();
                SelectedLayerIndex = Layers.Count - 1;
            }

            List<uint> outAssetIDs = new List<uint>();

            foreach (uint i in assetIDs)
                if (GetFromAssetID(i) is AssetMODL MODL)
                {
                    string simpName = "SIMP_" + MODL.assetName.Replace(".dff", "").ToUpper();
                    AssetSIMP simp = (AssetSIMP)PlaceTemplate(new Vector3(), ref outAssetIDs, simpName, AssetTemplate.Simple_Object);
                    simp.Model = i;
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

            if (!NoLayers)
            {
                layerIndex = Layers.FindIndex(l => l.Type == layerType);
                if (layerIndex == -1)
                {
                    AddLayer();
                    Layers.Last().Type = layerType;
                    layerIndex = LayerCount - 1;
                }
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
            {
                var prevLayerType = SelectedLayerIndex;

                if (!NoLayers) SelectedLayerIndex = IndexOfLayerOfType((int)LayerType_BFBB.DEFAULT);
                
                pipt = (AssetPIPT)PlaceTemplate(template: AssetTemplate.Pipe_Info_Table);

                if (!NoLayers) SelectedLayerIndex = prevLayerType;
            }

            List<PipeInfo> entries = pipt.Entries.ToList();

            foreach (uint u in assetIDs)
                if (GetFromAssetID(u) is AssetMODL)
                    entries.Add(new PipeInfo()
                    {
                        Model = u,
                        LightingMode = LightingMode.Prelight
                    });

            pipt.Entries = entries.ToArray();
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

        public static uint ApplyBakeScale(uint modelAssetId, Vector3 scale)
        {
            if (scale.X == 1f && scale.Y == 1f && scale.Z == 1f)
            {
                MessageBox.Show("Bake scale not applied as the scale vector is (1, 1, 1).");
                return modelAssetId;
            }

            if (Program.MainForm == null)
            {
                MessageBox.Show("Cannot bake scale on standalone Archive Editor.\nPlease do it manually by manually creating a copy of the model and changing the scale there.");
                return modelAssetId;
            }

            var bsmcs = new List<BakeScaleModelContainer>();
            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(modelAssetId) && ae.archive.GetFromAssetID(modelAssetId) is IAssetWithModel model)
                {
                    bsmcs.Add(new BakeScaleModelContainer()
                    {
                        archive = ae.archive,
                        model = model
                    });
                }

            if (bsmcs.Count == 0)
            {
                MessageBox.Show("Unable to find model in open archives.");
            }
            else if (bsmcs.Count == 1)
            {
                return bsmcs[0].archive.ApplyBakeScaleLocal(modelAssetId, scale);
            }
            else
            {
                BakeScale bs = new BakeScale(bsmcs);
                bs.ShowDialog();
                if (bs.OK)
                    bs.GetSelectedArchive().ApplyBakeScaleLocal(modelAssetId, scale);
            }

            return modelAssetId;
        }

        public uint ApplyBakeScaleLocal(uint modelAssetId, Vector3 scale)
        {
            if (ContainsAsset(modelAssetId) && GetFromAssetID(modelAssetId) is AssetMODL modl)
            {
                var AHDR = modl.BuildAHDR();
                var newAssetName = AHDR.ADBG.assetName + $"_{scale.X:.0###########}_{scale.Y:.0###########}_{scale.Z:.0###########}";
                var newAssetId = Functions.BKDRHash(newAssetName);

                if (!ContainsAsset(newAssetId))
                {
                    AHDR.ADBG.assetName = newAssetName;
                    AHDR.assetID = newAssetId;

                    var prevLayerType = SelectedLayerIndex;
                    if (!NoLayers)
                        SelectedLayerIndex = IndexOfLayerOfType((int)LayerType_BFBB.DEFAULT);

                    newAssetId = AddAsset(AHDR, game, platform.Endianness(), setTextureDisplay: false);

                    if (!NoLayers)
                        SelectedLayerIndex = prevLayerType;
                    ((AssetMODL)GetFromAssetID(newAssetId)).ApplyScale(scale);
                    UnsavedChanges = true;
                }

                return newAssetId;
            }
            else if (ContainsAsset(modelAssetId) && GetFromAssetID(modelAssetId) is AssetMINF minf)
            {
                var AHDR = minf.BuildAHDR();
                var newAssetName = AHDR.ADBG.assetName + $"_{scale.X:.0###########}_{scale.Y:.0###########}_{scale.Z:.0###########}";
                var newAssetId = Functions.BKDRHash(newAssetName);

                if (!ContainsAsset(newAssetId))
                {
                    AHDR.ADBG.assetName = newAssetName;
                    AHDR.assetID = newAssetId;

                    var prevLayerType = SelectedLayerIndex;
                    if (!NoLayers)
                        SelectedLayerIndex = IndexOfLayerOfType((int)LayerType_BFBB.DEFAULT);

                    var assetId = AddAsset(AHDR, game, platform.Endianness(), setTextureDisplay: false);

                    if (!NoLayers)
                        SelectedLayerIndex = prevLayerType;

                    ((AssetMINF)GetFromAssetID(assetId)).ApplyScale(scale);
                    UnsavedChanges = true;
                }

                return newAssetId;
            }
            return modelAssetId;
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
                                modelAsset = (Asset)renderingDictionary[entity.Model];

                            assetName = entity.assetName;
                            world = entity.world;
                        }
                        else if (v is DynaGObjectRing ring)
                        {
                            if (ring.isInvisible || DynaGObjectRing.dontRender)
                                continue;

                            modelAsset = (Asset)renderingDictionary[DynaGObjectRingControl.RingModelAssetID];
                            world = ring.world;
                            assetName = ring.assetName;
                        }
                        else if (v is DynaEnemySB enemySb)
                        {
                            if (enemySb.isInvisible || enemySb.DontRender)
                                continue;

                            modelAsset = (Asset)renderingDictionary[enemySb.Model];
                            world = enemySb.world;
                            assetName = enemySb.assetName;
                        }
                        else continue;

                        if (modelAsset is AssetMINF minf)
                            modelAsset = (Asset)renderingDictionary[minf.References[0].Model];

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
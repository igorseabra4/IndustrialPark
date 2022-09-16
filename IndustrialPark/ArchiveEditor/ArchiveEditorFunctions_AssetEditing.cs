using Assimp;
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
using Ray = SharpDX.Ray;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static List<uint> hiddenAssets = new List<uint>();

        public List<uint> GetHiddenAssets()
        {
            return (from asset in assetDictionary.Values where asset.isInvisible select asset.assetID).ToList();
        }

        private readonly List<IInternalEditor> internalEditors = new List<IInternalEditor>();

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

        private readonly List<InternalMultiAssetEditor> multiInternalEditors = new List<InternalMultiAssetEditor>();

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

        public static Vector3 GetRayInterserctionPosition(SharpRenderer renderer, Ray ray, uint assetIdSkip = 0)
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
                if (((Asset)ra).assetID == assetIdSkip)
                    continue;

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

            foreach (Asset ra in renderableAssets.Cast<Asset>())
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
            
            foreach (Asset ra in (from IRenderableAsset asset in renderableAssets
                                 where asset is AssetUI || asset is AssetUIFT
                                 select asset).Cast<Asset>())
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

        public void DropSelectedAssets(SharpRenderer renderer)
        {
            foreach (var a in from Asset a in currentlySelectedAssets where a is IClickableAsset select (IClickableAsset)a)
            {
                if ((a is AssetTRIG trig && trig.Shape == TriggerShape.Box) || (a is AssetVOLU volu && volu.Shape == VolumeType.Box))
                    continue;

                var position = GetRayInterserctionPosition(renderer,
                    new Ray(new Vector3(a.PositionX, a.PositionY, a.PositionZ), new Vector3(0f, -1f, 0f)),
                    ((Asset)a).assetID);

                a.PositionX = position.X;
                a.PositionY = position.Y;
                a.PositionZ = position.Z;

                UnsavedChanges = true;
            }
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
            Layer textureLayer0 = new Layer(LayerType.TEXTURE);
            Layer textureLayer1 = new Layer(LayerType.TEXTURE);
            Layer textureLayer2 = new Layer(LayerType.TEXTURE);
            Dictionary<string, (Layer, Layer, Layer, Layer)> jspLayers = new Dictionary<string, (Layer, Layer, Layer, Layer)>();
            Layer modelLayer0 = new Layer(LayerType.MODEL);
            Layer modelLayer1 = new Layer(LayerType.MODEL);
            Layer modelLayer2 = new Layer(LayerType.MODEL);
            Layer animationLayer = new Layer(LayerType.ANIMATION);
            Layer defaultLayer = new Layer(LayerType.DEFAULT);
            Layer cutsceneLayer = new Layer(LayerType.CUTSCENE);
            Layer sramLayer = new Layer(LayerType.SRAM);
            Layer sndtocLayer = new Layer(LayerType.SNDTOC);
            Layer cutscenetocLayer = new Layer(LayerType.CUTSCENETOC);

            Dictionary<int, List<Layer>> layers = new Dictionary<int, List<Layer>>();

            int textureIndex = 0;
            int modelIndex = 0;

            foreach (Asset a in assetDictionary.Values)
            {
                switch (a.assetType)
                {
                    case AssetType.Texture:
                    {
                        switch (textureIndex)
                        {
                            case 0:
                                textureLayer0.AssetIDs.Add(a.assetID);
                                break;
                            case 1:
                                textureLayer1.AssetIDs.Add(a.assetID);
                                break;
                            case 2:
                                textureLayer2.AssetIDs.Add(a.assetID);
                                break;
                        }
                        textureIndex = (textureIndex + 1) % 3;
                        break;
                    }
                    case AssetType.BSP:
                    case AssetType.JSP:
                    {
                        var key = a.assetName.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                        if (!jspLayers.ContainsKey(key))
                            jspLayers[key] = (new Layer(LayerType.BSP), new Layer(LayerType.BSP), new Layer(LayerType.BSP), new Layer(LayerType.JSPINFO));

                        if (jspLayers[key].Item1.AssetIDs.Count == 0)
                            jspLayers[key].Item1.AssetIDs.Add(a.assetID);
                        else if (jspLayers[key].Item2.AssetIDs.Count == 0)
                            jspLayers[key].Item2.AssetIDs.Add(a.assetID);
                        else if (jspLayers[key].Item3.AssetIDs.Count == 0)
                            jspLayers[key].Item3.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.JSPInfo:
                    {
                        var key = a.assetName;
                        if (!jspLayers.ContainsKey(key))
                            jspLayers[key] = (new Layer(LayerType.BSP), new Layer(LayerType.BSP), new Layer(LayerType.BSP), new Layer(LayerType.JSPINFO));
                        jspLayers[key].Item4.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.Model:
                    {
                        switch (modelIndex)
                        {
                            case 0:
                                modelLayer0.AssetIDs.Add(a.assetID);
                                break;
                            case 1:
                                modelLayer1.AssetIDs.Add(a.assetID);
                                break;
                            case 2:
                                modelLayer2.AssetIDs.Add(a.assetID);
                                break;
                        }
                        modelIndex = (modelIndex + 1) % 3;
                        break;
                    }
                    case AssetType.Animation:
                    {
                        if (game == Game.BFBB)
                            animationLayer.AssetIDs.Add(a.assetID);
                        else
                            defaultLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.Cutscene:
                    case AssetType.CutsceneStreamingSound:
                    {
                        cutsceneLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.Sound:
                    case AssetType.StreamingSound:
                    {
                        sramLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.SoundInfo:
                    {
                        sndtocLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.CutsceneTableOfContents:
                    {
                        if (game == Game.Incredibles)
                            cutscenetocLayer.AssetIDs.Add(a.assetID);
                        else
                            sndtocLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    default:
                    {
                        defaultLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                }
            }

            var list = new List<Layer>();
            void AddIfNotEmpty(Layer l)
            {
                if (l.AssetIDs.Count > 0)
                    list.Add(l);
            }

            AddIfNotEmpty(textureLayer0);
            AddIfNotEmpty(textureLayer1);
            AddIfNotEmpty(textureLayer2);
            foreach (var js in jspLayers.Values)
            {
                list.Add(js.Item1);
                list.Add(js.Item2);
                list.Add(js.Item3);
                list.Add(js.Item4);
            }
            AddIfNotEmpty(modelLayer0);
            AddIfNotEmpty(modelLayer1);
            AddIfNotEmpty(modelLayer2);
            AddIfNotEmpty(animationLayer);
            AddIfNotEmpty(defaultLayer);
            AddIfNotEmpty(cutsceneLayer);
            AddIfNotEmpty(sramLayer);
            AddIfNotEmpty(sndtocLayer);
            AddIfNotEmpty(cutscenetocLayer);

            return list;
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

        public void ApplyScale(Vector3 factor, IEnumerable<AssetType> assetTypes = null, bool bakeEntityUnproportionalScales = true, bool bakeNpcsVilScales = false)
        {
            if (factor.X == 1f && factor.Y == 1f && factor.Z == 1f)
            {
                MessageBox.Show("Scale not applied as the scale vector is (1, 1, 1).");
                return;
            }

            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values.Where(a => assetTypes == null || assetTypes.Contains(a.assetType)).ToList())
            {
                if (a is IVolumeAsset volume)
                {
                    volume.ApplyScale(factor, singleFactor);
                }
                else if (a is AssetMVPT MVPT)
                {
                    MVPT.PositionX *= factor.X;
                    MVPT.PositionY *= factor.Y;
                    MVPT.PositionZ *= factor.Z;

                    if (MVPT.ZoneRadius != -1)
                        MVPT.ZoneRadius *= singleFactor;
                    if (MVPT.ArenaRadius != -1)
                        MVPT.ArenaRadius *= singleFactor;
                }
                else if (a is AssetSFX SFX)
                {
                    SFX.PositionX *= factor.X;
                    SFX.PositionY *= factor.Y;
                    SFX.PositionZ *= factor.Z;

                    SFX.OuterRadius *= singleFactor;
                    SFX.InnerRadius *= singleFactor;
                }
                else if (a is AssetBOUL BOUL)
                {
                    BOUL.PositionX *= factor.X;
                    BOUL.PositionY *= factor.Y;
                    BOUL.PositionZ *= factor.Z;

                    BOUL.ScaleX *= factor.X;
                    BOUL.ScaleY *= factor.Y;
                    BOUL.ScaleZ *= factor.Z;

                    BOUL.OuterRadius *= singleFactor;
                    BOUL.InnerRadius *= singleFactor;
                }
                else if (a is AssetSGRP SGRP)
                {
                    SGRP.OuterRadius *= singleFactor;
                    SGRP.InnerRadius *= singleFactor;
                }
                else if (a is AssetPKUP PKUP)
                {
                    PKUP.PositionX *= factor.X;
                    PKUP.PositionY *= factor.Y;
                    PKUP.PositionZ *= factor.Z;
                }
                else if (a is EntityAsset placeable && !(a is AssetPLYR || a is AssetUI || a is AssetUIFT))
                {
                    placeable.PositionX *= factor.X;
                    placeable.PositionY *= factor.Y;
                    placeable.PositionZ *= factor.Z;

                    if (placeable is AssetNPC || placeable is AssetVIL)
                    {
                        if (bakeNpcsVilScales)
                            placeable.Model = ApplyBakeScale(placeable.Model, factor);
                    }
                    else if (factor.X != factor.Y || factor.X != factor.Z || factor.Y != factor.Z)
                    {
                        if (bakeEntityUnproportionalScales)
                            placeable.Model = ApplyBakeScale(placeable.Model, factor);
                    }
                    else
                    {
                        placeable.ScaleX *= factor.X;
                        placeable.ScaleY *= factor.Y;
                        placeable.ScaleZ *= factor.Z;
                    }
                }
                else if (a is DynaEnemySB enemysb)
                {
                    enemysb.PositionX *= factor.X;
                    enemysb.PositionY *= factor.Y;
                    enemysb.PositionZ *= factor.Z;

                    if (bakeNpcsVilScales)
                        enemysb.Model = ApplyBakeScale(enemysb.Model, factor);
                }
                else if (a is DynaGObjectTrainCar tcar)
                {
                    tcar.PositionX *= factor.X;
                    tcar.PositionY *= factor.Y;
                    tcar.PositionZ *= factor.Z;

                    if (bakeEntityUnproportionalScales)
                        tcar.Model = ApplyBakeScale(tcar.Model, factor);
                }
                else if (a is IClickableAsset ica && !(a is DynaGObjectTeleport))
                {
                    ica.PositionX *= factor.X;
                    ica.PositionY *= factor.Y;
                    ica.PositionZ *= factor.Z;
                }
                else if (a is AssetJSP jsp)
                {
                    jsp.ApplyScale(factor);
                }
                else if (a is AssetJSP_INFO jspinfo)
                {
                    jspinfo.ApplyScale(factor);
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
                }
            }

            UnsavedChanges = true;
            RecalculateAllMatrices();
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

        public int IndexOfLayerOfType(LayerType layerType)
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

                if (!NoLayers) SelectedLayerIndex = IndexOfLayerOfType(LayerType.DEFAULT);
                
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
                        SelectedLayerIndex = GetLayerFromAssetID(modelAssetId);

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
                        SelectedLayerIndex = GetLayerFromAssetID(modelAssetId);

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
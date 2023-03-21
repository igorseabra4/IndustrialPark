using Assimp;
using HipHopFile;
using IndustrialPark.Models;
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
                case AssetType.Model:
                case AssetType.BSP:
                case AssetType.JSP:
                    internalEditors.Add(new InternalModelEditor((AssetRenderWareModel)asset, this, updateListView));
                    break;
                case AssetType.Flythrough:
                    internalEditors.Add(new InternalFlyEditor((AssetFLY)asset, this, updateListView));
                    break;
                case AssetType.Texture:
                    if (asset is AssetRWTX rwtx)
                        internalEditors.Add(new InternalTextureEditor(rwtx, this, updateListView));
                    else
                        internalEditors.Add(new InternalAssetEditor(asset, this, updateListView));
                    break;
                case AssetType.Sound:
                case AssetType.SoundStream:
                    internalEditors.Add(new InternalSoundEditor((AssetSound)asset, this, updateListView));
                    break;
                case AssetType.Text:
                    internalEditors.Add(new InternalTextEditor((AssetTEXT)asset, this, updateListView));
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

        public AssetPIPT GetPIPT(bool create = false) => (AssetPIPT)GetAssetOfType(AssetType.PipeInfoTable, AssetTemplate.Pipe_Info_Table, create);

        public AssetCOLL GetCOLL(bool create = false) => (AssetCOLL)GetAssetOfType(AssetType.CollisionTable, AssetTemplate.Collision_Table, create);

        public AssetLODT GetLODT(bool create = false) => (AssetLODT)GetAssetOfType(AssetType.LevelOfDetailTable, AssetTemplate.Level_Of_Detail_Table, create);

        public AssetSHDW GetSHDW(bool create = false) => (AssetSHDW)GetAssetOfType(AssetType.ShadowTable, AssetTemplate.Shadow_Table, create);

        private Asset GetAssetOfType(AssetType assetType, AssetTemplate assetTemplate, bool create)
        {
            if (!ContainsAssetWithType(assetType))
            {
                if (!create)
                    return null;

                var prevIndex = SelectedLayerIndex;

                if (!NoLayers)
                    SelectedLayerIndex = IndexOfLayerOfType(LayerType.DEFAULT);

                PlaceTemplate(assetTemplate);
                if (!standalone)
                    foreach (var ae in Program.MainForm.archiveEditors)
                        ae.PopulateAssetListAndComboBox();

                if (!NoLayers)
                    SelectedLayerIndex = prevIndex;
            }
            return (from asset in assetDictionary.Values where asset.assetType == assetType select asset).FirstOrDefault();
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
                case AssetType.SoundStream:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM;
                case AssetType.BSP:
                case AssetType.Model:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM;
                case AssetType.AnimationTable:
                case AssetType.JSP:
                case AssetType.JSPInfo:
                case AssetType.Texture:
                case AssetType.TextureStream:
                    return AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM;
                case AssetType.LightKit:
                    return AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM;
                default:
                    return 0;
            }
        }

        public bool OrganizeLayers()
        {
            try
            {
                Layers = BuildLayers();
                UnsavedChanges = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private List<Layer> BuildLayers()
        {
            Layer textureLayer0 = new Layer(LayerType.TEXTURE);
            Layer textureLayer1 = new Layer(LayerType.TEXTURE);
            Layer textureLayer2 = new Layer(LayerType.TEXTURE);
            Layer textureStrmLayer = new Layer(LayerType.TEXTURE_STRM);
            Layer bspLayer = new Layer(LayerType.BSP);
            List<Layer> jspInfoLayers = new List<Layer>();
            Layer modelLayer0 = new Layer(LayerType.MODEL);
            Layer modelLayer1 = new Layer(LayerType.MODEL);
            Layer modelLayer2 = new Layer(LayerType.MODEL);
            Layer animationLayer = new Layer(LayerType.ANIMATION);
            Layer defaultLayer = new Layer(LayerType.DEFAULT);
            Layer cutsceneLayer = new Layer(LayerType.CUTSCENE);
            Layer sramLayer = new Layer(LayerType.SRAM);
            Layer sndtocLayer = new Layer(LayerType.SNDTOC);
            Layer cutscenetocLayer = new Layer(LayerType.CUTSCENETOC);

            int textureIndex = 0;
            int modelIndex = 0;

            var doneJsps = new List<uint>();

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
                        if (game != Game.Scooby)
                            textureIndex = (textureIndex + 1) % 3;
                        break;
                    }
                    case AssetType.BinkVideo:
                    case AssetType.TextureStream:
                    case AssetType.WireframeModel:
                    {
                        textureStrmLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                    case AssetType.BSP:
                    case AssetType.JSP:
                    {
                        if (game == Game.Scooby || !ContainsAssetWithType(AssetType.JSPInfo))
                        {
                            bspLayer.AssetIDs.Add(a.assetID);
                            doneJsps.Add(a.assetID);
                        }
                        break;
                    }
                    case AssetType.JSPInfo:
                    {
                        jspInfoLayers.Add(new Layer(LayerType.JSPINFO) { AssetIDs = new List<uint>() { a.assetID } });
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
                        if (game != Game.Scooby)
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
                    case AssetType.SoundStream:
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
            AddIfNotEmpty(textureStrmLayer);
            AddIfNotEmpty(bspLayer);
            foreach (var l in jspInfoLayers)
            {
                if (GetFromAssetID(l.AssetIDs[0]) is AssetJSP_INFO jspInfo)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var assetIDs = new List<uint>(1);
                        if (i < jspInfo.JSP_AssetIDs.Length)
                        {
                            assetIDs.Add(jspInfo.JSP_AssetIDs[i]);
                            doneJsps.Add(jspInfo.JSP_AssetIDs[i]);
                        }
                        list.Add(new Layer(LayerType.BSP) { AssetIDs = assetIDs });
                    }
                }
                list.Add(l);
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

            var unusedJsps = (from Asset a in assetDictionary.Values where (a.assetType == AssetType.JSP || a.assetType == AssetType.BSP) && !doneJsps.Contains(a.assetID) select a).ToList();
            if (unusedJsps.Any())
            {
                var message = "Unable to create a layer setup for your archive. The following BSP/JSP assets are not referenced in a JSPINFO asset:\n" +
                    string.Join("\n", from Asset a in unusedJsps select $"[{a.assetID:X8}] {a.assetName}");
                throw new Exception(message);
            }

            return list;
        }

        public string VerifyArchive()
        {
            List<string> result = new List<string>();

            ProgressBar progressBar = new ProgressBar("Verify Archive");
            progressBar.SetProgressBar(0, assetDictionary.Values.Count + 1, 1);
            progressBar.Show();

            List<Asset> ordered = assetDictionary.Values.OrderBy(f => f.assetName).ToList();
            ordered = ordered.OrderBy(f => f.assetType).ToList();

            if (!ContainsAssetWithType(AssetType.JSP))
                result.Add($"Archive: Does not contain any JSP asset.");

            progressBar.PerformStep();

            foreach (Asset asset in ordered)
            {
                try
                {
                    var resultAsset = new List<string>();
                    asset.Verify(ref resultAsset);
                    foreach (string s in resultAsset)
                        result.Add($"[{AssetTypeContainer.AssetTypeToString(asset.assetType)}] {asset.assetName}: " + s);
                }
                catch (Exception e)
                {
                    result.Add($"Failed verification on [{asset.assetType}] {asset.assetName}: " + e.Message);
                }

                progressBar.PerformStep();
            }

            progressBar.Close();

            return string.Join("\n", result);
        }

        public void ApplyScale(Vector3 factor, IEnumerable<AssetType> assetTypes = null, bool bakeEntityUnproportionalScales = true, bool bakeNpcsVilScales = false)
        {
            if (factor.X == 1f && factor.Y == 1f && factor.Z == 1f)
            {
                MessageBox.Show("Scale not applied as the scale vector is (1, 1, 1).");
                return;
            }

            float singleFactor = (factor.X + factor.Y + factor.Z) / 3;

            foreach (Asset a in assetDictionary.Values.Where(a => assetTypes == null || assetTypes.Contains(a.assetType)))
            {
                if (a is IVolumeAsset volume)
                {
                    volume.ApplyScale(factor, singleFactor);
                }
                else if (a is AssetVOLU volu)
                {
                    volu.ApplyScale(factor, singleFactor);
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

                    //if (bakeNpcsVilScales)
                    //    enemysb.Model = ApplyBakeScale(enemysb.Model, factor);
                }
                else if (a is DynaGObjectTrainCar tcar)
                {
                    tcar.PositionX *= factor.X;
                    tcar.PositionY *= factor.Y;
                    tcar.PositionZ *= factor.Z;

                    //if (bakeEntityUnproportionalScales)
                    //    tcar.Model = ApplyBakeScale(tcar.Model, factor);
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

            (ArchiveEditorFunctions, IAssetWithModel) bsmc = (null, null);
            int count = 0;

            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(modelAssetId) && ae.archive.GetFromAssetID(modelAssetId) is IAssetWithModel model)
                {
                    bsmc = (ae.archive, model);
                    count++;
                    if (count > 1)
                    {
                        MessageBox.Show("Unable to bake scale: model found in more than one open archive.");
                        return modelAssetId;
                    }
                }

            if (count == 0)
                MessageBox.Show("Unable bake scale: model not found in open archives.");
            else if (count == 1)
                return bsmc.Item1.ApplyBakeScaleLocal((Asset)bsmc.Item2, scale);

            return modelAssetId;
        }

        public uint ApplyBakeScaleLocal(Asset model, Vector3 scale)
        {
            var AHDR = model.BuildAHDR(platform.Endianness());
            var newAssetName = AHDR.ADBG.assetName + $"_{scale.X:.0###########}_{scale.Y:.0###########}_{scale.Z:.0###########}";
            var newAssetId = Functions.BKDRHash(newAssetName);

            if (!ContainsAsset(newAssetId))
            {
                AHDR.ADBG.assetName = newAssetName;
                AHDR.assetID = newAssetId;

                var prevLayerType = SelectedLayerIndex;
                if (!NoLayers)
                    SelectedLayerIndex = GetLayerFromAssetID(model.assetID);

                var newModel = (IAssetWithModel)AddAsset(AHDR, game, platform.Endianness(), setTextureDisplay: false);

                if (!NoLayers)
                    SelectedLayerIndex = prevLayerType;
                newModel.ApplyScale(scale);
                UnsavedChanges = true;
            }

            return newAssetId;
        }

        public static uint ApplyBakeRotation(uint modelAssetId, float yaw, float pitch, float roll)
        {
            if (yaw == 0f && pitch == 0f && roll == 0f)
            {
                MessageBox.Show("Bake rotation not applied as the rotation vector is (0, 0, 0).");
                return modelAssetId;
            }

            if (Program.MainForm == null)
            {
                MessageBox.Show("Cannot bake rotation on standalone Archive Editor.\nPlease do it manually by manually creating a copy of the model and changing the rotation there.");
                return modelAssetId;
            }

            (ArchiveEditorFunctions, AssetMODL) bsmc = (null, null);
            int count = 0;

            foreach (var ae in Program.MainForm.archiveEditors)
                if (ae.archive.ContainsAsset(modelAssetId) && ae.archive.GetFromAssetID(modelAssetId) is AssetMODL model)
                {
                    bsmc = (ae.archive, model);
                    count++;
                    if (count > 1)
                    {
                        MessageBox.Show("Unable to bake rotation: model found in more than one open archive.");
                        return modelAssetId;
                    }
                }

            if (count == 0)
                MessageBox.Show("Unable bake rotation: model not found in open archives.");
            else if (count == 1)
                return bsmc.Item1.ApplyBakeRotationLocal(bsmc.Item2, yaw, pitch, roll);

            return modelAssetId;
        }

        public uint ApplyBakeRotationLocal(AssetMODL model, float yaw, float pitch, float roll)
        {
            var AHDR = model.BuildAHDR(platform.Endianness());
            var newAssetName = AHDR.ADBG.assetName + $"_{MathUtil.RadiansToDegrees(yaw):.0###########}_{MathUtil.RadiansToDegrees(pitch):.0###########}_{MathUtil.RadiansToDegrees(roll):.0###########}";
            var newAssetId = Functions.BKDRHash(newAssetName);

            if (!ContainsAsset(newAssetId))
            {
                AHDR.ADBG.assetName = newAssetName;
                AHDR.assetID = newAssetId;

                var prevLayerType = SelectedLayerIndex;
                if (!NoLayers)
                    SelectedLayerIndex = GetLayerFromAssetID(model.assetID);

                var newModel = (AssetMODL)AddAsset(AHDR, game, platform.Endianness(), setTextureDisplay: false);

                if (!NoLayers)
                    SelectedLayerIndex = prevLayerType;
                newModel.ApplyRotation(yaw, pitch, roll);
                UnsavedChanges = true;
            }

            return newAssetId;
        }

        public static void ExportScene(string folderName, ExportFormatDescription format, string textureExtension, out string[] textureNames)
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
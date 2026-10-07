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
            for (int i = 0; i < multiInternalEditors.Count; i++)
                if (multiInternalEditors[i].AssetIDs.Contains(assetID))
                {
                    multiInternalEditors[i].Close();
                    multiInternalEditors.RemoveAt(i--);
                }
        }

        public void OpenInternalEditor(IEnumerable<uint> assets, bool openAnyway, System.Drawing.Point location, Action<Asset> updateListView)
        {
            bool willOpen = true;
            if (assets.Count() > 15 && !openAnyway)
            {
                willOpen = MessageBox.Show($"Warning: you're going to open {assets.Count()} Asset Data Editor windows. Are you sure you want to do that?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
            }

            if (willOpen)
                foreach (uint u in assets)
                    if (assetDictionary.ContainsKey(u))
                        OpenInternalEditor(assetDictionary[u], assets.Count() == 1 ? location : null, updateListView);
        }

        private void OpenInternalEditor(Asset asset, System.Drawing.Point? location, Action<Asset> updateListView)
        {
            CloseInternalEditor(asset.assetID);

            var editor = CreateInternalEditor(asset, updateListView);
            editor.Activated += (sender, e) =>
            {
                editor.Opacity = 1f;
                foreach (var child in editor.OwnedForms)
                    child.SendToBack();
                editor.BringToFront();
            };
            editor.Deactivate += (sender, e) =>
            {
                if (!editor.OwnedForms.Any() && Program.MainForm.TranslucentWhenOutOfFocus)
                    editor.Opacity = 0.5f;
            };
            internalEditors.Add(editor);
            if (location.HasValue)
            {
                editor.StartPosition = FormStartPosition.Manual;
                editor.Location = new System.Drawing.Point(Math.Max(0, location.Value.X - editor.Size.Width / 2), Math.Max(0, location.Value.Y - editor.Size.Height / 2));
            }
            editor.Show();
        }

        private IInternalEditor CreateInternalEditor(Asset asset, Action<Asset> updateListView)
        {
            switch (asset.assetType)
            {
                case AssetType.Model when ((AssetMODL)asset).IsCollisionModel:
                    internalEditors.Add(new InternalCollModelEditor((AssetRenderWareModel)asset, this, updateListView));
                    break;
                case AssetType.Model:
                case AssetType.BSP:
                case AssetType.JSP:
                    return new InternalModelEditor((AssetRenderWareModel)asset, this, updateListView);
                case AssetType.Flythrough:
                    return new InternalFlyEditor((AssetFLY)asset, this, updateListView);
                case AssetType.Texture:
                case AssetType.TextureStream:
                    if (asset is AssetRWTX rwtx)
                        return new InternalTextureEditor(rwtx, this, updateListView);
                    return new InternalAssetEditor(asset, this, updateListView);
                case AssetType.Sound:
                case AssetType.SoundStream:
                    return new InternalSoundEditor((AssetSound)asset, this, updateListView);
                case AssetType.Text:
                    return new InternalTextEditor((AssetTEXT)asset, this, updateListView);
            }
            return new InternalAssetEditor(asset, this, updateListView);
        }

        private readonly List<InternalMultiAssetEditor> multiInternalEditors = new List<InternalMultiAssetEditor>();

        public void OpenInternalEditorMulti(IEnumerable<uint> list, System.Drawing.Point? location, Action<Asset> updateListView)
        {
            var assets = new List<Asset>();
            foreach (var u in list)
            {
                CloseInternalEditor(u);
                if (assetDictionary.TryGetValue(u, out Asset value))
                    assets.Add(value);
            }
            var editor = new InternalMultiAssetEditor(assets.ToArray(), this, updateListView);
            if (location.HasValue)
            {
                editor.StartPosition = FormStartPosition.Manual;
                editor.Location = new System.Drawing.Point(Math.Max(0, location.Value.X - editor.Size.Width / 2), Math.Max(0, location.Value.Y - editor.Size.Height / 2));
            }
            multiInternalEditors.Add(editor);
            editor.Show();
        }

        public void SetAllTopMost(bool value)
        {
            foreach (var ie in internalEditors)
                ie.TopMost = value;
        }

        public void SetAllOpacity(double value)
        {
            foreach (var ie in internalEditors)
                ie.Opacity = value;
        }

        public static void OpenWikiPage(Asset asset)
        {
            var code = asset.assetType.GetCode();
            if (asset.assetType.IsDyna())
                code += $"/{asset.TypeString}";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = AboutBox.WikiLink + code, UseShellExecute = true });
        }

        public void ClearModelTemplateFocus()
        {
            foreach (var ie in internalEditors)
                if (ie is InternalModelEditor ime)
                    ime.ClearModelTemplateFocus();
        }

        public AssetPIPT GetPIPT(bool create = false) => (AssetPIPT)GetAssetOfType(AssetType.PipeInfoTable, AssetTemplate.Pipe_Info_Table, create);

        public AssetCOLL GetCOLL(bool create = false) => (AssetCOLL)GetAssetOfType(AssetType.CollisionTable, AssetTemplate.Collision_Table, create);

        public AssetLODT GetLODT(bool create = false) => (AssetLODT)GetAssetOfType(AssetType.LevelOfDetailTable, AssetTemplate.Level_Of_Detail_Table, create);

        public AssetSHDW GetSHDW(bool create = false) => (AssetSHDW)GetAssetOfType(AssetType.ShadowTable, AssetTemplate.Shadow_Table, create);

        public AssetJAW GetJAW(bool create = false) => (AssetJAW)GetAssetOfType(AssetType.JawDataTable, AssetTemplate.Jaw_Data_Table, create);

        public Asset GetSNDI(bool create = false) => GetAssetOfType(AssetType.SoundInfo, AssetTemplate.Sound_Info, create);

        private Asset GetAssetOfType(AssetType assetType, AssetTemplate assetTemplate, bool create)
        {
            if (!ContainsAssetWithType(assetType))
            {
                if (!create)
                    return null;

                var prevIndex = SelectedLayerIndex;

                var layerType = LayerType.DEFAULT;
                if (assetTemplate == AssetTemplate.Sound_Info)
                    layerType = LayerType.SNDTOC;

                if (!NoLayers)
                    SelectedLayerIndex = IndexOfLayerOfType(layerType);

                PlaceTemplate(assetTemplate);
                if (!standalone)
                    foreach (var ae in Program.MainForm.archiveEditors)
                        ae.PopulateAssetListAndComboBox();

                if (!NoLayers)
                    SelectedLayerIndex = prevIndex;
            }
            return (from asset in assetDictionary.Values where asset.assetType == assetType select asset).FirstOrDefault();
        }

        public static Vector3 GetRayIntersectionPosition(SharpRenderer renderer, Ray ray, uint assetIdSkip = 0)
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
            foreach (var a in from Asset a in CurrentlySelectedAssets where a is IClickableAsset select (IClickableAsset)a)
            {
                if ((a is AssetTRIG trig && trig.Shape == TriggerShape.Box) || (a is AssetVOLU volu && volu.Shape == VolumeType.Box))
                    continue;

                var position = GetRayIntersectionPosition(renderer,
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

        public static T DeepCopy<T>(T data) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));

        public void MergeSimilar()
        {
            UnsavedChanges = true;

            var actions = new List<IReversibleAction>();

            void MergeTableAssets<T, U>(AssetType assetType) where T : Asset, ITableAsset<T, U>
            {
                var assets = assetDictionary.Values.Where(asset => asset.assetType == assetType).Cast<T>().ToList();
                if (assets.Count == 0)
                    return;
                for (int i = 1; i < assets.Count; i++)
                {
                    actions.Add(GetAssetRemovedAction(assets[i]));
                    RemoveAsset(assets[i].assetID);
                }
                for (int i = 1; i < assets.Count; i++)
                    MergeTableAsset<T, U>(assets[i], actions);
            }

            MergeTableAssets<AssetCOLL, EntryCOLL>(AssetType.CollisionTable);
            MergeTableAssets<AssetLODT, EntryLODT>(AssetType.LevelOfDetailTable);
            MergeTableAssets<AssetPIPT, PipeInfo>(AssetType.PipeInfoTable);
            MergeTableAssets<AssetSHDW, EntrySHDW>(AssetType.ShadowTable);
            MergeTableAssets<AssetJAW, EntryJAW>(AssetType.JawDataTable);

            void MergeSoundInfoAssets<T>() where T : Asset, ISoundInfoAsset<T>
            {
                var assets = assetDictionary.Values.Where(asset => asset.assetType == AssetType.SoundInfo).Cast<T>().ToList();
                if (assets.Count == 0)
                    return;
                for (int i = 1; i < assets.Count; i++)
                {
                    actions.Add(GetAssetRemovedAction(assets[i]));
                    RemoveAsset(assets[i].assetID);
                }
                for (int i = 1; i < assets.Count; i++)
                    MergeSoundInfo<T>(assets[i], actions);
            }

            switch (platform)
            {
                case Platform.GameCube when game < Game.Incredibles:
                    MergeSoundInfoAssets<AssetSNDI_GCN_V1>();
                    break;
                case Platform.GameCube:
                    MergeSoundInfoAssets<AssetSNDI_GCN_V2>();
                    break;
                case Platform.Xbox:
                    MergeSoundInfoAssets<AssetSNDI_XBOX>();
                    break;
                case Platform.PS2:
                    MergeSoundInfoAssets<AssetSNDI_PS2>();
                    break;
            }

            Program.UndoBuffer.AddAction(new MultiAction(actions));
        }

        private void MergeTableAsset<T, U>(T asset, List<IReversibleAction> actions) where T : ITableAsset<T, U>
        {
            ITableAsset<T, U> current = assetDictionary.Values.OfType<ITableAsset<T, U>>().FirstOrDefault();
            var before = DeepCopy(current.Entries);
            current.Merge(asset);
            actions.Add(new AssetPropertyChangedAction(this, (Asset)current, "Entries", before, DeepCopy(current.Entries)));
        }

        private void MergeSoundInfo<T>(T asset, List<IReversibleAction> actions) where T : ISoundInfoAsset<T>
        {
            var SNDI = assetDictionary.Values.FirstOrDefault(a => a.assetType == AssetType.SoundInfo);
            var action1 = GetAssetRemovedAction(SNDI);
            ((ISoundInfoAsset<T>)SNDI).Merge(asset);
            var action2 = GetAssetAddedAction(SNDI);
            actions.Add(new MultiAction([action1, action2]));
        }

        public static AHDRFlags AHDRFlagsFromAssetType(AssetType assetType)
        {
            if (assetType.IsDyna())
                return AHDRFlags.SOURCE_VIRTUAL;
            return assetType switch
            {
                AssetType.AnimationList or AssetType.Boulder or AssetType.Button or AssetType.Camera or AssetType.Counter or AssetType.CollisionTable or
                AssetType.Conditional or AssetType.CutsceneManager or AssetType.CutsceneTableOfContents or AssetType.Dispatcher or AssetType.DiscoFloor or
                AssetType.DestructibleObject or AssetType.ElectricArcGenerator or AssetType.Environment or AssetType.Fog or AssetType.Hangable or
                AssetType.Group or AssetType.JawDataTable or AssetType.LevelOfDetailTable or AssetType.SurfaceMapper or AssetType.ModelInfo or
                AssetType.Marker or AssetType.MovePoint or AssetType.ParticleEmitter or AssetType.ParticleProperties or AssetType.ParticleSystem or
                AssetType.Pendulum or AssetType.PickupTable or AssetType.PipeInfoTable or AssetType.Pickup or AssetType.Platform or AssetType.Player or
                AssetType.Portal or AssetType.SFX or AssetType.ShadowTable or AssetType.Shrapnel or AssetType.SimpleObject or AssetType.SoundInfo or
                AssetType.Surface or AssetType.Text or AssetType.Timer or AssetType.Trigger or AssetType.UserInterface or AssetType.UserInterfaceFont or
                AssetType.NPC or AssetType.NPCProperties => AHDRFlags.SOURCE_VIRTUAL,

                AssetType.Cutscene or AssetType.Flythrough or AssetType.RawImage => AHDRFlags.SOURCE_FILE,

                AssetType.Animation or AssetType.Credits or AssetType.Sound or AssetType.SoundStream => AHDRFlags.SOURCE_FILE | AHDRFlags.WRITE_TRANSFORM,

                AssetType.BSP or AssetType.Model => AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM,

                AssetType.AnimationTable or AssetType.JSP or AssetType.JSPInfo or AssetType.Texture or AssetType.TextureStream => AHDRFlags.SOURCE_VIRTUAL | AHDRFlags.READ_TRANSFORM,

                AssetType.LightKit => AHDRFlags.SOURCE_FILE | AHDRFlags.READ_TRANSFORM | AHDRFlags.WRITE_TRANSFORM,
                _ => 0,
            };
        }

        public bool OrganizeLayers(bool legacy)
        {
            try
            {
                Layers = BuildLayers(legacy);
                UnsavedChanges = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private List<Layer> BuildLayers(bool legacy = false)
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
                        if (game != Game.Scooby && !legacy)
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
                        if (game != Game.Scooby && !legacy)
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
                        if (game >= Game.Incredibles)
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
                    if (legacy)
                    {
                        var assetIDs = new List<uint>(1);
                        foreach (var assetId in jspInfo.JSP_AssetIDs)
                        {
                            assetIDs.Add(assetId);
                            doneJsps.Add(assetId);
                        }
                        list.Add(new Layer(LayerType.BSP) { AssetIDs = assetIDs });
                    }
                    else
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
            ordered = ordered.OrderBy(f => f.assetType.ToString()).ToList();

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

            var assets = assetDictionary.Values.Where(a => assetTypes == null || assetTypes.Contains(a.assetType)).ToArray();

            foreach (Asset a in assets)
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
                            placeable.Model = ApplyBakeScale(placeable.assetName, placeable.Model, factor);
                    }
                    else if (factor.X != factor.Y || factor.X != factor.Z || factor.Y != factor.Z)
                    {
                        if (bakeEntityUnproportionalScales)
                            placeable.Model = ApplyBakeScale(placeable.assetName, placeable.Model, factor);
                    }
                    else
                    {
                        placeable.ScaleX *= factor.X;
                        placeable.ScaleY *= factor.Y;
                        placeable.ScaleZ *= factor.Z;
                    }
                }
                else if (a is DynaEnemy enemysb)
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

        public List<uint> MakeSimps(List<uint> assetIDs, bool solid, bool ledgeGrabSimps, bool placeOnExistingDefaultLayer)
        {
            if (!NoLayers)
            {
                bool defaultLayerExists = false;

                if (placeOnExistingDefaultLayer)
                {
                    // Check every layer to see whether it is of type default
                    for (int i = 0; i < Layers.Count; i++)
                    {
                        if (Layers[i].Type != LayerType.DEFAULT)
                            continue;

                        // If the layer is a default layer, select it.
                        // Pick the first default layer found.
                        defaultLayerExists = true;
                        SelectedLayerIndex = i;
                        break;
                    }
                }

                if (!placeOnExistingDefaultLayer || !defaultLayerExists)
                {
                    AddLayer();
                    SelectedLayerIndex = Layers.Count - 1;
                }
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

                if (!NoLayers)
                    SelectedLayerIndex = IndexOfLayerOfType(LayerType.DEFAULT);

                pipt = (AssetPIPT)PlaceTemplate(template: AssetTemplate.Pipe_Info_Table);

                if (!NoLayers)
                    SelectedLayerIndex = prevLayerType;
            }

            List<PipeInfo> entries = pipt.Entries.ToList();

            foreach (uint u in assetIDs)
                if (GetFromAssetID(u) is AssetMODL)
                    entries.Add(new PipeInfo(game)
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

        public static uint ApplyBakeScale(string assetName, uint modelAssetId, Vector3 scale)
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
                if (ae.archive.TryGetAsset(modelAssetId, out Asset asset) && asset is IAssetWithModel model)
                {
                    bsmc = (ae.archive, model);
                    count++;
                    if (count > 1)
                    {
                        MessageBox.Show($"Unable to bake scale for asset {assetName}: model 0x{modelAssetId:X8} found in more than one open archive.");
                        return modelAssetId;
                    }
                }

            if (count == 0)
                MessageBox.Show($"Unable to bake scale for asset {assetName}: model 0x{modelAssetId:X8} not found in open archives.");
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
                if (ae.archive.TryGetAsset(modelAssetId, out Asset asset) && asset is AssetMODL model)
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
                                else
                                    continue;
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
                        else if (v is DynaEnemy enemySb)
                        {
                            if (enemySb.isInvisible || enemySb.DontRender)
                                continue;

                            modelAsset = (Asset)renderingDictionary[enemySb.Model];
                            world = enemySb.world;
                            assetName = enemySb.assetName;
                        }
                        else
                            continue;

                        if (modelAsset is AssetMINF minf)
                            modelAsset = (Asset)renderingDictionary[minf.References[0].Model];

                        if (modelAsset is AssetMODL modl)
                            textureNamesList.AddRange(modl.Textures);
                        else
                            continue;

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

        public List<uint> ConvertScriptToGroupOfTimers(AssetSCRP script)
        {
            var previouslySelectedLayer = SelectedLayerIndex;
            SelectedLayerIndex = GetLayerFromAssetID(script.assetID);
            var timerAssetIDs = new List<uint>();
            var assets = new List<Asset>();
            var num = 1;
            foreach (Link link in script.TimedLinks)
            {
                var timer = (AssetTIMR)PlaceTemplate(customName: $"{script.assetName}_TIMER_{num:D2}", template: AssetTemplate.Timer, ignoreNumber: true);
                timer.Time = link.Time;
                timer.Links = new Link[] {
                    new Link(timer.game)
                    {
                        EventReceiveID = 0x0014,
                        EventSendID = link.EventSendID,
                        TargetAsset = link.TargetAsset,
                        ArgumentAsset = link.ArgumentAsset,
                        Parameter1 = link.Parameter1,
                        Parameter2 = link.Parameter2,
                        Parameter3 = link.Parameter3,
                        Parameter4 = link.Parameter4,
                    }
                };
                assets.Add(timer);
                timerAssetIDs.Add(timer.assetID);
                num++;
            }
            var group = (AssetGRUP)PlaceTemplate(customName: $"{script.assetName}_GROUP", template: AssetTemplate.Group, ignoreNumber: true);
            group.Items = timerAssetIDs.Select(id => new AssetID(id)).ToArray();
            group.Links = script.Links;
            SelectedLayerIndex = previouslySelectedLayer;
            timerAssetIDs.Insert(0, group.assetID);
            return timerAssetIDs;
        }
    }
}
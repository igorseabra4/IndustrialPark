using Assimp;
using DiscordRPC;
using HipHopFile;
using IndustrialPark.Archive;
using IndustrialPark.Models;
using RenderWareFile;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IndustrialPark;

public class AssetRenderer
{
    private Dictionary<uint, IAssetWithModel> renderingDictionary;
    private HashSet<IRenderableAsset> renderableAssets;
    private List<AssetJSP> renderableJSPs;
    private HashSet<AssetLKIT> renderableLightKits;
    private HashSet<AssetFOG> renderableFOGs;
    private Dictionary<uint, xJSPNodeInfo[]> jspInfoNodeInfo;

    public AssetRenderer()
    {
        renderingDictionary = new Dictionary<uint, IAssetWithModel>();
        renderableAssets = new HashSet<IRenderableAsset>();
        renderableJSPs = new List<AssetJSP>();
        renderableLightKits = new HashSet<AssetLKIT>();
        renderableFOGs = new HashSet<AssetFOG>();
        jspInfoNodeInfo = new Dictionary<uint, xJSPNodeInfo[]>();
    }

    public void AddToRenderableAssets(IRenderableAsset ira)
    {
        if (ira is AssetJSP jsp)
            lock (renderableJSPs)
            {
                renderableJSPs.Remove(jsp);
                renderableJSPs.Insert(0, jsp);
            }
        else
            lock (renderableAssets)
            {
                renderableAssets.Remove(ira);
                renderableAssets.Add(ira);
            }
    }

    public void RemoveFromRenderableAssets(IRenderableAsset ra)
    {
        lock (renderableAssets)
            renderableAssets.Remove(ra);
        if (ra is AssetJSP jsp)
            lock (renderableJSPs)
                renderableJSPs.Remove(jsp);
        lock (Program.Renderer.renderableAssets)
            Program.Renderer.renderableAssets.Remove(ra);
    }

    public IEnumerable<IRenderableAsset> GetRenderableAssets() => renderableAssets;
    public IEnumerable<IRenderableAsset> GetRenderableJSPs() => renderableJSPs;
    
    public bool PauseMatrixRecalculation = false;

    public void ResetModels(SharpRenderer renderer)
    {
        foreach (IAssetWithModel a in renderingDictionary.Values)
            if (a is AssetRenderWareModel model)
                model.SetupModel(renderer);
    }

    public void RecalculateAllMatrices(SharpRenderer renderer)
    {
        if (PauseMatrixRecalculation)
            return;

        lock (renderableAssets)
            foreach (var ra in renderableAssets)
                ra.CreateTransformMatrix(renderer);
        lock (renderableJSPs)
            foreach (var jsp in renderableJSPs)
                jsp.CreateTransformMatrix(renderer);
    }

    public void AddToRenderingDictionary(uint assetID, IAssetWithModel value)
    {
        lock (renderingDictionary)
            renderingDictionary[assetID] = value;
    }

    public void RemoveFromRenderingDictionary(IAssetWithModel iawm)
    {
        if (renderingDictionary.ContainsValue(iawm))
            lock (renderingDictionary)
                renderingDictionary.Remove(iawm.assetID);
    }

    public IAssetWithModel GetFromRenderingDictionary(uint assetID)
    {
        lock (renderingDictionary)
            return renderingDictionary.TryGetValue(assetID, out IAssetWithModel value) ? value : null;
    }

    public RenderWareModelFile GetModelFromRenderingDictionary(uint assetID)
    {
        lock (renderingDictionary)
            return renderingDictionary.TryGetValue(assetID, out IAssetWithModel value) ? value.GetRenderWareModelFile(this) : null;
    }

    public void AddToRenderableLightKits(AssetLKIT lkit)
    {
        lock (renderableLightKits)
        {
            foreach (var light in lkit.Lights)
            {
                if (light.ColorRed > 1f || light.ColorGreen > 1f || light.ColorBlue > 1f)
                {
                    float s = Math.Max(light.ColorRed, Math.Max(light.ColorGreen, light.ColorBlue));
                    s = Math.Max(s, 0.00001f);
                    s = 1f / s;

                    light.ColorRed *= s;
                    light.ColorGreen *= s;
                    light.ColorBlue *= s;
                }
            }
            renderableLightKits.Add(lkit);
            AssetLKIT.SceneLightKit = lkit;
        }
    }

    public void RemoveFromRenderableLightKits(AssetLKIT lkit)
    {
        lock (renderableLightKits)
        {
            renderableLightKits.Remove(lkit);
            AssetLKIT.SceneLightKit = renderableLightKits.LastOrDefault();
        }
    }

    public void AddToRenderableFOGs(AssetFOG fog)
    {
        lock (renderableFOGs)
        {
            renderableFOGs.Add(fog);
            Program.Renderer.Fog = fog;
        }
    }

    public void RemoveFromRenderableFOGs(AssetFOG fog)
    {
        lock (renderableFOGs)
        {
            renderableFOGs.Remove(fog);
            Program.Renderer.Fog = renderableFOGs.LastOrDefault();
        }
    }

    public void AddToJspNodeInfos(uint id, xJSPNodeInfo[] info)
    {
        lock (jspInfoNodeInfo)
            jspInfoNodeInfo[id] = info;
    }

    public xJSPNodeInfo[] GetFromJspNodeInfos(uint id)
    {
        lock (jspInfoNodeInfo)
            return jspInfoNodeInfo.TryGetValue(id, out xJSPNodeInfo[] info) ? info : null;
    }

    private void PipeInfoUpdate(AssetPIPT PIPT, bool isDisposing)
    {
        if (isDisposing)
            ResetModelBlendModes(PIPT.Entries.Select(e => (uint)e.Model));
        else
            UpdateModelBlendModes(PIPT.GetEntryDictionary());
    }

    private void UpdateModelBlendModes(Dictionary<uint, List<PipeInfo>> blendModes)
    {
        foreach (var k in blendModes.Keys)
            if (renderingDictionary.TryGetValue(k, out IAssetWithModel value) && value is AssetMODL MODL)
                MODL.SetPipeline(blendModes[k]);
    }

    private void ResetModelBlendModes(IEnumerable<uint> assetIDs)
    {
        var models = assetIDs.Where(renderingDictionary.ContainsKey).Select(m => renderingDictionary[m]).OfType<AssetMODL>();
        foreach (var MODL in models)
            MODL.ResetPipeline();
    }

    private void LightKitUpdate(Archive.Archive archive, uint lkitID)
    {
        AssetLKIT.SceneLightKit = null;
        if (archive.TryGetAsset(lkitID, out Asset asset) && asset is AssetLKIT lkit)
            AddToRenderableLightKits(lkit);
    }

    public void RegisterSubscriptions(Archive.Archive archive)
    {
        archive.Subscriber.SubscribeToAssetAdded((asset) =>
        {
            switch (asset)
            {
                case IAssetWithModel iawm:
                    AddToRenderingDictionary(asset.assetID, iawm);

                    if (asset.game >= Game.Incredibles)
                        AddToRenderingDictionary(new AssetID(iawm.fileNameWithoutExtension), iawm);

                    // recalculate matrices of assets which use me
                    if (!PauseMatrixRecalculation)
                        foreach (var user in AssetService.WhoTargets(asset.assetID).OfType<IRenderableAsset>())
                            user.CreateTransformMatrix(Program.Renderer);
                    break;
                case IRenderableAsset ira:
                    if (!PauseMatrixRecalculation)
                        ira.CreateTransformMatrix(Program.Renderer);
                    AddToRenderableAssets(ira);
                    break;
                case AssetPIPT PIPT:
                    PIPT.PipeInfoUpdate += isDisposing => PipeInfoUpdate(PIPT, isDisposing);
                    PipeInfoUpdate(PIPT, false);
                    break;
                case AssetENV ENV:
                    ENV.UpdateLightKit += lkitID => LightKitUpdate(archive, lkitID);
                    LightKitUpdate(archive, ENV.Object_LightKit);
                    break;
                case AssetFOG FOG:
                    if (FOG.Links.Any(l => (EventBFBB)l.EventSendID == EventBFBB.On))
                        AddToRenderableFOGs(FOG);
                    break;
                case AssetLKIT LKIT:
                    AddToRenderableLightKits(LKIT);
                    break;
            }
        });

        archive.Subscriber.SubscribeToAssetRemoved((asset) =>
        {
            switch (asset)
            {
                case IRenderableAsset ra:
                    RemoveFromRenderableAssets(ra);
                    break;
                case IAssetWithModel iawm:
                    RemoveFromRenderingDictionary(iawm);
                    break;
                case AssetJSP_INFO jspinfo:
                    jspInfoNodeInfo.Remove(jspinfo.assetID);
                    break;
                case AssetFOG fog:
                    RemoveFromRenderableFOGs(fog);
                    break;
                case AssetLKIT lkit:
                    RemoveFromRenderableLightKits(lkit);
                    break;
            }
        });
    }

    public Vector3 GetRayIntersectionPosition(SharpRenderer renderer, SharpDX.Ray ray, uint assetIdSkip = 0)
    {
        List<IRenderableAsset> l = new List<IRenderableAsset>();

        lock (renderableAssets)
            l.AddRange(renderableAssets);
        lock (renderableJSPs)
            l.AddRange(renderableJSPs);

        float? smallestDistance = null;

        foreach (IRenderableAsset ra in l)
        {
            if (((Asset)ra).assetID == assetIdSkip)
                continue;

            float? distance = ra.GetIntersectionPosition(renderer, ray);
            if (distance != null && (smallestDistance == null || distance < smallestDistance))
                smallestDistance = distance;
        }

        return ray.Position + Vector3.Normalize(ray.Direction) * (smallestDistance ?? 3f);
    }

    public IClickableAsset GetClickedAsset(SharpRenderer renderer, SharpDX.Ray ray, bool? selectionFilter)
    {
        var assets = renderableAssets.OfType<IClickableAsset>();
        if (renderer.isDrawingUI)
            assets = assets.Where(a => ((Asset)a).assetType is AssetType.UserInterface or AssetType.UserInterfaceFont);
        if (selectionFilter.HasValue)
            assets = assets.Where(a => selectionFilter.Value == a.isSelected);

        float smallestDistance = renderer.Camera.FarPlane + 1f;
        IClickableAsset resultAsset = null;
        foreach (var asset in assets.ToList())
            if (asset.GetIntersectionPosition(renderer, ray) is float distance && distance < smallestDistance)
            {
                smallestDistance = distance;
                resultAsset = asset;
            }
        return resultAsset;
    }

    public void DropSelectedAssets(SharpRenderer renderer, List<Asset> assets)
    {
        var beganTransaction = new HashSet<ArchiveEditor>();

        foreach (var asset in assets.OfType<IClickableAsset>())
        {
            if (asset is AssetTRIG || asset is AssetVOLU)
                continue;

            var position = GetRayIntersectionPosition(renderer,
                new SharpDX.Ray(new Vector3(asset.PositionX, asset.PositionY, asset.PositionZ), new Vector3(0f, -1f, 0f)),
                ((Asset)asset).assetID);

            asset.PositionY = position.Y;

            var assetasset = (Asset)asset;
            var ae = Program.ArchiveManager.ArchiveEditorContainingAsset(assetasset);
            if (beganTransaction.Add(ae))
                ae.BeginTransaction();
            assetasset.InvokePropertyChanged(true);
        }

        foreach (var ae in beganTransaction)
            ae.EndTransaction();
    }

    public void ExportScene(string folderName, ExportFormatDescription format, string textureExtension, out string[] textureNames)
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

    internal IEnumerable<IRenderableAsset> GetSelectedJSPs()
    {
        lock (renderableJSPs)
            foreach (var jsp in renderableJSPs)
                if (jsp.isSelected)
                   yield return jsp;
    }
}
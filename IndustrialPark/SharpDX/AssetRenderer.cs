using Assimp;
using HipHopFile;
using IndustrialPark.Archives;
using IndustrialPark.Assets;
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

    public void AddToRenderableAssets(IRenderableAsset renderable)
    {
        if (!PauseMatrixRecalculation)
            renderable.CalculateTransformation(Program.Renderer);

        if (renderable is AssetJSP jsp)
            lock (renderableJSPs)
            {
                renderableJSPs.Remove(jsp);
                renderableJSPs.Insert(0, jsp);
            }
        else
            lock (renderableAssets)
            {
                renderableAssets.Remove(renderable);
                renderableAssets.Add(renderable);
            }
    }

    public void RemoveFromRenderableAssets(IRenderableAsset ra)
    {
        lock (renderableAssets)
            renderableAssets.Remove(ra);
        if (ra is AssetJSP jsp)
            lock (renderableJSPs)
                renderableJSPs.Remove(jsp);
    }

    public List<IRenderableAsset> GetRenderableAssets()
    {
        lock (renderableAssets)
            return renderableAssets.ToList();
    }

    public List<AssetJSP> GetRenderableJSPs()
    {
        lock (renderableJSPs)
            return renderableJSPs.ToList();
    }

    public bool PauseMatrixRecalculation = false;

    public void RecalculateAllMatrices(SharpRenderer renderer)
    {
        if (PauseMatrixRecalculation)
            return;

        foreach (var ra in GetRenderableAssets().Concat(GetRenderableJSPs()))
            ra.CalculateTransformation(renderer);
    }

    public void AddToRenderingDictionary(IAssetWithModel withModel)
    {
        lock (renderingDictionary)
        {
            renderingDictionary[withModel.assetID] = withModel;
            if (((Asset)withModel).game >= Game.Incredibles)
                renderingDictionary[new AssetID(withModel.fileNameWithoutExtension)] = withModel;

            // recalculate matrices of assets which use me
            if (!PauseMatrixRecalculation)
                foreach (var user in AssetService.FindWhoTargets(withModel.assetID).OfType<IRenderableAsset>())
                    user.CalculateTransformation(Program.Renderer);
        }
    }

    public void RemoveFromRenderingDictionary(IAssetWithModel withModel)
    {
        lock (renderingDictionary)
        {
            renderingDictionary.Remove(withModel.assetID);
            renderingDictionary.Remove(new AssetID(withModel.fileNameWithoutExtension));
        }
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

                // warning: I shouldn't be modifying the asset
            }
        }
        lock (renderableLightKits)
            renderableLightKits.Add(lkit);
        AssetLKIT.SceneLightKit = lkit;
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
            renderableFOGs.Add(fog);
        Program.Renderer.Fog = fog;
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
            if (GetFromRenderingDictionary(k) is AssetMODL MODL)
                MODL.SetPipeline(blendModes[k]);
    }

    private void ResetModelBlendModes(IEnumerable<uint> assetIDs)
    {
        var models = assetIDs.Select(GetFromRenderingDictionary).OfType<AssetMODL>().ToList();
        foreach (var MODL in models)
            MODL.ResetPipeline();
    }

    private void LightKitUpdate(Archive archive, uint lkitID)
    {
        AssetLKIT.SceneLightKit = null;
        if (archive.TryGetAsset(lkitID, out AssetLKIT lkit))
            AddToRenderableLightKits(lkit);
    }

    public void SetupAsset(ISetupableAsset setupable)
    {
        try
        {
            setupable.Setup(Program.Renderer);
            if (setupable is IRenderableAsset renderable)
                AddToRenderableAssets(renderable);
            if (setupable is IAssetWithModel withModel)
                AddToRenderingDictionary(withModel);
        }
        catch
        {
            if (setupable is IRenderableAsset renderable)
                RemoveFromRenderableAssets(renderable);
            if (setupable is IAssetWithModel withModel)
                RemoveFromRenderingDictionary(withModel);
        }
    }

    public void RegisterSubscriptions(Archive archive)
    {
        archive.Subscriber.SubscribeToAssetAdded((asset) =>
        {
            if (asset is IRenderableAsset renderable0)
                asset.PropertyChanged += _ => renderable0.CalculateTransformation(Program.Renderer);

            switch (asset)
            {
                case ISetupableAsset setupable: // BSP, JSP, MODL, SPLN, WIRE
                    asset.PropertyChanged += _ => SetupAsset(setupable);
                    SetupAsset(setupable);
                    break;
                case IAssetWithModel withModel: // MINF
                    AddToRenderingDictionary(withModel);
                    break;
                case IRenderableAsset renderable1:
                    AddToRenderableAssets(renderable1);
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
                    if (FOG.Links.Any(l => l.EventSendBFBB == EventBFBB.On))
                        AddToRenderableFOGs(FOG);
                    break;
                case AssetLKIT LKIT:
                    AddToRenderableLightKits(LKIT);
                    break;
            }
        });

        archive.Subscriber.SubscribeToAssetRemoved((asset, isDisposing) =>
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
        var assets = GetRenderableAssets().Concat(GetRenderableJSPs()).ToList();

        float? smallestDistance = null;

        foreach (IRenderableAsset ra in assets)
        {
            if (ra.assetID == assetIdSkip)
                continue;

            try
            {
                float? distance = ra.GetIntersectionPosition(renderer, ray);
                if (distance != null && (smallestDistance == null || distance < smallestDistance))
                    smallestDistance = distance;
            }
            catch { }
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
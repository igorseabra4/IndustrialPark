using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public static class LayerService
{
    public static Layer LHDRToLayer(Section_LHDR LHDR, Game game, string layerName)
    {
        var layer = new Layer(LayerTypeSpecificToGeneric(LHDR.layerType, game), LHDR.assetIDlist.Count, layerName);
        foreach (var u in LHDR.assetIDlist)
            layer.AssetIDs.Add(u);
        return layer;
    }

    public static int LayerTypeGenericToSpecific(LayerType layerType, Game game)
    {
        if (game >= Game.Incredibles || layerType < LayerType.BSP)
            return (int)layerType;
        return (int)layerType - 1;
    }

    public static LayerType LayerTypeSpecificToGeneric(int layerType, Game game)
    {
        if (game >= Game.Incredibles || layerType < 2)
            return (LayerType)layerType;
        return (LayerType)(layerType + 1);
    }

    public static List<Layer> BuildLayers(IEnumerable<Asset> assets, Game game, bool singleLayerOfEachType = false)
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

        foreach (Asset a in assets)
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
                        if (game != Game.Scooby && !singleLayerOfEachType)
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
                        if (game == Game.Scooby || !assets.Any(a => a.assetType == AssetType.JSPInfo))
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
                        if (game != Game.Scooby && !singleLayerOfEachType)
                            modelIndex = (modelIndex + 1) % 3;
                        break;
                    }
                case AssetType.Animation when game == Game.BFBB:
                    {
                        animationLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                case AssetType.Animation:
                    {
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
                case AssetType.CutsceneTableOfContents when game >= Game.Incredibles:
                    {
                        cutscenetocLayer.AssetIDs.Add(a.assetID);
                        break;
                    }
                case AssetType.CutsceneTableOfContents:
                    {
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
            if (assets.FirstOrDefault(a => a.assetID == l.AssetIDs[0]) is AssetJSP_INFO jspInfo)
            {
                if (singleLayerOfEachType)
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

        var unusedJsps = assets.Where(a => a.assetType is (AssetType.JSP or AssetType.BSP) && !doneJsps.Contains(a.assetID));
        if (unusedJsps.Any())
        {
            var message = "Unable to create a layer setup for your archive. The following BSP/JSP assets are not referenced in a JSPINFO asset:\n" +
                string.Join("\n", unusedJsps.Select(a => $"[{a.assetID:X8}] {a.assetName}"));
            throw new Exception(message);
        }

        return list;
    }
}

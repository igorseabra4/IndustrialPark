using HipHopFile;
using IndustrialPark.Models.CollisionTree;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndustrialPark.Archive;

public static class JSPService
{
    public static void GenerateJSPInfo(Game game, Platform platform, AssetJSP_INFO jspInfo, IEnumerable<AssetJSP> jsps)
    {
        if (!jsps?.Any() ?? true)
            return;

        Clump_0010[] clumps = [.. jsps.Select(i => i.GetClump())];
        HI_Tags_BEEF03 beef03 = new HI_Tags_BEEF03(clumps);

        jspInfo.renderWareVersion = 0x0014FFFF;
        jspInfo.Section1 = new Collis_JSPINFO().HIJSPCollisionBuild(clumps);
        jspInfo.Section1.SetGame(game);

        jspInfo.Section2 = new HI_Tags_BEEF02(game, platform);
        jspInfo.Section2.jspNodeList = Enumerable.Range(0, clumps.Sum(c => c.atomicList.Length)).Select(_ => new xJSPNodeInfo() { nodeFlags = 1, originalMatIndex = -1 }).ToArray();
        if (jspInfo.Section2.Version == 5)
        {
            jspInfo.Section2.xJSPNodeTreeBuild(clumps);
            jspInfo.Section2.stripVecList = beef03.vertexList;
        }

        jspInfo.Section3 = (game == Game.BFBB && platform == Platform.GameCube ? beef03 : null);
        jspInfo.Section4 = (game >= Game.Incredibles ? new HI_Tags_BEEF04(platform) : null);
    }

    public static AssetID[] GetDefaultJspAssetIds(Section_DICT DICT, int jspInfoLayerIndex, Game game)
    {
        var result = new List<AssetID>();
        for (int j = jspInfoLayerIndex - 3; j < jspInfoLayerIndex; j++)
            if (j >= 0 && j < DICT.LTOC.LHDRList.Count)
            {
                var layerType = LayerService.LayerTypeSpecificToGeneric(DICT.LTOC.LHDRList[j].layerType, game);
                if (layerType == LayerType.BSP)
                {
                    foreach (var u in DICT.LTOC.LHDRList[j].assetIDlist)
                        foreach (var asset in DICT.ATOC.AHDRList.Where(a => a.assetID == u))
                            if (asset.assetType == AssetType.JSP)
                                result.Add(u);
                }
                else
                    result.Clear();
            }
        return [.. result];
    }

    /// <summary>
    /// Converts a BFBB JSPInfo to the newer version 5 (TSSM and beyond) format.
    /// </summary>
    /// <param name="jspInfo"></param>
    /// <param name="GetAsset"></param>
    /// <param name="platform"></param>
    public static void ConvertJSPINFOToNewerFormat(AssetJSP_INFO jspInfo, Func<uint, Asset> GetAsset, Platform platform)
    {
        if (jspInfo.Section2?.Version.Equals(5) ?? true)
            return;

        Clump_0010[] clumps = jspInfo.JSP_AssetIDs.Select(i => ((AssetJSP)GetAsset(i)).GetClump()).ToArray();

        int clumpVertOffset = 0;
        int atomicOffset = clumps.Sum(c => c.atomicList.Length) - 1;
        foreach (var clump in clumps.Reverse())
        {
            Geometry_000F[] geometries = clump.geometryList.geometryList.ToArray();
            for (int geo = geometries.Length - 1; geo >= 0; geo--)
            {
                BinMeshPLG_050E binMeshPLG = geometries[geo].geometryExtension.extensionSectionList.OfType<BinMeshPLG_050E>().FirstOrDefault();
                if (binMeshPLG == null)
                    throw new NullReferenceException("BinMesh PLG not present");

                for (int i = 0; i < jspInfo.Section1.triangles.Length; i++)
                {
                    ref xClumpCollBSPTriangle tri = ref jspInfo.Section1.triangles[i];

                    if ((tri.atomIndex & 0x8000) != 0)
                        continue;

                    if (tri.atomIndex == atomicOffset)
                    {
                        tri.rawIdx = clumpVertOffset + tri.meshVertIndex;
                        tri.atomIndex = 0x8000; // Mark triangle as processed
                    }
                }
                clumpVertOffset += binMeshPLG.totalIndexCount;
                atomicOffset--;
            }
        }

        jspInfo.Section2.Version = 5;
        jspInfo.Section2.xJSPNodeTreeBuild(clumps);
        jspInfo.Section2.stripVecList = jspInfo.Section3.vertexList;

        jspInfo.Section3 = null;
        jspInfo.Section4 = new HI_Tags_BEEF04(platform);
    }

    /// <summary>
    /// Converts a new version 5 JSPInfo (TSSM and beyond) to the legacy BFBB format.
    /// </summary>
    /// <param name="jspInfo"></param>
    /// <param name="GetFromAssetID"></param>
    public static void ConvertJSPINFOToLegacyFormat(AssetJSP_INFO jspInfo, Func<uint, Asset> GetFromAssetID)
    {
        if (jspInfo.Section2?.Version.Equals(3) ?? true)
            return;

        jspInfo.Section1?.RemoveTSSMFlags();
        Clump_0010[] clumps = jspInfo.JSP_AssetIDs.Select(i => ((AssetJSP)GetFromAssetID(i)).GetClump()).ToArray();

        int clumpVertOffset = 0;
        foreach (var clump in clumps.Reverse())
        {
            Geometry_000F[] geometries = clump.geometryList.geometryList.ToArray();
            for (int geo = geometries.Length - 1; geo >= 0; geo--)
            {
                BinMeshPLG_050E binMeshPLG = geometries[geo].geometryExtension.extensionSectionList.OfType<BinMeshPLG_050E>().FirstOrDefault();
                if (binMeshPLG == null)
                    throw new NullReferenceException("BinMesh PLG not present");

                for (int i = 0; i < jspInfo.Section1?.triangles.Length; i++)
                {
                    ref xClumpCollBSPTriangle tri = ref jspInfo.Section1.triangles[i];

                    if ((tri.atomIndex & 0x8000) != 0)
                        continue;

                    if (tri.meshVertIndex >= clumpVertOffset && tri.meshVertIndex < (binMeshPLG.totalIndexCount + clumpVertOffset))
                    {
                        tri.atomIndex = (ushort)(geo | 0x8000); // Mark triangle as processed, remove later
                        tri.meshVertIndex -= (ushort)clumpVertOffset;
                    }
                }
                clumpVertOffset += binMeshPLG.totalIndexCount;
            }
        }

        // Reset processed flag
        for (int i = 0; i < jspInfo.Section1.triangles.Length; i++)
        {
            ref xClumpCollBSPTriangle tri = ref jspInfo.Section1.triangles[i];
            unchecked { tri.atomIndex &= (ushort)~0x8000; }
        }

        jspInfo.Section2.Version = 3;
        jspInfo.Section3 = new HI_Tags_BEEF03();
        jspInfo.Section3.vertexList = jspInfo.Section2.stripVecList;
        jspInfo.Section4 = null;
    }
}

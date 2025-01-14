using HipHopFile;
using IndustrialPark.Models.CollisionTree;
using Octokit;
using RenderWareFile;
using RenderWareFile.Sections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace IndustrialPark
{
    public partial class ArchiveEditorFunctions
    {
        public static List<AssetJSP> renderableJSPs = new List<AssetJSP>();
        public static void AddToRenderableJSPs(AssetJSP jsp)
        {
            lock (renderableJSPs)
            {
                renderableJSPs.Remove(jsp);
                renderableJSPs.Insert(0, jsp);
            }
        }

        public static Dictionary<uint, xJSPNodeInfo[]> jspInfoNodeInfo = new Dictionary<uint, xJSPNodeInfo[]>();
        public static void AddToJspNodeInfo(uint id, xJSPNodeInfo[] info)
        {
            lock (jspInfoNodeInfo)
                jspInfoNodeInfo[id] = info;
        }

        public AssetID CreateJSPInfoAndBSPLayers(List<Section_AHDR> AHDRs, bool overwrite)
        {
            foreach (var ahdr in AHDRs)
                if (overwrite && ContainsAsset(ahdr.assetID))
                    RemoveAsset(ahdr.assetID);
            UnsavedChanges = true;
            
            int lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.JSPINFO);
            if (lastJSPINFOIndex == -1)
                lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.TEXTURE_STRM);
            if (lastJSPINFOIndex == -1)
                lastJSPINFOIndex = Layers.FindLastIndex(l => l.Type == LayerType.TEXTURE);

            int newIndex = lastJSPINFOIndex != -1 ? lastJSPINFOIndex : Layers.Count - 1;

            AddLayer(LayerType.BSP, ++newIndex);
            AddAsset(AHDRs[0], game, platform.Endianness(), true, newIndex);

            AddLayer(LayerType.BSP, ++newIndex);
            if (AHDRs.Count > 1)
                AddAsset(AHDRs[1], game, platform.Endianness(), true, newIndex);

            AddLayer(LayerType.BSP, ++newIndex);
            if (AHDRs.Count > 2)
                AddAsset(AHDRs[2], game, platform.Endianness(), true, newIndex);

            AddLayer(LayerType.JSPINFO, ++newIndex);

            AssetJSP_INFO info = new AssetJSP_INFO(AHDRs[0].ADBG.assetName[..^1], game, platform);
            info.JSP_AssetIDs = AHDRs.Select(a => (AssetID)a.assetID).ToArray();
            GenerateJSPInfo(info);

            if (overwrite && ContainsAsset(info.assetID))
                RemoveAsset(info.assetID);

            AddAsset(info, true);

            return info.assetID;
        }

        public void GenerateJSPInfo(AssetJSP_INFO jspInfo)
        {
            if (jspInfo.JSP_AssetIDs?.Length.Equals(0) ?? true)
                return;

            Clump_0010[] clumps = jspInfo.JSP_AssetIDs.Select(i => ((AssetJSP)GetFromAssetID(i)).GetClump()).ToArray();
            HI_Tags_BEEF03 beef03 = new HI_Tags_BEEF03(clumps);

            jspInfo.renderWareVersion = 0x0014FFFF;
            jspInfo.Section1 = new Collis_JSPINFO().HIJSPCollisionBuild(clumps);
            jspInfo.Section1.SetGame(game);

            jspInfo.Section2 = new HI_Tags_BEEF02(game, platform);
            jspInfo.Section2.jspNodeList = Enumerable.Range(0, clumps.Sum(c => c.atomicList.Count)).Select(_ => new xJSPNodeInfo() { nodeFlags = 1, originalMatIndex = -1 }).ToArray();
            if (jspInfo.Section2.Version == 5)
            {
                jspInfo.Section2.xJSPNodeTreeBuild(clumps);
                jspInfo.Section2.stripVecList = beef03.vertexList;
            }

            jspInfo.Section3 = (game == Game.BFBB && platform == Platform.GameCube ? beef03 : null);
            jspInfo.Section4 = (game >= Game.Incredibles ? new HI_Tags_BEEF04(platform) : null);
        }

        /// <summary>
        /// Converts a BFBB JSPInfo to the newer version 5 (TSSM and beyond) format.
        /// </summary>
        /// <param name="jspInfo"></param>
        public void ConvertJSPINFOToNewerFormat(AssetJSP_INFO jspInfo)
        {
            if (jspInfo.Section2?.Version.Equals(5) ?? true)
                return;

            Clump_0010[] clumps = jspInfo.JSP_AssetIDs.Select(i => ((AssetJSP)GetFromAssetID(i)).GetClump()).ToArray();

            int clumpVertOffset = 0;
            int atomicOffset = clumps.Sum(c => c.atomicList.Count) - 1;
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
        public void ConvertJSPINFOToLegacyFormat(AssetJSP_INFO jspInfo)
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
}

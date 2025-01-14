using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IndustrialPark.Models.CollisionTree
{
    using Triangle = RenderWareFile.Triangle;

    public sealed class Collis_31 : Collis_Shared
    {
        private RpCollBSPBranchNode[] branchNodes { get; set; }
        private RpCollBSPLeafNode[] leafNodes { get; set; }
        private ushort[] triangleMap { get; set; }

        public Collis_31() : base() { }

        public CollisionPLG_011D_Pre36001 RpCollisionGeometryBuildData(Geometry_000F geometry)
        {
            BuildData data = new BuildData(geometry.geometryStruct.morphTargets[0].vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(), 
                geometry.geometryStruct.triangles);
            BuildSector tree = BuildTreeGenerate(data);

            leafNodes = new RpCollBSPLeafNode[BuildTreeCountLeafNodes(tree)];
            branchNodes = new RpCollBSPBranchNode[leafNodes.Length - 1];

            int iBranch = 0;
            int iLeaf = 0;
            int numTotalPolygons = 0;

            ConvertNode(tree, ref iBranch, ref iLeaf, ref numTotalPolygons);

            Assert(iLeaf == leafNodes.Length);
            Assert(iBranch == branchNodes.Length);
            Assert(numTotalPolygons == data.numPolygons);

            triangleMap = new ushort[geometry.geometryStruct.triangles.Length];
            for (int iTri = 0; iTri < geometry.geometryStruct.triangles.Length; iTri++)
                triangleMap[iTri] = (ushort)Array.IndexOf(geometry.geometryStruct.triangles, data.polygons[iTri].poly);

            return new CollisionPLG_011D_Pre36001()
            {
                branchNodes = branchNodes,
                leafNodes = leafNodes,
                triangleMap = triangleMap.Select(t => (int)t).ToArray()
            };
        }

        private ushort ConvertNode(BuildSector sector, ref int iBranch, ref int iLeaf, ref int numTotalPolygons)
        {
            if (sector.type < 0)
            {
                BuildPolySector polySector = (BuildPolySector)sector;
                ref RpCollBSPLeafNode node = ref leafNodes[iLeaf];

                node.numPolygons = polySector.numPolygons;
                node.firstPolygon = polySector.iFirstPolygon;

                numTotalPolygons += node.numPolygons;

                return (ushort)iLeaf++;
            }
            else
            {
                BuildPlaneSector planeSector = (BuildPlaneSector)sector;
                ref RpCollBSPBranchNode branch = ref branchNodes[iBranch];

                branch.type = (SectorAxis)planeSector.type;
                branch.leftValue = planeSector.leftValue;
                branch.rightValue = planeSector.rightValue;
                branch.leftType = (planeSector.leftSubTree.type < 0) ? SectorType.rpCOLLBSP_LEAF_NODE : SectorType.rpCOLLBSP_BRANCH_NODE;
                branch.rightType = (planeSector.rightSubTree.type < 0) ? SectorType.rpCOLLBSP_LEAF_NODE : SectorType.rpCOLLBSP_BRANCH_NODE;

                ushort currentIndex = (ushort)iBranch++;

                branch.leftNode = ConvertNode(planeSector.leftSubTree, ref iBranch, ref iLeaf, ref numTotalPolygons);
                branch.rightNode = ConvertNode(planeSector.rightSubTree, ref iBranch, ref iLeaf, ref numTotalPolygons);

                return currentIndex;
            }
        }

    }

}

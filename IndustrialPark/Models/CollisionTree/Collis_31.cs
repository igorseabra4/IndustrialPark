using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Diagnostics;
using System.Linq;

namespace IndustrialPark.Models.CollisionTree
{
    using Triangle = RenderWareFile.Triangle;

    public sealed class Collis_31 : Collis_Shared
    {
        private const int rpCOLLBSP_LEAF_NODE = 1;
        private const int rpCOLLBSP_BRANCH_NODE = 2;

        private class RpCollBSPLeafNode
        {
            public ushort numPolygons;
            public ushort firstPolygon;
        }

        private class RpCollBSPBranchNode
        {
            public ushort type;
            public byte leftType;
            public byte rightType;
            public ushort leftNode;
            public ushort rightNode;
            public float leftValue;
            public float rightValue;
        }

        private class RpCollBSPTree
        {
            public int numLeafNodes;
            public RpCollBSPBranchNode[] branchNodes;
            public RpCollBSPLeafNode[] leafNodes;

            public RpCollBSPTree(int numLeafNodes)
            {
                this.numLeafNodes = numLeafNodes;
                branchNodes = Enumerable.Range(0, numLeafNodes - 1).Select(_ => new RpCollBSPBranchNode()).ToArray();
                leafNodes = Enumerable.Range(0, numLeafNodes).Select(_ => new RpCollBSPLeafNode()).ToArray();
            }
        }

        private class RpCollisionData
        {
            public int flags;
            public RpCollBSPTree tree;
            public int numTriangles;
            public ushort[] triangleMap;

            public RpCollisionData(Vector3[] vertices, Triangle[] triangles)
            {
                flags = 0;
                numTriangles = triangles.Length;
                triangleMap = new ushort[numTriangles];
                tree = _rpCollBSPTreeBuild(vertices, triangles, triangleMap);
            }
        }

        public static CollisionPLG_011D_Scooby RpCollisionGeometryBuildData(Geometry_000F geometry)
        {
            RpCollisionData collData = new RpCollisionData(geometry.geometryStruct.morphTargets[0].vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray(),
                geometry.geometryStruct.triangles);

            CollisionPLG_011D_Scooby collPLG = new CollisionPLG_011D_Scooby()
            {
                splits = collData.tree.branchNodes.Where(b => b != null).Select(b => new Split_Scooby()
                {
                    positiveType = (ScoobySectorType)b.rightType,
                    negativeType = (ScoobySectorType)b.leftType,
                    splitDirection = (SectorType)b.type,
                    positiveIndex = (short)b.rightNode,
                    negativeIndex = (short)b.leftNode,
                    negativeSplitPos = b.rightValue,
                    positiveSplitPos = b.leftValue,
                }).ToArray(),
                startIndex_amountOfTriangles = collData.tree.leafNodes.Select(l => new short[] { (short)l.firstPolygon, (short)l.numPolygons }).ToArray(),
                triangles = collData.triangleMap.Select(t => (int)t).ToArray()
            };

            return collPLG;
        }

        private static BuildSector BuildTreeGenerate(BuildData data)
        {
            int plane = 0;
            float value = 0;

            SetSortVertices(data);

            if (data.bspDepth >= rpCOLLTREE_MAXDEPTH || data.numPolygons < rpCOLLTREE_MIN_POLYGONS_FOR_SPLIT || !FindDividingPlane(data, ref plane, ref value))
            {
                return new BuildPolySector((ushort)data.numPolygons, (ushort)data.polygonOffset);
            }
            else
            {
                BuildData subdata;

                data.bspDepth++;

                SetClipCodes(data.numSortVerts, data.sortVerts, plane, value);
                GetClipStats(data, plane, value, out ClipStats stats);
                SortPolygons(data);

                BuildPlaneSector planeSector = new BuildPlaneSector(plane, stats.leftValue, stats.rightValue);

                subdata = data.Clone();
                subdata.numPolygons = stats.nLeft;
                SETCOORD(ref subdata.bbox.Maximum, plane, stats.leftValue);
                planeSector.leftSubTree = BuildTreeGenerate(subdata);

                subdata = data.Clone();
                subdata.numPolygons = stats.nRight;
                subdata.polygonOffset = data.polygonOffset + stats.nLeft;
                SETCOORD(ref subdata.bbox.Minimum, plane, stats.rightValue);
                planeSector.rightSubTree = BuildTreeGenerate(subdata);

                return planeSector;
            }
        }

        private static ushort ConvertNode(RpCollBSPTree flatTree, BuildSector sector, ref int iBranch, ref int iLeaf, ref int numTotalPolygons)
        {
            if (sector.type < 0) // Leaf node
            {
                BuildPolySector polySector = (BuildPolySector)sector;
                RpCollBSPLeafNode node = flatTree.leafNodes[iLeaf];

                node.numPolygons = polySector.numPolygons;
                node.firstPolygon = polySector.iFirstPolygon;

                numTotalPolygons += node.numPolygons;

                return (ushort)iLeaf++;
            }
            else // Branch node
            {
                BuildPlaneSector planeSector = (BuildPlaneSector)sector;
                RpCollBSPBranchNode node = flatTree.branchNodes[iBranch];

                node.type = (ushort)planeSector.type;
                node.leftValue = planeSector.leftValue;
                node.rightValue = planeSector.rightValue;
                node.leftType = (byte)((planeSector.leftSubTree.type < 0) ? rpCOLLBSP_LEAF_NODE : rpCOLLBSP_BRANCH_NODE);
                node.rightType = (byte)((planeSector.rightSubTree.type < 0) ? rpCOLLBSP_LEAF_NODE : rpCOLLBSP_BRANCH_NODE);

                ushort currentIndex = (ushort)iBranch++;

                node.leftNode = ConvertNode(flatTree, planeSector.leftSubTree, ref iBranch, ref iLeaf, ref numTotalPolygons);
                node.rightNode = ConvertNode(flatTree, planeSector.rightSubTree, ref iBranch, ref iLeaf, ref numTotalPolygons);

                return currentIndex;
            }
        }

        private static RpCollBSPTree BuildTreeConvertNodes(RpCollBSPTree flatTree, BuildSector buildTree, BuildData data)
        {
            int iBranch = 0;
            int iLeaf = 0;
            int numTotalPolygons = 0;

            ConvertNode(flatTree, buildTree, ref iBranch, ref iLeaf, ref numTotalPolygons);

            Assert(iLeaf == flatTree.numLeafNodes);
            Assert(iBranch == flatTree.numLeafNodes - 1);
            Assert(numTotalPolygons == data.numPolygons);

            return flatTree;
        }

        private static RpCollBSPTree _rpCollBSPTreeBuild(Vector3[] vertices,Triangle[] triangles, ushort[] triangleSortMap)
        {
            BuildData data = new BuildData(vertices, triangles);
            BuildSector tree = BuildTreeGenerate(data);
            RpCollBSPTree collBSPTree = new RpCollBSPTree(BuildTreeCountLeafNodes(tree));

            BuildTreeConvertNodes(collBSPTree, tree, data);

            for (int iTri = 0; iTri < triangles.Length; iTri++)
                triangleSortMap[iTri] = (ushort)Array.IndexOf(triangles, data.polygons[iTri].poly);

            return collBSPTree;
        }

    }

}

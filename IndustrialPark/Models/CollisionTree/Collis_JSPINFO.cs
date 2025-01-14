using System;
using SharpDX;
using RenderWareFile;
using RenderWareFile.Sections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace IndustrialPark.Models.CollisionTree
{
    using Triangle = RenderWareFile.Triangle;

    public sealed class Collis_JSPINFO : Collis_Shared
    {
        private xClumpCollBSPBranchNode[] branchNodes;
        private xClumpCollBSPTriangle[] triangles;

        public Collis_JSPINFO() : base(minPolygonsForSplit: 5) { }

        public HI_Tags_BEEF01 HIJSPCollisionBuild(params Clump_0010[] clumps)
        {
            if (clumps.Length != 1 && clumps.Length != 3)
                throw new ArgumentOutOfRangeException("Only 1 or 3 clump(s) are accepted");

            var vertices = InitVertices(clumps);

            var tris = InitTriangles(clumps);
            triangles = new xClumpCollBSPTriangle[tris.Item1.Length];

            BuildTree(vertices, tris);

            return new HI_Tags_BEEF01()
            {
                branchNodes = this.branchNodes,
                triangles = this.triangles,
            };
        }

        private void BuildTree(Vertex3[] vertices, (xClumpCollBSPTriangle[], Triangle[]) triangles)
        {
            BuildData data = new BuildData(vertices.Select(v => new SharpDX.Vector3(v.X, v.Y, v.Z)).ToArray(), triangles.Item2);
            BuildSector tree = BuildTreeGenerate(data);

            branchNodes = new xClumpCollBSPBranchNode[BuildTreeCountLeafNodes(tree) - 1];

            ushort[] triangleMap = new ushort[triangles.Item2.Length];
            for (int iTri = 0; iTri < triangles.Item2.Length; iTri++)
                triangleMap[iTri] = (ushort)Array.IndexOf(triangles.Item2, data.polygons[iTri].poly);

            int iBranch = 0;
            int numTotalPolygons = 0;

            ConvertNode(tree, triangles.Item1, triangleMap, ref iBranch, ref numTotalPolygons);

            Assert(numTotalPolygons == data.numPolygons);
            Assert(numTotalPolygons == this.triangles.Length);
            Assert(iBranch == branchNodes.Length);
        }

        private ushort ConvertNode(BuildSector sector, xClumpCollBSPTriangle[] tri, ushort[] triangleMap, ref int iBranch, ref int numTotalPolygons)
        {
            if (sector.type < 0)
            {
                BuildPolySector polySector = sector as BuildPolySector;

                int last = polySector.iFirstPolygon + polySector.numPolygons;
                for (int i = polySector.iFirstPolygon; i < last; i++)
                {
                    triangles[i] = tri[triangleMap[i]];
                    if (i == last - 1)
                        triangles[i].flags &= ~ClumpCollBSPTriangleFlags.kCLUMPCOLL_HASNEXT;
                }

                int triIndex = numTotalPolygons;
                numTotalPolygons += polySector.numPolygons;

                return (ushort)triIndex;
            }
            else
            {
                BuildPlaneSector planeSector = sector as BuildPlaneSector;
                ref xClumpCollBSPBranchNode branch = ref branchNodes[iBranch];

                branch.LeftAxis = ConvertDirection(planeSector.type);
                branch.RightAxis = branch.LeftAxis;
                branch.LeftValue = planeSector.leftValue;
                branch.RightValue = planeSector.rightValue;
                branch.LeftType = (planeSector.leftSubTree.type < 0) ? ClumpCollType.Leaf : ClumpCollType.Branch;
                branch.RightType = (planeSector.rightSubTree.type < 0) ? ClumpCollType.Leaf : ClumpCollType.Branch;

                ushort currentIndex = (ushort)iBranch++;

                branch.LeftIndex = ConvertNode(planeSector.leftSubTree, tri, triangleMap, ref iBranch, ref numTotalPolygons);
                branch.RightIndex = ConvertNode(planeSector.rightSubTree, tri, triangleMap, ref iBranch, ref numTotalPolygons);

                return currentIndex;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="clumps"></param>
        /// <returns>A tuple containing: <para/>
        /// - An Array of <see cref="xClumpCollBSPTriangle"/> representing the triangles in <see cref="HI_Tags_BEEF01.triangles"/>.
        /// Following flags are set: <see cref="ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISSOLID"/>, <see cref="ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISREVERSE"/> (in tristrips),
        /// <see cref="ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISVISIBLE"/> (when <see cref="AtomicFlags.Render"/> flag is set).
        /// They are also all in a big unsorted chain for now (<see cref="ClumpCollBSPTriangleFlags.kCLUMPCOLL_HASNEXT"/>). <para/>
        /// - An array of <see cref="Triangle"/> used for building the collision tree.
        /// </returns>
        private (xClumpCollBSPTriangle[], Triangle[]) InitTriangles(params Clump_0010[] clumps)
        {
            List<Triangle> binTris = new();
            List<xClumpCollBSPTriangle> mTriangles = new();

            xClumpCollBSPTriangle tri = new();
            tri.flags = ClumpCollBSPTriangleFlags.kCLUMPCOLL_HASNEXT;

            int stripVecOffset = 0;
            int atomicOffset = clumps.Sum(c => c.atomicList.Count) -1;
            int clumpVertOffset = 0;
            foreach (var clump in clumps.ToArray().Reverse())
            {
                for (int atomIndex = clump.atomicList.Count - 1; atomIndex >= 0; atomIndex--)
                {
                    Atomic_0014 atomic = clump.atomicList[atomIndex];
                    Geometry_000F geometry = clump.geometryList.geometryList[atomic.atomicStruct.geometryIndex];
                    BinMeshPLG_050E binMeshPLG = geometry.geometryExtension.extensionSectionList.OfType<BinMeshPLG_050E>().FirstOrDefault();
                    tri.atomIndex = (ushort)atomicOffset--;

                    if ((atomic.atomicStruct.flags & AtomicFlags.Render) != 0)
                        tri.flags |= ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISVISIBLE;
                    else
                        tri.flags &= ~ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISVISIBLE;

                    if ((atomic.atomicStruct.flags & AtomicFlags.CollisionTest) != 0)
                        tri.flags |= ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISSOLID;
                    else
                        tri.flags &= ~ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISSOLID;

                    int meshVertOffset = 0;
                    for (int meshIndex = 0; meshIndex < binMeshPLG.numMeshes; meshIndex++)
                    {
                        BinMesh mesh = binMeshPLG.binMeshList[meshIndex];
                        tri.matIndex = (short)(mesh.materialIndex);

                        bool isTristrip = (binMeshPLG.binMeshHeaderFlags & BinMeshHeaderFlags.TriangleStrip) != 0;
                        int step = isTristrip ? 1 : 3;
                        for (int vertIndex = 0; vertIndex < (isTristrip ? mesh.indexCount - 2 : mesh.indexCount); vertIndex += step)
                        {
                            if (isTristrip && RenderWareModelFile.IsDegenerate(mesh.vertexIndices[vertIndex], mesh.vertexIndices[vertIndex + 1], mesh.vertexIndices[vertIndex + 2]))
                                continue;
                            tri.meshVertIndex = (ushort)(meshVertOffset + vertIndex);
                            tri.rawIdx = clumpVertOffset + vertIndex;

                            binTris.Add(new Triangle()
                            {
                                materialIndex = (ushort)mesh.materialIndex,
                                vertex1 = (ushort)(mesh.vertexIndices[vertIndex] + stripVecOffset),
                                vertex2 = (ushort)(mesh.vertexIndices[vertIndex + 1] + stripVecOffset),
                                vertex3 = (ushort)(mesh.vertexIndices[vertIndex + 2] + stripVecOffset),
                            });

                            if (isTristrip && (vertIndex & 1) != 0)
                                tri.flags |= ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISREVERSE;
                            else
                                tri.flags &= ~ClumpCollBSPTriangleFlags.kCLUMPCOLL_ISREVERSE;
                            mTriangles.Add(tri);
                        }
                        meshVertOffset += mesh.indexCount;
                        clumpVertOffset += mesh.indexCount;
                    }
                    stripVecOffset += geometry.geometryStruct.numVertices;
                }
            }
            return (mTriangles.ToArray(), binTris.ToArray());
        }

        private Vertex3[] InitVertices(params Clump_0010[] clumps)
        {
            List<Vertex3> vertices = new();
            foreach (var clump in clumps.ToArray().Reverse())
                foreach (var geo in clump.geometryList.geometryList.ToArray().Reverse())
                    vertices.AddRange(geo.geometryStruct.morphTargets[0].vertices);
            return vertices.ToArray();
        }

        private ClumpAxis ConvertDirection(int axis)
        {
            return axis switch
            {
                0 => ClumpAxis.X,
                4 => ClumpAxis.Y,
                8 => ClumpAxis.Z,
                _ => ClumpAxis.Unknown
            };
        }

    }
}
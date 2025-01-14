using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace IndustrialPark.Models.CollisionTree
{
    using Triangle = RenderWareFile.Triangle;

    public sealed class Collis_36 : Collis_Shared
    {
        private const int rpCOLLSECTOR_CONTENTS_MAXCOUNT = 0xef;
        private const int rpCOLLSECTOR_CONTENTS_SPLIT = 0xff;
        private const int rpCOLLSECTOR_TYPE_NEG = 0x1;

        private class RpCollTree
        {
            public bool useMap;
            public BoundingBox bbox;
            public ushort numEntries;
            public ushort numSplits;
            public RpCollSplit[] splits;
            public ushort[] map;

            public RpCollTree(ushort numEntries, ushort numSplits, BoundingBox bbox, bool useMap)
            {
                this.useMap = useMap;
                this.numEntries = numEntries;
                this.numSplits = numSplits;
                this.bbox = bbox;
                splits = Enumerable.Range(0, numSplits).Select(_ => new RpCollSplit()).ToArray();

                if (useMap)
                    map = new ushort[numEntries];
            }
        }

        private class RpCollSplit
        {
            public RpCollSector leftSector;
            public RpCollSector rightSector;

            public RpCollSplit()
            {
                leftSector = new RpCollSector();
                rightSector = new RpCollSector();
            }
        }

        private class RpCollSector
        {
            public byte type;
            public byte contents;
            public ushort index;
            public float value;
        }

        public Collis_36() : base() { }

        public CollisionPLG_011D RpCollisionGeometryBuildData(Geometry_000F geometry, bool sortTriangles = false)
        {
            Vector3[] vertices = geometry.geometryStruct.morphTargets[0].vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray();
            Triangle[] tris = (Triangle[])geometry.geometryStruct.triangles.Clone();

            BuildData buildData = new BuildData(vertices, tris);
            ushort[] map = sortTriangles ? new ushort[tris.Length] : null;
            RpCollTree collTree = CreateCollTreeFromBuildTree(buildData, BuildTreeGenerate(buildData), map);

            if (sortTriangles)
                for (int i = 0; i < tris.Length; i++)
                    geometry.geometryStruct.triangles[i] = tris[map[i]];

            CollisionPLG_011D collPLG = new CollisionPLG_011D()
            {
                version = 0x37002,
                colTree = new ColTree_002C()
                {
                    colTreeStruct = new ColTreeStruct_0001()
                    {
                        useMap = Convert.ToInt32(collTree.useMap),
                        boxMaximum = new Vertex3(collTree.bbox.Maximum.X, collTree.bbox.Maximum.Y, collTree.bbox.Maximum.Z),
                        boxMinimum = new Vertex3(collTree.bbox.Minimum.X, collTree.bbox.Minimum.Y, collTree.bbox.Minimum.Z),
                        numTriangles = collTree.numEntries,
                        numSplits = collTree.numSplits,
                        splitArray = collTree.splits.Select(s => new Split()
                        {
                            negativeSector = new Sector()
                            {
                                type = (SectorAxis)s.leftSector.type,
                                triangleAmount = s.leftSector.contents,
                                referenceIndex = s.leftSector.index,
                                splitPosition = s.leftSector.value
                            },
                            positiveSector = new Sector()
                            {
                                type = (SectorAxis)s.rightSector.type,
                                triangleAmount = s.rightSector.contents,
                                referenceIndex = s.rightSector.index,
                                splitPosition = s.rightSector.value
                            },
                        }).ToArray(),
                        triangleArray = collTree.map
                    }
                }
            };

            return collPLG;
        }

        private RpCollTree CreateCollTreeFromBuildTree(BuildData buildData, BuildSector rootSector, ushort[] map)
        {
            int numSplits = BuildTreeCountLeafNodes(rootSector) - 1;
            bool useMap = (map != null) ? false : true;

            Assert(buildData.numPolygons <= 0xffff);
            Assert(numSplits <= 0xffff);
            Assert(buildData.numPolygons > 0);
            RpCollTree collTree = new RpCollTree((ushort)buildData.numPolygons, (ushort)numSplits, buildData.bbox, useMap);

            if (map == null)
                map = collTree.map;

            for (int iTri = 0; iTri < buildData.numPolygons; iTri++)
            {
                map[iTri] = (ushort)Array.IndexOf(buildData.origPolys, buildData.polygons[iTri].poly);
            }

            if (rootSector.type < 0)
                return collTree;

            int iSplit = 0;
            int totalNumpolys = 0;

            void ProcessSector(BuildSector sector, RpCollSector reff = null)
            {
                RpCollSplit split = collTree.splits[iSplit];
                BuildPlaneSector planeSector = (BuildPlaneSector)sector;
                BuildPolySector polySector;

                Assert(sector.type >= 0);

                if (reff != null)
                    reff.index = (ushort)iSplit;
                iSplit++;

                split.rightSector.type = (byte)planeSector.type;
                split.leftSector.type = (byte)(planeSector.type | rpCOLLSECTOR_TYPE_NEG);
                split.leftSector.value = planeSector.leftValue;
                split.rightSector.value = planeSector.rightValue;


                if (planeSector.rightSubTree.type < 0)
                {
                    polySector = (BuildPolySector)planeSector.rightSubTree;
                    Assert(polySector.numPolygons <= rpCOLLSECTOR_CONTENTS_MAXCOUNT);
                    split.rightSector.contents = (byte)polySector.numPolygons;
                    split.rightSector.index = polySector.iFirstPolygon;
                    totalNumpolys += split.rightSector.contents;
                }
                else
                {
                    split.rightSector.contents = rpCOLLSECTOR_CONTENTS_SPLIT;
                    ProcessSector(planeSector.rightSubTree, split.rightSector);
                }

                if (planeSector.leftSubTree.type < 0)
                {
                    polySector = (BuildPolySector)planeSector.leftSubTree;
                    Assert(polySector.numPolygons <= rpCOLLSECTOR_CONTENTS_MAXCOUNT);
                    split.leftSector.contents = (byte)polySector.numPolygons;
                    split.leftSector.index = polySector.iFirstPolygon;
                    totalNumpolys += split.leftSector.contents;
                }
                else
                {
                    split.leftSector.contents = rpCOLLSECTOR_CONTENTS_SPLIT;
                    ProcessSector(planeSector.leftSubTree, split.leftSector);
                }

            }
            ProcessSector(rootSector);

            Assert(iSplit == collTree.numSplits);
            Assert(totalNumpolys == buildData.numPolygons);

            return collTree;

        }

        protected override BuildSector BuildTreeGenerate(BuildData data)
        {
            int plane = 0;
            float value = 0;
            ClipStats stats;

            if (data.numPolygons < rpCOLLTREE_MIN_POLYGONS_FOR_SPLIT)
            {
                return new BuildPolySector((ushort)data.numPolygons, (ushort)data.polygonOffset);
            }
            else if (data.bspDepth < rpCOLLTREE_MAXDEPTH)
            {
                SetSortVertices(data);

                if (FindDividingPlane(data, ref plane, ref value))
                {
                    SetClipCodes(data.numSortVerts, data.sortVerts, plane, value);
                    GetClipStats(data, plane, value, out stats);
                    SortPolygons(data);
                }
                else
                {
                    if (data.numPolygons <= rpCOLLSECTOR_CONTENTS_MAXCOUNT)
                    {
                        return new BuildPolySector((ushort)data.numPolygons, (ushort)data.polygonOffset);
                    }
                    else
                    {
                        plane = 0;
                        stats = new ClipStats()
                        {
                            leftValue = data.bbox.Maximum.X,
                            rightValue = data.bbox.Minimum.X,
                            nLeft = data.numPolygons >> 1,
                            nRight = data.numPolygons - (data.numPolygons >> 1),
                        };
                    }
                }
            }
            else
            {
                if (data.numPolygons <= rpCOLLSECTOR_CONTENTS_MAXCOUNT)
                {
                    return new BuildPolySector((ushort)data.numPolygons, (ushort)data.polygonOffset);
                }
                else
                {
                    throw new Exception("Max tree depth reached with too many triangles remaining for a leaf node.");
                }
            }

            BuildData subdata;

            data.bspDepth++;

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
}

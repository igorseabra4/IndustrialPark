using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark.Models.CollisionTree
{
    using Triangle = RenderWareFile.Triangle;

    public enum CollTreeVersion
    {
        NONE,
        COLLTREE_31,
        COLLTREE_36,
        COLLTREE_36_SORTTRIANGLES
    }

    public abstract class Collis_Shared
    {
        protected const int CLIPVERTEXLEFT = 1;
        protected const int CLIPVERTEXRIGHT = 2;
        protected const int CLIPPOLYGONLEFT = 1;
        protected const int CLIPPOLYGONRIGHT = 2;

        protected readonly int rpCOLLTREE_MIN_POLYGONS_FOR_SPLIT;
        protected readonly int rpCOLLTREE_MAXDEPTH;
        protected readonly int MAXCLOSESTCHECK;
        protected readonly float RANGEEPS;

        protected class BuildSector
        {
            public int type;

            public BuildSector(int type)
            {
                this.type = type;
            }
        }

        protected class BuildPolySector : BuildSector
        {
            public ushort numPolygons;
            public ushort iFirstPolygon;

            public BuildPolySector(ushort numPolygons, ushort iFirstPolygon) : base(-1)
            {
                this.numPolygons = numPolygons;
                this.iFirstPolygon = iFirstPolygon;
            }
        }

        protected class BuildPlaneSector : BuildSector
        {
            public BuildSector leftSubTree;
            public BuildSector rightSubTree;
            public float leftValue;
            public float rightValue;

            public BuildPlaneSector(int type, float left, float right) : base(type)
            {
                leftValue = left;
                rightValue = right;
            }
        }

        protected class BuildData
        {
            public int numVertices;
            public BuildVertex[] vertices;
            public BuildPoly[] polygons;
            public Triangle[] origPolys;

            public int bspDepth;
            public BoundingBox bbox;
            public int numPolygons;
            public int polygonOffset;

            public int numSortVerts;
            public BuildVertex[] sortVerts;

            public BuildData() { }

            public BuildData(Vector3[] vertices, Triangle[] polygons)
            {
                numVertices = vertices.Length;
                this.vertices = vertices.Select(v => new BuildVertex() { pos = v }).ToArray();
                this.polygons = polygons.Select(p => new BuildPoly() { poly = p }).ToArray();
                origPolys = polygons;
                bspDepth = 0;
                bbox = BoundingBox.FromPoints(vertices);
                numPolygons = polygons.Length;
                polygonOffset = 0;
                sortVerts = new BuildVertex[numVertices];
            }

            public BuildData Clone()
            {
                return (BuildData)MemberwiseClone();
            }
        }

        protected class BuildVertex : IComparable<BuildVertex>
        {
            public Vector3 pos;
            public int flags;
            public float dist;

            public int CompareTo(object obj)
            {
                return CompareTo(obj as BuildVertex);
            }

            public int CompareTo(BuildVertex b)
            {
                return this.dist.CompareTo(b.dist);
            }

        }
        protected struct BuildPoly
        {
            public Triangle poly;
            public int flags;
            public ushort[] VertIndex
            {
                get => new ushort[] { poly.vertex2, poly.vertex1, poly.vertex3 };
            }
        }

        protected struct ClipStats
        {
            public int nLeft;
            public int nRight;
            public float leftValue;
            public float rightValue;
        }

        public Collis_Shared(int minPolygonsForSplit = 4, int maxDepth = 32, int maxClosestCheck = 50, float rangeEps = 0.0001f)
        {
            rpCOLLTREE_MIN_POLYGONS_FOR_SPLIT = minPolygonsForSplit;
            rpCOLLTREE_MAXDEPTH = maxDepth;
            MAXCLOSESTCHECK = maxClosestCheck;
            RANGEEPS = rangeEps;
        }

        protected bool FindDividingPlane(BuildData data, ref int plane, ref float value)
        {
            float bestScore = float.MaxValue;
            int bestPlane = 0;
            float bestValue = 0;
            int tryPlane;
            Vector3 vSize = data.bbox.Size;

            int maxExtentPlane = 0;
            for (tryPlane = 0; tryPlane < 12; tryPlane += 4)
            {
                if (GETCOORD(vSize, tryPlane) > GETCOORD(vSize, maxExtentPlane))
                    maxExtentPlane = tryPlane;
            }

            for (tryPlane = 0; tryPlane < 12; tryPlane += 4)
            {
                float quantThresh;
                int nI;

                float extentScore = FuzzyExtentScore(GETCOORD(vSize, tryPlane), GETCOORD(vSize, maxExtentPlane));

                for (nI = 0; nI < data.numSortVerts; nI++)
                {
                    data.sortVerts[nI].dist = GETCOORD(data.sortVerts[nI].pos, tryPlane);
                }

                Array.Sort(data.sortVerts, 0, data.numSortVerts);

                float median = data.sortVerts[nI >> 1].dist;
                int lowerQuartile = nI >> 2;
                int upperQuartile = (nI >> 1) + (nI >> 2);

                float interQuartileRange = data.sortVerts[upperQuartile].dist - data.sortVerts[lowerQuartile].dist;

                if (interQuartileRange < (RANGEEPS * GETCOORD(vSize, maxExtentPlane)))
                {
                    lowerQuartile = upperQuartile = (nI >> 1);
                    quantThresh = 0;
                }
                else
                {
                    quantThresh = MAXCLOSESTCHECK / interQuartileRange;

                    for (nI = lowerQuartile; nI <= upperQuartile; nI++)
                    {
                        float dist = median - data.sortVerts[nI].dist;
                        data.sortVerts[nI].dist = Math.Abs(dist);
                    }

                    Array.Sort(data.sortVerts, lowerQuartile, upperQuartile - lowerQuartile + 1);
                }

                int lastquantValue = -1;
                int numUniq = MAXCLOSESTCHECK;
                for (nI = lowerQuartile; nI <= upperQuartile && numUniq > 0; nI++)
                {
                    float tryValue;
                    int quantValue;

                    tryValue = GETCOORD(data.sortVerts[nI].pos, tryPlane);
                    quantValue = (int)(tryValue * quantThresh);
                    if (quantValue != lastquantValue)
                    {
                        SetClipCodes(data.numSortVerts, data.sortVerts, tryPlane, tryValue);

                        GetClipStats(data, tryPlane, tryValue, out ClipStats stats);

                        if (stats.nLeft != 0 && stats.nRight != 0)
                        {
                            float score = DivisionScore(data, stats, tryPlane) * extentScore;

                            if (score < bestScore)
                            {
                                bestScore = score;
                                bestPlane = tryPlane;
                                bestValue = tryValue;
                            }
                        }

                        lastquantValue = quantValue;
                        numUniq--;
                    }
                }

            }

            if (bestScore < float.MaxValue)
            {
                plane = bestPlane;
                value = bestValue;
                return true;
            }
            return false;
        }

        protected virtual BuildSector BuildTreeGenerate(BuildData data)
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

        protected void SetClipCodes(int numVert, BuildVertex[] sortVerts, int plane, float value)
        {
            for (int iVert = 0; iVert < numVert; iVert++)
            {
                float test = GETCOORD(sortVerts[iVert].pos, plane) - value;

                sortVerts[iVert].flags = 0;

                if (test <= 0)
                    sortVerts[iVert].flags |= CLIPVERTEXLEFT;

                if (test >= 0)
                    sortVerts[iVert].flags |= CLIPVERTEXRIGHT;
            }
        }

        protected void GetClipStats(BuildData data, int plane, float value, out ClipStats stats)
        {
            float overlapLeft = 0;
            float overlapRight = 0;
            int nLeft = 0;
            int nRight = 0;

            int pidx = data.polygonOffset;

            for (int numPolygons = data.numPolygons; --numPolygons >= 0; pidx++)
            {
                int clip = CLIPVERTEXLEFT | CLIPVERTEXRIGHT;
                float distLeft = 0;
                float distRight = 0;

                foreach (ushort vert in data.polygons[pidx].VertIndex)
                {
                    Vector3 pos = data.vertices[vert].pos;
                    float dist = GETCOORD(pos, plane) - value;
                    float mdist = -dist;

                    clip &= data.vertices[vert].flags;

                    if (mdist > distLeft)
                        distLeft = mdist;
                    else if (dist > distRight)
                        distRight = dist;
                }

                switch (clip & (CLIPVERTEXLEFT | CLIPVERTEXRIGHT))
                {
                    case CLIPVERTEXLEFT:
                        data.polygons[pidx].flags = CLIPPOLYGONLEFT;
                        nLeft++;
                        break;
                    case CLIPVERTEXLEFT | CLIPVERTEXRIGHT:
                    case CLIPVERTEXRIGHT:
                        data.polygons[pidx].flags = CLIPPOLYGONRIGHT;
                        nRight++;
                        break;
                    default:
                        if (distRight > distLeft)
                        {
                            nRight++;
                            data.polygons[pidx].flags = CLIPPOLYGONRIGHT;
                            if (distLeft > overlapRight)
                            {
                                overlapRight = distLeft;
                            }
                        }
                        else
                        {
                            nLeft++;
                            data.polygons[pidx].flags = CLIPPOLYGONLEFT;
                            if (distRight > overlapLeft)
                            {
                                overlapLeft = distRight;
                            }
                        }
                        break;
                }
            }

            stats = new ClipStats()
            {
                nLeft = nLeft,
                nRight = nRight,
                leftValue = value + overlapLeft,
                rightValue = value - overlapRight,
            };
        }

        private float DivisionScore(BuildData data, ClipStats stats, int plane)
        {
            float sectorExtent = GETCOORD(data.bbox.Maximum, plane) - GETCOORD(data.bbox.Minimum, plane);
            float leftExtent = stats.leftValue - GETCOORD(data.bbox.Minimum, plane);
            float rightExtent = GETCOORD(data.bbox.Maximum, plane) - stats.rightValue;

            return (leftExtent * AverageTreeDepth(stats.nLeft) + rightExtent * AverageTreeDepth(stats.nRight)) / sectorExtent;
        }

        private float AverageTreeDepth(int numLeaves)
        {
            Assert(numLeaves > 0);

            int depth = -1;
            int temp = numLeaves;
            while (temp != 0)
            {
                depth++;
                temp >>= 1;
            }

            return (depth + 2f) - (float)(1 << (depth + 1)) / numLeaves;
        }

        protected int BuildTreeCountLeafNodes(BuildSector tree)
        {
            if (tree.type < 0)
                return 1;

            BuildPlaneSector sector = (BuildPlaneSector)tree;
            return BuildTreeCountLeafNodes(sector.leftSubTree) + BuildTreeCountLeafNodes(sector.rightSubTree);
        }

        protected void SetSortVertices(BuildData data)
        {
            int iVert;

            for (iVert = 0; iVert < data.numVertices; iVert++)
                data.vertices[iVert].flags = 0;

            for (int iPoly = 0; iPoly < data.numPolygons; iPoly++)
                foreach (var ind in data.polygons[data.polygonOffset + iPoly].VertIndex)
                    data.vertices[ind].flags++;

            int iSort = 0;
            for (iVert = 0; iVert < data.numVertices; iVert++)
            {
                if (data.vertices[iVert].flags != 0)
                {
                    data.sortVerts[iSort] = data.vertices[iVert];
                    iSort++;
                }
            }
            data.numSortVerts = iSort;
        }

        private float FuzzyExtentScore(float extent, float maxExtent)
        {
            float value = extent / maxExtent;
            return (value > 0.5f) ? 1f : 1f / (0.5f + value);
        }

        protected void SortPolygons(BuildData data)
        {
            int pidx = data.polygonOffset;
            int leftCount = 0;

            for (int iPoly = 0; iPoly < data.numPolygons; iPoly++)
            {
                if (data.polygons[pidx + iPoly].flags == CLIPPOLYGONLEFT)
                {
                    if (iPoly > leftCount)
                    {
                        Triangle swap = data.polygons[pidx + leftCount].poly;
                        data.polygons[pidx + leftCount].poly = data.polygons[pidx + iPoly].poly;
                        data.polygons[pidx + iPoly].poly = swap;
                    }

                    leftCount++;
                }
            }
        }
        protected void Assert(bool condition)
        {
            if (!condition)
                throw new Exception();
        }

        private float GETCOORD(Vector3 v, int size)
        {
            return v[size / 4];
        }

        protected void SETCOORD(ref Vector3 v, int size, float value)
        {
            v[size / 4] = value;
        }

    }
}

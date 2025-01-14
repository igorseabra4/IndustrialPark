using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Vector3 = SharpDX.Vector3;

namespace IndustrialPark
{
    public struct xJSPNodeInfo
    {
        /// <summary>
        /// This is the value that will be placed in the <see cref="xClumpCollBSPTriangle.matIndex"/> field of all triangles.
        /// </summary>
        public int originalMatIndex { get; set; }
        /// <summary>
        /// Rendering flags
        /// </summary>
        public ushort nodeFlags;
        public bool Visible
        {
            get => (nodeFlags & 1) != 0;
            set
            {
                if (value)
                    nodeFlags |= 1;
                else
                    unchecked { nodeFlags &= (ushort)~1; }
            }
        }

        public bool DisableZBufferWrite
        {
            get => (nodeFlags & 2) != 0;
            set
            {
                if (value)
                    nodeFlags |= 2;
                else
                    unchecked { nodeFlags &= (ushort)~2; }
            }
        }

        public bool DisableBackFaceCulling
        {
            get => (nodeFlags & 4) != 0;
            set
            {
                if (value)
                    nodeFlags |= 4;
                else
                    unchecked { nodeFlags &= (ushort)~4; }
            }
        }
        /// <summary>
        /// Only present in version 5 (>= TSSM)
        /// </summary>
        public short sortOrder { get; set; }
    }

    public struct xJSPNodeTreeBranch
    {
        public ushort leftNode { get; set; }
        public ushort rightNode { get; set; }
        public byte leftType { get; set; }
        public byte rightType { get; set; }
        public ushort coord { get; set; }
        public float leftValue { get; set; }
        public float rightValue { get; set; }
    }

    public struct xJSPNodeTreeLeaf
    {
        public int nodeIndex { get; set; }
        public int leafCount { get; set; }
        public Vector3 sup { get; set; }
        public Vector3 inf { get; set; }

        public BoundingBox BBox
        {
            get => new BoundingBox(inf, sup);
            set
            {
                sup = value.Maximum;
                inf = value.Minimum;
            }
        }
    }

    public class HI_Tags_BEEF02 : GenericAssetDataContainer
    {
        public int RenderWareVersion;
        public Platform platform;

        /// <summary>
        /// Version 3 = BFBB. Version 5 = TSSM, Incredibles, ROTU, Ratproto
        /// </summary>
        [ReadOnly(true)]
        public int Version { get; set; }

        /// <summary>
        /// Rendering infos for all atomics 
        /// </summary>
        public xJSPNodeInfo[] jspNodeList { get; set; }

        public xJSPNodeTreeBranch[] branchNodes { get; set; }
        public xJSPNodeTreeLeaf[] leafNodes { get; set; }

        /// <summary>
        /// Pre-calculated vertices, accessed by <see cref="xClumpCollBSPTriangle.meshVertIndex"/>
        /// </summary>
        public Vertex3[] stripVecList { get; set; }

        /// <summary>
        /// 0x1 = Color <para/>
        /// 0x2 = Normal <para/>
        /// 0x4 = UV (int16) <para/>
        /// 0x8 = UV (float)
        /// </summary>
        public ushort VertDataFlags { get; set; }
        public ushort VertDataStride { get; set; }
        public byte[] VertData { get; set; }

        public HI_Tags_BEEF02(EndianBinaryReader reader, Endianness endianness)
        {
            reader.endianness = Endianness.Little;
            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();
            reader.ReadChars(4);

            reader.endianness = endianness;

            Version = reader.ReadInt32();
            int jspNodeCount = reader.ReadInt32();

            if (Version == 3)
                reader.BaseStream.Position += 12;
            else if (Version == 5)
            {
                reader.BaseStream.Position += 20;
                VertDataFlags = reader.ReadUInt16();
                VertDataStride = reader.ReadUInt16();
                reader.BaseStream.Position += 8;
            }

            jspNodeList = new xJSPNodeInfo[jspNodeCount];
            for (int i = 0; i < jspNodeCount; i++)
                if (Version == 3)
                {
                    jspNodeList[i] = new xJSPNodeInfo()
                    {
                        originalMatIndex = reader.ReadInt32(),
                        nodeFlags = (ushort)reader.ReadInt32(),
                    };
                }
                else if (Version == 5)
                {
                    jspNodeList[i] = new xJSPNodeInfo()
                    {
                        originalMatIndex = reader.ReadInt32(),
                        nodeFlags = reader.ReadUInt16(),
                        sortOrder = reader.ReadInt16(),
                    };
                }

            if (Version == 5)
            {
                int numBranchNodes = reader.ReadInt32();
                reader.BaseStream.Position += 4;
                int numLeafNodes = reader.ReadInt32();
                reader.BaseStream.Position += 4;

                branchNodes = new xJSPNodeTreeBranch[numBranchNodes];
                for (int i = 0; i < numBranchNodes; i++)
                {
                    branchNodes[i] = new xJSPNodeTreeBranch()
                    {
                        leftNode = reader.ReadUInt16(),
                        rightNode = reader.ReadUInt16(),
                        leftType = reader.ReadByte(),
                        rightType = reader.ReadByte(),
                        coord = reader.ReadUInt16(),
                        leftValue = reader.ReadSingle(),
                        rightValue = reader.ReadSingle(),
                    };
                }

                leafNodes = new xJSPNodeTreeLeaf[numLeafNodes];
                for (int i = 0; i < numLeafNodes; i++)
                {
                    leafNodes[i] = new xJSPNodeTreeLeaf()
                    {
                        nodeIndex = reader.ReadInt32(),
                        leafCount = reader.ReadInt32(), // In vanilla this seems to contain garbage bytes often, like due to allocation
                        sup = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        inf = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    };
                }

                int stripVecCount = reader.ReadInt32();
                stripVecList = new Vertex3[stripVecCount];
                for (int i = 0; i < stripVecCount; i++)
                    stripVecList[i] = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                if (VertDataFlags != 0)
                    VertData = reader.ReadBytes(VertDataStride * stripVecCount);
            }

        }

        public HI_Tags_BEEF02(Game game, Platform platform) : this()
        {
            this.platform = platform;
            // Version 5 doesnt not load for some reason.
            // The game converts everything at runtime anyway as long as we don't use native data models
            Version = game == Game.BFBB ? 3 : 5;
        }

        public HI_Tags_BEEF02()
        {
            jspNodeList = [];
            branchNodes = [];
            leafNodes = [];
            stripVecList = [];
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;
            writer.Write(new byte[12]);

            writer.Write("JSP\0");

            writer.Write(Version);
            writer.Write(jspNodeList.Length);

            if (Version == 3)
                writer.Write(new byte[12]);
            else if (Version == 5)
            {
                writer.Write(new byte[20]);
                writer.Write(VertDataFlags);
                writer.Write(VertDataStride);
                writer.Write(new byte[8]);
            }

            for (int i = 0; i < jspNodeList.Length; i++)
            {
                writer.Write(jspNodeList[i].originalMatIndex);
                if (Version == 3)
                    writer.Write((int)jspNodeList[i].nodeFlags);
                else if (Version == 5)
                {
                    writer.Write(jspNodeList[i].nodeFlags);
                    writer.Write(jspNodeList[i].sortOrder);
                }
            }

            if (Version == 5)
            {
                writer.Write(branchNodes?.Length ?? 0);
                writer.Write(0);
                writer.Write(leafNodes?.Length ?? 0);
                writer.Write(0);

                for (int i = 0; i < branchNodes?.Length; i++)
                {
                    writer.Write(branchNodes[i].leftNode);
                    writer.Write(branchNodes[i].rightNode);
                    writer.Write(branchNodes[i].leftType);
                    writer.Write(branchNodes[i].rightType);
                    writer.Write(branchNodes[i].coord);
                    writer.Write(branchNodes[i].leftValue);
                    writer.Write(branchNodes[i].rightValue);
                }

                for (int i = 0; i < leafNodes?.Length; i++) 
                {
                    writer.Write(leafNodes[i].nodeIndex);
                    writer.Write(leafNodes[i].leafCount);
                    writer.Write(leafNodes[i].sup.X);
                    writer.Write(leafNodes[i].sup.Y);
                    writer.Write(leafNodes[i].sup.Z);
                    writer.Write(leafNodes[i].inf.X);
                    writer.Write(leafNodes[i].inf.Y);
                    writer.Write(leafNodes[i].inf.Z);
                }

                writer.Write(stripVecList?.Length ?? 0);
                for (int i = 0; i < stripVecList?.Length; i++)
                {
                    writer.Write(stripVecList[i].X);
                    writer.Write(stripVecList[i].Y);
                    writer.Write(stripVecList[i].Z);
                }

                if (VertDataFlags != 0)
                    writer.Write(VertData);
            }

            var endian = writer.endianness;
            writer.endianness = Endianness.Little;

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = fileStart;

            writer.Write((int)RenderWareFile.Section.HI_TAGS_BEEF02);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
            writer.endianness = endian;
        }

        #region JSPNodeTreeSplit

        private const float MAX_SPLIT = 1.0E38f;
        private const float MIN_SPLIT = -1.0E38f;

        private TempSplit[] tempSplits;
        private int currSplitIndex = 0;

        private class TempSplit
        {
            public TempSplitChild leftChild { get; set; }
            public TempSplitChild rightChild { get; set; }
            public int leftCount { get; set; }
            public int rightCount { get; set; }
            public int axis { get; set; }
            public float leftValue { get; set; }
            public float rightValue { get; set; }

            public TempSplit()
            {
                leftChild = new TempSplitChild();
                rightChild = new TempSplitChild();
            }
        }

        private class TempSplitChild
        {
            public int node { get; set; }
            public int type { get; set; }

            public TempSplitChild() { }
        }

        private float GetSplitVolumeSum(xJSPNodeTreeLeaf[] tleaf, int tleafIndex, int count, int axis, float coord)
        {
            int numLeft = 0;
            int numRight = 0;
            BoundingBox leftbox = new();
            BoundingBox rightbox = new();

            for (int i = 0; i < count; i++)
            {
                if (tleaf[tleafIndex + i].inf[axis] < coord)
                {
                    if (numLeft == 0)
                        leftbox = new BoundingBox(tleaf[tleafIndex + i].inf, tleaf[tleafIndex + i].sup);
                    else
                    {
                        if (tleaf[tleafIndex + i].inf.X < leftbox.Minimum.X)
                            leftbox.Minimum.X = tleaf[tleafIndex + i].inf.X;
                        if (tleaf[tleafIndex + i].inf.Y < leftbox.Minimum.Y)
                            leftbox.Minimum.Y = tleaf[tleafIndex + i].inf.Y;
                        if (tleaf[tleafIndex + i].inf.Z < leftbox.Minimum.Z)
                            leftbox.Minimum.Z = tleaf[tleafIndex + i].inf.Z;
                        if (leftbox.Maximum.X <= tleaf[tleafIndex + i].sup.X)
                            leftbox.Maximum.X = tleaf[tleafIndex + i].sup.X;
                        if (leftbox.Maximum.Y <= tleaf[tleafIndex + i].sup.Y)
                            leftbox.Maximum.Y = tleaf[tleafIndex + i].sup.Y;
                        if (leftbox.Maximum.Z <= tleaf[tleafIndex + i].sup.Z)
                            leftbox.Maximum.Z = tleaf[tleafIndex + i].sup.Z;
                    }
                    numLeft++;
                }

                if (coord < tleaf[tleafIndex + i].sup[axis] || coord == tleaf[tleafIndex + i].inf[axis])
                {
                    if (numRight == 0)
                        rightbox = new BoundingBox(tleaf[tleafIndex + i].inf, tleaf[tleafIndex + i].sup);
                    else
                    {
                        if (tleaf[tleafIndex + i].inf.X < rightbox.Minimum.X)
                            rightbox.Minimum.X = tleaf[tleafIndex + i].inf.X;
                        if (tleaf[tleafIndex + i].inf.Y < rightbox.Minimum.Y)
                            rightbox.Minimum.Y = tleaf[tleafIndex + i].inf.Y;
                        if (tleaf[tleafIndex + i].inf.Z < rightbox.Minimum.Z)
                            rightbox.Minimum.Z = tleaf[tleafIndex + i].inf.Z;
                        if (rightbox.Maximum.X <= tleaf[tleafIndex + i].sup.X)
                            rightbox.Maximum.X = tleaf[tleafIndex + i].sup.X;
                        if (rightbox.Maximum.Y <= tleaf[tleafIndex + i].sup.Y)
                            rightbox.Maximum.Y = tleaf[tleafIndex + i].sup.Y;
                        if (rightbox.Maximum.Z <= tleaf[tleafIndex + i].sup.Z)
                            rightbox.Maximum.Z = tleaf[tleafIndex + i].sup.Z;
                    }
                    numRight++;
                }
            }

            if (numLeft != count && numRight != count)
                return (leftbox.Size.X * leftbox.Size.Y * leftbox.Size.Z) + (rightbox.Size.X * rightbox.Size.Y * rightbox.Size.Z);
            else
                return MAX_SPLIT;
        }

        private bool ChooseSplit_MeanMinSum(xJSPNodeTreeLeaf[] tleaf, int count, int tleafIndex, out int bestaxis, out float bestcoord)
        {
            Vector3 mean = new();
            float bestvolume = MAX_SPLIT;
            float[] testarray = new float[3];
            bestaxis = 0;
            bestcoord = 0;

            for (int i = 0; i < count; i++)
                mean += tleaf[tleafIndex + i].inf + tleaf[tleafIndex + i].sup;
            mean /= count << 1;

            for (int axis = 0; axis < 3; axis++)
            {
                testarray[1] = mean[axis];
                testarray[0] = MIN_SPLIT;
                testarray[2] = MAX_SPLIT;

                for (int i = 0; i < count; i++)
                {
                    float coord = tleaf[tleafIndex + i].inf[axis];
                    if (coord <= testarray[1] && testarray[0] < coord)
                        testarray[0] = coord;
                    if (coord < testarray[2] && testarray[1] <= coord)
                        testarray[2] = coord;
                }

                for (int i = 0; i < 3; i++)
                {
                    float coord = GetSplitVolumeSum(tleaf, tleafIndex, count, axis, testarray[i]);
                    if (coord < bestvolume)
                    {
                        bestaxis = axis;
                        bestcoord = testarray[i];
                        bestvolume = coord;
                    }
                }
            }

            return MAX_SPLIT == bestvolume;
        }

        private void RecurseSplitJSPNode(xJSPNodeTreeLeaf[] tleaf, int count, TempSplitChild child, int tleafIndex)
        {
            if (count < 2 || ChooseSplit_MeanMinSum(tleaf, count, tleafIndex, out int bestaxis, out float bestcoord))
            {
                if (child != null)
                {
                    child.node = tleafIndex;
                    child.type = 1;
                }
                tleaf[tleafIndex].leafCount = count;
            }
            else
            {
                int lastright = count;
                for (int i = 0; i < lastright; i++)
                {
                    if (bestcoord <= 0.5 * (tleaf[tleafIndex + i].inf[bestaxis] + tleaf[tleafIndex + i].sup[bestaxis]))
                    {
                        lastright--;
                        xJSPNodeTreeLeaf templeaf = tleaf[tleafIndex + i];
                        tleaf[tleafIndex + i] = tleaf[tleafIndex + lastright];
                        tleaf[tleafIndex + lastright] = templeaf;
                        i--;
                    }
                }

                if (lastright == count || lastright == 0)
                {
                    if (child != null)
                    {
                        child.node = tleafIndex;
                        child.type = 1;
                    }
                    tleaf[tleafIndex].leafCount = count;
                }
                else
                {
                    tempSplits[currSplitIndex].axis = bestaxis;
                    tempSplits[currSplitIndex].rightCount = count - lastright;
                    tempSplits[currSplitIndex].leftCount = lastright;
                    tempSplits[currSplitIndex].rightValue = MAX_SPLIT;
                    tempSplits[currSplitIndex].leftValue = MIN_SPLIT;

                    for (int i = 0; i < lastright; i++)
                    {
                        if (tempSplits[currSplitIndex].leftValue <= tleaf[tleafIndex + i].sup[bestaxis])
                            tempSplits[currSplitIndex].leftValue = tleaf[tleafIndex + i].sup[bestaxis];
                    }

                    for (int i = lastright; i < count; i++)
                    {
                        if (tleaf[tleafIndex + i].inf[bestaxis] <= tempSplits[currSplitIndex].rightValue)
                            tempSplits[currSplitIndex].rightValue = tleaf[tleafIndex + i].inf[bestaxis];
                    }

                    if (child != null)
                    {
                        child.node = currSplitIndex;
                        child.type = 2;
                    }

                    int splitIndex = currSplitIndex++;
                    RecurseSplitJSPNode(tleaf, lastright, tempSplits[splitIndex].leftChild, tleafIndex);
                    RecurseSplitJSPNode(tleaf, count - lastright, tempSplits[splitIndex].rightChild, tleafIndex + lastright);
                }
            }
        }

        public void xJSPNodeTreeBuild(params Clump_0010[] clumps)
        {
            currSplitIndex = 0;
            xJSPNodeTreeLeaf[] tleaf = new xJSPNodeTreeLeaf[clumps.Sum(c => c.atomicList.Count)];
            tempSplits = Enumerable.Range(0, tleaf.Length).Select(_ => new TempSplit()).ToArray();

            int atomCount = 0;
            foreach (var clump in clumps.Reverse())
            {
                foreach (var atomic in clump.atomicList.ToArray().Reverse())
                {
                    Geometry_000F geometry = clump.geometryList.geometryList[atomic.atomicStruct.geometryIndex];
                    NativeDataPLG_0510 nativePLG = geometry.geometryExtension.extensionSectionList.OfType<NativeDataPLG_0510>().FirstOrDefault();

                    BoundingBox bbox = new BoundingBox(new Vector3(MIN_SPLIT), new Vector3(MAX_SPLIT));
                    if (nativePLG != null)
                    {
                        switch (nativePLG.nativeDataStruct.nativeDataType)
                        {
                            case NativeDataType.GameCube:
                                bbox = BoundingBox.FromPoints(nativePLG.nativeDataStruct.nativeData.GetLinearVertices().Select(v => new Vector3(v.X, v.Y, v.Z)).Distinct().ToArray());
                                break;
                            case NativeDataType.PS2:
                                Vector3[] vertices = nativePLG.nativeDataStruct.nativeDataPs2.GetLinearVerticesList()?.Select(v => new Vector3(v.X, v.Y, v.Z)).Distinct().ToArray();
                                Vector3[] verticesFlag = nativePLG.nativeDataStruct.nativeDataPs2.GetLinearVerticesFlagList()?.Select(v => new Vector3(v.X, v.Y, v.Z)).Distinct().ToArray();
                                bbox = BoundingBox.FromPoints(vertices.Any() ? vertices : verticesFlag);
                                break;
                            default:
                                throw new Exception("Unknown native data platform");
                        }
                    }
                    else
                        bbox = BoundingBox.FromPoints(geometry.geometryStruct.morphTargets[0].vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());

                    tleaf[atomCount].nodeIndex = atomCount;
                    tleaf[atomCount].sup = bbox.Maximum;
                    tleaf[atomCount].inf = bbox.Minimum;

                    atomCount++;
                }
            }
            RecurseSplitJSPNode(tleaf, atomCount, null, 0);

            branchNodes = new xJSPNodeTreeBranch[currSplitIndex];
            for (int i = 0; i < currSplitIndex; i++)
            {
                branchNodes[i].leftNode = (ushort)tempSplits[i].leftChild.node;
                branchNodes[i].leftType = (byte)tempSplits[i].leftChild.type;
                branchNodes[i].rightNode = (ushort)tempSplits[i].rightChild.node;
                branchNodes[i].rightType = (byte)tempSplits[i].rightChild.type;
                branchNodes[i].coord = (ushort)(tempSplits[i].axis * 4);
                branchNodes[i].leftValue = tempSplits[i].leftValue;
                branchNodes[i].rightValue = tempSplits[i].rightValue;
            }
            leafNodes = tleaf;
        }

        #endregion
    }
}

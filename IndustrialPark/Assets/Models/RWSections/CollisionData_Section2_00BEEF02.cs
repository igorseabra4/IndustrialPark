using HipHopFile;
using RenderWareFile;
using System.Numerics;

namespace IndustrialPark
{
    public struct xJSPNodeInfo
    {
        public int originalMatIndex { get; set; }
        public ushort nodeFlags { get; set; }
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
    }

    public class CollisionData_Section2_00BEEF02 : GenericAssetDataContainer
    {
        public int RenderWareVersion;
        public Platform platform;

        public string JSP { get; set; }
        public int version { get; set; }
        public xJSPNodeInfo[] jspNodeList { get; set; }
        public xJSPNodeTreeBranch[] branchNodes { get; set; }
        public xJSPNodeTreeLeaf[] leafNodes { get; set; }
        public Vertex3[] stripVecList { get; set; }
        public ushort VertDataFlags { get; set; }
        public ushort VertDataStride { get; set; }
        public byte[] UnknownVertData { get; set; }

        public CollisionData_Section2_00BEEF02(EndianBinaryReader reader, Platform platform)
        {
            this.platform = platform;

            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();

            JSP = new string(reader.ReadChars(4));

            if (platform == Platform.GameCube)
                reader.endianness = Endianness.Big;

            version = reader.ReadInt32();
            int jspNodeCount = reader.ReadInt32();
            if (version == 3)
                reader.BaseStream.Position += 12;
            else if (version == 5)
            {
                reader.BaseStream.Position += 20;
                VertDataFlags = reader.ReadUInt16();
                VertDataStride = reader.ReadUInt16();
                reader.BaseStream.Position += 8;
            }


            jspNodeList = new xJSPNodeInfo[jspNodeCount];
            for (int i = 0; i < jspNodeCount; i++)
                if (version == 3)
                {
                    jspNodeList[i] = new xJSPNodeInfo()
                    {
                        originalMatIndex = reader.ReadInt32(),
                        nodeFlags = (ushort)reader.ReadInt32(),
                    };
                }
                else if (version == 5)
                {
                    jspNodeList[i] = new xJSPNodeInfo()
                    {
                        originalMatIndex = reader.ReadInt32(),
                        nodeFlags = reader.ReadUInt16(),
                        sortOrder = reader.ReadInt16(),
                    };
                }

            if (version == 5)
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
                        leafCount = reader.ReadInt32(),
                        sup = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                        inf = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                    };
                }

                int stripVecCount = reader.ReadInt32();
                stripVecList = new Vertex3[stripVecCount];
                for (int i = 0; i < stripVecCount; i++)
                    stripVecList[i] = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                if (VertDataFlags != 0)
                    UnknownVertData = reader.ReadBytes(VertDataStride * stripVecCount);
            }

        }

        public CollisionData_Section2_00BEEF02(Platform platform)
        {
            this.platform = platform;

            JSP = "JSP\0";
            version = 3;
            jspNodeList = new xJSPNodeInfo[0];
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            for (int i = 0; i < 4; i++)
                writer.Write((byte)(i < JSP.Length ? JSP[i] : 0));

            if (platform == Platform.GameCube)
                writer.endianness = Endianness.Big;

            writer.Write(version);
            writer.Write(jspNodeList.Length);

            if (version == 3)
                writer.Write(new byte[12]);
            else if (version == 5)
            {
                writer.Write(new byte[20]);
                writer.Write(VertDataFlags);
                writer.Write(VertDataStride);
                writer.Write(new byte[8]);
            }


            for (int i = 0; i < jspNodeList.Length; i++)
            {
                writer.Write(jspNodeList[i].originalMatIndex);
                if (version == 3)
                    writer.Write((int)jspNodeList[i].nodeFlags);
                else if (version == 5)
                {
                    writer.Write(jspNodeList[i].nodeFlags);
                    writer.Write(jspNodeList[i].sortOrder);
                }
            }

            if (version == 5)
            {
                writer.Write(branchNodes.Length);
                writer.Write(0);
                writer.Write(leafNodes.Length);
                writer.Write(0);

                foreach (var branch in branchNodes)
                {
                    writer.Write(branch.leftNode);
                    writer.Write(branch.rightNode);
                    writer.Write(branch.leftType);
                    writer.Write(branch.rightType);
                    writer.Write(branch.coord);
                    writer.Write(branch.leftValue);
                    writer.Write(branch.rightValue);
                }

                foreach (var leaf in leafNodes)
                {
                    writer.Write(leaf.nodeIndex);
                    writer.Write(leaf.leafCount);
                    writer.Write(leaf.sup.X);
                    writer.Write(leaf.sup.Y);
                    writer.Write(leaf.sup.Z);
                    writer.Write(leaf.inf.X);
                    writer.Write(leaf.inf.Y);
                    writer.Write(leaf.inf.Z);
                }

                writer.Write(stripVecList.Length);
                foreach (var vec in stripVecList)
                {
                    writer.Write(vec.X);
                    writer.Write(vec.Y);
                    writer.Write(vec.Z);
                }

                if (VertDataFlags != 0)
                    writer.Write(UnknownVertData);
            }

            writer.endianness = Endianness.Little;

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = fileStart;

            writer.Write((int)RenderWareFile.Section.BFBB_CollisionData_Section2);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
        }
    }
}

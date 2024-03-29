using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum ClumpCollType
    {
        Null = 0,
        Leaf = 1,
        Branch = 2
    }

    public enum ClumpDirection
    {
        X = 0,
        Y = 1,
        Z = 2,
        Unknown = 3
    }

    public struct xClumpCollBSPBranchNode
    {
        private static (int, ClumpCollType, ClumpDirection, int) UnpackInfo(int info) =>
            (info >> 12,
            (ClumpCollType)(info & 0b11),
            (ClumpDirection)((info & 0b1100) >> 2),
            (info & 0b11110000) >> 4);

        private static int PackInfo(int index, ClumpCollType type, ClumpDirection unk1, int unk2) =>
            (index << 12) |
            (((int)type) & 0b11) |
            ((((int)unk1) & 0b11) << 2) |
            ((unk2 & 0b1111) << 4);

        public int LeftListIndex { get; set; }
        public ClumpCollType LeftType { get; set; }
        public ClumpDirection LeftDirection { get; set; }
        public int LeftUnk { get; set; }

        [Browsable(false)]
        public int LeftInfo
        {
            get => PackInfo(LeftListIndex, LeftType, LeftDirection, LeftUnk);
            set
            {
                var info = UnpackInfo(value);
                LeftListIndex = info.Item1;
                LeftType = info.Item2;
                LeftDirection = info.Item3;
                LeftUnk = info.Item4;
            }
        }

        public int RightListIndex { get; set; }
        public ClumpCollType RightType { get; set; }
        public ClumpDirection RightDirection { get; set; }
        public int RightUnk { get; set; }

        [Browsable(false)]
        public int RightInfo
        {
            get => PackInfo(RightListIndex, RightType, RightDirection, RightUnk);
            set
            {
                var info = UnpackInfo(value);
                RightListIndex = info.Item1;
                RightType = info.Item2;
                RightDirection = info.Item3;
                RightUnk = info.Item4;
            }
        }

        public float LeftValue { get; set; }
        public float RightValue { get; set; }
    }

    public struct xClumpCollBSPTriangle
    {
        public short atomIndex { get; set; }
        public short meshVertIndex { get; set; }
        public byte flags { get; set; }
        public byte platData { get; set; }
        public short matIndex { get; set; }
    }

    public class CollisionData_Section1_00BEEF01 : GenericAssetDataContainer
    {
        public int RenderWareVersion;
        public Platform platform;

        public string CCOL { get; set; }
        public xClumpCollBSPBranchNode[] branchNodes { get; set; }
        public xClumpCollBSPTriangle[] triangles { get; set; }

        public CollisionData_Section1_00BEEF01(Platform platform)
        {
            this.platform = platform;

            CCOL = platform == Platform.GameCube ? "LOCC" : "CCOL";

            branchNodes = new xClumpCollBSPBranchNode[0];
            triangles = new xClumpCollBSPTriangle[0];
        }

        public CollisionData_Section1_00BEEF01(EndianBinaryReader reader, Platform platform)
        {
            this.platform = platform;

            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();

            CCOL = new string(reader.ReadChars(4));

            if (platform == Platform.GameCube)
                reader.endianness = Endianness.Big;

            int numBranchNodes = reader.ReadInt32();
            int numTriangles = reader.ReadInt32();

            branchNodes = new xClumpCollBSPBranchNode[numBranchNodes];
            for (int i = 0; i < numBranchNodes; i++)
            {
                branchNodes[i] = new xClumpCollBSPBranchNode()
                {
                    LeftInfo = reader.ReadInt32(),
                    RightInfo = reader.ReadInt32(),
                    LeftValue = reader.ReadSingle(),
                    RightValue = reader.ReadSingle()
                };
            }

            triangles = new xClumpCollBSPTriangle[numTriangles];
            for (int i = 0; i < numTriangles; i++)
            {
                triangles[i] = new xClumpCollBSPTriangle()
                {
                    atomIndex = reader.ReadInt16(),
                    meshVertIndex = reader.ReadInt16(),
                    flags = reader.ReadByte(),
                    platData = reader.ReadByte(),
                    matIndex = reader.ReadInt16()
                };
            }

            reader.endianness = Endianness.Little;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            for (int i = 0; i < 4; i++)
                writer.Write((byte)(i < CCOL.Length ? CCOL[i] : 0));

            if (platform == Platform.GameCube)
                writer.endianness = Endianness.Big;

            writer.Write(branchNodes.Length);
            writer.Write(triangles.Length);

            for (int i = 0; i < branchNodes.Length; i++)
            {
                writer.Write(branchNodes[i].LeftInfo);
                writer.Write(branchNodes[i].RightInfo);
                writer.Write(branchNodes[i].LeftValue);
                writer.Write(branchNodes[i].RightValue);
            }

            for (int i = 0; i < triangles.Length; i++)
            {
                writer.Write(triangles[i].atomIndex);
                writer.Write(triangles[i].meshVertIndex);
                writer.Write(triangles[i].flags);
                writer.Write(triangles[i].platData);
                writer.Write(triangles[i].matIndex);
            }

            writer.endianness = Endianness.Little;

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = fileStart;

            writer.Write((int)RenderWareFile.Section.BFBB_CollisionData_Section1);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
        }
    }
}

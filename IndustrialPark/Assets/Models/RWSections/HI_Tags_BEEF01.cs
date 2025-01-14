using Assimp;
using HipHopFile;
using RenderWareFile;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace IndustrialPark
{
    public enum ClumpCollType
    {
        Null = 0,
        Leaf = 1,
        Branch = 2
    }

    public enum ClumpAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
        Unknown = 3
    }

    public struct xClumpCollBSPBranchNode
    {
        private static (int, ClumpCollType, ClumpAxis, int) UnpackInfo(int info) =>
            (info >> 12,
            (ClumpCollType)(info & 0b11),
            (ClumpAxis)((info & 0b1100) >> 2),
            (info & 0b11110000) >> 4);

        private static int PackInfo(int index, ClumpCollType type, ClumpAxis unk1, int unk2) =>
            (index << 12) | 
            (((int)type) & 0b11) | 
            ((((int)unk1) & 0b11) << 2) |
            ((unk2 & 0b1111) << 4);

        public int LeftIndex { get; set; }
        public ClumpCollType LeftType { get; set; }
        public ClumpAxis LeftAxis { get; set; }
        public int LeftUnk { get; set; } // Unused

        [Browsable(false)]
        public int LeftInfo
        {
            get => PackInfo(LeftIndex, LeftType, LeftAxis, LeftUnk);
            set
            {
                var info = UnpackInfo(value);
                LeftIndex = info.Item1;
                LeftType = info.Item2;
                LeftAxis = info.Item3;
                LeftUnk = info.Item4;
            }
        }

        public int RightIndex { get; set; }
        public ClumpCollType RightType { get; set; }
        public ClumpAxis RightAxis { get; set; } // Unused
        public int RightUnk { get; set; } // Unused

        [Browsable(false)]
        public int RightInfo
        {
            get => PackInfo(RightIndex, RightType, RightAxis, RightUnk);
            set
            {
                var info = UnpackInfo(value);
                RightIndex = info.Item1;
                RightType = info.Item2;
                RightAxis = info.Item3;
                RightUnk = info.Item4;
            }
        }

        public float LeftValue { get; set; }
        public float RightValue { get; set; }
    }

    [Flags]
    public enum ClumpCollBSPTriangleFlags : byte
    {
        /// <summary>
        /// There is another triangle after this one in memory. If not set means this is the last triangle in its chain
        /// </summary>
        kCLUMPCOLL_HASNEXT = 0x1,
        /// <summary>
        /// This triangle is in reverse orientation (every other triangle in a tristrip mesh)
        /// </summary>
        kCLUMPCOLL_ISREVERSE = 0x2,
        /// <summary>
        /// This triangle has collison enabled
        /// </summary>
        kCLUMPCOLL_ISSOLID = 0x4,
        /// <summary>
        /// This triangle is visible in the world
        /// </summary>
        kCLUMPCOLL_ISVISIBLE = 0x8,
        /// <summary>
        /// Force the player to slide off this triangle instead of stand on it
        /// </summary>
        kCLUMPCOLL_NOSTAND = 0x10,
        /// <summary>
        /// Force this triangle to recieve shadows in the world
        /// </summary>
        kCLUMPCOLL_SHADOW = 0x20,
        /// <summary>
        /// Unknown flag used in tssm and beyond
        /// </summary>
        kCLUMPCOLL_UNKNOWN = 0x40,
    }

    public struct xClumpCollBSPTriangle
    {
        /// <summary>
        /// Atomic index in all JSP's combined. Only in BFBB present.
        /// </summary>
        public ushort atomIndex { get; set; }
        /// <summary>
        /// Vertex index in atomic's mesh. Only in BFBB present.
        /// </summary>
        public ushort meshVertIndex { get; set; }
        /// <summary>
        /// Vertex index in all clumps. Present in TSSM and beyond
        /// </summary>
        public int rawIdx { get; set; }
        /// <summary>
        /// Flags to set if a triangle is collidable, visible, etc.
        /// </summary>
        public ClumpCollBSPTriangleFlags flags { get; set; }
        /// <summary>
        /// "detailed_info_cache_index" in TSSM and beyond.
        /// </summary>
        public byte platData { get; set; }
        /// <summary>
        /// Material index into the geometry's material list
        /// </summary>
        public short matIndex { get; set; }

    }

    public class HI_Tags_BEEF01 : GenericAssetDataContainer
    {
        public int RenderWareVersion;

        public xClumpCollBSPBranchNode[] branchNodes { get; set; }
        public xClumpCollBSPTriangle[] triangles { get; set; }

        public HI_Tags_BEEF01()
        {
            branchNodes = new xClumpCollBSPBranchNode[0];
            triangles = new xClumpCollBSPTriangle[0];
            RenderWareVersion = 0x1400FFFF;
        }

        public HI_Tags_BEEF01(Game game) : this()
        {
            _game = game;
        }

        public HI_Tags_BEEF01(EndianBinaryReader reader, Game game) : this(game)
        {
            reader.endianness = Endianness.Little;
            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();

            if (reader.ReadString(4) == "LOCC")
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
                triangles[i] = new xClumpCollBSPTriangle();

                if (game == Game.BFBB)
                {
                    triangles[i].atomIndex = reader.ReadUInt16();
                    triangles[i].meshVertIndex = reader.ReadUInt16();
                }
                else
                    triangles[i].rawIdx = reader.ReadInt32();
                triangles[i].flags = (ClumpCollBSPTriangleFlags)reader.ReadByte();
                triangles[i].platData = reader.ReadByte();
                triangles[i].matIndex = reader.ReadInt16();
            }
        }

        public void RemoveTSSMFlags()
        {
            for (int i = 0; i < triangles.Length; i++)
            {
                ref xClumpCollBSPTriangle tri = ref triangles[i];
                tri.flags &= (ClumpCollBSPTriangleFlags)0x3F;
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;
            writer.Write(new byte[12]);

            writer.WriteMagic("CCOL");
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
                if (game == Game.BFBB)
                {
                    writer.Write(triangles[i].atomIndex);
                    writer.Write(triangles[i].meshVertIndex);
                }
                else
                    writer.Write(triangles[i].rawIdx);
                writer.Write((byte)triangles[i].flags);
                writer.Write(triangles[i].platData);
                writer.Write(triangles[i].matIndex);
            }

            var endian = writer.endianness;
            writer.endianness = Endianness.Little;

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = fileStart;

            writer.Write((int)RenderWareFile.Section.HI_TAGS_BEEF01);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
            writer.endianness = endian;
        }
    }
}

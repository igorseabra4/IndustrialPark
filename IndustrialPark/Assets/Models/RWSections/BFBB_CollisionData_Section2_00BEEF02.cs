using HipHopFile;

namespace IndustrialPark
{
    public struct xJSPNodeInfo
    {
        public int originalMatIndex { get; set; }
        public int nodeFlags { get; set; }
    }

    public class BFBB_CollisionData_Section2_00BEEF02 : GenericAssetDataContainer
    {
        public int RenderWareVersion;
        public Platform platform;

        public string JSP { get; set; }
        public int version { get; set; }
        public int null1 { get; set; }
        public int null2 { get; set; }
        public int null3 { get; set; }
        public xJSPNodeInfo[] jspNodeList { get; set; }

        public BFBB_CollisionData_Section2_00BEEF02(EndianBinaryReader reader, Platform platform)
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
            null1 = reader.ReadInt32();
            null2 = reader.ReadInt32();
            null3 = reader.ReadInt32();

            jspNodeList = new xJSPNodeInfo[jspNodeCount];
            for (int i = 0; i < jspNodeCount; i++)
                jspNodeList[i] = new xJSPNodeInfo()
                {
                    originalMatIndex = reader.ReadInt32(),
                    nodeFlags = reader.ReadInt32()
                };
        }

        public BFBB_CollisionData_Section2_00BEEF02(Platform platform)
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
            writer.Write(null1);
            writer.Write(null2);
            writer.Write(null3);

            for (int i = 0; i < jspNodeList.Length; i++)
            {
                writer.Write(jspNodeList[i].originalMatIndex);
                writer.Write(jspNodeList[i].nodeFlags);
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

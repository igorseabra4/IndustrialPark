using RenderWareFile;
using RenderWareFile.Sections;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class CollisionData_Section3_00BEEF03 : GenericAssetDataContainer
    {
        public int RenderWareVersion;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Vertex3[] vertexList { get; set; }

        public CollisionData_Section3_00BEEF03(EndianBinaryReader reader)
        {
            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();

            int vCount = reader.ReadInt32();
            vertexList = new Vertex3[vCount];
            for (int i = 0; i < vCount; i++)
                vertexList[i] = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public CollisionData_Section3_00BEEF03(List<Geometry_000F> geometries)
        {
            var vertexList = new List<Vertex3>();
            foreach (var geom in geometries)
                if (geom.geometryStruct.morphTargets != null)
                    vertexList.AddRange(geom.geometryStruct.morphTargets[0].vertices);
            this.vertexList = vertexList.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;

            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            writer.endianness = Endianness.Big;

            writer.Write(vertexList.Length);
            foreach (Vertex3 v in vertexList)
            {
                writer.Write(v.X);
                writer.Write(v.Y);
                writer.Write(v.Z);
            }

            writer.endianness = Endianness.Little;

            var fileEnd = writer.BaseStream.Position;

            writer.BaseStream.Position = fileStart;

            writer.Write((int)Section.BFBB_CollisionData_Section3);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
        }
    }
}

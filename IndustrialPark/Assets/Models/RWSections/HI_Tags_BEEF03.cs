using RenderWareFile;
using RenderWareFile.Sections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class HI_Tags_BEEF03 : GenericAssetDataContainer
    {
        public int RenderWareVersion;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Vertex3[] vertexList { get; set; }

        public HI_Tags_BEEF03(EndianBinaryReader reader)
        {
            reader.endianness = Endianness.Little;
            reader.ReadInt32();
            reader.ReadInt32();
            RenderWareVersion = reader.ReadInt32();

            reader.endianness = Endianness.Big;

            int vCount = reader.ReadInt32();
            vertexList = new Vertex3[vCount];
            for (int i = 0; i < vCount; i++)
                vertexList[i] = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public HI_Tags_BEEF03(params Clump_0010[] clumps)
        {
            List<Vertex3> vertices = new();
            foreach (var clump in clumps.ToArray().Reverse())
                foreach (var geo in clump.geometryList.geometryList.ToArray().Reverse())
                    foreach (var binmeshplg in geo.geometryExtension.extensionSectionList.OfType<BinMeshPLG_050E>())
                        foreach (var binmesh in binmeshplg.binMeshList)
                            foreach (var i in binmesh.vertexIndices)
                                vertices.Add(geo.geometryStruct.morphTargets[0].vertices[i]);
            this.vertexList = vertices.ToArray();
        }

        public HI_Tags_BEEF03() { }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var fileStart = writer.BaseStream.Position;
            var endian = writer.endianness;
            writer.Write(new byte[12]);

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

            writer.Write((int)Section.HI_TAGS_BEEF03);
            writer.Write((uint)(fileEnd - fileStart - 0xC));
            writer.Write(RenderWareVersion);

            writer.BaseStream.Position = fileEnd;
            writer.endianness = endian;
        }
    }
}

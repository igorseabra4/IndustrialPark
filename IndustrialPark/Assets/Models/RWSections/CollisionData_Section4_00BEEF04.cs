using RenderWareFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{
    public class CollisionData_Section4_00BEEF04 : GenericAssetDataContainer
    {
        public int RenderWareVersion;

        private string JSPX { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Vertex3 BoundsUpper { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Vertex3 BoundsLower { get; set; }

        public CollisionData_Section4_00BEEF04(EndianBinaryReader reader)
        {
            reader.BaseStream.Position += 8;
            RenderWareVersion = reader.ReadInt32();

            JSPX = new string(reader.ReadChars(4));
            BoundsUpper = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            BoundsLower = new Vertex3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.endianness = Endianness.Little;
            writer.Write(0x00BEEF04);
            writer.Write(0x1C);
            writer.Write(RenderWareVersion);

            writer.endianness = Endianness.Big;
            writer.Write(JSPX);
            writer.Write(BoundsUpper.X);
            writer.Write(BoundsUpper.Y);
            writer.Write(BoundsUpper.Z);
            writer.Write(BoundsLower.X);
            writer.Write(BoundsLower.Y);
            writer.Write(BoundsLower.Z);
        }
    }
}

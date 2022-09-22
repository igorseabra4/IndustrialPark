using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class AssetGenericBase : BaseAsset
    {
        private const string categoryName = "Generic Base Asset";

        private Endianness endianness;

        [Category(categoryName)]
        public byte[] Data { get; set; }

        public AssetGenericBase(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            Data = AHDR.data.Skip(baseHeaderEndPosition).Take(AHDR.data.Length - baseHeaderEndPosition - _links.Length * Link.sizeOfStruct).ToArray();
            this.endianness = endianness;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Data);
            SerializeLinks(writer);
        }

        [Category(categoryName)]
        public AssetID[] Data_AsHex
        {
            get
            {
                var values = new List<AssetID>();
                var reader = new EndianBinaryReader(Data, endianness);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadUInt32());
                return values.ToArray();
            }
            set
            {
                using (var writer = new EndianBinaryWriter(endianness))
                {
                    foreach (var f in value)
                        writer.Write(f);
                    Data = writer.ToArray();
                }
            }
        }

        [Category(categoryName)]
        public AssetSingle[] Data_AsFloat
        {
            get
            {
                var values = new List<AssetSingle>();
                var reader = new EndianBinaryReader(Data, endianness);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadSingle());
                return values.ToArray();
            }
            set
            {
                using (var writer = new EndianBinaryWriter(endianness))
                {
                    foreach (var f in value)
                        writer.Write(f);
                    Data = writer.ToArray();
                }
            }
        }
    }
}
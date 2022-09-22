using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetGeneric : Asset
    {
        private const string categoryName = "Generic Asset";

        private Endianness endianness;

        [Category(categoryName)]
        public byte[] Data { get; set; }

        public AssetGeneric(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            Data = AHDR.data;
            this.endianness = endianness;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Data);
        }

        [Category(categoryName)]
        public AssetID[] Data_AsHex
        {
            get
            {
                try
                {
                    var values = new List<AssetID>();
                    var reader = new EndianBinaryReader(Data, endianness);
                    while (!reader.EndOfStream)
                        values.Add(reader.ReadUInt32());
                    return values.ToArray();
                }
                catch { return new AssetID[0]; }
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
                try
                {
                    var values = new List<AssetSingle>();
                    var reader = new EndianBinaryReader(Data, endianness);
                    while (!reader.EndOfStream)
                        values.Add(reader.ReadSingle());
                    return values.ToArray();
                }
                catch { return new AssetSingle[0]; }
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
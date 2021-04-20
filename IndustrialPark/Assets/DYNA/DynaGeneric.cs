using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class DynaGeneric : AssetDYNA
    {
        private const string dynaCategoryName = "Generic Dynamic";

        [Category(dynaCategoryName)]
        public byte[] Data { get; set; }

        [Category(dynaCategoryName)]
        public AssetID[] Data_AsHex
        {
            get
            {
                var values = new List<AssetID>();
                var reader = new EndianBinaryReader(Data, platform);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadUInt32());
                return values.ToArray();
            }
            set
            {
                var writer = new EndianBinaryWriter(platform);
                foreach (var f in value)
                    writer.Write(f);
                Data = writer.ToArray();
            }
        }

        [Category(dynaCategoryName)]
        public AssetSingle[] Data_AsFloat
        {
            get
            {
                var values = new List<AssetSingle>();
                var reader = new EndianBinaryReader(Data, platform);
                while (!reader.EndOfStream)
                    values.Add(reader.ReadSingle());
                return values.ToArray();
            }
            set
            {
                var writer = new EndianBinaryWriter(platform);
                foreach (var f in value)
                    writer.Write(f);
                Data = writer.ToArray();
            }
        }

        public DynaGeneric(Section_AHDR AHDR, DynaType type, Game game, Platform platform) : base(AHDR, type, game, platform)
        {
            Data = AHDR.data.Skip(dynaDataStartPosition).Take(AHDR.data.Length - dynaDataStartPosition - _links.Length * Link.sizeOfStruct).ToArray();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform) => Data;
    }
}
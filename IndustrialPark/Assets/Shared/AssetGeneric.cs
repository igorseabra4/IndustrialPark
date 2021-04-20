using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetGeneric : Asset
    {
        private const string categoryName = "Generic Asset";

        [Category(categoryName)]
        public byte[] Data { get; set; }

        public AssetGeneric(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            Data = AHDR.data;
        }

        public override byte[] Serialize(Game game, Platform platform) => Data;

        public override bool HasReference(uint assetID)
        {
            foreach (var u in Data_AsHex)
                if (u == assetID)
                    return true;

            return false;
        }

        [Category(categoryName)]
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

        [Category(categoryName)]
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
    }
}
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryMAPR : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Surface { get; set; }
        public AssetID Unknown { get; set; }

        public EntryMAPR() { }
        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Surface)}] - [{HexUIntTypeConverter.StringFromAssetID(Unknown)}]";
        }

        public override void Serialize(EndianBinaryWriter writer) { }
    }

    public class AssetMAPR : Asset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        [Category("Surface Mapper")]
        public EntryMAPR[] Entries { get; set; }

        public AssetMAPR(string assetName) : base(assetName, AssetType.SurfaceMapper)
        {
            Entries = new EntryMAPR[0];
        }

        public AssetMAPR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                int maprCount = reader.ReadInt32();
                Entries = new EntryMAPR[maprCount];

                for (int i = 0; i < Entries.Length; i++)
                    Entries[i] = new EntryMAPR()
                    {
                        Surface = reader.ReadUInt32(),
                        Unknown = reader.ReadUInt32()
                    };
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(assetID);
            writer.Write(Entries.Length);
            foreach (var entry in Entries)
            {
                writer.Write(entry.Surface);
                writer.Write(entry.Unknown);
            }
        }
    }
}
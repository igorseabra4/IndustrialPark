using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryMAPR : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Surface { get; set; }
        public int MeshIndex { get; set; }

        public EntryMAPR() { }

        public EntryMAPR(EndianBinaryReader reader)
        {
            Surface = reader.ReadUInt32();
            MeshIndex = reader.ReadInt32();
        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Surface)}] - {MeshIndex}]";
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Surface);
            writer.Write(MeshIndex);
        }
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
                    Entries[i] = new EntryMAPR(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(assetID);
            writer.Write(Entries.Length);
            foreach (var entry in Entries)
                entry.Serialize(writer);
        }
    }
}
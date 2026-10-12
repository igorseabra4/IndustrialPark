using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;

namespace IndustrialPark
{
    public class EntryMAPR : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Surface { get; set; }

        public AssetID MeshAsset { get => MeshIndex; set => MeshIndex = value; }

        public uint MeshIndex { get; set; }

        public EntryMAPR() { }

        public EntryMAPR(Game game)
        {
            _game = game;
        }

        public EntryMAPR(EndianBinaryReader reader, Game game) : this(game)
        {
            Surface = reader.ReadUInt32();
            MeshIndex = reader.ReadUInt32();
        }

        public override string ToString()
        {
            if (game == Game.BFBB)
                return $"[{HexUIntTypeConverter.StringFromAssetID(Surface)}] - {MeshIndex}";
            return $"[{HexUIntTypeConverter.StringFromAssetID(Surface)}] - {HexUIntTypeConverter.StringFromAssetID(MeshAsset)}";
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Surface);
            writer.Write(MeshIndex);
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.BFBB)
                dt.RemoveProperty("MeshAsset");
            else
                dt.RemoveProperty("MeshIndex");
        }
    }

    public class AssetMAPR : Asset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        [Category("Surface Mapper"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
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
                    Entries[i] = new EntryMAPR(reader, game);
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
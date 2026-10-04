using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class EntryCOLL : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        public AssetID CollisionModel { get; set; }
        public AssetID CameraCollisionModel { get; set; }

        public EntryCOLL() { }
        public EntryCOLL(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
            CollisionModel = reader.ReadUInt32();
            CameraCollisionModel = reader.ReadUInt32();
        }

        public override string ToString()
        {
            if (CollisionModel != 0)
                return $"[{HexUIntTypeConverter.StringFromAssetID(Model)}] - [{HexUIntTypeConverter.StringFromAssetID(CollisionModel)}]";
            return $"[{HexUIntTypeConverter.StringFromAssetID(Model)}] - [{HexUIntTypeConverter.StringFromAssetID(CameraCollisionModel)}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryCOLL entryCOLL)
                return Model.Equals(entryCOLL.Model);
            return false;
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Model);
            writer.Write(CollisionModel);
            writer.Write(CameraCollisionModel);
        }
    }

    public class AssetCOLL : Asset, IAssetAddSelected, IControllerAsset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private List<EntryCOLL> _entries;
        [Category("Collision Table"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
        public EntryCOLL[] Entries { get => [.. _entries]; set => _entries = [.. value]; }

        public AssetCOLL(string assetName) : base(assetName, AssetType.CollisionTable)
        {
            _entries = [];
        }

        public AssetCOLL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            var len = reader.ReadInt32();
            _entries = [with(len)];
            for (int i = 0; i < len; i++)
                _entries.Add(new EntryCOLL(reader));
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries.Length);

            foreach (var entry in Entries)
                entry.Serialize(writer);
        }

        public void Merge(AssetCOLL asset)
        {
            _entries.RemoveAll(e => asset._entries.Any(entry => e.Model == entry.Model));
            _entries.AddRange(asset._entries);
        }

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> items)
        {
            foreach (var i in items)
                AddEntry(new EntryCOLL() { Model = i });
        }

        public void AddEntry(EntryCOLL entry)
        {
            RemoveEntry(entry.Model);
            _entries.Add(entry);
        }

        public void RemoveEntry(uint assetID)
        {
            _entries.RemoveAll(e => e.Model == assetID);
        }
    }
}
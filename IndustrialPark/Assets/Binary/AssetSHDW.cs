using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class EntrySHDW : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        [ValidReferenceRequired]
        public AssetID ShadowModel { get; set; }
        public int Unknown { get; set; }

        public EntrySHDW() { }
        public EntrySHDW(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
            ShadowModel = reader.ReadUInt32();
            Unknown = reader.ReadInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Model);
            writer.Write(ShadowModel);
            writer.Write(Unknown);
        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Model)}] - [{HexUIntTypeConverter.StringFromAssetID(ShadowModel)}]";
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntrySHDW entrySHDW)
                return Model.Equals(entrySHDW.Model);
            return false;
        }
    }

    public class AssetSHDW : Asset, IAssetAddSelected, ITableAsset<AssetSHDW, EntrySHDW>
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private List<EntrySHDW> _entries;
        [Category("Shadow Map"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
        public EntrySHDW[] Entries { get => [.. _entries]; set => _entries = [.. value]; }

        public AssetSHDW(string assetName) : base(assetName, AssetType.ShadowTable)
        {
            _entries = [];
        }

        public AssetSHDW(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using var reader = new EndianBinaryReader(AHDR.data, endianness);
            var len = reader.ReadInt32();
            _entries = [with(len)];
            for (int i = 0; i < len; i++)
                _entries.Add(new EntrySHDW(reader));
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries.Length);

            foreach (var l in Entries)
                l.Serialize(writer);
        }

        public void Merge(AssetSHDW asset)
        {
            _entries.RemoveAll(e => asset._entries.Any(entry => e.Model == entry.Model));
            _entries.AddRange(asset._entries);
        }

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> items)
        {
            foreach (var i in items)
                AddEntry(new EntrySHDW() { Model = i });
        }

        public void AddEntry(EntrySHDW entry)
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
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public class EntryLODT : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID BaseModel { get; set; }
        public AssetSingle MaxDistance { get; set; }
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        public AssetID LOD1_Model { get; set; }
        public AssetSingle LOD1_MinDistance { get; set; }
        public AssetID LOD2_Model { get; set; }
        public AssetSingle LOD2_MinDistance { get; set; }
        public AssetID LOD3_Model { get; set; }
        public AssetSingle LOD3_MinDistance { get; set; }

        public EntryLODT() { }
        public EntryLODT(EndianBinaryReader reader, Game game)
        {
            _game = game;

            BaseModel = reader.ReadUInt32();
            MaxDistance = reader.ReadSingle();
            if (game >= Game.Incredibles)
                Flags.FlagValueInt = reader.ReadUInt32();
            LOD1_Model = reader.ReadUInt32();
            LOD2_Model = reader.ReadUInt32();
            LOD3_Model = reader.ReadUInt32();
            LOD1_MinDistance = reader.ReadSingle();
            LOD2_MinDistance = reader.ReadSingle();
            LOD3_MinDistance = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(BaseModel);
            writer.Write(MaxDistance);
            if (game >= Game.Incredibles)
                writer.Write(Flags.FlagValueInt);
            writer.Write(LOD1_Model);
            writer.Write(LOD2_Model);
            writer.Write(LOD3_Model);
            writer.Write(LOD1_MinDistance);
            writer.Write(LOD2_MinDistance);
            writer.Write(LOD3_MinDistance);
        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(BaseModel)}] - {MaxDistance}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryLODT entry)
                return BaseModel.Equals(entry.BaseModel);
            return false;
        }

        public override int GetHashCode()
        {
            return BaseModel.GetHashCode();
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.Incredibles)
                dt.RemoveProperty("Flags");
        }
    }

    public class AssetLODT : Asset, IAssetAddSelected
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private static readonly Dictionary<uint, float> maxDistances = new Dictionary<uint, float>();

        public static float MaxDistanceTo(uint _model) => maxDistances.ContainsKey(_model) ? maxDistances[_model] : SharpRenderer.DefaultLODTDistance;

        private EntryLODT[] _entries;
        [Category("Level Of Detail Table"), Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public EntryLODT[] Entries
        {
            get => _entries;
            set
            {
                _entries = value;
                UpdateDictionary();
            }
        }

        public AssetLODT(string assetName) : base(assetName, AssetType.LevelOfDetailTable)
        {
            Entries = new EntryLODT[0];
        }

        public AssetLODT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                _entries = new EntryLODT[reader.ReadInt32()];

                for (int i = 0; i < _entries.Length; i++)
                    _entries[i] = new EntryLODT(reader, game);

                UpdateDictionary();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Entries.Length);

            foreach (var l in Entries)
                l.Serialize(writer);
        }

        public void UpdateDictionary()
        {
            foreach (var entry in Entries)
                maxDistances[entry.BaseModel] = entry.MaxDistance;
        }

        public void ClearDictionary()
        {
            foreach (var entry in Entries)
                maxDistances.Remove(entry.BaseModel);
        }

        public void Merge(AssetLODT asset)
        {
            var entries = Entries.ToList();

            foreach (var entry in asset.Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            Entries = entries.ToArray();
        }

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> items)
        {
            var entries = Entries.ToList();
            foreach (var i in items)
                if (!entries.Any(e => e.BaseModel == i))
                    entries.Add(new EntryLODT() { BaseModel = i, MaxDistance = 100f });
            Entries = entries.ToArray();
        }

        public void AddEntry(EntryLODT entry)
        {
            var entries = Entries.ToList();
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].BaseModel == entry.BaseModel)
                {
                    entries[i] = entry;
                    Entries = entries.ToArray();
                    return;
                }
            entries.Add(entry);
            Entries = entries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            var entries = Entries.ToList();
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].BaseModel == assetID)
                    entries.RemoveAt(i--);
            Entries = entries.ToArray();
        }
    }
}
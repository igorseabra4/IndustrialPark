using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class AssetSHDW : Asset, IAssetAddSelected
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        [Category("Shadow Map")]
        public EntrySHDW[] Entries { get; set; }

        public AssetSHDW(string assetName) : base(assetName, AssetType.ShadowTable)
        {
            Entries = new EntrySHDW[0];
        }

        public AssetSHDW(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                Entries = new EntrySHDW[reader.ReadInt32()];

                for (int i = 0; i < Entries.Length; i++)
                    Entries[i] = new EntrySHDW(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

                writer.Write(Entries.Length);

                foreach (var l in Entries)
                    l.Serialize(writer);

                
        }

        public void Merge(AssetSHDW asset)
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
                if (!entries.Any(e => e.Model == i))
                    entries.Add(new EntrySHDW() { Model = i });
            Entries = entries.ToArray();
        }
    }
}
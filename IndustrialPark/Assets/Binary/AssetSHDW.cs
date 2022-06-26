using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntrySHDW : GenericAssetDataContainer
    {
        public AssetID Model { get; set; }
        public AssetID ShadowModel { get; set; }
        public int Unknown { get; set; }

        public EntrySHDW() { }
        public EntrySHDW(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
            ShadowModel = reader.ReadUInt32();
            Unknown = reader.ReadInt32();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Model);
                writer.Write(ShadowModel);
                writer.Write(Unknown);

                return writer.ToArray();
            }
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

        public AssetSHDW(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                Entries = new EntrySHDW[reader.ReadInt32()];

                for (int i = 0; i < Entries.Length; i++)
                    Entries[i] = new EntrySHDW(reader);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Entries.Length);

                foreach (var l in Entries)
                    writer.Write(l.Serialize(endianness));

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySHDW a in Entries)
            {
                if (a.Model == 0)
                    result.Add("Shadow table entry with Model set to 0");
                Verify(a.Model, ref result);
                if (a.ShadowModel == 0)
                    result.Add("Shadow table entry with ShadowModel set to 0");
                Verify(a.ShadowModel, ref result);
            }
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
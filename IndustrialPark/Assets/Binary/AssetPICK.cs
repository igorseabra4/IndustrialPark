using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryPICK : GenericAssetDataContainer
    {
        public AssetID PickupHash { get; set; }
        public AssetByte PickupType { get; set; }
        public AssetByte PickupIndex { get; set; }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort PickupFlags { get; set; }
        public uint Quantity { get; set; }
        public AssetID Model { get; set; }
        public AssetID Animation { get; set; }

        public EntryPICK() { }
        public EntryPICK(EndianBinaryReader reader)
        {
            PickupHash = reader.ReadUInt32();
            PickupType = reader.ReadByte();
            PickupIndex = reader.ReadByte();
            PickupFlags = reader.ReadUInt16();
            Quantity = reader.ReadUInt32();
            Model = reader.ReadUInt32();
            Animation = reader.ReadUInt32();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(PickupHash);
                writer.Write(PickupType);
                writer.Write(PickupIndex);
                writer.Write(PickupFlags);
                writer.Write(Quantity);
                writer.Write(Model);
                writer.Write(Animation);

                return writer.ToArray();
            }
        }

        public override string ToString() =>
            $"[{Program.MainForm.GetAssetNameFromID(PickupHash)}] - [{Program.MainForm.GetAssetNameFromID(Model)}]";
    }

    public class AssetPICK : Asset
    {
        public static Dictionary<uint, uint> pickEntries = new Dictionary<uint, uint>();

        private EntryPICK[] _pick_Entries;
        [Category("Pickup Table")]
        public EntryPICK[] PICK_Entries
        {
            get => _pick_Entries;
            set
            {
                _pick_Entries = value;
                UpdateDictionary();
            }
        }

        public AssetPICK(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                _pick_Entries = new EntryPICK[reader.ReadInt32()];
                for (int i = 0; i < _pick_Entries.Length; i++)
                    _pick_Entries[i] = new EntryPICK(reader);

                UpdateDictionary();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.WriteMagic("PICK");
                writer.Write(_pick_Entries.Length);
                foreach (var l in _pick_Entries)
                    writer.Write(l.Serialize(endianness));

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryPICK a in _pick_Entries)
            {
                if (a.Model == 0)
                    result.Add("Pickup table entry with Model set to 0");
                Verify(a.Model, ref result);
            }
        }

        private void UpdateDictionary()
        {
            pickEntries.Clear();

            foreach (EntryPICK entry in _pick_Entries)
                pickEntries[entry.PickupHash] = entry.Model;
        }

        public void ClearDictionary()
        {
            foreach (EntryPICK entry in _pick_Entries)
                pickEntries.Remove(entry.PickupHash);
        }
    }
}
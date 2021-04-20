using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntryPICK
    {
        public AssetID PickupHash { get; set; }
        public AssetByte PickupType { get; set; }
        public AssetByte PickupIndex { get; set; }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort PickupFlags { get; set; }
        public uint Quantity { get; set; }
        public AssetID ModelAssetID { get; set; }
        public AssetID AnimAssetID { get; set; }

        public EntryPICK()
        {
            ModelAssetID = 0;
            AnimAssetID = 0;
            PickupHash = 0;
        }

        public EntryPICK(EndianBinaryReader reader)
        {
            PickupHash = reader.ReadUInt32();
            PickupType = reader.ReadByte();
            PickupIndex = reader.ReadByte();
            PickupFlags = reader.ReadUInt16();
            Quantity = reader.ReadUInt32();
            ModelAssetID = reader.ReadUInt32();
            AnimAssetID = reader.ReadUInt32();
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(PickupHash);
            writer.Write(PickupType);
            writer.Write(PickupIndex);
            writer.Write(PickupFlags);
            writer.Write(Quantity);
            writer.Write(ModelAssetID);
            writer.Write(AnimAssetID);

            return writer.ToArray();
        }

        public override string ToString() => 
            $"[{Program.MainForm.GetAssetNameFromID(PickupHash)}] - [{Program.MainForm.GetAssetNameFromID(ModelAssetID)}]";
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

        public AssetPICK(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            reader.ReadInt32();

            _pick_Entries = new EntryPICK[reader.ReadInt32()];

            for (int i = 0; i < _pick_Entries.Length; i++)
                _pick_Entries[i] = new EntryPICK(reader);

            UpdateDictionary();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            var chars = new char[] { 'P', 'I', 'C', 'K' };
            writer.Write(writer.endianness == Endianness.Little ? chars : chars.Reverse().ToArray());

            writer.Write(_pick_Entries.Length);

            foreach (var l in _pick_Entries)
                writer.Write(l.Serialize(platform));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryPICK a in _pick_Entries)
                if (a.ModelAssetID == assetID)
                    return true;
            
            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryPICK a in _pick_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("PICK entry with ModelAssetID set to 0");
                Verify(a.ModelAssetID, ref result);
            }
        }

        private void UpdateDictionary()
        {
            pickEntries.Clear();

            foreach (EntryPICK entry in _pick_Entries)
                pickEntries[entry.PickupHash] = entry.ModelAssetID;
        }

        public void ClearDictionary()
        {
            foreach (EntryPICK entry in _pick_Entries)
                pickEntries.Remove(entry.PickupHash);
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

namespace IndustrialPark
{
    public class EntryTPIK
    {
        public AssetID PickupHash { get; set; }
        public AssetID Model_AssetID { get; set; }
        public AssetID RingModel_AssetID { get; set; }
        public AssetSingle UnknownFloat_0C { get; set; }
        public AssetSingle UnknownFloat_10 { get; set; }
        public AssetSingle UnknownFloat_14 { get; set; }
        public AssetSingle RingColorR { get; set; }
        public AssetSingle RingColorG { get; set; }
        public AssetSingle RingColorB { get; set; }
        public AssetID Unknown_24 { get; set; }
        public AssetID Unknown_28 { get; set; }
        public AssetID Pickup_SGRP { get; set; }
        public AssetID Denied_SGRP { get; set; }
        public byte HealthValue { get; set; }
        public byte PowerValue { get; set; }
        public byte BonusValue { get; set; }
        public byte UnknownByte_37 { get; set; }

        public EntryTPIK() { }
        public EntryTPIK(EndianBinaryReader reader)
        {
            PickupHash = reader.ReadUInt32();
            Model_AssetID = reader.ReadUInt32();
            RingModel_AssetID = reader.ReadUInt32();
            UnknownFloat_0C = reader.ReadSingle();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14 = reader.ReadSingle();
            RingColorR = reader.ReadSingle();
            RingColorG = reader.ReadSingle();
            RingColorB = reader.ReadSingle();
            Unknown_24 = reader.ReadUInt32();
            Unknown_28 = reader.ReadUInt32();
            Pickup_SGRP = reader.ReadUInt32();
            Denied_SGRP = reader.ReadUInt32();
            HealthValue = reader.ReadByte();
            PowerValue = reader.ReadByte();
            BonusValue = reader.ReadByte();
            UnknownByte_37 = reader.ReadByte();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(PickupHash);
                writer.Write(Model_AssetID);
                writer.Write(RingModel_AssetID);
                writer.Write(UnknownFloat_0C);
                writer.Write(UnknownFloat_10);
                writer.Write(UnknownFloat_14);
                writer.Write(RingColorR);
                writer.Write(RingColorG);
                writer.Write(RingColorB);
                writer.Write(Unknown_24);
                writer.Write(Unknown_28);
                writer.Write(Pickup_SGRP);
                writer.Write(Denied_SGRP);
                writer.Write(HealthValue);
                writer.Write(PowerValue);
                writer.Write(BonusValue);
                writer.Write(UnknownByte_37);

                return writer.ToArray();
            }
        }

        [Browsable(false)]
        public static int StructSize => 0x38;

        public bool HasReference(uint assetID) =>
            PickupHash == assetID ||
            Model_AssetID == assetID ||
            RingModel_AssetID == assetID ||
            Unknown_24 == assetID ||
            Unknown_28 == assetID ||
            Pickup_SGRP == assetID ||
            Denied_SGRP == assetID;
    }

    public class AssetTPIK : Asset
    {
        private const string categoryName = "Pickup Types";

        [Category(categoryName)]
        public int Unknown04 { get; set; }
        [Category(categoryName)]
        public int Unknown08 { get; set; }
        private EntryTPIK[] _tpik_Entries;
        [Category(categoryName)]
        public EntryTPIK[] TPIK_Entries 
        {
            get => _tpik_Entries;
            set
            {
                _tpik_Entries = value;
                UpdateDictionary();
            }
        }

        public AssetTPIK(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = 0x4;

                Unknown04 = reader.ReadInt32();
                Unknown08 = reader.ReadInt32();
                int amountOfEntries = reader.ReadInt32();
                TPIK_Entries = new EntryTPIK[amountOfEntries];
                for (int i = 0; i < TPIK_Entries.Length; i++)
                    TPIK_Entries[i] = new EntryTPIK(reader);

                UpdateDictionary();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(assetID);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(TPIK_Entries.Length);
                foreach (var t in TPIK_Entries)
                    writer.Write(t.Serialize(endianness));
                return writer.ToArray();
            }
        }

        public static Dictionary<uint, EntryTPIK> tpikEntries = new Dictionary<uint, EntryTPIK>();

        private void UpdateDictionary()
        {
            tpikEntries.Clear();

            foreach (EntryTPIK entry in TPIK_Entries)
                tpikEntries[entry.PickupHash] = entry;
        }

        public void ClearDictionary()
        {
            foreach (EntryTPIK entry in TPIK_Entries)
                tpikEntries.Remove(entry.PickupHash);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryTPIK a in TPIK_Entries)
                if (a.HasReference(assetID))
                    return true;
                
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryTPIK a in TPIK_Entries)
            {
                Verify(a.PickupHash, ref result);
                Verify(a.Model_AssetID, ref result);
                Verify(a.RingModel_AssetID, ref result);
                Verify(a.Unknown_24, ref result);
                Verify(a.Unknown_28, ref result);
                Verify(a.Pickup_SGRP, ref result);
                Verify(a.Denied_SGRP, ref result);
            }
        }
    }
}
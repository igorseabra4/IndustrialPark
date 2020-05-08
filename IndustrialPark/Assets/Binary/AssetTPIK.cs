using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntryTPIK
    {
        public AssetID PickupHash { get; set; }
        public AssetID Model_AssetID { get; set; }
        public AssetID RingModel_AssetID { get; set; }
        public float UnknownFloat_0C { get; set; }
        public float UnknownFloat_10 { get; set; }
        public float UnknownFloat_14 { get; set; }
        public float RingColorR { get; set; }
        public float RingColorG { get; set; }
        public float RingColorB { get; set; }
        public AssetID Unknown_24 { get; set; }
        public AssetID Unknown_28 { get; set; }
        public AssetID Pickup_SGRP { get; set; }
        public AssetID Denied_SGRP { get; set; }
        public byte HealthValue { get; set; }
        public byte PowerValue { get; set; }
        public byte BonusValue { get; set; }
        public byte UnknownByte_37 { get; set; }

        public EntryTPIK()
        {
            PickupHash = 0;
            Model_AssetID = 0;
            RingModel_AssetID = 0;
            Unknown_24 = 0;
            Unknown_28 = 0;
            Pickup_SGRP = 0;
            Denied_SGRP = 0;
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
        public static Dictionary<uint, EntryTPIK> tpikEntries = new Dictionary<uint, EntryTPIK>();

        public AssetTPIK(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;

            SetupDictionary();
        }

        private void SetupDictionary()
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

        private AssetID AssetID
        {
            get => ReadUInt(0x00);
            set => Write(0x00, value);
        }

        [Category("Pickup Types")]
        public int Unknown04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Pickup Types")]
        public int Unknown08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        private int AmountOfEntries
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        private int EntriesStart => 0x10;

        [Category("Pickup Types")]
        public EntryTPIK[] TPIK_Entries
        {
            get
            {
                List<EntryTPIK> entries = new List<EntryTPIK>();
                
                for (int i = 0; i < AmountOfEntries; i++)
                {
                    entries.Add(new EntryTPIK()
                    {
                        PickupHash = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x00),
                        Model_AssetID = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x04),
                        RingModel_AssetID = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x08),
                        UnknownFloat_0C = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x0C),
                        UnknownFloat_10 = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x10),
                        UnknownFloat_14 = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x14),
                        RingColorR = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x18),
                        RingColorG = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x1C),
                        RingColorB = ReadFloat(EntriesStart + i * EntryTPIK.StructSize + 0x20),
                        Unknown_24 = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x24),
                        Unknown_28 = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x28),
                        Pickup_SGRP = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x2C),
                        Denied_SGRP = ReadUInt(EntriesStart + i * EntryTPIK.StructSize + 0x30),
                        HealthValue = ReadByte(EntriesStart + i * EntryTPIK.StructSize + 0x34),
                        PowerValue = ReadByte(EntriesStart + i * EntryTPIK.StructSize + 0x35),
                        BonusValue = ReadByte(EntriesStart + i * EntryTPIK.StructSize + 0x36),
                        UnknownByte_37 = ReadByte(EntriesStart + i * EntryTPIK.StructSize + 0x37)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = Data.Take(4).ToList();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (var i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.PickupHash)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Model_AssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.RingModel_AssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownFloat_0C)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownFloat_10)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.UnknownFloat_14)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.RingColorR)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.RingColorG)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.RingColorB)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown_24)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown_28)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Pickup_SGRP)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Denied_SGRP)));
                    newData.Add(i.HealthValue);
                    newData.Add(i.PowerValue);
                    newData.Add(i.BonusValue);
                    newData.Add(i.UnknownByte_37);
                }

                AmountOfEntries = value.Length;
                Data = newData.ToArray();
                SetupDictionary();
            }
        }
    }
}
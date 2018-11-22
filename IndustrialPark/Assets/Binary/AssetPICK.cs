using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryPICK
    {
        public AssetID ReferenceID { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown21 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown22 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown23 { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown24 { get; set; }
        public uint Unknown3 { get; set; }
        public AssetID ModelAssetID { get; set; }
        public uint Unknown5 { get; set; }

        public EntryPICK()
        {
            ReferenceID = 0;
            ModelAssetID = 0;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(ReferenceID)}] - [{Program.MainForm.GetAssetNameFromID(ModelAssetID)}]";
        }
    }

    public class AssetPICK : Asset
    {
        public static Dictionary<uint, AssetID> pickEntries = new Dictionary<uint, AssetID>();

        public AssetPICK(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup()
        {
            pickEntries = new Dictionary<uint, AssetID>();
            foreach (EntryPICK entryPICK in PICK_Entries)
                pickEntries.Add(entryPICK.ReferenceID, entryPICK.ModelAssetID);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryPICK a in PICK_Entries)
            {
                if (a.ModelAssetID == assetID)
                    return true;
                if (a.ReferenceID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        public EntryPICK[] PICK_Entries
        {
            get
            {
                List<EntryPICK> entries = new List<EntryPICK>();
                int amount = ReadInt(0x4);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntryPICK()
                    {
                        ReferenceID = ReadUInt(8 + i * 0x14),
                        Unknown21 = ReadByte(12 + i * 0x14),
                        Unknown22 = ReadByte(13 + i * 0x14),
                        Unknown23 = ReadByte(14 + i * 0x14),
                        Unknown24 = ReadByte(15 + i * 0x14),
                        Unknown3 = ReadUInt(16 + i * 0x14),
                        ModelAssetID = ReadUInt(20 + i * 0x14),
                        Unknown5 = ReadUInt(24 + i * 0x14)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<EntryPICK> newValues = value.ToList();

                List<byte> newData = Data.Take(4).ToList();
                newData.AddRange(BitConverter.GetBytes(Switch(newValues.Count)));

                foreach (EntryPICK i in newValues)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ReferenceID)));
                    newData.Add(i.Unknown21);
                    newData.Add(i.Unknown22);
                    newData.Add(i.Unknown23);
                    newData.Add(i.Unknown24);
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown3)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown5)));
                }
                
                Data = newData.ToArray();
                Write(0x4, newValues.Count);
            }
        }
    }
}
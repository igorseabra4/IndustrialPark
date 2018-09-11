using System;
using System.Collections.Generic;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class PICKentry
    {
        public AssetID ReferenceID { get; set; }
        public byte Unknown21 { get; set; }
        public byte Unknown22 { get; set; }
        public byte Unknown23 { get; set; }
        public byte Unknown24 { get; set; }
        public uint Unknown3 { get; set; }
        public AssetID ModelAssetID { get; set; }
        public uint Unknown5 { get; set; }
    }

    public class AssetPICK : Asset
    {
        public Dictionary<AssetID, AssetID> pickEntries;
        public static AssetPICK pick;

        public AssetPICK(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public void Setup()
        {
            pickEntries = new Dictionary<AssetID, AssetID>();

            foreach (PICKentry p in PICKentries)
                pickEntries.Add(p.ReferenceID, p.ModelAssetID);

            pick = this;
        }

        public PICKentry[] PICKentries
        {
            get
            {
                List<PICKentry> entries = new List<PICKentry>();
                int amount = ReadInt(0x4);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new PICKentry()
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
                List<PICKentry> newValues = value.ToList();

                List<byte> newData = Data.Take(4).ToList();
                newData.AddRange(BitConverter.GetBytes(Switch(newValues.Count)));

                foreach (PICKentry i in newValues)
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        public override string ToString()
        {
            return $"[{ReferenceID.ToString()}] - [{ModelAssetID.ToString()}]";
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

            BinaryWriter b = new BinaryWriter(new FileStream("out.txt", FileMode.Create));

            foreach (EntryPICK p in PICKentries)
            {
                pickEntries.Add(p.ReferenceID, p.ModelAssetID);
                b.Write(p.ReferenceID);
                b.Write(p.ModelAssetID);
            }

            b.Close();
        }

        public EntryPICK[] PICKentries
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
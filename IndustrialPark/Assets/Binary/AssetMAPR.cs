using System;
using System.Collections.Generic;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryMAPR
    {
        public AssetID AssetID_SURF { get; set; }
        public int Unknown { get; set; }

        public EntryMAPR()
        {
            AssetID_SURF = 0;
        }

        public override string ToString()
        {
            return $"[{AssetID_SURF.ToString()}] - {Unknown}";
        }
    }

    public class AssetMAPR : Asset
    {
        public AssetMAPR(Section_AHDR AHDR) : base(AHDR)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;
        }

        public AssetID AssetID
        {
            get => ReadUInt(0);
            set => Write(0, value);
        }

        public EntryMAPR[] MAPR_Entries
        {
            get
            {
                List<EntryMAPR> entries = new List<EntryMAPR>();
                int amount = ReadInt(4);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntryMAPR()
                    {
                        AssetID_SURF = ReadUInt(8 + i * 8),
                        Unknown = ReadInt(12 + i * 8)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(AssetID)));
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryMAPR i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.AssetID_SURF)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown)));
                }
                
                Data = newData.ToArray();
            }
        }
    }
}
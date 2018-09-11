using System;
using System.Collections.Generic;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class AssetPIPT : Asset
    {
        public AssetPIPT(Section_AHDR AHDR) : base(AHDR)
        {
        }
        
        public EntryPIPT[] PICKentries
        {
            get
            {
                List<EntryPIPT> entries = new List<EntryPIPT>();
                int amount = ReadInt(0);

                for (int i = 0; i < amount; i++)
                {
                    byte[] Flags = BitConverter.GetBytes(ReadInt(12 + i * 0xC));
                    entries.Add(new EntryPIPT()
                    {
                        ModelAssetID = ReadUInt(4 + i * 0xC),
                        MaybeMeshIndex = ReadInt(8 + i * 0xC),
                        RelatedToVisibility = Flags[3],
                        Culling = Flags[2],
                        DestinationSourceBlend = Flags[1],
                        Unknown34 = Flags[0],
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<EntryPIPT> newValues = value.ToList();

                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(newValues.Count)));

                foreach (EntryPIPT i in newValues)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.MaybeMeshIndex)));
                    int Flags = BitConverter.ToInt32(new byte[] { i.Unknown34, i.DestinationSourceBlend, i.Culling, i.RelatedToVisibility }, 0);
                    newData.AddRange(BitConverter.GetBytes(Switch(Flags)));
                }
                
                Data = newData.ToArray();
            }
        }
    }
}
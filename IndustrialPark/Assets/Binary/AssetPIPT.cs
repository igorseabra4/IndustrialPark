using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryPIPT
    {
        public AssetID ModelAssetID { get; set; }
        public int MaybeMeshIndex { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte RelatedToVisibility { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Culling { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte DestinationSourceBlend { get; set; }
        [TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown34 { get; set; }

        public EntryPIPT()
        {
            ModelAssetID = 0;
        }

        public override string ToString()
        {
            return $"{Program.MainForm.GetAssetNameFromID(ModelAssetID)} - {MaybeMeshIndex}";
        }
    }

    public class AssetPIPT : Asset
    {
        public AssetPIPT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryPIPT a in PIPT_Entries)
            {
                if (a.ModelAssetID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        public EntryPIPT[] PIPT_Entries
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
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryPIPT i in value)
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
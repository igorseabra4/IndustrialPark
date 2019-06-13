using System;
using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryMAPR
    {
        public AssetID AssetID_SURF { get; set; }
        public AssetID Unknown { get; set; }

        public EntryMAPR()
        {
            AssetID_SURF = 0;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(AssetID_SURF)}] - [{Program.MainForm.GetAssetNameFromID(Unknown)}]";
        }
    }

    public class AssetMAPR : Asset
    {
        public AssetMAPR(Section_AHDR AHDR) : base(AHDR)
        {
            if (AssetID != AHDR.assetID)
                AssetID = AHDR.assetID;
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryMAPR a in MAPR_Entries)
                if (a.AssetID_SURF == assetID || a.Unknown == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryMAPR a in MAPR_Entries)
            {
                if (a.AssetID_SURF == 0)
                    result.Add("MAPR entry with SurfaceAssetID set to 0");
                Verify(a.AssetID_SURF, ref result);
            }
        }

        [Category("Surface Map")]
        public AssetID AssetID
        {
            get => ReadUInt(0);
            set => Write(0, value);
        }

        [Category("Surface Map")]
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
                        Unknown = ReadUInt(12 + i * 8)
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryLODT
    {
        public AssetID ModelAssetID { get; set; }
        public float MaxDistance { get; set; }
        public AssetID LOD1_Model { get; set; }
        public float LOD1_Distance { get; set; }
        public AssetID LOD2_Model { get; set; }
        public float LOD2_Distance { get; set; }
        public AssetID LOD3_Model { get; set; }
        public float LOD3_Distance { get; set; }

        public EntryLODT()
        {
            ModelAssetID = 0;
            LOD1_Model = 0;
            LOD2_Model = 0;
            LOD3_Model = 0;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - {MaxDistance}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryLODT entryLODT)
                return ModelAssetID == entryLODT.ModelAssetID;
            return false;
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }
    }

    public class AssetLODT : Asset
    {
        public AssetLODT(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryLODT a in LODT_Entries)
            {
                if (a.ModelAssetID == assetID)
                    return true;
                if (a.LOD1_Model == assetID)
                    return true;
                if (a.LOD2_Model == assetID)
                    return true;
                if (a.LOD3_Model == assetID)
                    return true;
            }

            return base.HasReference(assetID);
        }

        [Category("Level Of Detail Table")]
        public EntryLODT[] LODT_Entries
        {
            get
            {
                List<EntryLODT> entries = new List<EntryLODT>();
                int amount = ReadInt(0);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntryLODT()
                    {
                        ModelAssetID = ReadUInt(i * 0x20 + 0x04),
                        MaxDistance = ReadFloat(i * 0x20 + 0x08),
                        LOD1_Model = ReadUInt(i * 0x20 + 0x0C),
                        LOD2_Model = ReadUInt(i * 0x20 + 0x10),
                        LOD3_Model = ReadUInt(i * 0x20 + 0x14),
                        LOD1_Distance = ReadFloat(i * 0x20 + 0x18),
                        LOD2_Distance = ReadFloat(i * 0x20 + 0x1C),
                        LOD3_Distance = ReadFloat(i * 0x20 + 0x20),
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryLODT i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.MaxDistance)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD1_Model)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD2_Model)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD3_Model)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD1_Distance)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD2_Distance)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.LOD3_Distance)));
                }
                
                Data = newData.ToArray();
            }
        }

        public void Merge(AssetLODT assetLODT)
        {
            List<EntryLODT> entriesLODT = LODT_Entries.ToList();
            foreach (EntryLODT entryLODT in assetLODT.LODT_Entries)
                if (!entriesLODT.Contains(entryLODT))
                    entriesLODT.Add(entryLODT);
            LODT_Entries = entriesLODT.ToArray();
        }
    }
}
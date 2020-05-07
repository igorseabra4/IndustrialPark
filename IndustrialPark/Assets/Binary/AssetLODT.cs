using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntryLODT
    {
        [Category("LODT Entry")]
        public AssetID ModelAssetID { get; set; }
        [Category("LODT Entry")]
        public float MaxDistance { get; set; }
        [Category("LODT Entry")]
        public AssetID LOD1_Model { get; set; }
        [Category("LODT Entry")]
        public float LOD1_Distance { get; set; }
        [Category("LODT Entry")]
        public AssetID LOD2_Model { get; set; }
        [Category("LODT Entry")]
        public float LOD2_Distance { get; set; }
        [Category("LODT Entry")]
        public AssetID LOD3_Model { get; set; }
        [Category("LODT Entry")]
        public float LOD3_Distance { get; set; }
        [Category("LODT Entry"), Description("Movie only.")]
        public float Unknown { get; set; }

        public static int SizeOfStruct(Game game) => game == Game.Incredibles ? 0x24 : 0x20;

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
            if (obj != null && obj is EntryLODT entry)
                return ModelAssetID.Equals(entry.ModelAssetID);
            return false;
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }
    }

    public class AssetLODT : Asset
    {
        public static Dictionary<uint, float> MaxDistances = new Dictionary<uint, float>();

        public AssetLODT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            UpdateDictionary();
        }

        public void UpdateDictionary()
        {
            foreach (EntryLODT entry in LODT_Entries)
                MaxDistances[entry.ModelAssetID] = entry.MaxDistance;
        }

        public void ClearDictionary()
        {
            foreach (EntryLODT entry in LODT_Entries)
                MaxDistances.Remove(entry.ModelAssetID);
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryLODT a in LODT_Entries)
                if (a.ModelAssetID == assetID || a.LOD1_Model == assetID || a.LOD2_Model == assetID || a.LOD3_Model == assetID)
                    return true;
            
            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryLODT a in LODT_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("LODT entry with ModelAssetID set to 0");

                Verify(a.ModelAssetID, ref result);
                Verify(a.LOD1_Model, ref result);
                Verify(a.LOD2_Model, ref result);
                Verify(a.LOD3_Model, ref result);
            }
        }

        [Category("Level Of Detail Table")]
        public EntryLODT[] LODT_Entries
        {
            get
            {
                List<EntryLODT> entries = new List<EntryLODT>();

                for (int i = 4; i < Data.Length; i += EntryLODT.SizeOfStruct(game))
                {
                    EntryLODT a = new EntryLODT
                    {
                        ModelAssetID = ReadUInt(i + 0x00),
                        MaxDistance = ReadFloat(i + 0x04),
                        LOD1_Model = ReadUInt(i + 0x08),
                        LOD2_Model = ReadUInt(i + 0x0C),
                        LOD3_Model = ReadUInt(i + 0x10),
                        LOD1_Distance = ReadFloat(i + 0x14),
                        LOD2_Distance = ReadFloat(i + 0x18),
                        LOD3_Distance = ReadFloat(i + 0x1C)
                    };

                    if (game == Game.Incredibles)
                        a.Unknown = ReadFloat(i + 0x20);
                    
                    entries.Add(a);
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

                    if (game == Game.Incredibles)
                        newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown)));
                }

                Data = newData.ToArray();
                UpdateDictionary();
            }
        }

        public void Merge(AssetLODT asset)
        {
            var entries = LODT_Entries.ToList();

            foreach (var entry in asset.LODT_Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            LODT_Entries = entries.ToArray();
        }
    }
}
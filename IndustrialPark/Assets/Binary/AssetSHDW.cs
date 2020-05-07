using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public class EntrySHDW
    {
        public AssetID ModelAssetID { get; set; }
        public AssetID ShadowModelAssetID { get; set; }
        public int Unknown { get; set; }

        public EntrySHDW()
        {
            ModelAssetID = 0;
            ShadowModelAssetID = 0;
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - [{Program.MainForm.GetAssetNameFromID(ShadowModelAssetID)}]";
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntrySHDW entrySHDW)
                return ModelAssetID.Equals(entrySHDW.ModelAssetID);
            return false;
        }
    }

    public class AssetSHDW : Asset
    {
        public AssetSHDW(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySHDW a in SHDW_Entries)
                if (a.ModelAssetID == assetID || a.ShadowModelAssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntrySHDW a in SHDW_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("SHDW entry with ModelAssetID set to 0");
                Verify(a.ModelAssetID, ref result);
                if (a.ShadowModelAssetID == 0)
                    result.Add("SHDW entry with ShadowModelAssetID set to 0");
                Verify(a.ShadowModelAssetID, ref result);
            }
        }

        [Category("Shadow Map")]
        public EntrySHDW[] SHDW_Entries
        {
            get
            {
                List<EntrySHDW> entries = new List<EntrySHDW>();
                int amount = ReadInt(0);

                for (int i = 0; i < amount; i++)
                {
                    entries.Add(new EntrySHDW()
                    {
                        ModelAssetID = ReadUInt(4 + i * 0xC),
                        ShadowModelAssetID = ReadUInt(8 + i * 0xC),
                        Unknown = ReadInt(12 + i * 0xC)
                    });
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntrySHDW i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ShadowModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown)));
                }
                
                Data = newData.ToArray();
            }
        }

        public void Merge(AssetSHDW asset)
        {
            var entries = SHDW_Entries.ToList();

            foreach (var entry in asset.SHDW_Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            SHDW_Entries = entries.ToArray();
        }
    }
}
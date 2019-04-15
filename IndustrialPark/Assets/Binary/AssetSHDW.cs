using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;
using static IndustrialPark.ConverterFunctions;

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
                return ModelAssetID == entrySHDW.ModelAssetID;
            return false;
        }
    }

    public class AssetSHDW : Asset
    {
        public AssetSHDW(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySHDW a in SHDW_Entries)
            {
                if (a.ModelAssetID == assetID)
                    return true;
                if (a.ShadowModelAssetID == assetID)
                    return true;
            }

            return base.HasReference(assetID);
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

        public void Merge(AssetSHDW assetSHDW)
        {
            List<EntrySHDW> entriesSHDW = SHDW_Entries.ToList();
            List<uint> assetIDsAlreadyPresent = new List<uint>();

            foreach (EntrySHDW entrySHDW in entriesSHDW)
                assetIDsAlreadyPresent.Add(entrySHDW.ModelAssetID);

            foreach (EntrySHDW entrySHDW in assetSHDW.SHDW_Entries)
                if (!assetIDsAlreadyPresent.Contains(entrySHDW.ModelAssetID))
                    entriesSHDW.Add(entrySHDW);

            SHDW_Entries = entriesSHDW.ToArray();
        }
    }
}
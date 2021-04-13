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

        public EntrySHDW(EndianBinaryReader reader)
        {
            ModelAssetID = reader.ReadUInt32();
            ShadowModelAssetID = reader.ReadUInt32();
            Unknown = reader.ReadInt32();
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(ModelAssetID);
            writer.Write(ShadowModelAssetID);
            writer.Write(Unknown);

            return writer.ToArray();
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
        [Category("Shadow Map")]
        public EntrySHDW[] SHDW_Entries { get; set; }

        public AssetSHDW(Section_AHDR AHDR, Platform platform) : base(AHDR)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);

            SHDW_Entries = new EntrySHDW[reader.ReadInt32()];

            for (int i = 0; i < SHDW_Entries.Length; i++)
                SHDW_Entries[i] = new EntrySHDW(reader);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SHDW_Entries.Length);

            foreach (var l in SHDW_Entries)
                writer.Write(l.Serialize(platform));

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntrySHDW a in SHDW_Entries)
                if (a.ModelAssetID == assetID || a.ShadowModelAssetID == assetID)
                    return true;

            return false;
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
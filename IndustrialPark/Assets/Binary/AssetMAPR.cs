using System.Collections.Generic;
using System.ComponentModel;
using HipHopFile;

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
        [Category("Surface Map")]
        public EntryMAPR[] MAPR_Entries { get; set; }

        public AssetMAPR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            
            reader.ReadInt32();
            int maprCount = reader.ReadInt32();
            MAPR_Entries = new EntryMAPR[maprCount];

            for (int i = 0; i < MAPR_Entries.Length; i++)
                MAPR_Entries[i] = new EntryMAPR()
                {
                    AssetID_SURF = reader.ReadUInt32(),
                    Unknown = reader.ReadUInt32()
                };
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(assetID);
            writer.Write(MAPR_Entries.Length);
            foreach (var entry in MAPR_Entries)
            {
                writer.Write(entry.AssetID_SURF);
                writer.Write(entry.Unknown);
            }

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryMAPR a in MAPR_Entries)
                if (a.AssetID_SURF == assetID || a.Unknown == assetID)
                    return true;
            
            return false;
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
    }
}
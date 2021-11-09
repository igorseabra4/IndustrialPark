using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryMAPR
    {
        public AssetID Surface_AssetID { get; set; }
        public AssetID Unknown { get; set; }

        public EntryMAPR() { }
        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(Surface_AssetID)}] - [{Program.MainForm.GetAssetNameFromID(Unknown)}]";
        }
    }

    public class AssetMAPR : Asset
    {
        [Category("Surface Mapper")]
        public EntryMAPR[] MAPR_Entries { get; set; }

        public AssetMAPR(string assetName) : base(assetName, AssetType.MAPR)
        {
            MAPR_Entries = new EntryMAPR[0];
        }

        public AssetMAPR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                int maprCount = reader.ReadInt32();
                MAPR_Entries = new EntryMAPR[maprCount];

                for (int i = 0; i < MAPR_Entries.Length; i++)
                    MAPR_Entries[i] = new EntryMAPR()
                    {
                        Surface_AssetID = reader.ReadUInt32(),
                        Unknown = reader.ReadUInt32()
                    };
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(assetID);
                writer.Write(MAPR_Entries.Length);
                foreach (var entry in MAPR_Entries)
                {
                    writer.Write(entry.Surface_AssetID);
                    writer.Write(entry.Unknown);
                }

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (EntryMAPR a in MAPR_Entries)
                if (a.Surface_AssetID == assetID || a.Unknown == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryMAPR a in MAPR_Entries)
            {
                if (a.Surface_AssetID == 0)
                    result.Add("MAPR entry with SurfaceAssetID set to 0");
                Verify(a.Surface_AssetID, ref result);
            }
        }
    }
}
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntryCOLL
    {
        public AssetID ModelAssetID { get; set; }
        public AssetID Collision_ModelAssetID { get; set; }
        public AssetID CameraCollision_ModelAssetID { get; set; }

        public EntryCOLL() { }
        public EntryCOLL(EndianBinaryReader reader)
        {
            ModelAssetID = reader.ReadUInt32();
            Collision_ModelAssetID = reader.ReadUInt32();
            CameraCollision_ModelAssetID = reader.ReadUInt32();
        }

        public override string ToString()
        {
            if (Collision_ModelAssetID != 0)
                return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - [{Program.MainForm.GetAssetNameFromID(Collision_ModelAssetID)}]";
            return $"[{Program.MainForm.GetAssetNameFromID(ModelAssetID)}] - [{Program.MainForm.GetAssetNameFromID(CameraCollision_ModelAssetID)}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryCOLL entryCOLL)
                return ModelAssetID.Equals(entryCOLL.ModelAssetID);
            return false;
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }
    }

    public class AssetCOLL : Asset
    {
        [Category("Collision Table")]
        public EntryCOLL[] CollisionTable_Entries { get; set; }

        public AssetCOLL(string assetName) : base(assetName, AssetType.COLL)
        {
            CollisionTable_Entries = new EntryCOLL[0];
        }

        public AssetCOLL(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {

                var entries = new EntryCOLL[reader.ReadInt32()];
                for (int i = 0; i < entries.Length; i++)
                    entries[i] = new EntryCOLL(reader);

                CollisionTable_Entries = entries;
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(CollisionTable_Entries.Length);

                foreach (var entry in CollisionTable_Entries)
                {
                    writer.Write(entry.ModelAssetID);
                    writer.Write(entry.Collision_ModelAssetID);
                    writer.Write(entry.CameraCollision_ModelAssetID);
                }

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in CollisionTable_Entries)
                if (a.ModelAssetID == assetID || a.Collision_ModelAssetID == assetID || a.CameraCollision_ModelAssetID == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (var a in CollisionTable_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("COLL entry with ModelAssetID set to 0");

                Verify(a.ModelAssetID, ref result);
                Verify(a.Collision_ModelAssetID, ref result);
                Verify(a.CameraCollision_ModelAssetID, ref result);
            }
        }

        public void Merge(AssetCOLL asset)
        {
            var entries = CollisionTable_Entries.ToList();

            foreach (var entry in asset.CollisionTable_Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            CollisionTable_Entries = entries.ToArray();
        }
    }
}
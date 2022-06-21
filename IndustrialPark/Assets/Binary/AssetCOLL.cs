using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntryCOLL : GenericAssetDataContainer
    {
        public AssetID Model { get; set; }
        public AssetID CollisionModel { get; set; }
        public AssetID CameraCollisionModel { get; set; }

        public EntryCOLL() { }
        public EntryCOLL(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
            CollisionModel = reader.ReadUInt32();
            CameraCollisionModel = reader.ReadUInt32();
        }

        public override string ToString()
        {
            if (CollisionModel != 0)
                return $"[{Program.MainForm.GetAssetNameFromID(Model)}] - [{Program.MainForm.GetAssetNameFromID(CollisionModel)}]";
            return $"[{Program.MainForm.GetAssetNameFromID(Model)}] - [{Program.MainForm.GetAssetNameFromID(CameraCollisionModel)}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryCOLL entryCOLL)
                return Model.Equals(entryCOLL.Model);
            return false;
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }
    }

    public class AssetCOLL : Asset, IAssetAddSelected
    {
        [Category("Collision Table")]
        public EntryCOLL[] CollisionTable_Entries { get; set; }

        public AssetCOLL(string assetName) : base(assetName, AssetType.CollisionTable)
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
                    writer.Write(entry.Model);
                    writer.Write(entry.CollisionModel);
                    writer.Write(entry.CameraCollisionModel);
                }

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            foreach (var a in CollisionTable_Entries)
            {
                if (a.Model == 0)
                    result.Add("CollisionTable entry with Model set to 0");

                Verify(a.Model, ref result);
                Verify(a.CollisionModel, ref result);
                Verify(a.CameraCollisionModel, ref result);
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

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> items)
        {
            var entries = CollisionTable_Entries.ToList();
            foreach (var i in items)
                if (!entries.Any(e => e.Model == i))
                    entries.Add(new EntryCOLL() { Model = i });
            CollisionTable_Entries = entries.ToArray();
        }
    }
}
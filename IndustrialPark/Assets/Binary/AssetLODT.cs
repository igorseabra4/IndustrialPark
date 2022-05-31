using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntryLODT
    {
        public AssetID BaseModelAssetID { get; set; }
        public AssetSingle MaxDistance { get; set; }
        public AssetID LOD1_Model { get; set; }
        public AssetSingle LOD1_MinDistance { get; set; }
        public AssetID LOD2_Model { get; set; }
        public AssetSingle LOD2_MinDistance { get; set; }
        public AssetID LOD3_Model { get; set; }
        public AssetSingle LOD3_MinDistance { get; set; }
        [Description("Incredibles only")]
        public AssetSingle Unknown { get; set; }

        public EntryLODT() { }
        public EntryLODT(EndianBinaryReader reader, Game game)
        {
            BaseModelAssetID = reader.ReadUInt32();
            MaxDistance = reader.ReadSingle();
            LOD1_Model = reader.ReadUInt32();
            LOD2_Model = reader.ReadUInt32();
            LOD3_Model = reader.ReadUInt32();
            LOD1_MinDistance = reader.ReadSingle();
            LOD2_MinDistance = reader.ReadSingle();
            LOD3_MinDistance = reader.ReadSingle();

            if (game == Game.Incredibles)
                Unknown = reader.ReadSingle();
        }

        public byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(BaseModelAssetID);
                writer.Write(MaxDistance);
                writer.Write(LOD1_Model);
                writer.Write(LOD2_Model);
                writer.Write(LOD3_Model);
                writer.Write(LOD1_MinDistance);
                writer.Write(LOD2_MinDistance);
                writer.Write(LOD3_MinDistance);

                if (game == Game.Incredibles)
                    writer.Write(Unknown);

                return writer.ToArray();
            }
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(BaseModelAssetID)}] - {MaxDistance}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryLODT entry)
                return BaseModelAssetID.Equals(entry.BaseModelAssetID);
            return false;
        }

        public override int GetHashCode()
        {
            return BaseModelAssetID.GetHashCode();
        }
    }

    public class AssetLODT : Asset
    {
        private static Dictionary<uint, float> maxDistances = new Dictionary<uint, float>();

        public static float MaxDistanceTo(uint _modelAssetID) => maxDistances.ContainsKey(_modelAssetID) ? maxDistances[_modelAssetID] : SharpRenderer.DefaultLODTDistance;

        private EntryLODT[] _lodt_Entries;
        [Category("Level Of Detail Table")]
        public EntryLODT[] LODT_Entries
        {
            get => _lodt_Entries;
            set
            {
                _lodt_Entries = value;
                UpdateDictionary();
            }
        }

        public AssetLODT(string assetName) : base(assetName, AssetType.LevelOfDetailTable)
        {
            LODT_Entries = new EntryLODT[0];
        }

        public AssetLODT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                _lodt_Entries = new EntryLODT[reader.ReadInt32()];

                for (int i = 0; i < _lodt_Entries.Length; i++)
                    _lodt_Entries[i] = new EntryLODT(reader, game);

                UpdateDictionary();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(LODT_Entries.Length);

                foreach (var l in LODT_Entries)
                    writer.Write(l.Serialize(game, endianness));

                return writer.ToArray();
            }
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var a in LODT_Entries)
                if (a.BaseModelAssetID == assetID || a.LOD1_Model == assetID || a.LOD2_Model == assetID || a.LOD3_Model == assetID)
                    return true;

            return false;
        }

        public override void Verify(ref List<string> result)
        {
            foreach (var a in LODT_Entries)
            {
                if (a.BaseModelAssetID == 0)
                    result.Add("LODT entry with ModelAssetID set to 0");

                Verify(a.BaseModelAssetID, ref result);
                Verify(a.LOD1_Model, ref result);
                Verify(a.LOD2_Model, ref result);
                Verify(a.LOD3_Model, ref result);
            }
        }

        public void UpdateDictionary()
        {
            foreach (var entry in LODT_Entries)
                maxDistances[entry.BaseModelAssetID] = entry.MaxDistance;
        }

        public void ClearDictionary()
        {
            foreach (var entry in LODT_Entries)
                maxDistances.Remove(entry.BaseModelAssetID);
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
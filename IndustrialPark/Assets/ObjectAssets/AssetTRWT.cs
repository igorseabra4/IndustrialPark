using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class ThrowableEntry : GenericAssetDataContainer
    {
        public AssetID Model { get; set; }
        public int Type { get; set; }
        public AssetID Shrapnel { get; set; }
        public int Damage { get; set; }
        public AssetSingle DamageRadius { get; set; }
        
        public ThrowableEntry() { }
        public ThrowableEntry(EndianBinaryReader reader)
        {
            Model = reader.ReadUInt32();
            Type = reader.ReadInt32();
            Shrapnel = reader.ReadUInt32();
            Damage = reader.ReadInt32();
            DamageRadius = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Model);
                writer.Write(Type);
                writer.Write(Shrapnel);
                writer.Write(Damage);
                writer.Write(DamageRadius);
                return writer.ToArray();
            }
        }

        public override string ToString()
        {
            return $"[{Program.MainForm.GetAssetNameFromID(Model)}]";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ThrowableEntry entry)
                return Model.Equals(entry.Model);
            return false;
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Model, ref result);
            if (Model == 0)
                result.Add("Throwable entry with Model set to 0");
            Verify(Shrapnel, ref result);
        }
    }

    public class AssetTRWT : BaseAsset
    {
        private const string categoryName = "Throwable Table";

        [Category(categoryName)]
        public int Version { get; set; }
        [Category(categoryName)]
        public ThrowableEntry[] Entries { get; set; }

        public AssetTRWT(string assetName) : base(assetName, AssetType.ThrowableTable, BaseAssetType.Unknown_Other)
        {
            Version = 3;
            Entries = new ThrowableEntry[0];
        }

        public AssetTRWT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Version = reader.ReadInt32();
                var count = reader.ReadInt32();
                Entries = new ThrowableEntry[count];
                for (int i = 0; i < count; i++)
                    Entries[i] = new ThrowableEntry(reader);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(Version);
                writer.Write(Entries.Length);
                foreach (var e in Entries)
                    writer.Write(e.Serialize(game, endianness));
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);
            foreach(var entry in Entries)
                entry.Verify(ref result);
        }
    }
}
﻿using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class ThrowableEntry : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
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

        public override void Serialize(EndianBinaryWriter writer)
        {

            writer.Write(Model);
            writer.Write(Type);
            writer.Write(Shrapnel);
            writer.Write(Damage);
            writer.Write(DamageRadius);

        }

        public override string ToString()
        {
            return $"[{HexUIntTypeConverter.StringFromAssetID(Model)}]";
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
    }

    public class AssetTRWT : BaseAsset
    {
        private const string categoryName = "Throwable Table";
        public override string AssetInfo => $"{Entries.Length} entries";

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

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
            writer.Write(Entries.Length);
            foreach (var e in Entries)
                e.Serialize(writer);
            SerializeLinks(writer);
        }
    }
}
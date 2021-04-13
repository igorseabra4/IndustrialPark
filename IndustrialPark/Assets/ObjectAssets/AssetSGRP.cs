using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntrySGRP
    {
        public AssetID Sound_AssetID { get; set; }
        [TypeConverter(typeof(FloatTypeConverter)), DisplayName("Volume (0-1)")]
        public float Volume { get; set; }
        public int UnknownInt08 { get; set; }
        public int UnknownInt0C { get; set; }

        public EntrySGRP()
        {
            Sound_AssetID = 0;
            Volume = 0.8f;
        }

        public EntrySGRP(EndianBinaryReader reader)
        {
            Sound_AssetID = reader.ReadUInt32();
            Volume = reader.ReadSingle();
            UnknownInt08 = reader.ReadInt32();
            UnknownInt0C = reader.ReadInt32();
        }

        public byte[] Serialize(Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(Sound_AssetID);
            writer.Write(Volume);
            writer.Write(UnknownInt08);
            writer.Write(UnknownInt0C);
            return writer.ToArray();
        }

        public override string ToString() => $"[{Program.MainForm.GetAssetNameFromID(Sound_AssetID)}] - [{Volume}]";
    }

    public class AssetSGRP : BaseAsset
    {
        private const string categoryName = "Sound Group";

        [Category(categoryName)]
        public int UnknownInt08 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0D { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0E { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte0F { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte10 { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte PlayGlobally { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte ChooseRandomEntry { get; set; }
        [Category(categoryName), TypeConverter(typeof(HexByteTypeConverter))]
        public byte UnknownByte13 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float InnerRadius { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float OuterRadius { get; set; }
        [Category(categoryName)]
        public int UnknownInt1C { get; set; }
        [Category(categoryName)]
        public EntrySGRP[] SGRP_Entries { get; set; }

        public AssetSGRP(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            UnknownInt08 = reader.ReadInt32();

            byte AmountOfReferences = reader.ReadByte();
            UnknownByte0D = reader.ReadByte();
            UnknownByte0E = reader.ReadByte();
            UnknownByte0F = reader.ReadByte();

            UnknownByte10 = reader.ReadByte();
            PlayGlobally = reader.ReadByte();
            ChooseRandomEntry = reader.ReadByte();
            UnknownByte13 = reader.ReadByte();

            InnerRadius = reader.ReadSingle();
            OuterRadius = reader.ReadSingle();
            UnknownInt1C = reader.ReadInt32();

            SGRP_Entries = new EntrySGRP[AmountOfReferences];
            for (int i = 0; i < SGRP_Entries.Length; i++)
                SGRP_Entries[i] = new EntrySGRP(reader);
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(UnknownInt08);

            writer.Write((byte)SGRP_Entries.Length);
            writer.Write(UnknownByte0D);
            writer.Write(UnknownByte0E);
            writer.Write(UnknownByte0F);

            writer.Write(UnknownByte10);
            writer.Write(PlayGlobally);
            writer.Write(ChooseRandomEntry);
            writer.Write(UnknownByte13);

            writer.Write(InnerRadius);
            writer.Write(OuterRadius);
            writer.Write(UnknownInt1C);

            foreach (var i in SGRP_Entries)
                writer.Write(i.Serialize(platform));

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public override bool HasReference(uint assetID)
        {
            foreach (var i in SGRP_Entries)
                if (i.Sound_AssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            base.Verify(ref result);

            foreach (var i in SGRP_Entries)
            {
                if (i.Sound_AssetID == 0)
                    result.Add("SGRP entry with Sound_AssetID set to 0");
                Verify(i.Sound_AssetID, ref result);
            }
        }
    }
}
using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class EntrySGRP
    {
        public AssetID Sound_AssetID { get; set; }
        [DisplayName("Volume (0-1)")]
        public AssetSingle Volume { get; set; }
        public AssetSingle MinPitchMult { get; set; }
        public AssetSingle MaxPitchMult { get; set; }

        public EntrySGRP()
        {
            Volume = 0.8f;
        }

        public EntrySGRP(EndianBinaryReader reader)
        {
            Sound_AssetID = reader.ReadUInt32();
            Volume = reader.ReadSingle();
            MinPitchMult = reader.ReadSingle();
            MaxPitchMult = reader.ReadSingle();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Sound_AssetID);
                writer.Write(Volume);
                writer.Write(MinPitchMult);
                writer.Write(MaxPitchMult);
                return writer.ToArray();
            }
        }

        public override string ToString() => $"[{Program.MainForm.GetAssetNameFromID(Sound_AssetID)}] - [{Volume}]";
    }

    public class AssetSGRP : BaseAsset
    {
        private const string categoryName = "Sound Group";

        [Category(categoryName)]
        public int uPlayedMask { get; set; }
        [Category(categoryName)]
        public AssetByte uSetBits { get; set; }
        [Category(categoryName)]
        public AssetByte nMaxPlays { get; set; }
        [Category(categoryName)]
        public AssetByte uPriority { get; set; }
        [Category(categoryName)]
        public FlagBitmask uFlags { get; set; } = ByteFlagsDescriptor("Play globally");
        [Category(categoryName)]
        public FlagBitmask eSoundCategory { get; set; } = ByteFlagsDescriptor("Choose random entry");
        [Category(categoryName)]
        public AssetByte ePlayRule { get; set; }
        [Category(categoryName)]
        public AssetByte uInfoPad0 { get; set; }
        [Category(categoryName)]
        public AssetSingle InnerRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle OuterRadius { get; set; }

        private char[] _pszGroupName;
        [Category(categoryName)]
        public string pszGroupName
        {
            get => new string(_pszGroupName);
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _pszGroupName = value.ToCharArray();
            }
        }

        [Category(categoryName)]
        public EntrySGRP[] SGRP_Entries { get; set; }

        public AssetSGRP(string assetName) : base(assetName, AssetType.SGRP, BaseAssetType.SGRP)
        {
            nMaxPlays = 0x30;
            uPriority = 0x80;
            uInfoPad0 = 0x42;
            InnerRadius = 8f;
            OuterRadius = 25f;
            SGRP_Entries = new EntrySGRP[] { new EntrySGRP() };
        }

        public AssetSGRP(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                uPlayedMask = reader.ReadInt32();

                byte entryCount = reader.ReadByte();
                uSetBits = reader.ReadByte();
                nMaxPlays = reader.ReadByte();
                uPriority = reader.ReadByte();

                uFlags.FlagValueByte = reader.ReadByte();
                eSoundCategory.FlagValueByte = reader.ReadByte();
                ePlayRule = reader.ReadByte();
                uInfoPad0 = reader.ReadByte();

                InnerRadius = reader.ReadSingle();
                OuterRadius = reader.ReadSingle();

                _pszGroupName = reader.ReadString(4).ToCharArray();
                _pszGroupName = reader.endianness == Endianness.Little ? _pszGroupName : _pszGroupName.Reverse().ToArray();

                SGRP_Entries = new EntrySGRP[entryCount];
                for (int i = 0; i < SGRP_Entries.Length; i++)
                    SGRP_Entries[i] = new EntrySGRP(reader);
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(uPlayedMask);
                writer.Write((byte)SGRP_Entries.Length);
                writer.Write(uSetBits);
                writer.Write(nMaxPlays);
                writer.Write(uPriority);
                writer.Write(uFlags.FlagValueByte);
                writer.Write(eSoundCategory.FlagValueByte);
                writer.Write(ePlayRule);
                writer.Write(uInfoPad0);
                writer.Write(InnerRadius);
                writer.Write(OuterRadius);
                writer.Write(new string(endianness == Endianness.Little ? _pszGroupName : _pszGroupName.Reverse().ToArray()));

                foreach (var i in SGRP_Entries)
                    writer.Write(i.Serialize(endianness));
                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
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
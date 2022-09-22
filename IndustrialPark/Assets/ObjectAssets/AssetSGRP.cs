using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public class SoundGroupInfo : GenericAssetDataContainer
    {
        [ValidReferenceRequired]
        public AssetID Sound { get; set; }
        [DisplayName("Volume (0-1)")]
        public AssetSingle Volume { get; set; }
        public AssetSingle MinPitchMult { get; set; }
        public AssetSingle MaxPitchMult { get; set; }

        public SoundGroupInfo()
        {
            Volume = 0.8f;
        }

        public SoundGroupInfo(AssetID sound) : this()
        {
            Sound = sound;
        }

        public SoundGroupInfo(EndianBinaryReader reader)
        {
            Sound = reader.ReadUInt32();
            Volume = reader.ReadSingle();
            MinPitchMult = reader.ReadSingle();
            MaxPitchMult = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Sound);
            writer.Write(Volume);
            writer.Write(MinPitchMult);
            writer.Write(MaxPitchMult);
        }

        public override string ToString() => $"[{HexUIntTypeConverter.StringFromAssetID(Sound)}] - [{Volume}]";
    }

    public class AssetSGRP : BaseAsset, IAssetAddSelected
    {
        private const string categoryName = "Sound Group";
        public override string AssetInfo => $"{Entries.Length} entries";

        [Category(categoryName)]
        public int uPlayedMask { get; set; }
        [Category(categoryName)]
        public byte uSetBits { get; set; }
        [Category(categoryName)]
        public byte nMaxPlays { get; set; }
        [Category(categoryName)]
        public byte uPriority { get; set; }
        [Category(categoryName)]
        public FlagBitmask uFlags { get; set; } = ByteFlagsDescriptor("Play globally");
        [Category(categoryName)]
        public byte eSoundCategory { get; set; }
        [Category(categoryName)]
        public byte ePlayRule { get; set; }
        [Category(categoryName)]
        public byte uInfoPad0 { get; set; }
        [Category(categoryName)]
        public AssetSingle InnerRadius { get; set; }
        [Category(categoryName)]
        public AssetSingle OuterRadius { get; set; }

        private int _pszGroupName;
        [Category(categoryName)]
        public string PszGroupName
        {
            get => System.Text.Encoding.GetEncoding(1252).GetString(BitConverter.GetBytes(_pszGroupName));
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("Value must be 4 characters long");
                _pszGroupName = BitConverter.ToInt32(value.ToCharArray().Cast<byte>().ToArray(), 0);
            }
        }

        [Category(categoryName)]
        public SoundGroupInfo[] Entries { get; set; }

        public AssetSGRP(string assetName) : base(assetName, AssetType.SoundGroup, BaseAssetType.SoundGroup)
        {
            nMaxPlays = 0x30;
            uPriority = 0x80;
            uInfoPad0 = 0x42;
            InnerRadius = 8f;
            OuterRadius = 25f;
            Entries = new SoundGroupInfo[] { new SoundGroupInfo() };
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
                eSoundCategory = reader.ReadByte();
                ePlayRule = reader.ReadByte();
                uInfoPad0 = reader.ReadByte();

                InnerRadius = reader.ReadSingle();
                OuterRadius = reader.ReadSingle();

                _pszGroupName = reader.ReadInt32();

                Entries = new SoundGroupInfo[entryCount];
                for (int i = 0; i < Entries.Length; i++)
                    Entries[i] = new SoundGroupInfo(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(uPlayedMask);
            writer.Write((byte)Entries.Length);
            writer.Write(uSetBits);
            writer.Write(nMaxPlays);
            writer.Write(uPriority);
            writer.Write(uFlags.FlagValueByte);
            writer.Write(eSoundCategory);
            writer.Write(ePlayRule);
            writer.Write(uInfoPad0);
            writer.Write(InnerRadius);
            writer.Write(OuterRadius);
            writer.Write(_pszGroupName);

            foreach (var i in Entries)
                i.Serialize(writer);
            SerializeLinks(writer);
        }

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> newItems)
        {
            var entries = Entries.ToList();
            foreach (uint i in newItems)
                if (!entries.Any(e => e.Sound == i))
                    entries.Add(new SoundGroupInfo(i));
            Entries = entries.ToArray();
        }
    }
}
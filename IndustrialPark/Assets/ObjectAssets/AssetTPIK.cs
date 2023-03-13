using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryTPIK : GenericAssetDataContainer
    {
        public AssetID PickupHash { get; set; }
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        public AssetID PulseModel { get; set; }
        public AssetSingle PulseTime { get; set; }
        public AssetSingle PulseAddScale { get; set; }
        public AssetSingle PulseMoveDown { get; set; }
        public AssetSingle ColorRed { get; set; }
        public AssetSingle ColorGreen { get; set; }
        public AssetSingle ColorBlue { get; set; }
        public AssetColor ColorRGB
        {
            get => AssetColor.FromVector4(ColorRed, ColorGreen, ColorBlue, 1f);
            set
            {
                var val = value.ToVector4();
                ColorRed = val.X;
                ColorGreen = val.Y;
                ColorBlue = val.Z;
            }
        }
        public AssetID Color { get; set; }
        public AssetID FlyingSoundGroup { get; set; }
        public AssetID PickupSoundGroup { get; set; }
        public AssetID DeniedSoundGroup { get; set; }
        public byte HealthValue { get; set; }
        public byte PowerValue { get; set; }
        public byte SaveFlag { get; set; }
        public byte BInitialized { get; set; }

        public EntryTPIK() { }
        public EntryTPIK(EndianBinaryReader reader)
        {
            PickupHash = reader.ReadUInt32();
            Model = reader.ReadUInt32();
            PulseModel = reader.ReadUInt32();
            PulseTime = reader.ReadSingle();
            PulseAddScale = reader.ReadSingle();
            PulseMoveDown = reader.ReadSingle();
            ColorRed = reader.ReadSingle();
            ColorGreen = reader.ReadSingle();
            ColorBlue = reader.ReadSingle();
            Color = reader.ReadUInt32();
            FlyingSoundGroup = reader.ReadUInt32();
            PickupSoundGroup = reader.ReadUInt32();
            DeniedSoundGroup = reader.ReadUInt32();
            HealthValue = reader.ReadByte();
            PowerValue = reader.ReadByte();
            SaveFlag = reader.ReadByte();
            BInitialized = reader.ReadByte();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(PickupHash);
            writer.Write(Model);
            writer.Write(PulseModel);
            writer.Write(PulseTime);
            writer.Write(PulseAddScale);
            writer.Write(PulseMoveDown);
            writer.Write(ColorRed);
            writer.Write(ColorGreen);
            writer.Write(ColorBlue);
            writer.Write(Color);
            writer.Write(FlyingSoundGroup);
            writer.Write(PickupSoundGroup);
            writer.Write(DeniedSoundGroup);
            writer.Write(HealthValue);
            writer.Write(PowerValue);
            writer.Write(SaveFlag);
            writer.Write(BInitialized);
        }
    }

    public class AssetTPIK : BaseAsset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private const string categoryName = "Pickup Types";

        [Category(categoryName)]
        public int Version { get; set; }
        private EntryTPIK[] _entries;
        [Category(categoryName)]
        public EntryTPIK[] Entries
        {
            get => _entries;
            set
            {
                _entries = value;
                UpdateDictionary();
            }
        }

        public AssetTPIK(string assetName) : base(assetName, AssetType.PickupTypes, BaseAssetType.Unknown_Other)
        {
            Version = 3;
            Entries = new EntryTPIK[0];
        }

        public AssetTPIK(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;
                Version = reader.ReadInt32();
                int count = reader.ReadInt32();
                _entries = new EntryTPIK[count];
                for (int i = 0; i < _entries.Length; i++)
                    _entries[i] = new EntryTPIK(reader);

                UpdateDictionary();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Version);
            writer.Write(Entries.Length);
            foreach (var t in Entries)
                t.Serialize(writer);
        }

        public static Dictionary<uint, EntryTPIK> tpikEntries = new Dictionary<uint, EntryTPIK>();

        private void UpdateDictionary()
        {
            tpikEntries.Clear();

            foreach (EntryTPIK entry in Entries)
                tpikEntries[entry.PickupHash] = entry;
        }

        public void ClearDictionary()
        {
            foreach (EntryTPIK entry in Entries)
                tpikEntries.Remove(entry.PickupHash);
        }
    }
}
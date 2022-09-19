using AssetEditorColors;
using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryTPIK : GenericAssetDataContainer
    {
        public AssetID PickupHash { get; set; }
        public AssetID Model { get; set; }
        public AssetID PulseModel { get; set; }
        public AssetSingle PulseTime { get; set; }
        public AssetSingle PulseAddScale { get; set; }
        public AssetSingle PulseMoveDown { get; set; }
        public AssetSingle ColorR { get; set; }
        public AssetSingle ColorG { get; set; }
        public AssetSingle ColorB { get; set; }
        public AssetColor ColorRGB
        {
            get => new AssetColor((byte)(ColorR * 255), (byte)(ColorG * 255), (byte)(ColorB * 255), 255);
            set
            {
                var val = value.ToVector4();
                ColorR = val.X;
                ColorG = val.Y;
                ColorB = val.Z;
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
            ColorR = reader.ReadSingle();
            ColorG = reader.ReadSingle();
            ColorB = reader.ReadSingle();
            Color = reader.ReadUInt32();
            FlyingSoundGroup = reader.ReadUInt32();
            PickupSoundGroup = reader.ReadUInt32();
            DeniedSoundGroup = reader.ReadUInt32();
            HealthValue = reader.ReadByte();
            PowerValue = reader.ReadByte();
            SaveFlag = reader.ReadByte();
            BInitialized = reader.ReadByte();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(PickupHash);
                writer.Write(Model);
                writer.Write(PulseModel);
                writer.Write(PulseTime);
                writer.Write(PulseAddScale);
                writer.Write(PulseMoveDown);
                writer.Write(ColorR);
                writer.Write(ColorG);
                writer.Write(ColorB);
                writer.Write(Color);
                writer.Write(FlyingSoundGroup);
                writer.Write(PickupSoundGroup);
                writer.Write(DeniedSoundGroup);
                writer.Write(HealthValue);
                writer.Write(PowerValue);
                writer.Write(SaveFlag);
                writer.Write(BInitialized);

                return writer.ToArray();
            }
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

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));
                writer.Write(Version);
                writer.Write(Entries.Length);
                foreach (var t in Entries)
                    writer.Write(t.Serialize(endianness));
                return writer.ToArray();
            }
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

        public override void Verify(ref List<string> result)
        {
            foreach (EntryTPIK a in Entries)
            {
                Verify(a.PickupHash, ref result);
                Verify(a.Model, ref result);
                Verify(a.PulseModel, ref result);
                Verify(a.Color, ref result);
                Verify(a.FlyingSoundGroup, ref result);
                Verify(a.PickupSoundGroup, ref result);
                Verify(a.DeniedSoundGroup, ref result);
            }
        }
    }
}
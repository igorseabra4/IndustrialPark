using HipHopFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IndustrialPark
{
    public enum BlendFactorType
    {
        None = 0x00,
        Zero = 0x01,
        One = 0x02,
        SourceColor = 0x03,
        InverseSourceColor = 0x04,
        SourceAlpha = 0x05,
        InverseSourceAlpha = 0x06,
        DestinationAlpha = 0x07,
        InverseDestinationAlpha = 0x08,
        DestinationColor = 0x09,
        InverseDestinationColor = 0x0A,
        SourceAlphaSaturated = 0x0B
    }

    public enum PiptPreset
    {
        None = 0,
        Default = 9961474,
        VertexColors = 9961538,
        AlphaBlend = 9987330,
        AlphaBlendVertexColors = 9987394,
        AdditiveAlpha = 10036486,
        AdditiveAlphaVertexColors = 10036550,
    }

    public enum LightingMode
    {
        LightKit = 0,
        Prelight = 1,
        Both = 2,
        Unknown = 3
    }

    public enum PiptCullMode
    {
        Unknown = 0,
        None = 1,
        Back = 2,
        Dual = 3
    }

    public enum ZWriteMode
    {
        Enabled = 0,
        Disabled = 1,
        Dual = 2,
        Unknown = 3
    }

    public class PipeInfo : GenericAssetDataContainer
    {
        private const string categoryName = "Pipe Info";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public FlagBitmask SubObjectBits { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public int PipeFlags { get; set; }
        [Category(categoryName)]
        public PiptPreset PipeFlags_Preset
        {
            get
            {
                foreach (PiptPreset v in Enum.GetValues(typeof(PiptPreset)))
                    if ((int)v == PipeFlags)
                        return v;
                return PiptPreset.None;
            }
            set
            {
                PipeFlags = (int)value;
            }
        }

        private const string categoryNameFlags = "Pipe Info Flags";

        [Category(categoryNameFlags), Description("0 - 255")]
        public byte AlphaCompareValue
        {
            get => (byte)((PipeFlags & 0xFF000000) >> 24);

            set
            {
                PipeFlags &= 0x00FFFFFF;
                PipeFlags |= value << 24;
            }
        }

        [Category(categoryNameFlags), Description("0 - 15")]
        public byte UnknownFlagB
        {
            get => (byte)((PipeFlags & 0x00F00000) >> 20);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFF0FFFFF;
                    PipeFlags |= value << 20;
                }
            }
        }

        [Category(categoryNameFlags), Description("0 - 7")]
        public byte UnknownFlagC
        {
            get => (byte)((PipeFlags & 0x000E0000) >> 17);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFF1FFFF;
                    PipeFlags |= value << 17;
                }
            }
        }

        [Category(categoryNameFlags)]
        public bool IgnoreFog
        {
            get => ((PipeFlags & 0x00010000) >> 16) != 0;

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFEFFFF;
                    PipeFlags |= (value ? 1 : 0) << 16;
                }
            }
        }

        [Category(categoryNameFlags)]
        public BlendFactorType DestinationBlend
        {
            get => (BlendFactorType)((PipeFlags & 0x0000F000) >> 12);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFF0FFF;
                    PipeFlags |= (byte)value << 12;
                }
            }
        }

        [Category(categoryNameFlags)]
        public BlendFactorType SourceBlend
        {
            get => (BlendFactorType)((PipeFlags & 0x0000F00) >> 8);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFF0FF;
                    PipeFlags |= (byte)value << 8;
                }
            }
        }

        [Category(categoryNameFlags)]
        public LightingMode LightingMode
        {
            get => (LightingMode)((PipeFlags & 0x000000C0) >> 6);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFF3F;
                    PipeFlags |= (byte)value << 6;
                }
            }
        }

        [Category(categoryNameFlags)]
        public PiptCullMode CullMode
        {
            get => (PiptCullMode)((PipeFlags & 0x00000030) >> 4);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFFCF;
                    PipeFlags |= (byte)value << 4;
                }
            }
        }

        [Category(categoryNameFlags)]
        public ZWriteMode ZWriteMode
        {
            get => (ZWriteMode)((PipeFlags & 0x000000C) >> 2);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFFF3;
                    PipeFlags |= (byte)value << 2;
                }
            }
        }

        [Category(categoryNameFlags), Description("0 - 3")]
        public byte UnknownFlagJ
        {
            get => (byte)(PipeFlags & 0x00000003);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFFFC;
                    PipeFlags |= value;
                }
            }
        }

        [Category(categoryName + " (Movie/Incredibles only)")]
        public AssetID Unknown { get; set; }

        public PipeInfo()
        {
            Model = 0;
            SubObjectBits.FlagValueInt = 0xFFFFFFFF;
            UnknownFlagB = 9;
            UnknownFlagC = 4;
            UnknownFlagJ = 2;
        }

        public override string ToString()
        {
            return $"{HexUIntTypeConverter.StringFromAssetID(Model)} - {SubObjectBits}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is PipeInfo entry)
                return Model.Equals(entry.Model);
            return false;
        }

        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        public PipeInfo(EndianBinaryReader reader, Game game)
        {
            _game = game;

            Model = reader.ReadUInt32();
            SubObjectBits.FlagValueInt = reader.ReadUInt32();
            PipeFlags = reader.ReadInt32();
            if (game == Game.Incredibles)
                Unknown = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Model);
            writer.Write(SubObjectBits.FlagValueInt);
            writer.Write(PipeFlags);

            if (game == Game.Incredibles)
                writer.Write(Unknown);
        }
    }

    public class AssetPIPT : Asset, IAssetAddSelected
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private PipeInfo[] _entries { get; set; }
        [Category("Pipe Info Table")]
        public PipeInfo[] Entries
        {
            get => _entries;
            set
            {
                _entries = value;
                UpdateDictionary();
            }
        }

        public AssetPIPT(string assetName) : base(assetName, AssetType.PipeInfoTable)
        {
            Entries = new PipeInfo[0];
        }

        public AssetPIPT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                _entries = new PipeInfo[reader.ReadInt32()];

                for (int i = 0; i < _entries.Length; i++)
                    _entries[i] = new PipeInfo(reader, game);

                UpdateDictionary();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_entries.Length);

            foreach (var l in _entries)
                l.Serialize(writer);
        }

        public AssetPIPT(Section_AHDR AHDR, Game game, Endianness endianness, OnPipeInfoTableEdited onPipeInfoTableEdited) : this(AHDR, game, endianness)
        {
            this.onPipeInfoTableEdited = onPipeInfoTableEdited;
        }

        public delegate void OnPipeInfoTableEdited(Dictionary<uint, (uint, BlendFactorType, BlendFactorType)[]> blendModes);
        private readonly OnPipeInfoTableEdited onPipeInfoTableEdited;

        public void UpdateDictionary()
        {
            ClearDictionary();

            Dictionary<uint, (uint, BlendFactorType, BlendFactorType)[]> BlendModes = new Dictionary<uint, (uint, BlendFactorType, BlendFactorType)[]>();

            foreach (PipeInfo entry in Entries)
            {
                if (!BlendModes.ContainsKey(entry.Model))
                    BlendModes[entry.Model] = new (uint, BlendFactorType, BlendFactorType)[0];

                var entries = BlendModes[entry.Model].ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].Item1 == entry.SubObjectBits.FlagValueInt)
                        entries.RemoveAt(i--);

                entries.Add((entry.SubObjectBits.FlagValueInt, entry.SourceBlend, entry.DestinationBlend));
                BlendModes[entry.Model] = entries.ToArray();
            }

            onPipeInfoTableEdited?.Invoke(BlendModes);
        }

        public void ClearDictionary()
        {
            onPipeInfoTableEdited?.Invoke(null);
        }

        public void Merge(AssetPIPT asset)
        {
            var entries = Entries.ToList();

            foreach (var entry in asset.Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            Entries = entries.ToArray();
        }

        [Browsable(false)]
        public string GetItemsText => "entries";

        public void AddItems(List<uint> items)
        {
            var entries = Entries.ToList();
            foreach (var i in items)
                if (!entries.Any(e => e.Model == i))
                    entries.Add(new PipeInfo() { Model = i, PipeFlags_Preset = PiptPreset.AlphaBlend });
            Entries = entries.ToArray();
        }

        public void AddEntry(PipeInfo entry)
        {
            var entries = Entries.ToList();
            entries.Add(entry);
            Entries = entries.ToArray();
        }

        public void AddEntries(List<PipeInfo> entries2)
        {
            var entries = Entries.ToList();
            entries.AddRange(entries2);
            Entries = entries.ToArray();
        }

        public void RemoveEntry(uint assetID)
        {
            var entries = Entries.ToList();
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].Model == assetID)
                    entries.RemoveAt(i--);
            Entries = entries.ToArray();
        }
    }
}
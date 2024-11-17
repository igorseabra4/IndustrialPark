using HipHopFile;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public enum PIPTLayerType
    {
        FIRST = 0,
        PREPICKUP = 1,
        POSTPICKUP = 2,
        PREOOB = 3,
        POSTOOB = 4,
        PRECUTSCENE = 5,
        POSTCUTSCENE = 6,
        PRENPC = 7,
        POSTNPC = 8,
        PRESHADOW = 9,
        POSTSHADOW = 10,
        PREFX = 11,
        POSTFX = 12,
        PREPARTICLES = 13,
        POSTPARTICELS = 14,
        PRENORMAL4 = 15,
        PRENORMAL3 = 16,
        PRENORMAL2 = 17,
        PRENORMAL = 18,
        NORMAL = 19,
        POSTNORMAL = 20,
        POSTNORMAL2 = 21,
        POSTNORMAL3 = 22,
        POSTNORMAL4 = 23,
        PREPTANK = 24,
        POSTPTANK = 25,
        PREDECAL = 26,
        POSTDECAL = 27,
        PRELASTFX = 28,
        POSTLASTFX = 29,
        PRELAST = 30,
        LAST = 31
    }

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
        BackThenFront = 3
    }

    public enum ZWriteMode
    {
        Enabled = 0,
        Disabled = 1,
        ZFirst = 2
    }

    public class PipeInfo : GenericAssetDataContainer
    {
        private const string categoryName = "Pipe Info";

        [Category(categoryName), ValidReferenceRequired]
        public AssetID Model { get; set; }
        [Category(categoryName)]
        public FlagBitmask SubObjectBits { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public uint PipeFlags { get; set; }
        [Category(categoryName)]
        public PiptPreset PipeFlags_Preset
        {
            get
            {
                foreach (PiptPreset v in Enum.GetValues(typeof(PiptPreset)))
                    if ((uint)v == PipeFlags)
                        return v;
                return PiptPreset.None;
            }
            set
            {
                PipeFlags = (uint)value;
            }
        }

        private const string categoryNameFlags = "Pipe Info Flags";

        [Category(categoryNameFlags)]
        public bool HDR_Brightening
        {
            get => ((PipeFlags & 0x00400000) >> 22) != 0;
            set
            {
                PipeFlags &= ~0x00400000u;
                PipeFlags |= (uint)(value ? 1 : 0) << 22;
            }
        }

        [Category(categoryNameFlags)]
        public bool IgnoreFog
        {
            get
            {
                if (game >= Game.Incredibles)
                    return ((PipeFlags & 0x8) >> 3) != 0;
                else
                    return ((PipeFlags & 0x00010000) >> 16) != 0;
            }
            set
            {
                if (game >= Game.Incredibles)
                {
                    PipeFlags &= ~0x8u;
                    PipeFlags |= (uint)(value ? 1 : 0) << 3;
                }
                else
                {
                    PipeFlags &= ~0x10000u;
                    PipeFlags |= (uint)(value ? 1 : 0) << 16;
                }
            }
        }

        [Category(categoryNameFlags)]
        public BlendFactorType DestinationBlend
        {
            get => (BlendFactorType)((PipeFlags & 0x0000F000) >> 12);

            set
            {
                PipeFlags &= ~0xF000u;
                PipeFlags |= (uint)(byte)value << 12;
            }
        }

        [Category(categoryNameFlags)]
        public BlendFactorType SourceBlend
        {
            get => (BlendFactorType)((PipeFlags & 0x0000F00) >> 8);

            set
            {
                PipeFlags &= ~0xF00u;
                PipeFlags |= (uint)(byte)value << 8;
            }
        }

        [Category(categoryNameFlags)]
        public LightingMode LightingMode
        {
            get => (LightingMode)((PipeFlags & 0x000000C0) >> 6);

            set
            {
                PipeFlags &= ~0xC0u;
                PipeFlags |= (uint)(byte)value << 6;
            }
        }

        [Category(categoryNameFlags)]
        public PiptCullMode CullMode
        {
            get => (PiptCullMode)((PipeFlags & 0x30) >> 4);

            set
            {
                PipeFlags &= ~0x30u;
                PipeFlags |= (uint)(byte)value << 4;
            }
        }

        [Category(categoryNameFlags)]
        public ZWriteMode ZWriteMode
        {
            get
            {
                if (game >= Game.Incredibles)
                    return (ZWriteMode)((PipeFlags & 0x30000) >> 16);
                else
                    return (ZWriteMode)((PipeFlags & 0x4) >> 2);
            }
            set
            {
                if (game >= Game.Incredibles)
                {
                    PipeFlags &= ~0x30000u;
                    PipeFlags |= (uint)value << 16;
                }
                else
                {
                    PipeFlags &= ~0x4u;
                    PipeFlags |= (uint)value << 2;
                }

            }
        }

        private PIPTLayerType _layer;
        [Category(categoryName)]
        public PIPTLayerType Layer
        {
            get
            {
                if (game >= Game.Incredibles)
                    return _layer;
                else
                    return (PIPTLayerType)((PipeFlags & 0xF80000) >> 19);
            }
            set
            {
                if (game >= Game.Incredibles)
                    _layer = value;
                else
                {
                    PipeFlags &= 0xF80000;
                    PipeFlags |= (uint)value << 19;
                }
                    
            }
        }

        private byte _alphadiscard;
        [Category(categoryName)]
        public byte AlphaDiscard
        {
            get
            {
                if (game >= Game.Incredibles)
                    return _alphadiscard;
                else
                    return (byte)(PipeFlags >> 24);
            }
            set
            {
                if (game >= Game.Incredibles)
                    _alphadiscard = value;
                else
                    PipeFlags = (PipeFlags & ~0xFF000000) | (uint)(value << 24);
            }
        }

        [Category(categoryName)]
        public ushort PipePad { get; set; }

        private const uint BFBBUnknownFlagsMask = 0xE0003;
        private const uint UnknownFlagsMask = 0xFC7C0007;
        [Category(categoryNameFlags)]
        public uint UnknownFlags
        {
            get
            {
                if (game >= Game.Incredibles)
                    return PipeFlags & UnknownFlagsMask;
                else
                    return PipeFlags & BFBBUnknownFlagsMask;
            }
            set
            {
                if (game >= Game.Incredibles)
                {
                    PipeFlags &= ~UnknownFlagsMask;
                    PipeFlags |= (value & UnknownFlagsMask);
                }
                else
                {
                    PipeFlags &= ~BFBBUnknownFlagsMask;
                    PipeFlags |= value & BFBBUnknownFlagsMask;
                }
            }
        }

        public PipeInfo(Game game)
        {
            _game = game;

            Model = 0;
            SubObjectBits.FlagValueInt = 0xFFFFFFFF;
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

        public BlendOption GetSharpBlendMode(bool src)
        {
            switch (src ? SourceBlend : DestinationBlend)
            {
                case BlendFactorType.Zero:
                    return BlendOption.Zero;
                case BlendFactorType.One:
                    return BlendOption.One;
                case BlendFactorType.SourceColor:
                    return BlendOption.SourceColor;
                case BlendFactorType.InverseSourceColor:
                    return BlendOption.InverseSourceColor;
                case BlendFactorType.SourceAlpha:
                    return BlendOption.SourceAlpha;
                case BlendFactorType.InverseSourceAlpha:
                    return BlendOption.InverseSourceAlpha;
                case BlendFactorType.DestinationAlpha:
                    return BlendOption.DestinationAlpha;
                case BlendFactorType.InverseDestinationAlpha:
                    return BlendOption.InverseDestinationAlpha;
                case BlendFactorType.DestinationColor:
                    return BlendOption.DestinationColor;
                case BlendFactorType.InverseDestinationColor:
                    return BlendOption.InverseDestinationColor;
                case BlendFactorType.SourceAlphaSaturated:
                    return BlendOption.SourceAlphaSaturate;
            }

            return src ? BlendOption.Zero : BlendOption.One;
        }

        public PipeInfo(EndianBinaryReader reader, Game game)
        {
            _game = game;

            Model = reader.ReadUInt32();
            SubObjectBits.FlagValueInt = reader.ReadUInt32();
            PipeFlags = reader.ReadUInt32();

            if (game >= Game.Incredibles)
            {
                Layer = (PIPTLayerType)reader.ReadByte();
                AlphaDiscard = reader.ReadByte();
                PipePad = reader.ReadUInt16();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Model);
            writer.Write(SubObjectBits.FlagValueInt);
            writer.Write(PipeFlags);

            if (game >= Game.Incredibles)
            {
                writer.Write((byte)Layer);
                writer.Write(AlphaDiscard);
                writer.Write(PipePad);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game < Game.Incredibles)
            {
                dt.RemoveProperty("HDR_Brightening");
                dt.RemoveProperty("PipePad");
            }
        }
    }

    public class AssetPIPT : Asset, IAssetAddSelected
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        private PipeInfo[] _entries { get; set; }
        [Category("Pipe Info Table"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
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

        public delegate void OnPipeInfoTableEdited(Dictionary<uint, PipeInfo[]> pipeEntries);
        private readonly OnPipeInfoTableEdited onPipeInfoTableEdited;

        public void UpdateDictionary()
        {
            Dictionary<uint, PipeInfo[]> piptEntries = new Dictionary<uint, PipeInfo[]>();

            foreach (PipeInfo entry in Entries)
            {
                if (!piptEntries.ContainsKey(entry.Model))
                    piptEntries[entry.Model] = new PipeInfo[0];

                var entries = piptEntries[entry.Model].ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].SubObjectBits.FlagValueInt == entry.SubObjectBits.FlagValueInt)
                        entries.RemoveAt(i--);

                entries.Add(entry);
                piptEntries[entry.Model] = entries.ToArray();
            }

            onPipeInfoTableEdited?.Invoke(piptEntries);
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
                    entries.Add(new PipeInfo(game) { Model = i, PipeFlags_Preset = PiptPreset.AlphaBlend });
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
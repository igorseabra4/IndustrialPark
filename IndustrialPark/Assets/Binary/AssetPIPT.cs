using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public enum BlendFactorType
    {
        None = 0x00,
        Zero                           = 0x01,
        One                            = 0x02,
        SourceColor                    = 0x03,
        InverseSourceColor             = 0x04,
        SourceAlpha                    = 0x05,
        InverseSourceAlpha             = 0x06,
        DestinationAlpha               = 0x07,
        InverseDestinationAlpha        = 0x08,
        DestinationColor               = 0x09,
        InverseDestinationColor        = 0x0A,
        SourceAlphaSaturated           = 0x0B
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

    public class EntryPIPT
    {
        [Category("PIPT Entry")]
        public AssetID ModelAssetID { get; set; }
        [Category("PIPT Entry")]
        public int SubObjectBits { get; set; }
        [Category("PIPT Entry")]
        public int PipeFlags { get; set; }
        [Category("PIPT Entry")]
        public PiptPreset Preset
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

        [Category("PIPT Pipe Flags")]
        public byte AlphaCompareValue
        {
            get => (byte)((PipeFlags & 0xFF000000) >> 24);

            set
            {
                PipeFlags &= 0x00FFFFFF;
                PipeFlags |= value << 24;
            }
        }

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Pipe Flags")]
        public byte LightingMode
        {
            get => (byte)((PipeFlags & 0x000000C0) >> 6);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFF3F;
                    PipeFlags |= value << 6;
                }
            }
        }

        [Category("PIPT Pipe Flags")]
        public byte CullMode
        {
            get => (byte)((PipeFlags & 0x00000030) >> 4);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFFCF;
                    PipeFlags |= value << 4;
                }
            }
        }

        [Category("PIPT Pipe Flags")]
        public byte ZWriteMode
        {
            get => (byte)((PipeFlags & 0x000000C) >> 2);

            set
            {
                unchecked
                {
                    PipeFlags &= (int)0xFFFFFFF3;
                    PipeFlags |= value << 2;
                }
            }
        }

        [Category("PIPT Pipe Flags")]
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

        [Category("PIPT Entry (Incredibles only)")]
        public int Unknown { get; set; }

        public EntryPIPT()
        {
            ModelAssetID = 0;
            SubObjectBits = -1;
            UnknownFlagB = 9;
            UnknownFlagC = 4;
            UnknownFlagJ = 2;
        }

        public override string ToString()
        {
            return $"{Program.MainForm.GetAssetNameFromID(ModelAssetID)} - {SubObjectBits}";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is EntryPIPT entry)
                return ModelAssetID.Equals(entry.ModelAssetID);
            return false;
        }

        public override int GetHashCode()
        {
            return ModelAssetID.GetHashCode();
        }

        public EntryPIPT(EndianBinaryReader reader, Game game)
        {
            ModelAssetID = reader.ReadUInt32();
            SubObjectBits = reader.ReadInt32();
            PipeFlags = reader.ReadInt32();
            if (game == Game.Incredibles)
                Unknown = reader.ReadInt32();
        }

        public byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(ModelAssetID);
                writer.Write(SubObjectBits);
                writer.Write(PipeFlags);

                if (game == Game.Incredibles)
                    writer.Write(Unknown);

                return writer.ToArray();
            }
        }
    }

    public class AssetPIPT : Asset
    {
        private EntryPIPT[] _pipt_Entries { get; set; }
        [Category("Pipe Info Table")]
        public EntryPIPT[] PIPT_Entries
        {
            get => _pipt_Entries;
            set
            {
                _pipt_Entries = value; 
                UpdateDictionary();
            }
        }

        public AssetPIPT(string assetName) : base(assetName, AssetType.PIPT)
        {
            PIPT_Entries = new EntryPIPT[0];
        }

        public AssetPIPT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                _pipt_Entries = new EntryPIPT[reader.ReadInt32()];

                for (int i = 0; i < _pipt_Entries.Length; i++)
                    _pipt_Entries[i] = new EntryPIPT(reader, game);

                UpdateDictionary();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(_pipt_Entries.Length);

                foreach (var l in _pipt_Entries)
                    writer.Write(l.Serialize(game, endianness));

                return writer.ToArray();
            }
        }

        public AssetPIPT(Section_AHDR AHDR, Game game, Endianness endianness, OnPipeInfoTableEdited onPipeInfoTableEdited) : this(AHDR, game, endianness)
        {
            this.onPipeInfoTableEdited = onPipeInfoTableEdited;
        }

        public delegate void OnPipeInfoTableEdited(Dictionary<uint, (int, BlendFactorType, BlendFactorType)[]> blendModes);
        private readonly OnPipeInfoTableEdited onPipeInfoTableEdited;

        public override bool HasReference(uint assetID)
        {
            foreach (EntryPIPT a in PIPT_Entries)
                if (a.ModelAssetID == assetID)
                    return true;

            return base.HasReference(assetID);
        }

        public override void Verify(ref List<string> result)
        {
            foreach (EntryPIPT a in PIPT_Entries)
            {
                if (a.ModelAssetID == 0)
                    result.Add("PIPT entry with ModelAssetID set to 0");
                Verify(a.ModelAssetID, ref result);
            }
        }

        public void UpdateDictionary()
        {
            ClearDictionary();

            Dictionary<uint, (int, BlendFactorType, BlendFactorType)[]> BlendModes = new Dictionary<uint, (int, BlendFactorType, BlendFactorType)[]>();

            foreach (EntryPIPT entry in PIPT_Entries)
            {
                if (!BlendModes.ContainsKey(entry.ModelAssetID))
                    BlendModes[entry.ModelAssetID] = new (int, BlendFactorType, BlendFactorType)[0];

                var entries = BlendModes[entry.ModelAssetID].ToList();
                for (int i = 0; i < entries.Count; i++)
                    if (entries[i].Item1 == entry.SubObjectBits)
                        entries.RemoveAt(i--);

                entries.Add((entry.SubObjectBits, entry.SourceBlend, entry.DestinationBlend));
                BlendModes[entry.ModelAssetID] = entries.ToArray();
            }

            onPipeInfoTableEdited?.Invoke(BlendModes);
        }

        public void ClearDictionary()
        {
            onPipeInfoTableEdited?.Invoke(null);
        }

        public void Merge(AssetPIPT asset)
        {
            var entries = PIPT_Entries.ToList();

            foreach (var entry in asset.PIPT_Entries)
            {
                entries.Remove(entry);
                entries.Add(entry);
            }

            PIPT_Entries = entries.ToArray();
        }
    }
}
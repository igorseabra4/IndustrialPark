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

        [Category("PIPT Entry (Movie only)")]
        public int Unknown { get; set; }

        public static int SizeOfStruct(Game game) => game == Game.Incredibles ? 16 : 12;

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
    }

    public class AssetPIPT : Asset
    {
        public static Dictionary<uint, (int, BlendFactorType, BlendFactorType)> BlendModes = new Dictionary<uint, (int, BlendFactorType, BlendFactorType)>();

        public AssetPIPT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            UpdateDictionary();
        }

        public void UpdateDictionary()
        {
            foreach (EntryPIPT entry in PIPT_Entries)
                BlendModes[entry.ModelAssetID] = (entry.SubObjectBits, entry.SourceBlend, entry.DestinationBlend);
        }

        public void ClearDictionary()
        {
            foreach (EntryPIPT entry in PIPT_Entries)
                BlendModes.Remove(entry.ModelAssetID);
        }

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

        [Category("Pipe Table")]
        public EntryPIPT[] PIPT_Entries
        {
            get
            {
                List<EntryPIPT> entries = new List<EntryPIPT>();
                
                for (int i = 4; i < Data.Length; i += EntryPIPT.SizeOfStruct(game))
                {
                    EntryPIPT a = new EntryPIPT()
                    {
                        ModelAssetID = ReadUInt(i),
                        SubObjectBits = ReadInt(i + 4),
                        PipeFlags = ReadInt(i + 8)
                    };

                    if (game == Game.Incredibles)
                        a.Unknown = ReadInt(i + 12);
                    
                    entries.Add(a);
                }
                
                return entries.ToArray();
            }
            set
            {
                List<byte> newData = new List<byte>();
                newData.AddRange(BitConverter.GetBytes(Switch(value.Length)));

                foreach (EntryPIPT i in value)
                {
                    newData.AddRange(BitConverter.GetBytes(Switch(i.ModelAssetID)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.SubObjectBits)));
                    newData.AddRange(BitConverter.GetBytes(Switch(i.PipeFlags)));

                    if (game == Game.Incredibles)
                        newData.AddRange(BitConverter.GetBytes(Switch(i.Unknown)));
                }
                
                Data = newData.ToArray();
                UpdateDictionary();
            }
        }

        public void Merge(AssetPIPT assetPIPT)
        {
            List<EntryPIPT> entriesPIPT = PIPT_Entries.ToList();
            List<uint> assetIDsAlreadyPresent = new List<uint>();

            foreach (EntryPIPT entryPIPT in entriesPIPT)
                assetIDsAlreadyPresent.Add(entryPIPT.ModelAssetID);

            foreach (EntryPIPT entryPIPT in assetPIPT.PIPT_Entries)
                if (!assetIDsAlreadyPresent.Contains(entryPIPT.ModelAssetID))
                    entriesPIPT.Add(entryPIPT);

            PIPT_Entries = entriesPIPT.ToArray();
        }
    }
}
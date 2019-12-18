using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public enum BlendFactorType
    {
        SourceAlpha_InverseSourceAlpha = 0x00,
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

    public class EntryPIPT
    {
        [Category("PIPT Entry")]
        public AssetID ModelAssetID { get; set; }
        [Category("PIPT Entry")]
        public int MeshIndex { get; set; }
        [Category("PIPT Entry"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte RelatedToVisibility { get; set; }
        [Category("PIPT Entry"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Culling { get; set; }
        [Category("PIPT Entry"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte DestinationSourceBlend { get; set; }

        [Category("PIPT Entry (Helper)")]
        public BlendFactorType DestinationBlend
        {
            get => (BlendFactorType)(DestinationSourceBlend & 0x0F);
            set
            {
                DestinationSourceBlend &= 0xF0;
                DestinationSourceBlend |= (byte)value;
            }
        }

        [Category("PIPT Entry (Helper)")]
        public BlendFactorType SourceBlend
        {
            get => (BlendFactorType)((DestinationSourceBlend & 0xF0) >> 4);
            set
            {
                DestinationSourceBlend &= 0x0F;
                DestinationSourceBlend |= (byte)((byte)value << 4);
            }
        }

        [Category("PIPT Entry"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte OtherFlags { get; set; }
        [Category("PIPT Entry (Movie only)"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown0C { get; set; }
        [Category("PIPT Entry (Movie only)"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown0D { get; set; }
        [Category("PIPT Entry (Movie only)"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown0E { get; set; }
        [Category("PIPT Entry (Movie only)"), TypeConverter(typeof(HexByteTypeConverter))]
        public byte Unknown0F { get; set; }

        public static int SizeOfStruct(Game game) => game == Game.Incredibles ? 16 : 12;

        public EntryPIPT()
        {
            ModelAssetID = 0;
        }
        
        public override string ToString()
        {
            return $"{Program.MainForm.GetAssetNameFromID(ModelAssetID)} - {MeshIndex}";
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
                BlendModes[entry.ModelAssetID] = (entry.MeshIndex, entry.DestinationBlend, entry.SourceBlend);
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
                    byte[] Flags = BitConverter.GetBytes(ReadInt(i + 8));

                    EntryPIPT a = new EntryPIPT()
                    {
                        ModelAssetID = ReadUInt(i),
                        MeshIndex = ReadInt(i + 4),
                        RelatedToVisibility = Flags[3],
                        Culling = Flags[2],
                        DestinationSourceBlend = Flags[1],
                        OtherFlags = Flags[0]
                    };

                    if (game == Game.Incredibles)
                    {
                        a.Unknown0C = ReadByte(i + 12);
                        a.Unknown0D = ReadByte(i + 13);
                        a.Unknown0E = ReadByte(i + 14);
                        a.Unknown0F = ReadByte(i + 15);
                    }

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
                    newData.AddRange(BitConverter.GetBytes(Switch(i.MeshIndex)));
                    int Flags = BitConverter.ToInt32(new byte[] { i.OtherFlags, i.DestinationSourceBlend, i.Culling, i.RelatedToVisibility }, 0);
                    newData.AddRange(BitConverter.GetBytes(Switch(Flags)));

                    if (game == Game.Incredibles)
                    {
                        newData.Add(i.Unknown0C);
                        newData.Add(i.Unknown0D);
                        newData.Add(i.Unknown0E);
                        newData.Add(i.Unknown0F);
                    }
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
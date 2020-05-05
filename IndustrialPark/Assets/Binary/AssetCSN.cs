using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using HipHopFile;

namespace IndustrialPark
{
    public enum CSNSec1EntryType
    {
        Model = 1,
        Animation = 2,
        Unknown_03 = 3,
        SoundName = 4,
        MTDJ = 6,
        ATDJ = 7,
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Animation_Entry : Section1_Entry
    {
        public int Unknown05 { get; set; }
        public float UnknownX { get; set; }
        public float UnknownY { get; set; }
        public float UnknownZ { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Section_Type3_Entry
    {   
        public int Unknown01 { get; set; }
        public float UnknownFloat01 { get; set; }
        public float UnknownFloat02 { get; set; }
        public float UnknownFloat03 { get; set; }
        public float UnknownFloat04 { get; set; }
        public float UnknownFloat05 { get; set; }
        public float UnknownFloat06 { get; set; }
        public float UnknownFloat07 { get; set; }
        public float UnknownFloat08 { get; set; }
        public float UnknownFloat09 { get; set; }
        public float UnknownFloat10 { get; set; }
        public float UnknownFloat11 { get; set; }
        public float UnknownFloat12 { get; set; }
        public float UnknownFloat13 { get; set; }
        public float UnknownFloat14 { get; set; }
        public float UnknownFloat15 { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SectionEntry_Type3 : Section1_Entry
    {
        public int AmountOfEntries { get; set; }
        public Section_Type3_Entry[] Entries { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Section1_Entry
    {
        public CSNSec1EntryType Unknown01 { get; set; }
        public AssetID AssetID { get; set; }
        [ReadOnly(true)]
        public int fileSize { get; set; }
        [ReadOnly(true)]
        public int fileOffset { get; set; }
        public byte[] fileData { get; set; }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Section2_Entry : EndianConvertibleWithData
    {
        public override byte[] Data { get; set; }

        public Section2_Entry(byte[] data, Endianness endianness) : base(endianness)
        {
            Data = data;
        }

        public int Unknown01
        {
            get => ReadInt(0x00);
            set => Write(0x00, value);
        }
        public int Unknown02
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }
        [ReadOnly(true)]
        public int AnimCount
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }
        public int Unknown04
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }
        public int Unknown05
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }
        public int Unknown06
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }
        [ReadOnly(true)]
        public int SizeOfData1
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }
        public int Unknown08
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }
        public byte[] Data1
        {
            get
            {
                return Data.Skip(0x20).Take(SizeOfData1).ToArray();
            }
            set
            {
                List<byte> before = Data.Take(0x10).ToList();
                before.AddRange(value);
                while (before.Count != 0x10)
                    before.Add(0x00);
                before.AddRange(Data.Skip(0x10 + SizeOfData1).ToArray());
                Data = before.ToArray();
                SizeOfData1 = value.Length;
            }
        }

        public Section1_Entry[] Animations
        {
            get
            {
                int currentPos = 0x20 + SizeOfData1;
                var aEs = new List<Section1_Entry>();
                while (currentPos < Data.Length)
                {
                    CSNSec1EntryType entryType = (CSNSec1EntryType)ReadInt(currentPos);
                    if (entryType == CSNSec1EntryType.Animation)
                    {
                        var aE = new Animation_Entry
                        {
                            Unknown01 = entryType,
                            AssetID = ReadUInt(currentPos + 4),
                            fileSize = ReadInt(currentPos + 8),
                            fileOffset = ReadInt(currentPos + 12),
                            Unknown05 = ReadInt(currentPos + 16),
                            UnknownX = ReadInt(currentPos + 20),
                            UnknownY = ReadInt(currentPos + 24),
                            UnknownZ = ReadInt(currentPos + 28)
                        };
                        aE.fileData = Data.Skip(currentPos + 32).Take(aE.fileSize - 0x10).ToArray();
                        aEs.Add(aE);

                        currentPos += aE.fileSize + 0x10;
                    }
                    else if (entryType == CSNSec1EntryType.Unknown_03)
                    {
                        var aE = new SectionEntry_Type3
                        {
                            Unknown01 = entryType,
                            AssetID = ReadUInt(currentPos + 4),
                            fileSize = ReadInt(currentPos + 8),
                            fileOffset = ReadInt(currentPos + 12),
                            AmountOfEntries = ReadInt(currentPos + 16),
                        };
                        currentPos += 20;
                        aE.Entries = new Section_Type3_Entry[aE.AmountOfEntries];

                        for (int j = 0; j < aE.AmountOfEntries; j++)
                        {
                            aE.Entries[j] = new Section_Type3_Entry
                            {
                                Unknown01 = ReadInt(currentPos),
                                UnknownFloat01 = ReadInt(currentPos + 0x04),
                                UnknownFloat02 = ReadInt(currentPos + 0x08),
                                UnknownFloat03 = ReadInt(currentPos + 0x0C),
                                UnknownFloat04 = ReadInt(currentPos + 0x10),
                                UnknownFloat05 = ReadInt(currentPos + 0x14),
                                UnknownFloat06 = ReadInt(currentPos + 0x18),
                                UnknownFloat07 = ReadInt(currentPos + 0x1C),
                                UnknownFloat08 = ReadInt(currentPos + 0x20),
                                UnknownFloat09 = ReadInt(currentPos + 0x24),
                                UnknownFloat10 = ReadInt(currentPos + 0x28),
                                UnknownFloat11 = ReadInt(currentPos + 0x2C),
                                UnknownFloat12 = ReadInt(currentPos + 0x30),
                                UnknownFloat13 = ReadInt(currentPos + 0x34),
                                UnknownFloat14 = ReadInt(currentPos + 0x38),
                                UnknownFloat15 = ReadInt(currentPos + 0x3C)
                            };
                            currentPos += 0x40;
                        }
                        aEs.Add(aE);
                    }
                    else
                    {
                        var aE = new Section1_Entry
                        {
                            Unknown01 = entryType,
                            AssetID = ReadUInt(currentPos + 4),
                            fileSize = ReadInt(currentPos + 8),
                            fileOffset = ReadInt(currentPos + 12),
                        };
                        currentPos += 20;
                        aE.fileData = Data.Skip(currentPos + 16).Take(aE.fileSize).ToArray();
                        currentPos += aE.fileSize;

                        aEs.Add(aE);
                    }

                    while (currentPos % 0x10 != 0)
                        currentPos++;
                }

                return aEs.ToArray();
            }
        }
    }

    public class AssetCSN : Asset
    {
        public AssetCSN(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override bool HasReference(uint assetID)
        {
            return base.HasReference(assetID);
        }

        private const string categoryName = "Cutscene";

        [Category(categoryName)]
        public string VideoMode
        {
            get
            {
                string result = "";
                foreach (byte b in BitConverter.GetBytes(ReadInt(0x00)))
                    result += (char)b;
                return result;
            }

            set
            {
                var result = new List<byte>();
                foreach (char b in VideoMode)
                    result.Add((byte)b);
                Write(0, BitConverter.ToInt32(result.ToArray(), 0));
            }
        }

        [Category(categoryName)]
        public AssetID AssetID
        {
            get => ReadUInt(0x04);
            set => Write(0x04, value);
        }

        [Category(categoryName)]
        public int Section1_Entries_Count
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category(categoryName)]
        public int Section2_Entries_Count
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category(categoryName)]
        public int UnknownInt_10
        {
            get => ReadInt(0x10);
            set => Write(0x10, value);
        }

        [Category(categoryName)]
        public int UnknownInt_14
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category(categoryName)]
        public int UnknownInt_18
        {
            get => ReadInt(0x18);
            set => Write(0x18, value);
        }

        [Category(categoryName)]
        public int UnknownInt_1C
        {
            get => ReadInt(0x1C);
            set => Write(0x1C, value);
        }

        [Category(categoryName)]
        public CSNSec1EntryType SoundName_EntryType
        {
            get => (CSNSec1EntryType)ReadInt(0x20);
            set => Write(0x20, (int)value);
        }

        [Category(categoryName)]
        public int UnknownInt_24
        {
            get => ReadInt(0x24);
            set => Write(0x24, value);
        }

        [Category(categoryName)]
        public int UnknownInt_28
        {
            get => ReadInt(0x28);
            set => Write(0x28, value);
        }

        [Category(categoryName)]
        public int UnknownInt_2C
        {
            get => ReadInt(0x2C);
            set => Write(0x2C, value);
        }

        [Category(categoryName)]
        public string SoundName
        {
            get
            {
                string result = "";
                foreach (byte b in Data.Skip(0x30).Take(0x20))
                    if (b != '\0')
                        result += (char)b;
                return result;
            }

            set
            {
                while (value.Length < 0x20)
                    value += '\0';
                for (int i = 0; i < 0x20; i++)
                    Data[0x30 + i] = (byte)value[i];
            }
        }

        [Category(categoryName)]
        public Section1_Entry[] entryType1s
        {
            get
            {
                var entries = new Section1_Entry[Section1_Entries_Count];
                const int startPos = 0x50;
                const int varSize = 0x10;

                for (int i = 0; i < entries.Length; i++)
                {
                    entries[i] = new Section1_Entry()
                    {
                        Unknown01 = (CSNSec1EntryType)ReadInt(startPos + i * varSize + 0),
                        AssetID = ReadUInt(startPos + i * varSize + 4),
                        fileSize = ReadInt(startPos + i * varSize + 8),
                        fileOffset = ReadInt(startPos + i * varSize + 12),
                    };

                    entries[i].fileData = Data.Skip(entries[i].fileOffset).Take(entries[i].fileSize).ToArray();
                }
                return entries;
            }
        }

        [Category(categoryName)]
        public Section2_Entry[] entryType2s
        {
            get
            {
                var startOffsets = new int[Section2_Entries_Count + 1];
                int startPos = 0x50 + Section1_Entries_Count * 0x10;

                for (int i = 0; i < startOffsets.Length; i++)
                    startOffsets[i] = ReadInt(startPos + i * 4);

                var entries = new Section2_Entry[Section2_Entries_Count];
                for (int i = 0; i < startOffsets.Length - 1; i++)
                    entries[i] = new Section2_Entry(Data
                        .Skip(startOffsets[i])
                        .Take(startOffsets[i + 1] - startOffsets[i])
                        .ToArray(), EndianConverter.PlatformEndianness(platform));
                
                return entries;
            }
        }

        public void ExtractToFolder(string folderName)
        {
            foreach (Section1_Entry entry in entryType1s)
                if (entry.Unknown01 == CSNSec1EntryType.Model)
                {
                    if (entry.fileData != null && entry.fileData.Length > 0)
                        File.WriteAllBytes(
                            Path.Combine(folderName, Path.ChangeExtension(Program.MainForm.GetAssetNameFromID(entry.AssetID), ".dff")),
                            entry.fileData);
                    else
                        foreach (var ae in Program.MainForm.archiveEditors)
                            if (ae.archive.ContainsAsset(entry.AssetID))
                                File.WriteAllBytes(
                                    Path.Combine(folderName, Program.MainForm.GetAssetNameFromID(entry.AssetID)),
                                    ae.archive.GetFromAssetID(entry.AssetID).Data);
                }
            int j = 0;
            foreach (Section2_Entry entry in entryType2s)
            {
                int k = 0;

                foreach (var animEntry in entry.Animations)
                    if (animEntry.Unknown01 == CSNSec1EntryType.Animation)
                        if (animEntry.fileData != null && animEntry.fileData.Length > 0)
                        {
                            File.WriteAllBytes(
                                Path.Combine(folderName,
                                $"skb1_s{j}_a{k}_" + 
                                Path.GetFileNameWithoutExtension(Program.MainForm.GetAssetNameFromID(animEntry.AssetID))),
                                animEntry.fileData);
                            k++;
                        }
                j++;
            }
        }
    }
}
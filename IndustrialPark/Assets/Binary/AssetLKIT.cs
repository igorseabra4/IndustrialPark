using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using static IndustrialPark.ConverterFunctions;

namespace IndustrialPark
{
    public class EntryLKIT
    {
        public int Type { get; set; }
        public float Unknown01 { get; set; }
        public float Unknown02 { get; set; }
        public float Unknown03 { get; set; }
        public float Unknown04 { get; set; }
        public float Unknown05 { get; set; }
        public float Unknown06 { get; set; }
        public float Unknown07 { get; set; }
        public float Unknown08 { get; set; }
        public float Unknown09 { get; set; }
        public float Unknown10 { get; set; }
        public float Unknown11 { get; set; }
        public float Unknown12 { get; set; }
        public float Unknown13 { get; set; }
        public float Unknown14 { get; set; }
        public float Unknown15 { get; set; }
        public float Unknown16 { get; set; }
        public float Unknown17 { get; set; }
        public float Unknown18 { get; set; }
        public float Unknown19 { get; set; }
        public float Unknown20 { get; set; }
        public float Unknown21 { get; set; }
        public float Unknown22 { get; set; }
        public float Unknown23 { get; set; }
    }

    public class AssetLKIT : Asset
    {
        public AssetLKIT(Section_AHDR AHDR) : base(AHDR) { }

        [Category("Light Kit")]
        public int UnknownInt04
        {
            get => ReadInt(0x4);
            set => Write(0x4, value);
        }

        [Category("Light Kit"), ReadOnly(true)]
        public int AmountOfLights
        {
            get => ReadInt(0x8);
            set => Write(0x8, value);
        }

        [Category("Light Kit")]
        public int UnknownInt0C
        {
            get => ReadInt(0xC);
            set => Write(0xC, value);
        }

        [Category("Light Kit")]
        public EntryLKIT[] Lights
        {
            get
            {
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data.Skip(0x10).ToArray()));
                List<EntryLKIT> lights = new List<EntryLKIT>();
                for (int i = 0; i < AmountOfLights; i++)
                {
                    lights.Add(new EntryLKIT()
                    {
                        Type = Switch(binaryReader.ReadInt32()),
                        Unknown01 = Switch(binaryReader.ReadSingle()),
                        Unknown02 = Switch(binaryReader.ReadSingle()),
                        Unknown03 = Switch(binaryReader.ReadSingle()),
                        Unknown04 = Switch(binaryReader.ReadSingle()),
                        Unknown05 = Switch(binaryReader.ReadSingle()),
                        Unknown06 = Switch(binaryReader.ReadSingle()),
                        Unknown07 = Switch(binaryReader.ReadSingle()),
                        Unknown08 = Switch(binaryReader.ReadSingle()),
                        Unknown09 = Switch(binaryReader.ReadSingle()),
                        Unknown10 = Switch(binaryReader.ReadSingle()),
                        Unknown11 = Switch(binaryReader.ReadSingle()),
                        Unknown12 = Switch(binaryReader.ReadSingle()),
                        Unknown13 = Switch(binaryReader.ReadSingle()),
                        Unknown14 = Switch(binaryReader.ReadSingle()),
                        Unknown15 = Switch(binaryReader.ReadSingle()),
                        Unknown16 = Switch(binaryReader.ReadSingle()),
                        Unknown17 = Switch(binaryReader.ReadSingle()),
                        Unknown18 = Switch(binaryReader.ReadSingle()),
                        Unknown19 = Switch(binaryReader.ReadSingle()),
                        Unknown20 = Switch(binaryReader.ReadSingle()),
                        Unknown21 = Switch(binaryReader.ReadSingle()),
                        Unknown22 = Switch(binaryReader.ReadSingle()),
                        Unknown23 = Switch(binaryReader.ReadSingle())
                    });
                }

                return lights.ToArray();
            }
            set
            {
                BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(value.Length * 24 * 4));
                foreach (EntryLKIT entry in value)
                {
                    binaryWriter.Write(Switch(entry.Type));
                    binaryWriter.Write(Switch(entry.Unknown01));
                    binaryWriter.Write(Switch(entry.Unknown02));
                    binaryWriter.Write(Switch(entry.Unknown03));
                    binaryWriter.Write(Switch(entry.Unknown04));
                    binaryWriter.Write(Switch(entry.Unknown05));
                    binaryWriter.Write(Switch(entry.Unknown06));
                    binaryWriter.Write(Switch(entry.Unknown07));
                    binaryWriter.Write(Switch(entry.Unknown08));
                    binaryWriter.Write(Switch(entry.Unknown09));
                    binaryWriter.Write(Switch(entry.Unknown10));
                    binaryWriter.Write(Switch(entry.Unknown11));
                    binaryWriter.Write(Switch(entry.Unknown12));
                    binaryWriter.Write(Switch(entry.Unknown13));
                    binaryWriter.Write(Switch(entry.Unknown14));
                    binaryWriter.Write(Switch(entry.Unknown15));
                    binaryWriter.Write(Switch(entry.Unknown16));
                    binaryWriter.Write(Switch(entry.Unknown17));
                    binaryWriter.Write(Switch(entry.Unknown18));
                    binaryWriter.Write(Switch(entry.Unknown19));
                    binaryWriter.Write(Switch(entry.Unknown20));
                    binaryWriter.Write(Switch(entry.Unknown21));
                    binaryWriter.Write(Switch(entry.Unknown22));
                    binaryWriter.Write(Switch(entry.Unknown23));
                }

                List<byte> newData = Data.Take(0x10).ToList();
                newData.AddRange(((MemoryStream)binaryWriter.BaseStream).ToArray());
                Data = newData.ToArray();
                AmountOfLights = value.Length;
            }
        }
    }
}
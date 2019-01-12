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
        public float ColorR { get; set; }
        public float ColorG { get; set; }
        public float ColorB { get; set; }
        public float Unknown04 { get; set; }
        public float Unknown05_X { get; set; }
        public float Unknown06_Y { get; set; }
        public float Unknown07_Z { get; set; }
        public float Unknown08 { get; set; }
        public float Unknown09_X { get; set; }
        public float Unknown10_Y { get; set; }
        public float Unknown11_Z { get; set; }
        public float Unknown12 { get; set; }
        public float Unknown13_X { get; set; }
        public float Unknown14_Y { get; set; }
        public float Unknown15_Z { get; set; }
        public float Unknown16 { get; set; }
        public float Unknown17_X { get; set; }
        public float Unknown18_Y { get; set; }
        public float Unknown19_Z { get; set; }
        public float Unknown20 { get; set; }
        public float Unknown21_X { get; set; }
        public float Unknown22_Y { get; set; }
        public float Unknown23_Z { get; set; }
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
                        ColorR = Switch(binaryReader.ReadSingle()),
                        ColorG = Switch(binaryReader.ReadSingle()),
                        ColorB = Switch(binaryReader.ReadSingle()),
                        Unknown04 = Switch(binaryReader.ReadSingle()),
                        Unknown05_X = Switch(binaryReader.ReadSingle()),
                        Unknown06_Y = Switch(binaryReader.ReadSingle()),
                        Unknown07_Z = Switch(binaryReader.ReadSingle()),
                        Unknown08 = Switch(binaryReader.ReadSingle()),
                        Unknown09_X = Switch(binaryReader.ReadSingle()),
                        Unknown10_Y = Switch(binaryReader.ReadSingle()),
                        Unknown11_Z = Switch(binaryReader.ReadSingle()),
                        Unknown12 = Switch(binaryReader.ReadSingle()),
                        Unknown13_X = Switch(binaryReader.ReadSingle()),
                        Unknown14_Y = Switch(binaryReader.ReadSingle()),
                        Unknown15_Z = Switch(binaryReader.ReadSingle()),
                        Unknown16 = Switch(binaryReader.ReadSingle()),
                        Unknown17_X = Switch(binaryReader.ReadSingle()),
                        Unknown18_Y = Switch(binaryReader.ReadSingle()),
                        Unknown19_Z = Switch(binaryReader.ReadSingle()),
                        Unknown20 = Switch(binaryReader.ReadSingle()),
                        Unknown21_X = Switch(binaryReader.ReadSingle()),
                        Unknown22_Y = Switch(binaryReader.ReadSingle()),
                        Unknown23_Z = Switch(binaryReader.ReadSingle())
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
                    binaryWriter.Write(Switch(entry.ColorR));
                    binaryWriter.Write(Switch(entry.ColorG));
                    binaryWriter.Write(Switch(entry.ColorB));
                    binaryWriter.Write(Switch(entry.Unknown04));
                    binaryWriter.Write(Switch(entry.Unknown05_X));
                    binaryWriter.Write(Switch(entry.Unknown06_Y));
                    binaryWriter.Write(Switch(entry.Unknown07_Z));
                    binaryWriter.Write(Switch(entry.Unknown08));
                    binaryWriter.Write(Switch(entry.Unknown09_X));
                    binaryWriter.Write(Switch(entry.Unknown10_Y));
                    binaryWriter.Write(Switch(entry.Unknown11_Z));
                    binaryWriter.Write(Switch(entry.Unknown12));
                    binaryWriter.Write(Switch(entry.Unknown13_X));
                    binaryWriter.Write(Switch(entry.Unknown14_Y));
                    binaryWriter.Write(Switch(entry.Unknown15_Z));
                    binaryWriter.Write(Switch(entry.Unknown16));
                    binaryWriter.Write(Switch(entry.Unknown17_X));
                    binaryWriter.Write(Switch(entry.Unknown18_Y));
                    binaryWriter.Write(Switch(entry.Unknown19_Z));
                    binaryWriter.Write(Switch(entry.Unknown20));
                    binaryWriter.Write(Switch(entry.Unknown21_X));
                    binaryWriter.Write(Switch(entry.Unknown22_Y));
                    binaryWriter.Write(Switch(entry.Unknown23_Z));
                }

                List<byte> newData = Data.Take(0x10).ToList();
                newData.AddRange(((MemoryStream)binaryWriter.BaseStream).ToArray());
                Data = newData.ToArray();
                AmountOfLights = value.Length;
            }
        }
    }
}
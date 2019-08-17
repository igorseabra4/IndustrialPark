using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using AssetEditorColors;
using HipHopFile;

namespace IndustrialPark
{
    public class SectionEntry : EndianConvertible
    {
        private int _size;
        [ReadOnly(true)]
        public int Size
        {
            get
            {
                int size = 14 * 0x04 + NumberOfStyles * StyleEntry.SizeOfEntry;
                foreach (TitleEntry t in Titles)
                    size += t.Size;
                return size;
            }
            set => _size = value;
        }

        public float Duration { get; set; }
        public int Unknown_08 { get; set; }
        public float Unknown_0C { get; set; }
        public float BeginY { get; set; }
        public float Unknown_14 { get; set; }
        public float EndY { get; set; }
        public float Unknown_1C { get; set; }
        public float Unknown_20 { get; set; }
        public float FadeInBegin { get; set; }
        public float FadeInEnd { get; set; }
        public float FadeOutBegin { get; set; }
        public float FadeOutEnd { get; set; }

        [ReadOnly(true)]
        public int NumberOfStyles { get; set; }

        private StyleEntry[] _styles { get; set; }
        public StyleEntry[] Styles { get => _styles; set { _styles = value; NumberOfStyles = value.Length; } }
        public TitleEntry[] Titles { get; set; }
        public long Start;

        public void SetupStylesAndTitles(BinaryReader binaryReader)
        {
            List<StyleEntry> styles = new List<StyleEntry>();
            for (int i = 0; i < NumberOfStyles; i++)
            {
                styles.Add(new StyleEntry
                {
                    Unknown_00 = Switch(binaryReader.ReadInt16()),
                    Unknown_02 = Switch(binaryReader.ReadInt16()),
                    Unknown_04 = Switch(binaryReader.ReadSingle()),
                    Unknown_08 = Switch(binaryReader.ReadSingle()),

                    Unknown_00_1 = Switch(binaryReader.ReadInt32()),
                    Color1_Int = Switch(binaryReader.ReadInt32()),
                    CharWidth1 = Switch(binaryReader.ReadSingle()),
                    CharHeight1 = Switch(binaryReader.ReadSingle()),
                    Unknown1_1 = Switch(binaryReader.ReadSingle()),
                    Unknown2_1 = Switch(binaryReader.ReadSingle()),
                    MaxScreenWidth1 = Switch(binaryReader.ReadSingle()),
                    MaxScreenHeight1 = Switch(binaryReader.ReadSingle()),

                    Unknown_00_2 = Switch(binaryReader.ReadInt32()),
                    Color2_Int = Switch(binaryReader.ReadInt32()),
                    CharWidth2 = Switch(binaryReader.ReadSingle()),
                    CharHeight2 = Switch(binaryReader.ReadSingle()),
                    Unknown1_2 = Switch(binaryReader.ReadSingle()),
                    Unknown2_2 = Switch(binaryReader.ReadSingle()),
                    MaxScreenWidth2 = Switch(binaryReader.ReadSingle()),
                    MaxScreenHeight2 = Switch(binaryReader.ReadSingle())
                });
            }

            Styles = styles.ToArray();

            List<TitleEntry> titles = new List<TitleEntry>();
            while (binaryReader.BaseStream.Position < Start + _size)
            {
                TitleEntry title = new TitleEntry
                {
                    Size = Switch(binaryReader.ReadInt32()),
                    StyleIndex = Switch(binaryReader.ReadInt32()),
                    StartTime = Switch(binaryReader.ReadSingle()),
                    EndTime = Switch(binaryReader.ReadSingle()),
                    OffsetOfText = Switch(binaryReader.ReadInt32()),
                    Unknown = Switch(binaryReader.ReadInt32()),
                    Text = Functions.ReadString(binaryReader)
                };

                while (binaryReader.BaseStream.Position % 4 != 0)
                    binaryReader.BaseStream.Position++;

                titles.Add(title);
            }

            Titles = titles.ToArray();
        }

        public List<byte> StylesAndTitlesToArray(int absolutePosition)
        {
            List<byte> result = new List<byte>();

            foreach (StyleEntry s in Styles)
            {
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_00)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_02)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_04)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_08)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_00_1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Color1_Int)));
                result.AddRange(BitConverter.GetBytes(Switch(s.CharWidth1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.CharHeight1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown1_1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown2_1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.MaxScreenWidth1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.MaxScreenHeight1)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_00_2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Color2_Int)));
                result.AddRange(BitConverter.GetBytes(Switch(s.CharWidth2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.CharHeight2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown1_2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown2_2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.MaxScreenWidth2)));
                result.AddRange(BitConverter.GetBytes(Switch(s.MaxScreenHeight2)));
            }

            absolutePosition += result.Count;

            foreach (TitleEntry s in Titles)
            {
                result.AddRange(BitConverter.GetBytes(Switch(s.Size)));
                result.AddRange(BitConverter.GetBytes(Switch(s.StyleIndex)));
                result.AddRange(BitConverter.GetBytes(Switch(s.StartTime)));
                result.AddRange(BitConverter.GetBytes(Switch(s.EndTime)));
                result.AddRange(BitConverter.GetBytes(Switch(absolutePosition)));
                result.AddRange(BitConverter.GetBytes(Switch(s.Unknown)));
                Functions.AddString(result, s.Text);

                while (result.Count % 4 != 0)
                    result.Add(0);

                absolutePosition += s.Size;
            }

            return result;
        }

        public SectionEntry(Endianness endianness) : base(endianness)
        {
            Styles = new StyleEntry[0];
            Titles = new TitleEntry[0];
        }
    }

    public class StyleEntry
    {
        public short Unknown_00 { get; set; }
        public short Unknown_02 { get; set; }
        public float Unknown_04 { get; set; }
        public float Unknown_08 { get; set; }

        public int Unknown_00_1 { get; set; }

        public int Color1_Int;
        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor Color1
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(Color1_Int);
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Color1_Int = BitConverter.ToInt32(new byte[] { Color1Alpha, value.B, value.G, value.R }, 0);
        }

        [Category("Fog"), DisplayName("Color 1 Alpha (0 - 255)")]
        public byte Color1Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(Color1_Int);
                return abgr[0];
            }

            set => Color1_Int = BitConverter.ToInt32(new byte[] { value, Color1.B, Color1.G, Color1.R }, 0);
        }

        public float CharWidth1 { get; set; }
        public float CharHeight1 { get; set; }
        public float Unknown1_1 { get; set; }
        public float Unknown2_1 { get; set; }
        public float MaxScreenWidth1 { get; set; }
        public float MaxScreenHeight1 { get; set; }

        public int Unknown_00_2 { get; set; }

        public int Color2_Int;
        [Category("Fog"), Editor(typeof(MyColorEditor), typeof(UITypeEditor)), DisplayName("Color 1 (R, G, B)")]
        public MyColor Color2
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(Color2_Int);
                return new MyColor(abgr[3], abgr[2], abgr[1], abgr[0]);
            }

            set => Color2_Int = BitConverter.ToInt32(new byte[] { Color2Alpha, value.B, value.G, value.R }, 0);
        }

        [Category("Fog"), DisplayName("Color 2 Alpha (0 - 255)")]
        public byte Color2Alpha
        {
            get
            {
                byte[] abgr = BitConverter.GetBytes(Color2_Int);
                return abgr[0];
            }

            set => Color2_Int = BitConverter.ToInt32(new byte[] { value, Color2.B, Color2.G, Color2.R }, 0);
        }

        public float CharWidth2 { get; set; }
        public float CharHeight2 { get; set; }
        public float Unknown1_2 { get; set; }
        public float Unknown2_2 { get; set; }
        public float MaxScreenWidth2 { get; set; }
        public float MaxScreenHeight2 { get; set; }

        public static int SizeOfEntry => 0x4C;
    }

    public class TitleEntry
    {
        private int _size;
        [ReadOnly(true)]
        public int Size
        {
            get
            {
                int size = 6 * 0x04 + Text.Length;
                if (size % 4 == 0)
                    size += 4;
                while (size % 4 != 0)
                    size++;
                return size;
            }
            set => _size = value;
        }

        public int StyleIndex { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        [ReadOnly(true)]
        public int OffsetOfText { get; set; }
        public int Unknown { get; set; }
        public string Text { get; set; }

        public TitleEntry()
        {
            Text = "";
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class AssetCRDT : Asset
    {
        public AssetCRDT(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        public override void Verify(ref List<string> result)
        {
            SectionEntry[] sections = Sections;
        }

        [Category("Credits")]
        public int UnknownInt04
        {
            get => ReadInt(0x04);
            set => Write(0x04, value);
        }

        [Category("Credits")]
        public int UnknownInt08
        {
            get => ReadInt(0x08);
            set => Write(0x08, value);
        }

        [Category("Credits")]
        public int UnknownInt0C
        {
            get => ReadInt(0x0C);
            set => Write(0x0C, value);
        }

        [Category("Credits")]
        public float Duration
        {
            get => ReadFloat(0x10);
            set => Write(0x10, value);
        }

        [Category("Credits"), ReadOnly(true)]
        public int Size
        {
            get => ReadInt(0x14);
            set => Write(0x14, value);
        }

        [Category("Credits")]
        public SectionEntry[] Sections
        {
            get
            {
                BinaryReader binaryReader = new BinaryReader(new MemoryStream(Data.Skip(0x18).ToArray()));
                List<SectionEntry> sections = new List<SectionEntry>();

                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    SectionEntry section = new SectionEntry(EndianConverter.PlatformEndianness(platform))
                    {
                        Start = binaryReader.BaseStream.Position,
                        Size = Switch(binaryReader.ReadInt32()),
                        Duration = Switch(binaryReader.ReadSingle()),
                        Unknown_08 = Switch(binaryReader.ReadInt32()),
                        Unknown_0C = Switch(binaryReader.ReadSingle()),
                        BeginY = Switch(binaryReader.ReadSingle()),
                        Unknown_14 = Switch(binaryReader.ReadSingle()),
                        EndY = Switch(binaryReader.ReadSingle()),
                        Unknown_1C = Switch(binaryReader.ReadSingle()),
                        Unknown_20 = Switch(binaryReader.ReadSingle()),
                        FadeInBegin = Switch(binaryReader.ReadSingle()),
                        FadeInEnd = Switch(binaryReader.ReadSingle()),
                        FadeOutBegin = Switch(binaryReader.ReadSingle()),
                        FadeOutEnd = Switch(binaryReader.ReadSingle()),
                        NumberOfStyles = Switch(binaryReader.ReadInt32())
                    };

                    section.SetupStylesAndTitles(binaryReader);

                    sections.Add(section);
                }

                return sections.ToArray();
            }
            set
            {
                List<byte> result = Data.Take(0x18).ToList();

                foreach (SectionEntry s in value)
                {
                    result.AddRange(BitConverter.GetBytes(Switch(s.Size)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Duration)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_08)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_0C)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.BeginY)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_14)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.EndY)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_1C)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.Unknown_20)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.FadeInBegin)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.FadeInEnd)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.FadeOutBegin)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.FadeOutEnd)));
                    result.AddRange(BitConverter.GetBytes(Switch(s.NumberOfStyles)));

                    int absolutePosition = 0x18 + result.Count;

                    result.AddRange(s.StylesAndTitlesToArray(absolutePosition));
                }

                Data = result.ToArray();
                Size = Data.Length;
            }
        }
    }
}
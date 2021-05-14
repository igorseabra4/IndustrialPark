using System.Collections.Generic;
using System.ComponentModel;
using AssetEditorColors;
using HipHopFile;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class StyleStyleEntry : GenericAssetDataContainer
    {
        public int Unknown_00_1 { get; set; }
        [DisplayName("Color (R, G, B)")]
        public AssetColor Color { get; set; }
        [DisplayName("Color Alpha (0 - 255)")]
        public byte ColorAlpha
        {
            get => Color.A;
            set => Color.A = value;
        }
        public AssetSingle CharWidth { get; set; }
        public AssetSingle CharHeight { get; set; }
        public AssetSingle Unknown1 { get; set; }
        public AssetSingle Unknown2 { get; set; }
        public AssetSingle MaxScreenWidth { get; set; }
        public AssetSingle MaxScreenHeight { get; set; }

        public StyleStyleEntry() 
        { 
            Color = new AssetColor();
        }
        public StyleStyleEntry(EndianBinaryReader reader)
        {
            Unknown_00_1 = reader.ReadInt32();
            Color = reader.ReadColor();
            CharWidth = reader.ReadSingle();
            CharHeight = reader.ReadSingle();
            Unknown1 = reader.ReadSingle();
            Unknown2 = reader.ReadSingle();
            MaxScreenWidth = reader.ReadSingle();
            MaxScreenHeight = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {

                writer.Write(Unknown_00_1);
                writer.Write(Color);
                writer.Write(CharWidth);
                writer.Write(CharHeight);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(MaxScreenWidth);
                writer.Write(MaxScreenHeight);

                return writer.ToArray();
            }
        }
    }

    public class StyleEntry : GenericAssetDataContainer
    {
        public short Unknown_00 { get; set; }
        public short Unknown_02 { get; set; }
        public AssetSingle Unknown_04 { get; set; }
        public AssetSingle Unknown_08 { get; set; }
        public StyleStyleEntry TextStyle { get; set; }
        public StyleStyleEntry BackdropStyle { get; set; }

        public StyleEntry()
        {
            TextStyle = new StyleStyleEntry();
            BackdropStyle = new StyleStyleEntry();
        }
        public StyleEntry(EndianBinaryReader reader)
        {
            Unknown_00 = reader.ReadInt16();
            Unknown_02 = reader.ReadInt16();
            Unknown_04 = reader.ReadSingle();
            Unknown_08 = reader.ReadSingle();

            TextStyle = new StyleStyleEntry(reader);
            BackdropStyle = new StyleStyleEntry(reader);
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {

                writer.Write(Unknown_00);
                writer.Write(Unknown_02);
                writer.Write(Unknown_04);
                writer.Write(Unknown_08);
                writer.Write(TextStyle.Serialize(game, endianness));
                writer.Write(BackdropStyle.Serialize(game, endianness));

                return writer.ToArray();
            }
        }
    }

    public class TitleEntry : GenericAssetDataContainer
    {
        public int StyleIndex { get; set; }
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public int Unknown { get; set; }
        public string Text { get; set; }

        public TitleEntry()
        {
            Text = "";
        }
        public TitleEntry(EndianBinaryReader reader)
        {
            reader.ReadInt32(); // size
            StyleIndex = reader.ReadInt32();
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            reader.ReadInt32(); // text offset
            Unknown = reader.ReadInt32();
            Text = Functions.ReadString(reader);
            
            while (reader.BaseStream.Position % 4 != 0)
                reader.BaseStream.Position++;
        }

        public byte[] Serialize(int prevFileSize, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {

                writer.Write(0);
                writer.Write(StyleIndex);
                writer.Write(StartTime);
                writer.Write(EndTime);
                writer.Write((int)(prevFileSize + writer.BaseStream.Position + 8)); // text offset
                writer.Write(Unknown);

                foreach (byte c in System.Text.Encoding.GetEncoding(AssetTEXT.Codepage).GetBytes(Text))
                    writer.Write(c);
                writer.Write((byte)0);

                while (writer.BaseStream.Length % 4 != 0)
                    writer.Write((byte)0);

                writer.BaseStream.Position = 0;
                writer.Write((int)writer.BaseStream.Length);

                return writer.ToArray();
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class SectionEntry : GenericAssetDataContainer
    {
        public AssetSingle Duration { get; set; }
        public int Unknown_08 { get; set; }
        public AssetSingle Unknown_0C { get; set; }
        public AssetSingle BeginY { get; set; }
        public AssetSingle Unknown_14 { get; set; }
        public AssetSingle EndY { get; set; }
        public AssetSingle Unknown_1C { get; set; }
        public AssetSingle Unknown_20 { get; set; }
        public AssetSingle FadeInBegin { get; set; }
        public AssetSingle FadeInEnd { get; set; }
        public AssetSingle FadeOutBegin { get; set; }
        public AssetSingle FadeOutEnd { get; set; }
        public StyleEntry[] Styles { get; set; }
        public TitleEntry[] Titles { get; set; }
        
        public SectionEntry(EndianBinaryReader reader)
        {
            var start = reader.BaseStream.Position;
            int size = reader.ReadInt32();
            Duration = reader.ReadSingle();
            Unknown_08 = reader.ReadInt32();
            Unknown_0C = reader.ReadSingle();
            BeginY = reader.ReadSingle();
            Unknown_14 = reader.ReadSingle();
            EndY = reader.ReadSingle();
            Unknown_1C = reader.ReadSingle();
            Unknown_20 = reader.ReadSingle();
            FadeInBegin = reader.ReadSingle();
            FadeInEnd = reader.ReadSingle();
            FadeOutBegin = reader.ReadSingle();
            FadeOutEnd = reader.ReadSingle();
            int numberOfStyles = reader.ReadInt32();

            Styles = new StyleEntry[numberOfStyles];
            for (int i = 0; i < Styles.Length; i++)
                Styles[i] = new StyleEntry(reader);

            var titles = new List<TitleEntry>();
            while (reader.BaseStream.Position < start + size)
                titles.Add(new TitleEntry(reader));
            Titles = titles.ToArray();
        }

        public byte[] Serialize(int prevSize, Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(0); // size
                writer.Write(Duration);
                writer.Write(Unknown_08);
                writer.Write(Unknown_0C);
                writer.Write(BeginY);
                writer.Write(Unknown_14);
                writer.Write(EndY);
                writer.Write(Unknown_1C);
                writer.Write(Unknown_20);
                writer.Write(FadeInBegin);
                writer.Write(FadeInEnd);
                writer.Write(FadeOutBegin);
                writer.Write(FadeOutEnd);
                writer.Write(Styles.Length);

                foreach (var s in Styles)
                    writer.Write(s.Serialize(game, endianness));
                foreach (var t in Titles)
                    writer.Write(t.Serialize((int)writer.BaseStream.Length + prevSize, endianness));

                writer.BaseStream.Position = 0;
                writer.Write((int)writer.BaseStream.Length);

                return writer.ToArray();
            }
        }
    }

    public class AssetCRDT : Asset
    {
        private const string categoryName = "Credits";

        [Category(categoryName)]
        public AssetID Unknown00 { get; set; }
        [Category(categoryName)]
        public int Unknown04 { get; set; }
        [Category(categoryName)]
        public int Unknown08 { get; set; }
        [Category(categoryName)]
        public int Unknown0C { get; set; }
        [Category(categoryName)]
        public AssetSingle Duration { get; set; }
        [Category(categoryName)]
        public SectionEntry[] Sections { get; set; }

        public AssetCRDT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                Unknown00 = reader.ReadUInt32();
                Unknown04 = reader.ReadInt32();
                Unknown08 = reader.ReadInt32();
                Unknown0C = reader.ReadInt32();
                Duration = reader.ReadSingle();
                reader.ReadInt32(); // file size
                var sections = new List<SectionEntry>();
                while (!reader.EndOfStream)
                    sections.Add(new SectionEntry(reader));
                Sections = sections.ToArray();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Unknown00);
                writer.Write(Unknown04);
                writer.Write(Unknown08);
                writer.Write(Unknown0C);
                writer.Write(Duration);
                writer.Write(0); // size
                foreach (var s in Sections)
                    writer.Write(s.Serialize((int)writer.BaseStream.Length, game, endianness));

                writer.BaseStream.Position = 0x14;
                writer.Write((int)writer.BaseStream.Length);

                return writer.ToArray();
            }
        }
    }
}
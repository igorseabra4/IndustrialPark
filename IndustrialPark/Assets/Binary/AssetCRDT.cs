using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CreditsTextBox : GenericAssetDataContainer
    {
        public FontEnum Font { get; set; }
        public AssetColor Color { get; set; }
        public AssetSingle CharWidth { get; set; }
        public AssetSingle CharHeight { get; set; }
        public AssetSingle CharSpacingX { get; set; }
        public AssetSingle CharSpacingY { get; set; }
        public AssetSingle MaxScreenWidth { get; set; }
        public AssetSingle MaxScreenHeight { get; set; }

        public CreditsTextBox()
        {
            Color = new AssetColor();
        }
        public CreditsTextBox(EndianBinaryReader reader)
        {
            Font = (FontEnum)reader.ReadInt32();
            Color = reader.ReadColor();
            CharWidth = reader.ReadSingle();
            CharHeight = reader.ReadSingle();
            CharSpacingX = reader.ReadSingle();
            CharSpacingY = reader.ReadSingle();
            MaxScreenWidth = reader.ReadSingle();
            MaxScreenHeight = reader.ReadSingle();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write((int)Font);
            writer.Write(Color);
            writer.Write(CharWidth);
            writer.Write(CharHeight);
            writer.Write(CharSpacingX);
            writer.Write(CharSpacingY);
            writer.Write(MaxScreenWidth);
            writer.Write(MaxScreenHeight);
        }
    }

    public class CreditsPreset : GenericAssetDataContainer
    {
        public short Num { get; set; }
        public short Align { get; set; }
        public AssetSingle Delay { get; set; }
        public AssetSingle Innerspace { get; set; }
        public CreditsTextBox TextStyle { get; set; }
        public CreditsTextBox BackdropStyle { get; set; }

        public CreditsPreset()
        {
            TextStyle = new CreditsTextBox();
            BackdropStyle = new CreditsTextBox();
        }
        public CreditsPreset(EndianBinaryReader reader)
        {
            Num = reader.ReadInt16();
            Align = reader.ReadInt16();
            Delay = reader.ReadSingle();
            Innerspace = reader.ReadSingle();

            TextStyle = new CreditsTextBox(reader);
            BackdropStyle = new CreditsTextBox(reader);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Num);
            writer.Write(Align);
            writer.Write(Delay);
            writer.Write(Innerspace);
            TextStyle.Serialize(writer);
            BackdropStyle.Serialize(writer);
        }
    }

    public class CreditsHunk : GenericAssetDataContainer
    {
        public int PresetIndex { get; set; }
        public AssetSingle StartTime { get; set; }
        public AssetSingle EndTime { get; set; }
        public string Text { get; set; }

        public CreditsHunk()
        {
            Text = "";
        }
        public CreditsHunk(EndianBinaryReader reader)
        {
            reader.ReadInt32(); // size
            PresetIndex = reader.ReadInt32();
            StartTime = reader.ReadSingle();
            EndTime = reader.ReadSingle();
            reader.ReadInt64(); // text offset
            Text = Functions.ReadString(reader);

            while (reader.BaseStream.Position % 4 != 0)
                reader.BaseStream.Position++;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var startPos = writer.BaseStream.Position;
            writer.Write(0);
            writer.Write(PresetIndex);
            writer.Write(StartTime);
            writer.Write(EndTime);
            writer.Write((int)(writer.BaseStream.Position + 8)); // text offset
            writer.Write(0);
            writer.Write(Text);

            do writer.Write((byte)0);
            while (writer.BaseStream.Length % 4 != 0);

            var savePos = writer.BaseStream.Position;
            var size = savePos - startPos;
            writer.BaseStream.Position = startPos;
            writer.Write((int)size);
            writer.BaseStream.Position = savePos;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class CreditsEntry : GenericAssetDataContainer
    {
        public AssetSingle Duration { get; set; }
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        public AssetSingle BeginX { get; set; }
        public AssetSingle BeginY { get; set; }
        public AssetSingle EndX { get; set; }
        public AssetSingle EndY { get; set; }
        public AssetSingle ScrollRate { get; set; }
        public AssetSingle Lifetime { get; set; }
        public AssetSingle FadeInBegin { get; set; }
        public AssetSingle FadeInEnd { get; set; }
        public AssetSingle FadeOutBegin { get; set; }
        public AssetSingle FadeOutEnd { get; set; }
        public CreditsPreset[] Presets { get; set; }
        public CreditsHunk[] Titles { get; set; }

        public CreditsEntry(EndianBinaryReader reader)
        {
            var start = reader.BaseStream.Position;
            int size = reader.ReadInt32();
            Duration = reader.ReadSingle();
            Flags.FlagValueInt = reader.ReadUInt32();
            BeginX = reader.ReadSingle();
            BeginY = reader.ReadSingle();
            EndX = reader.ReadSingle();
            EndY = reader.ReadSingle();
            ScrollRate = reader.ReadSingle();
            Lifetime = reader.ReadSingle();
            FadeInBegin = reader.ReadSingle();
            FadeInEnd = reader.ReadSingle();
            FadeOutBegin = reader.ReadSingle();
            FadeOutEnd = reader.ReadSingle();
            int numberOfStyles = reader.ReadInt32();

            Presets = new CreditsPreset[numberOfStyles];
            for (int i = 0; i < Presets.Length; i++)
                Presets[i] = new CreditsPreset(reader);

            var titles = new List<CreditsHunk>();
            while (reader.BaseStream.Position < start + size)
                titles.Add(new CreditsHunk(reader));
            Titles = titles.ToArray();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var startPos = writer.BaseStream.Position;
            writer.Write(0); // size
            writer.Write(Duration);
            writer.Write(Flags.FlagValueInt);
            writer.Write(BeginX);
            writer.Write(BeginY);
            writer.Write(EndX);
            writer.Write(EndY);
            writer.Write(ScrollRate);
            writer.Write(Lifetime);
            writer.Write(FadeInBegin);
            writer.Write(FadeInEnd);
            writer.Write(FadeOutBegin);
            writer.Write(FadeOutEnd);
            writer.Write(Presets.Length);

            foreach (var s in Presets)
                s.Serialize(writer);
            foreach (var t in Titles)
                t.Serialize(writer);

            var savePos = writer.BaseStream.Position;
            var size = savePos - startPos;
            writer.BaseStream.Position = startPos;
            writer.Write((int)size);
            writer.BaseStream.Position = savePos;
        }
    }

    public class AssetCRDT : Asset
    {
        private const string categoryName = "Credits";

        [Category(categoryName)]
        public int Version { get; set; }
        [Category(categoryName)]
        public int CrdId { get; set; }
        [Category(categoryName)]
        public int State { get; set; }
        [Category(categoryName)]
        public AssetSingle total_time { get; set; }
        [Category(categoryName)]
        public CreditsEntry[] Sections { get; set; }

        public AssetCRDT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadUInt32(); // beef
                Version = reader.ReadInt32();
                CrdId = reader.ReadInt32();
                State = reader.ReadInt32();
                total_time = reader.ReadSingle();
                reader.ReadInt32(); // file size
                var sections = new List<CreditsEntry>();
                while (!reader.EndOfStream)
                    sections.Add(new CreditsEntry(reader));
                Sections = sections.ToArray();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(0xBEEEEEEF);
            writer.Write(Version);
            writer.Write(CrdId);
            writer.Write(State);
            writer.Write(total_time);
            writer.Write(0); // size
            foreach (var s in Sections)
                s.Serialize(writer);

            writer.BaseStream.Position = 0x14;
            writer.Write((int)writer.BaseStream.Length);
        }
    }
}
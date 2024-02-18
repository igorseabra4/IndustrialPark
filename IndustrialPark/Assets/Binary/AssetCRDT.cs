using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;

namespace IndustrialPark
{
    public enum CMAlignType : short
    {
        Center = 0,
        Left = 1,
        Right = 2,
        Inner = 3,
    }

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
            Color = new AssetColor(reader.ReadUInt32());
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
            writer.Write((uint)Color);
            writer.Write(CharWidth);
            writer.Write(CharHeight);
            writer.Write(CharSpacingX);
            writer.Write(CharSpacingY);
            writer.Write(MaxScreenWidth);
            writer.Write(MaxScreenHeight);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CreditsTexture : GenericAssetDataContainer
    {
        public AssetID TextureAssetID { get; set; }
        public AssetColor Color { get; set; }
        public AssetSingle PositionX { get; set; }
        public AssetSingle PositionY { get; set; }
        public AssetSingle Width {  get; set; }
        public AssetSingle Height { get; set; }
        public uint Texture { get; set; }
        public uint Pad { get; set; }

        public CreditsTexture()
        {
            Color = new AssetColor();
        }

        public CreditsTexture(EndianBinaryReader reader)
        {
            TextureAssetID = reader.ReadUInt32();
            Color = new AssetColor(reader.ReadUInt32());
            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Texture = reader.ReadUInt32();
            Pad = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(TextureAssetID);
            writer.Write((uint)Color);
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Texture);
            writer.Write(Pad);
        }

    }

    public class CreditsPreset : GenericAssetDataContainer
    {
        private const string categoryName = "Preset";

        [Category(categoryName)]
        public short Num { get; set; }
        private short align;
        [Category(categoryName)]
        public CMAlignType Align
        {
            get => (CMAlignType)align;
            set { align = (short)value; }
        }
        [Category(categoryName)]
        public AssetSingle Delay { get; set; }
        [Category(categoryName)]
        public AssetSingle Innerspace { get; set; }
        [Category("Textbox")]
        public CreditsTextBox TextStyle { get; set; }
        [Category("Textbox")]
        public CreditsTextBox BackdropStyle { get; set; }
        [Category("Texture")]
        public CreditsTexture TextureFront { get; set; }
        [Category("Texture")]
        public CreditsTexture TextureBack { get; set; }

        public CreditsPreset(int type)
        {
            if (type == 4)
            {
                align = 4;
            }
            TextureFront = new CreditsTexture();
            TextureBack = new CreditsTexture();
            TextStyle = new CreditsTextBox();
            BackdropStyle = new CreditsTextBox();
        }

        public CreditsPreset(EndianBinaryReader reader)
        {
            Num = reader.ReadInt16();
            align = reader.ReadInt16();
            Delay = reader.ReadSingle();
            Innerspace = reader.ReadSingle();

            if (align == 4)
            {
                TextureFront = new CreditsTexture(reader);
                TextureBack = new CreditsTexture(reader);
            }
            else
            {
                TextStyle = new CreditsTextBox(reader);
                BackdropStyle = new CreditsTextBox(reader);
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Num);
            writer.Write(align);
            writer.Write(Delay);
            writer.Write(Innerspace);
            if (align == 4)
            {
                TextureFront.Serialize(writer);
                TextureBack.Serialize(writer);
            }
            else
            {
                TextStyle.Serialize(writer);
                BackdropStyle.Serialize(writer);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (align != 4)
            {
                dt.RemoveProperty("TextureFront");
                dt.RemoveProperty("TextureBack");
            }
            else
            {
                dt.RemoveProperty("TextStyle");
                dt.RemoveProperty("BackdropStyle");
                dt.RemoveProperty("Align");
            }
        }

        public override bool HasReference(uint assetID)
        {
            if (align == 4)
                return TextureFront.TextureAssetID == assetID || TextureBack.TextureAssetID == assetID;
            return false;
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
            int text1 = reader.ReadInt32(); // text offset
            int text2 = reader.ReadInt32();

            if (text1 != 0)
            {
                Text = Functions.ReadString(reader);
                while (reader.BaseStream.Position % 4 != 0)
                    reader.BaseStream.Position++;
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            var startPos = writer.BaseStream.Position;
            writer.Write(0);
            writer.Write(PresetIndex);
            writer.Write(StartTime);
            writer.Write(EndTime);
            if (!string.IsNullOrEmpty(Text))
            {
                writer.Write((int)(writer.BaseStream.Position + 8)); // text offset
                writer.Write(0);
                writer.Write(Text);

                do
                    writer.Write((byte)0);
                while (writer.BaseStream.Length % 4 != 0);
            }
            else
                writer.Write((long)0);

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
        private const string categoryName = "CreditsEntry";

        [Category(categoryName)]
        public AssetSingle Duration { get; set; }
        [Category(categoryName)]
        public FlagBitmask Flags { get; set; } = IntFlagsDescriptor();
        [Category(categoryName)]
        public AssetSingle BeginX { get; set; }
        [Category(categoryName)]
        public AssetSingle BeginY { get; set; }
        [Category(categoryName)]
        public AssetSingle EndX { get; set; }
        [Category(categoryName)]
        public AssetSingle EndY { get; set; }
        [Category(categoryName)]
        public AssetSingle ScrollRate { get; set; }
        [Category(categoryName)]
        public AssetSingle Lifetime { get; set; }
        [Category(categoryName)]
        public AssetSingle FadeInBegin { get; set; }
        [Category(categoryName)]
        public AssetSingle FadeInEnd { get; set; }
        [Category(categoryName)]
        public AssetSingle FadeOutBegin { get; set; }
        [Category(categoryName)]
        public AssetSingle FadeOutEnd { get; set; }
        [Category("Preset"), Editor(typeof(DynamicTypeDescriptorCollectionEditor), typeof(UITypeEditor))]
        public CreditsPreset[] Presets { get; set; }
        [Category("Preset"), Editor(typeof(CrdtAddEditor), typeof(UITypeEditor))]
        public string AddTextbox
        {
            get => "Click here ->";
            set
            {
                if (value == "add_textbox_texture")
                {
                    var presets = Presets.ToList();
                    presets.Add(new CreditsPreset(0));
                    Presets = presets.ToArray();
                }

            }
        }
        [Category("Preset"), Editor(typeof(CrdtAddEditor), typeof(UITypeEditor))]
        public string AddTexture
        {
            get => "Click here ->";
            set
            {
                if (value == "add_textbox_texture")
                {
                    var presets = Presets.ToList();
                    presets.Add(new CreditsPreset(4));
                    Presets = presets.ToArray();
                }
            }
        }
        [Category("Titles")]
        public CreditsHunk[] Titles { get; set; }

        public CreditsEntry()
        {
            Presets = new CreditsPreset[0];
            Titles = new CreditsHunk[0];
        }

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

            foreach (var p in Presets)
                p.Serialize(writer);

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
        public override string AssetInfo => $"{TotalTime} seconds";

        [Category(categoryName), ReadOnly(true)]
        public uint Version { get; set; }
        [Category(categoryName)]
        public AssetID CrdId { get; set; }
        [Category(categoryName), ReadOnly(true)]
        public uint State { get; set; }
        [Category(categoryName)]
        public AssetSingle TotalTime { get; set; }
        [Category(categoryName)]
        public CreditsEntry[] Sections { get; set; }

        public AssetCRDT(string assetName, Game game) : base(assetName, AssetType.Credits)
        {
            Version = (uint)(game >= Game.Incredibles ? 512 : 256);
            State = 1;
            Sections = new CreditsEntry[0];
        }

        public AssetCRDT(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game)
        {
            byte[] _data = new byte[AHDR.data.Length];
            System.Array.Copy(AHDR.data, _data, AHDR.data.Length);

            using (var reader = new EndianBinaryReader(_data, endianness))
            {
                reader.ReadUInt32(); // beef
                Version = reader.ReadUInt32();
                CrdId = reader.ReadUInt32();
                State = reader.ReadUInt32();
                TotalTime = reader.ReadSingle();
                int totalsize = reader.ReadInt32();

                if (State == 3)
                    DecipherData(ref _data);

                var sections = new List<CreditsEntry>();
                while (!reader.EndOfStream)
                    sections.Add(new CreditsEntry(reader));
                Sections = sections.ToArray();
            }
        }

        private const string Key = "xCMChunkHand";

        private void DecipherData(ref byte[] data)
        {
            byte last = 0;
            for (int i = 0x18; i < data.Length; i++)
            {
                last = (byte)(data[i] ^ last ^ Key[i % Key.Length]);
                data[i] = last;
            }
        }

        private byte[] CipherData(byte[] data)
        {
            byte last = 0;
            for (int i = 0x18; i < data.Length; i++)
            {
                byte current = data[i];
                data[i] = (byte)(current ^ last ^ Key[i % Key.Length]);
                last = current;
            }
            return data;
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            using (var temp = new EndianBinaryWriter(writer.endianness))
            {
                temp.Write(0xBEEEEEEF);
                temp.Write(Version);
                temp.Write(CrdId);
                temp.Write(State);
                temp.Write(TotalTime);
                temp.Write(0); // size

                foreach (var s in Sections)
                    s.Serialize(temp);

                temp.BaseStream.Position = 0x14;
                temp.Write((int)temp.BaseStream.Length);

                writer.Write(State == 3 ? CipherData(temp.ToArray()) : temp.ToArray());
            }
        }
    }
}
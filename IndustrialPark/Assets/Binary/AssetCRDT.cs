using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System;
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

    public class CreditsTextBox_Texture : GenericAssetDataContainer
    {
        private uint _00h;
        private float _08h;
        private float _0Ch;
        private float _10h;
        private float _14h;
        private uint _18h;
        private uint _1Ch;

        [Category("TextBox")]
        public FontEnum Font
        {
            get => (FontEnum)_00h;
            set => _00h = (uint)value;
        }

        public AssetColor Color { get; set; }

        [Category("TextBox")]
        public AssetSingle CharWidth { get => _08h; set => _08h = value; }

        [Category("TextBox")]
        public AssetSingle CharHeight { get => _0Ch; set => _0Ch = value; }

        [Category("TextBox")]
        public AssetSingle CharSpacingX { get => _10h; set => _10h = value; }

        [Category("TextBox")]
        public AssetSingle CharSpacingY { get => _14h; set => _14h = value; }

        [Category("TextBox")]
        public AssetSingle MaxScreenWidth
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(_18h));
            set => _18h = BitConverter.ToUInt32(BitConverter.GetBytes(value));
        }

        [Category("TextBox")]
        public AssetSingle MaxScreenHeight
        {
            get => BitConverter.ToSingle(BitConverter.GetBytes(_1Ch));
            set => _1Ch = BitConverter.ToUInt32(BitConverter.GetBytes(value));
        }


        [Category("Texture")]
        public AssetID TextureAssetID { get => _00h; set => _00h = value; }

        [Category("Texture")]
        public AssetSingle PositionX { get => _08h; set => _08h = value; }

        [Category("Texture")]
        public AssetSingle PositionY { get => _0Ch; set => _0Ch = value; }

        [Category("Texture")]
        public AssetSingle Width { get => _10h; set => _10h = value; }

        [Category("Texture")]
        public AssetSingle Height { get => _14h; set => _14h = value; }

        [Category("Texture")]
        public uint Texture
        {
            get => BitConverter.ToUInt32(BitConverter.GetBytes(_18h));
            set => _18h = BitConverter.ToUInt32(BitConverter.GetBytes(value));
        }

        [Category("Texture")]
        public uint Pad
        {
            get => BitConverter.ToUInt32(BitConverter.GetBytes(_1Ch));
            set => _1Ch = BitConverter.ToUInt32(BitConverter.GetBytes(value));
        }

        public CreditsTextBox_Texture()
        {
            Color = new AssetColor();
        }

        public CreditsTextBox_Texture(EndianBinaryReader reader)
        {
            _00h = reader.ReadUInt32();
            Color = new AssetColor(reader.ReadUInt32());
            _08h = reader.ReadSingle();
            _0Ch = reader.ReadSingle();
            _10h = reader.ReadSingle();
            _14h = reader.ReadSingle();
            _18h = reader.ReadUInt32();
            _1Ch = reader.ReadUInt32();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(_00h);
            writer.Write((uint)Color);
            writer.Write(_08h);
            writer.Write(_0Ch);
            writer.Write(_10h);
            writer.Write(_14h);
            writer.Write(_18h);
            writer.Write(_1Ch);
        }

    }

    public enum PresetType
    {
        Textbox = 0,
        Texture = 4
    }

    public class CreditsPreset : GenericAssetDataContainer
    {
        private const string categoryName = "Preset";

        [Category(categoryName)]
        public short Num { get; set; }
        [Category(categoryName)]
        public AssetSingle Delay { get; set; }
        [Category(categoryName)]
        public AssetSingle Innerspace { get; set; }

        [DisplayName("TextStyle/TextureFront")]
        public CreditsTextBox_Texture TextStyle_TextureFront { get; set; }

        [DisplayName("BackdropStyle/TextureBack")]
        public CreditsTextBox_Texture BackdropStyle_TextureBack { get; set; }


        public PresetType PresetType { get; set; }

        public CreditsPreset()
        {
            TextStyle_TextureFront = new CreditsTextBox_Texture();
            BackdropStyle_TextureBack = new CreditsTextBox_Texture();
        }

        public CreditsPreset(EndianBinaryReader reader)
        {
            Num = reader.ReadInt16();
            PresetType = (PresetType)reader.ReadInt16();
            Delay = reader.ReadSingle();
            Innerspace = reader.ReadSingle();

            TextStyle_TextureFront = new CreditsTextBox_Texture(reader);
            BackdropStyle_TextureBack = new CreditsTextBox_Texture(reader);
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Num);
            writer.Write((short)PresetType);
            writer.Write(Delay);
            writer.Write(Innerspace);

            TextStyle_TextureFront.Serialize(writer);
            BackdropStyle_TextureBack.Serialize(writer);
        }

        public override bool HasReference(uint assetID)
        {
            if (PresetType == PresetType.Texture)
                return TextStyle_TextureFront.TextureAssetID == assetID || BackdropStyle_TextureBack.TextureAssetID == assetID;
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
        [Category("Preset"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
        public CreditsPreset[] Presets { get; set; }

        [Category("Titles"), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
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
        [Category(categoryName), Editor(typeof(AssetPropertyCollectionEditor), typeof(UITypeEditor))]
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
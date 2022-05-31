using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUIText : DynaUI
    {
        private const string dynaCategoryName = "ui:text";

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetID Text { get; set; }
        [Category(dynaCategoryName)]
        public byte font { get; set; }
        [Category(dynaCategoryName)]
        public byte fontSizeW { get; set; }
        [Category(dynaCategoryName)]
        public byte fontSizeH { get; set; }
        [Category(dynaCategoryName)]
        public byte fontSpacingX { get; set; }
        [Category(dynaCategoryName)]
        public byte fontSpacingY { get; set; }
        [Category(dynaCategoryName)]
        public byte textBoxInsetTop { get; set; }
        [Category(dynaCategoryName)]
        public byte textBoxInsetLeft { get; set; }
        [Category(dynaCategoryName)]
        public byte textBoxInsetRight { get; set; }
        [Category(dynaCategoryName)]
        public byte textBoxInsetBottom { get; set; }
        [Category(dynaCategoryName)]
        public byte justifyX { get; set; }
        [Category(dynaCategoryName)]
        public byte justifyY { get; set; }
        [Category(dynaCategoryName)]
        public byte textFlags { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor ShadowColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle shadowOffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle shadowOffsetY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle shadowScaleX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle shadowScaleY { get; set; }

        protected int dynaUITextEnd => dynaUIEnd + 36;

        public DynaUIText(Section_AHDR AHDR, Game game, Endianness endianness) : this(AHDR, DynaType.ui__text, game, endianness) { }

        protected DynaUIText(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaUIEnd;

                Text = reader.ReadUInt32();
                font = reader.ReadByte();
                fontSizeW = reader.ReadByte();
                fontSizeH = reader.ReadByte();
                fontSpacingX = reader.ReadByte();
                fontSpacingY = reader.ReadByte();
                textBoxInsetTop = reader.ReadByte();
                textBoxInsetLeft = reader.ReadByte();
                textBoxInsetRight = reader.ReadByte();
                textBoxInsetBottom = reader.ReadByte();
                justifyX = reader.ReadByte();
                justifyY = reader.ReadByte();
                textFlags = reader.ReadByte();
                ShadowColor = reader.ReadColor();
                shadowOffsetX = reader.ReadSingle();
                shadowOffsetY = reader.ReadSingle();
                shadowScaleX = reader.ReadSingle();
                shadowScaleY = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaUI(endianness));
                writer.Write(Text);
                writer.Write(font);
                writer.Write(fontSizeW);
                writer.Write(fontSizeH);
                writer.Write(fontSpacingX);
                writer.Write(fontSpacingY);
                writer.Write(textBoxInsetTop);
                writer.Write(textBoxInsetLeft);
                writer.Write(textBoxInsetRight);
                writer.Write(textBoxInsetBottom);
                writer.Write(justifyX);
                writer.Write(justifyY);
                writer.Write(textFlags);
                writer.Write(ShadowColor);
                writer.Write(shadowOffsetX);
                writer.Write(shadowOffsetY);
                writer.Write(shadowScaleX);
                writer.Write(shadowScaleY);

                return writer.ToArray();
            }
        }
    }
}
using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System.ComponentModel;

namespace IndustrialPark
{
    public enum XJustify : int
    {
        XJ_LEFT,
        XJ_CENTER,
        XJ_RIGHT,
    }

    public enum YJustify : int
    {
        YJ_TOP,
        YJ_CENTER,
        YJ_BOTTOM
    }

    public enum Expand : int
    {
        EX_UP,
        EX_CENTER,
        EX_DOWN,
        EX_MAX = 100
    }

    public class DynaGObjectTextBox : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:text_box";
        public override string TypeString => dynaCategoryName;
        public override string AssetInfo => HexUIntTypeConverter.StringFromAssetID(DefaultTextID);

        public override string Note => "Version is always 1 for BFBB, 3 for Movie/Incredibles or 4 for ROTU/RatProto.";

        [Category(dynaCategoryName)]
        public AssetID DefaultTextID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle XPosition { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle YPosition { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Width { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Height { get; set; }
        [Category(dynaCategoryName)]
        public FontEnum Font { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Y_As_Bottom { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte AutoCenterOnScreen { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TextWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TextHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CharSpacingX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CharSpacingY { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor Color { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle LeftMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle TopMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle RightMargin { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BottomMargin { get; set; }
        [Category(dynaCategoryName)]
        public XJustify XAlignment { get; set; }
        [Category(dynaCategoryName)]
        public YJustify YAlignment { get; set; }
        [Category(dynaCategoryName)]
        public Expand ExpandMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxHeight { get; set; }
        [Category(dynaCategoryName), Description("0 = SolidColor, 1 = Texture")]
        public int BackgroundMode { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor BackdropColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetID BackgroundTextureID { get; set; }

        [Category(dynaCategoryName)]
        public AssetSingle BackgroundBorderU { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BackgroundBorderV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BackgroundBorderWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BackgroundBorderHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetColor ShadowColor { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShadowOffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShadowOffsetY { get; set; }

        public DynaGObjectTextBox(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__text_box, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                DefaultTextID = reader.ReadUInt32();
                XPosition = reader.ReadSingle();
                YPosition = reader.ReadSingle();
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
                if (Version == 4)
                {
                    Y_As_Bottom = reader.ReadByte();
                    AutoCenterOnScreen = reader.ReadByte();
                    reader.ReadInt16();
                }
                Font = (FontEnum)reader.ReadInt32();
                TextWidth = reader.ReadSingle();
                TextHeight = reader.ReadSingle();
                CharSpacingX = reader.ReadSingle();
                CharSpacingY = reader.ReadSingle();
                Color = reader.ReadColor();
                LeftMargin = reader.ReadSingle();
                TopMargin = reader.ReadSingle();
                RightMargin = reader.ReadSingle();
                BottomMargin = reader.ReadSingle();
                XAlignment = (XJustify)reader.ReadInt32();
                if (Version >= 3)
                    YAlignment = (YJustify)reader.ReadInt32();
                ExpandMode = (Expand)reader.ReadInt32();
                MaxHeight = reader.ReadSingle();
                BackgroundMode = reader.ReadInt32();
                BackdropColor = reader.ReadColor();
                BackgroundTextureID = reader.ReadUInt32();
                if (Version >= 3)
                {
                    BackgroundBorderU = reader.ReadSingle();
                    BackgroundBorderV = reader.ReadSingle();
                    BackgroundBorderWidth = reader.ReadSingle();
                    BackgroundBorderHeight = reader.ReadSingle();
                    ShadowColor = reader.ReadColor();
                    ShadowOffsetX = reader.ReadSingle();
                    ShadowOffsetY = reader.ReadSingle();
                }
                else
                    ShadowColor = new AssetColor();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(DefaultTextID);
            writer.Write(XPosition);
            writer.Write(YPosition);
            writer.Write(Width);
            writer.Write(Height);
            if (Version == 4)
            {
                writer.Write(Y_As_Bottom);
                writer.Write(AutoCenterOnScreen);
                writer.Write((short)0);
            }
            writer.Write((int)Font);
            writer.Write(TextWidth);
            writer.Write(TextHeight);
            writer.Write(CharSpacingX);
            writer.Write(CharSpacingY);
            writer.Write(Color);
            writer.Write(LeftMargin);
            writer.Write(TopMargin);
            writer.Write(RightMargin);
            writer.Write(BottomMargin);
            writer.Write((int)XAlignment);
            if (Version >= 3)
                writer.Write((int)YAlignment);
            writer.Write((int)ExpandMode);
            writer.Write(MaxHeight);
            writer.Write(BackgroundMode);
            writer.Write(BackdropColor);
            writer.Write(BackgroundTextureID);
            if (Version >= 3)
            {
                writer.Write(BackgroundBorderU);
                writer.Write(BackgroundBorderV);
                writer.Write(BackgroundBorderWidth);
                writer.Write(BackgroundBorderHeight);
                writer.Write(ShadowColor);
                writer.Write(ShadowOffsetX);
                writer.Write(ShadowOffsetY);
            }
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (Version == 1)
            {
                dt.RemoveProperty("YAlignment");
                dt.RemoveProperty("BackgroundBorderU");
                dt.RemoveProperty("BackgroundBorderV");
                dt.RemoveProperty("BackgroundBorderWidth");
                dt.RemoveProperty("BackgroundBorderHeight");
                dt.RemoveProperty("ShadowColor");
                dt.RemoveProperty("ShadowOffsetX");
                dt.RemoveProperty("ShadowOffsetY");
            }
            if (Version < 4)
            {
                dt.RemoveProperty("Y_As_Bottom");
                dt.RemoveProperty("AutoCenterOnScreen");
            }
        }
    }
}
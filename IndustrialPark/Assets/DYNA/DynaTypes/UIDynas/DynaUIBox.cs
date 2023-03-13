using HipHopFile;
using IndustrialPark.AssetEditorColors;
using System;
using System.ComponentModel;

namespace IndustrialPark
{
    public class UIBoxPart : GenericAssetDataContainer
    {
        public AssetID Texture { get; set; }
        public AssetColor Color { get; set; }
        public AssetSingle uv1u { get; set; }
        public AssetSingle uv1v { get; set; }
        public AssetSingle uv2u { get; set; }
        public AssetSingle uv2v { get; set; }
        public AssetSingle uv3u { get; set; }
        public AssetSingle uv3v { get; set; }
        public AssetSingle uv4u { get; set; }
        public AssetSingle uv4v { get; set; }
        public AssetSingle rotation { get; set; }
        public byte Enabled { get; set; }

        public UIBoxPart() { }
        public UIBoxPart(EndianBinaryReader reader)
        {
            Texture = reader.ReadUInt32();
            Color = reader.ReadColor();
            uv1u = reader.ReadSingle();
            uv1v = reader.ReadSingle();
            uv2u = reader.ReadSingle();
            uv2v = reader.ReadSingle();
            uv3u = reader.ReadSingle();
            uv3v = reader.ReadSingle();
            uv4u = reader.ReadSingle();
            uv4v = reader.ReadSingle();
            rotation = reader.ReadSingle();
            Enabled = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            writer.Write(Texture);
            writer.Write(Color);
            writer.Write(uv1u);
            writer.Write(uv1v);
            writer.Write(uv2u);
            writer.Write(uv2v);
            writer.Write(uv3u);
            writer.Write(uv3v);
            writer.Write(uv4u);
            writer.Write(uv4v);
            writer.Write(rotation);
            writer.Write(Enabled);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }

    public class DynaUIBox : DynaUI
    {
        private const string dynaCategoryName = "ui:box";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        private UIBoxPart[] _parts { get; set; }
        [Category(dynaCategoryName), Description("UI Box must have exactly 9 parts")]
        public UIBoxPart[] parts
        {
            get => _parts;
            set
            {
                if (value.Length != 9)
                    throw new ArgumentException("UI Box must have exactly 9 parts");
                _parts = value;
            }
        }
        [Category(dynaCategoryName)]
        public AssetSingle BorderWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle BorderHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WidthPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle HeightPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CenterWidthPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle CenterHeightPerUV { get; set; }
        [Category(dynaCategoryName)]
        public byte ScaleHSide { get; set; }
        [Category(dynaCategoryName)]
        public byte ScaleVSide { get; set; }
        [Category(dynaCategoryName)]
        public byte ScaleCenter { get; set; }
        [Category(dynaCategoryName)]
        public byte StretchUVsOnMotionScale { get; set; }
        [Category(dynaCategoryName)]
        public byte ForceAlphaWrite { get; set; }

        public DynaUIBox(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__box, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaUIEnd;

                _parts = new UIBoxPart[9];
                for (int i = 0; i < _parts.Length; i++)
                    _parts[i] = new UIBoxPart(reader);

                BorderWidth = reader.ReadSingle();
                BorderHeight = reader.ReadSingle();
                WidthPerUV = reader.ReadSingle();
                HeightPerUV = reader.ReadSingle();
                CenterWidthPerUV = reader.ReadSingle();
                CenterHeightPerUV = reader.ReadSingle();
                ScaleHSide = reader.ReadByte();
                ScaleVSide = reader.ReadByte();
                ScaleCenter = reader.ReadByte();
                StretchUVsOnMotionScale = reader.ReadByte();
                ForceAlphaWrite = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            SerializeDynaUI(writer);

            foreach (var p in _parts)
                p.Serialize(writer);
            writer.Write(BorderWidth);
            writer.Write(BorderHeight);
            writer.Write(WidthPerUV);
            writer.Write(HeightPerUV);
            writer.Write(CenterWidthPerUV);
            writer.Write(CenterHeightPerUV);
            writer.Write(ScaleHSide);
            writer.Write(ScaleVSide);
            writer.Write(ScaleCenter);
            writer.Write(StretchUVsOnMotionScale);
            writer.Write(ForceAlphaWrite);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
        }
    }
}
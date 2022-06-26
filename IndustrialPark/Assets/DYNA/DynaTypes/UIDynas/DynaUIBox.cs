using AssetEditorColors;
using HipHopFile;
using System;
using System.ComponentModel;
using System.Linq;

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
        public byte enabled { get; set; }

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
            enabled = reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
            reader.ReadByte();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
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
                writer.Write(enabled);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                return writer.ToArray();
            }
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
        public AssetSingle borderWidth { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle borderHeight { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle widthPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle heightPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle centerWidthPerUV { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle centerHeightPerUV { get; set; }
        [Category(dynaCategoryName)]
        public byte scaleHSide { get; set; }
        [Category(dynaCategoryName)]
        public byte scaleVSide { get; set; }
        [Category(dynaCategoryName)]
        public byte scaleCenter { get; set; }
        [Category(dynaCategoryName)]
        public byte stretchUVsOnMotionScale { get; set; }
        [Category(dynaCategoryName)]
        public byte forceAlphaWrite { get; set; }

        public DynaUIBox(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__box, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaUIEnd;

                _parts = new UIBoxPart[9];
                for (int i = 0; i < _parts.Length; i++)
                    _parts[i] = new UIBoxPart(reader);

                borderWidth = reader.ReadSingle();
                borderHeight = reader.ReadSingle();
                widthPerUV = reader.ReadSingle();
                heightPerUV = reader.ReadSingle();
                centerWidthPerUV = reader.ReadSingle();
                centerHeightPerUV = reader.ReadSingle();
                scaleHSide = reader.ReadByte();
                scaleVSide = reader.ReadByte();
                scaleCenter = reader.ReadByte();
                stretchUVsOnMotionScale = reader.ReadByte();
                forceAlphaWrite = reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaUI(endianness));

                foreach (var p in _parts)
                    writer.Write(p.Serialize(game, endianness));
                writer.Write(borderWidth);
                writer.Write(borderHeight);
                writer.Write(widthPerUV);
                writer.Write(heightPerUV);
                writer.Write(centerWidthPerUV);
                writer.Write(centerHeightPerUV);
                writer.Write(scaleHSide);
                writer.Write(scaleVSide);
                writer.Write(scaleCenter);
                writer.Write(stretchUVsOnMotionScale);
                writer.Write(forceAlphaWrite);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                return writer.ToArray();
            }
        }
    }
}
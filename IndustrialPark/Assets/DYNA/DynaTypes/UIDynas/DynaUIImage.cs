using AssetEditorColors;
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaUIImage : DynaUI
    {
        private const string dynaCategoryName = "ui:image";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Texture_AssetID { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv1u { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv1v { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv2u { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv2v { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv3u { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv3v { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv4u { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle uv4v { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle rotation { get; set; }
        [Category(dynaCategoryName)]
        public FlagBitmask UIImageFlags { get; set; } = ShortFlagsDescriptor();
        [Category(dynaCategoryName)]
        public byte addreasMoveU { get; set; }
        [Category(dynaCategoryName)]
        public byte addreasMoveV { get; set; }
        [Category(dynaCategoryName), DisplayName("Color 1 (R, G, B)")]
        public AssetColor Color1 { get; set; }
        [Category(dynaCategoryName), DisplayName("Color 1 Alpha (0 - 255)")]
        public byte Color1Alpha
        {
            get => Color1.A;
            set => Color1.A = value;
        }
        [Category(dynaCategoryName), DisplayName("Color 2 (R, G, B)")]
        public AssetColor Color2 { get; set; }
        [Category(dynaCategoryName), DisplayName("Color 2 Alpha (0 - 255)")]
        public byte Color2Alpha
        {
            get => Color2.A;
            set => Color2.A = value;
        }
        [Category(dynaCategoryName), DisplayName("Color 3 (R, G, B)")]
        public AssetColor Color3 { get; set; }
        [Category(dynaCategoryName), DisplayName("Color 3 Alpha (0 - 255)")]
        public byte Color3Alpha
        {
            get => Color3.A;
            set => Color3.A = value;
        }
        [Category(dynaCategoryName), DisplayName("Color 4 (R, G, B)")]
        public AssetColor Color4 { get; set; }
        [Category(dynaCategoryName), DisplayName("Color 4 Alpha (0 - 255)")]
        public byte Color4Alpha
        {
            get => Color4.A;
            set => Color4.A = value;
        }

        [Category(dynaCategoryName), Description("This unusual field is present in Incredibles, but not Movie.")]
        public int Unknown { get; set; }

        private bool extraFieldPresent = false;

        public DynaUIImage(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.ui__image, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaUIEnd;

            Texture_AssetID = reader.ReadUInt32();
            uv1u = reader.ReadSingle();
            uv1v = reader.ReadSingle();
            uv2u = reader.ReadSingle();
            uv2v = reader.ReadSingle();
            uv3u = reader.ReadSingle();
            uv3v = reader.ReadSingle();
            uv4u = reader.ReadSingle();
            uv4v = reader.ReadSingle();
            rotation = reader.ReadSingle();
            UIImageFlags.FlagValueShort = reader.ReadUInt16();
            addreasMoveU = reader.ReadByte();
            addreasMoveV = reader.ReadByte();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Color3 = reader.ReadColor();
            Color4 = reader.ReadColor();

            if (reader.BaseStream.Position != linkStartPosition(reader.BaseStream.Length, _links.Length))
            {
                extraFieldPresent = true;
                Unknown = reader.ReadInt32();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeDynaUI(endianness));

            writer.Write(Texture_AssetID);
            writer.Write(uv1u);
            writer.Write(uv1v);
            writer.Write(uv2u);
            writer.Write(uv2v);
            writer.Write(uv3u);
            writer.Write(uv3v);
            writer.Write(uv4u);
            writer.Write(uv4v);
            writer.Write(rotation);
            writer.Write(UIImageFlags.FlagValueShort);
            writer.Write(addreasMoveU);
            writer.Write(addreasMoveV);
            writer.Write(Color1);
            writer.Write(Color2);
            writer.Write(Color3);
            writer.Write(Color4);

            if (extraFieldPresent)
                writer.Write(Unknown);

            return writer.ToArray();
        }

        public override bool HasReference(uint assetID) => Texture_AssetID == assetID || base.HasReference(assetID);
    }
}
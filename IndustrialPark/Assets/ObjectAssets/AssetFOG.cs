using HipHopFile;
using System.ComponentModel;
using System.Drawing.Design;
using AssetEditorColors;

namespace IndustrialPark
{
    public class AssetFOG : BaseAsset
    {
        private const string categoryName = "Fog";

        [Category(categoryName), DisplayName("End Color (R, G, B)")]
        public AssetColor BackgroundColor { get; set; }
        [Category(categoryName), DisplayName("End Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get => BackgroundColor.A;
            set => BackgroundColor.A = value;
        }
        [Category(categoryName), DisplayName("Start Color (R, G, B)")]
        public AssetColor FogColor { get; set; }
        [Category(categoryName), DisplayName("Start Color Alpha (0 - 255)")]
        public byte FogColorAlpha
        {
            get => FogColor.A;
            set => FogColor.A = value;
        }
        [Category(categoryName)]
        public AssetSingle FogDensity { get; set; }
        [Category(categoryName)]
        public AssetSingle StartDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle EndDistance { get; set; }
        [Category(categoryName)]
        public AssetSingle TransitionTime { get; set; }
        [Category(categoryName)]
        public byte FogType { get; set; }

        public AssetFOG(string assetName) : base(assetName, AssetType.FOG, BaseAssetType.Fog)
        {
            BackgroundColor = new AssetColor();
            FogColor = new AssetColor();
            FogColorAlpha = 255;
            FogDensity = 1;
            StartDistance = 100;
            EndDistance = 400;
        }

        public AssetFOG(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                BackgroundColor = reader.ReadColor();
                FogColor = reader.ReadColor();
                FogDensity = reader.ReadSingle();
                StartDistance = reader.ReadSingle();
                EndDistance = reader.ReadSingle();
                TransitionTime = reader.ReadSingle();
                FogType = reader.ReadByte();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeBase(endianness));

                writer.Write(BackgroundColor);
                writer.Write(FogColor);
                writer.Write(FogDensity);
                writer.Write(StartDistance);
                writer.Write(EndDistance);
                writer.Write(TransitionTime);
                writer.Write(FogType);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }
    }
}
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
        public MyColor BackgroundColor { get; set; }
        [Category(categoryName), DisplayName("End Color Alpha (0 - 255)")]
        public byte BackgroundColorAlpha
        {
            get => BackgroundColor.A;
            set => BackgroundColor.A = value;
        }
        [Category(categoryName), DisplayName("Start Color (R, G, B)")]
        public MyColor FogColor { get; set; }
        [Category(categoryName), DisplayName("Start Color Alpha (0 - 255)")]
        public byte FogColorAlpha
        {
            get => FogColor.A;
            set => FogColor.A = value;
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float FogDensity { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float StartDistance { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float EndDistance { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float TransitionTime { get; set; }
        [Category(categoryName)]
        public byte FogType { get; set; }

        public AssetFOG(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            BackgroundColor = new MyColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            FogColor = new MyColor(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            FogDensity = reader.ReadSingle();
            StartDistance = reader.ReadSingle();
            EndDistance = reader.ReadSingle();
            TransitionTime = reader.ReadSingle();
            FogType = reader.ReadByte();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

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

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }
    }
}
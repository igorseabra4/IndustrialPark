using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPEND : EntityAsset
    {
        private const string categoryName = "Pendulum";

        [Category(categoryName)]
        public byte UnknownByte54 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte55 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte56 { get; set; }
        [Category(categoryName)]
        public byte UnknownByte57 { get; set; }
        [Category(categoryName)]
        public int UnknownInt58 { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementDistance { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float SteepnessRad { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float SteepnessDeg
        {
            get => MathUtil.RadiansToDegrees(SteepnessRad);
            set => SteepnessRad = MathUtil.DegreesToRadians(value);
        }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float MovementTime { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68Rad { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float UnknownFloat68Deg
        {
            get => MathUtil.RadiansToDegrees(UnknownFloat68Rad);
            set => UnknownFloat68Rad = MathUtil.DegreesToRadians(value);
        }
        [Category(categoryName)]
        public int UnknownInt6C { get; set; }
        [Category(categoryName)]
        public int UnknownInt70 { get; set; }
        [Category(categoryName)]
        public int UnknownInt74 { get; set; }
        [Category(categoryName)]
        public int UnknownInt78 { get; set; }
        [Category(categoryName)]
        public int UnknownInt7C { get; set; }
        [Category(categoryName)]
        public int UnknownInt80 { get; set; }

        public AssetPEND(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = entityEndPosition;

            UnknownByte54 = reader.ReadByte();
            UnknownByte55 = reader.ReadByte();
            UnknownByte56 = reader.ReadByte();
            UnknownByte57 = reader.ReadByte();
            UnknownInt58 = reader.ReadInt32();
            MovementDistance = reader.ReadSingle();
            SteepnessRad = reader.ReadSingle();
            MovementTime = reader.ReadSingle();
            UnknownFloat68Rad = reader.ReadSingle();
            UnknownInt6C = reader.ReadInt32();
            UnknownInt70 = reader.ReadInt32();
            UnknownInt74 = reader.ReadInt32();
            UnknownInt78 = reader.ReadInt32();
            UnknownInt7C = reader.ReadInt32();
            UnknownInt80 = reader.ReadInt32();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(UnknownByte54);
            writer.Write(UnknownByte55);
            writer.Write(UnknownByte56);
            writer.Write(UnknownByte57);
            writer.Write(UnknownInt58);
            writer.Write(MovementDistance);
            writer.Write(SteepnessRad);
            writer.Write(MovementTime);
            writer.Write(UnknownFloat68Rad);
            writer.Write(UnknownInt6C);
            writer.Write(UnknownInt70);
            writer.Write(UnknownInt74);
            writer.Write(UnknownInt78);
            writer.Write(UnknownInt7C);
            writer.Write(UnknownInt80);

            writer.Write(SerializeLinks(platform));
            return writer.ToArray();
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;        
    }
}
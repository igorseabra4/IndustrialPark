using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : BaseAsset
    {
        private const string categoryName = "Timer";

        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float Time { get; set; }
        [Category(categoryName), TypeConverter(typeof(FloatTypeConverter))]
        public float RandomRange { get; set; }

        public AssetTIMR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            Time = reader.ReadSingle();

            if (game != Game.Scooby)
                RandomRange = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);
            writer.Write(SerializeBase(platform));

            writer.Write(Time);

            if (game != Game.Scooby)
                writer.Write(RandomRange);

            writer.Write(SerializeLinks(platform));

            return writer.ToArray();
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("RandomRange");
            base.SetDynamicProperties(dt);
        }
    }
}
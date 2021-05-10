using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : BaseAsset
    {
        private const string categoryName = "Timer";

        [Category(categoryName)]
        public AssetSingle Time { get; set; }
        [Category(categoryName)]
        public AssetSingle RandomRange { get; set; }

        public AssetTIMR(string assetName) : base(assetName, AssetType.TIMR, BaseAssetType.Timer)
        {
            Time = 1;
        }

        public AssetTIMR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = baseHeaderEndPosition;

            Time = reader.ReadSingle();

            if (game != Game.Scooby)
                RandomRange = reader.ReadSingle();
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);
            writer.Write(SerializeBase(endianness));

            writer.Write(Time);

            if (game != Game.Scooby)
                writer.Write(RandomRange);

            writer.Write(SerializeLinks(endianness));

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
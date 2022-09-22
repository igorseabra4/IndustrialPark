using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetTIMR : BaseAsset
    {
        private const string categoryName = "Timer";
        public override string AssetInfo => $"Time: {Time}";

        [Category(categoryName)]
        public AssetSingle Time { get; set; }
        [Category(categoryName)]
        public AssetSingle RandomRange { get; set; }

        public AssetTIMR(string assetName) : base(assetName, AssetType.Timer, BaseAssetType.Timer)
        {
            Time = 1;
        }

        public AssetTIMR(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = baseHeaderEndPosition;

                Time = reader.ReadSingle();

                if (game != Game.Scooby)
                    RandomRange = reader.ReadSingle();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {

                base.Serialize(writer);
                writer.Write(Time);

                if (game != Game.Scooby)
                    writer.Write(RandomRange);
                SerializeLinks(writer);

                
        }

        public override void SetDynamicProperties(DynamicTypeDescriptor dt)
        {
            if (game == Game.Scooby)
                dt.RemoveProperty("RandomRange");
            base.SetDynamicProperties(dt);
        }
    }
}
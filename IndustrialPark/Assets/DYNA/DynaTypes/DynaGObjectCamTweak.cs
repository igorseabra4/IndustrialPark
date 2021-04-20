using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectCamTweak : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Camera_Tweak";

        protected override int constVersion => 1;

        [Category(dynaCategoryName)]
        public int Priority { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Time { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PitchAdjust { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DistAdjust { get; set; }

        public DynaGObjectCamTweak(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__Camera_Tweak, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            Priority = reader.ReadInt32();
            Time = reader.ReadSingle();
            PitchAdjust = reader.ReadSingle();
            DistAdjust = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(Priority);
            writer.Write(Time);
            writer.Write(PitchAdjust);
            writer.Write(DistAdjust);

            return writer.ToArray();
        }
    }
}
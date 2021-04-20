using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectRaceTimer : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:RaceTimer";

        protected override int constVersion => 2;

        [Category(dynaCategoryName)]
        public int UnknownInt_00 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_04 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_08 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_0C { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_10 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_14 { get; set; }

        public DynaGObjectRaceTimer(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, DynaType.game_object__RaceTimer, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = dynaDataStartPosition;

            UnknownInt_00 = reader.ReadInt32();
            UnknownInt_04 = reader.ReadInt32();
            UnknownInt_08 = reader.ReadInt32();
            UnknownFloat_0C = reader.ReadSingle();
            UnknownFloat_10 = reader.ReadSingle();
            UnknownFloat_14 = reader.ReadSingle();
        }

        protected override byte[] SerializeDyna(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(UnknownInt_00);
            writer.Write(UnknownInt_04);
            writer.Write(UnknownInt_08);
            writer.Write(UnknownFloat_0C);
            writer.Write(UnknownFloat_10);
            writer.Write(UnknownFloat_14);

            return writer.ToArray();
        }
    }
}
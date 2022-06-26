using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectCamTweak : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:Camera_Tweak";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public int Priority { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Time { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PitchAdjust { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DistAdjust { get; set; }

        public DynaGObjectCamTweak(string assetName) : base(assetName, DynaType.game_object__Camera_Tweak, 1)
        {
        }
        public DynaGObjectCamTweak(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__Camera_Tweak, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Priority = reader.ReadInt32();
                Time = reader.ReadSingle();
                PitchAdjust = reader.ReadSingle();
                DistAdjust = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Priority);
                writer.Write(Time);
                writer.Write(PitchAdjust);
                writer.Write(DistAdjust);

                return writer.ToArray();
            }
        }
    }
}
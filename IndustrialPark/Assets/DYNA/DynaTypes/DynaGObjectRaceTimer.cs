using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectRaceTimer : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:RaceTimer";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public bool CountDown { get; set; }
        [Category(dynaCategoryName)]
        public int StartTime { get; set; }
        [Category(dynaCategoryName)]
        public int VictoryTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WarnTime1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WarnTime2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle WarnTime3 { get; set; }

        public DynaGObjectRaceTimer(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__RaceTimer, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                CountDown = reader.ReadByteBool();
                reader.BaseStream.Position += 3;
                StartTime = reader.ReadInt32();
                VictoryTime = reader.ReadInt32();
                WarnTime1 = reader.ReadSingle();
                WarnTime2 = reader.ReadSingle();
                WarnTime3 = reader.ReadSingle();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write((byte)(CountDown ? 1 : 0));
                writer.Write(new byte[3]);
                writer.Write(StartTime);
                writer.Write(VictoryTime);
                writer.Write(WarnTime1);
                writer.Write(WarnTime2);
                writer.Write(WarnTime3);

                
        }
    }
}
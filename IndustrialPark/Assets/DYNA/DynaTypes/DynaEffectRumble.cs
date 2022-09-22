using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectRumble : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Rumble";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 3;

        [Category(dynaCategoryName)]
        public AssetSingle Time { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Intensity { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ID { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte Priority { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RumbleType { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte RumbleInPause { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Param1 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle Param2 { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShakeMagnitude { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShakeCycleMax { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ShakeRotationalMagnitude { get; set; }
        [Category(dynaCategoryName)]
        public AssetByte ShakeY { get; set; }

        public DynaEffectRumble(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Rumble, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Time = reader.ReadSingle();
                Intensity = reader.ReadSingle();
                ID = reader.ReadSingle();
                Priority = reader.ReadByte();
                RumbleType = reader.ReadByte();
                RumbleInPause = reader.ReadByte();
                reader.ReadByte();
                Param1 = reader.ReadSingle();
                Param2 = reader.ReadSingle();
                ShakeMagnitude = reader.ReadSingle();
                ShakeCycleMax = reader.ReadSingle();
                ShakeRotationalMagnitude = reader.ReadSingle();
                ShakeY = reader.ReadByte();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

                writer.Write(Time);
                writer.Write(Intensity);
                writer.Write(ID);
                writer.Write(Priority);
                writer.Write(RumbleType);
                writer.Write(RumbleInPause);
                writer.Write((byte)0);
                writer.Write(Param1);
                writer.Write(Param2);
                writer.Write(ShakeMagnitude);
                writer.Write(ShakeCycleMax);
                writer.Write(ShakeRotationalMagnitude);
                writer.Write(ShakeY);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);

                
        }
    }
}
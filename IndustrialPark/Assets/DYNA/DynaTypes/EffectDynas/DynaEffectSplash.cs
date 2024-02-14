using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaEffectSplash : AssetDYNA
    {
        private const string dynaCategoryName = "effect:Splash";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 2;

        [Category(dynaCategoryName)]
        public AssetSingle PositionX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PositionY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PositionZ { get; set; }
        [Category(dynaCategoryName)]
        public uint MotionType { get; set; }
        [Category(dynaCategoryName)]
        public AssetID SoundGroup { get; set; }

        public DynaEffectSplash(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.effect__Splash, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                PositionX = reader.ReadSingle();
                PositionY = reader.ReadSingle();
                PositionZ = reader.ReadSingle();
                MotionType = reader.ReadUInt32();
                SoundGroup = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {
            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(MotionType);
            writer.Write(SoundGroup);
        }
    }
}
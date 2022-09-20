using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class DynaHudMeter : DynaHud
    {
        private const string dynaCategoryName = "hud:meter";

        [Category(dynaCategoryName)]
        public AssetSingle StartValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MinValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle MaxValue { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle IncrementTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle DecrementTime { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartIncrementSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID IncrementSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID StartDecrementSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DecrementSound { get; set; }

        protected int dynaHudMeterEnd => dynaHudEnd + 36;

        public DynaHudMeter(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaHudEnd;

                StartValue = reader.ReadSingle();
                MinValue = reader.ReadSingle();
                MaxValue = reader.ReadSingle();
                IncrementTime = reader.ReadSingle();
                DecrementTime = reader.ReadSingle();
                StartIncrementSound = reader.ReadUInt32();
                IncrementSound = reader.ReadUInt32();
                StartDecrementSound = reader.ReadUInt32();
                DecrementSound = reader.ReadUInt32();
            }
        }

        protected byte[] SerializeDynaHudMeter(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeDynaHud(endianness));
                writer.Write(StartValue);
                writer.Write(MinValue);
                writer.Write(MaxValue);
                writer.Write(IncrementTime);
                writer.Write(DecrementTime);
                writer.Write(StartIncrementSound);
                writer.Write(IncrementSound);
                writer.Write(StartDecrementSound);
                writer.Write(DecrementSound);

                return writer.ToArray();
            }
        }
    }
}
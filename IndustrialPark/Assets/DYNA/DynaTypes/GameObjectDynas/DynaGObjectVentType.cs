using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectVentType : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:VentType";
        public override string TypeString => dynaCategoryName;

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID IdleEmitter { get; set; }
        [Category(dynaCategoryName)]
        public AssetID IdleSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID WarnEmitter { get; set; }
        [Category(dynaCategoryName)]
        public AssetID WarnSound { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DamageEmitter { get; set; }
        [Category(dynaCategoryName)]
        public AssetID DamageSound { get; set; }

        public DynaGObjectVentType(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__VentType, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                IdleEmitter = reader.ReadUInt32();
                IdleSound = reader.ReadUInt32();
                WarnEmitter = reader.ReadUInt32();
                WarnSound = reader.ReadUInt32();
                DamageEmitter = reader.ReadUInt32();
                DamageSound = reader.ReadUInt32();
            }
        }

        protected override void SerializeDyna(EndianBinaryWriter writer)
        {

            writer.Write(IdleEmitter);
            writer.Write(IdleSound);
            writer.Write(WarnEmitter);
            writer.Write(WarnSound);
            writer.Write(DamageEmitter);
            writer.Write(DamageSound);


        }
    }
}
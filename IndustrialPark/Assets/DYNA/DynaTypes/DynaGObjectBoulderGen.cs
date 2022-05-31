using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class DynaGObjectBoulderGen : AssetDYNA
    {
        private const string dynaCategoryName = "game_object:BoulderGenerator";

        protected override short constVersion => 1;

        [Category(dynaCategoryName)]
        public AssetID Object { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle OffsetRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitVelZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelAngleRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle VelMagRand { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle InitAxisZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle AngVel { get; set; }

        public DynaGObjectBoulderGen(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.game_object__BoulderGenerator, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = dynaDataStartPosition;

                Object = reader.ReadUInt32();
                OffsetX = reader.ReadSingle();
                OffsetY = reader.ReadSingle();
                OffsetZ = reader.ReadSingle();
                OffsetRand = reader.ReadSingle();
                InitVelX = reader.ReadSingle();
                InitVelY = reader.ReadSingle();
                InitVelZ = reader.ReadSingle();
                VelAngleRand = reader.ReadSingle();
                VelMagRand = reader.ReadSingle();
                InitAxisX = reader.ReadSingle();
                InitAxisY = reader.ReadSingle();
                InitAxisZ = reader.ReadSingle();
                AngVel = reader.ReadSingle();
            }
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(Object);
                writer.Write(OffsetX);
                writer.Write(OffsetY);
                writer.Write(OffsetZ);
                writer.Write(OffsetRand);
                writer.Write(InitVelX);
                writer.Write(InitVelY);
                writer.Write(InitVelZ);
                writer.Write(VelAngleRand);
                writer.Write(VelMagRand);
                writer.Write(InitAxisX);
                writer.Write(InitAxisY);
                writer.Write(InitAxisZ);
                writer.Write(AngVel);

                return writer.ToArray();
            }
        }

        public override void Verify(ref List<string> result)
        {
            Verify(Object, ref result);
            base.Verify(ref result);
        }
    }
}
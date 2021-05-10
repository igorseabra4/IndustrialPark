using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public abstract class DynaHud : AssetDYNA
    {
        private const string dynaCategoryName = "hud";

        [Category(dynaCategoryName)]
        public AssetSingle PositionX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PositionY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle PositionZ { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleX { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleY { get; set; }
        [Category(dynaCategoryName)]
        public AssetSingle ScaleZ { get; set; }

        protected int dynaHudEnd => dynaDataStartPosition + 24;

        public DynaHud(Section_AHDR AHDR, DynaType type, Game game, Endianness endianness) : base(AHDR, type, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            PositionX = reader.ReadSingle();
            PositionY = reader.ReadSingle();
            PositionZ = reader.ReadSingle();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            ScaleZ = reader.ReadSingle();
        }

        protected byte[] SerializeDynaHud(Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(PositionX);
            writer.Write(PositionY);
            writer.Write(PositionZ);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(ScaleZ);

            return writer.ToArray();
        }
    }
}
using HipHopFile;
using SharpDX;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetPEND : AssetWithMotion
    {
        private const string categoryName = "Pendulum";

        [Category(categoryName)]
        public AssetSingle Lt { get; set; }
        [Category(categoryName)]
        public AssetSingle Q1t { get; set; }
        [Category(categoryName)]
        public AssetSingle Q3t { get; set; }

        public AssetPEND(string assetName, Vector3 position) : base(assetName, AssetType.Pendulum, BaseAssetType.Pendulum, position)
        {
            Motion = new Motion_Pendulum();
        }

        public AssetPEND(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                Motion = new Motion_Pendulum(reader);

                reader.BaseStream.Position = 0x74 + (game == Game.BFBB ? 4 : 0);

                Lt = reader.ReadInt32();
                Q1t = reader.ReadInt32();
                Q3t = reader.ReadInt32();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(SerializeEntity(game, endianness));
                writer.Write(Motion.Serialize(game, endianness));
                writer.BaseStream.Position = 0x74 + (game == Game.BFBB ? 4 : 0);
                writer.Write(Lt);
                writer.Write(Q1t);
                writer.Write(Q3t);

                writer.Write(SerializeLinks(endianness));
                return writer.ToArray();
            }
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}
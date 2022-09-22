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
            Motion = new Motion_Pendulum(game);
        }

        public AssetPEND(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.BaseStream.Position = entityHeaderEndPosition;

                Motion = new Motion_Pendulum(reader, game);

                reader.BaseStream.Position = 0x74 + (game == Game.BFBB ? 4 : 0);

                Lt = reader.ReadInt32();
                Q1t = reader.ReadInt32();
                Q3t = reader.ReadInt32();
            }
        }

        public override void Serialize(EndianBinaryWriter writer)
        {
            base.Serialize(writer);
            Motion.Serialize(writer);
            writer.BaseStream.Position = 0x74 + (game == Game.BFBB ? 4 : 0);
            writer.Write(Lt);
            writer.Write(Q1t);
            writer.Write(Q3t);
            SerializeLinks(writer);
        }

        public static bool dontRender = false;

        public override bool DontRender => dontRender;
    }
}
using HipHopFile;
using System.ComponentModel;

namespace IndustrialPark
{
    public class AssetCNTR : BaseAsset
    {
        [Category("Counter")]
        public short Count { get; set; }

        public AssetCNTR(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform)
        {
            var reader = new EndianBinaryReader(AHDR.data, platform);
            reader.BaseStream.Position = baseEndPosition;

            Count = reader.ReadInt16();
        }

        public override byte[] Serialize(Game game, Platform platform)
        {
            var writer = new EndianBinaryWriter(platform);

            writer.Write(SerializeBase(platform));

            writer.Write(Count);
            writer.Write((short)0);

            writer.Write(SerializeLinks(platform));

            return writer.ToArray();
        }
    }
}
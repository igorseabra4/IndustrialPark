using HipHopFile;

namespace IndustrialPark
{
    public class AssetWithData : Asset
    {
        public byte[] Data { get; set; }

        public override string AssetInfo => Data.Length.ToString() + " bytes";

        public AssetWithData(string assetName, AssetType assetType, byte[] data) : base(assetName, assetType)
        {
            Data = data;
        }

        public AssetWithData(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            Data = AHDR.data;
        }

        public override byte[] Serialize(Game game, Endianness endianness) => Data;

        public string RwVersion(int renderWareVersion)
        {
            var str = RenderWareFile.Shared.UnpackLibraryVersion(renderWareVersion).ToString("X5");
            return $"RW {str[0]}.{str[1]}.{str[2]}.{str[3]}{str[4]}";
        }
    }
}
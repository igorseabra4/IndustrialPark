using HipHopFile;

namespace IndustrialPark
{
    public class AssetSound : AssetWithData
    {
        public override string AssetInfo => fileType + base.AssetInfo;

        private string fileType;

        private void SetFileType(Game game, Platform platform)
        {
            switch (platform)
            {
                case Platform.GameCube:
                    if (game == Game.Incredibles)
                        fileType = "GameCube FSB3 ";
                    else
                        fileType = "GameCube DSP ";
                    break;
                case Platform.Xbox:
                    fileType = "Xbox WAV ";
                    break;
                case Platform.PS2:
                    fileType = "PS2 VAG ";
                    break;
                default:
                    fileType = "Unknown ";
                    break;
            }
        }

        public AssetSound(string assetName, AssetType assetType, Game game, Platform platform, byte[] data) : base(assetName, assetType, data)
        {
            SetFileType(game, platform);
        }

        public AssetSound(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform.Endianness())
        {
            SetFileType(game, platform);
        }
    }
}
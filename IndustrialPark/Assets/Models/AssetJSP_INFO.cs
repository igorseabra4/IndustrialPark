using HipHopFile;
using RenderWareFile;
using SharpDX;

namespace IndustrialPark
{
    public class AssetJSP_INFO : Asset
    {
        public AssetJSP_INFO(Section_AHDR AHDR, Game game, Platform platform) : base(AHDR, game, platform) { }

        private int renderWareVersion;

        public RWSection[] File
        {
            get
            {
                RWSection[] data = ReadFileMethods.ReadRenderWareFile(Data);
                renderWareVersion = data[0].renderWareVersion;
                return data;
            }
            set => Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
        }
    }
}
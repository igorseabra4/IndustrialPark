using HipHopFile;
using RenderWareFile;

namespace IndustrialPark
{
    public class AssetJSP_INFO : AssetWithData
    {
        public AssetJSP_INFO(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness) { }

        public override byte[] Serialize(Game game, Endianness endianness) => Data;

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
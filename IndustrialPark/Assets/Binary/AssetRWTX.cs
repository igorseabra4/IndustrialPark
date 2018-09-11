using HipHopFile;
using System.IO;

namespace IndustrialPark
{
    public class AssetRWTX : Asset
    {
        public AssetRWTX(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public string Name { get => Path.GetFileNameWithoutExtension(AHDR.ADBG.assetName); }

        public void Setup()
        {
            //TextureManager.RemoveTexture(Name);
            //TextureManager.LoadTexturesFromTXD(Data, Name);
        }
    }
}
using HipHopFile;
using RenderWareFile;
using RenderWareFile.Sections;
using System.IO;

namespace IndustrialPark
{
    public class AssetRWTX : Asset
    {
        public AssetRWTX(Section_AHDR AHDR) : base(AHDR)
        {
        }

        public string Name { get => Path.GetFileNameWithoutExtension(AHDR.ADBG.assetName); }

        public void Setup() { }

        private int renderWareVersion;

        private RWSection[] TextureAsRWSections
        {
            get
            {
                try
                {
                    ReadFileMethods.treatStuffAsByteArray = true;
                    RWSection[] sections = ReadFileMethods.ReadRenderWareFile(Data);
                    renderWareVersion = sections[0].renderWareVersion;
                    ReadFileMethods.treatStuffAsByteArray = false;
                    return sections;
                }
                catch
                {
                    return new RWSection[0];
                }
            }
            set
            {
                ReadFileMethods.treatStuffAsByteArray = true;
                Data = ReadFileMethods.ExportRenderWareFile(value, renderWareVersion);
                ReadFileMethods.treatStuffAsByteArray = false;
            }
        }

        public TextureFilterMode FilterMode
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.filterMode;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.filterMode = value;

                TextureAsRWSections = file;
            }
        }

        public TextureAddressMode AddressModeU
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.addressModeU;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.addressModeU = value;

                TextureAsRWSections = file;
            }
        }

        public TextureAddressMode AddressModeV
        {
            get
            {
                foreach (RWSection rws in TextureAsRWSections)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            return native.textureNativeStruct.addressModeV;
                return 0;
            }
            set
            {
                RWSection[] file = TextureAsRWSections;

                foreach (RWSection rws in file)
                    if (rws is TextureDictionary_0016 textD)
                        foreach (TextureNative_0015 native in textD.textureNativeList)
                            native.textureNativeStruct.addressModeV = value;

                TextureAsRWSections = file;
            }
        }
    }
}
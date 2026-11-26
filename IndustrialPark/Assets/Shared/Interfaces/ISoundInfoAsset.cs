using HipHopFile;

namespace IndustrialPark.Assets
{
    public interface ISoundInfoAsset<T> where T : ISoundInfoAsset<T>
    {
        uint assetID { get; }
        Section_AHDR BuildAHDR(Platform platform);
        void Merge(T asset);
    }
}

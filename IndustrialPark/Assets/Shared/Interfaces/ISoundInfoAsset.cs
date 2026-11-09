using HipHopFile;

namespace IndustrialPark.Assets
{
    public interface ISoundInfoAsset<T> where T : ISoundInfoAsset<T>
    {
        uint assetID { get; }
        Section_AHDR BuildAHDR(Endianness endianness);
        void Merge(T asset);
    }
}

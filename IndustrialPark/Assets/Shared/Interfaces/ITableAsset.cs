namespace IndustrialPark
{
    public interface ITableAsset
    {
        void RemoveEntry(uint assetID);
    }

    public interface ITableAsset<T, U> where T : ITableAsset<T, U>
    {
        void Merge(T asset);
        U[] Entries { get; set; }
    }
}

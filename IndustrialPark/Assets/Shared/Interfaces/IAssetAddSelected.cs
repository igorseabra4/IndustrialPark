using System.Collections.Generic;

namespace IndustrialPark
{
    public interface IAssetAddSelected
    {
        string GetItemsText { get; }
        void AddItems(List<uint> items);
    }
}

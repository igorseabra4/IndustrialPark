using System.Collections.Generic;

namespace IndustrialPark.Assets
{
    public interface IAssetAddSelected
    {
        string GetItemsText { get; }
        void AddItems(List<uint> items);
        List<uint> GetItems();
    }
}

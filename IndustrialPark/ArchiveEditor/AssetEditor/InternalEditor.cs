namespace IndustrialPark
{
    public interface IInternalEditor
    {
        uint GetAssetID();
        void Close();
        void Show();
    }
}
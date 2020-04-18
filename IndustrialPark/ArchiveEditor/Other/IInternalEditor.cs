namespace IndustrialPark
{
    public interface IInternalEditor
    {
        bool TopMost { get; set; }
        uint GetAssetID();
        void Close();
        void Show();

        void SetHideHelp(bool hideHelp);

        void RefreshPropertyGrid();
    }
}
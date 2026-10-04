using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public interface IInternalEditor
    {
        bool TopMost { get; set; }
        uint GetAssetID();
        void Close();
        void Show();

        void RefreshPropertyGrid();

        double Opacity { get; set; }
        Form[] OwnedForms { get; }
        void BringToFront();

        event EventHandler Activated;
        event EventHandler Deactivate;
    }
}
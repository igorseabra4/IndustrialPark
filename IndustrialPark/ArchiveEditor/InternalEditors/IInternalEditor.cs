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
        FormStartPosition StartPosition { get; set; }
        System.Drawing.Point Location { get; set; }

        void RefreshPropertyGrid();

        double Opacity { get; set; }
        Form[] OwnedForms { get; }
        System.Drawing.Size Size { get; set; }

        void BringToFront();

        event EventHandler Activated;
        event EventHandler Deactivate;
    }
}
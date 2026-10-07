using System;
using System.Collections.Generic;
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

        public static string[] GetPropertyPath(GridItem item)
        {
            var path = new List<string>();
            while (item != null)
            {
                if (item.GridItemType == GridItemType.Property)
                    path.Add(item.Label);
                item = item.Parent;
            }
            path.Reverse();
            return path.ToArray();
        }
    }
}
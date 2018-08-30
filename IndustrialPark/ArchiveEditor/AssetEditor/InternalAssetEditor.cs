using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;

namespace IndustrialPark
{
    public partial class InternalAssetEditor : Form
    {
        public InternalAssetEditor()
        {
            InitializeComponent();
            TopMost = true;
        }
        
        private void InternalAssetEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        public void SelectAsset(Asset asset)
        {
            propertyGridAsset.SelectedObject = asset;
            labelAssetName.Text = asset.ToString();
        }
    }
}

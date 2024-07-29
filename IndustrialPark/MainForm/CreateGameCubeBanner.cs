using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class CreateGameCubeBanner : Form
    {
        public CreateGameCubeBanner()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
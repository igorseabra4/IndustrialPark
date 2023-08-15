using IndustrialPark.ImageHelpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ChangeImageScale : Form
    {
        public ChangeImageScale(Form callingForm)
        {
            mainForm = callingForm as ImageEditor;
            InitializeComponent();
        }

        private ImageEditor mainForm = null; // A null reference to the ImageEditor main window for later.


        private void btnApplyScale_Click(object sender, EventArgs e)
        {
            Bitmap bmp = ImageScalingAlgorithm.RescaleImage(this.mainForm.ImageToScale, Int16.Parse(txtWidth.Text), Int16.Parse(txtHeight.Text));
            this.mainForm.ImageToScale = bmp;
        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            Bitmap bmp = ImageScalingAlgorithm.RescaleImage(this.mainForm.ImageToScale, Int16.Parse("128"), Int16.Parse("128"));
            this.mainForm.ImageToScale = bmp;

            // Automatically use the String "128" for width and height without needing to type it in.
        }
    }
}

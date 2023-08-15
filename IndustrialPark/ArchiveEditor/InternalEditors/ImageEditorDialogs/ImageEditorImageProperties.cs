using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ImageEditorImageProperties : Form
    {
        public ImageEditorImageProperties()
        {
            InitializeComponent();
        }

        private void ImageEditorImageProperties_Load(object sender, EventArgs e)
        {
            txtWidth.Text = ImageEditor.imageWidth.ToString();
            txtHeight.Text = ImageEditor.imageHeight.ToString();

            txtFormat.Text = ImageEditor.imageFormatType;
        }
    }
}

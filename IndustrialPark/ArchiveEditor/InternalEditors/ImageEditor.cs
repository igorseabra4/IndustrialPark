using IndustrialPark.ImageHelpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;


namespace IndustrialPark
{
    public partial class ImageEditor : Form
    {
        public ImageEditor()
        {
            InitializeComponent();


        }

        public static Bitmap originalLoadedImage;

        public static int imageWidth;
        public static int imageHeight;
        public static string imageFormatType = "";

        /*
        The below Getter and Setter Property allows accessing of the "imageEditorViewerBox.Image" to allow getting it's data on 
        the "ChangeImageScale" form for scaling from another window.
        */
        public Image ImageToScale
        {
            get { return imageEditorViewerBox.Image; }
            set { imageEditorViewerBox.Image = value; }
        }
        private void importPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openPNGItem = new OpenFileDialog();
            openPNGItem.Filter = "Portable Network Graphics (*.png)|*.png|All files (*.*)|*.*";
            openPNGItem.RestoreDirectory = true;

            if (openPNGItem.ShowDialog() == DialogResult.OK)
            {
                imageEditorViewerBox.Image = Image.FromFile(openPNGItem.FileName);
                Bitmap ogImage = new Bitmap(imageEditorViewerBox.Image);
                originalLoadedImage = ogImage;
            }
        }

        private void importJPEGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openJPEGItem = new OpenFileDialog();
            openJPEGItem.Filter = "Joint Picture Experts Group (*.jpg)|*.jpg|All files (*.*)|*.*";
            openJPEGItem.RestoreDirectory = true;

            if (openJPEGItem.ShowDialog() == DialogResult.OK)
            {
                imageEditorViewerBox.Image = Image.FromFile(openJPEGItem.FileName);
                Bitmap ogImage = new Bitmap(imageEditorViewerBox.Image);
                originalLoadedImage = ogImage;
            }
        }

        private void closeImageEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void saveImageToHOPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveTexture = new SaveFileDialog();
            saveTexture.Filter = "Joint Picture Experts Group (*.jpg)|*.jpg|PNG Files (*.png)|*.png|All Files (*.*)|*.*";
            if (saveTexture.ShowDialog() == DialogResult.OK)
                if (imageEditorViewerBox.Image.Width > 128 && imageEditorViewerBox.Image.Height > 128)
                {
                    string message = "This image's pixel resolution is not optimized for a Hi game! Are you sure you still wan to save it?";
                    string caption = "Warning! Not Optimized";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    result = MessageBox.Show(message, caption, buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        if (saveTexture.FilterIndex.Equals(1))
                        {
                            imageEditorViewerBox.Image.Save(saveTexture.FileName, ImageFormat.Jpeg);
                        }
                        if (saveTexture.FilterIndex.Equals(2))
                        {
                            imageEditorViewerBox.Image.Save(saveTexture.FileName, ImageFormat.Png);
                        }
                    }
                    else if (imageEditorViewerBox.Image.Width < 128 && imageEditorViewerBox.Image.Height < 128)
                    {
                        if (saveTexture.FilterIndex.Equals(1))
                        {
                            imageEditorViewerBox.Image.Save(saveTexture.FileName, ImageFormat.Jpeg);
                        }
                        if (saveTexture.FilterIndex.Equals(2))
                        {
                            imageEditorViewerBox.Image.Save(saveTexture.FileName, ImageFormat.Png);
                        }
                    }
                }
        }


        private void blueOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(imageEditorViewerBox.Image);
            imageEditorViewerBox.Image = ImageEditorFilters.FilterAppliedBlue(bmp);
        }

        private void redOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(imageEditorViewerBox.Image);
            imageEditorViewerBox.Image = ImageEditorFilters.FilterAppliedRed(bmp);
        }

        private void greenOverlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(imageEditorViewerBox.Image);
            imageEditorViewerBox.Image = ImageEditorFilters.FilterAppliedGreen(bmp);
        }

        private void clearAllEditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageEditorViewerBox.Image.Dispose();
            imageEditorViewerBox.Image = originalLoadedImage;
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(imageEditorViewerBox.Image);
            imageEditorViewerBox.Image = ImageEditorFilters.InvertImage(bmp);
        }

        private void clearImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageEditorViewerBox.Image = null;
        }

        /*
        Below isn't perfect and is very hacky for detecting if the PixtureBox has no image, but it
        sorta works good enough to release for now at least.
        */

        private void currentImagePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imageEditorViewerBox.Image == null)
            {
                string message = "No image imported, so viewing properties makes no sense! Please import an image first.";
                string caption = "Warning! No Image File Imported!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
            else if (imageEditorViewerBox.Image.RawFormat.Equals(ImageFormat.Png))
            {
                imageWidth = imageEditorViewerBox.Image.Width;
                imageHeight = imageEditorViewerBox.Image.Height;
                imageFormatType = "Portable Network Graphics Image";

                ImageEditorImageProperties propertiesForm = new ImageEditorImageProperties();
                propertiesForm.TopMost = true;
                propertiesForm.ShowDialog();
            }
            else if (imageEditorViewerBox.Image.RawFormat.Equals(ImageFormat.Jpeg))
            {
                imageWidth = imageEditorViewerBox.Image.Width;
                imageHeight = imageEditorViewerBox.Image.Height;
                imageFormatType = "Joint Picture Experts Group Image";

                ImageEditorImageProperties propertiesForm = new ImageEditorImageProperties();
                propertiesForm.TopMost = true;
                propertiesForm.ShowDialog();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void changeScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeImageScale imageScaleTool = new ChangeImageScale(this);
            imageScaleTool.TopMost = true;
            imageScaleTool.ShowDialog();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpToRotate = new Bitmap(imageEditorViewerBox.Image);
            bmpToRotate.RotateFlip(RotateFlipType.Rotate90FlipNone);
            imageEditorViewerBox.Image = bmpToRotate;
        }

        private void rotate90LeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpToRotate = new Bitmap(imageEditorViewerBox.Image);
            bmpToRotate.RotateFlip(RotateFlipType.Rotate270FlipNone);
            imageEditorViewerBox.Image = bmpToRotate;
        }

        private void mirrorXAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpToRotate = new Bitmap(imageEditorViewerBox.Image);
            bmpToRotate.RotateFlip(RotateFlipType.RotateNoneFlipX);
            imageEditorViewerBox.Image = bmpToRotate;
        }

        private void mirrorYAxisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpToRotate = new Bitmap(imageEditorViewerBox.Image);
            bmpToRotate.RotateFlip(RotateFlipType.RotateNoneFlipY);
            imageEditorViewerBox.Image = bmpToRotate;
        }
        /*
        Tried adding a solid color maker for converting a solid color from a ColorDialog to a Bitmap for saving, but it wouldn't save the
        color as an image, and couldn't figure it out. But I left this here just in case I can someday.
        */
        private void solidColorGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                imageEditorViewerBox.Image = new Bitmap(128, 128);
                Graphics graphics = Graphics.FromImage(imageEditorViewerBox.Image);

                Brush brush = new SolidBrush(colorDialog.Color);

                graphics.FillRectangle(brush, new System.Drawing.Rectangle(0, 0, 128, 128));
                Bitmap solidColor = new Bitmap(imageEditorViewerBox.Image);
                imageEditorViewerBox.Image = solidColor;
            }

        }
    }
}

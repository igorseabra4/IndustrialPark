using System.Drawing;
/* 
This class stores algorithms for the filters used in the Image Editor. It takes an Image from the "Image" class and uses the
image from the "imageEditorViewerBox" PictureBox control and makes it into a Bitmap, fills a rectangle the same size of the image 
to a hard-coded color to match the specified filter. The "InvertImage" method reads an image Pixel by Pixel and inverts the color of
each one to return an inverted image result.
*/
namespace IndustrialPark.ImageHelpers
{
    internal class ImageEditorFilters
    {
        public static Image FilterAppliedBlue(Image inputImage)
        {
            Bitmap filteredImage = new Bitmap(inputImage.Width, inputImage.Height);
            Graphics graphicsData = Graphics.FromImage(filteredImage);
            graphicsData.DrawImage(inputImage, 0, 0);

            graphicsData.FillRectangle(new SolidBrush(Color.FromArgb(80, Color.Blue)), 0, 0, filteredImage.Width, filteredImage.Height);


            return filteredImage;
        }

        public static Image FilterAppliedRed(Image inputImage)
        {
            Bitmap filteredImage = new Bitmap(inputImage.Width, inputImage.Height);
            Graphics graphicsData = Graphics.FromImage(filteredImage);
            graphicsData.DrawImage(inputImage, 0, 0);

            graphicsData.FillRectangle(new SolidBrush(Color.FromArgb(80, Color.Red)), 0, 0, filteredImage.Width, filteredImage.Height);


            return filteredImage;
        }

        public static Image FilterAppliedGreen(Image inputImage)
        {
            Bitmap filteredImage = new Bitmap(inputImage.Width, inputImage.Height);
            Graphics graphicsData = Graphics.FromImage(filteredImage);
            graphicsData.DrawImage(inputImage, 0, 0);

            graphicsData.FillRectangle(new SolidBrush(Color.FromArgb(80, Color.Green)), 0, 0, filteredImage.Width, filteredImage.Height);


            return filteredImage;
        }
        public static Image InvertImage(Image inputImage)
        {
            Bitmap bmpDest = null;

            using (Bitmap bmpSource = new Bitmap(inputImage))
            {
                bmpDest = new Bitmap(bmpSource.Width, bmpSource.Height);

                for (int x = 0; x < bmpSource.Width; x++)
                {
                    for (int y = 0; y < bmpSource.Height; y++)
                    {

                        Color clrPixel = bmpSource.GetPixel(x, y);

                        clrPixel = Color.FromArgb(255 - clrPixel.R, 255 -
                           clrPixel.G, 255 - clrPixel.B);

                        bmpDest.SetPixel(x, y, clrPixel);
                    }
                }
            }

            return (Image)bmpDest;

        }
    }
}

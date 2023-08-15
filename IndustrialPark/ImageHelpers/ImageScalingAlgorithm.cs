using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

/* 
This class uses an algorithm I came up witht to read an image Pixel by Pixel, remove not needed pixles to get a smaller image as if
it were originally said size, and redraw the result and return it. This way rather than simply squishing the image down, you
get a high quality resize to look good in-game.
*/

namespace IndustrialPark.ImageHelpers
{
    internal class ImageScalingAlgorithm
    {
        public static Bitmap RescaleImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}

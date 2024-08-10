using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using SharpShell.Attributes;
using SharpShell.SharpThumbnailHandler;

namespace ShellExtensions
{
    /// <summary>
    /// Shell extension for GameCube banners (.bnr file extension).
    /// This allows the image from a .bnr to display as a thumbnail
    /// in Windows file explorer.
    /// </summary>
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.FileExtension, ".bnr")]
    public class GameCubeBannerThumbnailHandler : SharpThumbnailHandler
    {        
        /// <summary>
        /// Provides the Bitmap for the banner's thumbnail image.
        /// </summary>
        /// <param name="width">The expected thumbnail width</param>
        /// <returns>The banner thumbnail image bitmap</returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected override Bitmap GetThumbnailImage(uint width)
        {
            uint height = width / 3;
            
            Bitmap originalImage = new Bitmap(96, 32);
            Bitmap scaledImage = new Bitmap((int)width, (int)height);

            // Read the banner image data into originalImage
            // The banner data starts at 0x20 and ends at 0x181f
            // It is encoded in RGB5A1 in 4x4 blocks


            // 1. Read image data
            using (Stream stream = SelectedItemStream)
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("SelectedItemStream is null.");
                }
                
                // Banner data starts at 0x20
                stream.Seek(0x20, SeekOrigin.Begin);
                var imageData = new byte[96 * 32 * 2];
                stream.Read(imageData, 0, imageData.Length);
                
                int gridSize = 4;
                int gridWidth = 96 / gridSize;
                int gridHeight = 32 / gridSize;
            
                int index = 0;

                for (int gridY = 0; gridY < gridHeight; gridY++)
                {
                    for (int gridX = 0; gridX < gridWidth; gridX++)
                    {
                        for (int y = 0; y < gridSize; y++)
                        {
                            for (int x = 0; x < gridSize; x++)
                            {
                                // Read two bytes
                                byte upperByte = imageData[index++];
                                byte lowerByte = imageData[index++];

                                // Extract color components
                                // format: ARRR RRGG GGGB BBBB
                                int a = (upperByte & 0x80) >> 7;
                                int r = ((upperByte & 0x7C) >> 2);
                                int g = (((upperByte & 0x03) << 3) | ((lowerByte & 0xE0) >> 5));
                                int b = (lowerByte & 0x1F);

                                // Combine color components
                                Color pixel = Color.FromArgb(a * 255, r * 8, g * 8, b * 8);

                                // Set pixel in bitmap
                                originalImage.SetPixel(gridX * gridSize + x, gridY * gridSize + y, pixel);
                            }
                        }
                    }
                }
            }
            
            // 2. Scale the image
            using (var graphics = Graphics.FromImage(scaledImage))
            {
                // Set the interpolation mode to ensure high-quality scaling
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // Draw the original image onto the new bitmap with the desired size
                graphics.DrawImage(originalImage, 0, 0, (int)width, (int)height);
            }
            
            return scaledImage;
        }
    }
}
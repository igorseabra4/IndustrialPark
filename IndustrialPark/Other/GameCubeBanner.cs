using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace IndustrialPark
{
    /// <summary>
    /// Represents a (single-language) GameCube banner (BNR1).
    /// Supports ASCII text only (no Shift JIS).
    /// <para>
    /// Contains a 96x32 image, a title, a creator, a full title, a full creator, and a description.
    /// It appears in the GameCube system menu when a game disc is inserted.
    /// Read more about the BNR format here: https://hitmen.c02.at/files/yagcd/yagcd/chap14.html#sec14.1
    /// </para>
    /// </summary>
    public class GameCubeBanner
    {
        /// <summary>
        /// The width of the banner image.
        /// </summary>
        public const int IMAGE_WIDTH = 96;
        /// <summary>
        /// The height of the banner image.
        /// </summary>
        public const int IMAGE_HEIGHT = 32;
        /// <summary>
        /// The magic bytes at the start of the file.
        /// </summary>
        public const string MAGIC = "BNR1";
        /// <summary>
        /// The typical filename for a GameCube banner.
        /// </summary>
        public const string DEFAULT_FILENAME = "opening.bnr";
        /// <summary>
        /// The typical file extension for a GameCube banner.
        /// </summary>
        public const string DEFAULT_FILE_EXTENSION = ".bnr";

        /// <summary>
        /// The 96x32 bitmap image to use on the banner.
        /// </summary>
        public Bitmap Image { get; set; }
        /// <summary>
        /// The game's short title. Limited to 20 bytes.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The game's short creator. Limited to 20 bytes.
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// The game's full title. Limited to 40 bytes.
        /// </summary>
        public string TitleFull { get; set; }
        /// <summary>
        /// The game's full creator. Limited to 40 bytes.
        /// </summary>
        public string CreatorFull { get; set; } 
        /// <summary>
        /// The game's description. Limited to 80 bytes.
        /// </summary>
        public string Description { get; set; }
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCubeBanner"/> class.
        /// </summary>
        public GameCubeBanner()
        {
            Image = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
            Title = "";
            Creator = "";
            TitleFull = "";
            CreatorFull = "";
            Description = "";
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCubeBanner"/> class.
        /// </summary>
        /// <param name="image">The image to use on the banner</param>
        /// <param name="title">The game's title</param>
        /// <param name="creator">The game's creator</param>
        /// <param name="titleFull">The game's full title</param>
        /// <param name="creatorFull">The game's full creator</param>
        /// <param name="description">The game's description</param>
        public GameCubeBanner(Bitmap image, string title, string creator, string titleFull, string creatorFull, string description)
        {
            Image = image;
            Title = title;
            Creator = creator;
            TitleFull = titleFull;
            CreatorFull = creatorFull;
            Description = description;
        }

        /// <summary>
        /// Imports a GameCube banner from a file.
        /// </summary>
        /// <param name="filePath">The filepath to a .bnr file</param>
        /// <returns>The GameCube banner</returns>
        public static GameCubeBanner ImportFromFile(string filePath)
        {
            // Import a GameCube banner by reading from a file
            GameCubeBanner banner = new GameCubeBanner();
            
            // Open filestream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            // Read magic "BNR1"
            var magic = new byte[4];
            fileStream.Read(magic, 0, 4);
            
            // If magic is not "BNR1", return null
            if (Encoding.ASCII.GetString(magic) != MAGIC)
            {
                fileStream.Close();
                return null;
            }
            // Skip padding zeroes from 0x04 to 0x1f
            fileStream.Seek(0x20, SeekOrigin.Begin);
            
            // Read image data
            var imageData = new byte[IMAGE_WIDTH * IMAGE_HEIGHT * 2];
            fileStream.Read(imageData, 0, imageData.Length);
            
            // Convert image data to bitmap
            banner.Image = ConvertRGB5A1ToBitmap(imageData);
            
            // Read game name
            Debug.WriteLine("Position of metadata: " + fileStream.Position);
            var title = new byte[0x20];
            fileStream.Read(title, 0, 0x20);
            banner.Title = Encoding.ASCII.GetString(title).TrimEnd('\0');
            
            // Read creator name
            var creator = new byte[0x20];
            fileStream.Read(creator, 0, 0x20);
            banner.Creator = Encoding.ASCII.GetString(creator).TrimEnd('\0');
            
            // Read full game name
            var titleFull = new byte[0x40];
            fileStream.Read(titleFull, 0, 0x40);
            banner.TitleFull = Encoding.ASCII.GetString(titleFull).TrimEnd('\0');
            
            // Read full creator name
            var creatorFull = new byte[0x40];
            fileStream.Read(creatorFull, 0, 0x40);
            banner.CreatorFull = Encoding.ASCII.GetString(creatorFull).TrimEnd('\0');
            
            // Read description
            var description = new byte[0x80];
            fileStream.Read(description, 0, 0x80);
            banner.Description = Encoding.ASCII.GetString(description).TrimEnd('\0');
            
            // Close filestream
            fileStream.Close();            
            
            return banner;
        }

        /// <summary>
        /// Converts an image in RGB5A1 format to a bitmap.
        /// <para>
        /// The RGB5A1 image data is stored in a byte array where each pixel is 2 bytes
        /// (1 bit Alpha, followed by 5 bits each for Red, Green, and Blue) in 4x4 pixel tiles.
        /// Read more about the format here: https://hitmen.c02.at/files/yagcd/yagcd/chap17.html#sec17.11
        /// </para>
        /// </summary>
        /// <param name="imageData">The image data in a byte array</param>
        /// <returns>The bitmap image</returns>
        private static Bitmap ConvertRGB5A1ToBitmap(byte[] imageData)
        {
            // Convert bytes into a bitmage image
            // Each pixel is 2 bytes (5 bits for R, 5 bits for G, 5 bits for B, 1 bit for A)
            var bitmap = new Bitmap(IMAGE_WIDTH, IMAGE_HEIGHT);
            
            // Each tile is 4x4 pixels, so the image is 24 tiles wide and 8 tiles tall.
            int gridSize = 4;
            int gridWidth = IMAGE_WIDTH / gridSize;
            int gridHeight = IMAGE_HEIGHT / gridSize;
            
            int index = 0;
            
            // Read each 16 pixels in a 4x4 tile, write to byte stream, then move
            // to the next tile.
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
                            bitmap.SetPixel(gridX * gridSize + x, gridY * gridSize + y, pixel);
                        }
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Converts a bitmap to an image in RGB5A1 format.
        /// <para>
        /// The RGB5A1 image data is stored in a byte array where each pixel is 2 bytes
        /// (1 bit Alpha, followed by 5 bits each for Red, Green, and Blue) in 4x4 pixel tiles.
        /// Read more about the format here: https://hitmen.c02.at/files/yagcd/yagcd/chap17.html#sec17.11
        /// </para>
        /// </summary>
        /// <param name="image">The bitmap image</param>
        /// <returns>The RGB5A1 image as a byte array</returns>
        private static byte[] ConvertBitmapToRGB5A1(Bitmap image)
        {
            // Convert image to RGB5A1 format
            // Each pixel is 2 bytes (5 bits for R, 5 bits for G, 5 bits for B, 1 bit for A)
            
            byte[] rgb5a1Bytes = new byte[IMAGE_WIDTH * IMAGE_HEIGHT * 2];
            
            int gridSize = 4;
            int gridWidth = IMAGE_WIDTH / gridSize;
            int gridHeight = IMAGE_HEIGHT / gridSize;
            
            int index = 0;
            int pixelX, pixelY;
            
            // Store each 16 pixels in a 4x4 tile, then move to the next tile
            for (int gridY = 0; gridY < gridHeight; gridY++)
            {
                for (int gridX = 0; gridX < gridWidth; gridX++)
                {
                    for (int y = 0; y < gridSize; y++)
                    {
                        for (int x = 0; x < gridSize; x++)
                        {
                            pixelX = gridX * gridSize + x;
                            pixelY = gridY * gridSize + y;
                            
                            Color pixel = image.GetPixel(pixelX, pixelY);

                            // Extract color components
                            int r = pixel.R >> 3; // 5 bits
                            int g = pixel.G >> 3; // 5 bits
                            int b = pixel.B >> 3; // 5 bits
                            int a = (pixel.A >= 128) ? 1 : 0; // 1 bit

                            // Combine into two bytes
                            // format: ARRR RRGG GGGB BBBB
                            ushort rgb5a1 = (ushort)((a << 15) | (r << 10) | (g << 5) | b);
 
                            // Store the two bytes in the byte array
                            // Upper byte first
                            rgb5a1Bytes[index++] = (byte)(rgb5a1 >> 8);
                            rgb5a1Bytes[index++] = (byte)(rgb5a1 & 0xFF);
                        }
                    }
                }
            }
            return rgb5a1Bytes;
        }
        
        /// <summary>
        /// Saves the banner to a file.
        /// </summary>
        /// <param name="filepath">The destination file path</param>
        /// <returns>true if the file was saved, otherwise false</returns>
        public bool SaveToFile(string filepath)
        {
            // Save this banner to a file
            
            try
            {
                // Convert image to RGB5A1 format
                // Each pixel is 2 bytes (5 bits for R, 5 bits for G, 5 bits for B, 1 bit for A)
                byte[] imageData = ConvertBitmapToRGB5A1(Image);
             
                // Write byte stream to file
                var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write);
            
                // Write magic "BNR1"
                var magic = Encoding.ASCII.GetBytes(MAGIC);
                fileStream.Write(magic, 0, magic.Length);
            
                // Write padding zeroes from 0x04 to 0x1f
                fileStream.Write(new byte[0x1f - 0x04 + 1], 0, 0x1f - 0x04 + 1);
            
                // Write image data
                fileStream.Write(imageData, 0, imageData.Length);
                
                // Write game name
                var title = Encoding.ASCII.GetBytes(Title);
                fileStream.Write(title, 0, title.Length);

                // Write the creator name
                fileStream.Position = 0x1840;
                var creator = Encoding.ASCII.GetBytes(Creator);
                fileStream.Write(creator, 0, creator.Length);
                
                fileStream.Position = 0x1860;
                // Write the full game name
                var titleFull = Encoding.ASCII.GetBytes(TitleFull);
                fileStream.Write(titleFull, 0, titleFull.Length);
                
                // Write the full creator name
                fileStream.Position = 0x18A0;
                var creatorFull = Encoding.ASCII.GetBytes(CreatorFull);
                fileStream.Write(creatorFull, 0, creatorFull.Length);
                
                // Write the description
                fileStream.Position = 0x18E0;
                var description = Encoding.ASCII.GetBytes(Description);
                fileStream.Write(description, 0, description.Length);

                // Ensure file length is 0x195F
                fileStream.Position = 0x195F;
                fileStream.WriteByte(0);
                
                // Close file
                fileStream.Close();

                return true;
            } 
            catch (Exception)
            {
                return false;
            }
        }
    }
}
using System.Drawing;
using System.IO;
using System.Text;
using Xunit;
using IndustrialPark;

namespace IndustrialParkTest
{
    public class GameCubeBannerTests
    {
        private readonly GameCubeBanner banner;
        
        public GameCubeBannerTests()
        {
            banner = new GameCubeBanner()
            {
                Title = "Test Title",
                TitleFull = "Test Full Title",
                Creator = "Test Creator",
                CreatorFull = "Test Full Creator",
                Description = "Test Description",
                Image = new Bitmap(96, 32)
            };
        }

        [Fact]
        public void ImportFromFile_ReadsImageAndMetadata()
        {
            byte[] data = new byte[0x1960];
            Encoding.ASCII.GetBytes("BNR1").CopyTo(data, 0);
            data[0x20] = 0xFC;
            data[0x21] = 0x00;
            Encoding.ASCII.GetBytes("Title").CopyTo(data, 0x1820);
            Encoding.ASCII.GetBytes("Creator").CopyTo(data, 0x1840);
            Encoding.ASCII.GetBytes("Full Title").CopyTo(data, 0x1860);
            Encoding.ASCII.GetBytes("Full Creator").CopyTo(data, 0x18A0);
            Encoding.ASCII.GetBytes("First\nSecond").CopyTo(data, 0x18E0);
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(path, data);
                GameCubeBanner imported = GameCubeBanner.ImportFromFile(path);
                Assert.NotNull(imported);
                using (imported.Image)
                {
                    Assert.Equal(Color.FromArgb(255, 248, 0, 0).ToArgb(), imported.Image.GetPixel(0, 0).ToArgb());
                    Assert.Equal("Title", imported.Title);
                    Assert.Equal("Creator", imported.Creator);
                    Assert.Equal("Full Title", imported.TitleFull);
                    Assert.Equal("Full Creator", imported.CreatorFull);
                    Assert.Equal("First\r\nSecond", imported.Description);
                }
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(0x181F)]
        [InlineData(0x183F)]
        [InlineData(0x185F)]
        [InlineData(0x189F)]
        [InlineData(0x18DF)]
        [InlineData(0x195F)]
        public void ImportFromFile_RejectsTruncatedBanner(int length)
        {
            byte[] data = new byte[length];
            byte[] magic = Encoding.ASCII.GetBytes("BNR1");
            System.Array.Copy(magic, data, System.Math.Min(magic.Length, length));
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(path, data);
                Assert.Null(GameCubeBanner.ImportFromFile(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ImportFromFile_RejectsInvalidMagic()
        {
            string path = Path.GetTempFileName();
            try
            {
                File.WriteAllBytes(path, Encoding.ASCII.GetBytes("BNR2"));
                Assert.Null(GameCubeBanner.ImportFromFile(path));
            }
            finally
            {
                File.Delete(path);
            }
        }
        
        [Fact]
        public void SaveToFile_Output_Banner_Filesize_Is_Correct()
        {
            int expectedFilesizeBytes = 0x1960;
            banner.SaveToFile("test.bnr");
            Assert.Equal(expectedFilesizeBytes, new System.IO.FileInfo("test.bnr").Length);
            System.IO.File.Delete("test.bnr");
        }
        
        [Fact]
        public void SaveToFile_Magic_Bytes_Are_Correct()
        {
            string expectedMagic = "BNR1";
            banner.SaveToFile("test.bnr");
            
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(
                       System.IO.File.Open("test.bnr", System.IO.FileMode.Open)))
            {
                string magic = new string(reader.ReadChars(4));
                Assert.Equal(expectedMagic, magic);
            }
            System.IO.File.Delete("test.bnr");
        }
        
        [Fact]
        public void SaveToFile_Short_Game_Title_Is_Correct()
        {
            string expectedTitle = "Test Title";
            
            banner.SaveToFile("test.bnr");
            
            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(
                       System.IO.File.Open("test.bnr", System.IO.FileMode.Open)))
            {
                // Game title begins at 0x1820
                reader.BaseStream.Seek(0x1820, System.IO.SeekOrigin.Begin);
                string title = new string(reader.ReadChars(0x20)).TrimEnd('\0');
                Assert.Equal(expectedTitle, title);
            }
            System.IO.File.Delete("test.bnr");
        }
    }
}
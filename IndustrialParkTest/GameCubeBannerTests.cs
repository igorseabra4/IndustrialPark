using System.Drawing;
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
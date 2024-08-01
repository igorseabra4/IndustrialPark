using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using IndustrialPark.Other;
using Image = System.Web.UI.WebControls.Image;

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

        private void saveButton_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(GameCubeBanner.IMAGE_WIDTH, GameCubeBanner.IMAGE_HEIGHT);
            bannerPictureBox.DrawToBitmap(image, bannerPictureBox.ClientRectangle);

            GameCubeBanner banner = new GameCubeBanner
            {
                Image = image,
                Creator = creatorTextBox.Text,
                CreatorFull = creatorFullTextBox.Text,
                Description = descriptionTextBox.Text,
                Title = titleTextBox.Text,
                TitleFull = titleFullTextBox.Text
            };
            
            // Get save location from dialog
            
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "GameCube Banner (*.bnr)|*.bnr",
                Title = "Save GameCube Banner",
                FileName = GameCubeBanner.DEFAULT_FILENAME,
                AddExtension = true,
                DefaultExt = "bnr"
            };
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Save banner to file
                if (banner.SaveToFile(dialog.FileName))
                {
                    MessageBox.Show("Banner saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save banner.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void selectImageButton_Click(object sender, EventArgs e)
        {
            // Open file dialog to select image
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tif;*.tiff",
                Title = "Select an image file"
            };
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                imagePathTextBox.Text = dialog.FileName;
                metadataTextChanged(null, null);
                
                // Update PictureBox with image
                try
                {
                    bannerPictureBox.Image = new Bitmap(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void metadataTextChanged(object sender, EventArgs e)
        {
            // Enable save button if all metadata fields are filled
            saveButton.Enabled = !string.IsNullOrWhiteSpace(titleTextBox.Text) &&
                                 !string.IsNullOrWhiteSpace(creatorTextBox.Text) &&
                                 !string.IsNullOrWhiteSpace(titleFullTextBox.Text) &&
                                 !string.IsNullOrWhiteSpace(creatorFullTextBox.Text) &&
                                 !string.IsNullOrWhiteSpace(descriptionTextBox.Text);
                                 
        }

        private void importBnrButton_Click(object sender, EventArgs e)
        {
            // Open file dialog to select BNR file
            
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "GameCube Banner (*.bnr)|*.bnr",
                Title = "Select a GameCube Banner file"
            };
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Import banner from file
                GameCubeBanner banner = GameCubeBanner.ImportFromFile(dialog.FileName);
                
                if (banner != null)
                {
                    // Set metadata fields
                    titleTextBox.Text = banner.Title;
                    creatorTextBox.Text = banner.Creator;
                    titleFullTextBox.Text = banner.TitleFull;
                    creatorFullTextBox.Text = banner.CreatorFull;
                    descriptionTextBox.Text = banner.Description;
                    
                    // Convert image to PictureBox
                    bannerPictureBox.Image = banner.Image;
                    
                    // Update image path text box
                    imagePathTextBox.Text = dialog.FileName;
                }
                else
                {
                    MessageBox.Show("Failed to import banner.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
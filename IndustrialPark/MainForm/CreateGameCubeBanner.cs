using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class CreateGameCubeBanner : Form
    {
        public CreateGameCubeBanner()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Create a new CreateGameCubeBanner form with the specified banner filepath.
        /// </summary>
        /// <param name="banner">The filepath to the GameCube Banner (.bnr)</param>
        public CreateGameCubeBanner(string banner) : this()
        {
            UpdateFieldsFromBanner(banner);
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

        private void UpdateFieldsFromBanner(string filepath)
        {
            // Import banner from file
            GameCubeBanner banner = GameCubeBanner.ImportFromFile(filepath);
                
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
                imagePathTextBox.Text = filepath;
            }
            else
            {
                MessageBox.Show("Failed to import banner.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private bool IsSupportedImageFile(string extension)
        {
            // List of image file extensions
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".tif" };
            return Array.Exists(imageExtensions, ext => ext == extension);
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
                UpdateFieldsFromBanner(dialog.FileName);
            }
        }

        private void CreateGameCubeBanner_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string extension = Path.GetExtension(files[0]).ToLower();
                    if (IsSupportedImageFile(extension) || extension == GameCubeBanner.DEFAULT_FILE_EXTENSION)
                    {
                        e.Effect = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CreateGameCubeBanner_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    string filePath = files[0];
                    string extension = Path.GetExtension(filePath).ToLower();

                    try
                    {
                        if (IsSupportedImageFile(extension))
                        {
                            var image = System.Drawing.Image.FromFile(filePath);
                            bannerPictureBox.Image = image;
                            imagePathTextBox.Text = filePath;
                        }
                        else if (extension == GameCubeBanner.DEFAULT_FILE_EXTENSION)
                        {
                            UpdateFieldsFromBanner(filePath);
                        }
                        else
                        {
                            MessageBox.Show("Unsupported file type. Must be a supported image or GameCube Banner file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
    }
}
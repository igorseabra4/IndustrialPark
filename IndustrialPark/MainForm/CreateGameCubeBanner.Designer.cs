using System.ComponentModel;

namespace IndustrialPark
{
    partial class CreateGameCubeBanner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateGameCubeBanner));
            this.label1 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.bannerPictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.creatorFullTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.titleFullTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.creatorTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.titleTextBox = new System.Windows.Forms.TextBox();
            this.selectImageButton = new System.Windows.Forms.Button();
            this.imagePathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.importBnrButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(147, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(371, 46);
            this.label1.TabIndex = 1;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(296, 306);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(124, 23);
            this.saveButton.TabIndex = 16;
            this.saveButton.Text = "Save Banner As...";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(426, 306);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 23);
            this.cancelButton.TabIndex = 17;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // bannerPictureBox
            // 
            this.bannerPictureBox.Image = global::IndustrialPark.Properties.Resources.PlaceholderBanner;
            this.bannerPictureBox.InitialImage = null;
            this.bannerPictureBox.Location = new System.Drawing.Point(90, 22);
            this.bannerPictureBox.Name = "bannerPictureBox";
            this.bannerPictureBox.Size = new System.Drawing.Size(96, 32);
            this.bannerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.bannerPictureBox.TabIndex = 3;
            this.bannerPictureBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.descriptionTextBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.creatorFullTextBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.titleFullTextBox);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.creatorTextBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.titleTextBox);
            this.groupBox1.Controls.Add(this.selectImageButton);
            this.groupBox1.Controls.Add(this.imagePathTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.bannerPictureBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(506, 242);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Banner Metadata";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.Location = new System.Drawing.Point(90, 178);
            this.descriptionTextBox.MaxLength = 128;
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.Size = new System.Drawing.Size(407, 47);
            this.descriptionTextBox.TabIndex = 15;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.metadataTextChanged);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(3, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 47);
            this.label7.TabIndex = 14;
            this.label7.Text = "Description";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // creatorFullTextBox
            // 
            this.creatorFullTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.creatorFullTextBox.Location = new System.Drawing.Point(90, 139);
            this.creatorFullTextBox.MaxLength = 64;
            this.creatorFullTextBox.Name = "creatorFullTextBox";
            this.creatorFullTextBox.Size = new System.Drawing.Size(410, 20);
            this.creatorFullTextBox.TabIndex = 13;
            this.creatorFullTextBox.TextChanged += new System.EventHandler(this.metadataTextChanged);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(3, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 32);
            this.label6.TabIndex = 12;
            this.label6.Text = "Creator (Full)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // titleFullTextBox
            // 
            this.titleFullTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleFullTextBox.Location = new System.Drawing.Point(90, 104);
            this.titleFullTextBox.MaxLength = 64;
            this.titleFullTextBox.Name = "titleFullTextBox";
            this.titleFullTextBox.Size = new System.Drawing.Size(410, 20);
            this.titleFullTextBox.TabIndex = 11;
            this.titleFullTextBox.TextChanged += new System.EventHandler(this.metadataTextChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 32);
            this.label5.TabIndex = 10;
            this.label5.Text = "Game Title (Full)\r\n";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // creatorTextBox
            // 
            this.creatorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.creatorTextBox.Location = new System.Drawing.Point(358, 67);
            this.creatorTextBox.MaxLength = 32;
            this.creatorTextBox.Name = "creatorTextBox";
            this.creatorTextBox.Size = new System.Drawing.Size(142, 20);
            this.creatorTextBox.TabIndex = 9;
            this.creatorTextBox.TextChanged += new System.EventHandler(this.metadataTextChanged);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(299, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 32);
            this.label4.TabIndex = 8;
            this.label4.Text = "Creator";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 32);
            this.label3.TabIndex = 6;
            this.label3.Text = "Game Name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // titleTextBox
            // 
            this.titleTextBox.Location = new System.Drawing.Point(90, 67);
            this.titleTextBox.MaxLength = 32;
            this.titleTextBox.Name = "titleTextBox";
            this.titleTextBox.Size = new System.Drawing.Size(203, 20);
            this.titleTextBox.TabIndex = 7;
            this.titleTextBox.TextChanged += new System.EventHandler(this.metadataTextChanged);
            // 
            // selectImageButton
            // 
            this.selectImageButton.Location = new System.Drawing.Point(402, 25);
            this.selectImageButton.Name = "selectImageButton";
            this.selectImageButton.Size = new System.Drawing.Size(98, 26);
            this.selectImageButton.TabIndex = 5;
            this.selectImageButton.Text = "Select Image...";
            this.selectImageButton.UseVisualStyleBackColor = true;
            this.selectImageButton.Click += new System.EventHandler(this.selectImageButton_Click);
            // 
            // imagePathTextBox
            // 
            this.imagePathTextBox.Location = new System.Drawing.Point(192, 29);
            this.imagePathTextBox.Name = "imagePathTextBox";
            this.imagePathTextBox.ReadOnly = true;
            this.imagePathTextBox.Size = new System.Drawing.Size(204, 20);
            this.imagePathTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(18, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Image";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // importBnrButton
            // 
            this.importBnrButton.Location = new System.Drawing.Point(12, 16);
            this.importBnrButton.Name = "importBnrButton";
            this.importBnrButton.Size = new System.Drawing.Size(129, 26);
            this.importBnrButton.TabIndex = 0;
            this.importBnrButton.Text = "Import from BNR...";
            this.importBnrButton.UseVisualStyleBackColor = true;
            this.importBnrButton.Click += new System.EventHandler(this.importBnrButton_Click);
            // 
            // CreateGameCubeBanner
            // 
            this.AcceptButton = this.saveButton;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(530, 341);
            this.Controls.Add(this.importBnrButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Location = new System.Drawing.Point(15, 15);
            this.MaximizeBox = false;
            this.Name = "CreateGameCubeBanner";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create GameCube Banner";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.CreateGameCubeBanner_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.CreateGameCubeBanner_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.bannerPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button importBnrButton;

        private System.Windows.Forms.TextBox descriptionTextBox;

        private System.Windows.Forms.TextBox creatorFullTextBox;
        private System.Windows.Forms.Label label7;

        private System.Windows.Forms.Label label6;

        private System.Windows.Forms.TextBox titleFullTextBox;
        private System.Windows.Forms.Label label5;

        private System.Windows.Forms.TextBox imagePathTextBox;
        private System.Windows.Forms.Button selectImageButton;
        private System.Windows.Forms.TextBox titleTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox creatorTextBox;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.PictureBox bannerPictureBox;

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;

        private System.Windows.Forms.Label label1;

        #endregion
    }
}
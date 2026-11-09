namespace IndustrialPark
{
    partial class AboutBox
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            tabControl = new System.Windows.Forms.TabControl();
            tabWelcome = new System.Windows.Forms.TabPage();
            welcomeLayout = new System.Windows.Forms.TableLayoutPanel();
            welcomeTitle = new System.Windows.Forms.Label();
            welcomeText = new System.Windows.Forms.Label();
            resourcesLabel = new System.Windows.Forms.Label();
            checkShowOnStartup = new System.Windows.Forms.CheckBox();
            checkForUpdatesOnStartup = new System.Windows.Forms.CheckBox();
            buttonViewControls = new System.Windows.Forms.Button();
            buttonClose = new System.Windows.Forms.Button();
            tabAbout = new System.Windows.Forms.TabPage();
            aboutLayout = new System.Windows.Forms.TableLayoutPanel();
            labelProductName = new System.Windows.Forms.Label();
            aboutText = new System.Windows.Forms.TextBox();
            tabControl.SuspendLayout();
            tabWelcome.SuspendLayout();
            welcomeLayout.SuspendLayout();
            tabAbout.SuspendLayout();
            aboutLayout.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabWelcome);
            tabControl.Controls.Add(tabAbout);
            tabControl.Location = new System.Drawing.Point(6, 6);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(572, 370);
            tabControl.TabIndex = 0;
            // 
            // tabWelcome
            // 
            tabWelcome.Controls.Add(welcomeLayout);
            tabWelcome.Location = new System.Drawing.Point(4, 24);
            tabWelcome.Name = "tabWelcome";
            tabWelcome.Padding = new System.Windows.Forms.Padding(12);
            tabWelcome.Size = new System.Drawing.Size(564, 342);
            tabWelcome.TabIndex = 0;
            tabWelcome.Text = "Welcome";
            // 
            // welcomeLayout
            // 
            welcomeLayout.ColumnCount = 1;
            welcomeLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            welcomeLayout.Controls.Add(buttonViewControls, 0, 9);
            welcomeLayout.Controls.Add(welcomeTitle, 0, 0);
            welcomeLayout.Controls.Add(welcomeText, 0, 1);
            welcomeLayout.Controls.Add(resourcesLabel, 0, 2);
            welcomeLayout.Controls.Add(checkShowOnStartup, 0, 7);
            welcomeLayout.Controls.Add(checkForUpdatesOnStartup, 0, 8);
            welcomeLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            welcomeLayout.Location = new System.Drawing.Point(12, 12);
            welcomeLayout.Name = "welcomeLayout";
            welcomeLayout.RowCount = 10;
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            welcomeLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            welcomeLayout.Size = new System.Drawing.Size(540, 318);
            welcomeLayout.TabIndex = 0;
            // 
            // welcomeTitle
            // 
            welcomeTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            welcomeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold);
            welcomeTitle.Location = new System.Drawing.Point(3, 0);
            welcomeTitle.Name = "welcomeTitle";
            welcomeTitle.Size = new System.Drawing.Size(534, 36);
            welcomeTitle.TabIndex = 0;
            welcomeTitle.Text = "Welcome to Industrial Park";
            welcomeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // welcomeText
            // 
            welcomeText.Dock = System.Windows.Forms.DockStyle.Fill;
            welcomeText.Location = new System.Drawing.Point(3, 36);
            welcomeText.Name = "welcomeText";
            welcomeText.Size = new System.Drawing.Size(534, 54);
            welcomeText.TabIndex = 1;
            welcomeText.Text = "Industrial Park is an editor for HIP/HOP archives and game assets.\nUse the resources below to get started, learn more, or connect with the community.";
            welcomeText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // resourcesLabel
            // 
            resourcesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            resourcesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            resourcesLabel.Location = new System.Drawing.Point(3, 90);
            resourcesLabel.Name = "resourcesLabel";
            resourcesLabel.Size = new System.Drawing.Size(534, 30);
            resourcesLabel.TabIndex = 2;
            resourcesLabel.Text = "Resources";
            resourcesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkShowOnStartup
            // 
            checkShowOnStartup.AutoSize = true;
            checkShowOnStartup.Location = new System.Drawing.Point(3, 223);
            checkShowOnStartup.Name = "checkShowOnStartup";
            checkShowOnStartup.Size = new System.Drawing.Size(220, 20);
            checkShowOnStartup.TabIndex = 1;
            checkShowOnStartup.Text = "Show welcome window at startup";
            // 
            // checkForUpdatesOnStartup
            // 
            checkForUpdatesOnStartup.AutoSize = true;
            checkForUpdatesOnStartup.Location = new System.Drawing.Point(3, 253);
            checkForUpdatesOnStartup.Name = "checkForUpdatesOnStartup";
            checkForUpdatesOnStartup.Size = new System.Drawing.Size(276, 20);
            checkForUpdatesOnStartup.TabIndex = 2;
            checkForUpdatesOnStartup.Text = "Check for updates at startup, once per day";
            checkForUpdatesOnStartup.CheckedChanged += checkForUpdatesOnStartup_CheckedChanged;
            // 
            // buttonViewControls
            // 
            buttonViewControls.Anchor = System.Windows.Forms.AnchorStyles.Left;
            buttonViewControls.Location = new System.Drawing.Point(3, 286);
            buttonViewControls.Name = "buttonViewControls";
            buttonViewControls.Size = new System.Drawing.Size(121, 25);
            buttonViewControls.TabIndex = 3;
            buttonViewControls.Text = "View Controls";
            buttonViewControls.UseVisualStyleBackColor = true;
            buttonViewControls.Click += buttonViewControls_Click;
            // 
            // buttonClose
            // 
            buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonClose.Location = new System.Drawing.Point(489, 381);
            buttonClose.Name = "buttonClose";
            buttonClose.Size = new System.Drawing.Size(75, 25);
            buttonClose.TabIndex = 4;
            buttonClose.Text = "Close";
            buttonClose.Click += buttonClose_Click;
            // 
            // tabAbout
            // 
            tabAbout.Controls.Add(aboutLayout);
            tabAbout.Location = new System.Drawing.Point(4, 24);
            tabAbout.Name = "tabAbout";
            tabAbout.Padding = new System.Windows.Forms.Padding(12);
            tabAbout.Size = new System.Drawing.Size(552, 336);
            tabAbout.TabIndex = 1;
            tabAbout.Text = "About";
            // 
            // aboutLayout
            // 
            aboutLayout.ColumnCount = 1;
            aboutLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            aboutLayout.Controls.Add(labelProductName, 0, 0);
            aboutLayout.Controls.Add(aboutText, 0, 1);
            aboutLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            aboutLayout.Location = new System.Drawing.Point(12, 12);
            aboutLayout.Name = "aboutLayout";
            aboutLayout.RowCount = 2;
            aboutLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            aboutLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            aboutLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            aboutLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            aboutLayout.Size = new System.Drawing.Size(528, 312);
            aboutLayout.TabIndex = 0;
            // 
            // labelProductName
            // 
            labelProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            labelProductName.Location = new System.Drawing.Point(3, 0);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new System.Drawing.Size(522, 23);
            labelProductName.TabIndex = 5;
            labelProductName.Text = "Industrial Park";
            labelProductName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // aboutText
            // 
            aboutText.Dock = System.Windows.Forms.DockStyle.Fill;
            aboutText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            aboutText.Location = new System.Drawing.Point(3, 35);
            aboutText.Multiline = true;
            aboutText.Name = "aboutText";
            aboutText.ReadOnly = true;
            aboutText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            aboutText.Size = new System.Drawing.Size(522, 274);
            aboutText.TabIndex = 6;
            aboutText.TabStop = false;
            aboutText.Text = resources.GetString("aboutText.Text");
            // 
            // AboutBox
            // 
            AcceptButton = buttonClose;
            CancelButton = buttonClose;
            ClientSize = new System.Drawing.Size(584, 416);
            Controls.Add(tabControl);
            Controls.Add(buttonClose);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutBox";
            Padding = new System.Windows.Forms.Padding(3);
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Industrial Park";
            tabControl.ResumeLayout(false);
            tabWelcome.ResumeLayout(false);
            welcomeLayout.ResumeLayout(false);
            welcomeLayout.PerformLayout();
            tabAbout.ResumeLayout(false);
            aboutLayout.ResumeLayout(false);
            aboutLayout.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabWelcome;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.TableLayoutPanel welcomeLayout;
        private System.Windows.Forms.TableLayoutPanel aboutLayout;
        private System.Windows.Forms.Label welcomeTitle;
        private System.Windows.Forms.Label welcomeText;
        private System.Windows.Forms.Label resourcesLabel;
        private System.Windows.Forms.CheckBox checkShowOnStartup;
        private System.Windows.Forms.CheckBox checkForUpdatesOnStartup;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.TextBox aboutText;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonViewControls;
    }
}
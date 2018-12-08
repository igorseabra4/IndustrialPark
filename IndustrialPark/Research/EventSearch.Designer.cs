namespace IndustrialPark
{
    partial class EventSearch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.buttonChooseRoot = new System.Windows.Forms.Button();
            this.labelRootDir = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboSenderAsset = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboRecieveEvent = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboTargetEvent = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboTargetAsset = new System.Windows.Forms.ComboBox();
            this.buttonPerform = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonChooseRoot
            // 
            this.buttonChooseRoot.Location = new System.Drawing.Point(18, 129);
            this.buttonChooseRoot.Name = "buttonChooseRoot";
            this.buttonChooseRoot.Size = new System.Drawing.Size(188, 23);
            this.buttonChooseRoot.TabIndex = 0;
            this.buttonChooseRoot.Text = "Choose Root Directory";
            this.buttonChooseRoot.UseVisualStyleBackColor = true;
            this.buttonChooseRoot.Click += new System.EventHandler(this.buttonChooseRoot_Click);
            // 
            // labelRootDir
            // 
            this.labelRootDir.AutoSize = true;
            this.labelRootDir.Location = new System.Drawing.Point(12, 9);
            this.labelRootDir.Name = "labelRootDir";
            this.labelRootDir.Size = new System.Drawing.Size(107, 13);
            this.labelRootDir.TabIndex = 1;
            this.labelRootDir.Text = "Root Directory: None";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboSenderAsset);
            this.groupBox1.Location = new System.Drawing.Point(12, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 46);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sender Asset Filter";
            // 
            // comboSenderAsset
            // 
            this.comboSenderAsset.FormattingEnabled = true;
            this.comboSenderAsset.Location = new System.Drawing.Point(6, 19);
            this.comboSenderAsset.Name = "comboSenderAsset";
            this.comboSenderAsset.Size = new System.Drawing.Size(188, 21);
            this.comboSenderAsset.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboRecieveEvent);
            this.groupBox2.Location = new System.Drawing.Point(12, 77);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 46);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Recieve Event Filter";
            // 
            // comboRecieveEvent
            // 
            this.comboRecieveEvent.FormattingEnabled = true;
            this.comboRecieveEvent.Location = new System.Drawing.Point(6, 19);
            this.comboRecieveEvent.Name = "comboRecieveEvent";
            this.comboRecieveEvent.Size = new System.Drawing.Size(188, 21);
            this.comboRecieveEvent.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboTargetEvent);
            this.groupBox3.Location = new System.Drawing.Point(218, 77);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 46);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Target Event Filter";
            // 
            // comboTargetEvent
            // 
            this.comboTargetEvent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboTargetEvent.FormattingEnabled = true;
            this.comboTargetEvent.Location = new System.Drawing.Point(6, 19);
            this.comboTargetEvent.Name = "comboTargetEvent";
            this.comboTargetEvent.Size = new System.Drawing.Size(188, 21);
            this.comboTargetEvent.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboTargetAsset);
            this.groupBox4.Location = new System.Drawing.Point(218, 25);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 46);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Target Asset Filter";
            // 
            // comboTargetAsset
            // 
            this.comboTargetAsset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboTargetAsset.FormattingEnabled = true;
            this.comboTargetAsset.Location = new System.Drawing.Point(6, 19);
            this.comboTargetAsset.Name = "comboTargetAsset";
            this.comboTargetAsset.Size = new System.Drawing.Size(188, 21);
            this.comboTargetAsset.TabIndex = 3;
            // 
            // buttonPerform
            // 
            this.buttonPerform.Location = new System.Drawing.Point(224, 129);
            this.buttonPerform.Name = "buttonPerform";
            this.buttonPerform.Size = new System.Drawing.Size(188, 23);
            this.buttonPerform.TabIndex = 8;
            this.buttonPerform.Text = "Perform Search";
            this.buttonPerform.UseVisualStyleBackColor = true;
            this.buttonPerform.Click += new System.EventHandler(this.buttonPerform_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(12, 491);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(402, 70);
            this.richTextBox2.TabIndex = 9;
            this.richTextBox2.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 158);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(400, 23);
            this.progressBar1.TabIndex = 10;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 187);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(402, 298);
            this.richTextBox1.TabIndex = 11;
            this.richTextBox1.Text = "";
            // 
            // EventSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 573);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.buttonPerform);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelRootDir);
            this.Controls.Add(this.buttonChooseRoot);
            this.MaximizeBox = false;
            this.Name = "EventSearch";
            this.ShowIcon = false;
            this.Text = "Event Search";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EventSearch_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonChooseRoot;
        private System.Windows.Forms.Label labelRootDir;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboSenderAsset;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboRecieveEvent;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboTargetEvent;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboTargetAsset;
        private System.Windows.Forms.Button buttonPerform;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}
namespace IndustrialPark
{
    partial class BuildISO
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildISO));
            label1 = new System.Windows.Forms.Label();
            pcsx2PathTextBox = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            pcsx2ArgumentsTextBox = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            button2 = new System.Windows.Forms.Button();
            treeView1 = new System.Windows.Forms.TreeView();
            imageList1 = new System.Windows.Forms.ImageList(components);
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            buildRunButton = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            outputIsoPath = new System.Windows.Forms.TextBox();
            button4 = new System.Windows.Forms.Button();
            buildButton = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            gameDirBox = new System.Windows.Forms.ComboBox();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 13);
            label1.TabIndex = 0;
            label1.Text = "PCSX2 Path";
            // 
            // pcsx2PathTextBox
            // 
            pcsx2PathTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pcsx2PathTextBox.Location = new System.Drawing.Point(15, 25);
            pcsx2PathTextBox.Name = "pcsx2PathTextBox";
            pcsx2PathTextBox.Size = new System.Drawing.Size(351, 20);
            pcsx2PathTextBox.TabIndex = 1;
            pcsx2PathTextBox.TextChanged += pcsx2PathTextBox_TextChanged;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button1.Location = new System.Drawing.Point(372, 25);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(41, 20);
            button1.TabIndex = 2;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 48);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(94, 13);
            label2.TabIndex = 3;
            label2.Text = "PCSX2 Arguments";
            // 
            // pcsx2ArgumentsTextBox
            // 
            pcsx2ArgumentsTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pcsx2ArgumentsTextBox.Location = new System.Drawing.Point(15, 64);
            pcsx2ArgumentsTextBox.Name = "pcsx2ArgumentsTextBox";
            pcsx2ArgumentsTextBox.Size = new System.Drawing.Size(351, 20);
            pcsx2ArgumentsTextBox.TabIndex = 3;
            pcsx2ArgumentsTextBox.Text = "-nogui -fastboot";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 133);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(80, 13);
            label3.TabIndex = 5;
            label3.Text = "Game Directory";
            // 
            // button2
            // 
            button2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button2.Location = new System.Drawing.Point(372, 149);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(41, 20);
            button2.TabIndex = 8;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // treeView1
            // 
            treeView1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            treeView1.CheckBoxes = true;
            treeView1.ImageIndex = 0;
            treeView1.ImageList = imageList1;
            treeView1.Location = new System.Drawing.Point(15, 175);
            treeView1.Name = "treeView1";
            treeView1.SelectedImageIndex = 0;
            treeView1.Size = new System.Drawing.Size(398, 262);
            treeView1.TabIndex = 8;
            treeView1.AfterCheck += treeView1_AfterCheck;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = System.Drawing.Color.Transparent;
            imageList1.Images.SetKeyName(0, "folder.png");
            imageList1.Images.SetKeyName(1, "reload.png");
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2 });
            statusStrip1.Location = new System.Drawing.Point(0, 469);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(425, 22);
            statusStrip1.TabIndex = 9;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new System.Drawing.Size(23, 17);
            toolStripStatusLabel2.Text = "0 B";
            // 
            // buildRunButton
            // 
            buildRunButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buildRunButton.Location = new System.Drawing.Point(317, 443);
            buildRunButton.Name = "buildRunButton";
            buildRunButton.Size = new System.Drawing.Size(96, 23);
            buildRunButton.TabIndex = 10;
            buildRunButton.Text = "Build ISO && Run";
            buildRunButton.UseVisualStyleBackColor = true;
            buildRunButton.Click += buttonRunButton_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 94);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(85, 13);
            label4.TabIndex = 11;
            label4.Text = "Output ISO Path";
            // 
            // outputIsoPath
            // 
            outputIsoPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            outputIsoPath.Location = new System.Drawing.Point(15, 110);
            outputIsoPath.Name = "outputIsoPath";
            outputIsoPath.Size = new System.Drawing.Size(351, 20);
            outputIsoPath.TabIndex = 4;
            // 
            // button4
            // 
            button4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button4.Location = new System.Drawing.Point(372, 110);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(41, 20);
            button4.TabIndex = 5;
            button4.Text = "...";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // buildButton
            // 
            buildButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buildButton.Location = new System.Drawing.Point(215, 443);
            buildButton.Name = "buildButton";
            buildButton.Size = new System.Drawing.Size(96, 23);
            buildButton.TabIndex = 9;
            buildButton.Text = "Build ISO";
            buildButton.UseVisualStyleBackColor = true;
            buildButton.Click += button5_Click;
            // 
            // button3
            // 
            button3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            button3.ImageKey = "reload.png";
            button3.ImageList = imageList1;
            button3.Location = new System.Drawing.Point(342, 149);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(27, 21);
            button3.TabIndex = 7;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // gameDirBox
            // 
            gameDirBox.FormattingEnabled = true;
            gameDirBox.Location = new System.Drawing.Point(15, 150);
            gameDirBox.Name = "gameDirBox";
            gameDirBox.Size = new System.Drawing.Size(321, 21);
            gameDirBox.TabIndex = 6;
            gameDirBox.SelectedIndexChanged += gameDirBox_SelectedIndexChanged;
            gameDirBox.KeyPress += gameDirBox_KeyPress;
            // 
            // BuildISO
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(425, 491);
            Controls.Add(gameDirBox);
            Controls.Add(button3);
            Controls.Add(buildButton);
            Controls.Add(button4);
            Controls.Add(outputIsoPath);
            Controls.Add(label4);
            Controls.Add(buildRunButton);
            Controls.Add(statusStrip1);
            Controls.Add(treeView1);
            Controls.Add(button2);
            Controls.Add(label3);
            Controls.Add(pcsx2ArgumentsTextBox);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(pcsx2PathTextBox);
            Controls.Add(label1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "BuildISO";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Build/Run PS2 ISO";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pcsx2PathTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox pcsx2ArgumentsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button buildRunButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox outputIsoPath;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button buildButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox gameDirBox;
    }
}
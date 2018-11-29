namespace IndustrialPark
{
    partial class InternalSoundEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InternalSoundEditor));
            this.labelAssetName = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxSendToSNDI = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelAssetName
            // 
            this.labelAssetName.AutoSize = true;
            this.labelAssetName.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelAssetName.Location = new System.Drawing.Point(3, 0);
            this.labelAssetName.Name = "labelAssetName";
            this.labelAssetName.Size = new System.Drawing.Size(0, 20);
            this.labelAssetName.TabIndex = 6;
            this.labelAssetName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.button2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelAssetName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.richTextBox1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.button1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSendToSNDI, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(293, 307);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(3, 282);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(287, 22);
            this.buttonFindCallers.TabIndex = 11;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(3, 254);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(287, 22);
            this.button2.TabIndex = 10;
            this.button2.Text = "Export Sound Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(3, 73);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(287, 175);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(3, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(287, 22);
            this.button1.TabIndex = 7;
            this.button1.Text = "Import Sound Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxSendToSNDI
            // 
            this.checkBoxSendToSNDI.AutoSize = true;
            this.checkBoxSendToSNDI.Location = new System.Drawing.Point(3, 23);
            this.checkBoxSendToSNDI.Name = "checkBoxSendToSNDI";
            this.checkBoxSendToSNDI.Size = new System.Drawing.Size(178, 16);
            this.checkBoxSendToSNDI.TabIndex = 8;
            this.checkBoxSendToSNDI.Text = "Trim header and send it to SNDI";
            this.checkBoxSendToSNDI.UseVisualStyleBackColor = true;
            // 
            // InternalSoundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 307);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "InternalSoundEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalAssetEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelAssetName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxSendToSNDI;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonFindCallers;
    }
}
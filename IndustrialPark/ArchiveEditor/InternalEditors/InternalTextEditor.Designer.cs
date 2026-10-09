namespace IndustrialPark
{
    partial class InternalTextEditor
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
            richTextBoxAssetText = new System.Windows.Forms.RichTextBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonHelp = new System.Windows.Forms.Button();
            buttonFindCallers = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBoxAssetText
            // 
            tableLayoutPanel1.SetColumnSpan(richTextBoxAssetText, 2);
            richTextBoxAssetText.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxAssetText.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            richTextBoxAssetText.Location = new System.Drawing.Point(3, 3);
            richTextBoxAssetText.Name = "richTextBoxAssetText";
            richTextBoxAssetText.Size = new System.Drawing.Size(618, 247);
            richTextBoxAssetText.TabIndex = 7;
            richTextBoxAssetText.Text = "";
            richTextBoxAssetText.TextChanged += richTextBoxAssetText_TextChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonHelp, 0, 1);
            tableLayoutPanel1.Controls.Add(buttonFindCallers, 1, 1);
            tableLayoutPanel1.Controls.Add(richTextBoxAssetText, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(624, 281);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonHelp
            // 
            buttonHelp.AutoSize = true;
            buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonHelp.Location = new System.Drawing.Point(3, 256);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(306, 22);
            buttonHelp.TabIndex = 18;
            buttonHelp.Text = "Open Wiki Page";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // buttonFindCallers
            // 
            buttonFindCallers.AutoSize = true;
            buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonFindCallers.Location = new System.Drawing.Point(315, 256);
            buttonFindCallers.Name = "buttonFindCallers";
            buttonFindCallers.Size = new System.Drawing.Size(306, 22);
            buttonFindCallers.TabIndex = 8;
            buttonFindCallers.Text = "Find Who Targets Me";
            buttonFindCallers.UseVisualStyleBackColor = true;
            buttonFindCallers.Click += buttonFindCallers_Click;
            // 
            // InternalTextEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(624, 281);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            Name = "InternalTextEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Data Editor";
            FormClosing += InternalTextEditor_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBoxAssetText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonHelp;
    }
}
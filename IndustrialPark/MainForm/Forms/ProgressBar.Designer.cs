namespace IndustrialPark
{
    partial class ProgressBar
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
            pBar = new System.Windows.Forms.ProgressBar();
            labelLoading = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // pBar
            // 
            pBar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            pBar.Location = new System.Drawing.Point(12, 12);
            pBar.Name = "pBar";
            pBar.Size = new System.Drawing.Size(360, 26);
            pBar.TabIndex = 0;
            // 
            // labelLoading
            // 
            labelLoading.AutoSize = true;
            labelLoading.Location = new System.Drawing.Point(13, 41);
            labelLoading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelLoading.Name = "labelLoading";
            labelLoading.Size = new System.Drawing.Size(45, 13);
            labelLoading.TabIndex = 1;
            labelLoading.Text = "Loading";
            // 
            // ProgressBar
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(384, 61);
            ControlBox = false;
            Controls.Add(labelLoading);
            Controls.Add(pBar);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressBar";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Opening Archive";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pBar;
        private System.Windows.Forms.Label labelLoading;
    }
}
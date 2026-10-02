namespace IndustrialPark
{
    partial class RWAnalyze
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RWAnalyze));
            this.propertyGridData = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGridData
            // 
            resources.ApplyResources(this.propertyGridData, "propertyGridData");
            this.propertyGridData.Name = "propertyGridData";
            this.propertyGridData.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridData.ToolbarVisible = false;
            // 
            // RWAnalyze
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.propertyGridData);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "RWAnalyze";
            this.ShowIcon = false;
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridData;
    }
}
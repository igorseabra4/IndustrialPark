namespace IndustrialPark;

partial class ArchiveComparerForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        labelSource = new System.Windows.Forms.Label();
        labelDestination = new System.Windows.Forms.Label();
        comboBoxSource = new System.Windows.Forms.ComboBox();
        comboBoxDestination = new System.Windows.Forms.ComboBox();
        buttonGenerate = new System.Windows.Forms.Button();
        buttonSaveDelta = new System.Windows.Forms.Button();
        textBoxReport = new System.Windows.Forms.RichTextBox();
        SuspendLayout();
        // 
        // labelSource
        // 
        labelSource.AutoSize = true;
        labelSource.Location = new System.Drawing.Point(12, 15);
        labelSource.Name = "labelSource";
        labelSource.Size = new System.Drawing.Size(44, 15);
        labelSource.TabIndex = 0;
        labelSource.Text = "Source:";
        // 
        // labelDestination
        // 
        labelDestination.AutoSize = true;
        labelDestination.Location = new System.Drawing.Point(12, 48);
        labelDestination.Name = "labelDestination";
        labelDestination.Size = new System.Drawing.Size(73, 15);
        labelDestination.TabIndex = 1;
        labelDestination.Text = "Destination:";
        // 
        // comboBoxSource
        // 
        comboBoxSource.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        comboBoxSource.FormattingEnabled = true;
        comboBoxSource.Location = new System.Drawing.Point(91, 12);
        comboBoxSource.Name = "comboBoxSource";
        comboBoxSource.Size = new System.Drawing.Size(681, 23);
        comboBoxSource.TabIndex = 2;
        // 
        // comboBoxDestination
        // 
        comboBoxDestination.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        comboBoxDestination.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        comboBoxDestination.FormattingEnabled = true;
        comboBoxDestination.Location = new System.Drawing.Point(91, 45);
        comboBoxDestination.Name = "comboBoxDestination";
        comboBoxDestination.Size = new System.Drawing.Size(681, 23);
        comboBoxDestination.TabIndex = 3;
        // 
        // buttonGenerate
        // 
        buttonGenerate.Location = new System.Drawing.Point(12, 78);
        buttonGenerate.Name = "buttonGenerate";
        buttonGenerate.Size = new System.Drawing.Size(151, 29);
        buttonGenerate.TabIndex = 4;
        buttonGenerate.Text = "Generate Comparison";
        buttonGenerate.UseVisualStyleBackColor = true;
        buttonGenerate.Click += buttonGenerate_Click;
        // 
        // buttonSaveDelta
        // 
        buttonSaveDelta.Location = new System.Drawing.Point(169, 78);
        buttonSaveDelta.Name = "buttonSaveDelta";
        buttonSaveDelta.Size = new System.Drawing.Size(151, 29);
        buttonSaveDelta.TabIndex = 5;
        buttonSaveDelta.Text = "Save Delta Archive";
        buttonSaveDelta.UseVisualStyleBackColor = true;
        buttonSaveDelta.Click += buttonSaveDelta_Click;
        // 
        // textBoxReport
        // 
        textBoxReport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        textBoxReport.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        textBoxReport.Location = new System.Drawing.Point(12, 113);
        textBoxReport.Name = "textBoxReport";
        textBoxReport.ReadOnly = true;
        textBoxReport.Size = new System.Drawing.Size(760, 424);
        textBoxReport.TabIndex = 6;
        textBoxReport.Text = "";
        // 
        // ArchiveComparerForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(784, 549);
        Controls.Add(textBoxReport);
        Controls.Add(buttonSaveDelta);
        Controls.Add(buttonGenerate);
        Controls.Add(comboBoxDestination);
        Controls.Add(comboBoxSource);
        Controls.Add(labelDestination);
        Controls.Add(labelSource);
        MinimumSize = new System.Drawing.Size(500, 350);
        Name = "ArchiveComparerForm";
        ShowIcon = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Archive Comparer";
        ResumeLayout(false);
        PerformLayout();
    }

    private System.Windows.Forms.Label labelSource;
    private System.Windows.Forms.Label labelDestination;
    private System.Windows.Forms.ComboBox comboBoxSource;
    private System.Windows.Forms.ComboBox comboBoxDestination;
    private System.Windows.Forms.Button buttonGenerate;
    private System.Windows.Forms.Button buttonSaveDelta;
    private System.Windows.Forms.RichTextBox textBoxReport;
}

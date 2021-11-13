namespace IndustrialPark
{
    partial class InternalFlyEditor
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
            this.propertyGridSpecific = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonView = new System.Windows.Forms.Button();
            this.buttonFindCallers = new System.Windows.Forms.Button();
            this.buttonGetPos = new System.Windows.Forms.Button();
            this.buttonRecord = new System.Windows.Forms.Button();
            this.listBoxFlyEntries = new System.Windows.Forms.ListBox();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.labelFrame = new System.Windows.Forms.Label();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // propertyGridSpecific
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.propertyGridSpecific, 2);
            this.propertyGridSpecific.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridSpecific.HelpVisible = false;
            this.propertyGridSpecific.Location = new System.Drawing.Point(3, 160);
            this.propertyGridSpecific.Name = "propertyGridSpecific";
            this.propertyGridSpecific.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridSpecific.Size = new System.Drawing.Size(338, 174);
            this.propertyGridSpecific.TabIndex = 7;
            this.propertyGridSpecific.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridSpecific_PropertyValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonHelp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.buttonView, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonFindCallers, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.buttonGetPos, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonRecord, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridSpecific, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.listBoxFlyEntries, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonRemove, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonAdd, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonPlay, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelFrame, 0, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(344, 441);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonView
            // 
            this.buttonView.AutoSize = true;
            this.buttonView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonView.Location = new System.Drawing.Point(3, 340);
            this.buttonView.Name = "buttonView";
            this.buttonView.Size = new System.Drawing.Size(166, 22);
            this.buttonView.TabIndex = 30;
            this.buttonView.Text = "See View";
            this.buttonView.UseVisualStyleBackColor = true;
            this.buttonView.Click += new System.EventHandler(this.buttonView_Click);
            // 
            // buttonFindCallers
            // 
            this.buttonFindCallers.AutoSize = true;
            this.buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFindCallers.Location = new System.Drawing.Point(175, 396);
            this.buttonFindCallers.Name = "buttonFindCallers";
            this.buttonFindCallers.Size = new System.Drawing.Size(166, 22);
            this.buttonFindCallers.TabIndex = 28;
            this.buttonFindCallers.Text = "Find Who Targets Me";
            this.buttonFindCallers.UseVisualStyleBackColor = true;
            this.buttonFindCallers.Click += new System.EventHandler(this.buttonFindCallers_Click);
            // 
            // buttonGetPos
            // 
            this.buttonGetPos.AutoSize = true;
            this.buttonGetPos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonGetPos.Location = new System.Drawing.Point(175, 340);
            this.buttonGetPos.Name = "buttonGetPos";
            this.buttonGetPos.Size = new System.Drawing.Size(166, 22);
            this.buttonGetPos.TabIndex = 19;
            this.buttonGetPos.Text = "Get View";
            this.buttonGetPos.UseVisualStyleBackColor = true;
            this.buttonGetPos.Click += new System.EventHandler(this.buttonGetView_Click);
            // 
            // buttonRecord
            // 
            this.buttonRecord.AutoSize = true;
            this.buttonRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRecord.Location = new System.Drawing.Point(175, 368);
            this.buttonRecord.Name = "buttonRecord";
            this.buttonRecord.Size = new System.Drawing.Size(166, 22);
            this.buttonRecord.TabIndex = 18;
            this.buttonRecord.Text = "Start Recording";
            this.buttonRecord.UseVisualStyleBackColor = true;
            this.buttonRecord.Click += new System.EventHandler(this.buttonRecord_Click);
            // 
            // listBoxFlyEntries
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.listBoxFlyEntries, 2);
            this.listBoxFlyEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFlyEntries.FormattingEnabled = true;
            this.listBoxFlyEntries.Location = new System.Drawing.Point(3, 3);
            this.listBoxFlyEntries.Name = "listBoxFlyEntries";
            this.listBoxFlyEntries.Size = new System.Drawing.Size(338, 123);
            this.listBoxFlyEntries.TabIndex = 23;
            this.listBoxFlyEntries.SelectedIndexChanged += new System.EventHandler(this.listBoxFlyEntries_SelectedIndexChanged);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRemove.Location = new System.Drawing.Point(175, 132);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(166, 22);
            this.buttonRemove.TabIndex = 24;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonAdd.Location = new System.Drawing.Point(3, 132);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(166, 22);
            this.buttonAdd.TabIndex = 25;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPlay.Location = new System.Drawing.Point(3, 368);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(166, 22);
            this.buttonPlay.TabIndex = 26;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // labelFrame
            // 
            this.labelFrame.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelFrame, 2);
            this.labelFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFrame.Location = new System.Drawing.Point(3, 421);
            this.labelFrame.Name = "labelFrame";
            this.labelFrame.Size = new System.Drawing.Size(338, 20);
            this.labelFrame.TabIndex = 31;
            this.labelFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonHelp.Location = new System.Drawing.Point(3, 396);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(166, 22);
            this.buttonHelp.TabIndex = 32;
            this.buttonHelp.Text = "Open Wiki Page";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // InternalFlyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 441);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "InternalFlyEditor";
            this.ShowIcon = false;
            this.Text = "Asset Data Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InternalDynaEditor_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid propertyGridSpecific;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonGetPos;
        private System.Windows.Forms.Button buttonRecord;
        private System.Windows.Forms.ListBox listBoxFlyEntries;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonFindCallers;
        private System.Windows.Forms.Button buttonView;
        private System.Windows.Forms.Label labelFrame;
        private System.Windows.Forms.Button buttonHelp;
    }
}
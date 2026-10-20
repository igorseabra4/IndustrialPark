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
            propertyGridSpecific = new System.Windows.Forms.PropertyGrid();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            buttonHelp = new System.Windows.Forms.Button();
            buttonView = new System.Windows.Forms.Button();
            buttonFindCallers = new System.Windows.Forms.Button();
            buttonGetPos = new System.Windows.Forms.Button();
            buttonRecord = new System.Windows.Forms.Button();
            listBoxFlyEntries = new System.Windows.Forms.ListBox();
            buttonRemove = new System.Windows.Forms.Button();
            buttonAdd = new System.Windows.Forms.Button();
            buttonPlay = new System.Windows.Forms.Button();
            labelFrame = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // propertyGridSpecific
            // 
            propertyGridSpecific.BackColor = System.Drawing.SystemColors.Control;
            tableLayoutPanel1.SetColumnSpan(propertyGridSpecific, 2);
            propertyGridSpecific.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGridSpecific.HelpVisible = false;
            propertyGridSpecific.Location = new System.Drawing.Point(0, 157);
            propertyGridSpecific.Margin = new System.Windows.Forms.Padding(0);
            propertyGridSpecific.Name = "propertyGridSpecific";
            propertyGridSpecific.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridSpecific.Size = new System.Drawing.Size(344, 180);
            propertyGridSpecific.TabIndex = 7;
            propertyGridSpecific.PropertyValueChanged += propertyGridSpecific_PropertyValueChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(buttonHelp, 0, 4);
            tableLayoutPanel1.Controls.Add(buttonView, 0, 3);
            tableLayoutPanel1.Controls.Add(buttonFindCallers, 1, 5);
            tableLayoutPanel1.Controls.Add(buttonGetPos, 1, 3);
            tableLayoutPanel1.Controls.Add(buttonRecord, 0, 4);
            tableLayoutPanel1.Controls.Add(propertyGridSpecific, 0, 2);
            tableLayoutPanel1.Controls.Add(listBoxFlyEntries, 0, 0);
            tableLayoutPanel1.Controls.Add(buttonRemove, 1, 1);
            tableLayoutPanel1.Controls.Add(buttonAdd, 0, 1);
            tableLayoutPanel1.Controls.Add(buttonPlay, 0, 4);
            tableLayoutPanel1.Controls.Add(labelFrame, 0, 6);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(344, 441);
            tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonHelp
            // 
            buttonHelp.AutoSize = true;
            buttonHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonHelp.Location = new System.Drawing.Point(3, 396);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(166, 22);
            buttonHelp.TabIndex = 32;
            buttonHelp.Text = "Open Wiki Page";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // buttonView
            // 
            buttonView.AutoSize = true;
            buttonView.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonView.Location = new System.Drawing.Point(3, 340);
            buttonView.Name = "buttonView";
            buttonView.Size = new System.Drawing.Size(166, 22);
            buttonView.TabIndex = 30;
            buttonView.Text = "See View";
            buttonView.UseVisualStyleBackColor = true;
            buttonView.Click += buttonView_Click;
            // 
            // buttonFindCallers
            // 
            buttonFindCallers.AutoSize = true;
            buttonFindCallers.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonFindCallers.Location = new System.Drawing.Point(175, 396);
            buttonFindCallers.Name = "buttonFindCallers";
            buttonFindCallers.Size = new System.Drawing.Size(166, 22);
            buttonFindCallers.TabIndex = 28;
            buttonFindCallers.Text = "Find Who Targets Me";
            buttonFindCallers.UseVisualStyleBackColor = true;
            buttonFindCallers.Click += buttonFindCallers_Click;
            // 
            // buttonGetPos
            // 
            buttonGetPos.AutoSize = true;
            buttonGetPos.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonGetPos.Location = new System.Drawing.Point(175, 340);
            buttonGetPos.Name = "buttonGetPos";
            buttonGetPos.Size = new System.Drawing.Size(166, 22);
            buttonGetPos.TabIndex = 19;
            buttonGetPos.Text = "Get View";
            buttonGetPos.UseVisualStyleBackColor = true;
            buttonGetPos.Click += buttonGetView_Click;
            // 
            // buttonRecord
            // 
            buttonRecord.AutoSize = true;
            buttonRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonRecord.Location = new System.Drawing.Point(175, 368);
            buttonRecord.Name = "buttonRecord";
            buttonRecord.Size = new System.Drawing.Size(166, 22);
            buttonRecord.TabIndex = 18;
            buttonRecord.Text = "Start Recording";
            buttonRecord.UseVisualStyleBackColor = true;
            buttonRecord.Click += buttonRecord_Click;
            // 
            // listBoxFlyEntries
            // 
            listBoxFlyEntries.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tableLayoutPanel1.SetColumnSpan(listBoxFlyEntries, 2);
            listBoxFlyEntries.Dock = System.Windows.Forms.DockStyle.Fill;
            listBoxFlyEntries.FormattingEnabled = true;
            listBoxFlyEntries.Location = new System.Drawing.Point(1, 1);
            listBoxFlyEntries.Margin = new System.Windows.Forms.Padding(1);
            listBoxFlyEntries.Name = "listBoxFlyEntries";
            listBoxFlyEntries.Size = new System.Drawing.Size(342, 127);
            listBoxFlyEntries.TabIndex = 23;
            listBoxFlyEntries.SelectedIndexChanged += listBoxFlyEntries_SelectedIndexChanged;
            // 
            // buttonRemove
            // 
            buttonRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonRemove.Location = new System.Drawing.Point(175, 132);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new System.Drawing.Size(166, 22);
            buttonRemove.TabIndex = 24;
            buttonRemove.Text = "Remove";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Click += buttonRemove_Click;
            // 
            // buttonAdd
            // 
            buttonAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonAdd.Location = new System.Drawing.Point(3, 132);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(166, 22);
            buttonAdd.TabIndex = 25;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += buttonAdd_Click;
            // 
            // buttonPlay
            // 
            buttonPlay.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonPlay.Location = new System.Drawing.Point(3, 368);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new System.Drawing.Size(166, 22);
            buttonPlay.TabIndex = 26;
            buttonPlay.Text = "Play";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += buttonPlay_Click;
            // 
            // labelFrame
            // 
            labelFrame.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(labelFrame, 2);
            labelFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            labelFrame.Location = new System.Drawing.Point(3, 421);
            labelFrame.Name = "labelFrame";
            labelFrame.Size = new System.Drawing.Size(338, 20);
            labelFrame.TabIndex = 31;
            labelFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InternalFlyEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(344, 441);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            Name = "InternalFlyEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Asset Data Editor";
            FormClosing += InternalDynaEditor_FormClosing;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

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
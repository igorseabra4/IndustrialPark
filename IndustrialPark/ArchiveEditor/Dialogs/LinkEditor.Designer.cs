namespace IndustrialPark
{
    partial class LinkEditor
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
            groupBox1 = new System.Windows.Forms.GroupBox();
            listBoxLinks = new System.Windows.Forms.ListBox();
            buttonAdd = new System.Windows.Forms.Button();
            buttonRemove = new System.Windows.Forms.Button();
            buttonPaste = new System.Windows.Forms.Button();
            buttonCopy = new System.Windows.Forms.Button();
            buttonArrowDown = new System.Windows.Forms.Button();
            buttonArrowUp = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            groupBoxEventData = new System.Windows.Forms.GroupBox();
            groupBoxSourceCheckOrFlags = new System.Windows.Forms.GroupBox();
            textBoxSourceCheckOrFlags = new System.Windows.Forms.TextBox();
            enabledcheckBox = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            textBoxArgumentAsset = new System.Windows.Forms.TextBox();
            groupBox6 = new System.Windows.Forms.GroupBox();
            label6 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            checkBoxHex = new System.Windows.Forms.CheckBox();
            textBox3 = new System.Windows.Forms.TextBox();
            textBox4 = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            textBox1 = new System.Windows.Forms.TextBox();
            groupBox5 = new System.Windows.Forms.GroupBox();
            buttonTargetSelf = new System.Windows.Forms.Button();
            buttonTargetPlus1 = new System.Windows.Forms.Button();
            textBoxTargetAsset = new System.Windows.Forms.TextBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            comboSendEvent = new System.Windows.Forms.ComboBox();
            groupBoxSourceEvent = new System.Windows.Forms.GroupBox();
            comboRecieveEvent = new System.Windows.Forms.ComboBox();
            numericUpDownTime = new System.Windows.Forms.NumericUpDown();
            buttonHelp = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            groupBox1.SuspendLayout();
            groupBoxEventData.SuspendLayout();
            groupBoxSourceCheckOrFlags.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBoxSourceEvent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTime).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(listBoxLinks);
            groupBox1.Location = new System.Drawing.Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(310, 333);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Links";
            // 
            // listBoxLinks
            // 
            listBoxLinks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            listBoxLinks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxLinks.FormattingEnabled = true;
            listBoxLinks.Location = new System.Drawing.Point(6, 19);
            listBoxLinks.Name = "listBoxLinks";
            listBoxLinks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBoxLinks.Size = new System.Drawing.Size(298, 301);
            listBoxLinks.TabIndex = 1;
            listBoxLinks.SelectedIndexChanged += listBoxEvents_SelectedIndexChanged;
            // 
            // buttonAdd
            // 
            buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonAdd.Location = new System.Drawing.Point(18, 351);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new System.Drawing.Size(75, 23);
            buttonAdd.TabIndex = 2;
            buttonAdd.Text = "Add";
            buttonAdd.UseVisualStyleBackColor = true;
            buttonAdd.Click += buttonAdd_Click;
            // 
            // buttonRemove
            // 
            buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonRemove.Location = new System.Drawing.Point(99, 351);
            buttonRemove.Name = "buttonRemove";
            buttonRemove.Size = new System.Drawing.Size(75, 23);
            buttonRemove.TabIndex = 3;
            buttonRemove.Text = "Remove";
            buttonRemove.UseVisualStyleBackColor = true;
            buttonRemove.Click += buttonRemove_Click;
            // 
            // buttonPaste
            // 
            buttonPaste.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonPaste.Location = new System.Drawing.Point(99, 380);
            buttonPaste.Name = "buttonPaste";
            buttonPaste.Size = new System.Drawing.Size(75, 23);
            buttonPaste.TabIndex = 5;
            buttonPaste.Text = "Paste";
            buttonPaste.UseVisualStyleBackColor = true;
            buttonPaste.Click += buttonPaste_Click;
            // 
            // buttonCopy
            // 
            buttonCopy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonCopy.Location = new System.Drawing.Point(18, 380);
            buttonCopy.Name = "buttonCopy";
            buttonCopy.Size = new System.Drawing.Size(75, 23);
            buttonCopy.TabIndex = 4;
            buttonCopy.Text = "Copy";
            buttonCopy.UseVisualStyleBackColor = true;
            buttonCopy.Click += buttonCopy_Click;
            // 
            // buttonArrowDown
            // 
            buttonArrowDown.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonArrowDown.Location = new System.Drawing.Point(183, 380);
            buttonArrowDown.Name = "buttonArrowDown";
            buttonArrowDown.Size = new System.Drawing.Size(22, 23);
            buttonArrowDown.TabIndex = 7;
            buttonArrowDown.Text = "▼";
            buttonArrowDown.UseVisualStyleBackColor = true;
            buttonArrowDown.Click += buttonArrowDown_Click;
            // 
            // buttonArrowUp
            // 
            buttonArrowUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonArrowUp.Location = new System.Drawing.Point(183, 351);
            buttonArrowUp.Name = "buttonArrowUp";
            buttonArrowUp.Size = new System.Drawing.Size(22, 23);
            buttonArrowUp.TabIndex = 6;
            buttonArrowUp.Text = "▲";
            buttonArrowUp.UseVisualStyleBackColor = true;
            buttonArrowUp.Click += buttonArrowUp_Click;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonOK.Location = new System.Drawing.Point(336, 377);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(112, 23);
            buttonOK.TabIndex = 18;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(454, 377);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(112, 23);
            buttonCancel.TabIndex = 19;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // groupBoxEventData
            // 
            groupBoxEventData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            groupBoxEventData.Controls.Add(groupBoxSourceCheckOrFlags);
            groupBoxEventData.Controls.Add(enabledcheckBox);
            groupBoxEventData.Controls.Add(groupBox2);
            groupBoxEventData.Controls.Add(groupBox6);
            groupBoxEventData.Controls.Add(groupBox5);
            groupBoxEventData.Controls.Add(groupBox4);
            groupBoxEventData.Controls.Add(groupBoxSourceEvent);
            groupBoxEventData.Location = new System.Drawing.Point(328, 12);
            groupBoxEventData.Name = "groupBoxEventData";
            groupBoxEventData.Size = new System.Drawing.Size(244, 360);
            groupBoxEventData.TabIndex = 2;
            groupBoxEventData.TabStop = false;
            groupBoxEventData.Text = "Event Data";
            // 
            // groupBoxSourceCheckOrFlags
            // 
            groupBoxSourceCheckOrFlags.Controls.Add(textBoxSourceCheckOrFlags);
            groupBoxSourceCheckOrFlags.Location = new System.Drawing.Point(8, 231);
            groupBoxSourceCheckOrFlags.Name = "groupBoxSourceCheckOrFlags";
            groupBoxSourceCheckOrFlags.Size = new System.Drawing.Size(230, 47);
            groupBoxSourceCheckOrFlags.TabIndex = 11;
            groupBoxSourceCheckOrFlags.TabStop = false;
            groupBoxSourceCheckOrFlags.Text = "Source Check";
            toolTip1.SetToolTip(groupBoxSourceCheckOrFlags, "Link will only be activated if the sender asset matches this one. Optional.");
            // 
            // textBoxSourceCheckOrFlags
            // 
            textBoxSourceCheckOrFlags.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            textBoxSourceCheckOrFlags.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxSourceCheckOrFlags.Location = new System.Drawing.Point(6, 19);
            textBoxSourceCheckOrFlags.Name = "textBoxSourceCheckOrFlags";
            textBoxSourceCheckOrFlags.Size = new System.Drawing.Size(218, 20);
            textBoxSourceCheckOrFlags.TabIndex = 17;
            toolTip1.SetToolTip(textBoxSourceCheckOrFlags, "Link will only be activated if the sender asset matches this one. Optional.");
            textBoxSourceCheckOrFlags.TextChanged += textBoxSourceCheck_TextChanged;
            textBoxSourceCheckOrFlags.Leave += textBoxSourceCheck_Leave;
            // 
            // enabledcheckBox
            // 
            enabledcheckBox.Checked = true;
            enabledcheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            enabledcheckBox.Location = new System.Drawing.Point(9, 237);
            enabledcheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            enabledcheckBox.Name = "enabledcheckBox";
            enabledcheckBox.Size = new System.Drawing.Size(125, 33);
            enabledcheckBox.TabIndex = 11;
            enabledcheckBox.Text = "Enabled";
            toolTip1.SetToolTip(enabledcheckBox, "Defines whether this link is activated and will be executed");
            enabledcheckBox.UseVisualStyleBackColor = true;
            enabledcheckBox.Visible = false;
            enabledcheckBox.CheckedChanged += enabledcheckBox_CheckedChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(textBoxArgumentAsset);
            groupBox2.Location = new System.Drawing.Point(8, 178);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(230, 47);
            groupBox2.TabIndex = 11;
            groupBox2.TabStop = false;
            groupBox2.Text = "Argument Asset";
            toolTip1.SetToolTip(groupBox2, "Asset which will be sent as argument on this link. This varies depending on the target event. Optional on most cases.");
            // 
            // textBoxArgumentAsset
            // 
            textBoxArgumentAsset.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            textBoxArgumentAsset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxArgumentAsset.Location = new System.Drawing.Point(6, 19);
            textBoxArgumentAsset.Name = "textBoxArgumentAsset";
            textBoxArgumentAsset.Size = new System.Drawing.Size(218, 20);
            textBoxArgumentAsset.TabIndex = 16;
            toolTip1.SetToolTip(textBoxArgumentAsset, "Asset which will be sent as argument on this link. This varies depending on the target event. Optional on most cases.\r\n");
            textBoxArgumentAsset.TextChanged += textBoxArgumentAsset_TextChanged;
            textBoxArgumentAsset.Leave += textBoxArgumentAsset_Leave;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(label6);
            groupBox6.Controls.Add(label4);
            groupBox6.Controls.Add(label2);
            groupBox6.Controls.Add(label1);
            groupBox6.Controls.Add(checkBoxHex);
            groupBox6.Controls.Add(textBox3);
            groupBox6.Controls.Add(textBox4);
            groupBox6.Controls.Add(textBox2);
            groupBox6.Controls.Add(textBox1);
            groupBox6.Location = new System.Drawing.Point(8, 284);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(230, 70);
            groupBox6.TabIndex = 4;
            groupBox6.TabStop = false;
            groupBox6.Text = "Arguments";
            toolTip1.SetToolTip(groupBox6, "Arguments for link, vary depending on target event.");
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(117, 49);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(13, 13);
            label6.TabIndex = 24;
            label6.Text = "4";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(117, 22);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(13, 13);
            label4.TabIndex = 23;
            label4.Text = "2";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 48);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(13, 13);
            label2.TabIndex = 21;
            label2.Text = "3";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 22);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(13, 13);
            label1.TabIndex = 20;
            label1.Text = "1";
            // 
            // checkBoxHex
            // 
            checkBoxHex.AutoSize = true;
            checkBoxHex.Location = new System.Drawing.Point(173, 0);
            checkBoxHex.Name = "checkBoxHex";
            checkBoxHex.Size = new System.Drawing.Size(45, 17);
            checkBoxHex.TabIndex = 11;
            checkBoxHex.Text = "Hex";
            checkBoxHex.UseVisualStyleBackColor = true;
            checkBoxHex.CheckedChanged += checkBoxHex_CheckedChanged;
            // 
            // textBox3
            // 
            textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox3.Location = new System.Drawing.Point(25, 45);
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(86, 20);
            textBox3.TabIndex = 14;
            textBox3.TextChanged += textBox3_TextChanged;
            textBox3.Leave += textBox_Leave;
            // 
            // textBox4
            // 
            textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox4.Location = new System.Drawing.Point(136, 46);
            textBox4.Name = "textBox4";
            textBox4.Size = new System.Drawing.Size(85, 20);
            textBox4.TabIndex = 15;
            textBox4.TextChanged += textBox4_TextChanged;
            textBox4.Leave += textBox_Leave;
            // 
            // textBox2
            // 
            textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox2.Location = new System.Drawing.Point(136, 19);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(85, 20);
            textBox2.TabIndex = 13;
            textBox2.TextChanged += textBox2_TextChanged;
            textBox2.Leave += textBox_Leave;
            // 
            // textBox1
            // 
            textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox1.Location = new System.Drawing.Point(25, 19);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(86, 20);
            textBox1.TabIndex = 12;
            textBox1.TextChanged += textBox1_TextChanged;
            textBox1.Leave += textBox_Leave;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(buttonTargetSelf);
            groupBox5.Controls.Add(buttonTargetPlus1);
            groupBox5.Controls.Add(textBoxTargetAsset);
            groupBox5.Location = new System.Drawing.Point(8, 125);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(230, 47);
            groupBox5.TabIndex = 3;
            groupBox5.TabStop = false;
            groupBox5.Text = "Target Asset";
            toolTip1.SetToolTip(groupBox5, "Asset to which this link will be sent.");
            // 
            // buttonTargetSelf
            // 
            buttonTargetSelf.Location = new System.Drawing.Point(131, 0);
            buttonTargetSelf.Name = "buttonTargetSelf";
            buttonTargetSelf.Size = new System.Drawing.Size(42, 20);
            buttonTargetSelf.TabIndex = 11;
            buttonTargetSelf.Text = "Self";
            buttonTargetSelf.UseVisualStyleBackColor = true;
            buttonTargetSelf.Click += buttonTargetSelf_Click;
            // 
            // buttonTargetPlus1
            // 
            buttonTargetPlus1.Location = new System.Drawing.Point(179, 0);
            buttonTargetPlus1.Name = "buttonTargetPlus1";
            buttonTargetPlus1.Size = new System.Drawing.Size(42, 20);
            buttonTargetPlus1.TabIndex = 11;
            buttonTargetPlus1.Text = "+1";
            buttonTargetPlus1.UseVisualStyleBackColor = true;
            buttonTargetPlus1.Click += buttonTargetPlus1_Click;
            // 
            // textBoxTargetAsset
            // 
            textBoxTargetAsset.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            textBoxTargetAsset.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxTargetAsset.Location = new System.Drawing.Point(6, 19);
            textBoxTargetAsset.Name = "textBoxTargetAsset";
            textBoxTargetAsset.Size = new System.Drawing.Size(218, 20);
            textBoxTargetAsset.TabIndex = 10;
            toolTip1.SetToolTip(textBoxTargetAsset, "Asset to which this link will be sent.");
            textBoxTargetAsset.TextChanged += textBoxTargetAsset_TextChanged;
            textBoxTargetAsset.Leave += textBoxTargetAsset_Leave;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(comboSendEvent);
            groupBox4.Location = new System.Drawing.Point(8, 72);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(230, 47);
            groupBox4.TabIndex = 2;
            groupBox4.TabStop = false;
            groupBox4.Text = "Target Event";
            toolTip1.SetToolTip(groupBox4, "Event sent by this link.");
            // 
            // comboSendEvent
            // 
            comboSendEvent.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            comboSendEvent.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            comboSendEvent.FormattingEnabled = true;
            comboSendEvent.Location = new System.Drawing.Point(6, 19);
            comboSendEvent.Name = "comboSendEvent";
            comboSendEvent.Size = new System.Drawing.Size(218, 21);
            comboSendEvent.TabIndex = 9;
            toolTip1.SetToolTip(comboSendEvent, "Event sent by this link.");
            comboSendEvent.SelectedIndexChanged += comboSendEvent_SelectedIndexChanged;
            comboSendEvent.KeyDown += comboBox_KeyDown;
            // 
            // groupBoxSourceEvent
            // 
            groupBoxSourceEvent.Controls.Add(comboRecieveEvent);
            groupBoxSourceEvent.Controls.Add(numericUpDownTime);
            groupBoxSourceEvent.Location = new System.Drawing.Point(6, 19);
            groupBoxSourceEvent.Name = "groupBoxSourceEvent";
            groupBoxSourceEvent.Size = new System.Drawing.Size(232, 47);
            groupBoxSourceEvent.TabIndex = 1;
            groupBoxSourceEvent.TabStop = false;
            groupBoxSourceEvent.Text = "Source Event";
            toolTip1.SetToolTip(groupBoxSourceEvent, "Event which activates this link.");
            // 
            // comboRecieveEvent
            // 
            comboRecieveEvent.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            comboRecieveEvent.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            comboRecieveEvent.FormattingEnabled = true;
            comboRecieveEvent.Location = new System.Drawing.Point(6, 19);
            comboRecieveEvent.Name = "comboRecieveEvent";
            comboRecieveEvent.Size = new System.Drawing.Size(220, 21);
            comboRecieveEvent.TabIndex = 8;
            toolTip1.SetToolTip(comboRecieveEvent, "Event which activates this link.");
            comboRecieveEvent.SelectedIndexChanged += comboReceiveEvent_SelectedIndexChanged;
            comboRecieveEvent.KeyDown += comboBox_KeyDown;
            // 
            // numericUpDownTime
            // 
            numericUpDownTime.DecimalPlaces = 4;
            numericUpDownTime.Location = new System.Drawing.Point(8, 19);
            numericUpDownTime.Name = "numericUpDownTime";
            numericUpDownTime.Size = new System.Drawing.Size(218, 20);
            numericUpDownTime.TabIndex = 18;
            numericUpDownTime.Visible = false;
            numericUpDownTime.ValueChanged += numericUpDownTime_ValueChanged;
            // 
            // buttonHelp
            // 
            buttonHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonHelp.Location = new System.Drawing.Point(211, 351);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(105, 23);
            buttonHelp.TabIndex = 20;
            buttonHelp.Text = "Help!";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += button1_Click;
            // 
            // LinkEditor
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(584, 412);
            Controls.Add(buttonHelp);
            Controls.Add(groupBoxEventData);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(buttonArrowDown);
            Controls.Add(buttonArrowUp);
            Controls.Add(buttonPaste);
            Controls.Add(buttonCopy);
            Controls.Add(buttonRemove);
            Controls.Add(buttonAdd);
            Controls.Add(groupBox1);
            KeyPreview = true;
            MaximizeBox = false;
            Name = "LinkEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Link List Editor";
            groupBox1.ResumeLayout(false);
            groupBoxEventData.ResumeLayout(false);
            groupBoxSourceCheckOrFlags.ResumeLayout(false);
            groupBoxSourceCheckOrFlags.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBoxSourceEvent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDownTime).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxLinks;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonArrowDown;
        private System.Windows.Forms.Button buttonArrowUp;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxEventData;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox textBoxArgumentAsset;
        private System.Windows.Forms.TextBox textBoxSourceCheckOrFlags;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxHex;
        private System.Windows.Forms.TextBox textBoxTargetAsset;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxSourceCheckOrFlags;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonTargetPlus1;
        private System.Windows.Forms.Button buttonTargetSelf;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboSendEvent;
        private System.Windows.Forms.GroupBox groupBoxSourceEvent;
        private System.Windows.Forms.ComboBox comboRecieveEvent;
        private System.Windows.Forms.NumericUpDown numericUpDownTime;
        private System.Windows.Forms.CheckBox enabledcheckBox;
    }
}
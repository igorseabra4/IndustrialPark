namespace IndustrialPark
{
    partial class EventHack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventHack));
            this.buttonAddEvents = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelMemoryAddress = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxSourceEvent = new System.Windows.Forms.ComboBox();
            this.labelSourceEvent = new System.Windows.Forms.Label();
            this.checkBoxPrependHack = new System.Windows.Forms.CheckBox();
            this.radioButtonFloat = new System.Windows.Forms.RadioButton();
            this.radioButtonInt = new System.Windows.Forms.RadioButton();
            this.radioButtonByte = new System.Windows.Forms.RadioButton();
            this.textBoxPlayerAssetID = new System.Windows.Forms.TextBox();
            this.labelPlayerAssetID = new System.Windows.Forms.Label();
            this.labelOffset = new System.Windows.Forms.Label();
            this.textBoxNewValue = new System.Windows.Forms.TextBox();
            this.labelNewValue = new System.Windows.Forms.Label();
            this.textBoxOldValue = new System.Windows.Forms.TextBox();
            this.labelOldValue = new System.Windows.Forms.Label();
            this.textBoxMemoryAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCopyLinks = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonAddEvents
            // 
            this.buttonAddEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddEvents.Enabled = false;
            this.buttonAddEvents.Location = new System.Drawing.Point(65, 403);
            this.buttonAddEvents.Name = "buttonAddEvents";
            this.buttonAddEvents.Size = new System.Drawing.Size(140, 35);
            this.buttonAddEvents.TabIndex = 0;
            this.buttonAddEvents.Text = "Add Links";
            this.buttonAddEvents.UseVisualStyleBackColor = true;
            this.buttonAddEvents.Click += new System.EventHandler(this.buttonAddEvents_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(357, 403);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(107, 35);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelMemoryAddress
            // 
            this.labelMemoryAddress.AutoSize = true;
            this.labelMemoryAddress.Location = new System.Drawing.Point(6, 35);
            this.labelMemoryAddress.Name = "labelMemoryAddress";
            this.labelMemoryAddress.Size = new System.Drawing.Size(128, 20);
            this.labelMemoryAddress.TabIndex = 2;
            this.labelMemoryAddress.Text = "Memory Address";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxSourceEvent);
            this.groupBox1.Controls.Add(this.labelSourceEvent);
            this.groupBox1.Controls.Add(this.checkBoxPrependHack);
            this.groupBox1.Controls.Add(this.radioButtonFloat);
            this.groupBox1.Controls.Add(this.radioButtonInt);
            this.groupBox1.Controls.Add(this.radioButtonByte);
            this.groupBox1.Controls.Add(this.textBoxPlayerAssetID);
            this.groupBox1.Controls.Add(this.labelPlayerAssetID);
            this.groupBox1.Controls.Add(this.labelOffset);
            this.groupBox1.Controls.Add(this.textBoxNewValue);
            this.groupBox1.Controls.Add(this.labelNewValue);
            this.groupBox1.Controls.Add(this.textBoxOldValue);
            this.groupBox1.Controls.Add(this.labelOldValue);
            this.groupBox1.Controls.Add(this.textBoxMemoryAddress);
            this.groupBox1.Controls.Add(this.labelMemoryAddress);
            this.groupBox1.Location = new System.Drawing.Point(12, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 294);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory Modification";
            // 
            // comboBoxSourceEvent
            // 
            this.comboBoxSourceEvent.FormattingEnabled = true;
            this.comboBoxSourceEvent.Location = new System.Drawing.Point(233, 225);
            this.comboBoxSourceEvent.Name = "comboBoxSourceEvent";
            this.comboBoxSourceEvent.Size = new System.Drawing.Size(209, 28);
            this.comboBoxSourceEvent.TabIndex = 16;
            // 
            // labelSourceEvent
            // 
            this.labelSourceEvent.AutoSize = true;
            this.labelSourceEvent.Location = new System.Drawing.Point(225, 203);
            this.labelSourceEvent.Name = "labelSourceEvent";
            this.labelSourceEvent.Size = new System.Drawing.Size(105, 20);
            this.labelSourceEvent.TabIndex = 15;
            this.labelSourceEvent.Text = "Source Event";
            // 
            // checkBoxPrependHack
            // 
            this.checkBoxPrependHack.AutoSize = true;
            this.checkBoxPrependHack.Checked = true;
            this.checkBoxPrependHack.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxPrependHack.Location = new System.Drawing.Point(10, 259);
            this.checkBoxPrependHack.Name = "checkBoxPrependHack";
            this.checkBoxPrependHack.Size = new System.Drawing.Size(264, 24);
            this.checkBoxPrependHack.TabIndex = 14;
            this.checkBoxPrependHack.Text = "Prepend UpgradePowerUp hack";
            this.checkBoxPrependHack.UseVisualStyleBackColor = true;
            // 
            // radioButtonFloat
            // 
            this.radioButtonFloat.AutoSize = true;
            this.radioButtonFloat.Location = new System.Drawing.Point(268, 156);
            this.radioButtonFloat.Name = "radioButtonFloat";
            this.radioButtonFloat.Size = new System.Drawing.Size(124, 24);
            this.radioButtonFloat.TabIndex = 13;
            this.radioButtonFloat.Text = "Float (32-bit)";
            this.radioButtonFloat.UseVisualStyleBackColor = true;
            // 
            // radioButtonInt
            // 
            this.radioButtonInt.AutoSize = true;
            this.radioButtonInt.Location = new System.Drawing.Point(142, 156);
            this.radioButtonInt.Name = "radioButtonInt";
            this.radioButtonInt.Size = new System.Drawing.Size(107, 24);
            this.radioButtonInt.TabIndex = 12;
            this.radioButtonInt.Text = "Int (32-bit)";
            this.radioButtonInt.UseVisualStyleBackColor = true;
            // 
            // radioButtonByte
            // 
            this.radioButtonByte.AutoSize = true;
            this.radioButtonByte.Checked = true;
            this.radioButtonByte.Location = new System.Drawing.Point(10, 156);
            this.radioButtonByte.Name = "radioButtonByte";
            this.radioButtonByte.Size = new System.Drawing.Size(110, 24);
            this.radioButtonByte.TabIndex = 11;
            this.radioButtonByte.TabStop = true;
            this.radioButtonByte.Text = "UInt (8-bit)";
            this.radioButtonByte.UseVisualStyleBackColor = true;
            // 
            // textBoxPlayerAssetID
            // 
            this.textBoxPlayerAssetID.Location = new System.Drawing.Point(10, 226);
            this.textBoxPlayerAssetID.Name = "textBoxPlayerAssetID";
            this.textBoxPlayerAssetID.Size = new System.Drawing.Size(213, 26);
            this.textBoxPlayerAssetID.TabIndex = 10;
            // 
            // labelPlayerAssetID
            // 
            this.labelPlayerAssetID.AutoSize = true;
            this.labelPlayerAssetID.Location = new System.Drawing.Point(6, 203);
            this.labelPlayerAssetID.Name = "labelPlayerAssetID";
            this.labelPlayerAssetID.Size = new System.Drawing.Size(118, 20);
            this.labelPlayerAssetID.TabIndex = 9;
            this.labelPlayerAssetID.Text = "Player Asset ID";
            // 
            // labelOffset
            // 
            this.labelOffset.AutoSize = true;
            this.labelOffset.Location = new System.Drawing.Point(229, 61);
            this.labelOffset.Name = "labelOffset";
            this.labelOffset.Size = new System.Drawing.Size(57, 20);
            this.labelOffset.TabIndex = 8;
            this.labelOffset.Text = "Offset:";
            // 
            // textBoxNewValue
            // 
            this.textBoxNewValue.Location = new System.Drawing.Point(229, 124);
            this.textBoxNewValue.MaxLength = 100;
            this.textBoxNewValue.Name = "textBoxNewValue";
            this.textBoxNewValue.Size = new System.Drawing.Size(213, 26);
            this.textBoxNewValue.TabIndex = 7;
            this.textBoxNewValue.TextChanged += new System.EventHandler(this.textBoxNewValue_TextChanged);
            // 
            // labelNewValue
            // 
            this.labelNewValue.AutoSize = true;
            this.labelNewValue.Location = new System.Drawing.Point(225, 101);
            this.labelNewValue.Name = "labelNewValue";
            this.labelNewValue.Size = new System.Drawing.Size(85, 20);
            this.labelNewValue.TabIndex = 6;
            this.labelNewValue.Text = "New Value";
            // 
            // textBoxOldValue
            // 
            this.textBoxOldValue.Location = new System.Drawing.Point(10, 124);
            this.textBoxOldValue.MaxLength = 100;
            this.textBoxOldValue.Name = "textBoxOldValue";
            this.textBoxOldValue.Size = new System.Drawing.Size(213, 26);
            this.textBoxOldValue.TabIndex = 5;
            this.textBoxOldValue.TextChanged += new System.EventHandler(this.textBoxOldValue_TextChanged);
            // 
            // labelOldValue
            // 
            this.labelOldValue.AutoSize = true;
            this.labelOldValue.Location = new System.Drawing.Point(6, 101);
            this.labelOldValue.Name = "labelOldValue";
            this.labelOldValue.Size = new System.Drawing.Size(78, 20);
            this.labelOldValue.TabIndex = 4;
            this.labelOldValue.Text = "Old Value";
            // 
            // textBoxMemoryAddress
            // 
            this.textBoxMemoryAddress.Location = new System.Drawing.Point(10, 58);
            this.textBoxMemoryAddress.MaxLength = 10;
            this.textBoxMemoryAddress.Name = "textBoxMemoryAddress";
            this.textBoxMemoryAddress.Size = new System.Drawing.Size(213, 26);
            this.textBoxMemoryAddress.TabIndex = 3;
            this.textBoxMemoryAddress.Text = "0x80000000";
            this.textBoxMemoryAddress.TextChanged += new System.EventHandler(this.textBoxMemoryAddress_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 13);
            this.label1.MaximumSize = new System.Drawing.Size(450, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(446, 80);
            this.label1.TabIndex = 4;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // buttonCopyLinks
            // 
            this.buttonCopyLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCopyLinks.Enabled = false;
            this.buttonCopyLinks.Location = new System.Drawing.Point(211, 403);
            this.buttonCopyLinks.Name = "buttonCopyLinks";
            this.buttonCopyLinks.Size = new System.Drawing.Size(140, 35);
            this.buttonCopyLinks.TabIndex = 5;
            this.buttonCopyLinks.Text = "Copy Links";
            this.buttonCopyLinks.UseVisualStyleBackColor = true;
            this.buttonCopyLinks.Click += new System.EventHandler(this.buttonCopyLinks_Click);
            // 
            // EventHack
            // 
            this.AcceptButton = this.buttonAddEvents;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(476, 450);
            this.Controls.Add(this.buttonCopyLinks);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAddEvents);
            this.MaximizeBox = false;
            this.Name = "EventHack";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Event Hack";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAddEvents;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelMemoryAddress;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxMemoryAddress;
        private System.Windows.Forms.Label labelOldValue;
        private System.Windows.Forms.TextBox textBoxOldValue;
        private System.Windows.Forms.TextBox textBoxNewValue;
        private System.Windows.Forms.Label labelNewValue;
        private System.Windows.Forms.Label labelOffset;
        private System.Windows.Forms.Label labelPlayerAssetID;
        private System.Windows.Forms.TextBox textBoxPlayerAssetID;
        private System.Windows.Forms.RadioButton radioButtonByte;
        private System.Windows.Forms.RadioButton radioButtonInt;
        private System.Windows.Forms.RadioButton radioButtonFloat;
        private System.Windows.Forms.CheckBox checkBoxPrependHack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSourceEvent;
        private System.Windows.Forms.ComboBox comboBoxSourceEvent;
        private System.Windows.Forms.Button buttonCopyLinks;
    }
}
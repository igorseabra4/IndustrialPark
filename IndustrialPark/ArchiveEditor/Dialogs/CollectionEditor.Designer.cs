namespace IndustrialPark
{
    partial class CollectionEditor
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
            listBoxItems = new System.Windows.Forms.ListBox();
            buttonAdd = new System.Windows.Forms.Button();
            buttonRemove = new System.Windows.Forms.Button();
            buttonPaste = new System.Windows.Forms.Button();
            buttonCopy = new System.Windows.Forms.Button();
            buttonArrowDown = new System.Windows.Forms.Button();
            buttonArrowUp = new System.Windows.Forms.Button();
            buttonOK = new System.Windows.Forms.Button();
            buttonCancel = new System.Windows.Forms.Button();
            buttonHelp = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            propertyGridItem = new System.Windows.Forms.PropertyGrid();
            SuspendLayout();
            // 
            // listBoxItems
            // 
            listBoxItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            listBoxItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            listBoxItems.FormattingEnabled = true;
            listBoxItems.Location = new System.Drawing.Point(12, 12);
            listBoxItems.Name = "listBoxItems";
            listBoxItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            listBoxItems.Size = new System.Drawing.Size(187, 314);
            listBoxItems.TabIndex = 1;
            listBoxItems.SelectedIndexChanged += listBoxItems_SelectedIndexChanged;
            // 
            // buttonAdd
            // 
            buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonAdd.Location = new System.Drawing.Point(12, 332);
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
            buttonRemove.Location = new System.Drawing.Point(93, 332);
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
            buttonPaste.Location = new System.Drawing.Point(93, 361);
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
            buttonCopy.Location = new System.Drawing.Point(12, 361);
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
            buttonArrowDown.Location = new System.Drawing.Point(174, 361);
            buttonArrowDown.Name = "buttonArrowDown";
            buttonArrowDown.Size = new System.Drawing.Size(25, 23);
            buttonArrowDown.TabIndex = 7;
            buttonArrowDown.Text = "▼";
            buttonArrowDown.UseVisualStyleBackColor = true;
            buttonArrowDown.Click += buttonArrowDown_Click;
            // 
            // buttonArrowUp
            // 
            buttonArrowUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonArrowUp.Location = new System.Drawing.Point(174, 332);
            buttonArrowUp.Name = "buttonArrowUp";
            buttonArrowUp.Size = new System.Drawing.Size(25, 23);
            buttonArrowUp.TabIndex = 6;
            buttonArrowUp.Text = "▲";
            buttonArrowUp.UseVisualStyleBackColor = true;
            buttonArrowUp.Click += buttonArrowUp_Click;
            // 
            // buttonOK
            // 
            buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonOK.Location = new System.Drawing.Point(470, 361);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new System.Drawing.Size(68, 23);
            buttonOK.TabIndex = 10;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            buttonCancel.Location = new System.Drawing.Point(544, 361);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new System.Drawing.Size(68, 23);
            buttonCancel.TabIndex = 11;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // buttonHelp
            // 
            buttonHelp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonHelp.Location = new System.Drawing.Point(396, 361);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(68, 23);
            buttonHelp.TabIndex = 9;
            buttonHelp.Text = "Help";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // propertyGridItem
            // 
            propertyGridItem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            propertyGridItem.BackColor = System.Drawing.SystemColors.Control;
            propertyGridItem.Location = new System.Drawing.Point(205, 12);
            propertyGridItem.Name = "propertyGridItem";
            propertyGridItem.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            propertyGridItem.Size = new System.Drawing.Size(407, 343);
            propertyGridItem.TabIndex = 8;
            propertyGridItem.PropertyValueChanged += propertyGridItem_PropertyValueChanged;
            // 
            // CollectionEditor
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new System.Drawing.Size(624, 396);
            Controls.Add(listBoxItems);
            Controls.Add(propertyGridItem);
            Controls.Add(buttonHelp);
            Controls.Add(buttonOK);
            Controls.Add(buttonCancel);
            Controls.Add(buttonArrowDown);
            Controls.Add(buttonArrowUp);
            Controls.Add(buttonPaste);
            Controls.Add(buttonCopy);
            Controls.Add(buttonRemove);
            Controls.Add(buttonAdd);
            KeyPreview = true;
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(525, 435);
            Name = "CollectionEditor";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Collection Editor";
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListBox listBoxItems;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonPaste;
        private System.Windows.Forms.Button buttonCopy;
        private System.Windows.Forms.Button buttonArrowDown;
        private System.Windows.Forms.Button buttonArrowUp;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PropertyGrid propertyGridItem;
    }
}
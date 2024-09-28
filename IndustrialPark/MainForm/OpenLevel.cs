using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class OpenLevel : Form
    {
        public OpenLevel()
        {
            InitializeComponent();
        }

        private string _getLocalizationImageKeyFromFilename(string filename)
        {
            if (filename.Length < 2)
            {
                return "";
            }

            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

            return filenameWithoutExtension.Substring(filenameWithoutExtension.Length - 2) + ".png";
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            // Check for duplicates and warn as necessary
            List<string> files = new List<string>();
            files.Add(txtHIP.Text);
            files.Add(txtHOP.Text);
            files.Add(txtBOOT.Text);
            foreach (var item in lvwLocalization.CheckedItems)
            {
                files.Add(((ListViewItem)item).Text);
            }

            if (files.Count != files.Distinct().Count())
            {
                if (MessageBox.Show("Are you sure you want to open duplicate files?",
                    "Duplicate files detected",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.No)
                {
                    return;
                }
            }

            bool importSuccessful = true;

            if (chkHIP.Checked && txtHIP.TextLength > 0)
            {
                try
                {
                    Program.MainForm.AddArchiveEditor(txtHIP.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HIP could not be opened: " + ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            Program.MainForm.SetCloseAllArchivesEnabled(importSuccessful);

            if (chkHOP.Checked && txtHOP.TextLength > 0)
            {
                try
                {
                    Program.MainForm.AddArchiveEditor(txtHOP.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HOP could not be opened: " + ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            if (chkBOOT.Checked && txtBOOT.TextLength > 0)
            {
                try
                {
                    Program.MainForm.AddArchiveEditor(txtBOOT.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("BOOT.HIP could not be opened: " + ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            if (lvwLocalization.CheckedItems.Count > 0)
            {
                foreach (var item in lvwLocalization.CheckedItems)
                {
                    try
                    {
                        Program.MainForm.AddArchiveEditor(((ListViewItem)item).Text);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Localization file could not be opened: " + ex.Message, "Open failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        importSuccessful = false;
                    }
                }
            }

            if (importSuccessful)
            {
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _findCorrespondingFiles(string filePath, bool findHIP, bool findHOP)
        {
            // Try to find HIP
            if (findHIP && File.Exists(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".HIP"))
            {
                txtHIP.Text = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".HIP";
                chkHIP.Checked = true;

                txtHIP.SelectionStart = txtHIP.Text.Length;
                txtHIP.SelectionLength = 0;
            }

            // Try to find HOP
            if (findHOP && File.Exists(Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".HOP"))
            {
                txtHOP.Text = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileNameWithoutExtension(filePath) + ".HOP";
                chkHOP.Checked = true;

                txtHOP.SelectionStart = txtHOP.Text.Length;
                txtHOP.SelectionLength = 0;
            }

            if (File.Exists(Directory.GetParent(Directory.GetParent(filePath).ToString()) + "\\" + "BOOT.HIP"))
            {
                txtBOOT.Text = Directory.GetParent(Directory.GetParent(filePath).ToString()) + "\\" + "BOOT.HIP";
                chkBOOT.Checked = true;

                txtBOOT.SelectionStart = txtBOOT.Text.Length;
                txtBOOT.SelectionLength = 0;
            }

            string pattern = @"name_[a-z]{2}.h[io]p".Replace("name", Path.GetFileNameWithoutExtension(filePath));
            RegexOptions options = RegexOptions.IgnoreCase;
            Regex regex = new Regex(pattern, options);

            // Find localization files
            foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(filePath)))
            {
                if (regex.IsMatch(fileName) && !fileName.EndsWith(".txt"))
                {
                    var item = new ListViewItem()
                    {
                        Text = fileName,
                        Checked = true,
                        ImageKey = _getLocalizationImageKeyFromFilename(fileName)
                    };
                    lvwLocalization.Items.Add(item);
                }
            }

            checkBoxUpdated(null, null);
        }

        private void btnHIPSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "HIP Files|*.hip";
            dialog.Title = "Please choose a HIP file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtHIP.Text = dialog.FileName;
                chkHIP.Checked = true;
                Text = $"Open Level - {Path.GetFileNameWithoutExtension(dialog.FileName)}";
                txtHIP.SelectionStart = txtHIP.Text.Length;
                txtHIP.SelectionLength = 0;

                _findCorrespondingFiles(dialog.FileName, false, true);
            }
        }

        private void btnHOPSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "HOP Files|*.hop";
            dialog.Title = "Please choose a HOP file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtHOP.Text = dialog.FileName;
                chkHOP.Checked = true;
                Text = $"Open Level - {Path.GetFileNameWithoutExtension(dialog.FileName)}";
                txtHOP.SelectionStart = txtHOP.Text.Length;
                txtHOP.SelectionLength = 0;

                _findCorrespondingFiles(dialog.FileName, true, false);
            }
        }

        private void btnBOOTSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "BOOT HIP Files|BOOT.hip";
            dialog.Title = "Please choose a BOOT.HIP file";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtBOOT.Text = dialog.FileName;
                chkBOOT.Checked = true;
                txtBOOT.SelectionStart = txtBOOT.Text.Length;
                txtBOOT.SelectionLength = 0;
            }
        }

        private void checkBoxUpdated(object sender, EventArgs e)
        {
            btnImport.Enabled = chkHIP.Checked && txtHIP.Text.Length > 0
                || chkHOP.Checked && txtHOP.Text.Length > 0
                || chkBOOT.Checked && txtBOOT.Text.Length > 0
                || lvwLocalization.CheckedItems.Count > 0;
        }

        private void checkBoxUpdatedInList(object sender, ItemCheckEventArgs e)
        {
            // This is different to the above event handler since the CheckedListBox's
            // ItemCheck event is fired before the state is changed, instead of
            // after, like the regular checkboxes.
            btnImport.Enabled = chkHIP.Checked && txtHIP.Text.Length > 0
                || chkHOP.Checked && txtHOP.Text.Length > 0
                || chkBOOT.Checked && txtBOOT.Text.Length > 0
                || (e.NewValue == CheckState.Checked)
                || (e.NewValue == CheckState.Unchecked && lvwLocalization.CheckedItems.Count > 1);
        }

        private void btnAddLocalizationFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Localization HIP files|*.hip";
            dialog.Title = "Please choose one or more localization HIP files";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    var item = new ListViewItem()
                    {
                        Text = file,
                        Checked = true,
                        ImageKey = _getLocalizationImageKeyFromFilename(file)
                    };
                    lvwLocalization.Items.Add(item);
                }
            }
        }

        private void copyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lvwLocalization.Items[lvwLocalization.SelectedIndices[0]].Text);
        }

        private void pastePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipboardText = Clipboard.GetText();

            var item = new ListViewItem()
            {
                Text = clipboardText,
                Checked = true,
                ImageKey = _getLocalizationImageKeyFromFilename(clipboardText)
            };
            lvwLocalization.Items.Add(item);
        }

        private void ctxLocalization_Opening(object sender, CancelEventArgs e)
        {
            copyPathToolStripMenuItem.Enabled = lvwLocalization.SelectedIndices.Count > 0
                                                && lvwLocalization.Items.Count > 0;
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            txtHOP.Text = txtHIP.Text = txtBOOT.Text = string.Empty;
            lvwLocalization.Items.Clear();
            checkBoxUpdated(sender, e);
            Text = "Open Level";
        }

        private void lvwLocalization_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lvwLocalization.SelectedIndices.Count > 0)
            {
                lvwLocalization.Items.RemoveAt(lvwLocalization.SelectedIndices[0]);
            }
        }
    }
}

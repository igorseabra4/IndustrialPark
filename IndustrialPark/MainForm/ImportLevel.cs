using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ImportLevel : Form
    {
        public ImportLevel()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            // Check for duplicates and warn as necessary
            List<string> files = new List<string>();
            files.Add(txtHIP.Text);
            files.Add(txtHOP.Text);
            files.Add(txtBOOT.Text);
            foreach (var item in lstLocalization.CheckedItems)
            {
                files.Add(item.ToString());
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
                    ArchiveEditor temp = new ArchiveEditor();
                    //temp.Show();
                    //temp.Hide();
                    temp.Begin(txtHIP.Text, HipHopFile.Platform.Unknown);
                    Program.MainForm.archiveEditors.Add(temp);
                    temp.archive.ChangesMade += Program.MainForm.UpdateTitleBar;
                    temp.EditorClosed += Program.MainForm.UpdateCloseAllArchiveMenuItem;
                    Program.MainForm.AddArchiveDropdownListEntry(Path.GetFileName(temp.GetCurrentlyOpenFileName()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HIP could not be imported.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            Program.MainForm.SetCloseAllArchivesEnabled(importSuccessful);

            if (chkHOP.Checked && txtHOP.TextLength > 0)
            {
                try
                {
                    ArchiveEditor temp = new ArchiveEditor();
                    //temp.Show();
                    //temp.Hide();
                    temp.Begin(txtHOP.Text, HipHopFile.Platform.Unknown);
                    Program.MainForm.archiveEditors.Add(temp);
                    temp.archive.ChangesMade += Program.MainForm.UpdateTitleBar;
                    temp.EditorClosed += Program.MainForm.UpdateCloseAllArchiveMenuItem;
                    Program.MainForm.AddArchiveDropdownListEntry(Path.GetFileName(temp.GetCurrentlyOpenFileName()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("HOP could not be imported.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            if (chkBOOT.Checked && txtBOOT.TextLength > 0)
            {
                try
                {
                    ArchiveEditor temp = new ArchiveEditor();
                    //temp.Show();
                    //temp.Hide();
                    temp.Begin(txtBOOT.Text, HipHopFile.Platform.Unknown);
                    Program.MainForm.archiveEditors.Add(temp);
                    temp.archive.ChangesMade += Program.MainForm.UpdateTitleBar;
                    temp.EditorClosed += Program.MainForm.UpdateCloseAllArchiveMenuItem;
                    Program.MainForm.AddArchiveDropdownListEntry(Path.GetFileName(temp.GetCurrentlyOpenFileName()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("BOOT.HIP could not be imported.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importSuccessful = false;
                }
            }

            if (lstLocalization.CheckedItems.Count > 0)
            {
                foreach (var item in lstLocalization.CheckedItems)
                {
                    try
                    {
                        ArchiveEditor temp = new ArchiveEditor();
                        //temp.Show();
                        //temp.Hide();
                        temp.Begin(item.ToString(), HipHopFile.Platform.Unknown);
                        Program.MainForm.archiveEditors.Add(temp);
                        temp.archive.ChangesMade += Program.MainForm.UpdateTitleBar;
                        temp.EditorClosed += Program.MainForm.UpdateCloseAllArchiveMenuItem;
                        Program.MainForm.AddArchiveDropdownListEntry(Path.GetFileName(temp.GetCurrentlyOpenFileName()));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Localization file could not be imported.", "Import failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        importSuccessful = false;
                    }
                }
            }
            

            Program.MainForm.UpdateTitleBar();
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
                if (regex.IsMatch(fileName))
                {
                    lstLocalization.Items.Add(fileName, true);
                }
            }
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
                Text = $"Import Level - {Path.GetFileNameWithoutExtension(dialog.FileName)}";
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
                Text = $"Import Level - {Path.GetFileNameWithoutExtension(dialog.FileName)}";
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
                || lstLocalization.CheckedItems.Count > 0;
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
                || (e.NewValue == CheckState.Unchecked && lstLocalization.CheckedItems.Count > 1);
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
                    lstLocalization.Items.Add(file, true);
                }
            }
        }

        private void lstLocalization_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstLocalization.SelectedIndex >= 0)
            {
                lstLocalization.Items.RemoveAt(lstLocalization.SelectedIndex);
            }
        }

        private void copyPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lstLocalization.Items[lstLocalization.SelectedIndex].ToString());
        }

        private void pastePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstLocalization.Items.Add(Clipboard.GetText(), true);
        }

        private void ctxLocalization_Opening(object sender, CancelEventArgs e)
        {
            copyPathToolStripMenuItem.Enabled = lstLocalization.SelectedIndex >= 0 && lstLocalization.Items.Count > 0;
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            txtHOP.Text = txtHIP.Text = txtBOOT.Text = string.Empty;
            lstLocalization.Items.Clear();
            checkBoxUpdated(sender, e);
        }
    }
}

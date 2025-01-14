using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class UserTemplateManager : Form
    {
        public UserTemplateManager()
        {
            InitializeComponent();
            TopMost = true;
            UpdateListBox();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
            Hide();
        }

        private void UpdateListBox()
        {
            listBoxTemplates.Items.Clear();

            foreach (string s in Directory.GetFiles(Program.MainForm.userTemplatesFolder))
                listBoxTemplates.Items.Add(Path.GetFileName(s));

            Program.MainForm.UpdateUserTemplateComboBox();
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((listBoxTemplates.SelectedItem != null) && (e.KeyCode == Keys.Delete))
                buttonDelete_Click(sender, e);
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedIndex < 0)
                return;

            Clipboard.SetText(File.ReadAllText(Path.Combine(Program.MainForm.userTemplatesFolder, listBoxTemplates.SelectedItem.ToString())));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            AssetClipboard clipboard;

            try
            {
                clipboard = JsonConvert.DeserializeObject<AssetClipboard>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting objects from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            string templateName = "";
            foreach (var AHDR in clipboard.assets)
                templateName += "[" + AHDR.assetType.ToString() + "][" + AHDR.assetName + "]";

            File.WriteAllText(
                Path.Combine(Program.MainForm.userTemplatesFolder, templateName),
                JsonConvert.SerializeObject(clipboard, Formatting.None));

            UpdateListBox();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            File.Delete(Path.Combine(Program.MainForm.userTemplatesFolder, Path.GetFileName(listBoxTemplates.SelectedItem.ToString())));
            UpdateListBox();
        }

        private void listBoxTemplates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            buttonRename_Click(null, null);
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedIndex < 0)
                return;

            string newName = EditName.GetName(listBoxTemplates.SelectedItem.ToString(), "Template Name", out bool okED);

            if (okED)
            {
                File.Move(
                    Path.Combine(Program.MainForm.userTemplatesFolder, listBoxTemplates.SelectedItem.ToString()),
                    Path.Combine(Program.MainForm.userTemplatesFolder, newName));

                UpdateListBox();
            }
        }
    }
}

using HipHopFile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private void buttonImportRawData_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Multiselect = true };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in openFileDialog.FileNames)
                    File.Copy(s, Path.Combine(Program.MainForm.userTemplatesFolder, Path.GetFileName(s)));

                UpdateListBox();
            }
        }

        private void UpdateListBox()
        {
            listBoxTemplates.Items.Clear();

            foreach (string s in Directory.GetFiles(Program.MainForm.userTemplatesFolder))
                listBoxTemplates.Items.Add(Path.GetFileName(s));
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                File.Delete(Path.Combine(Program.MainForm.userTemplatesFolder, Path.GetFileName(listBoxTemplates.SelectedItem.ToString())));
                UpdateListBox();
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (listBoxTemplates.SelectedIndex < 0) return;

            // Generating Asset AHDR
            string assetName = listBoxTemplates.SelectedItem.ToString().Substring(listBoxTemplates.SelectedItem.ToString().IndexOf(']') + 2);

            string assetTypeName = listBoxTemplates.SelectedItem.ToString().Substring(listBoxTemplates.SelectedItem.ToString().IndexOf('[') + 1, listBoxTemplates.SelectedItem.ToString().IndexOf(']') - listBoxTemplates.SelectedItem.ToString().IndexOf('[') - 1);
            AssetType assetType = AssetType.Null;
            
            foreach (AssetType o in Enum.GetValues(typeof(AssetType)))
            {
                if (o.ToString() == assetTypeName.Trim().ToUpper())
                {
                    assetType = o;
                    break;
                }
            }
            if (assetType == AssetType.Null) throw new Exception("Unknown asset type: " + assetType);

            Section_AHDR newAsset = new Section_AHDR
            {
                assetID = Functions.BKDRHash(assetName),
                assetType = assetType,
                flags = ArchiveEditorFunctions.AHDRFlagsFromAssetType(assetType),
                data = File.ReadAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, listBoxTemplates.SelectedItem.ToString()))
            };

            newAsset.ADBG = new Section_ADBG(0, assetName, "", 0);
            
            // Asset AHDR is done
            Clipboard.SetText(JsonConvert.SerializeObject(new List<Section_AHDR>
            {
               newAsset
            }));
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            List<Section_AHDR> AHDRs;
            
            try
            {
                AHDRs = JsonConvert.DeserializeObject<List<Section_AHDR>>(Clipboard.GetText());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error pasting objects from clipboard: " + ex.Message + ". Are you sure you have assets copied?");
                return;
            }

            foreach (Section_AHDR AHDR in AHDRs)
            {
                string templateName = "[" + AHDR.assetType.ToString() + "] " + AHDR.ADBG.assetName;
                File.WriteAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, templateName), AHDR.data);
            }

            UpdateListBox();
        }

        private void listBoxTemplates_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string oldName = listBoxTemplates.SelectedItem.ToString();
            byte[] oldData = File.ReadAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, oldName));

            string newName = EditName.GetName(oldName);

            File.Delete(Path.Combine(Program.MainForm.userTemplatesFolder, oldName));
            File.WriteAllBytes(Path.Combine(Program.MainForm.userTemplatesFolder, newName), oldData);

            UpdateListBox();
        }
    }
}

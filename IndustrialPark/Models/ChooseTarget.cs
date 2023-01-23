using Assimp;
using System;
using System.Windows.Forms;

namespace IndustrialPark.Models
{
    public partial class ChooseTarget : Form
    {
        public ChooseTarget()
        {
            InitializeComponent();
            TopMost = true;
            formats = new AssimpContext().GetSupportedExportFormats();
        }

        ExportFormatDescription[] formats;

        private void ChooseTarget_Load(object sender, EventArgs e)
        {
            comboBoxFormat.Items.Add("RenderWare Stream (raw BSP/DFF)");
            foreach (ExportFormatDescription f in formats)
                comboBoxFormat.Items.Add(f.Description);

            comboBoxFormat.SelectedIndex = 0;
        }

        public static ExportFormatDescription GetExportFormat(string description)
        {
            ExportFormatDescription[] formats = new AssimpContext().GetSupportedExportFormats();

            foreach (ExportFormatDescription f in formats)
                if (f.Description.ToLower().Equals(description.ToLower()))
                    return f;

            return null;
        }

        public static (bool ok, ExportFormatDescription format, string textureExtension) GetTarget()
        {
            ChooseTarget c = new ChooseTarget();
            DialogResult d = c.ShowDialog();

            if (c.OKed || d == DialogResult.OK)
                return (true, c.comboBoxFormat.SelectedIndex == 0 ? null : c.formats[c.comboBoxFormat.SelectedIndex - 1], "." + c.textBoxExtension.Text);
            return (false, null, null);
        }

        bool OKed = false;
        private void ButtonOK_Click(object sender, EventArgs e)
        {
            OKed = true;
            Close();
        }

        private void ComboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxFormat.SelectedIndex == 0)
                groupBox2.Enabled = false;
            else
                groupBox2.Enabled = true;
        }
    }
}

using System;
using System.Windows.Forms;

namespace IndustrialPark.SaveFile
{
    public partial class AddSectionDialog : Form
    {
        public AddSectionDialog()
        {
            InitializeComponent();

            foreach (SaveFileSection s in Enum.GetValues(typeof(SaveFileSection)))
                comboBox1.Items.Add(s);
        }

        public SaveFileSection section;
        public bool OKed;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            section = (SaveFileSection)comboBox1.SelectedItem;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            OKed = true;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OKed = false;
            Close();
        }
    }
}

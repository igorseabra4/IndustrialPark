using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class EditName : Form
    {
        public EditName(string oldName, string windowText)
        {
            InitializeComponent();
            TopMost = true;
            Text = windowText;
            textBox1.Text = oldName;
        }

        public static string GetName(string oldName, string windowText, out bool OKed)
        {
            EditName edit = new EditName(oldName, windowText);
            edit.ShowDialog();

            OKed = edit.OKed;

            if (edit.OKed)
                return edit.textBox1.Text;
            return oldName;
        }

        private bool OKed = false;

        private void button1_Click(object sender, EventArgs e)
        {
            OKed = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

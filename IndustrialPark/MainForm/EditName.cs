using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class EditName : Form
    {
        public EditName(string oldName)
        {
            InitializeComponent();
            TopMost = true;
            textBox1.Text = oldName;
        }

        public static string GetName(string oldName)
        {
            EditName edit = new EditName(oldName);
            edit.ShowDialog();
                        
            if (edit.OKed)
            {
                return edit.textBox1.Text;
            }
            else
            {
                return oldName;
            }
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

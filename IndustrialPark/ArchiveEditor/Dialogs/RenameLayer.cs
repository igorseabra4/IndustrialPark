using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class RenameLayer : Form
    {
        public RenameLayer(string prevName)
        {
            InitializeComponent();

            TopMost = true;

            textBox1.Text = prevName ?? "";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string LayerName { get; private set; }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LayerName = textBox1.Text;
        }
    }
}

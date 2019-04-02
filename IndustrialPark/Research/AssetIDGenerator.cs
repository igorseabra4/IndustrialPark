using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AssetIDGenerator : Form
    {
        public AssetIDGenerator()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            foreach (string s in richTextBox1.Lines)
            {
                richTextBox2.Text += HipHopFile.Functions.BKDRHash(s).ToString("X8") + '\r'+'\n';
            }
        }
    }
}

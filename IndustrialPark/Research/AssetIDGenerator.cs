using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AssetIDGenerator : Form
    {
        public AssetIDGenerator()
        {
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

            e.Cancel = true;
            Hide();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.Clear();
            foreach (string s in richTextBox1.Lines)
            {
                richTextBox2.Text += HipHopFile.Functions.BKDRHash(s).ToString("X8") + '\r' + '\n';
            }
        }
    }
}

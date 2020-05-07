using HipHopFile;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ScrollableMessageBox : Form
    {
        public ScrollableMessageBox(string title, string text)
        {
            InitializeComponent();

            Text = title;
            richTextBox1.Text = text;

            TopMost = true;
        }
    }
}

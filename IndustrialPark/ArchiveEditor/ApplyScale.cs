using SharpDX;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ApplyScale : Form
    {
        public ApplyScale()
        {
            InitializeComponent();
            TopMost = true;

            numericUpDownX.Minimum = decimal.MinValue;
            numericUpDownY.Minimum = decimal.MinValue;
            numericUpDownZ.Minimum = decimal.MinValue;
            numericUpDownX.Maximum = decimal.MaxValue;
            numericUpDownY.Maximum = decimal.MaxValue;
            numericUpDownZ.Maximum = decimal.MaxValue;
        }

        public static Vector3 GetName(out bool OKed)
        {
            ApplyScale edit = new ApplyScale();
            edit.ShowDialog();

            OKed = edit.OKed;
            return new Vector3((float)edit.numericUpDownX.Value, (float)edit.numericUpDownY.Value, (float)edit.numericUpDownZ.Value);
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

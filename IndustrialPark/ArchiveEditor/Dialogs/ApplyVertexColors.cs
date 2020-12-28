using SharpDX;
using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ApplyVertexColors : Form
    {
        public ApplyVertexColors()
        {
            InitializeComponent();
            TopMost = true;

            numericUpDownX.Minimum = decimal.MinValue;
            numericUpDownY.Minimum = decimal.MinValue;
            numericUpDownZ.Minimum = decimal.MinValue;
            numericUpDownW.Minimum = decimal.MinValue;

            numericUpDownX.Maximum = decimal.MaxValue;
            numericUpDownY.Maximum = decimal.MaxValue;
            numericUpDownZ.Maximum = decimal.MaxValue;
            numericUpDownW.Maximum = decimal.MaxValue;

            foreach (var v in Enum.GetValues(typeof(Operation)))
                comboBoxOperation.Items.Add(v);

            comboBoxOperation.SelectedIndex = 0;
        }

        public static (Vector4?, Operation) GetColor()
        {
            ApplyVertexColors edit = new ApplyVertexColors();
            edit.ShowDialog();

            if (edit.OK)
                return (new Vector4((float)edit.numericUpDownX.Value, (float)edit.numericUpDownY.Value, (float)edit.numericUpDownZ.Value, (float)edit.numericUpDownW.Value),
                    (Operation)edit.comboBoxOperation.SelectedItem);
            return (null, 0);
        }

        private bool OK = false;

        private void button1_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

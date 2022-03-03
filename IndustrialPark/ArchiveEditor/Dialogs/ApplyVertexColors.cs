using SharpDX;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace IndustrialPark
{
    public partial class ApplyVertexColors : Form
    {
        SolidBrush colorPreview;
        readonly Graphics previewGraphics;

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

            colorPreview = new SolidBrush(System.Drawing.Color.FromArgb(
                255,
                ClampToByteRange((int)(numericUpDownX.Value * 255)),
                ClampToByteRange((int)(numericUpDownY.Value * 255)),
                ClampToByteRange((int)(numericUpDownZ.Value * 255))));

            previewGraphics = CreateGraphics();
            UpdateColorPreview();
        }

        private int ClampToByteRange(int num)
        {
            return Math.Min(Math.Max(num, 0), 255);
        }

        private void UpdateColorPreview()
        {
            colorPreview.Color = System.Drawing.Color.FromArgb(
                255,
                ClampToByteRange((int)(numericUpDownX.Value * 255)),
                ClampToByteRange((int)(numericUpDownY.Value * 255)),
                ClampToByteRange((int)(numericUpDownZ.Value * 255)));

            previewGraphics.FillRectangle(colorPreview, new System.Drawing.Rectangle(380, 15, 50, 50));
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

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateColorPreview();
        }

        private void ApplyVertexColors_Shown(object sender, EventArgs e)
        {
            UpdateColorPreview();
        }

        private void ApplyVertexColors_FormClosing(object sender, FormClosingEventArgs e)
        {
            colorPreview.Dispose();
            previewGraphics.Dispose();
        }
    }
}

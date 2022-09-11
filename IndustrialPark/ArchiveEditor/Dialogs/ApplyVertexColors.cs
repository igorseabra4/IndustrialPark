using SharpDX;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace IndustrialPark
{
    public partial class ApplyVertexColors : Form
    {
        SolidBrush colorPreview;

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

            comboBoxOperation.Items.AddRange(Enum.GetValues(typeof(Operation)).Cast<object>().Take(9).ToArray());

            comboBoxOperation.SelectedIndex = 0;

            colorPreview = new SolidBrush(System.Drawing.Color.FromArgb(
                255,
                ClampToByteRange((int)(numericUpDownX.Value * 255)),
                ClampToByteRange((int)(numericUpDownY.Value * 255)),
                ClampToByteRange((int)(numericUpDownZ.Value * 255))));

            UpdateColorPreview();
        }

        private int ClampToByteRange(int num)
        {
            return Math.Min(Math.Max(num, 0), 255);
        }

        private void UpdateColorPreview()
        {
            // Get color from control values
            colorPreview.Color = System.Drawing.Color.FromArgb(
                255,
                ClampToByteRange((int)(numericUpDownX.Value * 255)),
                ClampToByteRange((int)(numericUpDownY.Value * 255)),
                ClampToByteRange((int)(numericUpDownZ.Value * 255)));

            // TODO: Fix scaling on non-100% display scale for the color preview

            pnlColorPreview.BackColor = colorPreview.Color;
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
        }

        private void colorPickerBtn_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog()
            {
                FullOpen = true,
                ShowHelp = true,
                Color = System.Drawing.Color.FromArgb(
                    255,
                    ClampToByteRange((int)(numericUpDownX.Value * 255)),
                    ClampToByteRange((int)(numericUpDownY.Value * 255)),
                    ClampToByteRange((int)(numericUpDownZ.Value * 255))
                )
            };

            // Update the text box color if the user clicks OK 
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                numericUpDownX.Value = (decimal)colorDialog.Color.R / 255;
                numericUpDownY.Value = (decimal)colorDialog.Color.G / 255;
                numericUpDownZ.Value = (decimal)colorDialog.Color.B / 255;
            }
        }
    }
}

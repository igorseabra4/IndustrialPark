using System;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ProgressBar : Form
    {
        public ProgressBar(string text)
        {
            InitializeComponent();
            Text = text;
            TopMost = true;
        }

        public void SetProgressBar(int min, int max, int step)
        {
            if (InvokeRequired)
                Invoke(new Action<int, int, int>(SetProgressBar), min, max, step);
            else
            {
                pBar.Minimum = min;
                pBar.Maximum = max;
                pBar.Step = step;
                pBar.Value = 0;
            }
        }

        public void PerformStep(string label = "")
        {
            if (InvokeRequired)
                Invoke(new Action<string>(PerformStep), label);
            else
            {
                pBar.PerformStep();
                labelLoading.Text = $"Progress: {100 * pBar.Value / pBar.Maximum}% {label}";
                if (pBar.Value >= pBar.Maximum)
                    Close();
            }
            Update();
        }
    }
}

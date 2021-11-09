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
                Invoke(new SetValue(SetProgressBar), min, max, step);
            else
            {
                pBar.Minimum = min;
                pBar.Maximum = max;
                pBar.Step = step;
                pBar.Value = 0;
            }
        }

        private delegate void SetValue(int min, int max, int step);

        public void PerformStep()
        {
            if (InvokeRequired)
                Invoke(new Action(PerformStep));
            else
            {
                pBar.PerformStep();
                labelLoading.Text = $"Progress: {100 * pBar.Value / pBar.Maximum}%";
                if (pBar.Value >= pBar.Maximum)
                    Close();
            }
        }
    }
}

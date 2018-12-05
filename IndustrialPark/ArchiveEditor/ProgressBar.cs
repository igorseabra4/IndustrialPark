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
    public partial class ProgressBar : Form
    {
        public ProgressBar(string text)
        {
            InitializeComponent();
            Text = text;
        }

        public void SetProgressBar(int min, int max, int step)
        {
            progressBar1.Minimum = min;
            progressBar1.Maximum = max;
            progressBar1.Step = step;
            progressBar1.Value = 0;
        }
        
        public void PerformStep()
        {
            progressBar1.PerformStep();
            labelLoading.Text = $"Asset {progressBar1.Value}/{progressBar1.Maximum}";
            if (progressBar1.Value >= progressBar1.Maximum)
                Close();
        }
    }
}

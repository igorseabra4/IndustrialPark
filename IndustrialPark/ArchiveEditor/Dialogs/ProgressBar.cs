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
        private string Text2;

        public ProgressBar(string text, string text2 = "Asset")
        {
            InitializeComponent();
            Text = text;
            Text2 = text2;
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
            labelLoading.Text = $"{Text2} {progressBar1.Value}/{progressBar1.Maximum}";
            if (progressBar1.Value >= progressBar1.Maximum)
                Close();
        }

        public System.Windows.Forms.ProgressBar GetProgressBar()
        {
            return progressBar1;
        }
    }
}

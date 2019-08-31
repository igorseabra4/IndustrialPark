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
        public string Text2;

        public ProgressBar(string text, string text2 = "Asset")
        {
            InitializeComponent();
            Text = text;
            Text2 = text2;
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
                labelLoading.Text = $"{Text2} {pBar.Value}/{pBar.Maximum}";
                if (pBar.Value >= pBar.Maximum)
                    Close();
            }
        }
    }
}

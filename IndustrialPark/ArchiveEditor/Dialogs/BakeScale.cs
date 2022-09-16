using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class BakeScale : Form
    {
        public BakeScale(List<BakeScaleModelContainer> bsmcs)
        {
            InitializeComponent();
            TopMost = true;

            foreach (var bsmc in bsmcs)
            {
                var rb = new RadioButton()
                {
                    Text = bsmc.ToString(),
                    Tag = bsmc,
                };
                rb.CheckedChanged += (object sender, EventArgs e) =>
                {
                    foreach (RadioButton rbb in flowLayoutPanelRadioButtons.Controls)
                        if (!rbb.Equals(sender))
                            rbb.Checked = false;
                };
                flowLayoutPanelRadioButtons.Controls.Add(rb);
            }
            
            ((RadioButton)flowLayoutPanelRadioButtons.Controls[0]).Checked = true;
        }
                                        
        public bool OK { get; private set; } = false;

        private void button1_Click(object sender, EventArgs e)
        {
            OK = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        public ArchiveEditorFunctions GetSelectedArchive()
        {
            foreach (RadioButton rbb in flowLayoutPanelRadioButtons.Controls)
                if (rbb.Checked)
                    return ((BakeScaleModelContainer)rbb.Tag).archive;
            return null;
        }
    }
}

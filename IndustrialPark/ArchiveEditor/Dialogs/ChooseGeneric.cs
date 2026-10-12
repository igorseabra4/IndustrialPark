using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ChooseGeneric<T> : Form where T : Enum
    {
        public static (T selected, bool ok) Get() => Get(Enum.GetValues(typeof(T)).Cast<T>());

        public static (T selected, bool ok) Get(IEnumerable<T> options)
        {
            using var dialog = new ChooseGeneric<T>(options);
            return dialog.ShowDialog() == DialogResult.OK ? ((T)dialog.Selection, true) : (default, false);
        }

        private object Selection => comboBoxItems.SelectedItem;

        private ChooseGeneric(IEnumerable<T> options)
        {
            if (options == null || !options.Any())
                throw new ArgumentException("Must have options to select from.");

            InitializeComponent();

            TopMost = true;
            comboBoxItems.Items.Clear();
            foreach (T t in options)
                comboBoxItems.Items.Add(t);
            comboBoxItems.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

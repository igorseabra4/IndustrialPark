using System.Windows.Forms;

namespace IndustrialPark
{
    public static class MessageBox
    {
        public static DialogResult Show(string text)
        {
            return System.Windows.Forms.MessageBox.Show(text);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return System.Windows.Forms.MessageBox.Show(text, caption, buttons);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return System.Windows.Forms.MessageBox.Show(text, caption, buttons, icon);
        }
    }
}

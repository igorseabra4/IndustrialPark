using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class ScrollableMessageBox : Form
    {
        public ScrollableMessageBox(string title, string text, bool preventClosing = false)
        {
            InitializeComponent();

            Text = title;
            richTextBox1.Text = text;
            this.preventClosing = preventClosing;
        }

        private readonly bool preventClosing;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            if (preventClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}

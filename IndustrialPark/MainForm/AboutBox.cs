using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace IndustrialPark
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.labelProductName.Text = AssemblyProduct;
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutBox));
            this.textBoxDescription.Text = resources.GetString("textBoxDescription.Text");
            this.labelVersion.Text = new IPversion().version;
            TopMost = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
                return;
            if (e.CloseReason == CloseReason.FormOwnerClosing)
                return;

            e.Cancel = true;
            Hide();
        }

        #region Acessório de Atributos do Assembly

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/igorseabra4/IndustrialPark");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/9eAE6UB");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(WikiLink);
        }

        public static string WikiLink => "https://heavyironmodding.org/wiki/";

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            SetRandomImage();
        }

        private void logoPictureBox_Click(object sender, EventArgs e)
        {
            SetRandomImage();
        }

        private int num = -1;
        private readonly Random random = new Random();

        private void SetRandomImage()
        {
            var images = (from fileName in Directory.GetFiles(Application.StartupPath + "/Resources/")
                          where Path.GetFileName(fileName).StartsWith("AboutImage")
                          select fileName).ToArray();

            int newNum;

            do
                newNum = random.Next(0, images.Length);
            while (newNum == num);

            num = newNum;

            logoPictureBox.Image = System.Drawing.Image.FromFile(images[num]);
        }
    }
}

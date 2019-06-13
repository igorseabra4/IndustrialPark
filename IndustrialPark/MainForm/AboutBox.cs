using System;
using System.ComponentModel;
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
            this.labelVersion.Text = new IPversion().versionName.Split('.')[0];
            TopMost = true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (e.CloseReason == CloseReason.FormOwnerClosing) return;

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

        public static string WikiLink => "https://battlepedia.org/";

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

        private void SetRandomImage()
        {
            switch (new Random((int)DateTime.UtcNow.ToBinary()).Next(0, 6))
            {
                case 1:
                    logoPictureBox.Image = System.Drawing.Image.FromFile(Application.StartupPath + "/Resources/AboutImage1.png");
                    break;
                case 2:
                    logoPictureBox.Image = System.Drawing.Image.FromFile(Application.StartupPath + "/Resources/AboutImage2.png");
                    break;
                case 3:
                    logoPictureBox.Image = System.Drawing.Image.FromFile(Application.StartupPath + "/Resources/AboutImage3.png");
                    break;
                default:
                    logoPictureBox.Image = System.Drawing.Image.FromFile(Application.StartupPath + "/Resources/AboutImage0.png");
                    break;
            }
        }
    }
}

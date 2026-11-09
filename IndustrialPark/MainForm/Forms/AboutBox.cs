using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace IndustrialPark
{
    public partial class AboutBox : Form
    {
        private static Color DarkModeLinkColor => Color.FromArgb(100, 180, 255);

        public const string WikiLink = "https://www.heavyironmodding.org/wiki/";
        private const string DiscordLink = "https://discord.gg/9eAE6UB";
        private const string SourceLink = "https://github.com/igorseabra4/IndustrialPark";
        private const string IssuesLink = "https://github.com/igorseabra4/IndustrialPark/issues";

        public AboutBox()
        {
            InitializeComponent();

            welcomeLayout.Controls.Add(CreateLinkLabel("Wiki - Use Guide and Modding Information", WikiLink), 0, 3);
            welcomeLayout.Controls.Add(CreateLinkLabel("Discord Server", DiscordLink), 0, 4);
            welcomeLayout.Controls.Add(CreateLinkLabel("Source on GitHub", SourceLink), 0, 5);
            welcomeLayout.Controls.Add(CreateLinkLabel("Report Issues and Feature Requests", IssuesLink), 0, 6);

            labelProductName.Text = $"Industrial Park {new IPversion().version}";
        }

        public bool ShowOnStartup => checkShowOnStartup.Checked;

        internal void SetCheckForUpdatesOnStartup(bool value)
        {
            if (checkForUpdatesOnStartup.Checked != value)
                checkForUpdatesOnStartup.Checked = value;
        }

        public void ApplySettings(IPSettings settings)
        {
            checkShowOnStartup.Checked = settings.ShowAboutOnStartup;
            checkForUpdatesOnStartup.Checked = settings.CheckForUpdatesOnStartup;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.WindowsShutDown && e.CloseReason != CloseReason.FormOwnerClosing)
            {
                e.Cancel = true;
                Hide();
            }

            base.OnFormClosing(e);
        }

        private LinkLabel CreateLinkLabel(string text, string url)
        {
            var result = new LinkLabel
            {
                Text = text,
                Tag = url,
                AutoSize = true
            };
            if (Program.IsDarkMode)
                result.LinkColor = DarkModeLinkColor;
            result.LinkClicked += LinkLabel_LinkClicked;
            return result;
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = ((LinkLabel)sender).Tag as string;
            if (!string.IsNullOrWhiteSpace(url))
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void checkForUpdatesOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            Program.MainForm.SetCheckForUpdatesOnStartup(checkForUpdatesOnStartup.Checked);
        }

        private void buttonViewControls_Click(object sender, EventArgs e)
        {
            Program.MainForm.HandleAction(KeyboardAction.ViewControls);
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;

namespace IndustrialPark.Updater;

public static class Program
{
    [STAThread]
    static int Main(string[] args) // returns 0 for updated, 1 for already up to date, 2 for user declined, 3 for error
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        string appDir = Application.StartupPath;
        string industrialPark = Path.Combine(appDir, "IndustrialPark.exe");

        if (!File.Exists(industrialPark))
        {
            MessageBox.Show("IndustrialPark.exe not found.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 3;
        }

        try
        {
            var ipVersionInfo = FileVersionInfo.GetVersionInfo(industrialPark);
            var service = new UpdateService(new GitHubReleaseProvider());
            ReleaseInfo newRelease = service.GetAvailableRelease(ipVersionInfo.ProductVersion);

            if (newRelease == null)
                return 1;

            if (!ConfirmUpdate(newRelease))
            {
                if (args.Length == 0)
                    LaunchApplication(industrialPark);

                return 2;
            }

            service.InstallUpdate(newRelease, appDir);

            var appToLaunch = (args.Length > 0 && File.Exists(args[0])) ?
                Path.GetFullPath(args[0]) : industrialPark;

            LaunchApplication(appToLaunch);

            return 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("There was an error updating the application: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 3;
        }
    }

    private static void WaitForApplicationExit(string targetExe)
    {
        foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(targetExe)))
        {
            using (process)
                process.WaitForExit();
        }
    }

    private static bool ConfirmUpdate(ReleaseInfo newRelease)
    {
        string messageText = $"There is an update available: Industrial Park ({newRelease.Name}).\n\n{TrimBody(newRelease.Body)}\n\nDo you wish to download it?";
        return MessageBox.Show(messageText, "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes;
    }

    private static void LaunchApplication(string targetExe)
    {
        Process.Start(new ProcessStartInfo(targetExe) { UseShellExecute = true });
    }

    private static string TrimBody(string body)
    {
        const int MaxLines = 20;
        body ??= string.Empty;

        var lines = body.Split('\n');
        if (lines.Length > MaxLines)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < MaxLines; i++)
            {
                if (i > 0)
                    sb.Append('\n');
                sb.Append(lines[i].TrimEnd('\r'));
            }
            sb.Append("\n... (truncated)");
            body = sb.ToString();
        }

        return body;
    }
}
